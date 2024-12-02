using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using System;

namespace Engine.Core
{
    public class Window : GameWindow
    {
        public Action LoadAction;
        public Action UpdateFrameAction;
        public Action RenderFrameAction;

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
            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            base.OnUpdateFrame(e);

            UpdateFrameAction?.Invoke();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            RenderFrameAction?.Invoke();

            SwapBuffers();
        }
    }
}