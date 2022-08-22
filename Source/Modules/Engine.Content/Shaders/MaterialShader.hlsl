#include "Shaders/Include/Geometry.hlsl"
#include "Shaders/Include/Common.hlsl"

cbuffer viewConstants : register(b1)
{
	float4x4 View;
	float4x4 Projection;
}

Texture2D<uint2> VisBuffer : register(t0);
Texture2D<float> DepthBuffer : register(t1);
RWTexture2D<float4> RenderTarget : register(u0);

float3 IndexToColor(uint i)
{
	if (i % 6 == 0)
	{
		return float3(0.82, 0.8, 0.57);
	}
	else if (i % 6 == 1)
	{
		return float3(0.58, 0.37, 0.87);
	}
	else if (i % 6 == 2)
	{
		return float3(0.88, 0.07, 0.6);
	}
	else if (i % 6 == 3)
	{
		return float3(0.89, 0.89, 0.14);
	}
	else if (i % 6 == 4)
	{
		return float3(0.58, 0.86, 0.89);
	}
	else
	{
		return float3(0, 0.47, 0.84);
	}
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

uint3 GetPrimitive(Mesh mesh, Meshlet meshlet, uint prim)
{
	uint prim0 = mesh.PrimStart + meshlet.PrimStart + (prim * 3);
	uint prim1 = prim0 + 1;
	uint prim2 = prim0 + 2;
	return uint3(Primitives[prim0], Primitives[prim1], Primitives[prim2]);
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

[numthreads(8, 8, 1)]
void ComputeEntry(uint3 ID : SV_DispatchThreadID)
{
	// Grab the frame width/height.
	int2 frameSize;
	RenderTarget.GetDimensions(frameSize.x, frameSize.y);

	// Make sure we don't try to read outside the bounds of the RT.
	if (ID.x >= frameSize.x || ID.y >= frameSize.y)
	{
		return;
	}

	// Early out if this pixel is empty.
	if (DepthBuffer[ID.xy] == 0)
	{
		return;
	}

	// Unpack data from visbuffer.
	uint2 unpacked = BitUnpack(VisBuffer[ID.xy][1], 25);
	uint instanceID = VisBuffer[ID.xy][0];
	uint meshletID = unpacked[0];
	uint primID = unpacked[1];

	const Instance instance = Instances[instanceID];
	const Mesh mesh = Meshes[instance.Mesh];
	const Meshlet meshlet = Meshlets[mesh.MeshletStart + meshletID];

	// Read the triangle's vertex data.
	uint3 prim = GetPrimitive(mesh, meshlet, primID);
	Vertex vert0 = Vertices[mesh.VertStart + meshlet.VertStart + prim[0]];
	Vertex vert1 = Vertices[mesh.VertStart + meshlet.VertStart + prim[1]];
	Vertex vert2 = Vertices[mesh.VertStart + meshlet.VertStart + prim[2]];

	// Transform vertices to clip space.
	float4x4 mvp = mul(Projection, mul(View, instance.Transform));
	const float4 pt0 = mul(mvp, float4(vert0.Position, 1));
	const float4 pt1 = mul(mvp, float4(vert1.Position, 1));
	const float4 pt2 = mul(mvp, float4(vert2.Position, 1));

	// Calculate pixel location in NDC.
	float2 pixelNDC = (float2(ID.xy) / float2(frameSize)) * 2.f - 1.f;
	pixelNDC.y *= -1;

	// Calculate barycentric.
	const BarycentricDeriv deriv = CalcFullBary(pt0, pt1, pt2, pixelNDC, frameSize);

	// Interp normals.
	float x = Interp(deriv, vert0.Normal.x, vert1.Normal.x, vert2.Normal.x)[0];
	float y = Interp(deriv, vert0.Normal.y, vert1.Normal.y, vert2.Normal.y)[0];
	float z = Interp(deriv, vert0.Normal.z, vert1.Normal.z, vert2.Normal.z)[0];
	float3 normal = float3(x, y, z);

	// Write it to the render target.
	RenderTarget[ID.xy] = float4(normal, 1);
}