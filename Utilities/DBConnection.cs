using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace SIMS_Akura.Utilities
{
    public static class DBConnection
    {
        private static readonly string conString =
            ConfigurationManager.ConnectionStrings["db_sims_akura"].ConnectionString;

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(conString);
        }

        public static int ExecuteNonQuery(string query, SqlParameter[] parameters = null, bool isStoredProc = true)
        {
            using (SqlConnection con = GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.CommandType = isStoredProc ? CommandType.StoredProcedure : CommandType.Text;
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);

                    con.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public static DataTable ExecuteQuery(string query, SqlParameter[] parameters = null, bool isStoredProc = true)
        {
            using (SqlConnection con = GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.CommandType = isStoredProc ? CommandType.StoredProcedure : CommandType.Text;
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }
    }
}
