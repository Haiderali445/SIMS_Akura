using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace SIMS_Akura.Utilities
{
    public class CodeGenerator
    {
        public static string GenerateCode(string prefix, string tableName, string columnName)
        {
            string query = $"SELECT MAX({columnName}) FROM {tableName}";
            DataTable dt = DBConnection.ExecuteQuery(query);

            string newCode = $"{prefix}-001";
            if (dt.Rows.Count > 0 && dt.Rows[0][0] != DBNull.Value)
            {
                string lastCode = dt.Rows[0][0].ToString();
                int lastNumber = int.Parse(lastCode.Split('-')[1]);
                newCode = $"{prefix}-{(lastNumber + 1):D3}";
            }
            return newCode;
        }
    }
}
