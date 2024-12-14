using Engine.Core;
using OpenTK.Windowing.Common;

namespace Engine.Game
{
    public abstract class GameScript
    {
        internal protected WindowCycler? cycler;
        internal protected Window? window;

        public void Init(WindowCycler cycler, Window? window)
        {
            this.cycler = cycler;
            this.window = window;

            cycler.LoadAction += OnLoad;
            cycler.UpdateFrameAction += OnUpdate;
            cycler.SleepAction += OnSleep;
        }

        public void Destroy()
        {
            cycler.LoadAction -= OnLoad;
            cycler.UpdateFrameAction -= OnUpdate;
        }

        public abstract void OnLoad();
        public abstract void OnUpdate(float dt);
        public abstract void OnSleep();
    }
}