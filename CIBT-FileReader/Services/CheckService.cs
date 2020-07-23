using CIBT_FileReader.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
            //put stored procedure to check if check exists in globe here
            return true;
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
                    string spName = @"dbo.[uspEmployeeInfo]";

                    //define the SqlCommand object
                    SqlCommand cmd = new SqlCommand(spName, conn1);

                    //Set SqlParameter - the employee id parameter value will be set from the command line
                    SqlParameter param1 = new SqlParameter();
                    param1.ParameterName = "@employeeID";
                    param1.SqlDbType = SqlDbType.Int;
                    param1.Value = int.Parse(args[0].ToString());

                    //add the parameter to the SqlCommand object
                    cmd.Parameters.Add(param1);

                    //open connection
                    conn1.Open();

                    //set the SqlCommand type to stored procedure and execute
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader dr = cmd.ExecuteReader();

                    Console.WriteLine(Environment.NewLine + "Retrieving data from database..." + Environment.NewLine);
                    Console.WriteLine("Retrieved records:");

                    //check if there are records
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            empID = dr.GetInt32(0);
                            empCode = dr.GetString(1);
                            empFirstName = dr.GetString(2);
                            empLastName = dr.GetString(3);
                            locationCode = dr.GetString(4);
                            locationDescr = dr.GetString(5);

                            //display retrieved record
                            Console.WriteLine("{0},{1},{2},{3},{4},{5}", empID.ToString(), empCode, empFirstName, empLastName, locationCode, locationDescr);
                        }
                    }
                    else
                    {
                        Console.WriteLine("No data found.");
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









            var macolaStrEnv = SysConfig.ConfigurationManager.AppSettings["MACOLA_CONNECTION"].ToString();
            string macolaConnectionString = SysConfig.ConfigurationManager.ConnectionStrings[macolaStrEnv].ConnectionString;

            SqlConnection conn = null;
            SqlDataReader rdr = null;

            try
            {
                conn = new SqlConnection(macolaConnectionString);
                conn.Open();
                SqlCommand cmd = new SqlCommand("dbo.msmProcessUploadedCheck", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                rdr = cmd.ExecuteReader();
                /*while (rdr.Read())
                {
                    Console.WriteLine(
                        "Product: {0,-25} Price: ${1,6:####.00}",
                        rdr["TenMostExpensiveProducts"],
                        rdr["UnitPrice"]);
                }*/
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
                if (rdr != null)
                {
                    rdr.Close();
                }
            }
        }
        public void LogError(Check check)
        {

        }
    }
}
