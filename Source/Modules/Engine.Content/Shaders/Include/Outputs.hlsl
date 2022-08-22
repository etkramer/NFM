struct VertAttribute
{
	float4 Position : SV_POSITION;

	uint InstanceID : INSTANCEID;
	uint MeshletID : MESHLETID;
};

struct PrimAttribute
{
	uint PrimitiveID : PRIMITIVEID;
};