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
            // RunGame(windowCycler, nativeWindowSettings);

            // Editor
            RunEditor(windowCycler, nativeWindowSettings);
        }

        private static void RunGame(WindowCycler windowCycler, NativeWindowSettings nativeWindowSettings)
        {
            using (var window = new Core.EngineWindow(GameWindowSettings.Default, nativeWindowSettings, windowCycler))
            {
                GameScript game = new Game.Game(window.Renderer);
                game.Init(windowCycler, window);
                window.Run();
            }
        }

        private static void RunEditor(WindowCycler windowCycler, NativeWindowSettings nativeWindowSettings)
        {
            using (var window = new Editor.EditorWindow(GameWindowSettings.Default, nativeWindowSettings, windowCycler))
            {
                GameScript game = new Game.Game(window.Renderer);
                game.Init(windowCycler, window);
                window.Run();
            }
        }
    }
}