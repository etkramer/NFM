//
//  Vector3i.cs
//
//  Copyright (C) OpenTK
//
//  This software may be modified and distributed under the terms
//  of the MIT license. See the LICENSE file for details.
//

using System;
using System.Runtime.InteropServices;

namespace  Engine.Mathematics
{
    /// <summary>
    /// Represents an RGBA color using four 8-bit unsigned integers.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Color : IEquatable<Color>
    {
        public float R;
        public float G;
        public float B;
        public float A;

		#region Constructors
		public static Color FromHex(int Value)
		{
			byte[] Bytes = BitConverter.GetBytes(Value);

			Color Col = new();

			string Hex = Value.ToString("X6");
			string SR = Hex[0].ToString() + Hex[1].ToString();
			string SG = Hex[2].ToString() + Hex[3].ToString();
			string SB = Hex[4].ToString() + Hex[5].ToString();

			int IR = int.Parse(SR, System.Globalization.NumberStyles.HexNumber);
			int IG = int.Parse(SG, System.Globalization.NumberStyles.HexNumber);
			int IB = int.Parse(SB, System.Globalization.NumberStyles.HexNumber);

			Col.R = (1f / 255f) * IR;
			Col.G = (1f / 255f) * IG;
			Col.B = (1f / 255f) * IB;
			Col.A = 1;

			return Col;
		}

		public Color(float Value)
		{
			R = Value;
			G = Value;
			B = Value;
			A = 1;
		}

		public Color(float R, float G, float B)
		{
			this.R = R;
			this.G = G;
			this.B = B;
			this.A = 1f;
		}

		public Color(float R, float G, float B, float A)
        {
            this.R = R;
			this.G = G;
			this.B = B;
			this.A = A;
        }

		#endregion
		#region Conversions
		public static implicit operator System.Drawing.Color(Color col)
		{
			int a = (int)(col.A / (1f/255f));
			int r = (int)(col.R / (1f / 255f));
			int g = (int)(col.G / (1f / 255f));
			int b = (int)(col.B / (1f / 255f));
			return System.Drawing.Color.FromArgb(a, r, g, b);
		}

		public static implicit operator Color(int col)
		{
			return FromHex(col);
		}

		#endregion
		#region Common Utilities
		/// <summary>
		/// Gets or sets the value at the index of the Vector.
		/// </summary>
		public float this[float index]
        {
            get
            {
                if (index == 0)
                {
                    return R;
                }

                if (index == 1)
                {
                    return G;
                }

                if (index == 2)
                {
                    return B;
                }

                if (index == 3)
                {
                    return A;
                }

                throw new IndexOutOfRangeException("You tried to access this Vector at index: " + index);
            }

            set
            {
                if (index == 0)
                {
                    R = value;
                }
                else if (index == 1)
                {
                    G = value;
                }
                else if (index == 2)
                {
                    B = value;
                }
                else if (index == 3)
                {
                    A = value;
                }
                else
                {
                    throw new IndexOutOfRangeException("You tried to set this Vector at index: " + index);
                }
            }
        }

        /// <summary>
        /// Compares two instances for equality.
        /// </summary>
        public static bool operator ==(Color left, Color right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares two instances for inequality.
        /// </summary>
        public static bool operator !=(Color left, Color right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Converts OpenTK.Color to OpenTK.Vector4.
        /// </summary>
        public static implicit operator Vector4(Color vec)
        {
            return new Vector4(vec.R, vec.G, vec.B, vec.A);
        }

        public override string ToString()
        {
            return string.Format("({0}{4} {1}{4} {2}{4} {3})", R, G, B, A, MathHelper.ListSeparator);
        }

        public bool Equals(Color other)
        {
            return R == other.R &&
                   G == other.G &&
                   B == other.B &&
                   A == other.A;
        }

		public override bool Equals(object obj)
		{
			return obj is Color && Equals((Color)obj);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(R, G, B, A);
		}

		public Color WithAlpha(float Alpha)
		{
			return new Color(R, G, B, Alpha);
		}

		#endregion

		public static Color Invisible = new Color(0, 0, 0, 0);
		public static Color Black = new Color(0, 0, 0, 1);
		public static Color White = new Color(1, 1, 1, 1);
	}
}
