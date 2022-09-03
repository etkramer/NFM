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

namespace  Engine.Mathematics
{
    /// <summary>
    /// Represents a 3D Vector using three 32-bit integer numbers.
    /// </summary>
    /// <remarks>
    /// The Vector3i structure is suitable for interoperation with unmanaged code requiring three consecutive integers.
    /// </remarks>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector3i : IEquatable<Vector3i>
    {
        /// <summary>
        /// The X component of the Vector3i.
        /// </summary>
        public int X;

        /// <summary>
        /// The Y component of the Vector3i.
        /// </summary>
        public int Y;

        /// <summary>
        /// The Z component of the Vector3i.
        /// </summary>
        public int Z;

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector3i"/> struct.
        /// </summary>
        /// <param name="value">The value that will initialize this instance.</param>
        public Vector3i(int value)
        {
            X = value;
            Y = value;
            Z = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector3i"/> struct.
        /// </summary>
        /// <param name="x">The x component of the Vector3.</param>
        /// <param name="y">The y component of the Vector3.</param>
        /// <param name="z">The z component of the Vector3.</param>
        public Vector3i(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector3i"/> struct.
        /// </summary>
        /// <param name="v">The <see cref="Vector2i"/> to copy components from.</param>
        public Vector3i(Vector2i v)
        {
            X = v.X;
            Y = v.Y;
            Z = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector3i"/> struct.
        /// </summary>
        /// <param name="v">The <see cref="Vector2i"/> to copy components from.</param>
        /// <param name="z">The z component of the new Vector3.</param>
        public Vector3i(Vector2i v, int z)
        {
            X = v.X;
            Y = v.Y;
            Z = z;
        }

        /// <summary>
        /// Gets or sets the value at the index of the Vector.
        /// </summary>
        /// <param name="index">The index of the component from the Vector.</param>
        /// <exception cref="IndexOutOfRangeException">Thrown if the index is less than 0 or greater than 2.</exception>
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

                if (index == 2)
                {
                    return Z;
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
                else if (index == 2)
                {
                    Z = value;
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
        public int ManhattanLength => System.Math.Abs(X) + System.Math.Abs(Y) + System.Math.Abs(Z);

        /// <summary>
        /// Gets the euclidian length of the Vector.
        /// </summary>
        public float EuclideanLength => MathF.Sqrt((X * X) + (Y * Y) + (Z * Z));

        /// <summary>
        /// Defines a unit-length Vector3i that points towards the X-axis.
        /// </summary>
        public static readonly Vector3i UnitX = new Vector3i(1, 0, 0);

        /// <summary>
        /// Defines a unit-length Vector3i that points towards the Y-axis.
        /// </summary>
        public static readonly Vector3i UnitY = new Vector3i(0, 1, 0);

        /// <summary>
        /// Defines a unit-length Vector3i that points towards the Z-axis.
        /// </summary>
        public static readonly Vector3i UnitZ = new Vector3i(0, 0, 1);

        /// <summary>
        /// Defines the size of the Vector3i struct in bytes.
        /// </summary>
        public static readonly int SizeInBytes = Unsafe.SizeOf<Vector3i>();

        /// <summary>
        /// Adds two Vectors.
        /// </summary>
        /// <param name="a">Left operand.</param>
        /// <param name="b">Right operand.</param>
        /// <returns>Result of operation.</returns>
        [Pure]
        public static Vector3i Add(Vector3i a, Vector3i b)
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
        public static void Add(in Vector3i a, in Vector3i b, out Vector3i result)
        {
            result.X = a.X + b.X;
            result.Y = a.Y + b.Y;
            result.Z = a.Z + b.Z;
        }

        /// <summary>
        /// Subtract one Vector from another.
        /// </summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <returns>Result of subtraction.</returns>
        [Pure]
        public static Vector3i Subtract(Vector3i a, Vector3i b)
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
        public static void Subtract(in Vector3i a, in Vector3i b, out Vector3i result)
        {
            result.X = a.X - b.X;
            result.Y = a.Y - b.Y;
            result.Z = a.Z - b.Z;
        }

        /// <summary>
        /// Multiplies a Vector by a scalar.
        /// </summary>
        /// <param name="Vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <returns>Result of the operation.</returns>
        [Pure]
        public static Vector3i Multiply(Vector3i Vector, int scale)
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
        public static void Multiply(in Vector3i Vector, int scale, out Vector3i result)
        {
            result.X = Vector.X * scale;
            result.Y = Vector.Y * scale;
            result.Z = Vector.Z * scale;
        }

        /// <summary>
        /// Multiplies a Vector by the components a Vector (scale).
        /// </summary>
        /// <param name="Vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <returns>Result of the operation.</returns>
        [Pure]
        public static Vector3i Multiply(Vector3i Vector, Vector3i scale)
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
        public static void Multiply(in Vector3i Vector, in Vector3i scale, out Vector3i result)
        {
            result.X = Vector.X * scale.X;
            result.Y = Vector.Y * scale.Y;
            result.Z = Vector.Z * scale.Z;
        }

        /// <summary>
        /// Divides a Vector by a scalar using integer division, floor(a/b).
        /// </summary>
        /// <param name="Vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <returns>Result of the operation.</returns>
        [Pure]
        public static Vector3i Divide(Vector3i Vector, int scale)
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
        public static void Divide(in Vector3i Vector, int scale, out Vector3i result)
        {
            result.X = Vector.X / scale;
            result.Y = Vector.Y / scale;
            result.Z = Vector.Z / scale;
        }

        /// <summary>
        /// Divides a Vector by the components of a Vector using integer division, floor(a/b).
        /// </summary>
        /// <param name="Vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <returns>Result of the operation.</returns>
        [Pure]
        public static Vector3i Divide(Vector3i Vector, Vector3i scale)
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
        public static void Divide(in Vector3i Vector, in Vector3i scale, out Vector3i result)
        {
            result.X = Vector.X / scale.X;
            result.Y = Vector.Y / scale.Y;
            result.Z = Vector.Z / scale.Z;
        }

        /// <summary>
        /// Returns a Vector created from the smallest of the corresponding components of the given Vectors.
        /// </summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <returns>The component-wise minimum.</returns>
        [Pure]
        public static Vector3i ComponentMin(Vector3i a, Vector3i b)
        {
            a.X = a.X < b.X ? a.X : b.X;
            a.Y = a.Y < b.Y ? a.Y : b.Y;
            a.Z = a.Z < b.Z ? a.Z : b.Z;
            return a;
        }

        /// <summary>
        /// Returns a Vector created from the smallest of the corresponding components of the given Vectors.
        /// </summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">The component-wise minimum.</param>
        public static void ComponentMin(in Vector3i a, in Vector3i b, out Vector3i result)
        {
            result.X = a.X < b.X ? a.X : b.X;
            result.Y = a.Y < b.Y ? a.Y : b.Y;
            result.Z = a.Z < b.Z ? a.Z : b.Z;
        }

        /// <summary>
        /// Returns a Vector created from the largest of the corresponding components of the given Vectors.
        /// </summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <returns>The component-wise maximum.</returns>
        [Pure]
        public static Vector3i ComponentMax(Vector3i a, Vector3i b)
        {
            a.X = a.X > b.X ? a.X : b.X;
            a.Y = a.Y > b.Y ? a.Y : b.Y;
            a.Z = a.Z > b.Z ? a.Z : b.Z;
            return a;
        }

        /// <summary>
        /// Returns a Vector created from the largest of the corresponding components of the given Vectors.
        /// </summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">The component-wise maximum.</param>
        public static void ComponentMax(in Vector3i a, in Vector3i b, out Vector3i result)
        {
            result.X = a.X > b.X ? a.X : b.X;
            result.Y = a.Y > b.Y ? a.Y : b.Y;
            result.Z = a.Z > b.Z ? a.Z : b.Z;
        }

        /// <summary>
        /// Clamp a Vector to the given minimum and maximum Vectors.
        /// </summary>
        /// <param name="vec">Input Vector.</param>
        /// <param name="min">Minimum Vector.</param>
        /// <param name="max">Maximum Vector.</param>
        /// <returns>The clamped Vector.</returns>
        [Pure]
        public static Vector3i Clamp(Vector3i vec, Vector3i min, Vector3i max)
        {
            vec.X = MathHelper.Clamp(vec.X, min.X, max.X);
            vec.Y = MathHelper.Clamp(vec.Y, min.Y, max.Y);
            vec.Z = MathHelper.Clamp(vec.Z, min.Z, max.Z);
            return vec;
        }

        /// <summary>
        /// Clamp a Vector to the given minimum and maximum Vectors.
        /// </summary>
        /// <param name="vec">Input Vector.</param>
        /// <param name="min">Minimum Vector.</param>
        /// <param name="max">Maximum Vector.</param>
        /// <param name="result">The clamped Vector.</param>
        public static void Clamp(in Vector3i vec, in Vector3i min, in Vector3i max, out Vector3i result)
        {
            result.X = MathHelper.Clamp(vec.X, min.X, max.X);
            result.Y = MathHelper.Clamp(vec.Y, min.Y, max.Y);
            result.Z = MathHelper.Clamp(vec.Z, min.Z, max.Z);
        }

        /// <summary>
        /// Gets or sets a <see cref="Vector2i"/> with the X and Y components of this instance.
        /// </summary>
        [XmlIgnore]
        public Vector2i Xy
        {
            get => Unsafe.As<Vector3i, Vector2i>(ref this);
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }

        /// <summary>
        /// Gets or sets a <see cref="Vector2i"/> with the X and Z components of this instance.
        /// </summary>
        [XmlIgnore]
        public Vector2i Xz
        {
            get => new Vector2i(X, Z);
            set
            {
                X = value.X;
                Z = value.Y;
            }
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
        /// Gets or sets a <see cref="Vector2i"/> with the Y and Z components of this instance.
        /// </summary>
        [XmlIgnore]
        public Vector2i Yz
        {
            get => new Vector2i(Y, Z);
            set
            {
                Y = value.X;
                Z = value.Y;
            }
        }

        /// <summary>
        /// Gets or sets a <see cref="Vector2i"/> with the Z and X components of this instance.
        /// </summary>
        [XmlIgnore]
        public Vector2i Zx
        {
            get => new Vector2i(Z, X);
            set
            {
                Z = value.X;
                X = value.Y;
            }
        }

        /// <summary>
        /// Gets or sets a <see cref="Vector2i"/> with the Z and Y components of this instance.
        /// </summary>
        [XmlIgnore]
        public Vector2i Zy
        {
            get => new Vector2i(Z, Y);
            set
            {
                Z = value.X;
                Y = value.Y;
            }
        }

        /// <summary>
        /// Gets or sets a <see cref="Vector3i"/> with the X, Z, and Y components of this instance.
        /// </summary>
        [XmlIgnore]
        public Vector3i Xzy
        {
            get => new Vector3i(X, Z, Y);
            set
            {
                X = value.X;
                Z = value.Y;
                Y = value.Z;
            }
        }

        /// <summary>
        /// Gets or sets a <see cref="Vector3i"/> with the Y, X, and Z components of this instance.
        /// </summary>
        [XmlIgnore]
        public Vector3i Yxz
        {
            get => new Vector3i(Y, X, Z);
            set
            {
                Y = value.X;
                X = value.Y;
                Z = value.Z;
            }
        }

        /// <summary>
        /// Gets or sets a <see cref="Vector3i"/> with the Y, Z, and X components of this instance.
        /// </summary>
        [XmlIgnore]
        public Vector3i Yzx
        {
            get => new Vector3i(Y, Z, X);
            set
            {
                Y = value.X;
                Z = value.Y;
                X = value.Z;
            }
        }

        /// <summary>
        /// Gets or sets a <see cref="Vector3i"/> with the Z, X, and Y components of this instance.
        /// </summary>
        [XmlIgnore]
        public Vector3i Zxy
        {
            get => new Vector3i(Z, X, Y);
            set
            {
                Z = value.X;
                X = value.Y;
                Y = value.Z;
            }
        }

        /// <summary>
        /// Gets or sets a <see cref="Vector3i"/> with the Z, Y, and X components of this instance.
        /// </summary>
        [XmlIgnore]
        public Vector3i Zyx
        {
            get => new Vector3i(Z, Y, X);
            set
            {
                Z = value.X;
                Y = value.Y;
                X = value.Z;
            }
        }

        /// <summary>
        /// Gets a <see cref="Vector3"/> object with the same component values as the <see cref="Vector3i"/> instance.
        /// </summary>
        /// <returns>The resulting <see cref="Vector3"/> instance.</returns>
        public Vector3 ToVector3()
        {
            return new Vector3(X, Y, Z);
        }

        /// <summary>
        /// Gets a <see cref="Vector3"/> object with the same component values as the <see cref="Vector3i"/> instance.
        /// </summary>
        /// <param name="input">The given <see cref="Vector3i"/> to convert.</param>
        /// <param name="result">The resulting <see cref="Vector3"/>.</param>
        public static void ToVector3(in Vector3i input, out Vector3 result)
        {
            result.X = input.X;
            result.Y = input.Y;
            result.Z = input.Z;
        }

        /// <summary>
        /// Adds two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>The result of the calculation.</returns>
        [Pure]
        public static Vector3i operator +(Vector3i left, Vector3i right)
        {
            left.X += right.X;
            left.Y += right.Y;
            left.Z += right.Z;
            return left;
        }

        /// <summary>
        /// Subtracts two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>The result of the calculation.</returns>
        [Pure]
        public static Vector3i operator -(Vector3i left, Vector3i right)
        {
            left.X -= right.X;
            left.Y -= right.Y;
            left.Z -= right.Z;
            return left;
        }

        /// <summary>
        /// Negates an instance.
        /// </summary>
        /// <param name="vec">The instance.</param>
        /// <returns>The result of the calculation.</returns>
        [Pure]
        public static Vector3i operator -(Vector3i vec)
        {
            vec.X = -vec.X;
            vec.Y = -vec.Y;
            vec.Z = -vec.Z;
            return vec;
        }

        /// <summary>
        /// Multiplies an instance by an integer scalar.
        /// </summary>
        /// <param name="vec">The instance.</param>
        /// <param name="scale">The scalar.</param>
        /// <returns>The result of the calculation.</returns>
        [Pure]
        public static Vector3i operator *(Vector3i vec, int scale)
        {
            vec.X *= scale;
            vec.Y *= scale;
            vec.Z *= scale;
            return vec;
        }

        /// <summary>
        /// Multiplies an instance by an integer scalar.
        /// </summary>
        /// <param name="scale">The scalar.</param>
        /// <param name="vec">The instance.</param>
        /// <returns>The result of the calculation.</returns>
        [Pure]
        public static Vector3i operator *(int scale, Vector3i vec)
        {
            vec.X *= scale;
            vec.Y *= scale;
            vec.Z *= scale;
            return vec;
        }

        /// <summary>
        /// Component-wise multiplication between the specified instance by a scale Vector.
        /// </summary>
        /// <param name="scale">Left operand.</param>
        /// <param name="vec">Right operand.</param>
        /// <returns>Result of multiplication.</returns>
        [Pure]
        public static Vector3i operator *(Vector3i vec, Vector3i scale)
        {
            vec.X *= scale.X;
            vec.Y *= scale.Y;
            vec.Z *= scale.Z;
            return vec;
        }

        /// <summary>
        /// Divides the instance by a scalar using integer division, floor(a/b).
        /// </summary>
        /// <param name="vec">The instance.</param>
        /// <param name="scale">The scalar.</param>
        /// <returns>The result of the calculation.</returns>
        [Pure]
        public static Vector3i operator /(Vector3i vec, int scale)
        {
            vec.X /= scale;
            vec.Y /= scale;
            vec.Z /= scale;
            return vec;
        }

        /// <summary>
        /// Compares two instances for equality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left equals right; false otherwise.</returns>
        public static bool operator ==(Vector3i left, Vector3i right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares two instances for inequality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left does not equal right; false otherwise.</returns>
        public static bool operator !=(Vector3i left, Vector3i right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Converts OpenTK.Vector3i to OpenTK.Vector3.
        /// </summary>
        /// <param name="vec">The Vector3i to convert.</param>
        /// <returns>The resulting Vector3.</returns>
        [Pure]
        public static implicit operator Vector3(Vector3i vec)
        {
            return new Vector3(vec.X, vec.Y, vec.Z);
        }

        /// <summary>
        /// Converts OpenTK.Vector3i to OpenTK.Vector3d.
        /// </summary>
        /// <param name="vec">The Vector3i to convert.</param>
        /// <returns>The resulting Vector3d.</returns>
        [Pure]
        public static implicit operator Vector3d(Vector3i vec)
        {
            return new Vector3d(vec.X, vec.Y, vec.Z);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector3i"/> struct using a tuple containing the component
        /// values.
        /// </summary>
        /// <param name="values">A tuple containing the component values.</param>
        /// <returns>A new instance of the <see cref="Vector3i"/> struct with the given component values.</returns>
        [Pure]
        public static implicit operator Vector3i((int X, int Y, int Z) values)
        {
            return new Vector3i(values.X, values.Y, values.Z);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format("({0}{3} {1}{3} {2})", X, Y, Z, MathHelper.ListSeparator);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is Vector3i && Equals((Vector3i)obj);
        }

        /// <inheritdoc />
        public bool Equals(Vector3i other)
        {
            return X == other.X &&
                   Y == other.Y &&
                   Z == other.Z;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }

        /// <summary>
        /// Deconstructs the Vector into it's individual components.
        /// </summary>
        /// <param name="x">The X component of the Vector.</param>
        /// <param name="y">The Y component of the Vector.</param>
        /// <param name="z">The Z component of the Vector.</param>
        [Pure]
        public void Deconstruct(out int x, out int y, out int z)
        {
            x = X;
            y = Y;
            z = Z;
        }
    }
}
