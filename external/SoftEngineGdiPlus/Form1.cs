using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenTK;

namespace SoftEngine
{
    public partial class Form1 : Form
    {
        Device device;
        Graphics _graphics;
        Bitmap bmp;
        Mesh mesh = new Mesh("Cube", 8, 12);
        Camera mera = new Camera();
        Texture _texture;


        //
        Texture _simpleTexture2;
        Mesh _myrectMesh;
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.panel1.Size = new Size(640, 480);
            this._graphics = this.panel1.CreateGraphics();
            device = new Device(640, 480);
            bmp = new Bitmap(640, 480);
            device.Clear(0, 0, 0, 255);
            device.DrawPoint(new OpenTK.Vector3d(0, 0, 0), new Color4(1, 1, 1, 1));
            _texture = new Texture("Suzanne.jpg", 512, 512);

            //mesh.Vertices[0] = new Vector3(-1, 1, 1);
            //mesh.Vertices[1] = new Vector3(1, 1, 1);
            //mesh.Vertices[2] = new Vector3(-1, -1, 1);
            //mesh.Vertices[3] = new Vector3(1, -1, 1);
            //mesh.Vertices[4] = new Vector3(-1, 1, -1);
            //mesh.Vertices[5] = new Vector3(1, 1, -1);
            //mesh.Vertices[6] = new Vector3(1, -1, -1);
            //mesh.Vertices[7] = new Vector3(-1, -1, -1);

            //mesh.Faces[0] = new Face { A = 0, B = 1, C = 2 };
            //mesh.Faces[1] = new Face { A = 1, B = 2, C = 3 };
            //mesh.Faces[2] = new Face { A = 1, B = 3, C = 6 };
            //mesh.Faces[3] = new Face { A = 1, B = 5, C = 6 };
            //mesh.Faces[4] = new Face { A = 0, B = 1, C = 4 };
            //mesh.Faces[5] = new Face { A = 1, B = 4, C = 5 };

            //mesh.Faces[6] = new Face { A = 2, B = 3, C = 7 };
            //mesh.Faces[7] = new Face { A = 3, B = 6, C = 7 };
            //mesh.Faces[8] = new Face { A = 0, B = 2, C = 7 };
            //mesh.Faces[9] = new Face { A = 0, B = 4, C = 7 };
            //mesh.Faces[10] = new Face { A = 4, B = 5, C = 6 };
            //mesh.Faces[11] = new Face { A = 4, B = 6, C = 7 }; 

            mera.Position = new Vector3d(0, 0, 10.0f);
            mera.Target = Vector3d.Zero;
            _meshes = device.LoadJSONFileAsync("monkey.babylon");

            //----------

            //---------------------
            //another scene 
            _simpleTexture2 = new Texture("favorites32.png", 32, 32);
            //create a simple rect 
            _myrectMesh = new Mesh("test", 4, 2);
            //_myrectMesh.Vertices[0] = new Vertex() { Coordinates = new Vector3d(-0.5, 0, 1), Normal = new Vector3d(-0.5, 0, -1), TextureCoordinates = new Vector2d(0, 1) };
            //_myrectMesh.Vertices[1] = new Vertex() { Coordinates = new Vector3d(-0.5, 0.5, 1), Normal = new Vector3d(-0.5, 0.5, -1), TextureCoordinates = new Vector2d(0, 0) };
            //_myrectMesh.Vertices[2] = new Vertex() { Coordinates = new Vector3d(0.5, 0.5, 1), Normal = new Vector3d(0.5, 0.5, -1), TextureCoordinates = new Vector2d(1, 0) };
            //_myrectMesh.Vertices[3] = new Vertex() { Coordinates = new Vector3d(0.5, 0, 1), Normal = new Vector3d(0.5, 0, -1), TextureCoordinates = new Vector2d(1, 1) };

            //_myrectMesh.Vertices[0] = new Vertex() { Coordinates = new Vector3d(-1, 0, 1), Normal = new Vector3d(0, 0, 1), TextureCoordinates = new Vector2d(0, 0) };
            //_myrectMesh.Vertices[1] = new Vertex() { Coordinates = new Vector3d(-1, 1, 1), Normal = new Vector3d(0, 0, 1), TextureCoordinates = new Vector2d(0, 1) };
            //_myrectMesh.Vertices[2] = new Vertex() { Coordinates = new Vector3d(0, 1, 1), Normal = new Vector3d(0, 0, 1), TextureCoordinates = new Vector2d(1, 1) };
            //_myrectMesh.Vertices[3] = new Vertex() { Coordinates = new Vector3d(0, 0, 1), Normal = new Vector3d(0, 0, 1), TextureCoordinates = new Vector2d(1, 0) };


            _myrectMesh.Vertices[0] = new Vertex() { Coordinates = new Vector3d(-1, 1, 1), Normal = new Vector3d(0, 0, 1), TextureCoordinates = new Vector2d(0, 1) };
            _myrectMesh.Vertices[1] = new Vertex() { Coordinates = new Vector3d(-1, -1, 1), Normal = new Vector3d(0, 0, 1), TextureCoordinates = new Vector2d(0, 0) };
            _myrectMesh.Vertices[2] = new Vertex() { Coordinates = new Vector3d(0, 1, 1), Normal = new Vector3d(0, 0, 1), TextureCoordinates = new Vector2d(1, 1) };
            _myrectMesh.Vertices[3] = new Vertex() { Coordinates = new Vector3d(0, -1, 1), Normal = new Vector3d(0, 0, 1), TextureCoordinates = new Vector2d(1, 0) };



            //each face is defined by a set of coords A,B,C
            _myrectMesh.Faces[0] = new Face { A = 0, B = 1, C = 2 };
            _myrectMesh.Faces[1] = new Face { A = 2, B = 1, C = 3 };
            //
            _myrectMesh.ComputeFaceNormals();
            _myrectMesh.Texture = _simpleTexture2;
        }
        Mesh[] _meshes;
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        private void button1_Click(object sender, EventArgs e)
        {
            UpdateOutput();
        }

        void UpdateOutput()
        {
            sw.Reset();
            sw.Start();
            mesh.Rotation = new Vector3d(mesh.Rotation.X + 0.01f, mesh.Rotation.Y + 0.01f, mesh.Rotation.Z);
            device.Clear(0, 0, 0, 255);

            foreach (Mesh mesh in _meshes)
            {
                // rotating slightly the meshes during each frame rendered
                mesh.Rotation = new Vector3d(mesh.Rotation.X, mesh.Rotation.Y + 0.1f, mesh.Rotation.Z);
            }

            //device.Render(mera, mesh);
            if (this.chkWireFrame.Checked)
            {
                device.RenderWireFrame(mera, _meshes);
            }
            else
            {
                device.Render(mera, _meshes);
            }

            var bmpdata = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            device.Present(bmpdata.Scan0);
            bmp.UnlockBits(bmpdata);


            this._graphics.Clear(Color.White);
            this._graphics.DrawImage(bmp, 0, 0);
            sw.Stop();

            //
            long ms = sw.ElapsedMilliseconds;
            this.Text = "interval: " + ms + "ms, " + (1000f / ms) + " fps";
        }
        private void chkWireFrame_CheckedChanged(object sender, EventArgs e)
        {
            UpdateOutput();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //test drawing a simple rect image on the screen


            sw.Reset();
            sw.Start();

            //-------------------------
            device.Clear(0, 0, 0, 255);

            //device.RenderWireFrame(mera, _myrectMesh);


            if (this.chkWireFrame.Checked)
            {
                device.RenderWireFrame(mera, _myrectMesh);
            }
            else
            {
                device.Render(mera, _myrectMesh);
            }
            //-------------------------

            var bmpdata = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            device.Present(bmpdata.Scan0);
            bmp.UnlockBits(bmpdata);


            this._graphics.Clear(Color.White);
            this._graphics.DrawImage(bmp, 0, 0);
            sw.Stop();

            //
            long ms = sw.ElapsedMilliseconds;
            this.Text = "interval: " + ms + "ms, " + (1000f / ms) + " fps";
        }
    }
}
