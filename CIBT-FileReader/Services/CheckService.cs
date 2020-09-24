using CIBT_FileReader.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SysConfig = System.Configuration;

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
                LogCheckError(check);
            }
        }

        public bool CheckExistsInGlobe(Check check)
        {
            //return true;
            bool foundRecord = false;
            //put stored procedure to check if check exists in globe here
            //set the connection string
            var macolaStrEnv1 = SysConfig.ConfigurationManager.AppSettings["MACOLA_CONNECTION"].ToString();
            string connString = SysConfig.ConfigurationManager.ConnectionStrings[macolaStrEnv1].ConnectionString;

            try
            {
                //sql connection object
                using (SqlConnection conn1 = new SqlConnection(connString))
                {
                    //set stored procedure name
                    string spName = @"dbo.[mgCheckForExistingCheck]";
                    //define the SqlCommand object

                    SqlCommand cmd = new SqlCommand(spName, conn1);
                    //Set SqlParameter - the employee id parameter value will be set from the command line

                    SqlParameter param1 = new SqlParameter();
                    param1.ParameterName = "@checkId";
                    param1.SqlDbType = SqlDbType.Int;
                    param1.Value = Convert.ToInt32(check.CheckNumber);

                    //add the parameter to the SqlCommand object
                    cmd.Parameters.Add(param1);

                    SqlParameter param2 = new SqlParameter();
                    param2.ParameterName = "@amount";
                    param2.SqlDbType = SqlDbType.Decimal;
                    param2.Value = check.Amount;

                    cmd.Parameters.Add(param2);

                    //open connection
                    conn1.Open();
                    //set the SqlCommand type to stored procedure and execute
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlDataReader dr = cmd.ExecuteReader();
                    if(dr.HasRows) {
                        foundRecord = true;
                    }

                    //close data reader
                    dr.Close();

                    //close connection
                    conn1.Close();
                }
            }

            catch (Exception ex)
            {
                //display error message
                LogError(ex);
            }

            return foundRecord;
        }
        public void ReconcileCheck(Check check)
        {
            //set the connection string
            var macolaStrEnv1 = SysConfig.ConfigurationManager.AppSettings["MACOLA_CONNECTION"].ToString();
            string connString = SysConfig.ConfigurationManager.ConnectionStrings[macolaStrEnv1].ConnectionString;

            string dateString = check.StatementID.ToString();
            DateTime date = DateTime.ParseExact(dateString, "yyMMdd", CultureInfo.InvariantCulture);

            try
            {
                //sql connection object
                using (SqlConnection conn1 = new SqlConnection(connString))
                {
                    //set stored procedure name
                    string spName = @"dbo.[mgProcessCheck]";
                    //define the SqlCommand object

                    SqlCommand cmd = new SqlCommand(spName, conn1);
                    //Set SqlParameter - the employee id parameter value will be set from the command line

                    SqlParameter param1 = new SqlParameter();
                    param1.ParameterName = "@checkId";
                    param1.SqlDbType = SqlDbType.Int;
                    param1.Value = Convert.ToInt32(check.CheckNumber);

                    //add the parameter to the SqlCommand object
                    cmd.Parameters.Add(param1);

                    SqlParameter param2 = new SqlParameter();
                    param2.ParameterName = "@amount";
                    param2.SqlDbType = SqlDbType.Decimal;
                    param2.Value = check.Amount;

                    //add the parameter to the SqlCommand object
                    cmd.Parameters.Add(param2);

                    SqlParameter param3 = new SqlParameter();
                    param3.ParameterName = "@statementId";
                    param3.SqlDbType = SqlDbType.Int;
                    param3.Value = check.StatementID;

                    //add the parameter to the SqlCommand object
                    cmd.Parameters.Add(param3);

                    SqlParameter param4 = new SqlParameter();
                    param4.ParameterName = "@statementDate";
                    param4.SqlDbType = SqlDbType.DateTime;
                    param4.Value = date;

                    //add the parameter to the SqlCommand object
                    cmd.Parameters.Add(param4);

                    //open connection
                    conn1.Open();
                    //set the SqlCommand type to stored procedure and execute
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlDataReader dr = cmd.ExecuteReader();

                    //close data reader
                    dr.Close();

                    //close connection
                    conn1.Close();
                }
            }

            catch (Exception ex)
            {
                //display error message
                LogError(ex);
            }
        }

        public void LogCheckError(Check check)
        {
            string filePath = SysConfig.ConfigurationManager.AppSettings["PathToErrorLog"].ToString();
            System.IO.FileInfo file = new System.IO.FileInfo(filePath);
            file.Directory.Create(); // If the directory already exists, this method does nothing.

            Exception ex = new Exception();

            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine("-----------------------------------------------------------------------------");
                writer.WriteLine("Date : " + DateTime.Now.ToString());
                writer.WriteLine();


                writer.WriteLine("ERROR");
                writer.WriteLine("STATEMENT NUMBER: " + check.StatementID);
                writer.WriteLine("CHECK ID [TransactionNumber]: " + check.CheckNumber + "; CHECK AMT [AmountTC]: " + check.Amount + "; TYPE ID: " + check.Type);
                writer.WriteLine("Could not find Check in Globe, or could not successfully update BankTransaction with statementNumber");

                ex = ex.InnerException;
            }
        }

        public void LogError(Exception ex)
        {
            string filePath = SysConfig.ConfigurationManager.AppSettings["PathToErrorLog"].ToString();
            System.IO.FileInfo file = new System.IO.FileInfo(filePath);
            file.Directory.Create(); // If the directory already exists, this method does nothing.

            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine("-----------------------------------------------------------------------------");
                writer.WriteLine("Date : " + DateTime.Now.ToString());
                writer.WriteLine();

                writer.WriteLine(ex.GetType().FullName);
                writer.WriteLine("Message : " + ex.Message);

                ex = ex.InnerException;
            }
        }
    }
}
