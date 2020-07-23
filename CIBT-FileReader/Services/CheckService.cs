using CIBT_FileReader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIBT_FileReader.Services
{
    public class CheckService
    {
        public void ProcessCheck(Check check)
        {
            //see if check exists in the globe
            if (CheckExistsInGlobe(check))
            {
                ReconcileCheck(check);
            }
            else
            {
                LogError(check);
            }
        }

        public bool CheckExistsInGlobe(Check check)
        {
            //put stored procedure to check if check exists in globe here
            return true;
        }
        public void ReconcileCheck(Check check)
        {
            //call stored procedure to reconcile check by marking check with statementID
            
        }
        public void LogError(Check check)
        {

        }
    }
}
