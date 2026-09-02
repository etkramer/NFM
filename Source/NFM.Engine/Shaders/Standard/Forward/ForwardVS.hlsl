#include "Shaders/Standard/Forward/Forward.h"

ForwardVertex main(uint vertexID : SV_VertexID)
{
	// Grab instance data.
	Instance instance = Instances[InstanceID];

	Vertex vertex = Vertices[instance.VertexOffset + vertexID];
	Transform transform = Transforms[instance.TransformID];

	float3 worldPos = mul(transform.ObjectToWorld, float4(vertex.Position, 1)).xyz;

	ForwardVertex output;
	output.Position = mul(ViewConstants.ViewToClip, mul(ViewConstants.WorldToView, float4(worldPos, 1)));
	output.WorldPos = worldPos;

	// The same basis the visbuffer's material pass builds.
	output.Normal = mul(vertex.Normal, (float3x3)transform.WorldToObject);
	output.Tangent = float4(mul((float3x3)transform.ObjectToWorld, vertex.Tangent.xyz), vertex.Tangent.w);

	output.UV0 = vertex.UV0;
	return output;
}
