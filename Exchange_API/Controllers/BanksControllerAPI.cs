using Azure.Core;
using Exchange_API.Data;
using Exchange_API.Models;
using Exchange_API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;

namespace Exchange_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class BanksControllerAPI : ControllerBase
	{
		private readonly IBankService _iBankService;
		//---------------------------------------------------------------------------------------------
		public BanksControllerAPI(IBankService iBankService)
		{
			_iBankService = iBankService;
		}
		//---------------------------------------------------------------------------------------------
		[Route("GetAllBanksInfo")]
        [HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public ActionResult GetAll()
		{
			var Result = _iBankService.Geterr();
			if (Result == null)
				return StatusCode(StatusCodes.Status500InternalServerError);

            return Ok(Result);
		}
        [HttpGet("GetBanksCurrency/{Currancy}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public IActionResult GetCurrencyBn(string Currancy,[Required][FromQuery] List<string> BankName, string StartDate = null, string EnDate = null)
		{
			var Result = _iBankService.GetParameter(Currancy, BankName, StartDate, EnDate);

			if (!Result.Any())
			{
				return NotFound("Not Found");
			}
			if (Result.Any(x => x.GetType().GetProperty("ErrorFlag") != null && x.GetType().GetProperty("ErrorFlag").GetValue(x) != null))
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
			return Ok(Result);
        }
		//---------------------------------------------------------------------------------------------
		[HttpPost("AddNewCurrency/{CurrancyCode}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public IActionResult CreateCurrency([Required]string CurrancyCode, [Required] string FullName)
		{
			var Result = _iBankService.CurrencyCreate(CurrancyCode, FullName);
			if (Result == 99)
				return Ok("Currency Already Existed");
			if(Result==0)
				return StatusCode(StatusCodes.Status500InternalServerError);

			return Ok();
		}
		//---------------------------------------------------------------------------------------------
		[HttpPost("PlacementCurrency/{CurrancyCode}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public IActionResult placementCurrency([Required] string CurrancyCode, [Required] string BankCode,[Required]decimal ByRate,[Required]decimal SellRate)
		{
			var Result = _iBankService.Belonging(CurrancyCode, BankCode, ByRate, SellRate);
			if (Result == 0)
				return BadRequest("the requested host is unknown");
			return Ok();
		}

	}
}
