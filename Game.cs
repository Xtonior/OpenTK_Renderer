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
        -1.0f, -1.0f, 0.0f,  // Bottom-left
        1.0f, -1.0f, 0.0f,  // Bottom-right
        -1.0f,  1.0f, 0.0f,  // Top-left
        1.0f,  1.0f, 0.0f   // Top-right
        };

        int vbo, vao;

        public override void OnLoad()
        {
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();

            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, quadVertices.Length * sizeof(float), quadVertices, BufferUsageHint.StaticDraw);

            // Set vertex attribute pointers
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            mainShader = new Shader("Assets/Shaders/shader.vert", "Assets/Shaders/shader.frag");
            mainShader.Use();
        }

        public override void OnRender()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            mainShader.Use();

            int u_resolution = GL.GetUniformLocation(mainShader.Handle, "u_resolution");
            GL.Uniform2(u_resolution, 800, 600);

            int u_pos = GL.GetUniformLocation(mainShader.Handle, "u_pos");
            GL.Uniform4(u_pos, 0.0f, 0.0f, 0.0f, 0.0f); // Sphere center at (0,0) and radius 1

            GL.BindVertexArray(vao);
            GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);
        }

        public override void OnUpdate() { }
    }
}