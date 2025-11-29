namespace UserService.Exceptions
{
    public class UserAlreadyExistException : Exception
    {
        public UserAlreadyExistException(string email) : base($"User with email {email} already exist") { }
    }
}
