namespace _Project.Scripts.Effect
{
    public interface IRemoveActivityFlow<in T> : IActivityFlow
    {
        public void OnRemove(T obj);
    }
}