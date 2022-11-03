/*
Copyright (c) 2006 - 2008 The Open Toolkit library.

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
of the Software, and to permit persons to whom the Software is furnished to do
so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
 */

using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace Engine.Mathematics
{
    /// <summary>
    /// Represents a 2D Vector using two single-precision floating-point numbers.
    /// </summary>
    /// <remarks>
    /// The Vector2 structure is suitable for interoperation with unmanaged code requiring two consecutive floats.
    /// </remarks>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector2 : IEquatable<Vector2>
    {
        /// <summary>
        /// The X component of the Vector2.
        /// </summary>
        public float X;

        /// <summary>
        /// The Y component of the Vector2.
        /// </summary>
        public float Y;

		public float Width
		{
			get { return X; }
			set { X = value; }
		}

		public float Height
		{
			get { return Y; }
			set { Y = value; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Vector2"/> struct.
		/// </summary>
		/// <param name="value">The value that will initialize this instance.</param>
		public Vector2(float value)
        {
            X = value;
            Y = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector2"/> struct.
        /// </summary>
        /// <param name="x">The x coordinate of the net Vector2.</param>
        /// <param name="y">The y coordinate of the net Vector2.</param>
        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Gets or sets the value at the index of the Vector.
        /// </summary>
        /// <param name="index">The index of the component from the Vector.</param>
        /// <exception cref="IndexOutOfRangeException">Thrown if the index is less than 0 or greater than 1.</exception>
        public float this[int index]
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
        /// Gets the length (magnitude) of the Vector.
        /// </summary>
        /// <see cref="LengthFast"/>
        /// <seealso cref="LengthSquared"/>
        public float Length => MathF.Sqrt((X * X) + (Y * Y));

        /// <summary>
        /// Gets an approximation of the Vector length (magnitude).
        /// </summary>
        /// <remarks>
        /// This property uses an approximation of the square root function to calculate Vector magnitude, with
        /// an upper error bound of 0.001.
        /// </remarks>
        /// <see cref="Length"/>
        /// <seealso cref="LengthSquared"/>
        public float LengthFast => 1.0f / MathHelper.InverseSqrtFast((X * X) + (Y * Y));

        /// <summary>
        /// Gets the square of the Vector length (magnitude).
        /// </summary>
        /// <remarks>
        /// This property avoids the costly square root operation required by the Length property. This makes it more suitable
        /// for comparisons.
        /// </remarks>
        /// <see cref="Length"/>
        /// <seealso cref="LengthFast"/>
        public float LengthSquared => (X * X) + (Y * Y);

        /// <summary>
        /// Gets the perpendicular Vector on the right side of this Vector.
        /// </summary>
        public Vector2 PerpendicularRight => new Vector2(Y, -X);

        /// <summary>
        /// Gets the perpendicular Vector on the left side of this Vector.
        /// </summary>
        public Vector2 PerpendicularLeft => new Vector2(-Y, X);

		public Vector2 Normalized
		{
			get
			{
				Vector2 v = this;
				v.Normalize();
				return v;
			}
		}

        /// <summary>
        /// Scales the Vector2 to unit length.
        /// </summary>
        public void Normalize()
        {
            var scale = 1.0f / Length;
            X *= scale;
            Y *= scale;
        }

        /// <summary>
        /// Scales the Vector2 to approximately unit length.
        /// </summary>
        public void NormalizeFast()
        {
            var scale = MathHelper.InverseSqrtFast((X * X) + (Y * Y));
            X *= scale;
            Y *= scale;
        }

        /// <summary>
        /// Defines a unit-length Vector2 that points towards the X-axis.
        /// </summary>
        public static readonly Vector2 UnitX = new Vector2(1, 0);

        /// <summary>
        /// Defines a unit-length Vector2 that points towards the Y-axis.
        /// </summary>
        public static readonly Vector2 UnitY = new Vector2(0, 1);

        /// <summary>
        /// Defines a zero-length Vector2.
        /// </summary>
        public static readonly Vector2 Zero = new Vector2(0, 0);

        /// <summary>
        /// Defines a NaN-length Vector2.
        /// </summary>
        public static readonly Vector2 NaN = new Vector2(float.NaN, float.NaN);

        /// <summary>
        /// Defines an instance with all components set to 1.
        /// </summary>
        public static readonly Vector2 One = new Vector2(1, 1);

        /// <summary>
        /// Defines an instance with all components set to positive infinity.
        /// </summary>
        public static readonly Vector2 PositiveInfinity = new Vector2(float.PositiveInfinity, float.PositiveInfinity);

        /// <summary>
        /// Defines an instance with all components set to negative infinity.
        /// </summary>
        public static readonly Vector2 NegativeInfinity = new Vector2(float.NegativeInfinity, float.NegativeInfinity);

        /// <summary>
        /// Defines the size of the Vector2 struct in bytes.
        /// </summary>
        public static readonly int SizeInBytes = Unsafe.SizeOf<Vector2>();

        /// <summary>
        /// Adds two Vectors.
        /// </summary>
        /// <param name="a">Left operand.</param>
        /// <param name="b">Right operand.</param>
        /// <returns>Result of operation.</returns>
        [Pure]
        public static Vector2 Add(Vector2 a, Vector2 b)
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
        public static void Add(in Vector2 a, in Vector2 b, out Vector2 result)
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
        public static Vector2 Subtract(Vector2 a, Vector2 b)
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
        public static void Subtract(in Vector2 a, in Vector2 b, out Vector2 result)
        {
            result.X = a.X - b.X;
            result.Y = a.Y - b.Y;
        }

        /// <summary>
        /// Multiplies a Vector by a scalar.
        /// </summary>
        /// <param name="Vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <returns>Result of the operation.</returns>
        [Pure]
        public static Vector2 Multiply(Vector2 Vector, float scale)
        {
            Multiply(in Vector, scale, out Vector);
            return Vector;
        }

        /// <summary>
        /// Multiplies a Vector by a scalar.
        /// </summary>
        /// <param name="Vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <param name="result">Result of the operation.</param>
        public static void Multiply(in Vector2 Vector, float scale, out Vector2 result)
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
        public static Vector2 Multiply(Vector2 Vector, Vector2 scale)
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
        public static void Multiply(in Vector2 Vector, in Vector2 scale, out Vector2 result)
        {
            result.X = Vector.X * scale.X;
            result.Y = Vector.Y * scale.Y;
        }

        /// <summary>
        /// Divides a Vector by a scalar.
        /// </summary>
        /// <param name="Vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <returns>Result of the operation.</returns>
        [Pure]
        public static Vector2 Divide(Vector2 Vector, float scale)
        {
            Divide(in Vector, scale, out Vector);
            return Vector;
        }

        /// <summary>
        /// Divides a Vector by a scalar.
        /// </summary>
        /// <param name="Vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <param name="result">Result of the operation.</param>
        public static void Divide(in Vector2 Vector, float scale, out Vector2 result)
        {
            result.X = Vector.X / scale;
            result.Y = Vector.Y / scale;
        }

        /// <summary>
        /// Divides a Vector by the components of a Vector (scale).
        /// </summary>
        /// <param name="Vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <returns>Result of the operation.</returns>
        [Pure]
        public static Vector2 Divide(Vector2 Vector, Vector2 scale)
        {
            Divide(in Vector, in scale, out Vector);
            return Vector;
        }

        /// <summary>
        /// Divide a Vector by the components of a Vector (scale).
        /// </summary>
        /// <param name="Vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <param name="result">Result of the operation.</param>
        public static void Divide(in Vector2 Vector, in Vector2 scale, out Vector2 result)
        {
            result.X = Vector.X / scale.X;
            result.Y = Vector.Y / scale.Y;
        }

		[Pure]
		public static Vector2 Min(Vector2 a, Vector2 b) => ComponentMin(a, b);

        /// <summary>
        /// Returns a Vector created from the smallest of the corresponding components of the given Vectors.
        /// </summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <returns>The component-wise minimum.</returns>
        [Pure]
        public static Vector2 ComponentMin(Vector2 a, Vector2 b)
        {
            a.X = a.X < b.X ? a.X : b.X;
            a.Y = a.Y < b.Y ? a.Y : b.Y;
            return a;
        }

        /// <summary>
        /// Returns a Vector created from the smallest of the corresponding components of the given Vectors.
        /// </summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">The component-wise minimum.</param>
        public static void ComponentMin(in Vector2 a, in Vector2 b, out Vector2 result)
        {
            result.X = a.X < b.X ? a.X : b.X;
            result.Y = a.Y < b.Y ? a.Y : b.Y;
        }

        /// <summary>
        /// Returns a Vector created from the largest of the corresponding components of the given Vectors.
        /// </summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <returns>The component-wise maximum.</returns>
        [Pure]
        public static Vector2 ComponentMax(Vector2 a, Vector2 b)
        {
            a.X = a.X > b.X ? a.X : b.X;
            a.Y = a.Y > b.Y ? a.Y : b.Y;
            return a;
        }

        /// <summary>
        /// Returns a Vector created from the largest of the corresponding components of the given Vectors.
        /// </summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">The component-wise maximum.</param>
        public static void ComponentMax(in Vector2 a, in Vector2 b, out Vector2 result)
        {
            result.X = a.X > b.X ? a.X : b.X;
            result.Y = a.Y > b.Y ? a.Y : b.Y;
        }

        /// <summary>
        /// Returns the Vector2 with the minimum magnitude. If the magnitudes are equal, the second Vector
        /// is selected.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns>The minimum Vector2.</returns>
        [Pure]
        public static Vector2 MagnitudeMin(Vector2 left, Vector2 right)
        {
            return left.LengthSquared < right.LengthSquared ? left : right;
        }

        /// <summary>
        /// Returns the Vector2 with the minimum magnitude. If the magnitudes are equal, the second Vector
        /// is selected.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <param name="result">The magnitude-wise minimum.</param>
        public static void MagnitudeMin(in Vector2 left, in Vector2 right, out Vector2 result)
        {
            result = left.LengthSquared < right.LengthSquared ? left : right;
        }

        /// <summary>
        /// Returns the Vector2 with the maximum magnitude. If the magnitudes are equal, the first Vector
        /// is selected.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns>The maximum Vector2.</returns>
        [Pure]
        public static Vector2 MagnitudeMax(Vector2 left, Vector2 right)
        {
            return left.LengthSquared >= right.LengthSquared ? left : right;
        }

        /// <summary>
        /// Returns the Vector2 with the maximum magnitude. If the magnitudes are equal, the first Vector
        /// is selected.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <param name="result">The magnitude-wise maximum.</param>
        public static void MagnitudeMax(in Vector2 left, in Vector2 right, out Vector2 result)
        {
            result = left.LengthSquared >= right.LengthSquared ? left : right;
        }

        /// <summary>
        /// Clamp a Vector to the given minimum and maximum Vectors.
        /// </summary>
        /// <param name="vec">Input Vector.</param>
        /// <param name="min">Minimum Vector.</param>
        /// <param name="max">Maximum Vector.</param>
        /// <returns>The clamped Vector.</returns>
        [Pure]
        public static Vector2 Clamp(Vector2 vec, Vector2 min, Vector2 max)
        {
            vec.X = vec.X < min.X ? min.X : vec.X > max.X ? max.X : vec.X;
            vec.Y = vec.Y < min.Y ? min.Y : vec.Y > max.Y ? max.Y : vec.Y;
            return vec;
        }

        /// <summary>
        /// Clamp a Vector to the given minimum and maximum Vectors.
        /// </summary>
        /// <param name="vec">Input Vector.</param>
        /// <param name="min">Minimum Vector.</param>
        /// <param name="max">Maximum Vector.</param>
        /// <param name="result">The clamped Vector.</param>
        public static void Clamp(in Vector2 vec, in Vector2 min, in Vector2 max, out Vector2 result)
        {
            result.X = vec.X < min.X ? min.X : vec.X > max.X ? max.X : vec.X;
            result.Y = vec.Y < min.Y ? min.Y : vec.Y > max.Y ? max.Y : vec.Y;
        }

        /// <summary>
        /// Compute the euclidean distance between two Vectors.
        /// </summary>
        /// <param name="vec1">The first Vector.</param>
        /// <param name="vec2">The second Vector.</param>
        /// <returns>The distance.</returns>
        [Pure]
        public static float Distance(Vector2 vec1, Vector2 vec2)
        {
            Distance(in vec1, in vec2, out float result);
            return result;
        }

        /// <summary>
        /// Compute the euclidean distance between two Vectors.
        /// </summary>
        /// <param name="vec1">The first Vector.</param>
        /// <param name="vec2">The second Vector.</param>
        /// <param name="result">The distance.</param>
        public static void Distance(in Vector2 vec1, in Vector2 vec2, out float result)
        {
            result = MathF.Sqrt(((vec2.X - vec1.X) * (vec2.X - vec1.X)) + ((vec2.Y - vec1.Y) * (vec2.Y - vec1.Y)));
        }

        /// <summary>
        /// Compute the squared euclidean distance between two Vectors.
        /// </summary>
        /// <param name="vec1">The first Vector.</param>
        /// <param name="vec2">The second Vector.</param>
        /// <returns>The squared distance.</returns>
        [Pure]
        public static float DistanceSquared(Vector2 vec1, Vector2 vec2)
        {
            DistanceSquared(in vec1, in vec2, out float result);
            return result;
        }

        /// <summary>
        /// Compute the squared euclidean distance between two Vectors.
        /// </summary>
        /// <param name="vec1">The first Vector.</param>
        /// <param name="vec2">The second Vector.</param>
        /// <param name="result">The squared distance.</param>
        public static void DistanceSquared(in Vector2 vec1, in Vector2 vec2, out float result)
        {
            result = ((vec2.X - vec1.X) * (vec2.X - vec1.X)) + ((vec2.Y - vec1.Y) * (vec2.Y - vec1.Y));
        }

        /// <summary>
        /// Scale a Vector to unit length.
        /// </summary>
        /// <param name="vec">The input Vector.</param>
        /// <returns>The normalized copy.</returns>
        [Pure]
        public static Vector2 Normalize(Vector2 vec)
        {
            var scale = 1.0f / vec.Length;
            vec.X *= scale;
            vec.Y *= scale;
            return vec;
        }

        /// <summary>
        /// Scale a Vector to unit length.
        /// </summary>
        /// <param name="vec">The input Vector.</param>
        /// <param name="result">The normalized Vector.</param>
        public static void Normalize(in Vector2 vec, out Vector2 result)
        {
            var scale = 1.0f / vec.Length;
            result.X = vec.X * scale;
            result.Y = vec.Y * scale;
        }

        /// <summary>
        /// Scale a Vector to approximately unit length.
        /// </summary>
        /// <param name="vec">The input Vector.</param>
        /// <returns>The normalized copy.</returns>
        [Pure]
        public static Vector2 NormalizeFast(Vector2 vec)
        {
            var scale = MathHelper.InverseSqrtFast((vec.X * vec.X) + (vec.Y * vec.Y));
            vec.X *= scale;
            vec.Y *= scale;
            return vec;
        }

        /// <summary>
        /// Scale a Vector to approximately unit length.
        /// </summary>
        /// <param name="vec">The input Vector.</param>
        /// <param name="result">The normalized Vector.</param>
        public static void NormalizeFast(in Vector2 vec, out Vector2 result)
        {
            var scale = MathHelper.InverseSqrtFast((vec.X * vec.X) + (vec.Y * vec.Y));
            result.X = vec.X * scale;
            result.Y = vec.Y * scale;
        }

        /// <summary>
        /// Calculate the dot (scalar) product of two Vectors.
        /// </summary>
        /// <param name="left">First operand.</param>
        /// <param name="right">Second operand.</param>
        /// <returns>The dot product of the two inputs.</returns>
        [Pure]
        public static float Dot(Vector2 left, Vector2 right)
        {
            return (left.X * right.X) + (left.Y * right.Y);
        }

        /// <summary>
        /// Calculate the dot (scalar) product of two Vectors.
        /// </summary>
        /// <param name="left">First operand.</param>
        /// <param name="right">Second operand.</param>
        /// <param name="result">The dot product of the two inputs.</param>
        public static void Dot(in Vector2 left, in Vector2 right, out float result)
        {
            result = (left.X * right.X) + (left.Y * right.Y);
        }

        /// <summary>
        /// Calculate the perpendicular dot (scalar) product of two Vectors.
        /// </summary>
        /// <param name="left">First operand.</param>
        /// <param name="right">Second operand.</param>
        /// <returns>The perpendicular dot product of the two inputs.</returns>
        [Pure]
        public static float PerpDot(Vector2 left, Vector2 right)
        {
            return (left.X * right.Y) - (left.Y * right.X);
        }

        /// <summary>
        /// Calculate the perpendicular dot (scalar) product of two Vectors.
        /// </summary>
        /// <param name="left">First operand.</param>
        /// <param name="right">Second operand.</param>
        /// <param name="result">The perpendicular dot product of the two inputs.</param>
        public static void PerpDot(in Vector2 left, in Vector2 right, out float result)
        {
            result = (left.X * right.Y) - (left.Y * right.X);
        }

        /// <summary>
        /// Returns a new Vector that is the linear blend of the 2 given Vectors.
        /// </summary>
        /// <param name="a">First input Vector.</param>
        /// <param name="b">Second input Vector.</param>
        /// <param name="blend">The blend factor. a when blend=0, b when blend=1.</param>
        /// <returns>a when blend=0, b when blend=1, and a linear combination otherwise.</returns>
        [Pure]
        public static Vector2 Lerp(Vector2 a, Vector2 b, float blend)
        {
            a.X = (blend * (b.X - a.X)) + a.X;
            a.Y = (blend * (b.Y - a.Y)) + a.Y;
            return a;
        }

        /// <summary>
        /// Returns a new Vector that is the linear blend of the 2 given Vectors.
        /// </summary>
        /// <param name="a">First input Vector.</param>
        /// <param name="b">Second input Vector.</param>
        /// <param name="blend">The blend factor. a when blend=0, b when blend=1.</param>
        /// <param name="result">a when blend=0, b when blend=1, and a linear combination otherwise.</param>
        public static void Lerp(in Vector2 a, in Vector2 b, float blend, out Vector2 result)
        {
            result.X = (blend * (b.X - a.X)) + a.X;
            result.Y = (blend * (b.Y - a.Y)) + a.Y;
        }

        /// <summary>
        /// Interpolate 3 Vectors using Barycentric coordinates.
        /// </summary>
        /// <param name="a">First input Vector.</param>
        /// <param name="b">Second input Vector.</param>
        /// <param name="c">Third input Vector.</param>
        /// <param name="u">First Barycentric Coordinate.</param>
        /// <param name="v">Second Barycentric Coordinate.</param>
        /// <returns>a when u=v=0, b when u=1,v=0, c when u=0,v=1, and a linear combination of a,b,c otherwise.</returns>
        [Pure]
        public static Vector2 BaryCentric(Vector2 a, Vector2 b, Vector2 c, float u, float v)
        {
            BaryCentric(in a, in b, in c, u, v, out var result);
            return result;
        }

        /// <summary>
        /// Interpolate 3 Vectors using Barycentric coordinates.
        /// </summary>
        /// <param name="a">First input Vector.</param>
        /// <param name="b">Second input Vector.</param>
        /// <param name="c">Third input Vector.</param>
        /// <param name="u">First Barycentric Coordinate.</param>
        /// <param name="v">Second Barycentric Coordinate.</param>
        /// <param name="result">
        /// Output Vector. a when u=v=0, b when u=1,v=0, c when u=0,v=1, and a linear combination of a,b,c
        /// otherwise.
        /// </param>
        public static void BaryCentric
        (
            in Vector2 a,
            in Vector2 b,
            in Vector2 c,
            float u,
            float v,
            out Vector2 result
        )
        {
            Subtract(in b, in a, out var ab);
            Multiply(in ab, u, out var abU);
            Add(in a, in abU, out var uPos);

            Subtract(in c, in a, out var ac);
            Multiply(in ac, v, out var acV);
            Add(in uPos, in acV, out result);
        }

        /// <summary>
        /// Transform a Vector by the given Matrix.
        /// </summary>
        /// <param name="vec">The Vector to transform.</param>
        /// <param name="mat">The desired transformation.</param>
        /// <returns>The transformed Vector.</returns>
        [Pure]
        public static Vector2 TransformRow(Vector2 vec, Matrix2 mat)
        {
            TransformRow(in vec, in mat, out Vector2 result);
            return result;
        }

        /// <summary>
        /// Transform a Vector by the given Matrix.
        /// </summary>
        /// <param name="vec">The Vector to transform.</param>
        /// <param name="mat">The desired transformation.</param>
        /// <param name="result">The transformed Vector.</param>
        public static void TransformRow(in Vector2 vec, in Matrix2 mat, out Vector2 result)
        {
            result = new Vector2(
                (vec.X * mat.Row0.X) + (vec.Y * mat.Row1.X),
                (vec.X * mat.Row0.Y) + (vec.Y * mat.Row1.Y));
        }

        /// <summary>
        /// Transforms a Vector by a quaternion rotation.
        /// </summary>
        /// <param name="vec">The Vector to transform.</param>
        /// <param name="quat">The quaternion to rotate the Vector by.</param>
        /// <returns>The result of the operation.</returns>
        [Pure]
        public static Vector2 Transform(Vector2 vec, Rotation quat)
        {
            Transform(in vec, in quat, out Vector2 result);
            return result;
        }

        /// <summary>
        /// Transforms a Vector by a quaternion rotation.
        /// </summary>
        /// <param name="vec">The Vector to transform.</param>
        /// <param name="quat">The quaternion to rotate the Vector by.</param>
        /// <param name="result">The result of the operation.</param>
        public static void Transform(in Vector2 vec, in Rotation quat, out Vector2 result)
        {
            Rotation v = new Rotation(vec.X, vec.Y, 0, 0);
            Rotation.Invert(in quat, out Rotation i);
            Rotation.Multiply(in quat, in v, out Rotation t);
            Rotation.Multiply(in t, in i, out v);

            result.X = v.X;
            result.Y = v.Y;
        }

        /// <summary>
        /// Transform a Vector by the given Matrix using right-handed notation.
        /// </summary>
        /// <param name="mat">The desired transformation.</param>
        /// <param name="vec">The Vector to transform.</param>
        /// <returns>The transformed Vector.</returns>
        [Pure]
        public static Vector2 TransformColumn(Matrix2 mat, Vector2 vec)
        {
            TransformColumn(in mat, in vec, out Vector2 result);
            return result;
        }

        /// <summary>
        /// Transform a Vector by the given Matrix using right-handed notation.
        /// </summary>
        /// <param name="mat">The desired transformation.</param>
        /// <param name="vec">The Vector to transform.</param>
        /// <param name="result">The transformed Vector.</param>
        public static void TransformColumn(in Matrix2 mat, in Vector2 vec, out Vector2 result)
        {
            result.X = (mat.Row0.X * vec.X) + (mat.Row0.Y * vec.Y);
            result.Y = (mat.Row1.X * vec.X) + (mat.Row1.Y * vec.Y);
        }

        /// <summary>
        /// Gets or sets an OpenTK.Vector2 with the Y and X components of this instance.
        /// </summary>
        [XmlIgnore]
        public Vector2 Yx
        {
            get => new Vector2(Y, X);
            set
            {
                Y = value.X;
                X = value.Y;
            }
        }

        /// <summary>
        /// Adds the specified instances.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns>Result of addition.</returns>
        [Pure]
        public static Vector2 operator +(Vector2 left, Vector2 right)
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
        public static Vector2 operator -(Vector2 left, Vector2 right)
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
        public static Vector2 operator -(Vector2 vec)
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
        public static Vector2 operator *(Vector2 vec, float scale)
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
        public static Vector2 operator *(float scale, Vector2 vec)
        {
            vec.X *= scale;
            vec.Y *= scale;
            return vec;
        }

        /// <summary>
        /// Component-wise multiplication between the specified instance by a scale Vector.
        /// </summary>
        /// <param name="scale">Left operand.</param>
        /// <param name="vec">Right operand.</param>
        /// <returns>Result of multiplication.</returns>
        [Pure]
        public static Vector2 operator *(Vector2 vec, Vector2 scale)
        {
            vec.X *= scale.X;
            vec.Y *= scale.Y;
            return vec;
        }

        /// <summary>
        /// Transform a Vector by the given Matrix.
        /// </summary>
        /// <param name="vec">The Vector to transform.</param>
        /// <param name="mat">The desired transformation.</param>
        /// <returns>The transformed Vector.</returns>
        [Pure]
        public static Vector2 operator *(Vector2 vec, Matrix2 mat)
        {
            TransformRow(in vec, in mat, out Vector2 result);
            return result;
        }

        /// <summary>
        /// Transform a Vector by the given Matrix using right-handed notation.
        /// </summary>
        /// <param name="mat">The desired transformation.</param>
        /// <param name="vec">The Vector to transform.</param>
        /// <returns>The transformed Vector.</returns>
        [Pure]
        public static Vector2 operator *(Matrix2 mat, Vector2 vec)
        {
            TransformColumn(in mat, in vec, out Vector2 result);
            return result;
        }

        /// <summary>
        /// Transforms a Vector by a quaternion rotation.
        /// </summary>
        /// <param name="vec">The Vector to transform.</param>
        /// <param name="quat">The quaternion to rotate the Vector by.</param>
        /// <returns>The multiplied Vector.</returns>
        [Pure]
        public static Vector2 operator *(Rotation quat, Vector2 vec)
        {
            Transform(in vec, in quat, out Vector2 result);
            return result;
        }

        /// <summary>
        /// Divides the specified instance by a scalar.
        /// </summary>
        /// <param name="vec">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <returns>Result of the division.</returns>
        [Pure]
        public static Vector2 operator /(Vector2 vec, float scale)
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
        public static bool operator ==(Vector2 left, Vector2 right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares the specified instances for inequality.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns>True if both instances are not equal; false otherwise.</returns>
        public static bool operator !=(Vector2 left, Vector2 right)
        {
            return !(left == right);
        }

		public static implicit operator float[](Vector2 vec)
		{
			return new float[] {vec.X, vec.Y };
		}

		[Pure]
		public static implicit operator System.Drawing.Size(Vector2 vec)
		{
			return new((int)vec.X, (int)vec.Y);
		}

		[Pure]
		public static implicit operator System.Drawing.Point(Vector2 vec)
		{
			return new((int)vec.X, (int)vec.Y);
		}

		[Pure]
		public static implicit operator System.Drawing.PointF(Vector2 vec)
		{
			return new((int)vec.X, (int)vec.Y);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Vector2"/> struct using a tuple containing the component
		/// values.
		/// </summary>
		/// <param name="values">A tuple containing the component values.</param>
		/// <returns>A new instance of the <see cref="Vector2"/> struct with the given component values.</returns>
		[Pure]
        public static implicit operator Vector2((float X, float Y) values)
        {
            return new Vector2(values.X, values.Y);
        }

        /// <summary>
        /// Converts OpenTK.Vector2 to OpenTK.Vector2i.
        /// </summary>
        /// <param name="vec">The Vector2 to convert.</param>
        /// <returns>The resulting Vector2i.</returns>
        [Pure]
        public static explicit operator Vector2i(Vector2 vec)
        {
            return new Vector2i((int)vec.X, (int)vec.Y);
        }

        public static implicit operator Vector2(System.Numerics.Vector2 v)
        {
            return new Vector2(v.X, v.Y);
        }

        private static readonly string ListSeparator = CultureInfo.CurrentCulture.TextInfo.ListSeparator;

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format("({0}{2} {1})", X, Y, MathHelper.ListSeparator);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is Vector2 && Equals((Vector2)obj);
        }

        /// <inheritdoc/>
        public bool Equals(Vector2 other)
        {
			// Special NaN comparison. Probably not the best place to do this.
			return (X == other.X && Y == other.Y) || (float.IsNaN(X) && float.IsNaN(other.X) && float.IsNaN(Y) && float.IsNaN(other.Y));
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
        public void Deconstruct(out float x, out float y)
        {
            x = X;
            y = Y;
        }
    }
}
