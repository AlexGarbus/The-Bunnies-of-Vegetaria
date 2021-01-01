namespace TheBunniesOfVegetaria
{
    public class PushPopValue<T>
    {
        private T value;

        /// <summary>
        /// Store a new value. This will overwrite a previously stored value.
        /// </summary>
        /// <param name="value">The value to store.</param>
        public void Push(T value)
        {
            this.value = value;
        }

        /// <summary>
        /// Get and remove the stored value.
        /// </summary>
        /// <returns>The stored value.</returns>
        public T Pop()
        {
            T tmp = value;
            value = default;
            return tmp;
        }

        /// <summary>
        /// Try to get and remove the stored value.
        /// </summary>
        /// <param name="value">The stored value, or null if it has not been set.</param>
        /// <returns>Whether the operation successfully popped a value.</returns>
        public bool TryPop(out T value)
        {
            value = Pop();
            return value != null;
        }

        /// <summary>
        /// Check whether a value has been stored.
        /// </summary>
        /// <returns>Whether a value has been stored.</returns>
        public bool HasValue()
        {
            return value != null;
        }

        public PushPopValue()
        {
            value = default;
        }

        public PushPopValue(T value)
        {
            this.value = value;
        }
    }
}