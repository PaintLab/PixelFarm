//MIT, 2009-2015, Rene Schulte and WriteableBitmapEx Contributors, https://github.com/teichgraf/WriteableBitmapEx
//MIT, 2017-2018, WinterDev

using System;
namespace PixelFarm.DrawingBuffer
{

    public enum AffineMatrixCommand : byte
    {
        None,
        Scale,
        Skew,
        Rotate,
        Translate,
        Invert
    }

    public struct AffinePlan
    {
        public readonly AffineMatrixCommand cmd;
        public readonly double x;
        public readonly double y;
        public AffinePlan(AffineMatrixCommand cmd, double x, double y)
        {
            this.x = x;
            this.y = y;
            this.cmd = cmd;
        }
        public AffinePlan(AffineMatrixCommand cmd, double x)
        {
            this.x = x;
            this.y = 0;
            this.cmd = cmd;
        }


        //----------------------------------------------------------------------------
        public static AffinePlan Translate(double x, double y)
        {
            return new AffinePlan(AffineMatrixCommand.Translate, x, y);
        }
        public static AffinePlan Rotate(double radAngle)
        {
            return new AffinePlan(AffineMatrixCommand.Rotate, radAngle);
        }
        public static AffinePlan Skew(double x, double y)
        {
            return new AffinePlan(AffineMatrixCommand.Skew, x, y);
        }
        public static AffinePlan Scale(double x, double y)
        {
            return new AffinePlan(AffineMatrixCommand.Scale, x, y);
        }
        public static AffinePlan Scale(double both)
        {
            return new AffinePlan(AffineMatrixCommand.Scale, both, both);
        }
    }
    public sealed class Affine
    {
        //agg's Affine

        const double EPSILON = 1e-14;
        public readonly double sx, shy, shx, sy, tx, ty;
        bool isIdenHint;
        public static readonly Affine IdentityMatrix = Affine.NewIdentity();
        //------------------------------------------ Construction
        private Affine(Affine copyFrom)
        {
            sx = copyFrom.sx;
            shy = copyFrom.shy;
            shx = copyFrom.shx;
            sy = copyFrom.sy;
            tx = copyFrom.tx;
            ty = copyFrom.ty;
        }
        internal Affine(AffinePlan[] creationPlans)
        {
            //-----------------------
            //start with identity matrix

            sx = 1; shy = 0;
            shx = 0; sy = 1;
            tx = 0; ty = 0;

            //-----------------------
            int j = creationPlans.Length;
            for (int i = 0; i < j; ++i)
            {
                AffinePlan plan = creationPlans[i];
                switch (plan.cmd)
                {
                    case AffineMatrixCommand.None:
                        break;
                    case AffineMatrixCommand.Rotate:
                        {
                            double angleRad = plan.x;
                            double ca = Math.Cos(angleRad);
                            double sa = Math.Sin(angleRad);
                            double t0 = sx * ca - shy * sa;
                            double t2 = shx * ca - sy * sa;
                            double t4 = tx * ca - ty * sa;
                            shy = sx * sa + shy * ca;
                            sy = shx * sa + sy * ca;
                            ty = tx * sa + ty * ca;
                            sx = t0;
                            shx = t2;
                            tx = t4;
                        }
                        break;
                    case AffineMatrixCommand.Scale:
                        {
                            double mm0 = plan.x;
                            double mm3 = plan.y;
                            sx *= mm0;
                            shx *= mm0;
                            tx *= mm0;
                            shy *= mm3;
                            sy *= mm3;
                            ty *= mm3;
                        }
                        break;
                    case AffineMatrixCommand.Translate:
                        {
                            tx += plan.x;
                            ty += plan.y;
                        }
                        break;
                    case AffineMatrixCommand.Skew:
                        {
                            double m_sx = 1;
                            double m_sy = 1;
                            double m_shx = Math.Tan(plan.x);
                            double m_shy = Math.Tan(plan.y);
                            double t0 = sx * m_sx + shy * m_shx;
                            double t2 = shx * m_sx + sy * m_shx;
                            double t4 = tx * m_sx + ty * m_shx + 0;//0=m.tx
                            shy = sx * m_shy + shy * m_sy;
                            sy = shx * m_shy + sy * m_sy;
                            ty = tx * m_shy + ty * m_sy + 0;//0= m.ty;
                            sx = t0;
                            shx = t2;
                            tx = t4;
                        }
                        break;
                    case AffineMatrixCommand.Invert:
                        {
                            double d = CalculateDeterminantReciprocal();
                            double t0 = sy * d;
                            sy = sx * d;
                            shy = -shy * d;
                            shx = -shx * d;
                            double t4 = -tx * t0 - ty * shx;
                            ty = -tx * shy - ty * sy;
                            sx = t0;
                            tx = t4;
                        }
                        break;
                    default:
                        {
                            throw new NotSupportedException();
                        }
                }
            }
        }
        // Custom matrix. Usually used in derived classes
        private Affine(double v0_sx, double v1_shy,
                       double v2_shx, double v3_sy,
                       double v4_tx, double v5_ty)
        {
            sx = v0_sx;
            shy = v1_shy;
            shx = v2_shx;
            sy = v3_sy;
            tx = v4_tx;
            ty = v5_ty;
        }
        public double m11 { get { return sx; } }
        public double m12 { get { return shy; } }
        public double m21 { get { return shx; } }
        public double m22 { get { return sy; } }
        public double dx { get { return tx; } }
        public double dy { get { return ty; } }
        // Custom matrix from m[6]
        private Affine(double[] m)
        {
            sx = m[0];
            shy = m[1];
            shx = m[2];
            sy = m[3];
            tx = m[4];
            ty = m[5];
        }
        private Affine(Affine a, Affine b)
        {
            //copy from a
            //multiply with b
            sx = a.sx;
            shy = a.shy;
            shx = a.shx;
            sy = a.sy;
            tx = a.tx;
            ty = a.ty;
            MultiplyMatrix(ref sx, ref sy, ref shx, ref shy, ref tx, ref ty, b);
        }
        private Affine(Affine copyFrom, AffinePlan creationPlan)
        {
            //-----------------------
            sx = copyFrom.sx;
            shy = copyFrom.shy;
            shx = copyFrom.shx;
            sy = copyFrom.sy;
            tx = copyFrom.tx;
            ty = copyFrom.ty;
            //-----------------------
            switch (creationPlan.cmd)
            {
                default:
                    {
                        throw new NotSupportedException();
                    }
                case AffineMatrixCommand.None:
                    break;
                case AffineMatrixCommand.Rotate:
                    {
                        double angleRad = creationPlan.x;
                        double ca = Math.Cos(angleRad);
                        double sa = Math.Sin(angleRad);
                        double t0 = sx * ca - shy * sa;
                        double t2 = shx * ca - sy * sa;
                        double t4 = tx * ca - ty * sa;
                        shy = sx * sa + shy * ca;
                        sy = shx * sa + sy * ca;
                        ty = tx * sa + ty * ca;
                        sx = t0;
                        shx = t2;
                        tx = t4;
                    }
                    break;
                case AffineMatrixCommand.Scale:
                    {
                        double mm0 = creationPlan.x;
                        double mm3 = creationPlan.y;
                        sx *= mm0;
                        shx *= mm0;
                        tx *= mm0;
                        shy *= mm3;
                        sy *= mm3;
                        ty *= mm3;
                    }
                    break;
                case AffineMatrixCommand.Skew:
                    {
                        double m_sx = 1;
                        double m_sy = 1;
                        double m_shx = Math.Tan(creationPlan.x);
                        double m_shy = Math.Tan(creationPlan.y);
                        double t0 = sx * m_sx + shy * m_shx;
                        double t2 = shx * m_sx + sy * m_shx;
                        double t4 = tx * m_sx + ty * m_shx + 0;//0=m.tx
                        shy = sx * m_shy + shy * m_sy;
                        sy = shx * m_shy + sy * m_sy;
                        ty = tx * m_shy + ty * m_sy + 0;//0= m.ty;
                        sx = t0;
                        shx = t2;
                        tx = t4;
                        //return new Affine(1.0, Math.Tan(y), Math.Tan(x), 1.0, 0.0, 0.0);

                    }
                    break;
                case AffineMatrixCommand.Translate:
                    {
                        tx += creationPlan.x;
                        ty += creationPlan.y;
                    }
                    break;
                case AffineMatrixCommand.Invert:
                    {
                        double d = CalculateDeterminantReciprocal();
                        double t0 = sy * d;
                        sy = sx * d;
                        shy = -shy * d;
                        shx = -shx * d;
                        double t4 = -tx * t0 - ty * shx;
                        ty = -tx * shy - ty * sy;
                        sx = t0;
                        tx = t4;
                    }
                    break;
            }
        }


        //----------------------------------------------------------
        public static Affine operator *(Affine a, Affine b)
        {
            //new input
            return new Affine(a, b);
        }
        //----------------------------------------------------------

        // Identity matrix
        static Affine NewIdentity()
        {
            var newIden = new Affine(
                1, 0,
                0, 1,
                0, 0);
            newIden.isIdenHint = true;
            return newIden;
        }

        //====================================================trans_affine_rotation
        // Rotation matrix. sin() and cos() are calculated twice for the same angle.
        // There's no harm because the performance of sin()/cos() is very good on all
        // modern processors. Besides, this operation is not going to be invoked too 
        // often.
        public static Affine NewRotation(double angRad)
        {
            double cos_rad, sin_rad;
            return new Affine(
               cos_rad = Math.Cos(angRad), sin_rad = Math.Sin(angRad),
                -sin_rad, cos_rad,
                0.0, 0.0);
        }

        //====================================================trans_affine_scaling
        // Scaling matrix. x, y - scale coefficients by X and Y respectively
        public static Affine NewScaling(double scale)
        {
            return new Affine(
                scale, 0.0,
                0.0, scale,
                0.0, 0.0);
        }

        public static Affine NewScaling(double x, double y)
        {
            return new Affine(
                x, 0.0,
                0.0, y,
                0.0, 0.0);
        }

        public static Affine NewTranslation(double x, double y)
        {
            return new Affine(
                1.0, 0.0,
                0.0, 1.0,
                x, y);
        }


        public static Affine NewSkewing(double x, double y)
        {
            return new Affine(
                1.0, Math.Tan(y),
                Math.Tan(x), 1.0,
                0.0, 0.0);
        }

        static void MultiplyMatrix(
            ref double sx, ref double sy,
            ref double shx, ref double shy,
            ref double tx, ref double ty,
            Affine m)
        {
            double t0 = sx * m.sx + shy * m.shx;
            double t2 = shx * m.sx + sy * m.shx;
            double t4 = tx * m.sx + ty * m.shx + m.tx;
            shy = sx * m.shy + shy * m.sy;
            sy = shx * m.shy + sy * m.sy;
            ty = tx * m.shy + ty * m.sy + m.ty;
            sx = t0;
            shx = t2;
            tx = t4;
        }

        // Invert matrix. Do not try to invert degenerate matrices, 
        // there's no check for validity. If you set scale to 0 and 
        // then try to invert matrix, expect unpredictable result.
        public Affine CreateInvert()
        {
            return new Affine(this, new AffinePlan(AffineMatrixCommand.Invert, 0));
        }
        //-------------------------------------------- Transformations
        // Direct transformation of x and y
        public void Transform(ref double x, ref double y)
        {
            double tmp = x;
            x = tmp * sx + y * shx + tx;
            y = tmp * shy + y * sy + ty;
        }


        // Inverse transformation of x and y. It works slower than the 
        // direct transformation. For massive operations it's better to 
        // invert() the matrix and then use direct transformations. 
        public void InverseTransform(ref double x, ref double y)
        {
            double d = CalculateDeterminantReciprocal();
            double a = (x - tx) * d;
            double b = (y - ty) * d;
            x = a * sy - b * shx;
            y = b * sx - a * shy;
        }


        // Calculate the reciprocal of the determinant
        double CalculateDeterminantReciprocal()
        {
            return 1.0 / (sx * sy - shy * shx);
        }

        // Get the average scale (by X and Y). 
        // Basically used to calculate the approximation_scale when
        // decomposinting curves into line segments.
        public double GetScale()
        {
            double x = 0.707106781 * sx + 0.707106781 * shx;
            double y = 0.707106781 * shy + 0.707106781 * sy;
            return Math.Sqrt(x * x + y * y);
        }

        // Check to see if the matrix is not degenerate
        public bool IsNotDegenerated(double epsilon)
        {
            return Math.Abs(sx) > epsilon && Math.Abs(sy) > epsilon;
        }

        // Check to see if it's an identity matrix
        public bool IsIdentity()
        {
            if (!isIdenHint)
            {
                return is_equal_eps(sx, 1.0) && is_equal_eps(shy, 0.0) &&
                   is_equal_eps(shx, 0.0) && is_equal_eps(sy, 1.0) &&
                   is_equal_eps(tx, 0.0) && is_equal_eps(ty, 0.0);
            }
            else
            {
                return true;
            }
        }

        static bool is_equal_eps(double v1, double v2)
        {
            return Math.Abs(v1 - v2) <= (EPSILON);
        }

    }


    public abstract class GeneralTransform
    {
        public abstract RectD TransformBounds(RectD r1);
        public abstract PointD Transform(PointD p);

        public abstract MatrixTransform Inverse
        {
            get;
        }

    }
    public class MatrixTransform : GeneralTransform
    {
        MatrixTransform _inverseVersion;
        Affine affine;
        public MatrixTransform(AffinePlan[] affPlans)
        {
            affine = new Affine(affPlans);
        }
        public MatrixTransform(Affine affine)
        {
            this.affine = affine;
        }
        public override PointD Transform(PointD p)
        {
            double p_x = p.X;
            double p_y = p.Y;
            affine.Transform(ref p_x, ref p_y);
            return new PointD(p_x, p_y);
        }
        public override MatrixTransform Inverse
        {
            get
            {
                if (_inverseVersion == null)
                {
                    return _inverseVersion = new MatrixTransform(affine.CreateInvert());
                }
                return _inverseVersion;
            }
        }

        class InternalPointD
        {
            public double X;
            public double Y;
            public InternalPointD(double x, double y)
            {
                this.X = x;
                this.Y = y;
            }
        }

        public override RectD TransformBounds(RectD r1)
        {
            InternalPointD tmp0 = new InternalPointD(r1.Left, r1.Top);
            InternalPointD tmp1 = new InternalPointD(r1.Right, r1.Top);
            InternalPointD tmp2 = new InternalPointD(r1.Right, r1.Bottom);
            InternalPointD tmp3 = new InternalPointD(r1.Left, r1.Bottom);

            affine.Transform(ref tmp0.X, ref tmp0.Y);
            affine.Transform(ref tmp1.X, ref tmp1.Y);
            affine.Transform(ref tmp2.X, ref tmp2.Y);
            affine.Transform(ref tmp3.X, ref tmp3.Y);


            //_tmp2[0] = new System.Drawing.PointF((float)r1.Left, (float)r1.Top);
            //_tmp2[1] = new System.Drawing.PointF((float)r1.Right, (float)r1.Top);
            //_tmp2[2] = new System.Drawing.PointF((float)r1.Right, (float)r1.Bottom);
            //_tmp2[3] = new System.Drawing.PointF((float)r1.Left, (float)r1.Bottom);
            ////find a new bound

            //return new RectD(_tmp2[0].X, _tmp2[0].Y, _tmp2[2].X - _tmp[0].X, _tmp2[2].Y - _tmp2[1].Y);

            return new RectD(tmp0.X, tmp0.Y, tmp2.X - tmp0.X, tmp2.Y - tmp1.Y);
        }

    }






    public struct RectD
    {
        public RectD(double left, double top, double width, double height)
        {
            this.Left = left;
            this.Top = top;
            this.Width = width;
            this.Height = height;
        }
        public RectD(PointD location, SizeD size)
        {
            this.Left = location.Left;
            this.Top = location.Top;
            this.Width = size.Width;
            this.Height = size.Height;
        }
        public double Left { get; private set; }
        public double Top { get; private set; }
        public double Width { get; private set; }
        public double Height { get; private set; }
        public double X { get { return this.Left; } }
        public double Y { get { return this.Top; } }
        public double Bottom { get { return Y + Height; } }
        public double Right { get { return Left + Width; } }
        public bool IsEmpty
        {
            get
            {
                //TODO: eval once
                return this.Left == 0 && this.Top == 0
                    && this.Width == 0 && this.Height == 0;
            }
        }
        private bool IntersectsWithInclusive(RectD r)
        {
            return !((Left > r.Right) || (Right < r.Left) ||
                (Top > r.Bottom) || (Bottom < r.Top));
        }
        public static RectD Intersect(RectD a, RectD b)
        {
            // MS.NET returns a non-empty rectangle if the two rectangles
            // touch each other
            if (!a.IntersectsWithInclusive(b))
                return new RectD(0, 0, 0, 0);
            return RectD.FromLTRB(
                Math.Max(a.Left, b.Left),
                Math.Max(a.Top, b.Top),
                Math.Min(a.Right, b.Right),
                Math.Min(a.Bottom, b.Bottom));
        }

        public static RectD FromLTRB(double left, double top, double right, double bottom)
        {
            // MS.NET returns a non-empty rectangle if the two rectangles
            // touch each other
            return new RectD(left, top, right - left, bottom - top);
        }

        /// <summary>
        ///	Intersect Method
        /// </summary>
        ///
        /// <remarks>
        ///	Replaces the Rectangle with the intersection of itself
        ///	and another Rectangle.
        /// </remarks>

        public void Intersect(RectD rect)
        {
            this = RectD.Intersect(this, rect);
        }
    }
    public struct PointD
    {
        public PointD(int x, int y)
        {
            this.Left = x;
            this.Top = y;
        }
        public PointD(double x, double y)
        {
            this.Left = x;
            this.Top = y;
        }
        public double Left { get; private set; }
        public double Top { get; private set; }
        public double X
        {
            get
            {
                return this.Left;
            }

            set
            {
                this.Left = value;
            }
        }
        public double Y
        {
            get
            {
                return this.Top;
            }
            set
            {
                this.Top = value;
            }

        }
    }
    public struct SizeD
    {
        public SizeD(int w, int h)
        {
            this.Width = w;
            this.Height = h;
        }
        public SizeD(double w, double h)
        {
            this.Width = w;
            this.Height = h;
        }
        public double Width { get; private set; }
        public double Height { get; private set; }
    }
    public struct ColorInt
    {
        //see https://github.com/PaintLab/PixelFarm/issues/12
        //we store color as 'straight alpha'
        byte _r, _g, _b, _a;

        public byte R { get { return _r; } }
        public byte G { get { return _g; } }
        public byte B { get { return _b; } }
        public byte A { get { return _a; } }

        public static ColorInt CreateNew(ColorInt oldColor, byte a)
        {
            ColorInt c = new ColorInt();
            c._r = oldColor._r;
            c._g = oldColor._g;
            c._b = oldColor._b;
            c._a = a;
            return c;
        }
        public static ColorInt FromArgb(byte a, byte r, byte g, byte b)
        {
            ColorInt c = new ColorInt();
            c._r = r;
            c._g = g;
            c._b = b;
            c._a = a;
            return c;
        }
        public static ColorInt FromArgb(int argb)
        {
            ColorInt c = new ColorInt();
            c._a = (byte)((argb >> 24));
            c._r = (byte)((argb >> 16) & 0xff);
            c._g = (byte)((argb >> 8) & 0xff);
            c._b = (byte)((argb >> 0) & 0xff);

            return c;
        }
        public static bool operator ==(ColorInt c1, ColorInt c2)
        {
            return (uint)((c1._a << 24) | (c1._r << 16) | (c1._g << 8) | (c1._b)) ==
                   (uint)((c2._a << 24) | (c2._r << 16) | (c2._g << 8) | (c2._b));
        }
        public static bool operator !=(ColorInt c1, ColorInt c2)
        {
            return (uint)((c1._a << 24) | (c1._r << 16) | (c1._g << 8) | (c1._b)) !=
                  (uint)((c2._a << 24) | (c2._r << 16) | (c2._g << 8) | (c2._b));
        }
        public override bool Equals(object obj)
        {
            if (obj is ColorInt)
            {
                ColorInt c = (ColorInt)obj;
                return c._a == this._a &&
                    c._b == this._b &&
                    c._r == this._r &&
                    c._g == this._g;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }


        /// <summary>
        /// convert to 'premultiplied alpha' and arrange to int value
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public int ToPreMultAlphaColor()
        {
            //see more at https://github.com/PaintLab/PixelFarm/issues/12
            if (_a == 0) return 0; //for premultiplied alpha => this return (0,0,0,0) NOT (r,g,b,0)
            //
            int a = _a + 1; // Add one to use mul and cheap bit shift for multiplicaltion

            return (_a << 24)
             | ((byte)((_r * a) >> 8) << 16)
             | ((byte)((_g * a) >> 8) << 8)
             | ((byte)((_b * a) >> 8));
        }
        /// <summary>
        /// check if this color is equals on another compare on RGB only, not alpha
        /// </summary>
        /// <param name="another"></param>
        /// <returns></returns>
        public bool EqualsOnRGB(ref ColorInt c2)
        {
            return (uint)((this._r << 16) | (this._g << 8) | (this._b)) ==
                (uint)((c2._r << 16) | (c2._g << 8) | (c2._b));
        }
        public bool EqualsOnRGB(int c2_r, int c2_g, int c2_b)
        {
            return (uint)((this._r << 16) | (this._g << 8) | (this._b)) ==
                (uint)((c2_r << 16) | (c2_g << 8) | (c2_b));
        }
    }

    public struct BitmapBuffer
    {
        public static readonly BitmapBuffer Empty = new BitmapBuffer();

        //in this version , only 32 bits 

        public BitmapBuffer(int w, int h, int[] orgBuffer)
        {
            this.PixelWidth = w;
            this.PixelHeight = h;
            this.Pixels = orgBuffer;
        }
        public int PixelWidth { get; private set; }
        public int PixelHeight { get; private set; }
        /// <summary>
        /// pre-multiplied alpha color pixels
        /// </summary>
        public int[] Pixels { get; private set; }

        public bool IsEmpty { get { return Pixels == null; } }
    }
}
