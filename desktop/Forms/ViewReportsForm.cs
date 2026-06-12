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
    public partial class ViewReportsForm : Form
    {
        private int leaderboardPage { get; set; } = 1;
        private int wasteTypePage { get; set; } = 1;
        private int voucherPage { get; set; } = 1;
        public ViewReportsForm()
        {
            InitializeComponent();
            Helper.LockWindow(this);
            Helper.GenerateColumns(leaderboard, ["No.", "Full Name", "Total Points", "Current Balance", "CO2 Saved"], ["rank", "fullName", "totalPoints", "currentBalance", "environmentalImpact"]);
            Helper.GenerateColumns(wasteTypes, ["No.", "Name", "Total Weight", "Total Points"], ["rank", "name", "totalWeight", "totalPoints"]);
            Helper.GenerateColumns(vouchers, ["No.", "Name", "Total Redemption", "Total Points"], ["rank", "name", "totalRedemption", "totalPointsRedeemed"]);
            RefreshPoints();
            RefreshLeaderboard();
            RefreshWasteTypes();
            RefreshVouchers();
        }

        async private Task RefreshPoints()
        {
            var (isSuccess, msg, res) = await Helper.JsonReq<PointsReport>("points");
            if (!isSuccess || res == null)
            {
                if (msg == "") msg = "Unknown error";
                MessageBox.Show(msg, "Error");
                return;
            }
            totalPoints.Text = $"Total Points : {res.totalPoints}";
            totalPointsRedeemed.Text = $"Total Points Redeemed : {res.redeemedPoints}";
            pointsOutstanding.Text = $"Points Outstanding : {res.pointOutstanding}";
        }

        async private Task RefreshLeaderboard()
        {
            var (isSuccess, msg, res, paging) = await Helper.PaginatedReq<LeaderboardRes>($"leaderboard?page={leaderboardPage}");
            if (!isSuccess || res == null || paging == null)
            {
                if (msg == "") msg = "Unknown error";
                MessageBox.Show(msg, "Error");
                return; 
            }
            leaderboard.DataSource = res;
            pagingL.Text = $"{leaderboardPage} / {paging.totalPage}";
            prevL.Enabled = leaderboardPage > 1;
            nextL.Enabled = leaderboardPage < paging.totalPage;
        }

        async private Task RefreshWasteTypes()
        {
            var (isSuccess, msg, res, paging) = await Helper.PaginatedReq<WasteTypeRes2>($"wastetypes/top?page={wasteTypePage}");
            if (!isSuccess || res == null || paging == null)
            {
                if (msg == "") msg = "Unknown error";
                MessageBox.Show(msg, "Error");
                return;
            }
            wasteTypes.DataSource = res;
            pagingW.Text = $"{wasteTypePage} / {paging.totalPage}";
            prevW.Enabled = wasteTypePage > 1;
            nextW.Enabled = wasteTypePage < paging.totalPage;
        }

        async private Task RefreshVouchers()
        {
            var (isSuccess, msg, res, paging) = await Helper.PaginatedReq<VoucherRes2>($"vouchers/top?page={voucherPage}");
            if (!isSuccess || res == null || paging == null)
            {
                if (msg == "") msg = "Unknown error";
                MessageBox.Show(msg, "Error");
                return;
            }
            vouchers.DataSource = res;
            pagingV.Text = $"{voucherPage} / {paging.totalPage}";
            prevV.Enabled = voucherPage > 1;
            nextV.Enabled = voucherPage < paging.totalPage;
        }


        private void OnNextLeaderboard(object sender, EventArgs e)
        {
            leaderboardPage += 1;
            RefreshLeaderboard();
        }

        private void OnPrevLeaderboard(object sender, EventArgs e)
        {
            leaderboardPage -= 1;
            RefreshLeaderboard();
        }

        private void OnNextWasteType(object sender, EventArgs e)
        {
            wasteTypePage += 1;
            RefreshWasteTypes();
        }

        private void OnPrevWasteType(object sender, EventArgs e)
        {
            wasteTypePage -= 1;
            RefreshWasteTypes();
        }

        private void OnPrevVoucher(object sender, EventArgs e)
        {
            voucherPage -= 1;
            RefreshWasteTypes();
        }

        private void OnNextVoucher(object sender, EventArgs e)
        {
            voucherPage += 1;
            RefreshWasteTypes();
        }
    }


    public class PointsReport
    {
        public int totalPoints { get; set; }
        public int redeemedPoints { get; set; }
        public int pointOutstanding { get; set; }
    }


    public class LeaderboardRes
    {
        public int rank { get; set; }
        public string fullName { get; set; }
        public int totalPoints { get; set; }
        public int currentBalance { get; set; }
        public decimal environmentalImpact { get; set; }
    }


    public class VoucherRes2
    {
        public int rank { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public int totalRedemption { get; set; }
        public int totalPointsRedeemed { get; set; }
        public bool isActive { get; set; }
    }


    public class WasteTypeRes2
    {
        public int rank { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public decimal totalWeight { get; set; }
        public int totalPoints { get; set; }
    }


}
