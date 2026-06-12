namespace EcoPoinDesktop
{
    partial class ManageVouchersForm
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
            table1 = new DataGridView();
            insert = new Button();
            search = new TextBox();
            delete = new Button();
            update = new Button();
            currentPage = new Label();
            voucherName = new TextBox();
            label3 = new Label();
            code = new TextBox();
            label4 = new Label();
            pointCost = new TextBox();
            label5 = new Label();
            cancel = new Button();
            save = new Button();
            prev = new Button();
            next = new Button();
            isActive = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)table1).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 73);
            label1.Name = "label1";
            label1.Size = new Size(60, 20);
            label1.TabIndex = 0;
            label1.Text = "Search :";
            // 
            // table1
            // 
            table1.AllowUserToAddRows = false;
            table1.AllowUserToDeleteRows = false;
            table1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            table1.Location = new Point(12, 115);
            table1.Name = "table1";
            table1.ReadOnly = true;
            table1.RowHeadersWidth = 51;
            table1.Size = new Size(890, 333);
            table1.TabIndex = 1;
            table1.CellClick += OnCellClicked;
            // 
            // insert
            // 
            insert.Location = new Point(666, 11);
            insert.Name = "insert";
            insert.Size = new Size(236, 37);
            insert.TabIndex = 2;
            insert.Text = "Insert New";
            insert.UseVisualStyleBackColor = true;
            insert.Click += insert_Click;
            // 
            // search
            // 
            search.Location = new Point(87, 70);
            search.Name = "search";
            search.Size = new Size(198, 27);
            search.TabIndex = 4;
            search.TextChanged += OnSearchChanged;
            // 
            // delete
            // 
            delete.Location = new Point(787, 60);
            delete.Name = "delete";
            delete.Size = new Size(115, 37);
            delete.TabIndex = 6;
            delete.Text = "Delete";
            delete.UseVisualStyleBackColor = true;
            delete.Click += delete_Click;
            // 
            // update
            // 
            update.Location = new Point(666, 60);
            update.Name = "update";
            update.Size = new Size(115, 37);
            update.TabIndex = 7;
            update.Text = "Edit";
            update.UseVisualStyleBackColor = true;
            update.Click += update_Click;
            // 
            // currentPage
            // 
            currentPage.AutoSize = true;
            currentPage.Location = new Point(813, 462);
            currentPage.Name = "currentPage";
            currentPage.Size = new Size(39, 20);
            currentPage.TabIndex = 10;
            currentPage.Text = "1 / 4";
            // 
            // voucherName
            // 
            voucherName.Location = new Point(154, 462);
            voucherName.Name = "voucherName";
            voucherName.Size = new Size(198, 27);
            voucherName.TabIndex = 12;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 465);
            label3.Name = "label3";
            label3.Size = new Size(56, 20);
            label3.TabIndex = 11;
            label3.Text = "Name :";
            // 
            // code
            // 
            code.Location = new Point(154, 495);
            code.Name = "code";
            code.Size = new Size(198, 27);
            code.TabIndex = 14;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 498);
            label4.Name = "label4";
            label4.Size = new Size(51, 20);
            label4.TabIndex = 13;
            label4.Text = "Code :";
            // 
            // pointCost
            // 
            pointCost.Location = new Point(154, 528);
            pointCost.Name = "pointCost";
            pointCost.Size = new Size(198, 27);
            pointCost.TabIndex = 16;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(12, 531);
            label5.Name = "label5";
            label5.Size = new Size(82, 20);
            label5.TabIndex = 15;
            label5.Text = "Point Cost :";
            // 
            // cancel
            // 
            cancel.Location = new Point(787, 579);
            cancel.Name = "cancel";
            cancel.Size = new Size(115, 37);
            cancel.TabIndex = 24;
            cancel.Text = "Cancel";
            cancel.UseVisualStyleBackColor = true;
            cancel.Click += OnCancel;
            // 
            // save
            // 
            save.BackColor = Color.LightGreen;
            save.Location = new Point(787, 622);
            save.Name = "save";
            save.Size = new Size(115, 37);
            save.TabIndex = 23;
            save.Text = "Save";
            save.UseVisualStyleBackColor = false;
            save.Click += OnSave;
            // 
            // prev
            // 
            prev.Location = new Point(764, 458);
            prev.Name = "prev";
            prev.Size = new Size(38, 29);
            prev.TabIndex = 25;
            prev.Text = "<";
            prev.UseVisualStyleBackColor = true;
            prev.Click += OnPrev;
            // 
            // next
            // 
            next.Location = new Point(864, 458);
            next.Name = "next";
            next.Size = new Size(38, 29);
            next.TabIndex = 26;
            next.Text = ">";
            next.UseVisualStyleBackColor = true;
            next.Click += OnNext;
            // 
            // isActive
            // 
            isActive.AutoSize = true;
            isActive.Location = new Point(154, 564);
            isActive.Name = "isActive";
            isActive.Size = new Size(86, 24);
            isActive.TabIndex = 27;
            isActive.Text = "Is Active";
            isActive.UseVisualStyleBackColor = true;
            // 
            // ManageVouchersForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(914, 666);
            Controls.Add(isActive);
            Controls.Add(next);
            Controls.Add(prev);
            Controls.Add(cancel);
            Controls.Add(save);
            Controls.Add(pointCost);
            Controls.Add(label5);
            Controls.Add(code);
            Controls.Add(label4);
            Controls.Add(voucherName);
            Controls.Add(label3);
            Controls.Add(currentPage);
            Controls.Add(update);
            Controls.Add(delete);
            Controls.Add(search);
            Controls.Add(insert);
            Controls.Add(table1);
            Controls.Add(label1);
            Name = "ManageVouchersForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Manage Vouchers";
            ((System.ComponentModel.ISupportInitialize)table1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private DataGridView table1;
        private Button insert;
        private ComboBox filterRoles;
        private TextBox search;
        private Label label2;
        private Button delete;
        private Button update;
        private Label currentPage;
        private TextBox voucherName;
        private Label label3;
        private TextBox code;
        private Label label4;
        private TextBox pointCost;
        private Label label5;
        private Button cancel;
        private Button save;
        private Button prev;
        private Button next;
        private CheckBox isActive;
    }
}