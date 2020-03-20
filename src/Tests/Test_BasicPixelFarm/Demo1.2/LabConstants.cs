//BSD, 2011-2019, Gregor Aisch, Chroma.js, https://github.com/gka/chroma.js/blob/master/LICENSE
namespace ChromaJs
{
    public static class LabConsts
    {
        // Corresponds roughly to RGB brighter/darker
        public const float Kn = 18;

        //D65 standard referent
        public const float Xn = 0.950470f;
        public const float Yn = 1;
        public const float Zn = 1.088830f;

        public const float t0 = 4f / 29;
        public const float t1 = 6f / 29;
        public const float t2 = 3 * t1 * t1;
        public const float t3 = t1 * t1 * t1;

    }
}