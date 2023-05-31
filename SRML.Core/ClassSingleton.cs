namespace SRML.Core
{
    public abstract class ClassSingleton<T> where T : ClassSingleton<T>
    {
        public static T Instance { get => _instance; }
        private static T _instance;

        public ClassSingleton()
        {
            _instance = (T)this;
        }
    }
}
