﻿using Engine.Core.Rendering;
using Engine.Game;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Engine
{
    public static class Program
    {
        private static void Main()
        {
            var nativeWindowSettings = new NativeWindowSettings()
            {
                ClientSize = new Vector2i(800, 600),
                Title = "RT",
                // This is needed to run on macos
                Flags = ContextFlags.ForwardCompatible,
            };

            WindowCycler windowCycler = new WindowCycler();

            // Game
            /*using (var window = new Core.EngineWindow(GameWindowSettings.Default, nativeWindowSettings, windowCycler))
            {
                GameScript game = new Game.Game(window.Renderer);
                game.Init(windowCycler, window);
                window.Run();
            }*/

            // Editor
            using (var window = new Editor.EditorWindow(GameWindowSettings.Default, nativeWindowSettings, windowCycler))
            {
                GameScript game = new Game.Game(window.Renderer);
                game.Init(windowCycler, window);
                window.Run();
            }
        }
    }
}