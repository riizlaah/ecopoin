namespace EcoPoinDesktop.Forms
{
    partial class ViewDepositsForm
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
            label1 = new Label();
            deposits = new FlowLayoutPanel();
            currentPage = new Label();
            prev = new Button();
            next = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = DockStyle.Top;
            label1.Font = new Font("Segoe UI", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(0, 0);
            label1.Name = "label1";
            label1.Padding = new Padding(12);
            label1.Size = new Size(240, 62);
            label1.TabIndex = 0;
            label1.Text = "Latest Deposits";
            // 
            // deposits
            // 
            deposits.AutoScroll = true;
            deposits.Dock = DockStyle.Fill;
            deposits.Location = new Point(0, 62);
            deposits.Name = "deposits";
            deposits.Padding = new Padding(12);
            deposits.Size = new Size(893, 521);
            deposits.TabIndex = 1;
            // 
            // currentPage
            // 
            currentPage.AutoSize = true;
            currentPage.Location = new Point(776, 23);
            currentPage.Name = "currentPage";
            currentPage.Size = new Size(39, 20);
            currentPage.TabIndex = 2;
            currentPage.Text = "1 / 4";
            // 
            // prev
            // 
            prev.Location = new Point(715, 19);
            prev.Name = "prev";
            prev.Size = new Size(38, 29);
            prev.TabIndex = 3;
            prev.Text = "<";
            prev.UseVisualStyleBackColor = true;
            prev.Click += OnPrev;
            // 
            // next
            // 
            next.Location = new Point(838, 19);
            next.Name = "next";
            next.Size = new Size(38, 29);
            next.TabIndex = 4;
            next.Text = ">";
            next.UseVisualStyleBackColor = true;
            next.Click += OnNext;
            // 
            // ViewDepositsForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(893, 583);
            Controls.Add(next);
            Controls.Add(prev);
            Controls.Add(currentPage);
            Controls.Add(deposits);
            Controls.Add(label1);
            Name = "ViewDepositsForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "View Deposits";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private FlowLayoutPanel deposits;
        private Label currentPage;
        private Button prev;
        private Button next;
    }
}