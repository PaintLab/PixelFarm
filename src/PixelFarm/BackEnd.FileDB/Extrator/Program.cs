//MIT, 2015, Mauricio David
using System;
using System.IO;

namespace Extrator
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 0)
            {
                Console.WriteLine("Usage: Extrator <file>");
            }

            var file = args[0];
            var dir = Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file));

            if (Directory.Exists(dir))
            {
                Directory.Delete(dir);
            }
            Directory.CreateDirectory(dir);

            using (var db = new Numeria.IO.FileDB(file, FileAccess.Read))
            {
                Numeria.IO.EntryInfo[] entities = db.ListFiles();
                foreach (var entity in entities)
                {
                    var filename = UniqueFilename(dir, entity.FileUrl);
                    Console.WriteLine("Extrating.... " + filename);
                    db.Read(entity.ID, filename);
                }
            }

            Console.WriteLine("Done.");
        }

        static string UniqueFilename(string dir, string filename)
        {
            var f = Path.Combine(dir, filename);
            var index = 1;

            while (File.Exists(f))
            {
                index++;
                f = Path.Combine(dir, Path.GetFileNameWithoutExtension(filename) + " (" + index + ")" + Path.GetExtension(filename));
            }

            return f;
        }
    }
}
