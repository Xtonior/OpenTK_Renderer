using Engine.Core;
using OpenTK.Windowing.Common;

namespace Engine.Game
{
    public abstract class GameScript
    {
        internal protected Window? window;

        public void Init(Window window)
        {
            this.window = window;

            window.LoadAction += OnLoad;
            window.UpdateFrameAction += OnUpdate;
            window.RenderFrameAction += OnRender;
        }

        public void Destroy()
        {
            window.LoadAction -= OnLoad;
            window.UpdateFrameAction -= OnUpdate;
            window.RenderFrameAction -= OnRender;
        }

        public abstract void OnLoad();
        public abstract void OnUpdate(float dt);
        public abstract void OnRender();
    }
}