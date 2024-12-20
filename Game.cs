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
        private bool isFocused = true;

        private Vector2 lastPos;

        public Game(Core.Window? window) : base(window)
        {
            this.renderer = window.Renderer;
        }

        public override void OnLoad()
        {

        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Focus();
        }

        protected override void OnDisable()
        {
            base.OnDisable();   
            UnFocus();
        }

        public override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);

            if (!isFocused)
            {
                return;
            }

            var input = window.KeyboardState;
            var mouse = window.MouseState;

            if (input.IsKeyPressed(Keys.Escape)) UnFocus();
            if (mouse.IsButtonPressed(MouseButton.Left)) Focus();

            window.ChangeGrabMouseState(true);

            renderer.RenderCamera.UpdateVectors();

            if (input.IsKeyDown(Keys.W))
            {
                renderer.RenderCamera.Position += renderer.RenderCamera.Front * renderer.CurrentCameraSettings.CameraSpeed * dt; // Forward
            }
            if (input.IsKeyDown(Keys.S))
            {
                renderer.RenderCamera.Position -= renderer.RenderCamera.Front * renderer.CurrentCameraSettings.CameraSpeed * dt; // Backwards
            }
            if (input.IsKeyDown(Keys.A))
            {
                renderer.RenderCamera.Position -= renderer.RenderCamera.Right * renderer.CurrentCameraSettings.CameraSpeed * dt; // Left
            }
            if (input.IsKeyDown(Keys.D))
            {
                renderer.RenderCamera.Position += renderer.RenderCamera.Right * renderer.CurrentCameraSettings.CameraSpeed * dt; // Right
            }
            if (input.IsKeyDown(Keys.Space))
            {
                renderer.RenderCamera.Position += Vector3.UnitZ * renderer.CurrentCameraSettings.CameraSpeed * dt; // Up
            }
            if (input.IsKeyDown(Keys.LeftShift))
            {
                renderer.RenderCamera.Position -= Vector3.UnitZ * renderer.CurrentCameraSettings.CameraSpeed * dt; // Down
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

                renderer.RenderCamera.Yaw += deltaX * renderer.CurrentCameraSettings.Sensitivity;
                renderer.RenderCamera.Pitch += deltaY * renderer.CurrentCameraSettings.Sensitivity;
            }
        }

        private void Focus()
        {
            isFocused = true;
            lastPos = window.MouseState.Position;
            window.ChangeGrabMouseState(true);
        }

        private void UnFocus()
        {
            isFocused = false;
            lastPos = window.MouseState.Position;
            window.ChangeGrabMouseState(false);
        }
    }
}