using System;
using Engine.Core;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;

namespace Engine.Game
{
    class Game : GameScript
    {
        private Shader? mainShader;

        float[] quadVertices = {
    // Positions   // Texture Coords
     1.0f,  1.0f, 1.0f, 1.0f, // Bottom-left
     1.0f, -1.0f, 1.0f, 1.0f, // Bottom-right
    -1.0f, -1.0f, 1.0f, 1.0f, // Top-left
    -1.0f,  1.0f, 1.0f, 1.0f, // Top-left
};

        int[] indices = {
            0, 1, 3,
            1, 2, 3
        };

        int vbo, vao, ebo;

        public override void OnLoad()
        {
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();
            ebo = GL.GenBuffer();

            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, quadVertices.Length * sizeof(float), quadVertices, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(int), indices, BufferUsageHint.StaticDraw);

            mainShader = new Shader("Assets/Shaders/shader.vert", "Assets/Shaders/shader.frag");
            mainShader.Use();

            // Position attribute (2 floats)
            var vertexLocation = mainShader.GetAttribLocation("aPosition");
            GL.VertexAttribPointer(vertexLocation, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.EnableVertexAttribArray(vertexLocation);

            // Texture coordinate attribute (2 floats)
            var texCoordLocation = mainShader.GetAttribLocation("aTexCoord");
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));
            GL.EnableVertexAttribArray(texCoordLocation);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }

        public override void OnRender()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            mainShader.Use();

            int u_resolution = GL.GetUniformLocation(mainShader.Handle, "u_resolution");
            GL.Uniform2(u_resolution, 800f, 600f);

            GL.BindVertexArray(vao);
            // GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 6); // Six vertices for two triangles
            // GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.DrawElements(BeginMode.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        public override void OnUpdate() { }
    }
}