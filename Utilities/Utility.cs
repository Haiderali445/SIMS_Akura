using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace SIMS_Akura.Utilities
{
    public class Utility
    {
        public static void ShowAlert(Page page, string message)
        {
            string script = $"alert('{message.Replace("'", "\\'")}');";
            ScriptManager.RegisterStartupScript(page, page.GetType(), "alert", script, true);
        }

        public static string FormatCurrency(decimal amount)
        {
            return string.Format("{0:C2}", amount);
        }

        public static string FormatDate(DateTime date)
        {
            return date.ToString("dd-MMM-yyyy");
        }
    }
}