namespace EcoPoinDesktop.UserControls
{
    partial class DepositCard
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            title = new Label();
            points = new Label();
            residentName = new Label();
            weight = new Label();
            lastUpdated = new Label();
            SuspendLayout();
            // 
            // title
            // 
            title.Dock = DockStyle.Top;
            title.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            title.Location = new Point(12, 12);
            title.Name = "title";
            title.Size = new Size(297, 85);
            title.TabIndex = 0;
            title.Text = "{categoryName} x {weight} ({status})";
            // 
            // points
            // 
            points.AutoSize = true;
            points.Dock = DockStyle.Top;
            points.Location = new Point(12, 97);
            points.Margin = new Padding(3, 12, 3, 0);
            points.Name = "points";
            points.Padding = new Padding(0, 24, 0, 0);
            points.Size = new Size(55, 44);
            points.TabIndex = 1;
            points.Text = "Points :";
            // 
            // residentName
            // 
            residentName.AutoSize = true;
            residentName.Dock = DockStyle.Top;
            residentName.Location = new Point(12, 141);
            residentName.Margin = new Padding(3, 12, 3, 0);
            residentName.Name = "residentName";
            residentName.Padding = new Padding(0, 8, 0, 0);
            residentName.Size = new Size(117, 28);
            residentName.TabIndex = 2;
            residentName.Text = "Resident Name :";
            // 
            // weight
            // 
            weight.AutoSize = true;
            weight.Dock = DockStyle.Top;
            weight.Location = new Point(12, 169);
            weight.Margin = new Padding(3, 12, 3, 0);
            weight.Name = "weight";
            weight.Padding = new Padding(0, 8, 0, 0);
            weight.Size = new Size(63, 28);
            weight.TabIndex = 3;
            weight.Text = "Weight :";
            // 
            // lastUpdated
            // 
            lastUpdated.AutoSize = true;
            lastUpdated.Dock = DockStyle.Top;
            lastUpdated.Location = new Point(12, 197);
            lastUpdated.Margin = new Padding(3, 12, 3, 0);
            lastUpdated.Name = "lastUpdated";
            lastUpdated.Padding = new Padding(0, 24, 0, 0);
            lastUpdated.Size = new Size(108, 44);
            lastUpdated.TabIndex = 5;
            lastUpdated.Text = "Last Updated : ";
            // 
            // DepositCard
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(lastUpdated);
            Controls.Add(weight);
            Controls.Add(residentName);
            Controls.Add(points);
            Controls.Add(title);
            Margin = new Padding(12);
            Name = "DepositCard";
            Padding = new Padding(12);
            Size = new Size(321, 251);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label title;
        private Label points;
        private Label residentName;
        private Label weight;
        private Label lastUpdated;
    }
}
