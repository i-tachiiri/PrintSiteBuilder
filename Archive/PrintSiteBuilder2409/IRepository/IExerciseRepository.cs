using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using PrintSiteBuilder2409.Entities;

namespace PrintSiteBuilder2409.IRepository
{
    public interface IExerciseRepository
    {
        public Task RenewMasterAsync(Func<ExerciseMaster, bool> condition, IEnumerable<ExerciseMaster> problems);
        Task<ExerciseMaster> GetByIdAsync(string PrintId);
    }
}
