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
        public bool IsFocusedWindow { get; set; } = true;
        public bool AutoHideMouse { get; set; } = false;
        public Vector2 LastMousePos { get; internal set; } = Vector2.Zero;

        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
            Renderer = new Renderer(this);
        }

        internal void ChangeGrabMouseState(bool state)
        {
            switch (state)
            {
                case true:
                    LastMousePos = MousePosition;
                    CursorState = CursorState.Grabbed;
                    break;
                case false:
                    MousePosition = LastMousePos;
                    CursorState = CursorState.Normal;
                    break;
            }
        }
    }
}