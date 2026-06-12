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
    public partial class DepositDetailForm : Form
    {

        public DepositDetailForm(int id)
        {
            InitializeComponent();
            Helper.LockWindow(this);
            LoadData(id);
        }

        async public Task LoadData(int id)
        {
            var (isSuccess, msg, rec) = await Helper.JsonReq<DepositRes>($"deposits/{id}");
            if(!isSuccess)
            {
                if (msg == "") msg = "Unkwonw error";
                MessageBox.Show(msg, "Error");
                return;
            }
            var weightDec = rec.actualWeight ?? rec.estimatedWeight;
            title.Text = $"{rec.wasteType.name} x {weightDec}Kg ({rec.status})";
            estimatedWeight.Text = $"EstimatedWeight : {rec.estimatedWeight}Kg";
            estimatedPoints.Text = $"Estimated Points : {rec.estimatedPoints}";
            actualPoints.Text = rec.actualWeight == null ? "Actual Points : ?" : $"Actual Points : {rec.actualPoints}";
            actualWeight.Text = rec.actualWeight == null ? "Actual Weight : ?" : $"Actual Weight : {rec.actualWeight}Kg";
            residentName.Text = $"Resident Name : {rec.resident.name}";
            officerName.Text = $"Officer Name (Reviewer) : {rec.officer?.name ?? "None"}";
            notes.Text = $"Notes : {rec.notes ?? "Empty"}";
            rejectionReason.Text = $"Rejection Reason : {rec.rejectionReason ?? "Empty"}";
            var point = rec.actualWeight != null ? rec.actualPoints : rec.estimatedPoints;
            lastUpdated.Text = $"Last Updated : {rec.updatedAt}";
            submittedAt.Text = $"Submitted At : {rec.createdAt}";
            photo.Image = await Helper.FetchImg(rec.photoPath);
        }
    }
}
