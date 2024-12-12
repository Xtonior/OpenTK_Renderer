using System;
using Engine.Core;
using Engine.Core.GUI;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Editor
{
    public class EditorWindow : Window
    {
        private ImGuiController controller;

        public EditorWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        public void Init()
        {
            Load += OnLoaded;
            UpdateFrame += OnUpdated;
            RenderFrame += OnRendered;
            Resize += OnResized;
            TextInput += OnTextInputed;
            MouseWheel += OnMouseWheel;

            AutoHideMouse = false;
        }

        private void OnResized(ResizeEventArgs e)
        {
            controller.WindowResized(ClientSize.X, ClientSize.Y);
        }

        private void OnLoaded()
        {
            controller = new ImGuiController(ClientSize.X, ClientSize.Y);
        }

        private void OnUpdated(FrameEventArgs e) {   }

        private void OnRendered(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            controller.Update(this, (float)e.Time);

            GL.ClearColor(new Color4(0, 32, 48, 255));
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            // Enable Docking
            ImGui.DockSpaceOverViewport();

            ImGui.ShowDemoWindow();

            controller.Render();

            ImGuiController.CheckGLError("End of frame");

            SwapBuffers();
        }

        private void OnTextInputed(TextInputEventArgs e)
        {
            controller.PressChar((char)e.Unicode);
        }

        private void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            
            controller.MouseScroll(e.Offset);
        }
    }
}