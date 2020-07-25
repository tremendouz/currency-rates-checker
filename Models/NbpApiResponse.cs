using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcCurrency.Models
{
    public class NbpApiResponse
    {
        public CurrencyRate CurrencyRate { get; set; }
        public string ErrorMsg { get; set; }
    }
}
