using System.ComponentModel.DataAnnotations;

namespace Exchange_API.Models
{
    public class ExchangeRate
    {
        public string BankCode { get; set; }
        [Required]
        public string BankName { get; set; }
        public string CurrencyName { get; set; }
        public decimal BayRate { get; set; }
        public decimal SellRate { get; set; }
        public DateTime RateDate { get; set; }
        public string ErrorFlag { get; set; }
        public int ErrorNum {get;set;}
    }
}
