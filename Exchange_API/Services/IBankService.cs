using Exchange_API.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Exchange_API.Services
{
    public interface IBankService
    {
        object Geterr();
	    IEnumerable<object> GetParameter(string Currancy, [FromQuery] List<string> BankName, string StartDate = null, string EnDate = null);
	    public int CurrencyCreate(string CurrancyCode, string FulName);

	    public int Belonging(string CurrancyCode,  string BankCode, decimal ByRate, decimal SellRate);
    }
}
