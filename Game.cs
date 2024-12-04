using System;
using Engine.Core;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

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

        private Camera camera;
        private bool firstMove = true;

        private Vector2 lastPos;
        private Vector2 mouse;

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

            camera = new Camera(Vector3.UnitZ * 3, window.Size.X / (float)window.Size.Y);
            window.CursorState = CursorState.Grabbed;
        }

        float t = 0.0f;

        public override void OnRender()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            mainShader.Use();

            int u_resolution = GL.GetUniformLocation(mainShader.Handle, "u_resolution");
            GL.Uniform2(u_resolution, (float)window.ClientSize.X, (float)window.ClientSize.Y);

            t += window.Time;

            int u_time = GL.GetUniformLocation(mainShader.Handle, "u_time");
            GL.Uniform1(u_time, t);

            Random r = new Random();
            int seed = r.Next();

            int u_seed1 = GL.GetUniformLocation(mainShader.Handle, "u_seed1");
            GL.Uniform1(u_seed1, seed);

            int u_seed2 = GL.GetUniformLocation(mainShader.Handle, "u_seed2");
            GL.Uniform1(u_seed2, seed);

            int u_pos = GL.GetUniformLocation(mainShader.Handle, "u_pos");
            GL.Uniform3(u_pos, camera.Position);

            Matrix3 rot = Matrix3.CreateRotationY(camera.Pitch) * Matrix3.CreateRotationZ(camera.Yaw);

            mainShader.SetMatrix3("u_rot", rot);

            GL.BindVertexArray(vao);
            // GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 6); // Six vertices for two triangles
            // GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.DrawElements(BeginMode.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        public override void OnUpdate() 
        { 
            if (!window.IsFocused) // Check to see if the window is focused
            {
                return;
            }

            var input = window.KeyboardState;

            const float cameraSpeed = 0.005f;
            const float sensitivity = 0.002f;

            if (input.IsKeyDown(Keys.W))
            {
                camera.Position += camera.Front * cameraSpeed * (float)t; // Forward
            }

            if (input.IsKeyDown(Keys.S))
            {
                camera.Position -= camera.Front * cameraSpeed * (float)t; // Backwards
            }
            if (input.IsKeyDown(Keys.A))
            {
                camera.Position -= camera.Right * cameraSpeed * (float)t; // Left
            }
            if (input.IsKeyDown(Keys.D))
            {
                camera.Position += camera.Right * cameraSpeed * (float)t; // Right
            }
            if (input.IsKeyDown(Keys.Space))
            {
                camera.Position += Vector3.UnitZ * cameraSpeed * (float)t; // Up
            }
            if (input.IsKeyDown(Keys.LeftShift))
            {
                camera.Position -= Vector3.UnitZ * cameraSpeed * (float)t; // Down
            }

            // Get the mouse state
            var m = window.MouseState;

            if (firstMove) // This bool variable is initially set to true.
            {
                lastPos = new Vector2(m.X, m.Y);
                firstMove = false;
            }
            else
            {
                // Calculate the offset of the mouse position
                var deltaX = m.X - lastPos.X;
                var deltaY = m.Y - lastPos.Y;
                lastPos = new Vector2(m.X, m.Y);
                mouse = new Vector2(deltaX, deltaY); 

                // Apply the camera pitch and yaw (we clamp the pitch in the camera class)
                camera.Yaw += deltaX * sensitivity;
                camera.Pitch += deltaY * sensitivity; // Reversed since y-coordinates range from bottom to top
            }
        }
    }
}