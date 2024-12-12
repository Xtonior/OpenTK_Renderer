using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Engine.Core.Rendering;

namespace Engine.Core
{
    public class Window : GameWindow
    {
        public Renderer Renderer {get; private set;}
        public WindowCycler Cycler {get; private set;}
        public bool IsActiveWindow { get; set; } = true;
        public bool AutoHideMouse { get; set; } = false;
        public Vector2 LastMousePos { get; internal set; } = Vector2.Zero;

        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings, WindowCycler windowCycler)
            : base(gameWindowSettings, nativeWindowSettings)
        {
            Cycler = windowCycler;
            Renderer = new Renderer(this);
        }

        internal void ChangeActive(bool active)
        {
            switch (active)
            {
                case true:
                    LastMousePos = MousePosition;
                    IsActiveWindow = true;
                    CursorState = CursorState.Grabbed;
                    break;
                case false:
                    MousePosition = LastMousePos;
                    IsActiveWindow = false;
                    CursorState = CursorState.Normal;
                    break;
            }
        }
    }
}