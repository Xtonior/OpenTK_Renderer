using System;
using Engine.Core.Texturing;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Engine.Core.Rendering
{
    [System.Serializable]
    public class RendererException : Exception
    {
        public RendererException() { }
        public RendererException(string message) : base(message) { }
        public RendererException(string message, Exception inner) : base(message, inner) { }
        protected RendererException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    public class Renderer
    {
        public Camera? RenderCamera;
        public bool IsRenderMode = false;
        public Framebuffer Framebuffer;
        private Window? renderWindow;
        private Shader? mainShader;
        public RenderSettings CurrentRenderSettings = new RenderSettings();
        public CameraSettings CurrentCameraSettings = new CameraSettings();

        private float[] quadVertices = {
    // Positions   // Texture Coords
     1.0f,  1.0f, 1.0f, 1.0f, // Bottom-left
     1.0f, -1.0f, 1.0f, 1.0f, // Bottom-right
    -1.0f, -1.0f, 1.0f, 1.0f, // Top-left
    -1.0f,  1.0f, 1.0f, 1.0f, // Top-left
};

        private int[] indices = {
            0, 1, 3,
            1, 2, 3
        };

        private int vbo, vao, ebo;
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

        public Renderer(Window window)
        {
            renderWindow = window;

            Init();
        }

        public void Init()
        {
            if (renderWindow == null) throw new NullReferenceException("Rendering Window is not specified");

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

            // Unbind VBO and VAO
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            skyboxID = Cubemap.LoadCubeMap(skybox);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            RenderCamera = new Camera(Vector3.UnitZ * 3, renderWindow.Size.X / (float)renderWindow.Size.Y);

            Framebuffer = new Framebuffer(renderWindow.ClientSize.X, renderWindow.ClientSize.Y);
            // Framebuffer.Bind(renderWindow.ClientSize.X, renderWindow.ClientSize.Y);

            Clear();
        }

        public void Render()
        {
            if (renderWindow == null) throw new NullReferenceException("Rendering Window is not specified");
            if (mainShader == null) throw new NullReferenceException("Main Shader is not specified");
            if (RenderCamera == null) throw new NullReferenceException("Render camera is not specified");

            GL.Clear(ClearBufferMask.ColorBufferBit);

            mainShader.Use();

            Vector2 aspect = Vector2.Normalize(renderWindow.Size);
            float numRays = CurrentRenderSettings.RaysPerPixel;

            int u_resolution = GL.GetUniformLocation(mainShader.Handle, "u_resolution");
            GL.Uniform2(u_resolution, numRays * aspect.X, numRays * aspect.Y);

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
            GL.Uniform3(u_pos, RenderCamera.Position);

            int u_cubemap = GL.GetUniformLocation(mainShader.Handle, "u_cubemap");
            GL.Uniform1(u_cubemap, 0);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.TextureCubeMap, skyboxID);

            mainShader.SetMatrix3("u_rot", RenderCamera.Rotation);

            mainShader.SetInt("u_maxrefs", CurrentRenderSettings.NumRefs);
            mainShader.SetInt("u_maxsamples", CurrentRenderSettings.NumSamples);

            GL.BindVertexArray(vao);
            GL.DrawElements(BeginMode.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
        }
        
        public void Clear()
        {
            // Unbind VBO and VAO
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            // Unbind previous textures
            GL.BindTexture(TextureTarget.Texture2D, 0);
            Framebuffer.Unbind();
        }

        public void UpdateRenderSettings(RenderSettings renderSettings)
        {
            CurrentRenderSettings = renderSettings;
        }

        public void UpdateCameraSettings(CameraSettings cameraSettings)
        {
            CurrentCameraSettings = cameraSettings;
        }
    }
}