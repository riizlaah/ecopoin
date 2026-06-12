namespace EcoPoinDesktop.Forms
{
    partial class ExchangeVoucherForm
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
            useBtn = new Button();
            label1 = new Label();
            voucherCode = new TextBox();
            voucherData = new GroupBox();
            exchangedAt = new Label();
            residentName = new Label();
            usedAt = new Label();
            isUsed = new Label();
            pointCost = new Label();
            voucherName = new Label();
            voucherData.SuspendLayout();
            SuspendLayout();
            // 
            // useBtn
            // 
            useBtn.Location = new Point(523, 35);
            useBtn.Name = "useBtn";
            useBtn.Size = new Size(94, 29);
            useBtn.TabIndex = 0;
            useBtn.Text = "Check";
            useBtn.UseVisualStyleBackColor = true;
            useBtn.Click += onTryUse;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(28, 38);
            label1.Name = "label1";
            label1.Size = new Size(108, 20);
            label1.TabIndex = 1;
            label1.Text = "Voucher Code :";
            // 
            // voucherCode
            // 
            voucherCode.Location = new Point(156, 35);
            voucherCode.Name = "voucherCode";
            voucherCode.Size = new Size(345, 27);
            voucherCode.TabIndex = 2;
            voucherCode.TextChanged += OnVoucherCodeChanged;
            // 
            // voucherData
            // 
            voucherData.Controls.Add(exchangedAt);
            voucherData.Controls.Add(residentName);
            voucherData.Controls.Add(usedAt);
            voucherData.Controls.Add(isUsed);
            voucherData.Controls.Add(pointCost);
            voucherData.Controls.Add(voucherName);
            voucherData.Location = new Point(28, 83);
            voucherData.Name = "voucherData";
            voucherData.Size = new Size(589, 235);
            voucherData.TabIndex = 3;
            voucherData.TabStop = false;
            voucherData.Text = "Voucher";
            // 
            // exchangedAt
            // 
            exchangedAt.AutoSize = true;
            exchangedAt.Location = new Point(22, 194);
            exchangedAt.Name = "exchangedAt";
            exchangedAt.Size = new Size(111, 20);
            exchangedAt.TabIndex = 5;
            exchangedAt.Text = "Exchanged At : ";
            // 
            // residentName
            // 
            residentName.AutoSize = true;
            residentName.Location = new Point(22, 97);
            residentName.Name = "residentName";
            residentName.Size = new Size(121, 20);
            residentName.TabIndex = 4;
            residentName.Text = "Resident Name : ";
            // 
            // usedAt
            // 
            usedAt.AutoSize = true;
            usedAt.Location = new Point(22, 162);
            usedAt.Name = "usedAt";
            usedAt.Size = new Size(68, 20);
            usedAt.TabIndex = 3;
            usedAt.Text = "Used At :";
            // 
            // isUsed
            // 
            isUsed.AutoSize = true;
            isUsed.Location = new Point(22, 129);
            isUsed.Name = "isUsed";
            isUsed.Size = new Size(67, 20);
            isUsed.TabIndex = 2;
            isUsed.Text = "Is Used : ";
            // 
            // pointCost
            // 
            pointCost.AutoSize = true;
            pointCost.Location = new Point(22, 64);
            pointCost.Name = "pointCost";
            pointCost.Size = new Size(86, 20);
            pointCost.TabIndex = 1;
            pointCost.Text = "Point Cost : ";
            // 
            // voucherName
            // 
            voucherName.AutoSize = true;
            voucherName.Location = new Point(22, 34);
            voucherName.Name = "voucherName";
            voucherName.Size = new Size(60, 20);
            voucherName.TabIndex = 0;
            voucherName.Text = "Name : ";
            // 
            // ExchangeVoucherForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(640, 340);
            Controls.Add(voucherData);
            Controls.Add(voucherCode);
            Controls.Add(label1);
            Controls.Add(useBtn);
            Name = "ExchangeVoucherForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Exchange Voucher";
            voucherData.ResumeLayout(false);
            voucherData.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button useBtn;
        private Label label1;
        private TextBox voucherCode;
        private GroupBox voucherData;
        private Label pointCost;
        private Label voucherName;
        private Label residentName;
        private Label usedAt;
        private Label isUsed;
        private Label exchangedAt;
    }
}