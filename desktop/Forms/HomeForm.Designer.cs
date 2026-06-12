namespace EcoPoinDesktop.Forms
{
    partial class HomeForm
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
            button1 = new Button();
            greeterLb = new Label();
            button2 = new Button();
            button3 = new Button();
            button4 = new Button();
            button5 = new Button();
            button6 = new Button();
            datetimeLb = new Label();
            button7 = new Button();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(56, 52);
            button1.Name = "button1";
            button1.Size = new Size(193, 59);
            button1.TabIndex = 0;
            button1.Text = "Manage Users";
            button1.UseVisualStyleBackColor = true;
            button1.Click += OnManageUsers;
            // 
            // greeterLb
            // 
            greeterLb.AutoSize = true;
            greeterLb.Location = new Point(12, 9);
            greeterLb.Name = "greeterLb";
            greeterLb.Size = new Size(123, 20);
            greeterLb.TabIndex = 1;
            greeterLb.Text = "Hello, {fullName}";
            // 
            // button2
            // 
            button2.Location = new Point(286, 132);
            button2.Name = "button2";
            button2.Size = new Size(193, 59);
            button2.TabIndex = 2;
            button2.Text = "Manage Vouchers";
            button2.UseVisualStyleBackColor = true;
            button2.Click += OnManageVouchers;
            // 
            // button3
            // 
            button3.Location = new Point(286, 52);
            button3.Name = "button3";
            button3.Size = new Size(193, 59);
            button3.TabIndex = 3;
            button3.Text = "Manage Waste Types";
            button3.UseVisualStyleBackColor = true;
            button3.Click += OnManageWasteTypes;
            // 
            // button4
            // 
            button4.Location = new Point(56, 207);
            button4.Name = "button4";
            button4.Size = new Size(193, 59);
            button4.TabIndex = 4;
            button4.Text = "View Reports";
            button4.UseVisualStyleBackColor = true;
            button4.Click += OnViewReports;
            // 
            // button5
            // 
            button5.Location = new Point(171, 281);
            button5.Name = "button5";
            button5.Size = new Size(193, 59);
            button5.TabIndex = 5;
            button5.Text = "Log Out";
            button5.UseVisualStyleBackColor = true;
            button5.Click += OnLogOut;
            // 
            // button6
            // 
            button6.Location = new Point(56, 132);
            button6.Name = "button6";
            button6.Size = new Size(193, 59);
            button6.TabIndex = 6;
            button6.Text = "Exchange Voucher";
            button6.UseVisualStyleBackColor = true;
            button6.Click += OnExchangeVoucher;
            // 
            // datetimeLb
            // 
            datetimeLb.Dock = DockStyle.Bottom;
            datetimeLb.Location = new Point(0, 357);
            datetimeLb.Name = "datetimeLb";
            datetimeLb.Size = new Size(552, 33);
            datetimeLb.TabIndex = 7;
            datetimeLb.Text = "dddd, dd MMMM yyyy (HH:mm:ss)";
            datetimeLb.TextAlign = ContentAlignment.MiddleRight;
            // 
            // button7
            // 
            button7.Location = new Point(286, 207);
            button7.Name = "button7";
            button7.Size = new Size(193, 59);
            button7.TabIndex = 8;
            button7.Text = "View Deposits";
            button7.UseVisualStyleBackColor = true;
            button7.Click += OnViewDeposits;
            // 
            // HomeForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(552, 390);
            Controls.Add(button7);
            Controls.Add(datetimeLb);
            Controls.Add(button6);
            Controls.Add(button5);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(greeterLb);
            Controls.Add(button1);
            Name = "HomeForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Home";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private Label greeterLb;
        private Button button2;
        private Button button3;
        private Button button4;
        private Button button5;
        private Button button6;
        private Label datetimeLb;
        private Button button7;
    }
}