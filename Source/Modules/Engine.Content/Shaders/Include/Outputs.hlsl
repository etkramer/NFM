struct MeshOut
{
	float4 Position : SV_POSITION;

	uint InstanceID : INSTANCEID;
	uint MeshletID : MESHLETID;
	uint TriangleID : TRIANGLEID;
};