using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIMS_Akura.Utilities
{
    public class SecurityManager
    {
        public static bool IsUserLoggedIn()
        {
            return HttpContext.Current.Session["UserID"] != null;
        }

        public static string GetUserRole()
        {
            return HttpContext.Current.Session["UserRole"]?.ToString() ?? "Guest";
        }

        public static void EnforceRole(string requiredRole)
        {
            string currentRole = GetUserRole();

            if (!currentRole.Equals(requiredRole, StringComparison.OrdinalIgnoreCase)
                && !currentRole.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                HttpContext.Current.Response.Redirect("~/UI/AccessDenied.aspx");
            }
        }

        public static void Logout()
        {
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Response.Redirect("~/UI/Login.aspx");
        }
    }
}