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
                ClientSize = new Vector2i(1920, 1080),
                Title = "RT",
                // This is needed to run on macos
                Flags = ContextFlags.ForwardCompatible,
            };

            // Game
            // RunGame(engineCycler, nativeWindowSettings);

            // Editor
            RunEditor(nativeWindowSettings);
        }

        private static void RunGame(NativeWindowSettings nativeWindowSettings)
        {
            using (var window = new Core.EngineWindow(GameWindowSettings.Default, nativeWindowSettings))
            {
                EngineCycler.Init(window);
                window.Run();
            }
        }

        private static void RunEditor(NativeWindowSettings nativeWindowSettings)
        {
            using (var window = new Editor.EditorWindow(GameWindowSettings.Default, nativeWindowSettings))
            {
                EngineCycler.Init(window);
                window.Run();
            }
        }
    }
}