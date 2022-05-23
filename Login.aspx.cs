using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BoilerPlateWeb
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Universal();
            if (!IsPostBack)
            {
                FreshLoad();
            }
        }

        private void FreshLoad()
        {

        }

        private void Universal()
        {
            alertLabel.Text = "";
        }

        protected void loginButton_Click(object sender, EventArgs e)
        {
            try
            {
                Authentication.SignIn(this, userTB.Text, pwdTB.Text);
            }
            catch (Exception er)
            {
                alertLabel.Text = "Username/password invalid.  Try again.";
            }
        }
    }
}