using EcoPoinDesktop.Forms;
using System.Diagnostics;

namespace EcoPoinDesktop
{
    public partial class Login : Form
    {
        public Login()
        {
            var loginToken = Properties.Settings.Default.LoginToken;
            TryLogin(loginToken);
            InitializeComponent();
            Helper.LockWindow(this);
        }

        private void OnTryLogin(object sender, EventArgs e)
        {
            if (username.Text == "")
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

        async private Task TryLogin(string token)
        {
            if (token == "") return;
            Helper.loginToken = token;
            if (!await Helper.Profile())
            {
                Helper.loginToken = "";
                Properties.Settings.Default.LoginToken = "";
                Properties.Settings.Default.Save();
                return;
            }
            var window = new HomeForm();
            Hide();
            window.Show();
            window.FormClosed += (s, e) =>
            {
                Show();
            };
        }

        async private Task TryLogin()
        {
            button1.Enabled = false;
            button1.Text = "Loading...";
            var (isSuccess, msg, res) = await Helper.JsonReq<LoginRes, LoginReq>("users/login", "post", new LoginReq { username = username.Text, password = password.Text });
            button1.Enabled = true;
            button1.Text = "Login";
            if (isSuccess && res != null)
            {
                Helper.loginToken = res.token;
                if(!await Helper.Profile())
                {
                    MessageBox.Show("Can't fetch profile data", "Error");
                    return;
                }
                Properties.Settings.Default.LoginToken = res.token;
                Properties.Settings.Default.Save();
                var window = new HomeForm();
                username.Text = "";
                password.Text = "";
                Hide();
                window.Show();
                window.FormClosed += (s, e) =>
                {
                    Show();
                };
            }
            else
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

    public class LoginRes
    {
        public int id { get; set; }
        public string fullName { get; set; }
        public string username { get; set; }
        public string role { get; set; }
        public string token { get; set; }
    }


    public class ProfileRes
    {
        public int id { get; set; }
        public string username { get; set; }
        public string fullName { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string role { get; set; }
        public int balance { get; set; }
        public int environmentalImpact { get; set; }
    }

}
