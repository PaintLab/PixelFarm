using System;
using Pencil.Gaming.MathUtils;

namespace Pencil.Gaming.MathUtils {
	public struct Rectangle {
		public float X, Y, Width, Height;
		public Vector2 Position {
			get {
				return new Vector2(X, Y);
			}
			set {
				X = value.X;
				Y = value.Y;
			}
		}
		public Vector2 Size {
			get {
				return new Vector2(Width, Height);
			}
			set {
				Width = value.X;
				Height = value.Y;
			}
		}

		public float Top {
			get {
				return Y;
			}
			set {
				Y = value;
			}
		}

		public float Bottom {
			get {
				return Y + Height;
			}
			set {
				Y = value - Height;
			}
		}

		public float Right {
			get {
				return X + Width;
			}
			set {
				X = value - Width;
			}
		}

		public float Left {
			get {
				return X;
			}
			set {
				X = value;
			}
		}

		public Rectangle(float x, float y, float width, float height) {
			X = x;
			Y = y;
			Width = width;
			Height = height;
		}
		public Rectangle(Vector2 position, float width, float height) : this(position.X, position.Y, width, height) {
		}
		public Rectangle(Vector2 position, Vector2 size) : this(position, size.X, size.Y) {
		}

		public Rectangle(Rectanglei rect) : this(rect.X, rect.Y, rect.Width, rect.Height) {
		}

		public bool Intersects(Rectangle rect) {
			return !((X >= rect.Right) || (Right <= rect.X) ||
				(Y >= rect.Bottom) || (Bottom <= rect.Y));
		}
		public bool IsVectorEnclosedBy(Vector2 vec) {
			return 
				(vec.X >= X && vec.X <= Right &&
				vec.Y >= Y && vec.Y <= Bottom);
		}
	}
}

