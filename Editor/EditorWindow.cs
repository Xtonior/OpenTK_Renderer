using System;
using Engine;
using Engine.Core;
using Engine.Core.GUI;
using Engine.Core.Rendering;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Editor
{
    public class EditorWindow : Window
    {
        private Framebuffer viewportFramebuffer;
        private ImGuiController controller;

        public EditorWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings, WindowCycler windowCycler)
        : base(gameWindowSettings, nativeWindowSettings, windowCycler)
        {
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
            Renderer.Framebuffer.Resize(ClientSize.X, ClientSize.Y);
            controller.WindowResized(ClientSize.X, ClientSize.Y);
        }

        protected override void OnLoad()
        {
            controller = new ImGuiController(ClientSize.X, ClientSize.Y);

            Cycler.Load();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            Cycler.Update((float)e.Time);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            controller.Update(this, (float)e.Time);

            Renderer.Render();
            RenderViewport();
            Renderer.Clear();
            
            // Enable Docking
            ImGui.DockSpaceOverViewport();
            ImGui.ShowDemoWindow();

            GL.ClearColor(new Color4(0, 32, 48, 255));
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
            controller.Render();

            ImGuiController.CheckGLError("End of frame");

            SwapBuffers();
        }

        protected override void OnTextInput(TextInputEventArgs e)
        {
            controller.PressChar((char)e.Unicode);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            controller.MouseScroll(e.Offset);
        }

        private void RenderViewport()
        {
            ImGui.Begin("Scene");
            {
                Renderer.Render();
                viewportFramebuffer = Renderer.Framebuffer;
                System.Numerics.Vector2 wsize = ImGui.GetWindowSize();

                IntPtr tex = (IntPtr)viewportFramebuffer.GetFramebufferTexture();
                // viewportFramebuffer.Resize((int)wsize.X, (int)wsize.Y);
                Renderer.Clear();
                ImGui.Image(tex, wsize, new System.Numerics.Vector2(0, 1), new System.Numerics.Vector2(1, 0));
            }
            ImGui.End();
        }
    }
}