namespace FVTC.LearningInnovations.Unity.Extensions
{
    public static class TypeExtensions
    {
        public static bool Is<T>(this object obj)
        {
            return obj is T;
        }

        public static bool IsNot<T>(this object obj)
        {
            return !obj.Is<T>();
        }

        public static bool IsNull<T>(this object obj) where T : class
        {
            return obj == null;
        }
    }
}
