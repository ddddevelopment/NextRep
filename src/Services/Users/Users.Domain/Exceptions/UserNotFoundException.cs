namespace Users.Domain.Exceptions {
    public class UserNotFoundException<T> : Exception {
        public T Key { get; }
        public UserNotFoundException(T key) : base($"User with key = {key} was not found")
        {
            Key = key;
        }
    }
}