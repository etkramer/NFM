//
//  Vector3i.cs
//
//  Copyright (C) OpenTK
//
//  This software may be modified and distributed under the terms
//  of the MIT license. See the LICENSE file for details.
//

using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace NFM.Mathematics
{
    /// <summary>
    /// Represents a 2D Vector using two 32-bit integer numbers.
    /// </summary>
    /// <remarks>
    /// The Vector2i structure is suitable for interoperation with unmanaged code requiring two consecutive integers.
    /// </remarks>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector2i : IEquatable<Vector2i>
    {
        /// <summary>
        /// The X component of the Vector2i.
        /// </summary>
        public int X;

        /// <summary>
        /// The Y component of the Vector2i.
        /// </summary>
        public int Y;

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector2i"/> struct.
        /// </summary>
        /// <param name="value">The value that will initialize this instance.</param>
        public Vector2i(int value)
        {
            X = value;
            Y = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector2i"/> struct.
        /// </summary>
        /// <param name="x">The X component of the Vector2i.</param>
        /// <param name="y">The Y component of the Vector2i.</param>
        public Vector2i(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Gets or sets the value at the index of the Vector.
        /// </summary>
        /// <param name="index">The index of the component from the Vector.</param>
        /// <exception cref="IndexOutOfRangeException">Thrown if the index is less than 0 or greater than 1.</exception>
        public int this[int index]
        {
            get
            {
                if (index == 0)
                {
                    return X;
                }

                if (index == 1)
                {
                    return Y;
                }

                throw new IndexOutOfRangeException("You tried to access this Vector at index: " + index);
            }

            set
            {
                if (index == 0)
                {
                    X = value;
                }
                else if (index == 1)
                {
                    Y = value;
                }
                else
                {
                    throw new IndexOutOfRangeException("You tried to set this Vector at index: " + index);
                }
            }
        }

        /// <summary>
        /// Gets the manhattan length of the Vector.
        /// </summary>
        public int ManhattanLength => System.Math.Abs(X) + System.Math.Abs(Y);

		/// <summary>
		/// Gets the euclidian length of the Vector.
		/// </summary>
		public float EuclideanLength => MathF.Sqrt((X * X) + (Y * Y));

        /// <summary>
        /// Gets the perpendicular Vector on the right side of this Vector.
        /// </summary>
        public Vector2i PerpendicularRight => new Vector2i(Y, -X);

        /// <summary>
        /// Gets the perpendicular Vector on the left side of this Vector.
        /// </summary>
        public Vector2i PerpendicularLeft => new Vector2i(-Y, X);

        /// <summary>
        /// Defines a unit-length <see cref="Vector2i"/> that points towards the X-axis.
        /// </summary>
        public static readonly Vector2i UnitX = new Vector2i(1, 0);

        /// <summary>
        /// Defines a unit-length <see cref="Vector2i"/> that points towards the Y-axis.
        /// </summary>
        public static readonly Vector2i UnitY = new Vector2i(0, 1);

        /// <summary>
        /// Defines a zero-length <see cref="Vector2i"/>.
        /// </summary>
        public static readonly Vector2i Zero = new Vector2i(0, 0);

		/// <summary>
		/// Defines an instance with all components set to -1.
		/// </summary>
		public static readonly Vector2i Default = new Vector2i(-1, -1);

		/// <summary>
		/// Defines an instance with all components set to 1.
		/// </summary>
		public static readonly Vector2i One = new Vector2i(1, 1);

        /// <summary>
        /// Defines the size of the <see cref="Vector2i"/> struct in bytes.
        /// </summary>
        public static readonly int SizeInBytes = Unsafe.SizeOf<Vector2i>();

        /// <summary>
        /// Adds two Vectors.
        /// </summary>
        /// <param name="a">Left operand.</param>
        /// <param name="b">Right operand.</param>
        /// <returns>Result of operation.</returns>
        [Pure]
        public static Vector2i Add(Vector2i a, Vector2i b)
        {
            Add(in a, in b, out a);
            return a;
        }

        /// <summary>
        /// Adds two Vectors.
        /// </summary>
        /// <param name="a">Left operand.</param>
        /// <param name="b">Right operand.</param>
        /// <param name="result">Result of operation.</param>
        public static void Add(in Vector2i a, in Vector2i b, out Vector2i result)
        {
            result.X = a.X + b.X;
            result.Y = a.Y + b.Y;
        }

        /// <summary>
        /// Subtract one Vector from another.
        /// </summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <returns>Result of subtraction.</returns>
        [Pure]
        public static Vector2i Subtract(Vector2i a, Vector2i b)
        {
            Subtract(in a, in b, out a);
            return a;
        }

        /// <summary>
        /// Subtract one Vector from another.
        /// </summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Result of subtraction.</param>
        public static void Subtract(in Vector2i a, in Vector2i b, out Vector2i result)
        {
            result.X = a.X - b.X;
            result.Y = a.Y - b.Y;
        }

        /// <summary>
        /// Multiplies a Vector by an integer scalar.
        /// </summary>
        /// <param name="Vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <returns>Result of the operation.</returns>
        [Pure]
        public static Vector2i Multiply(Vector2i Vector, int scale)
        {
            Multiply(in Vector, scale, out Vector);
            return Vector;
        }

        /// <summary>
        /// Multiplies a Vector by an integer scalar.
        /// </summary>
        /// <param name="Vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <param name="result">Result of the operation.</param>
        public static void Multiply(in Vector2i Vector, int scale, out Vector2i result)
        {
            result.X = Vector.X * scale;
            result.Y = Vector.Y * scale;
        }

        /// <summary>
        /// Multiplies a Vector by the components a Vector (scale).
        /// </summary>
        /// <param name="Vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <returns>Result of the operation.</returns>
        [Pure]
        public static Vector2i Multiply(Vector2i Vector, Vector2i scale)
        {
            Multiply(in Vector, in scale, out Vector);
            return Vector;
        }

        /// <summary>
        /// Multiplies a Vector by the components of a Vector (scale).
        /// </summary>
        /// <param name="Vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <param name="result">Result of the operation.</param>
        public static void Multiply(in Vector2i Vector, in Vector2i scale, out Vector2i result)
        {
            result.X = Vector.X * scale.X;
            result.Y = Vector.Y * scale.Y;
        }

        /// <summary>
        /// Divides a Vector by a scalar using integer division, floor(a/b).
        /// </summary>
        /// <param name="Vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <returns>Result of the operation.</returns>
        [Pure]
        public static Vector2i Divide(Vector2i Vector, int scale)
        {
            Divide(in Vector, scale, out Vector);
            return Vector;
        }

        /// <summary>
        /// Divides a Vector by a scalar using integer division, floor(a/b).
        /// </summary>
        /// <param name="Vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <param name="result">Result of the operation.</param>
        public static void Divide(in Vector2i Vector, int scale, out Vector2i result)
        {
            result.X = Vector.X / scale;
            result.Y = Vector.Y / scale;
        }

        /// <summary>
        /// Divides a Vector by the components of a Vector using integer division, floor(a/b).
        /// </summary>
        /// <param name="Vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <returns>Result of the operation.</returns>
        [Pure]
        public static Vector2i Divide(Vector2i Vector, Vector2i scale)
        {
            Divide(in Vector, in scale, out Vector);
            return Vector;
        }

        /// <summary>
        /// Divides a Vector by the components of a Vector using integer division, floor(a/b).
        /// </summary>
        /// <param name="Vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <param name="result">Result of the operation.</param>
        public static void Divide(in Vector2i Vector, in Vector2i scale, out Vector2i result)
        {
            result.X = Vector.X / scale.X;
            result.Y = Vector.Y / scale.Y;
        }

        /// <summary>
        /// Returns a Vector created from the smallest of the corresponding components of the given Vectors.
        /// </summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <returns>The component-wise minimum.</returns>
        [Pure]
        public static Vector2i ComponentMin(Vector2i a, Vector2i b)
        {
            a.X = System.Math.Min(a.X, b.X);
            a.Y = System.Math.Min(a.Y, b.Y);
            return a;
        }

        /// <summary>
        /// Returns a Vector created from the smallest of the corresponding components of the given Vectors.
        /// </summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">The component-wise minimum.</param>
        public static void ComponentMin(in Vector2i a, in Vector2i b, out Vector2i result)
        {
            result.X = System.Math.Min(a.X, b.X);
            result.Y = System.Math.Min(a.Y, b.Y);
        }

        /// <summary>
        /// Returns a Vector created from the largest of the corresponding components of the given Vectors.
        /// </summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <returns>The component-wise maximum.</returns>
        [Pure]
        public static Vector2i ComponentMax(Vector2i a, Vector2i b)
        {
            a.X = System.Math.Max(a.X, b.X);
            a.Y = System.Math.Max(a.Y, b.Y);
            return a;
        }

        /// <summary>
        /// Returns a Vector created from the largest of the corresponding components of the given Vectors.
        /// </summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">The component-wise maximum.</param>
        public static void ComponentMax(in Vector2i a, in Vector2i b, out Vector2i result)
        {
            result.X = System.Math.Max(a.X, b.X);
            result.Y = System.Math.Max(a.Y, b.Y);
        }

        /// <summary>
        /// Clamp a Vector to the given minimum and maximum Vectors.
        /// </summary>
        /// <param name="vec">Input Vector.</param>
        /// <param name="min">Minimum Vector.</param>
        /// <param name="max">Maximum Vector.</param>
        /// <returns>The clamped Vector.</returns>
        [Pure]
        public static Vector2i Clamp(Vector2i vec, Vector2i min, Vector2i max)
        {
            vec.X = MathHelper.Clamp(vec.X, min.X, max.X);
            vec.Y = MathHelper.Clamp(vec.Y, min.Y, max.Y);
            return vec;
        }

        /// <summary>
        /// Clamp a Vector to the given minimum and maximum Vectors.
        /// </summary>
        /// <param name="vec">Input Vector.</param>
        /// <param name="min">Minimum Vector.</param>
        /// <param name="max">Maximum Vector.</param>
        /// <param name="result">The clamped Vector.</param>
        public static void Clamp(in Vector2i vec, in Vector2i min, in Vector2i max, out Vector2i result)
        {
            result.X = MathHelper.Clamp(vec.X, min.X, max.X);
            result.Y = MathHelper.Clamp(vec.Y, min.Y, max.Y);
        }

        /// <summary>
        /// Gets or sets a <see cref="Vector2i"/> with the Y and X components of this instance.
        /// </summary>
        [XmlIgnore]
        public Vector2i Yx
        {
            get => new Vector2i(Y, X);
            set
            {
                Y = value.X;
                X = value.Y;
            }
        }

        /// <summary>
        /// Gets a <see cref="Vector2"/> object with the same component values as the <see cref="Vector2i"/> instance.
        /// </summary>
        /// <returns>The resulting <see cref="Vector3"/> instance.</returns>
        public Vector2 ToVector2()
        {
            return new Vector2(X, Y);
        }

        /// <summary>
        /// Gets a <see cref="Vector2"/> object with the same component values as the <see cref="Vector2i"/> instance.
        /// </summary>
        /// <param name="input">The given <see cref="Vector2i"/> to convert.</param>
        /// <param name="result">The resulting <see cref="Vector2"/>.</param>
        public static void ToVector2(in Vector2i input, out Vector2 result)
        {
            result.X = input.X;
            result.Y = input.Y;
        }

        /// <summary>
        /// Adds the specified instances.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns>Result of addition.</returns>
        [Pure]
        public static Vector2i operator +(Vector2i left, Vector2i right)
        {
            left.X += right.X;
            left.Y += right.Y;
            return left;
        }

        /// <summary>
        /// Subtracts the specified instances.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns>Result of subtraction.</returns>
        [Pure]
        public static Vector2i operator -(Vector2i left, Vector2i right)
        {
            left.X -= right.X;
            left.Y -= right.Y;
            return left;
        }

        /// <summary>
        /// Negates the specified instance.
        /// </summary>
        /// <param name="vec">Operand.</param>
        /// <returns>Result of negation.</returns>
        [Pure]
        public static Vector2i operator -(Vector2i vec)
        {
            vec.X = -vec.X;
            vec.Y = -vec.Y;
            return vec;
        }

        /// <summary>
        /// Multiplies the specified instance by a scalar.
        /// </summary>
        /// <param name="vec">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <returns>Result of multiplication.</returns>
        [Pure]
        public static Vector2i operator *(Vector2i vec, int scale)
        {
            vec.X *= scale;
            vec.Y *= scale;
            return vec;
        }

        /// <summary>
        /// Multiplies the specified instance by a scalar.
        /// </summary>
        /// <param name="scale">Left operand.</param>
        /// <param name="vec">Right operand.</param>
        /// <returns>Result of multiplication.</returns>
        [Pure]
        public static Vector2i operator *(int scale, Vector2i vec)
        {
            vec.X *= scale;
            vec.Y *= scale;
            return vec;
        }

        [Pure]
        public static Vector2i operator *(Vector2i vec, float scale)
        {
            vec.X = (int)(vec.X * scale);
            vec.Y = (int)(vec.Y * scale);
            return vec;
        }

        [Pure]
        public static Vector2i operator *(float scale, Vector2i vec)
        {
            vec.X = (int)(vec.X * scale);
            vec.Y = (int)(vec.Y * scale);
            return vec;
        }

        /// <summary>
        /// Component-wise multiplication between the specified instance by a scale Vector.
        /// </summary>
        /// <param name="scale">Left operand.</param>
        /// <param name="vec">Right operand.</param>
        /// <returns>Result of multiplication.</returns>
        [Pure]
        public static Vector2i operator *(Vector2i vec, Vector2i scale)
        {
            vec.X *= scale.X;
            vec.Y *= scale.Y;
            return vec;
        }

        /// <summary>
        /// Divides the instance by a scalar using integer division, floor(a/b).
        /// </summary>
        /// <param name="vec">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <returns>Result of the division.</returns>
        [Pure]
        public static Vector2i operator /(Vector2i vec, int scale)
        {
            vec.X /= scale;
            vec.Y /= scale;
            return vec;
        }

        /// <summary>
        /// Compares the specified instances for equality.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns>True if both instances are equal; false otherwise.</returns>
        public static bool operator ==(Vector2i left, Vector2i right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares the specified instances for inequality.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns>True if both instances are not equal; false otherwise.</returns>
        public static bool operator !=(Vector2i left, Vector2i right)
        {
            return !(left == right);
        }

		[Pure]
		public static implicit operator System.Drawing.Size(Vector2i vec)
		{
			return new(vec.X, vec.Y);
		}

		/// <summary>
		/// Converts OpenTK.Vector2i to OpenTK.Vector2.
		/// </summary>
		/// <param name="vec">The Vector2i to convert.</param>
		/// <returns>The resulting Vector2.</returns>
		[Pure]
        public static implicit operator Vector2(Vector2i vec)
        {
            return new Vector2(vec.X, vec.Y);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector2i"/> struct using a tuple containing the component
        /// values.
        /// </summary>
        /// <param name="values">A tuple containing the component values.</param>
        /// <returns>A new instance of the <see cref="Vector2i"/> struct with the given component values.</returns>
        [Pure]
        public static implicit operator Vector2i((int X, int Y) values)
        {
            return new Vector2i(values.X, values.Y);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format("({0}{2} {1})", X, Y, MathHelper.ListSeparator);
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return obj is Vector2i && Equals((Vector2i)obj);
        }

        /// <inheritdoc/>
        public bool Equals(Vector2i other)
        {
            return X == other.X &&
                   Y == other.Y;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        /// <summary>
        /// Deconstructs the Vector into it's individual components.
        /// </summary>
        /// <param name="x">The X component of the Vector.</param>
        /// <param name="y">The Y component of the Vector.</param>
        [Pure]
        public void Deconstruct(out int x, out int y)
        {
            x = X;
            y = Y;
        }
    }
}
