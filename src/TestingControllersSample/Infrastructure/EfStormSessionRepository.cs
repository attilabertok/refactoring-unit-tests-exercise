using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestingControllersSample.Core.Interfaces;
using TestingControllersSample.Core.Model;

namespace TestingControllersSample.Infrastructure
{
    public class EfStormSessionRepository : IBrainstormSessionRepository
    {
        private readonly AppDbContext dbContext;

        public EfStormSessionRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Task<BrainstormSession> GetByIdAsync(int id)
        {
            return dbContext.BrainstormSessions
                .Include(s => s.Ideas)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public Task<List<BrainstormSession>> ListAsync()
        {
            return dbContext.BrainstormSessions
                .Include(s => s.Ideas)
                .OrderByDescending(s => s.DateCreated)
                .ToListAsync();
        }

        public Task AddAsync(BrainstormSession session)
        {
            dbContext.BrainstormSessions.Add(session);
            return dbContext.SaveChangesAsync();
        }

        public Task UpdateAsync(BrainstormSession session)
        {
            dbContext.Entry(session).State = EntityState.Modified;
            return dbContext.SaveChangesAsync();
        }
    }
}
