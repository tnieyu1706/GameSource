namespace _Project.Scripts.Effect
{
    public interface IEffectExecutor
    {
        void Start();
        void Trigger();
        void Stop();
        void Kill();
    }
}