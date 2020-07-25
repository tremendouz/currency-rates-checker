using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcCurrency.Models
{
    public class ApiResponse<T>
    {
        public T Payload { get; set; }
        public string ErrorMsg { get; set; }
    }
}
