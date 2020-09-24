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

            System.IO.FileInfo file = new System.IO.FileInfo(ConfigurationManager.AppSettings["ArchivePath"]);
            file.Directory.Create(); // If the directory already exists, this method does nothing.
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
            List<string> typesToProcessPositive = new List<string>();
            //typesToProcessPositive.Add("158"); --Could not find
            typesToProcessPositive.Add("169");
            typesToProcessPositive.Add("195");
            typesToProcessPositive.Add("201");
            typesToProcessPositive.Add("208");
            typesToProcessPositive.Add("277");
            typesToProcessPositive.Add("301");

            List<string> typesToProcessNegative = new List<string>();
            typesToProcessNegative.Add("469");
            typesToProcessNegative.Add("475");
            typesToProcessNegative.Add("495");
            typesToProcessNegative.Add("506");
            typesToProcessNegative.Add("508");
            typesToProcessNegative.Add("577");
            typesToProcessNegative.Add("698");
            typesToProcessNegative.Add("699");

            /*  CHECK EACH LINE TO SEE IF ITS ONE TO PROCESS (TYPES 158 169 195 201 208 277 301 469 475 495 506 508 577 698 699) */
            string lineString = line.TrimEnd().TrimEnd('/');
            string[] myLine = lineString.Split(',');

            if(myLine[0] == "02")
            {
                statementID = Convert.ToInt32(myLine[4]);
            }

            if(myLine[0] == "16" && (typesToProcessPositive.Contains(myLine[1]) || typesToProcessNegative.Contains(myLine[1])))
            {
                decimal amount = Convert.ToDecimal(myLine[2]) * .01m;
                //add check to multiply by -1 for certain types
                if(typesToProcessNegative.Contains(myLine[1]))
                {
                    amount = amount * -1;
                }
                string amountString = amount.ToString();
                myLine[2] = amountString;


                Check myCheck = new Check();
                myCheck.StatementID = statementID;
                myCheck.CheckNumber = myLine[5];
                myCheck.Amount = Convert.ToDecimal(myLine[2]);
                myCheck.Type = Convert.ToInt32(myLine[1]);
                return myCheck;
            }
            else
            {
                return null;
            }
        }
    }
}
