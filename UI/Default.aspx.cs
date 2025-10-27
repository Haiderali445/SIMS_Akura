using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SIMS_Akura.Utilities;

namespace SIMS_Akura.UI
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    DataTable dt = DBConnection.ExecuteQuery("SELECT GETDATE() AS ServerTime");

                    if (dt.Rows.Count > 0)
                    {
                        string serverTime = dt.Rows[0]["ServerTime"].ToString();
                        Response.Write("<h3 style='color:green;'>✅ Database Connected Successfully!</h3>");
                        Response.Write("<p>Server Time: " + serverTime + "</p>");
                    }
                }
                catch (Exception ex)
                {
                    Response.Write("<h3 style='color:red;'>❌ Connection Failed:</h3>");
                    Response.Write("<pre>" + ex.Message + "</pre>");
                }
            }
        }

    }
}