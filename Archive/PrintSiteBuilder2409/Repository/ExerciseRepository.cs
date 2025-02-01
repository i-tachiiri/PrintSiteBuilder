using System.Linq.Expressions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrintSiteBuilder2409.Entities;
using PrintSiteBuilder2409.IRepository;
using PrintSiteBuilder2409.Config;

namespace PrintSiteBuilder2409.Repository
{
    public class ExerciseRepository : IExerciseRepository
    {
        private readonly AppDbContext _context;

        public ExerciseRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<ExerciseMaster> GetByIdAsync(string id)
        {
            return await _context.ExerciseMaster.FindAsync(id);
        }
        public async Task RenewMasterAsync(Func<ExerciseMaster, bool> condition, IEnumerable<ExerciseMaster> problems)
        {
            var entities = _context.ExerciseMaster.Where(condition).ToList();
            if (entities.Any())
            {
                _context.ExerciseMaster.RemoveRange(entities);
            }
            _context.ExerciseMaster.AddRange(problems);
            await _context.SaveChangesAsync();
        }
    }
}
