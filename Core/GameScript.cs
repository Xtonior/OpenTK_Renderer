using Engine.Core;
using OpenTK.Windowing.Common;

namespace Engine.Game
{
    public abstract class GameScript
    {
        internal protected Window? window;

        public bool IsActive { get; private set; }

        public GameScript(Window? window)
        {
            this.window = window;

            EngineCycler.LoadAction += OnLoad;
            EngineCycler.UpdateFrameAction += OnUpdate;
        }

        public void Destroy()
        {
            EngineCycler.LoadAction -= OnLoad;
            EngineCycler.UpdateFrameAction -= OnUpdate;
        }

        public abstract void OnLoad();
        public virtual void OnUpdate(float dt)
        {
            if (!IsActive) return;
        }
        public void SetActive(bool value)
        {
            if (IsActive == value) return;

            IsActive = value;
            if (IsActive)
            {
                OnEnable();
            }
            else
            {
                OnDisable();
            }
        }

        protected virtual void OnEnable()
        {
            
        }

        protected virtual void OnDisable()
        {
            
        }
    }
}