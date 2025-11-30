namespace UserService.Infrastructure
{
    public interface IUnitOfWork
    {
        Task SaveChangesAsync();
    }
}
