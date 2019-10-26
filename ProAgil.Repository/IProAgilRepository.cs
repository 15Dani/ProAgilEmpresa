using System.Threading.Tasks;
using ProAgil.Domain;

namespace ProAgil.Repository
{
    public interface IProAgilRepository
    {
        void Add<T>(T entity) where T : class;
        void Update<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        void DeleteRange<T>(T[] entityArray) where T : class;
        Task<bool> SaveChangesAsync();

        Task<Empresa[]> GetAllEmpresaAsyncByNome(string nome);
        Task<Empresa[]> GetAllEmpresaAsync();
        Task<Empresa> GetEmpresaAsyncById(int empresaid);

     
        
    }
}