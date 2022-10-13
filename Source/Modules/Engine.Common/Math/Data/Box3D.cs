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

		public Vector3 LeftNearBottom => new Vector3(Min.X, Min.Y, Min.Z);
		public Vector3 LeftFarBottom => new Vector3(Min.X, Max.Y, Min.Z);
		public Vector3 RightNearBottom => new Vector3(Max.X, Min.Y, Min.Z);
		public Vector3 RightFarBottom => new Vector3(Max.X, Max.Y, Min.Z);
		public Vector3 LeftNearTop => new Vector3(Min.X, Min.Y, Max.Z);
		public Vector3 LeftFarTop => new Vector3(Min.X, Max.Y, Max.Z);
		public Vector3 RightNearTop => new Vector3(Max.X, Min.Y, Max.Z);
		public Vector3 RightFarTop => new Vector3(Max.X, Max.Y, Max.Z);

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

		public static Box3D operator+(Box3D left, Vector3 right)
		{
			return new Box3D(left.Min + right, left.Max + right);
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
