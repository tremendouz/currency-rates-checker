using MvcCurrency.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using System.IO;

namespace MvcCurrency.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly HttpClient _httpClient;

        public CurrencyService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponse<Rate>> GetCurrentAverageRate(string currencyName)
        {
            var endpoint = $"/rates/a/{currencyName}/";
            var response = await GetCurrencyRate(endpoint);

            return new ApiResponse<Rate>
            {
                Payload = response.CurrencyRate.Rates.FirstOrDefault(),
                ErrorMsg = response.ErrorMsg
            };
        }

        public async Task<ApiResponse<List<MonthlyRate>>> GetMonthlyAverageRates(string currencyName)
        {
            var endpoint = $"rates/a/{currencyName}/";

            var currentDate = DateTime.Now;
            var currentMonth = currentDate.Month;

            var offset = 0;

            var monthlyAvarages = new List<MonthlyRate>();

            for (int i = currentMonth; i >= 1; --i)
            {
                var dateWithOffset = currentDate.AddMonths(offset);
                var monthlyAvarage = await GetRatesForGivenMonth(currencyName, dateWithOffset);

                if (monthlyAvarage.Payload != null)
                {
                    var avgRate = monthlyAvarage.Payload.Average(m => m.Mid);
                    monthlyAvarages.Add(new MonthlyRate()
                    {
                        Date = dateWithOffset,
                        Rate = avgRate
                    });
                } 
                else
                {
                    return new ApiResponse<List<MonthlyRate>>()
                    {
                        ErrorMsg = monthlyAvarage.ErrorMsg
                    };
            }

                offset--;
            }

            return new ApiResponse<List<MonthlyRate>>() { 
                Payload = monthlyAvarages
            };
        }

        public async Task<ApiResponse<List<Rate>>> GetRatesForGivenMonth(string currencyName, DateTime currentDate)
        {
            var firstDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1)
                .ToString("yyyy-MM-dd");
            var lastDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, DateTime.DaysInMonth(currentDate.Year, currentDate.Month))
                .ToString("yyyy-MM-dd");

            var endpoint = $"rates/a/{currencyName}/{firstDayOfMonth}/{lastDayOfMonth}";
            var response = await GetCurrencyRate(endpoint);

            return new ApiResponse<List<Rate>>
            {
                Payload = response.CurrencyRate.Rates,
                ErrorMsg = response.ErrorMsg
            };
        }

        public async Task<ApiResponse<List<Rate>>> GetRatesForCurrentMonth(string currencyName)
        {
            var currentDate = DateTime.Now;
            var firstDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1)
                .ToString("yyyy-MM-dd");
            var currentFormattedDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day)
                .ToString("yyyy-MM-dd");

            var endpoint = $"rates/a/{currencyName}/{firstDayOfMonth}/{currentFormattedDate}";
            var response = await GetCurrencyRate(endpoint);

            return new ApiResponse<List<Rate>>
            {
                Payload = response.CurrencyRate?.Rates,
                ErrorMsg = response.ErrorMsg
            };
        }

        private async Task<NbpApiResponse> GetCurrencyRate(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync(endpoint);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var currencyRate = JsonConvert.DeserializeObject<CurrencyRate>(content);
                return new NbpApiResponse() { CurrencyRate = currencyRate };
            }
            catch (HttpRequestException ex)
            {
                return new NbpApiResponse() { ErrorMsg = ex.Message };
            }
        }
    }
}
