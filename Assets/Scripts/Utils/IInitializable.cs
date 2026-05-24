namespace Utils
{
    public interface IInitializable<in T>
    {
        void Initialize(T data);
    }
}