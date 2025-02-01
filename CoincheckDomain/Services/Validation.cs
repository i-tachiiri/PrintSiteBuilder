

using CoincheckDomain.Entity;

namespace CoincheckDomain.Services
{
    public class Validation
    {
        public bool HasBalance(BalanceEntity entity)
        {
            return entity.Jpy > 1000;
        }
    }
}
