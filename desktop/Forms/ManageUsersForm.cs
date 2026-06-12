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
    public partial class ManageUsersForm : Form
    {
        private int currentPageInt { get; set; } = 1;
        private readonly System.Windows.Forms.Timer timer1;
        private bool editMode = false;

        public ManageUsersForm()
        {
            timer1 = new System.Windows.Forms.Timer
            {
                Interval = 300
            };
            timer1.Tick += TrySearch;
            InitializeComponent();
            Helper.LockWindow(this);
            filterRoles.DataSource = new[] { "All", "Admin", "Officer", "Resident" };
            roles.DataSource = new[] { "admin", "officer", "resident" };
            filterRoles.SelectedIndex = 0;
            Helper.GenerateColumns(table1, new[] { "Id", "Full Name", "Username", "Email", "Phone", "Role" }, new[] { "id", "fullName", "username", "email", "phone", "role" });
            RefreshData();
            ToggleInput(false);
        }

        async public Task RefreshData(bool reset = false)
        {
            if (reset) currentPageInt = 1;
            var searchStr = UrlEncoder.Default.Encode(search.Text.Trim());
            var role = filterRoles.SelectedItem?.ToString() == "All" ? "" : filterRoles.SelectedItem?.ToString()?.ToLower() ?? "";
            table1.Enabled = false;
            var url = $"users?page={currentPageInt}";
            if (searchStr != "") url += $"&search={searchStr}";
            if (role != "") url += $"&role={role}";
            var (success, msg, result, paging) = await Helper.PaginatedReq<UserRes>(url);
            table1.Enabled = true;
            if (!success || paging == null)
            {
                MessageBox.Show(msg, "Error");
                return;
            }
            table1.DataSource = result;
            currentPage.Text = $"{paging.page} / {paging.totalPage}";
            //currentPageInt = reset ? 1 : paging.page;
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
            var confirmed = MessageBox.Show($"Are you sure want to delete '{rec.fullName}'?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes;
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

        private void OnFilterRolesChanged(object sender, EventArgs e)
        {
            RefreshData(true);
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

        private UserRes? GetSelected()
        {
            if (table1.SelectedCells.Count == 0) return null;
            return table1.SelectedCells[0].OwningRow.DataBoundItem as UserRes;
        }

        private void OnCellClicked(object sender, DataGridViewCellEventArgs e)
        {
            var rec = GetSelected();
            if (rec == null) return;
            username.Text = rec.username;
            fullName.Text = rec.fullName;
            email.Text = rec.email;
            phone.Text = rec.phone;
            roles.Text = rec.role;
        }

        private void ToggleInput(bool enabled, bool clear = false)
        {
            if (clear)
            {
                username.Text = "";
                fullName.Text = "";
                email.Text = "";
                phone.Text = "";
                password.Text = "";
                roles.SelectedIndex = 0;
            }
            username.ReadOnly = !enabled;
            fullName.ReadOnly = !enabled;
            email.ReadOnly = !enabled;
            phone.ReadOnly = !enabled;
            password.ReadOnly = !enabled;
            roles.Enabled = enabled;
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
            if (username.Text == "")
            {
                MessageBox.Show("Username required");
                return;
            }
            if (fullName.Text == "")
            {
                MessageBox.Show("Full name required");
                return;
            }
            if (email.Text == "")
            {
                MessageBox.Show("Email required");
                return;
            }
            if (phone.Text == "")
            {
                MessageBox.Show("Phone required");
                return;
            }
            if (!editMode)
            {
                if (password.Text == "")
                {
                    MessageBox.Show("Password required");
                    return;
                }
            }
            if (roles.SelectedValue == null || roles.SelectedValue?.ToString() == "")
            {
                MessageBox.Show("Role required");
                return;
            }
            TrySave();
        }

        private async Task TrySave()
        {
            var rec1 = new UserReq
            {
                username = username.Text,
                fullName = fullName.Text,
                phone = phone.Text,
                email = email.Text,
                password = password.Text,
                role = roles.Text
            };
            if (editMode)
            {
                var rec = GetSelected();
                if (rec == null) return;
                var (isSuccess, msg, res) = await Helper.JsonReq<object, UserReq>($"users/{rec.id}", "put", rec1);
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
                var (isSuccess, msg, res) = await Helper.JsonReq<object, UserReq>($"users", "post", rec1);
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
            var (isSuccess, msg, res) = await Helper.JsonReq<object>($"users/{rec.id}", "delete");
            if (!isSuccess)
            {
                if (msg == "") msg = "Unknown error";
                MessageBox.Show(msg, "Error");
                return;
            }
            await RefreshData();
            ToggleInput(false, true);
        }

        private void OnTogglePassword(object sender, EventArgs e)
        {
            password.PasswordChar = showPassword.Checked ? '\0' : '*';
        }
    }


    public class UserRes
    {
        public int id { get; set; }
        public string username { get; set; }
        public string fullName { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string role { get; set; }
    }


    public class UserReq
    {
        public string username { get; set; }
        public string fullName { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string password { get; set; }
        public string role { get; set; }
    }


}
