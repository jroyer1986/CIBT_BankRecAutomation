using CIBT_FileReader.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
                LogError(check);
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
                    param1.Value = check.CheckNumber;

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
                Console.WriteLine("Exception: " + ex.Message);
            }

            return foundRecord;
        }
        public void ReconcileCheck(Check check)
        {
            //set the connection string
            var macolaStrEnv1 = SysConfig.ConfigurationManager.AppSettings["MACOLA_CONNECTION"].ToString();
            string connString = SysConfig.ConfigurationManager.ConnectionStrings[macolaStrEnv1].ConnectionString;

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
                    param1.Value = check.CheckNumber;

                    //add the parameter to the SqlCommand object
                    cmd.Parameters.Add(param1);

                    SqlParameter param2 = new SqlParameter();
                    param2.ParameterName = "@amount";
                    param2.SqlDbType = SqlDbType.Decimal;
                    param2.Value = check.Amount;

                    cmd.Parameters.Add(param2);

                    SqlParameter param3 = new SqlParameter();
                    param3.ParameterName = "@statementId";
                    param3.SqlDbType = SqlDbType.Int;
                    param3.Value = check.StatementID;

                    //add the parameter to the SqlCommand object
                    cmd.Parameters.Add(param3);

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
                Console.WriteLine("Exception: " + ex.Message);
            }
        }

        public void LogError(Check check)
        {
            string filePath = @"C:\Text\logs\ErrorLog.txt";

            Exception ex = new Exception();

            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine("-----------------------------------------------------------------------------");
                writer.WriteLine("Date : " + DateTime.Now.ToString());
                writer.WriteLine();


                    writer.WriteLine("ERROR");
                    writer.WriteLine("CHECK ID:"+ check.CheckNumber);

                    ex = ex.InnerException;
            }
        }
    }
}
