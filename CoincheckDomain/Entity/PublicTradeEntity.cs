using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;


namespace CoincheckDomain.Entity
{
#if DEBUG
    [Table("t_trade_public")]
#elif RELEASE
    [Table("t_trade_public")]
#endif
    [PrimaryKey(nameof(Id))]

    public class PublicTradeEntity
    {
        public int Id { get; set; }

        [JsonProperty("pair")]
        [Column("pair")]
        public string Pair { get; set; }

        [JsonProperty("id")]
        [Column("trade_id")]
        public string TradeId { get; set; }

        [JsonProperty("amount")]
        [Column("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("rate")]
        [Column("rate")]
        public decimal Rate { get; set; }

        [JsonProperty("order_type")]
        [Column("order_type")]
        public string OrderType { get; set; }

        [JsonProperty("created_at")]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
