using System.Linq;
using Microsoft.EntityFrameworkCore;
using PocketBook.Core.IRepositories;
using PocketBook.Data;
using PocketBook.Models;

namespace PocketBook.Core.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context, ILogger logger) : base(context, logger)
        {

        }
        public override async Task<IEnumerable<User>> All()
        {
            try
            {
                return await dbSet.ToListAsync();
            }
            catch (System.Exception exp)
            {

                _logger.LogError(exp, "{Repo} All method", typeof(UserRepository));
                return new List<User>();
            }
        }
        public override async Task<bool> Upsert(User entity)
        {
            try
            {
                var existingUser = await dbSet.Where(x => x.Id == entity.Id).FirstOrDefaultAsync();
                if (existingUser == null)
                {
                    return await Add(entity);
                }
                existingUser.FirstName = entity.FirstName;
                existingUser.LastName = entity.LastName;
                existingUser.Email = entity.Email;
                return true;


            }
            catch (System.Exception exp)
            {

                _logger.LogError(exp, "{Repo} Upsert method", typeof(UserRepository));
                return false;
            }
        }
        public override async Task<bool> Delete(Guid id)
        {
            try
            {
                var exist = await dbSet.Where(x => x.Id == id).FirstOrDefaultAsync();
                if (exist != null)
                {
                    dbSet.Remove(exist);
                    return true;
                }
                return false;
            }
            catch (System.Exception exp)
            {

                _logger.LogError(exp, "{Repo} Delete method", typeof(UserRepository));
                return false;
            }
        }
    }
}