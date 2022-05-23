using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Data.SqlClient;
using System.Data;
using System.Text;

namespace BoilerPlateWeb
{
    public class Authentication
    {
        public static void Authenticate(Page page)
        {
            if ((user)page.Session["ASL_ENTERPRISE_USER"] == null)
            {
                page.Response.Redirect("Login.aspx");
            }
        }

        public static void SignIn(Page page, string username, string password)
        {
            ReportingEntities re = new ReportingEntities();

            var user = re.users
                        .FirstOrDefault(m => m.userid.Equals(username, StringComparison.OrdinalIgnoreCase)
                                                && m.password == password);

            page.Session.Add("ASL_ENTERPRISE_USER", user);

            Authentication.SetToken(page);

            page.Response.Redirect("Admin.aspx");
        }

        internal static string GetCurrentToken(Page page)
        {
            var sessionUser = Authentication.GetUser(page);

            ReportingEntities re = new ReportingEntities();

            var user = re.users
                            .First(m => m.user_id == sessionUser.user_id);

            return user.token;
        }

        public static string SetToken(Page page)
        {
            int id = ((user)page.Session["ASL_ENTERPRISE_USER"]).user_id;

            ReportingEntities ae = new ReportingEntities();

            var user = ae.users
                            .First(m => m.user_id == id);

            user.token = GetToken(ae);
            user.tokendatetime = DateTime.Now;

            ae.SaveChanges();

            return user.token;
        }

        public static user GetUser(Page page)
        {
            return (user)page.Session["ASL_ENTERPRISE_USER"];
        }

        public static void SignOut(Page page)
        {
            page.Session.Remove("ASL_ENTERPRISE_USER");
            page.Response.Redirect("Login.aspx");
        }

        public static bool LoggedIn(Page page)
        {
            if (page.Session["ASL_ENTERPRISE_USER"] == null)
            {
                return false;
            }
            
            return true;
        }

        private static string GetToken(ReportingEntities ae)
        {
            int length = 32;
            StringBuilder str_build = new StringBuilder();
            string token = null;
            Random random = new Random(DateTime.Now.Millisecond);

            char letter;

            bool foundUniqueToken = false;
            while (!foundUniqueToken)
            {
                str_build.Clear();
                for (int i = 0; i < length; i++)
                {
                    double flt = random.NextDouble();
                    int shift = Convert.ToInt32(Math.Floor(25 * flt));
                    letter = Convert.ToChar(shift + 65);
                    str_build.Append(letter);
                }
                token = str_build.ToString().ToLower();

                var user = ae.users.FirstOrDefault(c => c.token == token);
                if (user == null)
                {
                    foundUniqueToken = true;
                }
            }

            return token;
        }
    }
}