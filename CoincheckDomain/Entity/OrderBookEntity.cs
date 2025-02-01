using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoincheckDomain.Entity
{
#if DEBUG
    [Table("t_order_book")]
#elif RELEASE
    [Table("t_order_book")]
#endif
    [PrimaryKey(nameof(Id))]
    public class OrderBookEntity
    {
        public int Id { get; set; }
        [JsonProperty("timestamp")]
        [Column("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty("pair")]
        [Column("pair")]
        public string Pair { get; set; }
        [JsonProperty("type")]
        [Column("type")]
        public string Type { get; set; }
        [JsonProperty("price")]
        [Column("price")]
        public decimal Price { get; set; }
        [JsonProperty("quantity")]
        [Column("quantity")]
        public decimal Quantity { get; set; }   

    }
}
