using System;
using Engine.Core;
using Engine.Game;

namespace Engine
{
    public static class EngineCycler
    {
        public static Action? LoadAction;
        public static Action<float>? UpdateFrameAction;
        public static Action? DisableScriptAction;
        public static Action? RenderFrameAction;

        public static GameScript Game;

        public static void Init(Window window)
        {
            Game = new Game.Game(window);
        }

        public static void Load()
        {
            LoadAction?.Invoke();
        }

        public static void Update(float time)
        {
            UpdateFrameAction?.Invoke(time);
        }

        public static void Update(float time, bool isActive)
        {
            if (!isActive)
            {
                DisableScriptAction?.Invoke();
                return;
            }

            UpdateFrameAction?.Invoke(time);
        }

        public static void Render()
        {
            RenderFrameAction?.Invoke();
        }
    }
}