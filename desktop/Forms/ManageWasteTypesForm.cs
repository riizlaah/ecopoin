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
    public partial class ManageWasteTypesForm : Form
    {
        private int currentPageInt { get; set; } = 1;
        private readonly System.Windows.Forms.Timer timer1;
        private bool editMode = false;

        public ManageWasteTypesForm()
        {
            timer1 = new System.Windows.Forms.Timer
            {
                Interval = 300
            };
            timer1.Tick += TrySearch;
            InitializeComponent();
            Helper.LockWindow(this);
            Helper.GenerateColumns(table1, new[] { "Id", "Name", "Code", "Point Tariff", "CO2 Factor", "Is Active" }, new[] { "id", "name", "code", "pointTariff", "cO2Factor", "isActive" });
            RefreshData();
            ToggleInput(false);
        }

        async public Task RefreshData(bool reset = false)
        {
            if (reset) currentPageInt = 1;
            var searchStr = UrlEncoder.Default.Encode(search.Text.Trim());
            table1.Enabled = false;
            var url = $"wastetypes?page={currentPageInt}";
            if (searchStr != "") url += $"&search={searchStr}";
            var (success, msg, result, paging) = await Helper.PaginatedReq<WasteTypeRes>(url);
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

        private WasteTypeRes? GetSelected()
        {
            if (table1.SelectedCells.Count == 0) return null;
            return table1.SelectedCells[0].OwningRow.DataBoundItem as WasteTypeRes;
        }

        private void OnCellClicked(object sender, DataGridViewCellEventArgs e)
        {
            var rec = GetSelected();
            if (rec == null) return;
            wasteTypeName.Text = rec.name;
            code.Text = rec.code;
            pointTariff.Text = rec.pointTariff.ToString();
            co2Factor.Text = rec.cO2Factor.ToString();
            isActive.Checked = rec.isActive;
        }

        private void ToggleInput(bool enabled, bool clear = false)
        {
            if (clear)
            {
                wasteTypeName.Text = "";
                code.Text = "";
                pointTariff.Text = "";
                co2Factor.Text = "";
                isActive.Checked = false;
            }
            wasteTypeName.ReadOnly = !enabled;
            code.ReadOnly = !enabled;
            pointTariff.ReadOnly = !enabled;
            co2Factor.ReadOnly = !enabled;
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
            if (wasteTypeName.Text == "")
            {
                MessageBox.Show("Name required");
                return;
            }
            if (code.Text == "")
            {
                MessageBox.Show("Code required");
                return;
            }
            if (!pointTariff.Text.All(Char.IsDigit))
            {
                MessageBox.Show("Point tariff not valid");
                return;
            }
            if (!decimal.TryParse(co2Factor.Text, out decimal _))
            {
                MessageBox.Show("CO2 Factor not valid");
                return;
            }
            TrySave();
        }

        private async Task TrySave()
        {
            var rec1 = new WasteTypeReq
            {
                name = wasteTypeName.Text,
                code = code.Text,
                pointTariff = Convert.ToInt32(pointTariff.Text),
                cO2Factor = Convert.ToDecimal(pointTariff.Text),
                isActive = isActive.Checked
            };
            if (editMode)
            {
                var rec = GetSelected();
                if (rec == null) return;
                var (isSuccess, msg, res) = await Helper.JsonReq<object, WasteTypeReq>($"wastetypes/{rec.id}", "put", rec1);
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
                var (isSuccess, msg, res) = await Helper.JsonReq<object, WasteTypeReq>($"wastetypes", "post", rec1);
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
            var (isSuccess, msg, res) = await Helper.JsonReq<object>($"wastetypes/{rec.id}", "delete");
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




    public class WasteTypeRes
    {
        public int id { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public int pointTariff { get; set; }
        public decimal cO2Factor { get; set; }
        public bool isActive { get; set; }
    }





    public class WasteTypeReq
    {
        public string name { get; set; }
        public string code { get; set; }
        public int pointTariff { get; set; }
        public decimal cO2Factor { get; set; }
        public bool isActive { get; set; }
    }



}
