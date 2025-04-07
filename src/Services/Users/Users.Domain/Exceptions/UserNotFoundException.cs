namespace Users.Domain.Exceptions {
    public class UserNotFoundException : Exception {
        public Guid Id { get; }
        public UserNotFoundException(Guid id) : base($"User with id = {id} was not found")
        {
            Id = id;
        }
    }
}