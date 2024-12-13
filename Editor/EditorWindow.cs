using System;
using System.Collections.Generic;
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

        private List<string> renderSettingNames = new List<string>();
        private List<RenderSettings> renderSettings = new List<RenderSettings>();
        private RenderSettings fastRenderSettings = new RenderSettings();
        private RenderSettings midRenderSettings = new RenderSettings();
        private RenderSettings highRenderSettings = new RenderSettings();
        private RenderSettings currentRenderSettings = new RenderSettings();
        private CameraSettings cameraSettings = new CameraSettings();

        int currentRenderSettingsID = 0;

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
            viewportFramebuffer = Renderer.Framebuffer;

            Cycler.Load();

            fastRenderSettings.NumSamples = 6;
            // fastRenderSettings.RaysPerPixel = 1024;

            midRenderSettings.NumSamples = 128;
            // midRenderSettings.RaysPerPixel = 2048;

            highRenderSettings.NumSamples = 256;

            renderSettings.Add(fastRenderSettings);
            renderSettings.Add(midRenderSettings);
            renderSettings.Add(highRenderSettings);

            renderSettingNames.Add("Low");
            renderSettingNames.Add("Mid");
            renderSettingNames.Add("High");
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            Cycler.Update((float)e.Time);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.ClearColor(new Color4(0, 32, 48, 255));
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            controller.Update(this, (float)e.Time);

            // Enable Docking
            ImGui.DockSpaceOverViewport();

            viewportFramebuffer.Bind(ClientSize.X, ClientSize.Y);
            Renderer.Render();
            RenderViewport();
            Renderer.Clear();

            RenderPropertiesPanel();

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
                IntPtr framebufferTexture = (IntPtr)viewportFramebuffer.GetFramebufferTexture();

                System.Numerics.Vector2 viewportSize = ImGui.GetContentRegionAvail();
                float framebufferAspect = (float)viewportFramebuffer.Size.X / viewportFramebuffer.Size.Y;
                float viewportAspect = viewportSize.X / viewportSize.Y;

                System.Numerics.Vector2 imageSize;

                if (viewportAspect > framebufferAspect)
                {
                    // Viewport is wider than texture
                    imageSize.Y = viewportSize.Y;
                    imageSize.X = viewportSize.Y * framebufferAspect;
                }
                else
                {
                    // Viewport is higher than texture
                    imageSize.X = viewportSize.X;
                    imageSize.Y = viewportSize.X / framebufferAspect;
                }

                System.Numerics.Vector2 imagePos = (viewportSize - imageSize) / 2;
                ImGui.SetCursorPos(imagePos);
                ImGui.Image(framebufferTexture, imageSize, new System.Numerics.Vector2(0, 1), new System.Numerics.Vector2(1, 0));
            }
            ImGui.End();
        }

        private void RenderRenderSettingsTab()
        {
            if (ImGui.BeginTabItem("Graphics Settings"))
            {
                if(ImGui.Combo("##GraphicsPresets", ref currentRenderSettingsID, renderSettingNames.ToArray(), renderSettingNames.Count))
                {
                    currentRenderSettings = renderSettings[currentRenderSettingsID];
                }

                ImGui.InputInt("Rays per Pixel", ref currentRenderSettings.RaysPerPixel, 1, 16);
                ImGui.InputInt("Samples", ref currentRenderSettings.NumSamples, 1, 16);

                if (ImGui.Button("Apply"))
                {
                    renderSettings[currentRenderSettingsID] = currentRenderSettings;
                    Renderer.UpdateRenderSettings(renderSettings[currentRenderSettingsID]);
                }

                ImGui.EndTabItem();
            }
        }

        private void RenderCameraSettingsTab()
        {
            if (ImGui.BeginTabItem("Camera Settings"))
            {
                ImGui.InputFloat("Speed", ref cameraSettings.CameraSpeed, 0.1f, 100.0f);
                ImGui.InputFloat("Sensitivity", ref cameraSettings.Sensitivity, 0.001f, 1.0f);

                if (ImGui.Button("Apply"))
                {
                    Renderer.UpdateCameraSettings(cameraSettings);
                }

                ImGui.EndTabItem();
            }
        }

        private void RenderPropertiesPanel()
        {
            ImGui.Begin("Settings Panel");
            if (ImGui.BeginTabBar("SettingsTabs"))
            {
                RenderRenderSettingsTab();
                RenderCameraSettingsTab();

                ImGui.EndTabBar();
            }
            ImGui.End();
        }
    }
}