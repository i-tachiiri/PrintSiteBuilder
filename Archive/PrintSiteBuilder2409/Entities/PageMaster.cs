using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintSiteBuilder2409.Entities
{
    [Table("m_page")]
    [PrimaryKey(nameof(PrintId), nameof(PageNumber))]
    public class PageMaster
    {
        public string PrintId { get; set; }
        public int PageNumber { get; set; }
        public string PrintSerial { get; set; }
        public bool IsLogoCreated { get; set; }
        public bool IsQrCreated { get; set; }

        public bool IsPdfCreated { get; set; }
        public bool IsPdf4Created { get; set; }
        public bool IsWebpCreated { get; set; }
        public bool IsUuidCreated { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
