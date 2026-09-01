using System;
using System.Runtime.InteropServices;

namespace NFM.Mathematics
{
	/// <summary>
	/// A world-space ray, used for cursor-driven hit testing and dragging.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct Ray
	{
		public Vector3 Origin;
		public Vector3 Direction;

		public Ray(Vector3 origin, Vector3 direction)
		{
			Origin = origin;
			Direction = direction.Normalized();
		}

		public readonly Vector3 At(float distance) => Origin + (Direction * distance);

		/// <summary>
		/// Intersects the plane through <paramref name="point"/> with the given normal.
		/// Fails when the ray is parallel to the plane, or when the hit is behind the origin.
		/// </summary>
		public readonly bool IntersectPlane(Vector3 point, Vector3 normal, out Vector3 hit)
		{
			float denominator = Vector3.Dot(Direction, normal);
			if (MathF.Abs(denominator) < 1e-6f)
			{
				hit = default;
				return false;
			}

			float distance = Vector3.Dot(point - Origin, normal) / denominator;
			hit = At(distance);

			return distance > 0;
		}

		/// <summary>
		/// Finds the point on the infinite line through <paramref name="point"/> closest to this ray.
		/// Fails when the two are near-parallel, where the closest point is unstable.
		/// </summary>
		public readonly bool ClosestPointOnLine(Vector3 point, Vector3 direction, out Vector3 closest)
		{
			direction = direction.Normalized();

			Vector3 offset = point - Origin;
			float dirDot = Vector3.Dot(direction, Direction);
			float denominator = 1 - (dirDot * dirDot);

			if (MathF.Abs(denominator) < 1e-5f)
			{
				closest = point;
				return false;
			}

			float alongLine = ((dirDot * Vector3.Dot(offset, Direction)) - Vector3.Dot(offset, direction)) / denominator;
			closest = point + (direction * alongLine);

			return true;
		}
	}
}
