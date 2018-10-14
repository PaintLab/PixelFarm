using Hjg.Pngcs;
using Hjg.Pngcs.Zlib;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace TestNet45 {
    
    class TestZlib {

        // this generates 2000 files!
        static public void testGenerateAll() {
            long t0 = Environment.TickCount;
            Random r = new Random();
            for (int n = 0; n < 2000; n++) {
                Stream bos = new FileStream("C:/temp/z/zlibcs" + n + ".bin", FileMode.Create);
                AZlibOutputStream ost = ZlibStreamFactory.createZlibOutputStream(bos, false);
                if (n == 0) Console.WriteLine("Using: " + ost);
                byte[] b = createBytes7(n < 50 ? n : n * n - 7);
                int offset = 0;
                while (offset < b.Length) {
                    int len = r.Next(b.Length - offset) + 1;
                    ost.Write(b, offset, len);
                    offset += len;
                }
                ost.Close();
            }
            long t1 = Environment.TickCount;
            Console.WriteLine("generated 2000 files in " + (t1 - t0));
        }




        static public void testReadall() {
            long t0 = Environment.TickCount;
            for (int n = 0; n < 2000; n++) {
                String f = "C:/temp/z/zlibcs" + n + ".bin";
                Stream bos = new FileStream(f, FileMode.Open);
                AZlibInputStream ist = ZlibStreamFactory.createZlibInputStream(bos, false);
                if (n == 0) Console.WriteLine("Using: " + ist);
                byte[] res = readAll(ist);
                long expectedlen = n < 50 ? n : n * n - 7;
                if (res.Length != expectedlen)
                    throw new Exception("error 0: " + f + " expected:" + expectedlen + " len=" + res.Length);
                for (int i = 0; i < expectedlen; i++) {
                    if ((res[i] & 0xff) != ((i * 7) & 0xff))
                        throw new Exception("error 1: " + f);
                }
                //Console.WriteLine(f + " :" + expectedlen);
            }
            long t1 = Environment.TickCount;
            Console.WriteLine("read 3000 files in " + (t1 - t0));
        }

        private static byte[] readAll(Stream ist) {
            MemoryStream bos = new MemoryStream();
            int c;
            byte[] buf = new byte[1000];
            while ((c = ist.Read(buf, 0, 1000)) > 0) {
                bos.Write(buf, 0, c);
            }
            return bos.ToArray();
        }

        /*
val= 3066839698
val= 0
val= 3799812176 t=14446
 */
        static public void testCRC32() {
            CRC32 crc1 = new CRC32();
            crc1.Update(new byte[] { 1, 2 });
            if (crc1.GetValue() != 3066839698) throw new Exception("Bad CRC32!");
            Console.WriteLine("Testing CRC32");
            Console.WriteLine("val= " + crc1.GetValue());
            crc1.Reset();
            Console.WriteLine("val= " + crc1.GetValue());
            if (crc1.GetValue() != 0) throw new Exception("Bad CRC32!!");
            Random r = new Random();
            byte[] all = new byte[2000 * 4];
            long t0 = Environment.TickCount;
            for (int n = 0; n < 2000; n++) {
                byte[] b = createBytes7(n < 50 ? n : n * n - 7);
                CRC32 crc = new CRC32();
                int offset = 0;
                while (offset < b.Length) {
                    int len = r.Next(b.Length - offset) + 1;
                    crc.Update(b, offset, len);
                    offset += len;
                }
                long x = crc.GetValue();
                all[n * 4] = (byte)((x >> 24) & 0xff);
                all[n * 4 + 1] = (byte)((x >> 16) & 0xff);
                all[n * 4 + 2] = (byte)((x >> 8) & 0xff);
                all[n * 4 + 3] = (byte)((x) & 0xff);
            }
            long t1 = Environment.TickCount;
            Adler32 a = new Adler32();
            a.Update(all);
            long v = a.GetValue();
            Console.WriteLine("val= " + v + " t=" + (t1 - t0));
            if (v != 3799812176) throw new Exception("Bad cRC32");// tested with Java CRC32
        }

        /*
val= 393220
val= 1
val= 3817105751 t=10982
         */
        static public void testAdler() {
            Console.WriteLine("Testing Adler32");
            Adler32 crc1 = new Adler32();
            crc1.Update(new byte[] { 1, 2 });
            Console.WriteLine("val= " + crc1.GetValue());
            if (crc1.GetValue() != 393220) throw new Exception("Bad Adler32!");
            crc1.Reset();
            Console.WriteLine("val= " + crc1.GetValue());
            if (crc1.GetValue() != 1) throw new Exception("Bad Adler32!!");
            Random r = new Random();
            byte[] all = new byte[2000 * 4];
            long t0 = Environment.TickCount;
            for (int n = 0; n < 2000; n++) {
                byte[] b = createBytes7(n < 50 ? n : n * n - 7);
                Adler32 crc = new Adler32();
                int offset = 0;
                while (offset < b.Length) {
                    int len = r.Next(b.Length - offset) + 1;
                    crc.Update(b, offset, len);
                    offset += len;
                }
                long x = crc.GetValue();
                all[n * 4] = (byte)((x >> 24) & 0xff);
                all[n * 4 + 1] = (byte)((x >> 16) & 0xff);
                all[n * 4 + 2] = (byte)((x >> 8) & 0xff);
                all[n * 4 + 3] = (byte)((x ) & 0xff);
            }
            long t1 = Environment.TickCount;
            Adler32 a = new Adler32();
            a.Update(all);
            long v = a.GetValue();
            Console.WriteLine("val= " + v + " t=" + (t1 - t0));
            if (v != 3817105751) throw new Exception("Bad Adler32");// tested with Java CRC32
        }

        private static byte[] createByteAscending(int n) {
            byte[] b = new byte[n];
            for (int i = 0; i < n; i++)
                b[i] = (byte)i;
            return b;
        }

        private static byte[] createBytes7(int n) {
            byte[] b = new byte[n];
            for (int i = 0; i < n; i++)
                b[i] = (byte)(i * 7);
            return b;
        }

       
    }
}
