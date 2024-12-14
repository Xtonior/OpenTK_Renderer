using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Engine.Core
{
    public class EngineWindow : Window
    {
        public float Time { get; private set; }

        public EngineWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings, WindowCycler windowCycler) 
        : base(gameWindowSettings, nativeWindowSettings, windowCycler)
        {
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            VSync = VSyncMode.Off;
            Cycler.Load();

            AutoHideMouse = true;
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (AutoHideMouse)
            {
                if (KeyboardState.IsKeyPressed(Keys.Escape))
                {
                    ChangeGrabMouseState(false);
                }

                if (MouseState.IsButtonPressed(MouseButton.Left))
                {
                    ChangeGrabMouseState(true);
                }
            }

            base.OnUpdateFrame(e);

            Cycler.Update((float)e.Time);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            Renderer.Render();
            Cycler.Render();
            Renderer.Clear();

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