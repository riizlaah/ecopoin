namespace EcoPoinDesktop
{
    partial class ManageUsersForm
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
            filterRoles = new ComboBox();
            search = new TextBox();
            label2 = new Label();
            delete = new Button();
            update = new Button();
            currentPage = new Label();
            username = new TextBox();
            label3 = new Label();
            fullName = new TextBox();
            label4 = new Label();
            email = new TextBox();
            label5 = new Label();
            phone = new TextBox();
            label6 = new Label();
            password = new TextBox();
            label7 = new Label();
            label8 = new Label();
            roles = new ComboBox();
            cancel = new Button();
            save = new Button();
            prev = new Button();
            next = new Button();
            showPassword = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)table1).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 28);
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
            // filterRoles
            // 
            filterRoles.DropDownStyle = ComboBoxStyle.DropDownList;
            filterRoles.FormattingEnabled = true;
            filterRoles.Location = new Point(87, 65);
            filterRoles.Name = "filterRoles";
            filterRoles.Size = new Size(198, 28);
            filterRoles.TabIndex = 3;
            filterRoles.SelectedIndexChanged += OnFilterRolesChanged;
            // 
            // search
            // 
            search.Location = new Point(87, 25);
            search.Name = "search";
            search.Size = new Size(198, 27);
            search.TabIndex = 4;
            search.TextChanged += OnSearchChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 65);
            label2.Name = "label2";
            label2.Size = new Size(50, 20);
            label2.TabIndex = 5;
            label2.Text = "Role : ";
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
            // username
            // 
            username.Location = new Point(154, 462);
            username.Name = "username";
            username.Size = new Size(198, 27);
            username.TabIndex = 12;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 465);
            label3.Name = "label3";
            label3.Size = new Size(82, 20);
            label3.TabIndex = 11;
            label3.Text = "Username :";
            // 
            // fullName
            // 
            fullName.Location = new Point(154, 495);
            fullName.Name = "fullName";
            fullName.Size = new Size(198, 27);
            fullName.TabIndex = 14;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 498);
            label4.Name = "label4";
            label4.Size = new Size(83, 20);
            label4.TabIndex = 13;
            label4.Text = "Full Name :";
            // 
            // email
            // 
            email.Location = new Point(154, 528);
            email.Name = "email";
            email.Size = new Size(198, 27);
            email.TabIndex = 16;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(12, 531);
            label5.Name = "label5";
            label5.Size = new Size(53, 20);
            label5.TabIndex = 15;
            label5.Text = "Email :";
            // 
            // phone
            // 
            phone.Location = new Point(154, 561);
            phone.Name = "phone";
            phone.Size = new Size(198, 27);
            phone.TabIndex = 18;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(12, 564);
            label6.Name = "label6";
            label6.Size = new Size(57, 20);
            label6.TabIndex = 17;
            label6.Text = "Phone :";
            // 
            // password
            // 
            password.Location = new Point(154, 594);
            password.Name = "password";
            password.PasswordChar = '*';
            password.Size = new Size(198, 27);
            password.TabIndex = 20;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(12, 597);
            label7.Name = "label7";
            label7.Size = new Size(77, 20);
            label7.TabIndex = 19;
            label7.Text = "Password :";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(12, 630);
            label8.Name = "label8";
            label8.Size = new Size(50, 20);
            label8.TabIndex = 22;
            label8.Text = "Role : ";
            // 
            // roles
            // 
            roles.DropDownStyle = ComboBoxStyle.DropDownList;
            roles.FormattingEnabled = true;
            roles.Location = new Point(154, 627);
            roles.Name = "roles";
            roles.Size = new Size(198, 28);
            roles.TabIndex = 21;
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
            // showPassword
            // 
            showPassword.AutoSize = true;
            showPassword.Location = new Point(364, 597);
            showPassword.Name = "showPassword";
            showPassword.Size = new Size(67, 24);
            showPassword.TabIndex = 27;
            showPassword.Text = "Show";
            showPassword.UseVisualStyleBackColor = true;
            showPassword.CheckedChanged += OnTogglePassword;
            // 
            // ManageUsersForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(914, 666);
            Controls.Add(showPassword);
            Controls.Add(next);
            Controls.Add(prev);
            Controls.Add(cancel);
            Controls.Add(save);
            Controls.Add(label8);
            Controls.Add(roles);
            Controls.Add(password);
            Controls.Add(label7);
            Controls.Add(phone);
            Controls.Add(label6);
            Controls.Add(email);
            Controls.Add(label5);
            Controls.Add(fullName);
            Controls.Add(label4);
            Controls.Add(username);
            Controls.Add(label3);
            Controls.Add(currentPage);
            Controls.Add(update);
            Controls.Add(delete);
            Controls.Add(label2);
            Controls.Add(search);
            Controls.Add(filterRoles);
            Controls.Add(insert);
            Controls.Add(table1);
            Controls.Add(label1);
            Name = "ManageUsersForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Manage Users";
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
        private TextBox username;
        private Label label3;
        private TextBox fullName;
        private Label label4;
        private TextBox email;
        private Label label5;
        private TextBox phone;
        private Label label6;
        private TextBox password;
        private Label label7;
        private Label label8;
        private ComboBox roles;
        private Button cancel;
        private Button save;
        private Button prev;
        private Button next;
        private CheckBox showPassword;
    }
}