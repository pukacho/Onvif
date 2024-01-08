namespace OnvifAPI.Interfaces
{
    public interface IRepository<T>
    {
        T GetById(int id);
        IEnumerable<T> GetAll();

        IEnumerable<T> AddList(IEnumerable<T> entitys);
        T Add(T entity);
        T Update(T entity);
        bool Delete(int id);
    }
}
