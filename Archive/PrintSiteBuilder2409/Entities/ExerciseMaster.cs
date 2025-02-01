using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PrintSiteBuilder2409.Entities
{
    [Table("m_exercise")]
    [PrimaryKey(nameof(PrintId), nameof(PageId), nameof(ExerciseId), nameof(CellNumber))]
    public class ExerciseMaster
    {
        public string PrintId { get; set; }

        public int PageId { get; set; }
        public int ExerciseId { get; set; }

        public int CellNumber { get; set; }

        public string ValueType { get; set; }

        public int ColumnNumber { get; set; }

        public int RowNumber { get; set; }
        public string Value { get; set; }

    }
}
