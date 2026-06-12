using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EcoPoinDesktop
{
    public partial class ManageVouchersForm : Form
    {
        private int currentPageInt { get; set; } = 1;
        private readonly System.Windows.Forms.Timer timer1;
        private bool editMode = false;

        public ManageVouchersForm()
        {
            timer1 = new System.Windows.Forms.Timer
            {
                Interval = 300
            };
            timer1.Tick += TrySearch;
            InitializeComponent();
            Helper.LockWindow(this);
            Helper.GenerateColumns(table1, new[] { "Id", "Name", "Code", "Point Cost", "Is Active" }, new[] { "id", "name", "code", "pointCost", "isActive" });
            RefreshData();
            ToggleInput(false);
        }

        async public Task RefreshData(bool reset = false)
        {
            if (reset) currentPageInt = 1;
            var searchStr = UrlEncoder.Default.Encode(search.Text.Trim());
            table1.Enabled = false;
            var url = $"vouchers?page={currentPageInt}";
            if (searchStr != "") url += $"&search={searchStr}";
            var (success, msg, result, paging) = await Helper.PaginatedReq<VoucherRes>(url);
            table1.Enabled = true;
            if (!success || paging == null)
            {
                MessageBox.Show(msg, "Error");
                return;
            }
            table1.DataSource = result;
            currentPage.Text = $"{paging.page} / {paging.totalPage}";
            prev.Enabled = currentPageInt != 1;
            next.Enabled = currentPageInt < paging.totalPage;

        }

        public void insert_Click(object sender, EventArgs e)
        {
            editMode = false;
            ToggleInput(true, true);
        }
        public void delete_Click(object sender, EventArgs e)
        {
            var rec = GetSelected();
            if (rec == null)
            {
                MessageBox.Show("Please select one row");
                return;
            }
            var confirmed = MessageBox.Show($"Are you sure want to delete '{rec.name}'?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes;
            if (confirmed)
            {
                TryDelete();
            }
        }
        public void update_Click(object sender, EventArgs e)
        {
            var rec = GetSelected();
            if (rec == null)
            {
                MessageBox.Show("Please select one row");
                return;
            }
            editMode = true;
            ToggleInput(true);
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

        private void OnSearchChanged(object sender, EventArgs e)
        {
            timer1.Stop();
            timer1.Start();
        }

        private void TrySearch(object? sender, EventArgs e)
        {
            RefreshData(true);
            timer1.Stop();
        }

        private VoucherRes? GetSelected()
        {
            if (table1.SelectedCells.Count == 0) return null;
            return table1.SelectedCells[0].OwningRow.DataBoundItem as VoucherRes;
        }

        private void OnCellClicked(object sender, DataGridViewCellEventArgs e)
        {
            var rec = GetSelected();
            if (rec == null) return;
            voucherName.Text = rec.name;
            code.Text = rec.code;
            pointCost.Text = rec.pointCost.ToString();
            isActive.Checked = rec.isActive;
        }

        private void ToggleInput(bool enabled, bool clear = false)
        {
            if (clear)
            {
                voucherName.Text = "";
                code.Text = "";
                pointCost.Text = "";
                isActive.Checked = false;
            }
            voucherName.ReadOnly = !enabled;
            code.ReadOnly = !enabled;
            pointCost.ReadOnly = !enabled;
            isActive.Enabled = enabled;
            table1.Enabled = !enabled;

            insert.Enabled = !enabled;
            update.Enabled = !enabled;
            delete.Enabled = !enabled;

            cancel.Visible = enabled;
            save.Visible = enabled;
        }

        private void OnCancel(object sender, EventArgs e)
        {
            ToggleInput(false, true);
        }

        private void OnSave(object sender, EventArgs e)
        {
            if (voucherName.Text == "")
            {
                MessageBox.Show("Name required");
                return;
            }
            if (code.Text == "")
            {
                MessageBox.Show("Code required");
                return;
            }
            if(!pointCost.Text.All(Char.IsDigit))
            {
                MessageBox.Show("Point cost not valid");
                return;
            }
            TrySave();
        }

        private async Task TrySave()
        {
            var rec1 = new VoucherReq
            {
                name = voucherName.Text,
                code = code.Text,
                pointCost = Convert.ToInt32(pointCost.Text),
                isActive = isActive.Checked
            };
            if (editMode)
            {
                var rec = GetSelected();
                if (rec == null) return;
                var (isSuccess, msg, res) = await Helper.JsonReq<object, VoucherReq>($"vouchers/{rec.id}", "put", rec1);
                if (!isSuccess)
                {
                    if (msg == "") msg = "Unknown error";
                    MessageBox.Show(msg, "Error");
                    return;
                }
                ToggleInput(false, true);
            }
            else
            {
                var (isSuccess, msg, res) = await Helper.JsonReq<object, VoucherReq>($"vouchers", "post", rec1);
                if (!isSuccess)
                {
                    if (msg == "") msg = "Unknown error";
                    MessageBox.Show(msg, "Error");
                    return;
                }
                ToggleInput(false, true);
            }
            await RefreshData();
        }

        async private Task TryDelete()
        {
            var rec = GetSelected();
            if (rec == null) return;
            var (isSuccess, msg, res) = await Helper.JsonReq<object>($"vouchers/{rec.id}", "delete");
            if (!isSuccess)
            {
                if (msg == "") msg = "Unknown error";
                MessageBox.Show(msg, "Error");
                return;
            }
            await RefreshData();
            ToggleInput(false, true);
        }
    }



    public class VoucherRes
    {
        public int id { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public int pointCost { get; set; }
        public bool isActive { get; set; }
    }




    public class VoucherReq
    {
        public string name { get; set; }
        public string code { get; set; }
        public int pointCost { get; set; }
        public bool isActive { get; set; }
    }



}
