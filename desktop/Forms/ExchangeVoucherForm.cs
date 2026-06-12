using EcoPoinDesktop.UserControls;
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
    public partial class ExchangeVoucherForm : Form
    {
        private VoucherRes? currentVoucher { get; set; } = null;
        public ExchangeVoucherForm()
        {
            InitializeComponent();
            voucherData.Hide();
        }

        private void onTryUse(object sender, EventArgs e)
        {
            if (currentVoucher == null)
            {
                CheckVoucher();
            }
            else
            {
                TryUse();
            }
        }

        async private Task CheckVoucher()
        {
            var (isSuccess, msg, res) = await Helper.JsonReq<VoucherRes>($"vouchers/{voucherCode.Text}/detail");
            if (!isSuccess || res == null)
            {
                useBtn.Text = "Check";
                voucherData.Hide();
                if (msg == "") msg = "Unknown error";
                MessageBox.Show(msg, "Error");
                return;
            }
            voucherData.Show();
            useBtn.Text = "Use";
            currentVoucher = res;
            voucherName.Text = $"Name : {res.voucher.name}";
            pointCost.Text = $"Point Cost : {res.voucher.pointCost}";
            residentName.Text = $"Resident Name : {res.resident.fullName}";
            isUsed.Text = $"Is Used : " + (res.isUsed ? "Yes" : "No");
            if (res.isUsed)
            {
                usedAt.Text = $"Used At : {res.updatedAt}";
                useBtn.Enabled = false;
            }
            else usedAt.Hide();
            exchangedAt.Text = $"Exchanged At : {res.createdAt}";
        }

        async private Task TryUse()
        {
            var confirmed = MessageBox.Show("Are you sure?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes;
            if (!confirmed) return;
            var (isSuccess, msg, res) = await Helper.JsonReq<object, object>($"vouchers/{currentVoucher?.id}/use", "post", null);
            if (!isSuccess)
            {
                useBtn.Text = "Check";
                voucherData.Hide();
                if (msg == "") msg = "Unknown error";
                MessageBox.Show(msg, "Error");
                return;
            }
            MessageBox.Show("Voucher exchanged successfully", "Info");
            currentVoucher = null;
        }

        private void exchangedAt_Click(object sender, EventArgs e)
        {

        }

        private void OnVoucherCodeChanged(object sender, EventArgs e)
        {
            if(currentVoucher != null && currentVoucher.code != voucherCode.Text)
            {
                currentVoucher = null;
                useBtn.Text = "Check";
                voucherData.Hide();
            }
        }
    }


    public class VoucherRes
    {
        public int id { get; set; }
        public BaseVoucherRes voucher { get; set; }
        public ResidentRes2 resident { get; set; }
        public string code { get; set; }
        public int amount { get; set; }
        public bool isUsed { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
    }

    public class BaseVoucherRes
    {
        public int id { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public int pointCost { get; set; }
        public bool isActive { get; set; }
    }

    public class ResidentRes2
    {
        public int id { get; set; }
        public string fullName { get; set; }
        public string email { get; set; }
    }

}
