using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CIBT_FileReader.Models;
using CIBT_FileReader.Services;
using System.Configuration;

namespace CIBT_FileReader
{
    class Program
    {
        const string PathToMonitor = @"c:\Test";
        static void Main(string[] args)
        {
            CheckService checkService = new CheckService();
            FileService fileService = new FileService(checkService);

            string pathToMonitor = ConfigurationManager.AppSettings["PathToMonitor"];

            fileService.ProcessDirectory(pathToMonitor);
        }
    }
}
