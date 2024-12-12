﻿using Engine.Game;
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

            // Game
            /*using (var window = new Engine.Core.Window(GameWindowSettings.Default, nativeWindowSettings))
            {
                GameScript game = new Game.Game();
                game.Init(window);

                window.Run();
            }*/

            // Editor
            using (var window = new Editor.EditorWindow(GameWindowSettings.Default, nativeWindowSettings))
            {
                window.Init();
                
                window.Run();
            }
        }
    }
}