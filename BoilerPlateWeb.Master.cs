using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BoilerPlateWeb
{
    public partial class BoilerPlateWeb : System.Web.UI.MasterPage
    {
        user user = null;
        string currentURL = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            DoUniversal();
            if (!IsPostBack)
            {
                DoFreshLoad();
            }
        }

        private void DoUniversal()
        {
            currentURL = HttpContext.Current.Request.Url.AbsoluteUri;
            if (currentURL.Contains("Login.aspx"))
            {
                if (Authentication.GetUser(this.Page) != null)
                {
                    Page.Response.Redirect("Admin.aspx");
                }
                menu1.Visible = false;
                sitemap.Visible = false;
                signoutB.Visible = false;
            }
            else
            {
                Authentication.Authenticate(this.Page);
                user = Authentication.GetUser(this.Page);
            }
        }

        private void DoFreshLoad()
        {

        }

        protected void signoutB_Click(object sender, EventArgs e)
        {
            Authentication.SignOut(this.Page);
        }

        protected void enterpriseHomeLB_Click(object sender, EventArgs e)
        {
            if (Authentication.LoggedIn(this.Page))
            {
                string token = Authentication.SetToken(this.Page);
                Response.Redirect("http://10.10.20.4/enterprisemastersite/enterprisemastersite/landingpage.aspx?token=" + token);
            }
            else
            {
                Response.Redirect("http://10.10.20.4/enterprisemastersite/enterprisemastersite/login.aspx");
            }
        }
    }
}