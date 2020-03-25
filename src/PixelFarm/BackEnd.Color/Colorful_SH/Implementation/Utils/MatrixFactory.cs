//MIT, 2018, Tomáš Pažourek, https://github.com/tompazourek/Colourful

namespace PaintLab.Colourful.Implementation
{
    internal static class MatrixFactory
    {
        public static Matrix CreateEmpty(int rows, int columns)
        {
            return new Matrix(rows, columns, new double[rows * columns]);
        }

        public static Matrix CreateIdentity(int size)
        {
            double[] arr = new double[size * size];

            int rowHead = 0;
            for (var i = 0; i < size; i++)
            {
                arr[rowHead + i] = 1;
                rowHead += size;
            }
            // ReSharper disable once CoVariantArrayConversion
            return new Matrix(size, size, arr);
        }

        public static Matrix CreateDiagonal(params double[] items)
        {
            var size = items.Length;

            double[] arr = new double[size * size];

            int rowHead = 0;
            for (var i = 0; i < size; i++)
            {
                arr[rowHead + i] = items[i];
                rowHead += size;
            }
            // ReSharper disable once CoVariantArrayConversion
            return new Matrix(size, size, arr);
 
        }
    }
}