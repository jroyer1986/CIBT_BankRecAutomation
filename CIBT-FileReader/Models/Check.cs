using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIBT_FileReader.Models
{
    public class Check
    {
        public string AccountNumber
        { get; set; }
        public string CheckNumber
        { get; set; }
        public decimal Amount
        { get; set; }
        public int StatementID
        { get; set; }
        public int Type
        { get; set; }
    }
}
