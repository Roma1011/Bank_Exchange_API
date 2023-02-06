using Exchange_API.Data;
using Exchange_API.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Exchange_API.Services
{
    public class BankService : IBankService
    {
        public object Geterr()
        {
            var All = GetBankInfo.NonParameterGetter();

            if (All != null)
            {
                var obj =   All.Values.Select(item => new
                {
	                item.BankName,
	                item.BankCode,
	                item.CurrencyName
                });
                return obj;
            }
            return All;
        }
		//------------------------------------------------------------------------------------------------------------------------
        public IEnumerable<object> GetParameter(string Currency, List<string> BankName, string StartDate = null, string EndDate = null)
        {
			DateTime? startDate = null;
			DateTime? endDate = null;

			ExchangeRate ex =new ExchangeRate();
			if (!DateTime.TryParseExact(StartDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime tempStartDate))
			{
				startDate = null;
			}
			else
			{
				startDate = DateTime.SpecifyKind(tempStartDate, DateTimeKind.Utc);
			}

			if (!DateTime.TryParseExact(EndDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime tempEndDate))
			{
				endDate = null;
			}
			else
			{
				endDate = DateTime.SpecifyKind(tempEndDate, DateTimeKind.Utc);
			}
			if (BankName == null|| BankName.Count==0)
			{
				return new List<object> { new { ErrorCode = 400} };
			}

			var list = GetBankInfo.ParameterGetter(new List<object>() { Currency, BankName, startDate, endDate });

            if (list.Any(x => x.ErrorFlag != null))
            {
				return list.Select(item => new
				{
					item.ErrorFlag
				});
            }

            return list.Select(item => new
				{
					item.BankCode,
					item.BayRate,
					item.SellRate,
					item.RateDate
				});

        }
        //------------------------------------------------------------------------------------------------------------------------
		public int CurrencyCreate(string CurrancyCode, string FulName)
		{
			int ChseckResult=Chack(CurrancyCode);
			if (ChseckResult==99)
				return ChseckResult;
			var result=GetBankInfo.AddCurrenc(CurrancyCode, FulName);
	        return result;
        }
		//------------------------------------------------------------------------------------------------------------------------
		public int Belonging(string CurrancyCode, string BankCode, decimal ByRate, decimal SellRate)
        {
	        var Result=GetBankInfo.Belonging_to_the_bank(new List<object>() { CurrancyCode, BankCode, ByRate, SellRate });
			return Result;
		}

		private int Chack(string CurrancyCode)
		{
			int Result=GetBankInfo.ChackerCurency(CurrancyCode);
			return Result;
		}
	}
}