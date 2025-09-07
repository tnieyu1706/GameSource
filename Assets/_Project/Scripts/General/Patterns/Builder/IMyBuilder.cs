namespace _Project.Scripts.General.Patterns.Builder
{
    public interface IMyBuilder<out T>
    {
        T Build();
    }
}