using System;
using System.Diagnostics.CodeAnalysis;

namespace Engine.Mathematics
{
	public struct Box2D
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
				Width = value.X;
				Height = value.Y;
			}
		}

		public Vector2 TopLeft => new Vector2(Left, Bottom + Height);
		public Vector2 TopRight => new Vector2(Left + Width, Bottom + Height);
		public Vector2 BottomLeft => new Vector2(Left, Bottom);
		public Vector2 BottomRight => new Vector2(Left + Width, Bottom);

		public Box2D(Box2D @base, float leftOffset, float rightOffset, float topOffset, float bottomOffset)
		{
			Left = @base.Left + leftOffset;
			Right = @base.Right + rightOffset;
			Top = @base.Top + topOffset;
			Bottom = @base.Bottom + bottomOffset;
		}

		/// <param name="position">Uses Windows-style coordinates - (0,0) is top left.</param>
		public Box2D(Vector2 position, Vector2 size)
		{
			Left = position.X;
			Right = position.X + size.X;
			Top = position.Y + size.Y;
			Bottom = position.Y;
		}

		public Box2D(Vector2 topLeft, Vector2 bottomLeft, Vector2 topRight, Vector2 bottomRight)
		{
			Left = topLeft.X;
			Right = topRight.X;
			Top = topLeft.Y;
			Bottom = bottomLeft.Y;
		}

		public Box2D(float left, float right, float top, float bottom)
		{
			Left = left;
			Right = right;
			Top = top;
			Bottom = bottom;
		}

		public static Box2D operator -(Box2D left, Box2D right)
		{
			return new Box2D(left.Position - right.Position, left.Size - right.Size);
		}

		public static Box2D operator *(Box2D left, float right)
		{
			return new Box2D(left.Position * right, left.Size * right);
		}

		public static Box2D operator /(Box2D left, float right)
		{
			return new Box2D(left.Position / right, left.Size / right);
		}

		public static bool operator ==(Box2D left, Box2D right)
		{
			return (left.Left == right.Left) && (left.Right == right.Right) && (left.Top == right.Top) && (left.Bottom == right.Bottom);
		}

		public static bool operator !=(Box2D left, Box2D right)
		{
			return (left.Left != right.Left) || (left.Right != right.Right) || (left.Top != right.Top) || (left.Bottom != right.Bottom);
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static implicit operator System.Drawing.Rectangle(Box2D box)
		{
			return new System.Drawing.Rectangle((int)box.Left, (int)box.Bottom, (int)box.Width, (int)box.Height);
		}

		public static implicit operator System.Drawing.RectangleF(Box2D box)
		{
			return new System.Drawing.RectangleF(box.Left, box.Bottom, box.Width, box.Height);
		}

		public static explicit operator Box2D(Vector2 Size)
		{
			return new Box2D(0, Size.Width, Size.Height, 0);
		}

		public static readonly Box2D Zero = new Box2D(0, 0, 0, 0);

		public override string ToString()
		{
			return $"{Position}, {Size}";
		}
	}
}
