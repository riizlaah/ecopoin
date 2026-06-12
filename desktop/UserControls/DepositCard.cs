using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EcoPoinDesktop.UserControls
{
    public partial class DepositCard : UserControl
    {
        public DepositCard(DepositRes rec)
        {
            InitializeComponent();
            var weightDec = rec.actualWeight ?? rec.estimatedWeight;
            title.Text = $"{rec.wasteType.name} x {weightDec}Kg ({rec.status})";
            weight.Text = $"Weight : {weightDec}Kg ";
            if (rec.actualWeight != null) weight.Text += $"(previously {rec.estimatedWeight})";
            residentName.Text = $"Resident Name : {rec.resident.name}";
            var point = rec.actualWeight != null ? rec.actualPoints : rec.estimatedPoints;
            points.Text = $"Points : {point}";
            lastUpdated.Text = $"Last Updated : {rec.updatedAt}";
        }
    }


    public class DepositRes
    {
        public int id { get; set; }
        public ResidentRes resident { get; set; }
        public OfficerRes? officer { get; set; } = null;
        public WasteTypeRes2 wasteType { get; set; }
        public decimal estimatedWeight { get; set; }
        public int estimatedPoints { get; set; }
        public decimal? actualWeight { get; set; } = null;
        public int? actualPoints { get; set; } = null;
        public string photoPath { get; set; }
        public string status { get; set; }
        public string notes { get; set; }
        public string rejectionReason { get; set; }
        public DateTime updatedAt { get; set; }
        public DateTime? createdAt { get; set; } = null;
    }

    public class ResidentRes
    {
        public int id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
    }

    public class OfficerRes
    {
        public int id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
    }

    public class WasteTypeRes2
    {
        public int id { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public int pointTariff { get; set; }
        public bool isActive { get; set; }
    }

}
