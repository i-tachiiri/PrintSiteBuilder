
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoincheckDomain.Entity
{
#if DEBUG
    [Table("t_balance")]
#elif RELEASE
    [Table("t_balance")]
#endif
    [PrimaryKey(nameof(Id))]
    public class BalanceEntity
    {
        public int Id { get; set; }

        public bool Success { get; set; }

        // 現在の残高
        [Column("jpy")]
        public decimal Jpy { get; set; }

        [Column("btc")]
        public decimal Btc { get; set; }

        [Column("eth")]
        public decimal Eth { get; set; }

        [Column("etc")]
        public decimal Etc { get; set; }

        [Column("lsk")]
        public decimal Lsk { get; set; }

        [Column("xrp")]
        public decimal Xrp { get; set; }

        [Column("xem")]
        public decimal Xem { get; set; }

        [Column("ltc")]
        public decimal Ltc { get; set; }

        [Column("bch")]
        public decimal Bch { get; set; }

        [Column("mona")]
        public decimal Mona { get; set; }

        [Column("xlm")]
        public decimal Xlm { get; set; }

        [Column("qtum")]
        public decimal Qtum { get; set; }

        [Column("bat")]
        public decimal Bat { get; set; }

        [Column("iost")]
        public decimal Iost { get; set; }

        [Column("enj")]
        public decimal Enj { get; set; }

        [Column("plt")]
        public decimal Plt { get; set; }

        [Column("sand")]
        public decimal Sand { get; set; }

        [Column("xym")]
        public decimal Xym { get; set; }

        [Column("dot")]
        public decimal Dot { get; set; }

        [Column("flr")]
        public decimal Flr { get; set; }

        [Column("fnct")]
        public decimal Fnct { get; set; }

        [Column("chz")]
        public decimal Chz { get; set; }

        [Column("link")]
        public decimal Link { get; set; }

        [Column("dai")]
        public decimal Dai { get; set; }

        [Column("mkr")]
        public decimal Mkr { get; set; }

        [Column("matic")]
        public decimal Matic { get; set; }

        [Column("ape")]
        public decimal Ape { get; set; }

        [Column("axs")]
        public decimal Axs { get; set; }

        [Column("imx")]
        public decimal Imx { get; set; }

        [Column("wbtc")]
        public decimal Wbtc { get; set; }

        [Column("shib")]
        public decimal Shib { get; set; }

        [Column("avax")]
        public decimal Avax { get; set; }

        [Column("bril")]
        public decimal Bril { get; set; }

        [Column("bc")]
        public decimal Bc { get; set; }

        // 予約済み残高
        [Column("jpy_reserved")]
        public decimal JpyReserved { get; set; }

        [Column("btc_reserved")]
        public decimal BtcReserved { get; set; }

        [Column("eth_reserved")]
        public decimal EthReserved { get; set; }

        [Column("etc_reserved")]
        public decimal EtcReserved { get; set; }

        [Column("lsk_reserved")]
        public decimal LskReserved { get; set; }

        [Column("xrp_reserved")]
        public decimal XrpReserved { get; set; }

        [Column("xem_reserved")]
        public decimal XemReserved { get; set; }

        [Column("ltc_reserved")]
        public decimal LtcReserved { get; set; }

        [Column("bch_reserved")]
        public decimal BchReserved { get; set; }

        [Column("mona_reserved")]
        public decimal MonaReserved { get; set; }

        [Column("xlm_reserved")]
        public decimal XlmReserved { get; set; }

        [Column("qtum_reserved")]
        public decimal QtumReserved { get; set; }

        [Column("bat_reserved")]
        public decimal BatReserved { get; set; }

        [Column("iost_reserved")]
        public decimal IostReserved { get; set; }

        [Column("enj_reserved")]
        public decimal EnjReserved { get; set; }

        [Column("plt_reserved")]
        public decimal PltReserved { get; set; }

        [Column("sand_reserved")]
        public decimal SandReserved { get; set; }

        [Column("xym_reserved")]
        public decimal XymReserved { get; set; }

        [Column("dot_reserved")]
        public decimal DotReserved { get; set; }

        [Column("flr_reserved")]
        public decimal FlrReserved { get; set; }

        [Column("fnct_reserved")]
        public decimal FnctReserved { get; set; }

        [Column("chz_reserved")]
        public decimal ChzReserved { get; set; }

        [Column("link_reserved")]
        public decimal LinkReserved { get; set; }

        [Column("dai_reserved")]
        public decimal DaiReserved { get; set; }

        [Column("mkr_reserved")]
        public decimal MkrReserved { get; set; }

        [Column("matic_reserved")]
        public decimal MaticReserved { get; set; }

        [Column("ape_reserved")]
        public decimal ApeReserved { get; set; }

        [Column("axs_reserved")]
        public decimal AxsReserved { get; set; }

        [Column("imx_reserved")]
        public decimal ImxReserved { get; set; }

        [Column("wbtc_reserved")]
        public decimal WbtcReserved { get; set; }

        [Column("shib_reserved")]
        public decimal ShibReserved { get; set; }

        [Column("avax_reserved")]
        public decimal AvaxReserved { get; set; }

        [Column("bril_reserved")]
        public decimal BrilReserved { get; set; }

        [Column("bc_reserved")]
        public decimal BcReserved { get; set; }

        // 貸出中の残高
        [Column("jpy_lend_in_use")]
        public decimal JpyLendInUse { get; set; }

        [Column("btc_lend_in_use")]
        public decimal BtcLendInUse { get; set; }

        [Column("eth_lend_in_use")]
        public decimal EthLendInUse { get; set; }

        [Column("etc_lend_in_use")]
        public decimal EtcLendInUse { get; set; }

        [Column("lsk_lend_in_use")]
        public decimal LskLendInUse { get; set; }

        [Column("xrp_lend_in_use")]
        public decimal XrpLendInUse { get; set; }

        [Column("xem_lend_in_use")]
        public decimal XemLendInUse { get; set; }

        [Column("ltc_lend_in_use")]
        public decimal LtcLendInUse { get; set; }

        [Column("bch_lend_in_use")]
        public decimal BchLendInUse { get; set; }

        [Column("mona_lend_in_use")]
        public decimal MonaLendInUse { get; set; }

        [Column("xlm_lend_in_use")]
        public decimal XlmLendInUse { get; set; }

        [Column("qtum_lend_in_use")]
        public decimal QtumLendInUse { get; set; }

        [Column("bat_lend_in_use")]
        public decimal BatLendInUse { get; set; }

        [Column("iost_lend_in_use")]
        public decimal IostLendInUse { get; set; }

        [Column("enj_lend_in_use")]
        public decimal EnjLendInUse { get; set; }

        [Column("plt_lend_in_use")]
        public decimal PltLendInUse { get; set; }

        [Column("sand_lend_in_use")]
        public decimal SandLendInUse { get; set; }

        [Column("xym_lend_in_use")]
        public decimal XymLendInUse { get; set; }

        [Column("dot_lend_in_use")]
        public decimal DotLendInUse { get; set; }

        [Column("flr_lend_in_use")]
        public decimal FlrLendInUse { get; set; }

        [Column("fnct_lend_in_use")]
        public decimal FnctLendInUse { get; set; }

        [Column("chz_lend_in_use")]
        public decimal ChzLendInUse { get; set; }

        [Column("link_lend_in_use")]
        public decimal LinkLendInUse { get; set; }

        [Column("dai_lend_in_use")]
        public decimal DaiLendInUse { get; set; }

        [Column("mkr_lend_in_use")]
        public decimal MkrLendInUse { get; set; }

        [Column("matic_lend_in_use")]
        public decimal MaticLendInUse { get; set; }

        [Column("ape_lend_in_use")]
        public decimal ApeLendInUse { get; set; }

        [Column("axs_lend_in_use")]
        public decimal AxsLendInUse { get; set; }

        [Column("imx_lend_in_use")]
        public decimal ImxLendInUse { get; set; }

        [Column("wbtc_lend_in_use")]
        public decimal WbtcLendInUse { get; set; }

        [Column("shib_lend_in_use")]
        public decimal ShibLendInUse { get; set; }

        [Column("avax_lend_in_use")]
        public decimal AvaxLendInUse { get; set; }

        [Column("bril_lend_in_use")]
        public decimal BrilLendInUse { get; set; }

        [Column("bc_lend_in_use")]
        public decimal BcLendInUse { get; set; }

        // 貸出済み残高
        [Column("jpy_lent")]
        public decimal JpyLent { get; set; }

        [Column("btc_lent")]
        public decimal BtcLent { get; set; }

        [Column("eth_lent")]
        public decimal EthLent { get; set; }

        [Column("etc_lent")]
        public decimal EtcLent { get; set; }

        [Column("lsk_lent")]
        public decimal LskLent { get; set; }

        [Column("xrp_lent")]
        public decimal XrpLent { get; set; }

        [Column("xem_lent")]
        public decimal XemLent { get; set; }

        [Column("ltc_lent")]
        public decimal LtcLent { get; set; }

        [Column("bch_lent")]
        public decimal BchLent { get; set; }

        [Column("mona_lent")]
        public decimal MonaLent { get; set; }

        [Column("xlm_lent")]
        public decimal XlmLent { get; set; }

        [Column("qtum_lent")]
        public decimal QtumLent { get; set; }

        [Column("bat_lent")]
        public decimal BatLent { get; set; }

        [Column("iost_lent")]
        public decimal IostLent { get; set; }

        [Column("enj_lent")]
        public decimal EnjLent { get; set; }

        [Column("plt_lent")]
        public decimal PltLent { get; set; }

        [Column("sand_lent")]
        public decimal SandLent { get; set; }

        [Column("xym_lent")]
        public decimal XymLent { get; set; }

        [Column("dot_lent")]
        public decimal DotLent { get; set; }

        [Column("flr_lent")]
        public decimal FlrLent { get; set; }

        [Column("fnct_lent")]
        public decimal FnctLent { get; set; }

        [Column("chz_lent")]
        public decimal ChzLent { get; set; }

        [Column("link_lent")]
        public decimal LinkLent { get; set; }

        [Column("dai_lent")]
        public decimal DaiLent { get; set; }

        [Column("mkr_lent")]
        public decimal MkrLent { get; set; }

        [Column("matic_lent")]
        public decimal MaticLent { get; set; }

        [Column("ape_lent")]
        public decimal ApeLent { get; set; }

        [Column("axs_lent")]
        public decimal AxsLent { get; set; }

        [Column("imx_lent")]
        public decimal ImxLent { get; set; }

        [Column("wbtc_lent")]
        public decimal WbtcLent { get; set; }

        [Column("shib_lent")]
        public decimal ShibLent { get; set; }

        [Column("avax_lent")]
        public decimal AvaxLent { get; set; }

        [Column("bril_lent")]
        public decimal BrilLent { get; set; }

        [Column("bc_lent")]
        public decimal BcLent { get; set; }

        // 借入金
        [Column("jpy_debt")]
        public decimal JpyDebt { get; set; }

        [Column("btc_debt")]
        public decimal BtcDebt { get; set; }

        [Column("eth_debt")]
        public decimal EthDebt { get; set; }

        [Column("etc_debt")]
        public decimal EtcDebt { get; set; }

        [Column("lsk_debt")]
        public decimal LskDebt { get; set; }

        [Column("xrp_debt")]
        public decimal XrpDebt { get; set; }

        [Column("xem_debt")]
        public decimal XemDebt { get; set; }

        [Column("ltc_debt")]
        public decimal LtcDebt { get; set; }

        [Column("bch_debt")]
        public decimal BchDebt { get; set; }

        [Column("mona_debt")]
        public decimal MonaDebt { get; set; }

        [Column("xlm_debt")]
        public decimal XlmDebt { get; set; }

        [Column("qtum_debt")]
        public decimal QtumDebt { get; set; }

        [Column("bat_debt")]
        public decimal BatDebt { get; set; }

        [Column("iost_debt")]
        public decimal IostDebt { get; set; }

        [Column("enj_debt")]
        public decimal EnjDebt { get; set; }

        [Column("plt_debt")]
        public decimal PltDebt { get; set; }

        [Column("sand_debt")]
        public decimal SandDebt { get; set; }

        [Column("xym_debt")]
        public decimal XymDebt { get; set; }

        [Column("dot_debt")]
        public decimal DotDebt { get; set; }

        [Column("flr_debt")]
        public decimal FlrDebt { get; set; }

        [Column("fnct_debt")]
        public decimal FnctDebt { get; set; }

        [Column("chz_debt")]
        public decimal ChzDebt { get; set; }

        [Column("link_debt")]
        public decimal LinkDebt { get; set; }

        [Column("dai_debt")]
        public decimal DaiDebt { get; set; }

        [Column("mkr_debt")]
        public decimal MkrDebt { get; set; }

        [Column("matic_debt")]
        public decimal MaticDebt { get; set; }

        [Column("ape_debt")]
        public decimal ApeDebt { get; set; }

        [Column("axs_debt")]
        public decimal AxsDebt { get; set; }

        [Column("imx_debt")]
        public decimal ImxDebt { get; set; }

        [Column("wbtc_debt")]
        public decimal WbtcDebt { get; set; }

        [Column("shib_debt")]
        public decimal ShibDebt { get; set; }

        [Column("avax_debt")]
        public decimal AvaxDebt { get; set; }

        [Column("bril_debt")]
        public decimal BrilDebt { get; set; }

        [Column("bc_debt")]
        public decimal BcDebt { get; set; }

        // 積立中の残高
        [Column("jpy_tsumitate")]
        public decimal JpyTsumitate { get; set; }

        [Column("btc_tsumitate")]
        public decimal BtcTsumitate { get; set; }

        [Column("eth_tsumitate")]
        public decimal EthTsumitate { get; set; }

        [Column("etc_tsumitate")]
        public decimal EtcTsumitate { get; set; }

        [Column("lsk_tsumitate")]
        public decimal LskTsumitate { get; set; }

        [Column("xrp_tsumitate")]
        public decimal XrpTsumitate { get; set; }

        [Column("xem_tsumitate")]
        public decimal XemTsumitate { get; set; }

        [Column("ltc_tsumitate")]
        public decimal LtcTsumitate { get; set; }

        [Column("bch_tsumitate")]
        public decimal BchTsumitate { get; set; }

        [Column("mona_tsumitate")]
        public decimal MonaTsumitate { get; set; }

        [Column("xlm_tsumitate")]
        public decimal XlmTsumitate { get; set; }

        [Column("qtum_tsumitate")]
        public decimal QtumTsumitate { get; set; }

        [Column("bat_tsumitate")]
        public decimal BatTsumitate { get; set; }

        [Column("iost_tsumitate")]
        public decimal IostTsumitate { get; set; }

        [Column("enj_tsumitate")]
        public decimal EnjTsumitate { get; set; }

        [Column("plt_tsumitate")]
        public decimal PltTsumitate { get; set; }

        [Column("sand_tsumitate")]
        public decimal SandTsumitate { get; set; }

        [Column("xym_tsumitate")]
        public decimal XymTsumitate { get; set; }

        [Column("dot_tsumitate")]
        public decimal DotTsumitate { get; set; }

        [Column("flr_tsumitate")]
        public decimal FlrTsumitate { get; set; }

        [Column("fnct_tsumitate")]
        public decimal FnctTsumitate { get; set; }

        [Column("chz_tsumitate")]
        public decimal ChzTsumitate { get; set; }

        [Column("link_tsumitate")]
        public decimal LinkTsumitate { get; set; }

        [Column("dai_tsumitate")]
        public decimal DaiTsumitate { get; set; }

        [Column("mkr_tsumitate")]
        public decimal MkrTsumitate { get; set; }

        [Column("matic_tsumitate")]
        public decimal MaticTsumitate { get; set; }

        [Column("ape_tsumitate")]
        public decimal ApeTsumitate { get; set; }

        [Column("axs_tsumitate")]
        public decimal AxsTsumitate { get; set; }

        [Column("imx_tsumitate")]
        public decimal ImxTsumitate { get; set; }

        [Column("wbtc_tsumitate")]
        public decimal WbtcTsumitate { get; set; }

        [Column("shib_tsumitate")]
        public decimal ShibTsumitate { get; set; }

        [Column("avax_tsumitate")]
        public decimal AvaxTsumitate { get; set; }

        [Column("bril_tsumitate")]
        public decimal BrilTsumitate { get; set; }

        [Column("bc_tsumitate")]
        public decimal BcTsumitate { get; set; }

        // 日次情報
        [Column("date")]
        public DateTime Date { get; set; }
    }


}
