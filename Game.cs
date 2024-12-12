using System;
using Engine.Core;
using Engine.Core.Texturing;
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

        private bool isRenderMode = false;

        private int renderModeNumSamples = 1024;
        private int viewModeNumSamples = 8;

        string[] skybox =
        {
                                        
            "Assets/Skybox/right.jpg",  // +X
            "Assets/Skybox/left.jpg",   // -X
            "Assets/Skybox/bottom.jpg", // +Y
            "Assets/Skybox/top.jpg",    // -Y
            "Assets/Skybox/front.jpg",  // +Z
            "Assets/Skybox/back.jpg"    // -Z
        };

        uint skyboxID;

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

            mainShader = new Shader("Assets/Shaders/PathTracing/PathTracing.vert", "Assets/Shaders/PathTracing/PathTracing.frag");
            mainShader.Use();

            // Position attribute (2 floats)
            var vertexLocation = mainShader.GetAttribLocation("aPosition");
            GL.VertexAttribPointer(vertexLocation, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.EnableVertexAttribArray(vertexLocation);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            skyboxID = Cubemap.LoadCubeMap(skybox);

            camera = new Camera(Vector3.UnitZ * 3, window.Size.X / (float)window.Size.Y);
        }

        float t = 0.0f;

        public override void OnRender()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            mainShader.Use();

            Vector2 aspect = Vector2.Normalize(window.Size);
            float numRays = 1024.0f;

            int u_resolution = GL.GetUniformLocation(mainShader.Handle, "u_resolution");
            GL.Uniform2(u_resolution, numRays * aspect.X, numRays * aspect.Y);

            t += window.Time;

            int u_time = GL.GetUniformLocation(mainShader.Handle, "u_time");
            GL.Uniform1(u_time, -60.0f);

            Random r = new Random();
            int seed1 = r.Next();
            int seed2 = r.Next();

            int u_seed1 = GL.GetUniformLocation(mainShader.Handle, "u_seed1");
            GL.Uniform1(u_seed1, seed1);

            int u_seed2 = GL.GetUniformLocation(mainShader.Handle, "u_seed2");
            GL.Uniform1(u_seed2, seed2);

            int u_pos = GL.GetUniformLocation(mainShader.Handle, "u_pos");
            GL.Uniform3(u_pos, camera.Position);

            int u_cubemap = GL.GetUniformLocation(mainShader.Handle, "u_cubemap");
            GL.Uniform1(u_cubemap, 0);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.TextureCubeMap, skyboxID);

            mainShader.SetMatrix3("u_rot", camera.Rotation);

            int samples = isRenderMode ? renderModeNumSamples : viewModeNumSamples;

            mainShader.SetInt("u_maxrefs", 16);
            mainShader.SetInt("u_maxsamples", samples);

            GL.BindVertexArray(vao);
            GL.DrawElements(BeginMode.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        public override void OnUpdate(float dt)
        {
            if (!window.IsActiveWindow)
            {
                lastPos = window.MouseState.Position;
                isRenderMode = false;
                return;
            }

            var input = window.KeyboardState;
            var mouse = window.MouseState;

            const float cameraSpeed = 5.0f;
            const float sensitivity = 0.01f;

            camera.UpdateVectors();
            if (input.IsKeyDown(Keys.W))
            {
                camera.Position += camera.Front * cameraSpeed * dt; // Forward
            }
            if (input.IsKeyDown(Keys.S))
            {
                camera.Position -= camera.Front * cameraSpeed * dt; // Backwards
            }
            if (input.IsKeyDown(Keys.A))
            {
                camera.Position -= camera.Right * cameraSpeed * dt; // Left
            }
            if (input.IsKeyDown(Keys.D))
            {
                camera.Position += camera.Right * cameraSpeed * dt; // Right
            }
            if (input.IsKeyDown(Keys.Space))
            {
                camera.Position += Vector3.UnitZ * cameraSpeed * dt; // Up
            }
            if (input.IsKeyDown(Keys.LeftShift))
            {
                camera.Position -= Vector3.UnitZ * cameraSpeed * dt; // Down
            }

            if (input.IsKeyPressed(Keys.R))
            {
                isRenderMode = !isRenderMode;
            }

            if (firstMove)
            {
                lastPos = new Vector2(mouse.X, mouse.Y);
                firstMove = false;
            }
            else
            {
                var deltaX = mouse.X - lastPos.X;
                var deltaY = mouse.Y - lastPos.Y;
                lastPos = new Vector2(mouse.X, mouse.Y);

                camera.Yaw += deltaX * sensitivity;
                camera.Pitch += deltaY * sensitivity;
            }
        }
    }
}