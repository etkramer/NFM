#include "Shaders/Common.h"
#include "Shaders/World.h"

RWTexture2D<float4> RT : register(u0);
Texture2D<uint2> VisBuffer : register(t0);
Texture2D<float> DepthBuffer : register(t1);

uint3 FetchTriangleIndices(Mesh mesh, Meshlet meshlet, uint triangleID)
{
	triangleID = mesh.PrimStart + meshlet.PrimStart + (triangleID * 3);
	return uint3(Primitives[triangleID], Primitives[triangleID + 1], Primitives[triangleID + 2]);
}

Vertex FetchVertexData(Mesh mesh, Meshlet meshlet, uint vert)
{
	return Vertices[mesh.VertStart + meshlet.VertStart + vert];
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

	float3 invW = rcp(float3(pt0.w, pt1.w, pt2.w));

	float2 ndc0 = pt0.xy * invW.x;
	float2 ndc1 = pt1.xy * invW.y;
	float2 ndc2 = pt2.xy * invW.z;

	float invDet = rcp(determinant(float2x2(ndc2 - ndc1, ndc0 - ndc1)));
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

float3 Interp(BarycentricDeriv deriv, float v0, float v1, float v2)
{
	float3 mergedV = float3(v0, v1, v2);
	float3 ret;
	ret.x = dot(mergedV, deriv.m_lambda);
	ret.y = dot(mergedV, deriv.m_ddx);
	ret.z = dot(mergedV, deriv.m_ddy);
	return ret;
}

[numthreads(32, 32, 1)]
void MaterialCS(uint2 id : SV_DispatchThreadID)
{
	// Grab the frame width/height
	int2 frameSize;
	RT.GetDimensions(frameSize.x, frameSize.y);

	// Don't try to process out of bounds pixels
	if (id.x >= frameSize.x || id.y >= frameSize.y)
	{
		return;
	}

	// Early out if this pixel is empty
	if (DepthBuffer[id.xy] == 0)
	{
		return;
	}

	// Unpack visbuffer
	uint instanceID = VisBuffer[id.xy].x;
	uint meshletID = UnpackBits(VisBuffer[id.xy].y, 25).x;
	uint triangleID = UnpackBits(VisBuffer[id.xy].y, 25).y;

	// Fetch instance data
	Instance instance = Instances[instanceID];
	Mesh mesh = Meshes[instance.MeshID];
	Meshlet meshlet = Meshlets[mesh.MeshletStart + meshletID];
	Transform transform = Transforms[instance.TransformID];

	// Fetch vertex data for triangle
	uint3 triangleIndices = FetchTriangleIndices(mesh, meshlet, triangleID);
	Vertex v0 = FetchVertexData(mesh, meshlet, triangleIndices[0]);
	Vertex v1 = FetchVertexData(mesh, meshlet, triangleIndices[1]);
	Vertex v2 = FetchVertexData(mesh, meshlet, triangleIndices[2]);

	// Transform vertices to clip space
	float4x4 mvp = mul(ViewConstants.ViewToClip, mul(ViewConstants.WorldToView, transform.ObjectToWorld));
	float4 pt0 = mul(mvp, float4(v0.Position, 1));
	float4 pt1 = mul(mvp, float4(v1.Position, 1));
	float4 pt2 = mul(mvp, float4(v2.Position, 1));

	// Calculate pixel location in NDC
	float2 pixelNDC = (float2(id.xy) / float2(frameSize)) * 2.f - 1.f;
	pixelNDC.y *= -1;

	// Calculate barycentric derivs
	const BarycentricDeriv deriv = CalcFullBary(pt0, pt1, pt2, pixelNDC, frameSize);

	// TODO: Understand why this needs to be done
	float3 norm0 = normalize(mul(v0.Normal, (float3x3)transform.WorldToObject));
	float3 norm1 = normalize(mul(v1.Normal, (float3x3)transform.WorldToObject));
	float3 norm2 = normalize(mul(v2.Normal, (float3x3)transform.WorldToObject));

	// Interp normals
	float3 normal;
	normal.x = Interp(deriv, norm0.x, norm1.x, norm2.x)[0];
	normal.y = Interp(deriv, norm0.y, norm1.y, norm2.y)[0];
	normal.z = Interp(deriv, norm0.z, norm1.z, norm2.z)[0];

	RT[id.xy] = SRGBToLinear(float4(normal / 2 + 0.5, 1));
}