using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EcoPoinDesktop.Forms
{
    public partial class HomeForm : Form
    {
        private bool logout = false;
        private readonly System.Windows.Forms.Timer timer;

        public HomeForm()
        {
            timer = new System.Windows.Forms.Timer
            {
                Interval = 1000,
            };
            InitializeComponent();
            greeterLb.Text = $"Hello, {Helper.session?.fullName}!";
            OnUpdateDateTimeLb(null, null);
            timer.Tick += OnUpdateDateTimeLb;
            timer.Start();
            Helper.LockWindow(this);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            var res = MessageBox.Show("Are you sure?", "Confirmation", MessageBoxButtons.YesNo);
            e.Cancel = res == DialogResult.No;
        }

        protected override void OnClosed(EventArgs e)
        {
            Helper.session = null;
            if (!logout) Application.Exit();
        }

        private void OnUpdateDateTimeLb(object? sender, EventArgs e)
        {
            datetimeLb.Text = DateTime.Now.ToString("dddd, dd MMMM yyyy (HH:mm:ss)");
        }

        private void OnManageUsers(object sender, EventArgs e)
        {

        }

        private void OnExchangeVoucher(object sender, EventArgs e)
        {

        }

        private void OnViewReports(object sender, EventArgs e)
        {

        }

        private void OnManageWasteTypes(object sender, EventArgs e)
        {

        }

        private void OnManageVouchers(object sender, EventArgs e)
        {

        }

        private void OnLogOut(object sender, EventArgs e)
        {
            logout = true;
            Close();
        }
    }
}
