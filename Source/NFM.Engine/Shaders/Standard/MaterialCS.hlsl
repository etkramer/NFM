#include "Shaders/Common.h"
#include "Shaders/World.h"

RWTexture2D<float4> MatBuffer0 : register(u0);
RWTexture2D<half4> MatBuffer1 : register(u1);
RWTexture2D<float4> MatBuffer2 : register(u2);
RWTexture2D<float4> MatBuffer3 : register(u3);

Texture2D<uint2> VisBuffer : register(t0);
Texture2D<float> DepthBuffer : register(t1);

// Pixels bucketed by shader stack, produced by the binning passes.
StructuredBuffer<uint> BinPixels : register(t2);
StructuredBuffer<uint> BinCounts : register(t3);
StructuredBuffer<uint> BinOffsets : register(t4);

ByteAddressBuffer MaterialParams : register(t0, space2);
int ShaderID : register(b0);

uint3 FetchTriangleIndices(Mesh mesh, uint triangleID)
{
	triangleID = mesh.IndexOffset + (triangleID * 3);
	return uint3(Indices[triangleID], Indices[triangleID + 1], Indices[triangleID + 2]);
}

Vertex FetchVertexData(Mesh mesh, uint index)
{
	return Vertices[mesh.VertexOffset + index];
}

struct BarycentricDeriv
{
	float3 m_lambda;
	float3 m_ddx;
	float3 m_ddy;
};

BarycentricDeriv CalcFullBary(float4 pt0, float4 pt1, float4 pt2, float2 pixelNdc, float2 winSize)
{
	BarycentricDeriv ret = (BarycentricDeriv)0;

	// Clamped away from zero - w reaches it on near-plane triangles.
	float3 w = float3(pt0.w, pt1.w, pt2.w);
	float3 invW = rcp(select(abs(w) < 1e-6f, 1e-6f, w));

	float2 ndc0 = pt0.xy * invW.x;
	float2 ndc1 = pt1.xy * invW.y;
	float2 ndc2 = pt2.xy * invW.z;

	// Clamped away from zero - degenerate triangles have no area in NDC.
	float det = determinant(float2x2(ndc2 - ndc1, ndc0 - ndc1));
	float invDet = rcp(select(abs(det) < 1e-12f, 1e-12f, det));
	ret.m_ddx = float3(ndc1.y - ndc2.y, ndc2.y - ndc0.y, ndc0.y - ndc1.y) * invDet * invW;
	ret.m_ddy = float3(ndc2.x - ndc1.x, ndc0.x - ndc2.x, ndc1.x - ndc0.x) * invDet * invW;
	float ddxSum = dot(ret.m_ddx, float3(1, 1, 1));
	float ddySum = dot(ret.m_ddy, float3(1, 1, 1));

	float2 deltaVec = pixelNdc - ndc0;
	float interpInvW = invW.x + deltaVec.x * ddxSum + deltaVec.y * ddySum;
	float interpW = rcp(interpInvW);

	ret.m_lambda.x = interpW * (invW[0] + deltaVec.x * ret.m_ddx.x + deltaVec.y * ret.m_ddy.x);
	ret.m_lambda.y = interpW * (0.0f + deltaVec.x * ret.m_ddx.y + deltaVec.y * ret.m_ddy.y);
	ret.m_lambda.z = interpW * (0.0f + deltaVec.x * ret.m_ddx.z + deltaVec.y * ret.m_ddy.z);

	ret.m_ddx *= (2.0f / winSize.x);
	ret.m_ddy *= (2.0f / winSize.y);
	ddxSum *= (2.0f / winSize.x);
	ddySum *= (2.0f / winSize.y);

	ret.m_ddy *= -1.0f;
	ddySum *= -1.0f;

	float interpW_ddx = 1.0f / (interpInvW + ddxSum);
	float interpW_ddy = 1.0f / (interpInvW + ddySum);

	ret.m_ddx = interpW_ddx * (ret.m_lambda * interpInvW + ret.m_ddx) - ret.m_lambda;
	ret.m_ddy = interpW_ddy * (ret.m_lambda * interpInvW + ret.m_ddy) - ret.m_lambda;

	return ret;
}

float BarycentricLerp(in float v0, in float v1, in float v2, in float3 barycentrics)
{
	return v0 * barycentrics.x + v1 * barycentrics.y + v2 * barycentrics.z;
}

float2 BarycentricLerp(in float2 v0, in float2 v1, in float2 v2, in float3 barycentrics)
{
	return v0 * barycentrics.x + v1 * barycentrics.y + v2 * barycentrics.z;
}

float3 BarycentricLerp(in float3 v0, in float3 v1, in float3 v2, in float3 barycentrics)
{
	return v0 * barycentrics.x + v1 * barycentrics.y + v2 * barycentrics.z;
}

float4 BarycentricLerp(in float4 v0, in float4 v1, in float4 v2, in float3 barycentrics)
{
	return v0 * barycentrics.x + v1 * barycentrics.y + v2 * barycentrics.z;
}

float3 InterpolateWithDeriv(BarycentricDeriv deriv, float v0, float v1, float v2)
{
	float3 mergedV = float3(v0, v1, v2);
	float3 ret;
	ret.x = dot(mergedV, deriv.m_lambda);
	ret.y = dot(mergedV, deriv.m_ddx);
	ret.z = dot(mergedV, deriv.m_ddy);
	return ret;
}

struct SurfaceModel
{
	// Geoemtry (tangent space)
	float3 Normal;

	// PBR
	float3 Albedo;
	float Metallic;
	float Roughness;
	float Specular;
	float3 Emissive;

	// Non-opaque
	float Opacity;
};

SurfaceModel EvalSurface(uint materialID, float2 uv0, float2 ddx, float2 ddy);

[numthreads(64, 1, 1)]
void main(uint threadID : SV_DispatchThreadID)
{
	// This dispatch only covers pixels the binning pass assigned to this shader.
	if (threadID >= BinCounts[ShaderID])
	{
		return;
	}

	uint packed = BinPixels[BinOffsets[ShaderID] + threadID];
	uint2 id = uint2(packed & 0xFFFF, packed >> 16);

	// Grab the frame width/height
	int2 frameSize;
	DepthBuffer.GetDimensions(frameSize.x, frameSize.y);

	// Unpack visbuffer
	uint instanceID = VisBuffer[id].x;
	uint triangleID = VisBuffer[id].y;

	// Fetch instance data
	Instance instance = Instances[instanceID];
	Mesh mesh = Meshes[instance.MeshID];
	Transform transform = Transforms[instance.TransformID];

	// Fetch vertex data for triangle
	uint3 triangleIndices = FetchTriangleIndices(mesh, triangleID);
	Vertex v0 = FetchVertexData(mesh, triangleIndices[0]);
	Vertex v1 = FetchVertexData(mesh, triangleIndices[1]);
	Vertex v2 = FetchVertexData(mesh, triangleIndices[2]);

	// Transform vertices to clip space
	float4x4 mvp = mul(ViewConstants.ViewToClip, mul(ViewConstants.WorldToView, transform.ObjectToWorld));
	float4 pt0 = mul(mvp, float4(v0.Position, 1));
	float4 pt1 = mul(mvp, float4(v1.Position, 1));
	float4 pt2 = mul(mvp, float4(v2.Position, 1));

	// Calculate pixel location in NDC
	float2 pixelNDC = (float2(id) / float2(frameSize)) * 2.f - 1.f;
	pixelNDC.y *= -1;

	// Calculate barycentric derivs
	const BarycentricDeriv deriv = CalcFullBary(pt0, pt1, pt2, pixelNDC, frameSize);

	// TODO: Understand why this needs to be done
	float3 norm0 = normalize(mul(v0.Normal, (float3x3)transform.WorldToObject));
	float3 norm1 = normalize(mul(v1.Normal, (float3x3)transform.WorldToObject));
	float3 norm2 = normalize(mul(v2.Normal, (float3x3)transform.WorldToObject));

	float3 tan0 = normalize(mul((float3x3)transform.ObjectToWorld, v0.Tangent.xyz));
	float3 tan1 = normalize(mul((float3x3)transform.ObjectToWorld, v1.Tangent.xyz));
	float3 tan2 = normalize(mul((float3x3)transform.ObjectToWorld, v2.Tangent.xyz));

	// Interp vertex data
	float3 normal = normalize(BarycentricLerp(norm0, norm1, norm2, deriv.m_lambda));
	float3 tangent = BarycentricLerp(tan0, tan1, tan2, deriv.m_lambda);
	float2 uv0 = BarycentricLerp(v0.UV0, v1.UV0, v2.UV0, deriv.m_lambda);

	// Build an orthonormal tangent frame, handedness from the vertex data
	tangent = normalize(tangent - normal * dot(normal, tangent));
	float3 bitangent = cross(normal, tangent) * v0.Tangent.w;
	float3x3 tangentToWorld = float3x3(tangent, bitangent, normal);
	
	// Calculate UV derivs
	float2 ddx = InterpolateWithDeriv(deriv, v0.UV0.x, v1.UV0.x, v2.UV0.x).yz;
	float2 ddy = InterpolateWithDeriv(deriv, v0.UV0.y, v1.UV0.y, v2.UV0.y).yz;
	
	// Evaluate surface
	SurfaceModel surface = EvalSurface(instance.MaterialID, uv0, ddx, ddy);
	
	// Write to g-buffer
	MatBuffer0[id] = float4(surface.Albedo, 1);
	MatBuffer1[id] = float4(normalize(mul(surface.Normal, tangentToWorld)), 1);
	MatBuffer2[id] = float4(surface.Metallic, surface.Specular, surface.Roughness, 1);
	MatBuffer3[id] = float4(surface.Emissive, 1);
}