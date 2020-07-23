using CIBT_FileReader.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIBT_FileReader.Services
{
    public class FileService
    {
        public int statementID;

        CheckService _checkService;
       public FileService(CheckService checkService){
            _checkService = checkService;
        }
        // Process all files in the directory passed in, recurse on any directories 
        // that are found, and process the files they contain.
        public void ProcessDirectory(string targetDirectory)
        {
            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
            {
                ProcessFile(fileName);
                ArchiveFile(fileName);
            }
        }

        void ProcessFile(string fileName)
        {
            var checks = LoadCheckFile(fileName);
            var mychecks = checks.ToList();
            mychecks.RemoveAll(item => item == null);
            foreach (var check in mychecks)
            {
                _checkService.ProcessCheck(check);
            }

        }
        void ArchiveFile(string fileName)
        {
            var destFileName = String.Format("{0}{1}{2}{3}",
                        ConfigurationManager.AppSettings["ArchivePath"],
                        Path.GetFileNameWithoutExtension(fileName),
                        DateTime.Now.ToString("yyyyMMdd_hhmmss"),
                        Path.GetExtension(fileName));
            File.Move(fileName, destFileName);
        }
        IEnumerable<Check> LoadCheckFile(string path)
        {
            if (File.Exists(path))
            {
                var checks = File.ReadAllLines(path);
                return checks.Select(m=> ConvertCheck(m));
            }
            return Enumerable.Empty<Check>();
        }
        Check ConvertCheck(string line)
        {
            /*  CHECK EACH LINE TO SEE IF ITS A CHECK (TYPE 475) */
            string lineString = line.TrimEnd().TrimEnd('/');
            string[] myLine = lineString.Split(',');

            if(myLine[0] == "02")
            {
                statementID = Convert.ToInt32(myLine[4]);
            }

            if(myLine[0] == "16" && myLine[1] == "475")
            {
                decimal amount = Convert.ToDecimal(myLine[2]) * .01m;
                string amountString = amount.ToString();
                myLine[2] = amountString;


                Check myCheck = new Check();
                myCheck.StatementID = statementID;
                myCheck.CheckNumber = myLine[5];
                myCheck.Amount = Convert.ToDecimal(myLine[2]);

                return myCheck;
            }
            else
            {
                return null;
            }
        }
    }
}
