//2013, David Rousset, https://www.davrous.com
using OpenTK;
using SharpDX;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SoftEngine
{
    public class Device
    {
        private int[] backBuffer;
        private BitmapBuffer bmp;
        private readonly double[] depthBuffer;

        public Device(int w, int h)
        {

            // the back buffer size is equal to the number of pixels to draw
            // on screen (width*height) * 4 (R,G,B & Alpha values). 
            backBuffer = new int[w * h];
            bmp = new BitmapBuffer(w, h, backBuffer);
            depthBuffer = new double[w * h];
        }
        // Called to put a pixel on screen at a specific X,Y coordinates
        public void PutPixel(int x, int y, double z, Color4 color)
        {
            int index = (x + y * bmp.PixelWidth);
            if (depthBuffer[index] < z)
            {
                return; // Discard
            }

            depthBuffer[index] = z;

            // As we have a 1-D Array for our back buffer
            // we need to know the equivalent cell in 1-D based
            // on the 2D coordinates on screen

            int totalColor = ((byte)(color.Alpha * 255) << 24) | ((byte)(color.Red * 255) << 16) | ((byte)(color.Green * 255) << 8) | (byte)(color.Blue * 255);
            backBuffer[index] = totalColor;

            //backBuffer[index] = (byte)(color.Blue * 255);
            //backBuffer[index + 1] = (byte)(color.Green * 255);
            //backBuffer[index + 2] = (byte)(color.Red * 255);
            //backBuffer[index + 3] = (byte)(color.Alpha * 255);
        }
        // This method is called to clear the back buffer with a specific color
        public void Clear(byte r, byte g, byte b, byte a)
        {
            int len = backBuffer.Length;
            int totalColor = (a << 24) | (r << 16) | (g << 8) | b;
            for (int index = 0; index < len; index++)
            {
                // BGRA is used by Windows instead by RGBA in HTML5
                backBuffer[index] = totalColor;

                //backBuffer[index] = b;
                //backBuffer[index + 1] = g;
                //backBuffer[index + 2] = r;
                //backBuffer[index + 3] = a;
            }

            // Clearing Depth Buffer
            for (int index = 0; index < len; index++)
            {
                depthBuffer[index] = double.MaxValue;
            }
        }

        // Once everything is ready, we can flush the back buffer
        // into the front buffer. 

        public void Present(IntPtr targetBuffer)
        {
            System.Runtime.InteropServices.Marshal.Copy(backBuffer, 0, targetBuffer, backBuffer.Length);
            //using (var stream = bmp.PixelBuffer.AsStream())
            //{
            //    // writing our byte[] back buffer into our WriteableBitmap stream
            //    stream.Write(backBuffer, 0, backBuffer.Length);
            //}
            //// request a redraw of the entire bitmap
            //bmp.Invalidate();
        }
        public void DrawBline(Vector3d point0, Vector3d point1)
        {
            //DrawBline is added in part2

            int x0 = (int)point0.X;
            int y0 = (int)point0.Y;
            int x1 = (int)point1.X;
            int y1 = (int)point1.Y;

            var dx = Math.Abs(x1 - x0);
            var dy = Math.Abs(y1 - y0);
            var sx = (x0 < x1) ? 1 : -1;
            var sy = (y0 < y1) ? 1 : -1;
            var err = dx - dy;

            while (true)
            {
                DrawPoint(new Vector3d(x0, y0, 0), new Color4(1, 1, 1, 1));

                if ((x0 == x1) && (y0 == y1)) break;
                var e2 = 2 * err;
                if (e2 > -dy) { err -= dy; x0 += sx; }
                if (e2 < dx) { err += dx; y0 += sy; }
            }
        }



        // DrawPoint calls PutPixel but does the clipping operation before
        public void DrawPoint(Vector3d point, Color4 color)
        {
            // Clipping what's visible on screen
            if (point.X >= 0 && point.Y >= 0 &&
                point.X < bmp.PixelWidth &&
                point.Y < bmp.PixelHeight)
            {
                // Drawing a yellow point
                PutPixel((int)point.X, (int)point.Y, point.Z, color);
            }
        }
        // Clamping values to keep them between 0 and 1
        static double Clamp(double value, double min = 0, double max = 1)
        {
            return Math.Max(min, Math.Min(value, max));
        }

        // Interpolating the value between 2 vertices 
        // min is the starting point, max the ending point
        // and gradient the % between the 2 points
        static double Interpolate(double min, double max, double gradient)
        {
            return min + (max - min) * Clamp(gradient);
        }
        public Vertex Project(Vertex vertex, Matrix4d transMat, Matrix4d world)
        {
            // transforming the coordinates into 2D space
            Vector3d point;
            Vector3d.Transform(ref vertex.Coordinates, ref transMat, out point);
            // transforming the coordinates & the normal to the vertex in the 3D world

            Vector3d point3dWorld;
            Vector3d.Transform(ref vertex.Coordinates, ref world, out point3dWorld);


            Vector3d normal3dWorld;
            Vector3d.Transform(ref vertex.Normal, ref world, out normal3dWorld);


            // The transformed coordinates will be based on coordinate system
            // starting on the center of the screen. But drawing on screen normally starts
            // from top left. We then need to transform them again to have x:0, y:0 on top left.
            double x = point.X * bmp.PixelWidth + bmp.PixelWidth / 2.0f;
            double y = -point.Y * bmp.PixelHeight + bmp.PixelHeight / 2.0f;

            return new Vertex
            {
                Coordinates = new Vector3d(x, y, point.Z),
                Normal = normal3dWorld,
                WorldCoordinates = point3dWorld,
                TextureCoordinates = vertex.TextureCoordinates
            };
        }
        // Compute the cosine of the angle between the light vector and the normal vector
        // Returns a value between 0 and 1
        static double ComputeNDotL(Vector3d vertex, Vector3d normal, Vector3d lightPosition)
        {
            Vector3d lightDirection = lightPosition - vertex;

            normal.Normalize();
            lightDirection.Normalize();

            return Math.Max(0, Vector3d.Dot(normal, lightDirection));
        }
        // drawing line between 2 points from left to right
        // papb -> pcpd
        // pa, pb, pc, pd must then be sorted before
        void ProcessScanLine(ScanLineData data, Vertex va, Vertex vb, Vertex vc, Vertex vd, Color4 color, Texture texture)
        {
            Vector3d pa = va.Coordinates;
            Vector3d pb = vb.Coordinates;
            Vector3d pc = vc.Coordinates;
            Vector3d pd = vd.Coordinates;

            // Thanks to current Y, we can compute the gradient to compute others values like
            // the starting X (sx) and ending X (ex) to draw between
            // if pa.Y == pb.Y or pc.Y == pd.Y, gradient is forced to 1
            double gradient1 = pa.Y != pb.Y ? (data.currentY - pa.Y) / (pb.Y - pa.Y) : 1;
            double gradient2 = pc.Y != pd.Y ? (data.currentY - pc.Y) / (pd.Y - pc.Y) : 1;

            int sx = (int)Interpolate(pa.X, pb.X, gradient1);
            int ex = (int)Interpolate(pc.X, pd.X, gradient2);

            // starting Z & ending Z
            double z1 = Interpolate(pa.Z, pb.Z, gradient1);
            double z2 = Interpolate(pc.Z, pd.Z, gradient2);


            // Interpolating normals on Y
            double snl = Interpolate(data.ndotla, data.ndotlb, gradient1);
            double enl = Interpolate(data.ndotlc, data.ndotld, gradient2);

            // Interpolating texture coordinates on Y
            double su = Interpolate(data.ua, data.ub, gradient1);
            double eu = Interpolate(data.uc, data.ud, gradient2);
            double sv = Interpolate(data.va, data.vb, gradient1);
            double ev = Interpolate(data.vc, data.vd, gradient2);




            // drawing a line from left (sx) to right (ex) 
            for (int x = sx; x < ex; x++)
            {
                double gradient = (x - sx) / (double)(ex - sx);

                double z = Interpolate(z1, z2, gradient);
                //double ndotl = data.ndotla; //flat shader
                double ndotl = Interpolate(snl, enl, gradient); //for gradient 
                double u = Interpolate(su, eu, gradient);
                double v = Interpolate(sv, ev, gradient);

                //// changing the color value using the cosine of the angle
                //// between the light vector and the normal vector
                //var c1 = new Color4((float)(color.r * ndotl),
                //         (float)(color.g * ndotl),
                //         (float)(color.b * ndotl),
                //         (float)(1));
                //DrawPoint(new Vector3d(x, data.currentY, z), c1);

                Color4 textureColor;

                if (texture != null)
                    textureColor = texture.Map(u, v);
                else
                    textureColor = new Color4(1, 1, 1, 1);

                // changing the native color value using the cosine of the angle
                // between the light vector and the normal vector
                // and the texture color

                DrawPoint(new Vector3d(x, data.currentY, z), new Color4(
                  (float)(color.r * ndotl * textureColor.r),
                  (float)(color.g * ndotl * textureColor.g),
                  (float)(color.b * ndotl * textureColor.b),
                  1
                ));
            }
        }
        public void DrawBline(Vector2 point0, Vector2 point1)
        {
            int x0 = (int)point0.X;
            int y0 = (int)point0.Y;
            int x1 = (int)point1.X;
            int y1 = (int)point1.Y;

            var dx = Math.Abs(x1 - x0);
            var dy = Math.Abs(y1 - y0);
            var sx = (x0 < x1) ? 1 : -1;
            var sy = (y0 < y1) ? 1 : -1;
            var err = dx - dy;

            Color4 color = new Color4(1, 1, 0, 1);
            while (true)
            {
                DrawPoint(new Vector3d(x0, y0, 1), color);

                if ((x0 == x1) && (y0 == y1)) break;
                var e2 = 2 * err;
                if (e2 > -dy) { err -= dy; x0 += sx; }
                if (e2 < dx) { err += dx; y0 += sy; }
            }
        }
        public void DrawTriangle(Vertex v1, Vertex v2, Vertex v3, Color4 color, Texture texture)
        {
            // Sorting the points in order to always have this order on screen p1, p2 & p3
            // with p1 always up (thus having the Y the lowest possible to be near the top screen)
            // then p2 between p1 & p3
            if (v1.Coordinates.Y > v2.Coordinates.Y)
            {
                Vertex temp = v2;
                v2 = v1;
                v1 = temp;
            }

            if (v2.Coordinates.Y > v3.Coordinates.Y)
            {
                Vertex temp = v2;
                v2 = v3;
                v3 = temp;
            }

            if (v1.Coordinates.Y > v2.Coordinates.Y)
            {
                Vertex temp = v2;
                v2 = v1;
                v1 = temp;
            }

            Vector3d p1 = v1.Coordinates;
            Vector3d p2 = v2.Coordinates;
            Vector3d p3 = v3.Coordinates;

            // normal face's vector is the average normal between each vertex's normal
            // computing also the center point of the face
            //Vector3d vnFace = (v1.Normal + v2.Normal + v3.Normal) / 3.0;
            //Vector3d centerPoint = (v1.WorldCoordinates + v2.WorldCoordinates + v3.WorldCoordinates) / 3.0;

            // Light position 
            Vector3d lightPos = new Vector3d(0, 10, 10);
            // computing the cos of the angle between the light vector and the normal vector
            // it will return a value between 0 and 1 that will be used as the intensity of the color

            //double ndotl = ComputeNDotL(centerPoint, vnFace, lightPos);


            // computing the cos of the angle between the light vector and the normal vector
            // it will return a value between 0 and 1 that will be used as the intensity of the color
            double nl1 = ComputeNDotL(v1.WorldCoordinates, v1.Normal, lightPos);
            double nl2 = ComputeNDotL(v2.WorldCoordinates, v2.Normal, lightPos);
            double nl3 = ComputeNDotL(v3.WorldCoordinates, v3.Normal, lightPos);

            var data = new ScanLineData();

            // computing lines' directions
            double dP1P2, dP1P3;

            // http://en.wikipedia.org/wiki/Slope
            // Computing slopes
            if (p2.Y - p1.Y > 0)
                dP1P2 = (p2.X - p1.X) / (p2.Y - p1.Y);
            else
                dP1P2 = 0;

            if (p3.Y - p1.Y > 0)
                dP1P3 = (p3.X - p1.X) / (p3.Y - p1.Y);
            else
                dP1P3 = 0;

            // First case where triangles are like that:
            // P1
            // -
            // -- 
            // - -
            // -  -
            // -   - P2
            // -  -
            // - -
            // -
            // P3
            if (dP1P2 > dP1P3)
            {
                for (int y = (int)p1.Y; y <= (int)p3.Y; y++)
                {
                    data.currentY = y;

                    if (y < p2.Y)
                    {
                        data.ndotla = nl1;
                        data.ndotlb = nl3;
                        data.ndotlc = nl1;
                        data.ndotld = nl2;

                        data.ua = v1.TextureCoordinates.X;
                        data.ub = v3.TextureCoordinates.X;
                        data.uc = v1.TextureCoordinates.X;
                        data.ud = v2.TextureCoordinates.X;

                        data.va = v1.TextureCoordinates.Y;
                        data.vb = v3.TextureCoordinates.Y;
                        data.vc = v1.TextureCoordinates.Y;
                        data.vd = v2.TextureCoordinates.Y;
                        ProcessScanLine(data, v1, v3, v1, v2, color, texture);
                    }
                    else
                    {
                        data.ndotla = nl1;
                        data.ndotlb = nl3;
                        data.ndotlc = nl2;
                        data.ndotld = nl3;

                        data.ua = v1.TextureCoordinates.X;
                        data.ub = v3.TextureCoordinates.X;
                        data.uc = v2.TextureCoordinates.X;
                        data.ud = v3.TextureCoordinates.X;

                        data.va = v1.TextureCoordinates.Y;
                        data.vb = v3.TextureCoordinates.Y;
                        data.vc = v2.TextureCoordinates.Y;
                        data.vd = v3.TextureCoordinates.Y;
                        ProcessScanLine(data, v1, v3, v2, v3, color, texture);
                    }
                }
            }
            // First case where triangles are like that:
            //       P1
            //        -
            //       -- 
            //      - -
            //     -  -
            // P2 -   - 
            //     -  -
            //      - -
            //        -
            //       P3
            else
            {
                for (int y = (int)p1.Y; y <= (int)p3.Y; y++)
                {
                    data.currentY = y;

                    if (y < p2.Y)
                    {
                        data.ndotla = nl1;
                        data.ndotlb = nl2;
                        data.ndotlc = nl1;
                        data.ndotld = nl3;

                        data.ua = v1.TextureCoordinates.X;
                        data.ub = v2.TextureCoordinates.X;
                        data.uc = v1.TextureCoordinates.X;
                        data.ud = v3.TextureCoordinates.X;

                        data.va = v1.TextureCoordinates.Y;
                        data.vb = v2.TextureCoordinates.Y;
                        data.vc = v1.TextureCoordinates.Y;
                        data.vd = v3.TextureCoordinates.Y;
                        ProcessScanLine(data, v1, v2, v1, v3, color, texture);
                    }
                    else
                    {
                        data.ndotla = nl2;
                        data.ndotlb = nl3;
                        data.ndotlc = nl1;
                        data.ndotld = nl3;

                        data.ua = v2.TextureCoordinates.X;
                        data.ub = v3.TextureCoordinates.X;
                        data.uc = v1.TextureCoordinates.X;
                        data.ud = v3.TextureCoordinates.X;

                        data.va = v2.TextureCoordinates.Y;
                        data.vb = v3.TextureCoordinates.Y;
                        data.vc = v1.TextureCoordinates.Y;
                        data.vd = v3.TextureCoordinates.Y;
                        ProcessScanLine(data, v2, v3, v1, v3, color, texture);
                    }
                }
            }
        }
        public void RenderWireFrame(Camera camera, params Mesh[] meshes)
        {
            // To understand this part, please read the prerequisites resources
            Matrix4d viewMatrix = Matrix.LookAtLHd(camera.Position, camera.Target, Vector3d.UnitY);
            //Matrix4d projectionMatrix = Matrix.PerspectiveFovLHd(0.78f,
            //                                    (double)bmp.PixelWidth / (double)bmp.PixelHeight,
            //                                    0.01, 1.0);

            //test orthogonal view
            Matrix4d projectionMatrix;
            Matrix.OrthoOffCenterLHd(-2, 2, -2, 2, 0.01f, 1, out projectionMatrix);

            foreach (Mesh mesh in meshes)
            {
                // Beware to apply rotation before translation 
                Vector3d pos = mesh.Position;
                Matrix4d worldMatrix = Matrix4d.RotationYawPitchRoll(mesh.Rotation.Y, mesh.Rotation.X, mesh.Rotation.Z) *
                                  Matrix4d.CreateTranslation(pos.X, pos.Y, pos.Z);
                Matrix4d worldView = worldMatrix * viewMatrix;
                Matrix4d transformMatrix = worldMatrix * viewMatrix * projectionMatrix;


                foreach (Face face in mesh.Faces)
                {
                    // Face-back culling
                    //var transformedNormal = Vector3.TransformNormals(face.Normal, worldView);

                    //if (transformedNormal.Z >= 0)
                    //{
                    //    continue;
                    //}

                    Vertex vertexA = mesh.Vertices[face.A];
                    Vertex vertexB = mesh.Vertices[face.B];
                    Vertex vertexC = mesh.Vertices[face.C];

                    Vertex pixelA = this.Project(vertexA, transformMatrix, worldMatrix);
                    Vertex pixelB = this.Project(vertexB, transformMatrix, worldMatrix);
                    Vertex pixelC = this.Project(vertexC, transformMatrix, worldMatrix);


                    DrawBline(pixelA.Coordinates, pixelB.Coordinates);
                    DrawBline(pixelB.Coordinates, pixelC.Coordinates);
                    DrawBline(pixelC.Coordinates, pixelA.Coordinates);

                    //float color = 1.0f;
                    //this.DrawTriangle(pixelA, pixelB, pixelC, new Color4(color, color, color, 1), mesh.Texture);
                }
            }

        }
        // The main method of the engine that re-compute each vertex projection
        // during each frame
        public void Render(Camera camera, params Mesh[] meshes)
        {
            // To understand this part, please read the prerequisites resources
            Matrix4d viewMatrix = Matrix.LookAtLHd(camera.Position, camera.Target, Vector3d.UnitY);
            //Matrix4d projectionMatrix = Matrix.PerspectiveFovLHd(0.78f,
            //                                    (double)bmp.PixelWidth / (double)bmp.PixelHeight,
            //                                    0.01, 1.0);

            //test orthogonal view
            Matrix4d projectionMatrix;
            Matrix.OrthoOffCenterLHd(-2, 2, -2, 2, 0.01f, 1, out projectionMatrix);

            foreach (Mesh mesh in meshes)
            {
                // Beware to apply rotation before translation 
                Vector3d pos = mesh.Position;
                Matrix4d worldMatrix = Matrix4d.RotationYawPitchRoll(mesh.Rotation.Y, mesh.Rotation.X, mesh.Rotation.Z) *
                                  Matrix4d.CreateTranslation(pos.X, pos.Y, pos.Z);
                Matrix4d worldView = worldMatrix * viewMatrix;
                Matrix4d transformMatrix = worldMatrix * viewMatrix * projectionMatrix;


                foreach (Face face in mesh.Faces)
                {
                    // Face-back culling
                    var transformedNormal = Vector3d.TransformNormal(face.Normal, worldView);

                    if (transformedNormal.Z >= 0)
                    {
                        continue;
                    }

                    Vertex vertexA = mesh.Vertices[face.A];
                    Vertex vertexB = mesh.Vertices[face.B];
                    Vertex vertexC = mesh.Vertices[face.C];

                    Vertex pixelA = this.Project(vertexA, transformMatrix, worldMatrix);
                    Vertex pixelB = this.Project(vertexB, transformMatrix, worldMatrix);
                    Vertex pixelC = this.Project(vertexC, transformMatrix, worldMatrix);

                    float color = 1.0f;
                    this.DrawTriangle(pixelA, pixelB, pixelC, new Color4(color, color, color, 1), mesh.Texture);
                }
            }
        }


        class MyMesh
        {
            public string name;
            public string id;
            public double[] position;
            public double[] vertices;
            public int[] indices;
            public int uvCount;
            public string materialId;
        }
        class MyMeshCollection
        {
            public List<MyMesh> meshes;
            public List<MyMaterial> materials;
        }
        class MyMaterial
        {

            public string name;
            public string id;
            public double[] ambient;
            public double[] diffuse;
            public double[] specular;
            public bool backFaceCulling;
            public DiffuseTexture diffuseTexture;
            public string DiffuseTextureName;
        }
        class DiffuseTexture
        {
            public string name;

        }
        public Mesh[] LoadJSONFileAsync(string fileName)
        {
            string data = System.IO.File.ReadAllText(fileName);
            var meshes = new List<Mesh>();
            JToken jtoken = Newtonsoft.Json.JsonConvert.DeserializeObject(data) as JToken;
            MyMeshCollection myMesh = jtoken.ToObject<MyMeshCollection>();


            List<Mesh> meshList = new List<Mesh>();

            List<MyMaterial> materials = myMesh.materials;

            Dictionary<string, MyMaterial> material_dic = new Dictionary<string, MyMaterial>();
            int meshCount = myMesh.meshes.Count;

            for (int materialIndex = 0; materialIndex < materials.Count; materialIndex++)
            {
                MyMaterial myMaterial = materials[materialIndex];

                if (myMaterial.diffuseTexture != null)
                {
                    myMaterial.DiffuseTextureName = (myMaterial.diffuseTexture.name);
                }


                material_dic.Add(myMaterial.id, myMaterial);
            }

            for (int meshIndex = 0; meshIndex < meshCount; ++meshIndex)
            {
                MyMesh mymesh = myMesh.meshes[meshIndex];
                double[] verticesArray = mymesh.vertices;
                int[] indicesArray = mymesh.indices;
                double[] position = mymesh.position;

                int uvCount = mymesh.uvCount;
                int verticesStep = 1;
                switch (uvCount)
                {
                    case 0:
                        verticesStep = 6;
                        break;
                    case 1:
                        verticesStep = 8;
                        break;
                    case 2:
                        verticesStep = 10;
                        break;
                }
                // the number of interesting vertices information for us
                int verticesCount = verticesArray.Length / verticesStep;
                // number of faces is logically the size of the array divided by 3 (A, B, C)
                int facesCount = indicesArray.Length / 3;


                Mesh mesh = new Mesh(mymesh.name, verticesCount, facesCount);
                meshList.Add(mesh);
                // Filling the Vertices array of our mesh first
                for (var index = 0; index < verticesCount; index++)
                {
                    double x = verticesArray[index * verticesStep];
                    double y = verticesArray[index * verticesStep + 1];
                    double z = verticesArray[index * verticesStep + 2];
                    // Loading the vertex normal exported by Blender
                    double nx = verticesArray[index * verticesStep + 3];
                    double ny = verticesArray[index * verticesStep + 4];
                    double nz = verticesArray[index * verticesStep + 5];

                    if (uvCount > 0)
                    {   // Loading the texture coordinates
                        double u = verticesArray[index * verticesStep + 6];
                        double v = verticesArray[index * verticesStep + 7];
                        mesh.Vertices[index] = new Vertex
                        {
                            Coordinates = new Vector3d(x, y, z),
                            Normal = new Vector3d(nx, ny, nz),
                            TextureCoordinates = new Vector2d(u, v)
                        };
                    }
                    else
                    {   //no uv
                        mesh.Vertices[index] = new Vertex
                        {
                            Coordinates = new Vector3d(x, y, z),
                            Normal = new Vector3d(nx, ny, nz)
                        };
                    }
                }


                // Then filling the Faces array
                for (var index = 0; index < facesCount; index++)
                {
                    int a = indicesArray[index * 3];
                    int b = indicesArray[index * 3 + 1];
                    int c = indicesArray[index * 3 + 2];
                    mesh.Faces[index] = new Face { A = a, B = b, C = c };
                }

                // Getting the position you've set in Blender 
                mesh.Position = new Vector3d(position[0], position[1], position[2]);


                if (uvCount > 0)
                {
                    // Texture 
                    string meshTextureID = mymesh.materialId;
                    string meshTextureName = material_dic[meshTextureID].DiffuseTextureName;
                    mesh.Texture = new Texture(meshTextureName, 512, 512);
                }
                mesh.ComputeFaceNormals();
            }

            return meshList.ToArray();

        }
    }
}
