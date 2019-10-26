using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProAgil.Domain;

namespace ProAgil.Repository
{
    public class ProAgilRepository : IProAgilRepository
    {
        private readonly ProAgilContext _context;

        public ProAgilRepository(ProAgilContext context)
        {
            _context = context;
        }
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }
        public void Update<T>(T entity) where T : class
        {
            _context.Update(entity);
        }
        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }
        public void DeleteRange<T>(T[] entityArray) where T : class
        {
            _context.RemoveRange(entityArray);
        }
        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() > 0);
        }

        //Empresa por id
        public async Task<Empresa> GetEmpresaAsyncById(int empresaid)
        {

            return await _context.Empresas.FindAsync(empresaid);

        }

        //Empresa retornar todos, incluir redes sociais aqui
        public async Task<IEnumerable<Empresa>> GetAllEmpresaAsync()
        {

            return await _context.Empresas.OrderBy(c => c.Id).ToListAsync();     
                  
                                   
        }

        //Enterprise retorna por nome
        public async Task<IEnumerable<Empresa>> GetAllEmpresaAsyncByNome(string nome)
        {
            IQueryable<Empresa> query = _context.Empresas;

            query = query.AsNoTracking()
                        .Where(e => e.Nome.ToLower().Contains(nome.ToLower()))
                        .OrderByDescending(d => d.DataCadastro);

            return await query.ToArrayAsync();


        }

        //Não entendo porque ele puxou sendo que já tenho todos os metoodos
        Task<Empresa[]> IProAgilRepository.GetAllEmpresaAsyncByNome(string nome)
        {
            throw new System.NotImplementedException();
        }

        Task<Empresa[]> IProAgilRepository.GetAllEmpresaAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}
