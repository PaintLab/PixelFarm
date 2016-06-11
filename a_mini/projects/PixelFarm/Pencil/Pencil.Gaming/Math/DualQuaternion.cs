using System;
using Pencil.Gaming.MathUtils;

namespace Pencil.Gaming {
	/// <summary>
	/// Dual quaternion class represents a dual quaternion which an represent multiple transfromations
	/// in a in single structure an alternative to a 4x4 matrix 
	/// </summary>
	public class DualQuaternion {
		public DualQuaternion() {
			real = new Quaternion();
			dual = new Quaternion(0,0,0,0);
		}
		public DualQuaternion(Quaternion rotation)
		{
			real = rotation;
			dual = 0.5f* rotation * new Quaternion(0,0,0,0);
		}
		public DualQuaternion(Vector3 tranlation)
		{
			real = Quaternion.Identity;
			dual = 0.5f*Quaternion.FromVector(tranlation);
		}
		public DualQuaternion(Quaternion rotation, Vector3 translation)
		{
			real = rotation;
			dual = 0.5f*rotation*Quaternion.FromVector(translation);
		}
		/// <summary>
		/// Initializes a new instance of the 
		/// not public since it could be confusing as to what 
		/// this could refer to<see cref="Pencil.Gaming.DualQuaternion"/> class.
		/// </summary>
		/// <param name='real'>
		/// Real.
		/// </param>
		/// <param name='dual'>
		/// Dual.
		/// </param>
		DualQuaternion(Quaternion real, Quaternion dual)
		{
			this.real = real;
			this.dual = dual;
		}
		public Quaternion real;
		public Quaternion dual;

		public Quaternion Rotation{
			get{return real;}
		}

		public Quaternion Translation{
			get{return 2.0f*dual*(Quaternion.Conjugate(real));}
		}
		public static DualQuaternion Add(DualQuaternion left, DualQuaternion right)
		{
			return new DualQuaternion(left.real+ right.real, left.dual+right.dual);
		}
		public static DualQuaternion operator+(DualQuaternion left, DualQuaternion right)
		{
			return Add(left, right);
		}
		public static DualQuaternion Multiply(DualQuaternion left, DualQuaternion right)
		{
			return new DualQuaternion(left.real*right.real, left.real*right.dual + left.dual*right.real);
		}
		public static DualQuaternion Multiply(DualQuaternion left, DualQuaternion mid, DualQuaternion right)
		{
			return new DualQuaternion( left.real* mid.real*right.real, left.real*mid.dual*right.real + left.dual*mid.real*right.real);
		}
		public static DualQuaternion operator*(DualQuaternion left, DualQuaternion right)
		{
			return Multiply(left, right);
		}
		public static DualQuaternion Conjugate(DualQuaternion q)
		{
			return new DualQuaternion(Quaternion.Conjugate(q.real), Quaternion.Conjugate(q.dual));
		}
		public override string ToString() {
			return string.Format("[DualQuaternion: dual={0}, real={1}]", dual, real);
		}
	}
}

