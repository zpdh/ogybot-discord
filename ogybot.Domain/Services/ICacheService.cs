namespace ogybot.Domain.Services;

public interface ICacheService
{
    T? QueryFor<T>(object key);
    void InsertInto<T>(object key, T entity);
}