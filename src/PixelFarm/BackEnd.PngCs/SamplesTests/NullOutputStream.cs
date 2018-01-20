using System;
using System.Collections.Generic;
using System.Text;

namespace SamplesTests {
    public  class NullOutputStream : System.IO.MemoryStream {
        private int cont = 0;

        public void Write(int arg0) {
            // nothing!
            cont++;
        }

        public override void Write(byte[] b, int off, int len) {
            cont += len;
        }

        public override void WriteByte(byte b) {
            cont++;
        }

        public int getCont() {
            return cont;
        }
    }

}
