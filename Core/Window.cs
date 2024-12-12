using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Engine.Core
{
    public class Window : GameWindow
    {
        public Action LoadAction;
        public Action<float> UpdateFrameAction;
        public Action RenderFrameAction;

        public bool IsActiveWindow { get; set; } = true;
        public bool AutoHideMouse { get; set; } = true;

        public float Time { get; private set; }

        private Vector2 lastMousePos = Vector2.Zero;

        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            VSync = VSyncMode.Off;
            LoadAction?.Invoke();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (AutoHideMouse)
            {
                if (KeyboardState.IsKeyPressed(Keys.Escape))
                {
                    ChangeActive(false);
                }

                if (MouseState.IsButtonPressed(MouseButton.Left))
                {
                    ChangeActive(true);
                }
            }

            base.OnUpdateFrame(e);

            UpdateFrameAction?.Invoke((float)e.Time);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            RenderFrameAction?.Invoke();

            Time = (float)e.Time;

            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Size.X, Size.Y);
        }

        private void ChangeActive(bool active)
        {
            switch (active)
            {
                case true:
                    lastMousePos = MousePosition;
                    IsActiveWindow = true;
                    CursorState = CursorState.Grabbed;
                    break;
                case false:
                    MousePosition = lastMousePos;
                    IsActiveWindow = false;
                    CursorState = CursorState.Normal;
                    break;
            }
        }
    }
}