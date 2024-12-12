using System;
using Engine.Core;
using Engine.Core.Rendering;
using Engine.Core.Texturing;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Engine.Game
{
    class Game : GameScript
    {
        private Renderer renderer;
        private bool firstMove = true;

        private Vector2 lastPos;

        public Game(Renderer renderer)
        {
            this.renderer = renderer;
        }

        public override void OnLoad()
        {

        }

        public override void OnUpdate(float dt)
        {
            if (!window.IsActiveWindow)
            {
                lastPos = window.MouseState.Position;
                renderer.IsRenderMode = false;
                return;
            }

            var input = window.KeyboardState;
            var mouse = window.MouseState;

            const float cameraSpeed = 5.0f;
            const float sensitivity = 0.01f;

            renderer.RenderCamera.UpdateVectors();

            if (input.IsKeyDown(Keys.W))
            {
                renderer.RenderCamera.Position += renderer.RenderCamera.Front * cameraSpeed * dt; // Forward
            }
            if (input.IsKeyDown(Keys.S))
            {
                renderer.RenderCamera.Position -= renderer.RenderCamera.Front * cameraSpeed * dt; // Backwards
            }
            if (input.IsKeyDown(Keys.A))
            {
                renderer.RenderCamera.Position -= renderer.RenderCamera.Right * cameraSpeed * dt; // Left
            }
            if (input.IsKeyDown(Keys.D))
            {
                renderer.RenderCamera.Position += renderer.RenderCamera.Right * cameraSpeed * dt; // Right
            }
            if (input.IsKeyDown(Keys.Space))
            {
                renderer.RenderCamera.Position += Vector3.UnitZ * cameraSpeed * dt; // Up
            }
            if (input.IsKeyDown(Keys.LeftShift))
            {
                renderer.RenderCamera.Position -= Vector3.UnitZ * cameraSpeed * dt; // Down
            }

            if (input.IsKeyPressed(Keys.R))
            {
                renderer.IsRenderMode = !renderer.IsRenderMode;
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

                renderer.RenderCamera.Yaw += deltaX * sensitivity;
                renderer.RenderCamera.Pitch += deltaY * sensitivity;
            }
        }
    }
}