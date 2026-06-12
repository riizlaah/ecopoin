namespace EcoPoinDesktop.Forms
{
    partial class ViewReportsForm
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
            totalPoints = new Label();
            totalPointsRedeemed = new Label();
            pointsOutstanding = new Label();
            label1 = new Label();
            leaderboard = new DataGridView();
            pagingL = new Label();
            prevL = new Button();
            nextL = new Button();
            nextW = new Button();
            prevW = new Button();
            pagingW = new Label();
            wasteTypes = new DataGridView();
            label3 = new Label();
            nextV = new Button();
            prevV = new Button();
            pagingV = new Label();
            vouchers = new DataGridView();
            label5 = new Label();
            ((System.ComponentModel.ISupportInitialize)leaderboard).BeginInit();
            ((System.ComponentModel.ISupportInitialize)wasteTypes).BeginInit();
            ((System.ComponentModel.ISupportInitialize)vouchers).BeginInit();
            SuspendLayout();
            // 
            // totalPoints
            // 
            totalPoints.AutoSize = true;
            totalPoints.Location = new Point(12, 9);
            totalPoints.Name = "totalPoints";
            totalPoints.Size = new Size(92, 20);
            totalPoints.TabIndex = 0;
            totalPoints.Text = "Total Points :";
            // 
            // totalPointsRedeemed
            // 
            totalPointsRedeemed.AutoSize = true;
            totalPointsRedeemed.Location = new Point(12, 38);
            totalPointsRedeemed.Name = "totalPointsRedeemed";
            totalPointsRedeemed.Size = new Size(168, 20);
            totalPointsRedeemed.TabIndex = 1;
            totalPointsRedeemed.Text = "Total Points Redeemed :";
            // 
            // pointsOutstanding
            // 
            pointsOutstanding.AutoSize = true;
            pointsOutstanding.Location = new Point(12, 68);
            pointsOutstanding.Name = "pointsOutstanding";
            pointsOutstanding.Size = new Size(140, 20);
            pointsOutstanding.TabIndex = 2;
            pointsOutstanding.Text = "Points Outstanding :";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(393, 96);
            label1.Name = "label1";
            label1.Size = new Size(130, 28);
            label1.TabIndex = 3;
            label1.Text = "Leaderboard";
            // 
            // leaderboard
            // 
            leaderboard.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            leaderboard.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            leaderboard.Location = new Point(15, 127);
            leaderboard.Name = "leaderboard";
            leaderboard.RowHeadersWidth = 51;
            leaderboard.Size = new Size(855, 188);
            leaderboard.TabIndex = 4;
            // 
            // pagingL
            // 
            pagingL.AutoSize = true;
            pagingL.Location = new Point(753, 328);
            pagingL.Name = "pagingL";
            pagingL.Size = new Size(39, 20);
            pagingL.TabIndex = 5;
            pagingL.Text = "1 / 2";
            // 
            // prevL
            // 
            prevL.Location = new Point(686, 324);
            prevL.Name = "prevL";
            prevL.Size = new Size(36, 29);
            prevL.TabIndex = 6;
            prevL.Text = "<";
            prevL.UseVisualStyleBackColor = true;
            prevL.Click += OnPrevLeaderboard;
            // 
            // nextL
            // 
            nextL.Location = new Point(833, 324);
            nextL.Name = "nextL";
            nextL.Size = new Size(36, 29);
            nextL.TabIndex = 7;
            nextL.Text = ">";
            nextL.UseVisualStyleBackColor = true;
            nextL.Click += OnNextLeaderboard;
            // 
            // nextW
            // 
            nextW.Location = new Point(833, 580);
            nextW.Name = "nextW";
            nextW.Size = new Size(36, 29);
            nextW.TabIndex = 12;
            nextW.Text = ">";
            nextW.UseVisualStyleBackColor = true;
            nextW.Click += OnNextWasteType;
            // 
            // prevW
            // 
            prevW.Location = new Point(686, 580);
            prevW.Name = "prevW";
            prevW.Size = new Size(36, 29);
            prevW.TabIndex = 11;
            prevW.Text = "<";
            prevW.UseVisualStyleBackColor = true;
            prevW.Click += OnPrevWasteType;
            // 
            // pagingW
            // 
            pagingW.AutoSize = true;
            pagingW.Location = new Point(753, 584);
            pagingW.Name = "pagingW";
            pagingW.Size = new Size(39, 20);
            pagingW.TabIndex = 10;
            pagingW.Text = "1 / 2";
            // 
            // wasteTypes
            // 
            wasteTypes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            wasteTypes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            wasteTypes.Location = new Point(15, 384);
            wasteTypes.Name = "wasteTypes";
            wasteTypes.RowHeadersWidth = 51;
            wasteTypes.Size = new Size(855, 188);
            wasteTypes.TabIndex = 9;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.Location = new Point(384, 337);
            label3.Name = "label3";
            label3.Size = new Size(170, 28);
            label3.TabIndex = 8;
            label3.Text = "Top Waste Types";
            // 
            // nextV
            // 
            nextV.Location = new Point(833, 846);
            nextV.Name = "nextV";
            nextV.Size = new Size(36, 29);
            nextV.TabIndex = 17;
            nextV.Text = ">";
            nextV.UseVisualStyleBackColor = true;
            nextV.Click += OnNextVoucher;
            // 
            // prevV
            // 
            prevV.Location = new Point(686, 846);
            prevV.Name = "prevV";
            prevV.Size = new Size(36, 29);
            prevV.TabIndex = 16;
            prevV.Text = "<";
            prevV.UseVisualStyleBackColor = true;
            prevV.Click += OnPrevVoucher;
            // 
            // pagingV
            // 
            pagingV.AutoSize = true;
            pagingV.Location = new Point(753, 850);
            pagingV.Name = "pagingV";
            pagingV.Size = new Size(39, 20);
            pagingV.TabIndex = 15;
            pagingV.Text = "1 / 2";
            // 
            // vouchers
            // 
            vouchers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            vouchers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            vouchers.Location = new Point(12, 647);
            vouchers.Name = "vouchers";
            vouchers.RowHeadersWidth = 51;
            vouchers.Size = new Size(855, 188);
            vouchers.TabIndex = 14;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label5.Location = new Point(340, 602);
            label5.Name = "label5";
            label5.Size = new Size(243, 28);
            label5.TabIndex = 13;
            label5.Text = "Top Redeemed Vouchers";
            // 
            // ViewReportsForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(882, 894);
            Controls.Add(nextV);
            Controls.Add(prevV);
            Controls.Add(pagingV);
            Controls.Add(vouchers);
            Controls.Add(label5);
            Controls.Add(nextW);
            Controls.Add(prevW);
            Controls.Add(pagingW);
            Controls.Add(wasteTypes);
            Controls.Add(label3);
            Controls.Add(nextL);
            Controls.Add(prevL);
            Controls.Add(pagingL);
            Controls.Add(leaderboard);
            Controls.Add(label1);
            Controls.Add(pointsOutstanding);
            Controls.Add(totalPointsRedeemed);
            Controls.Add(totalPoints);
            Name = "ViewReportsForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "View Reports";
            ((System.ComponentModel.ISupportInitialize)leaderboard).EndInit();
            ((System.ComponentModel.ISupportInitialize)wasteTypes).EndInit();
            ((System.ComponentModel.ISupportInitialize)vouchers).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label totalPoints;
        private Label totalPointsRedeemed;
        private Label pointsOutstanding;
        private Label label1;
        private DataGridView leaderboard;
        private Label pagingL;
        private Button prevL;
        private Button nextL;
        private Button nextW;
        private Button prevW;
        private Label pagingW;
        private DataGridView wasteTypes;
        private Label label3;
        private Button nextV;
        private Button prevV;
        private Label pagingV;
        private DataGridView vouchers;
        private Label label5;
    }
}