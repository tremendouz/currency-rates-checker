using MvcCurrency.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MvcCurrency.Services
{
    public interface IExcelExportService
    {
        byte[] GetFileContent(List<Rate> rates);
    }
}
