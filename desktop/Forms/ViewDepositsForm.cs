using EcoPoinDesktop.UserControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EcoPoinDesktop.Forms
{
    public partial class ViewDepositsForm : Form
    {
        private int currentPageInt { get; set; } = 1;
        public ViewDepositsForm()
        {
            InitializeComponent();
            Helper.LockWindow(this);
            RefreshData();
        }

        async private Task RefreshData()
        {
            deposits.Enabled = false;
            var url = $"deposits?page={currentPageInt}";
            var (success, msg, result, paging) = await Helper.PaginatedReq<DepositRes>(url);
            deposits.Enabled = true;
            if (!success || paging == null)
            {
                if (msg == "") msg = "Unknown error";
                MessageBox.Show(msg, "Error");
                return;
            }
            deposits.Controls.Clear();
            foreach (var item in result)
            {
                var card = new DepositCard(item);
                deposits.Controls.Add(card);
                card.Click += (s, e) =>
                {
                    var window = new DepositDetailForm(item.id);
                    window.ShowDialog();
                };
            }
            currentPage.Text = $"{paging.page} / {paging.totalPage}";
            prev.Enabled = currentPageInt != 1;
            next.Enabled = currentPageInt < paging.totalPage;
        }

        private void OnPrev(object sender, EventArgs e)
        {
            currentPageInt -= 1;
            RefreshData();
        }

        private void OnNext(object sender, EventArgs e)
        {
            currentPageInt += 1;
            RefreshData();
        }
    }
}
