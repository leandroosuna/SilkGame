using Silk.NET.OpenGL;

namespace SilkGame.Components.Internal
{
    public static class FullScreenQuad
    {
        static GL GL;

        static VertexArrayObject<float, byte> VAO { get; set; }
        static BufferObject<float> VBO { get; set; }
        static BufferObject<byte> EBO { get; set; }

        static byte[] indices;
        static float[] vertices;
        public static unsafe void Init(GL gl)
        {
            GL = gl;

            vertices = [
              -1f, -1f,  0f, 0f,   // bottom-left
               1f, -1f,  1f, 0f,   // bottom-right
               1f,  1f,  1f, 1f,   // top-right
              -1f,  1f,  0f, 1f    // top-left
            ];
            indices = [
                0, 1, 2,
                2, 3, 0
            ];

            EBO = new BufferObject<byte>(GL, indices, BufferTargetARB.ElementArrayBuffer);
            VBO = new BufferObject<float>(GL, vertices, BufferTargetARB.ArrayBuffer);
            VAO = new VertexArrayObject<float, byte>(GL, VBO, EBO);

            VAO.Bind();
            byte vertexSize = 4; // 2 for position, 2 for texture coordinates
            var offset = 0;
            var size = 2;

            VAO.VertexAttributePointer(0, size, VertexAttribPointerType.Float, vertexSize, offset);
            offset += size;
            VAO.VertexAttributePointer(1, size, VertexAttribPointerType.Float, vertexSize, offset);
            
            GL.BindVertexArray(0);
        }


        public static unsafe void Draw()
        {
            VAO.Bind();
            GL.DrawElements(PrimitiveType.Triangles, (uint)indices.Length, DrawElementsType.UnsignedByte, null);
        }
    }
}
