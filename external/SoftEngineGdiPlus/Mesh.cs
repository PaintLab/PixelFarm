//2013, David Rousset, https://www.davrous.com
// Mesh.cs
using OpenTK;

namespace SoftEngine
{
    public struct Vertex
    {
        public Vector3d Normal;
        public Vector3d Coordinates;
        public Vector3d WorldCoordinates;
        public Vector2d TextureCoordinates;
    }
    public class Mesh
    {
        public string Name { get; set; }
        public Vertex[] Vertices { get; private set; }
        public Face[] Faces { get; set; }
        public Vector3d Position { get; set; }
        public Vector3d Rotation { get; set; }
        public Texture Texture { get; set; }

        public Mesh(string name, int verticesCount, int facesCount)
        {
            Vertices = new Vertex[verticesCount];
            Faces = new Face[facesCount];
            Name = name;
        }
        public void ComputeFaceNormals()
        {
            int j = Faces.Length;
            int faceIndex = 0;
            for (int i = 0; i < j; ++i)
            {
                var face = Faces[faceIndex];
                var vertexA = Vertices[face.A];
                var vertexB = Vertices[face.B];
                var vertexC = Vertices[face.C];

                Faces[faceIndex].Normal = (vertexA.Normal + vertexB.Normal + vertexC.Normal) / 3.0f;
                Faces[faceIndex].Normal.Normalize();

                faceIndex++;
            } 
        }
    }

    public struct Face
    {
        public int A;
        public int B;
        public int C;

        public Vector3d Normal;
    }
}
