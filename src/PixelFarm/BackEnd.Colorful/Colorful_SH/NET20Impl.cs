//MIT, 2020, WinterDev
#if PIXEL_FARM_NET20
namespace System.Runtime.CompilerServices
{
    partial class ExtensionAttribute { }
}
namespace PaintLab.Colourful
{
    public class Vector : IReadOnlyList<double>
    {
        readonly double[] _arr;
        public Vector(double[] arr)
        {
            _arr = arr;
        }
        public double this[int index] => _arr[index];
        public static implicit operator Vector(double[] input)
        {
            return new Vector(input);
        }
        public int Count => _arr.Length;
    }
    public interface IReadOnlyList<T>
    {
        T this[int index] { get; }
    }

    public class ColorList : IReadOnlyList<RGBColor>
    {
        readonly RGBColor[] _colors;
        public ColorList(RGBColor[] colors)
        {
            _colors = colors;
        }
        public RGBColor this[int index] => _colors[index];
        public static implicit operator ColorList(RGBColor[] input)
        {
            return new ColorList(input);
        }
    }

    public class Matrix
    {
        readonly double[] _matrixArr;

        public Matrix(int row, int col, double[] matrixArr)
        {
            _matrixArr = matrixArr;
            RowCount = row;
            ColCount = col;
        }
        public Matrix(Vector[] rows)
        {
            RowCount = rows.Length;
            ColCount = rows[0].Count;
            _matrixArr = new double[RowCount * ColCount];
            int i = 0;
            for (int y = 0; y < RowCount; ++y)
            {
                Vector v = rows[y];
                for (int x = 0; x < ColCount; ++x)
                {
                    _matrixArr[i] = v[x];
                    i++;
                }
            }
        }
        public int Count => RowCount;
        public int RowCount { get; }
        public int ColCount { get; }
        public double this[int row, int col] => _matrixArr[row * ColCount + col];

        public static implicit operator Matrix(Vector[] rows)
        {
            return new Matrix(rows);
        }
        public double GetCell(int row, int col)
        {
            return _matrixArr[row * ColCount + col];
        }
        public MatrixRow this[int rowId] => new MatrixRow(this, rowId);

        /// <summary>
        /// + new value to the exisiting value
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="value"></param>
        public void AddValue(int row, int col, double value)
        {
            _matrixArr[row * ColCount + col] += value;
        }
    }
    public struct MatrixRow
    {
        Matrix _owner;
        int _rowId;
        internal MatrixRow(Matrix owner, int rowId)
        {
            _owner = owner;
            _rowId = rowId;
        }
        public double this[int index] => _owner.GetCell(_rowId, index);
        public int Count => _owner.ColCount;
    }

}
#endif