using MvcCurrency.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcCurrency.Services
{
    public interface ICurrencyService
    {
        public Task<ApiResponse<Rate>> GetCurrentAverageRate(string currencyName);
        public Task<ApiResponse<List<Rate>>> GetRatesForCurrentMonth(string currencyName);
        public Task<ApiResponse<List<MonthlyRate>>> GetMonthlyAverageRates(string currencyName);
    }
}
