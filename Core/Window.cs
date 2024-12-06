using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using System;
using OpenTK.Graphics.OpenGL4;

namespace Engine.Core
{
    public class Window : GameWindow
    {
        public Action LoadAction;
        public Action<float> UpdateFrameAction;
        public Action RenderFrameAction;

        public bool IsActiveWindow { get; set; } = true;

        public float Time {get; private set;}

        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            LoadAction?.Invoke();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (!IsActiveWindow && IsFocused) IsActiveWindow = true;

            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                IsActiveWindow = false;
            }

            if (IsActiveWindow)
            {
                CursorState = CursorState.Grabbed;
            }
            else
            {
                CursorState = CursorState.Normal;
                return;
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
    }
}