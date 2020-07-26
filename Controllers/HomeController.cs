using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MvcCurrency.Models;
using MvcCurrency.Services;

namespace MvcCurrency.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IExcelExportService _excelExportService;
        private readonly ICurrencyService _currencyService;

        public HomeController(ILogger<HomeController> logger,
            IExcelExportService excelExportService,
            ICurrencyService currencyService)
        {
            _logger = logger;
            _excelExportService = excelExportService;
            _currencyService = currencyService;
        }

        public async Task<IActionResult> Index()
        {
            var eurToPln = await _currencyService.GetCurrentAverageRate(CurrencyNames.eur.ToString());
            var usdToPln = await _currencyService.GetCurrentAverageRate(CurrencyNames.usd.ToString());

            ViewData["EurToPln"] = eurToPln.Payload?.Mid;
            ViewData["UsdToPln"] = usdToPln.Payload?.Mid;

            var usdToPlnForCurrentMonth = await _currencyService.GetRatesForCurrentMonth(CurrencyNames.usd.ToString());

            return View(usdToPlnForCurrentMonth.Payload);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> GetExcelFile(string currencyName)
        {
            var data = await _currencyService.GetRatesForCurrentMonth(currencyName);
            if (!string.IsNullOrEmpty(data.ErrorMsg))
            {
                return NotFound($"NBP API error. {data.ErrorMsg}");
            }

            var fileContent = _excelExportService.GetFileContent(data.Payload);
            return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"rates_{currencyName}_PLN_{DateTime.Now:yyyy-MM-dd}.xlsx");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
