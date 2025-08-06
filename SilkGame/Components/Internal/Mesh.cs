using Silk.NET.OpenGL;

namespace SilkGame
{
    public class Mesh : IDisposable
    {
        public Mesh(GL gl, float[] vertices, uint[] indices, List<Texture> textures)
        {
            GL = gl;
            Vertices = vertices;
            Indices = indices;
            Textures = textures;
            SetupMesh();
        }

        public float[] Vertices { get; private set; }
        public uint[] Indices { get; private set; }
        public IReadOnlyList<Texture> Textures { get; private set; }
        public VertexArrayObject<float, uint> VAO { get; set; }
        public BufferObject<float> VBO { get; set; }
        public BufferObject<uint> EBO { get; set; }
        public GL GL { get; }

        public unsafe void SetupMesh()
        {
            EBO = new BufferObject<uint>(GL, Indices, BufferTargetARB.ElementArrayBuffer);
            VBO = new BufferObject<float>(GL, Vertices, BufferTargetARB.ArrayBuffer);
            VAO = new VertexArrayObject<float, uint>(GL, VBO, EBO);

            VAO.Bind();
            //Warning: this should be updated if the vertex structure changes in Model.BuildVertices()
            uint vertexSize = 8; // 3 for position, 2 for texture coordinates, and 3 for normals
            var offset = 0;
            var size = 3;
            VAO.VertexAttributePointer(0, size, VertexAttribPointerType.Float, vertexSize, offset);
            offset += size;
            size = 2;
            VAO.VertexAttributePointer(1, size, VertexAttribPointerType.Float, vertexSize, offset);
            offset += size;
            size = 3;
            VAO.VertexAttributePointer(2, size, VertexAttribPointerType.Float, vertexSize, offset);

            GL.BindVertexArray(0);
        }

        public void Bind()
        {
            VAO.Bind();
        }

        public void Dispose()
        {
            Textures = null;
            VAO.Dispose();
            VBO.Dispose();
            EBO.Dispose();
        }
    }
}
