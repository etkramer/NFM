using System;
using System.Diagnostics.CodeAnalysis;
using Engine.Common;

namespace Engine.Mathematics
{
	public struct Box3D
	{
		/// <summary>
		/// The box's left-near-bottom (-X/-Y/-Z) corner
		/// </summary>
		public Vector3 Min;

		/// <summary>
		/// The box's right-far-top (+X/+Y/+Z) corner
		/// </summary>
		public Vector3 Max;

		/// <summary>
		/// The point at the center of the box
		/// </summary>
		public Vector3 Center => new Vector3((Min.X + Max.X) / 2f, (Min.Y + Max.Y) / 2f, (Min.Z + Max.Z) / 2f);

		public Vector3 BottomLeftNear => new Vector3(Min.X, Min.Y, Min.Z);
		public Vector3 BottomLeftFar => new Vector3(Min.X, Max.Y, Min.Z);
		public Vector3 BottomRightNear => new Vector3(Max.X, Min.Y, Min.Z);
		public Vector3 BottomRightFar => new Vector3(Max.X, Max.Y, Min.Z);
		public Vector3 TopLeftNear => new Vector3(Min.X, Min.Y, Max.Z);
		public Vector3 TopLeftFar => new Vector3(Min.X, Max.Y, Max.Z);
		public Vector3 TopRightNear => new Vector3(Max.X, Min.Y, Max.Z);
		public Vector3 TopRightFar => new Vector3(Max.X, Max.Y, Max.Z);

		public float Width => Max.X - Min.X;
		public float Depth => Max.Y - Min.Y;
		public float Height => Max.Z - Min.Z;

		public Box3D(Vector3 min, Vector3 max)
		{
			Min = min;
			Max = max;
		}

		public override string ToString()
		{
			return $"{Min}, {Max}";
		}

		public static Box3D operator+(Box3D left, Box3D right)
		{
			return new Box3D(Vector3.ComponentMin(left.Min, right.Min), Vector3.ComponentMax(left.Max, right.Max));
		}

		public static bool operator==(Box3D left, Box3D right)
		{
			return left.Equals(right);
		}

		public static bool operator!=(Box3D left, Box3D right)
		{
			return !(left == right);
		}

		public override bool Equals(object obj)
		{
			return obj is Box3D && Equals((Box3D)obj);
		}

		public bool Equals(Box3D other)
		{
			return Min.Equals(other.Min) && Max.Equals(other.Max);
		}

        public override int GetHashCode()
        {
            return HashCode.Combine(Min, Max);
        }

		public static Box3D Zero => new Box3D(Vector3.Zero, Vector3.Zero);
		public static Box3D Infinity => new Box3D(Vector3.NegativeInfinity, Vector3.PositiveInfinity);
	}
}
