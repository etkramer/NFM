#include "World.h"

// Continues the global scene bindings, kept out of World.h so raster stages never declare it.
RaytracingAccelerationStructure Scene : register(t8, space1);

// Traces an occlusion ray, stopping at the first hit it finds.
bool TraceVisibility(float3 origin, float3 direction, float tMax)
{
	RayDesc ray;
	ray.Origin = origin;
	ray.Direction = direction;
	ray.TMin = 0;
	ray.TMax = tMax;

	RayQuery<RAY_FLAG_ACCEPT_FIRST_HIT_AND_END_SEARCH | RAY_FLAG_SKIP_CLOSEST_HIT_SHADER | RAY_FLAG_CULL_NON_OPAQUE> query;
	query.TraceRayInline(Scene, RAY_FLAG_NONE, 0xFF, ray);

	// Every instance is opaque, so traversal finishes without ever handing back a candidate.
	query.Proceed();

	return query.CommittedStatus() == COMMITTED_NOTHING;
}
