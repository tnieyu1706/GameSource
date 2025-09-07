namespace _Project.Scripts.General.ReusePatterns
{
    public abstract class MyGeneralState<T>
    {
        protected T Context;

        public MyGeneralState(T context)
        {
            this.Context = context;
        }

        public abstract void Entry();
        public abstract void Exit();

        public abstract void Update();

        public abstract void SubscribeInteraction();
        public abstract void UnsubscribeInteraction();
    }
}