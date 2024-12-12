using System;

namespace Engine
{
    public class WindowCycler
    {
        public Action? LoadAction;
        public Action<float>? UpdateFrameAction;
        public Action? RenderFrameAction;

        public void Load()
        {
            LoadAction?.Invoke();
        }

        public void Update(float time)
        {
            UpdateFrameAction?.Invoke(time);
        }

        public void Render()
        {
            RenderFrameAction?.Invoke();
        }
    }
}