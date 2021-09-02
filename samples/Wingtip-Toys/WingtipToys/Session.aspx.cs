using System;
using System.Web.UI;

namespace WingtipToys
{
    public partial class Session : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var pulledFromSession = Session["CocoonSessionShare"];
            pullFromSession.Text = pulledFromSession?.ToString();
        }
    }
}