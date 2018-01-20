//MIT, 2015, Mauricio David
using System;
using System.Collections.Generic;
using System.Text;
using Numeria.IO;
using System.IO;



#if !NET20
using System.Threading.Tasks;
using System.Linq;
#endif
namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Test1();

        }


        static byte[] GenerateTestDataBuffer(string datastring)
        {
            return Encoding.UTF8.GetBytes(datastring.ToCharArray());
        }

        class SampleStoreRequest
        {
            public readonly string fileName;
            public readonly byte[] buffer;
            public SampleStoreRequest(string fileName, byte[] buffer)
            {
                this.fileName = fileName;
                this.buffer = buffer;
            }
        }


        static void Test1()
        {
#if NET20
            //---------------------------------------------------------------------------
            string testfile = @"d:\\WImageTest\\testdb.dat";
            //test store in the same file name
            EntryInfo en1 = FileDB.Store(testfile, "/usr/test/d1/aaaaaaaaa/bbbbbbbbb/ccccccccddddddddddddddd.a.txt", GenerateTestDataBuffer("hello!...1"));
            EntryInfo en2 = FileDB.Store(testfile, "/usr/test/d1/aaaaaaaaa/bbbbbbbbb/ccccccccddddddddddddddd.a.txt", GenerateTestDataBuffer("hello!...2"));
            EntryInfo en3 = FileDB.Store(testfile, "/usr/test/d1/aaaaaaaaa/bbbbbbbbb/ccccccccddddddddddddddd.a.txt", GenerateTestDataBuffer("hello!...3"));
            EntryInfo en4 = FileDB.Store(testfile, "/usr/test/d1/aaaaaaaaa/bbbbbbbbb/ccccccccddddddddddddddd.a.txt", GenerateTestDataBuffer("hello!...4"));
            EntryInfo en5 = FileDB.Store(testfile, "/usr/test/d1/aaaaaaaaa/bbbbbbbbb/ccccccccddddddddddddddd.a.txt", GenerateTestDataBuffer("hello!...5"));
            //---------------------------------------------------------------------------
            EntryInfo[] fileList = FileDB.ListFiles(testfile);

            using (MemoryStream ms = new MemoryStream())
            {
                //test read file and metadata
                //EntryInfo enInfo = FileDB.Read(testfile, en5.ID, ms); 

                FileDB.ReadFileContent(testfile, fileList[0], ms);
                string content = System.Text.Encoding.UTF8.GetString(ms.ToArray());

                //read only file metadata
                //EntryInfo enInfo = FileDB.ReadMetadata(testfile, en5.ID);


                ms.Close();
            }

            //foreach (var f in fileList)
            //{   
            //    FileDB.Delete(testfile, en1.ID);
            //}
            //---------------------------------------------------------------------------
            fileList = FileDB.ListFiles(testfile);
#else
            //
            // Parallel Insert
            //
            string dbFile = @"C:\Temp\MvcDemo.dat"; 
            string[] files = new string[] {
                @"C:\Temp\DSC04901.jpg", @"C:\Temp\DSC04902.jpg", @"C:\Temp\ZipFile.zip" }; 
            Parallel.For(0, 3, (i) =>
            {
                Console.WriteLine("Starting " + Path.GetFileName(files[i]));
                FileDB.Store(dbFile, files[i]);
                Console.WriteLine("Ended " + Path.GetFileName(files[i]));
            }); 
            Console.ReadLine();
#endif
        }


    }
}
