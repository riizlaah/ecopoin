namespace EcoPoinDesktop.Forms
{
    partial class DepositDetailForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lastUpdated = new Label();
            estimatedWeight = new Label();
            residentName = new Label();
            estimatedPoints = new Label();
            title = new Label();
            officerName = new Label();
            notes = new Label();
            rejectionReason = new Label();
            photo = new PictureBox();
            submittedAt = new Label();
            actualPoints = new Label();
            actualWeight = new Label();
            ((System.ComponentModel.ISupportInitialize)photo).BeginInit();
            SuspendLayout();
            // 
            // lastUpdated
            // 
            lastUpdated.AutoSize = true;
            lastUpdated.Location = new Point(14, 613);
            lastUpdated.Name = "lastUpdated";
            lastUpdated.Size = new Size(108, 20);
            lastUpdated.TabIndex = 10;
            lastUpdated.Text = "Last Updated : ";
            // 
            // estimatedWeight
            // 
            estimatedWeight.AutoSize = true;
            estimatedWeight.Location = new Point(12, 168);
            estimatedWeight.Name = "estimatedWeight";
            estimatedWeight.Size = new Size(133, 20);
            estimatedWeight.TabIndex = 9;
            estimatedWeight.Text = "Estimated Weight :";
            // 
            // residentName
            // 
            residentName.AutoSize = true;
            residentName.Location = new Point(12, 274);
            residentName.Name = "residentName";
            residentName.Size = new Size(117, 20);
            residentName.TabIndex = 8;
            residentName.Text = "Resident Name :";
            // 
            // estimatedPoints
            // 
            estimatedPoints.AutoSize = true;
            estimatedPoints.Location = new Point(12, 203);
            estimatedPoints.Name = "estimatedPoints";
            estimatedPoints.Size = new Size(125, 20);
            estimatedPoints.TabIndex = 7;
            estimatedPoints.Text = "Estimated Points :";
            // 
            // title
            // 
            title.Dock = DockStyle.Top;
            title.Font = new Font("Segoe UI", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            title.Location = new Point(0, 0);
            title.Name = "title";
            title.Padding = new Padding(8);
            title.Size = new Size(1004, 82);
            title.TabIndex = 6;
            title.Text = "{categoryName} x {weight} ({status})";
            // 
            // officerName
            // 
            officerName.AutoSize = true;
            officerName.Location = new Point(12, 306);
            officerName.Name = "officerName";
            officerName.Size = new Size(105, 20);
            officerName.TabIndex = 11;
            officerName.Text = "Officer Name :";
            // 
            // notes
            // 
            notes.Location = new Point(12, 371);
            notes.Name = "notes";
            notes.Size = new Size(329, 84);
            notes.TabIndex = 12;
            notes.Text = "Notes :";
            // 
            // rejectionReason
            // 
            rejectionReason.Location = new Point(12, 471);
            rejectionReason.Name = "rejectionReason";
            rejectionReason.Size = new Size(329, 96);
            rejectionReason.TabIndex = 13;
            rejectionReason.Text = "Rejection Reason :";
            // 
            // photo
            // 
            photo.BackColor = SystemColors.ControlLight;
            photo.Location = new Point(453, 103);
            photo.Name = "photo";
            photo.Size = new Size(530, 530);
            photo.SizeMode = PictureBoxSizeMode.Zoom;
            photo.TabIndex = 14;
            photo.TabStop = false;
            // 
            // submittedAt
            // 
            submittedAt.AutoSize = true;
            submittedAt.Location = new Point(14, 581);
            submittedAt.Name = "submittedAt";
            submittedAt.Size = new Size(108, 20);
            submittedAt.TabIndex = 15;
            submittedAt.Text = "Sumbitted At : ";
            // 
            // actualPoints
            // 
            actualPoints.AutoSize = true;
            actualPoints.Location = new Point(12, 136);
            actualPoints.Name = "actualPoints";
            actualPoints.Size = new Size(101, 20);
            actualPoints.TabIndex = 16;
            actualPoints.Text = "Actual Points :";
            // 
            // actualWeight
            // 
            actualWeight.AutoSize = true;
            actualWeight.Location = new Point(12, 103);
            actualWeight.Name = "actualWeight";
            actualWeight.Size = new Size(109, 20);
            actualWeight.TabIndex = 17;
            actualWeight.Text = "Actual Weight :";
            // 
            // DepositDetailForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1004, 653);
            Controls.Add(actualWeight);
            Controls.Add(actualPoints);
            Controls.Add(submittedAt);
            Controls.Add(photo);
            Controls.Add(rejectionReason);
            Controls.Add(notes);
            Controls.Add(officerName);
            Controls.Add(lastUpdated);
            Controls.Add(estimatedWeight);
            Controls.Add(residentName);
            Controls.Add(estimatedPoints);
            Controls.Add(title);
            Name = "DepositDetailForm";
            Text = "Deposit Detail";
            ((System.ComponentModel.ISupportInitialize)photo).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lastUpdated;
        private Label estimatedWeight;
        private Label residentName;
        private Label estimatedPoints;
        private Label title;
        private Label officerName;
        private Label notes;
        private Label rejectionReason;
        private PictureBox photo;
        private Label submittedAt;
        private Label actualPoints;
        private Label actualWeight;
    }
}