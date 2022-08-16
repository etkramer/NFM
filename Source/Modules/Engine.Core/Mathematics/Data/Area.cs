using System;
using System.Diagnostics.CodeAnalysis;

namespace  Engine.Mathematics
{
	public struct Area
	{
		public float Left;
		public float Right;
		public float Top;
		public float Bottom;

		public float X => Left;
		public float Y => Bottom;

		public float Width
		{
			get => Right - Left;
			set
			{
				Right = Left + value;
			}
		}

		public float Height
		{
			get => Bottom - Top;
			set
			{
				Top = Bottom + value;
			}
		}

		public Vector2 Center => Middle;
		public Vector2 Middle => new Vector2((Left + Right) / 2, (Top + Bottom) / 2);
		public Vector2 Position => new Vector2(Left, Bottom);
		
		public Vector2 Size
		{
			get => new Vector2(Width, Height);
			set
			{
				Width = value.X; Height = value.Y;
			}
		}

		public Vector2 TopLeft => new Vector2(Left, Bottom + Height);
		public Vector2 TopRight => new Vector2(Left + Width, Bottom + Height);
		public Vector2 BottomLeft => new Vector2(Left, Bottom);
		public Vector2 BottomRight => new Vector2(Left + Width, Bottom);

		public Area(Area Base, float LeftOffset, float RightOffset, float TopOffset, float BottomOffset)
		{
			Left = Base.Left + LeftOffset;
			Right = Base.Right + RightOffset;
			Top = Base.Top + TopOffset;
			Bottom = Base.Bottom + BottomOffset;
		}

		/// <param name="Position">Uses Windows-style coordinates - (0,0) is top left.</param>
		public Area(Vector2 Position, Vector2 Size)
		{
			Left = Position.X;
			Right = Position.X + Size.X;
			Top = Position.Y + Size.Y;
			Bottom = Position.Y;
		}

		public Area(Vector2 TopLeft, Vector2 BottomLeft, Vector2 TopRight, Vector2 BottomRight)
		{
			Left = TopLeft.X;
			Right = TopRight.X;
			Top = TopLeft.Y;
			Bottom = BottomLeft.Y;
		}

		public Area(float Left, float Right, float Top, float Bottom)
		{
			this.Left = Left;
			this.Right = Right;
			this.Top = Top;
			this.Bottom = Bottom;
		}

		public static Area operator -(Area A, Area B)
		{
			return new Area(A.Position - B.Position, A.Size - B.Size);
		}

		public static Area operator *(Area A, float B)
		{
			return new Area(A.Position * B, A.Size * B);
		}

		public static Area operator /(Area A, float B)
		{
			return new Area(A.Position / B, A.Size / B);
		}

		public static bool operator ==(Area x, Area y)
		{
			return (x.Left == y.Left) && (x.Right == y.Right) && (x.Top == y.Top) && (x.Bottom == y.Bottom);
		}

		public static bool operator !=(Area x, Area y)
		{
			return (x.Left != y.Left) || (x.Right != y.Right) || (x.Top != y.Top) || (x.Bottom != y.Bottom);
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static implicit operator System.Drawing.Rectangle(Area Rect)
		{
			return new System.Drawing.Rectangle((int)Rect.Left, (int)Rect.Bottom, (int)Rect.Width, (int)Rect.Height);
		}

		public static implicit operator System.Drawing.RectangleF(Area Rect)
		{
			return new System.Drawing.RectangleF(Rect.Left, Rect.Bottom, Rect.Width, Rect.Height);
		}

		public static explicit operator Area(Vector2 Size)
		{
			return new Area(0, Size.Width, Size.Height, 0);
		}

		public static readonly Area Zero = new Area(0, 0, 0, 0);

		public override string ToString()
		{
			return $"{Position}, {Size}";
		}
	}
}
