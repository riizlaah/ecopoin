using EcoPoinDesktop.Forms;
using System.Diagnostics;

namespace EcoPoinDesktop
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
            Helper.LockWindow(this);
        }

        private void OnTryLogin(object sender, EventArgs e)
        {
            if(username.Text == "")
            {
                MessageBox.Show("Username required");
                return;
            }
            if (password.Text == "")
            {
                MessageBox.Show("Password required");
                return;
            }
            TryLogin();
        }

        async private Task TryLogin()
        {
            button1.Enabled = false;
            button1.Text = "Loading...";
            var (isSuccess, msg, res) = await Helper.JsonReq<LoginRes, LoginReq>("users/login", "post", new LoginReq { username = username.Text, password = password.Text});
            button1.Enabled = true;
            button1.Text = "Login";
            if (isSuccess)
            {
                Helper.session = res;
                var window = new HomeForm();
                username.Text = "";
                password.Text = "";
                Hide();
                window.Show();
                window.FormClosed += (s, e) =>
                {
                    Show();
                };
            } else
            {
                MessageBox.Show(msg);
            }
        }
    }

    public class LoginReq
    {
        public string username { get; set; }
        public string password { get; set; }
    }
}
