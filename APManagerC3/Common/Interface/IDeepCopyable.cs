namespace APManagerC3 {
    public interface IDeepCopyable<T> {
        T GetDeepCopy();
        void DeepCopyFrom(T source);
    }
}
