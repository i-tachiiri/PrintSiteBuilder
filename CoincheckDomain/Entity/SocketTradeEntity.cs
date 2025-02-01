using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoincheckDomain.Entity
{
    public class SocketTradeEntity
    {
        public long Timestamp { get; set; } // 約定タイムスタンプ (unix時間)
        public string TradeId { get; set; } // 約定ID
        public string Pair { get; set; } // 取引ペア
        public decimal Rate { get; set; } // 約定のレート
        public decimal Amount { get; set; } // 約定の量
        public string OrderType { get; set; } // 注文方法 (buy/sell)
        public string TakerOrderId { get; set; } // Takerの注文ID
        public string MakerOrderId { get; set; } // Makerの注文ID
    }
}
