using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BoilerPlateWeb
{
    public partial class LandingPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DoUniversal();
        }

        private void DoUniversal()
        {
            alertLabel.Text = "";

            try
            {
                if (Request.QueryString != null)
                {
                    if (Request.QueryString.Keys != null)
                    {
                        if (Request.QueryString.AllKeys
                                    .FirstOrDefault(m => m == "token") != null)
                        {
                            if (Request.QueryString["token"] != null)
                            {
                                string token = Request.QueryString["token"];

                                ReportingEntities re = new ReportingEntities();

                                var user = re.users
                                                .FirstOrDefault(m => m.token == token);

                                if (user != null)
                                {
                                    if ((DateTime.Now - user.tokendatetime.Value).TotalSeconds <= 180)
                                    {
                                        Session.Add("ASL_ENTERPRISE_USER", user);
                                    }
                                }
                            }
                        }
                    }
                }

                Response.Redirect("Login.aspx");
            }
            catch (Exception ex)
            {
                alertLabel.Text = ex.Message;
            }
        }
    }
}