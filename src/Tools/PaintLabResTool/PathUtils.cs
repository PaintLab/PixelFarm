//MIT, 2020, WinterDev
using System;
using System.IO;
using System.Text;
namespace Mini
{
    static class PathUtils
    {
        static readonly char[] s_pathSeps = new char[] { '/', '\\' };

        struct SimplePathWalker
        {
            string[] _splitedParts;
            public SimplePathWalker(string inputPath)
            {
                _splitedParts = inputPath.Split(s_pathSeps);
                CurrentIndex = 0;
            }
            public int SplitCount => _splitedParts.Length;
            public int CurrentIndex { get; set; }
            public string CurrentSplitPart => _splitedParts[CurrentIndex];
        }
        public static string GetAbsolutePathRelativeTo(string input, string referenceAbsPath)
        {
            if (Path.IsPathRooted(input))
            {
                return input;
            }
            //--
            if (!Path.IsPathRooted(referenceAbsPath))
            {
                throw new NotSupportedException();
            }

            StringBuilder output = new StringBuilder();
            SimplePathWalker absPaths = new SimplePathWalker(referenceAbsPath);
            SimplePathWalker inputPaths = new SimplePathWalker(input);
            //now resolve
            absPaths.CurrentIndex = absPaths.SplitCount - 1;

            for (int i = 0; i < inputPaths.SplitCount; ++i)
            {
                string currentPath = inputPaths.CurrentSplitPart;
                if (currentPath == "..")
                {
                    //step back
                    absPaths.CurrentIndex--;
                }
                else
                {
                    break;
                }
                inputPaths.CurrentIndex++;
            }
            int abs_stopAt = absPaths.CurrentIndex;
            for (int i = 0; i <= abs_stopAt; ++i)
            {
                absPaths.CurrentIndex = i;
                if (i > 0)
                {
                    output.Append(Path.DirectorySeparatorChar);
                }
                output.Append(absPaths.CurrentSplitPart);
            }
            for (int i = inputPaths.CurrentIndex; i < inputPaths.SplitCount; ++i)
            {
                output.Append(Path.DirectorySeparatorChar);
                output.Append(inputPaths.CurrentSplitPart);
                inputPaths.CurrentIndex++;
            }

            return output.ToString();
        }
    }
}