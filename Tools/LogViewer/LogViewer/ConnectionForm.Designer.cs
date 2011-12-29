namespace LogViewer
{
    partial class ConnectionForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConnectionForm));
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtUrl = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cbxSaveAuthInfo = new System.Windows.Forms.CheckBox();
            this.txtLoggerPassword = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.rbnLoggerPassword = new System.Windows.Forms.RadioButton();
            this.rbnAdminUser = new System.Windows.Forms.RadioButton();
            this.lblLogin = new System.Windows.Forms.Label();
            this.txtLogin = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(71, 278);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(85, 23);
            this.btnOk.TabIndex = 8;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(197, 278);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(89, 23);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnFalse_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "URL";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtUrl);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(336, 57);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Channel";
            // 
            // txtUrl
            // 
            this.txtUrl.FormattingEnabled = true;
            this.txtUrl.Location = new System.Drawing.Point(60, 19);
            this.txtUrl.Name = "txtUrl";
            this.txtUrl.Size = new System.Drawing.Size(260, 21);
            this.txtUrl.TabIndex = 1;
            this.txtUrl.SelectedIndexChanged += new System.EventHandler(this.txtUrl_SelectedIndexChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lblLogin);
            this.groupBox2.Controls.Add(this.txtLogin);
            this.groupBox2.Controls.Add(this.cbxSaveAuthInfo);
            this.groupBox2.Controls.Add(this.txtLoggerPassword);
            this.groupBox2.Controls.Add(this.lblPassword);
            this.groupBox2.Controls.Add(this.txtPassword);
            this.groupBox2.Controls.Add(this.rbnLoggerPassword);
            this.groupBox2.Controls.Add(this.rbnAdminUser);
            this.groupBox2.Location = new System.Drawing.Point(12, 89);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(336, 183);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Authentication";
            // 
            // cbxSaveAuthInfo
            // 
            this.cbxSaveAuthInfo.AutoSize = true;
            this.cbxSaveAuthInfo.Location = new System.Drawing.Point(24, 154);
            this.cbxSaveAuthInfo.Name = "cbxSaveAuthInfo";
            this.cbxSaveAuthInfo.Size = new System.Drawing.Size(141, 17);
            this.cbxSaveAuthInfo.TabIndex = 7;
            this.cbxSaveAuthInfo.Text = "Save authentication info";
            this.cbxSaveAuthInfo.UseVisualStyleBackColor = true;
            // 
            // txtLoggerPassword
            // 
            this.txtLoggerPassword.Location = new System.Drawing.Point(24, 128);
            this.txtLoggerPassword.Name = "txtLoggerPassword";
            this.txtLoggerPassword.Size = new System.Drawing.Size(296, 20);
            this.txtLoggerPassword.TabIndex = 6;
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(21, 75);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(53, 13);
            this.lblPassword.TabIndex = 9;
            this.lblPassword.Text = "Password";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(80, 72);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(240, 20);
            this.txtPassword.TabIndex = 4;
            // 
            // rbnLoggerPassword
            // 
            this.rbnLoggerPassword.AutoSize = true;
            this.rbnLoggerPassword.Location = new System.Drawing.Point(9, 105);
            this.rbnLoggerPassword.Name = "rbnLoggerPassword";
            this.rbnLoggerPassword.Size = new System.Drawing.Size(106, 17);
            this.rbnLoggerPassword.TabIndex = 5;
            this.rbnLoggerPassword.Text = "Logger password";
            this.rbnLoggerPassword.UseVisualStyleBackColor = true;
            this.rbnLoggerPassword.CheckedChanged += new System.EventHandler(this.radioButton4_CheckedChanged);
            // 
            // rbnAdminUser
            // 
            this.rbnAdminUser.AutoSize = true;
            this.rbnAdminUser.Checked = true;
            this.rbnAdminUser.Location = new System.Drawing.Point(9, 19);
            this.rbnAdminUser.Name = "rbnAdminUser";
            this.rbnAdminUser.Size = new System.Drawing.Size(92, 17);
            this.rbnAdminUser.TabIndex = 2;
            this.rbnAdminUser.TabStop = true;
            this.rbnAdminUser.Text = "C1 admin user";
            this.rbnAdminUser.UseVisualStyleBackColor = true;
            this.rbnAdminUser.CheckedChanged += new System.EventHandler(this.rbnAdminUser_CheckedChanged);
            // 
            // lblLogin
            // 
            this.lblLogin.AutoSize = true;
            this.lblLogin.Location = new System.Drawing.Point(21, 49);
            this.lblLogin.Name = "lblLogin";
            this.lblLogin.Size = new System.Drawing.Size(33, 13);
            this.lblLogin.TabIndex = 13;
            this.lblLogin.Text = "Login";
            // 
            // txtLogin
            // 
            this.txtLogin.Location = new System.Drawing.Point(79, 46);
            this.txtLogin.Name = "txtLogin";
            this.txtLogin.Size = new System.Drawing.Size(241, 20);
            this.txtLogin.TabIndex = 3;
            this.txtLogin.Text = "admin";
            // 
            // ConnectionForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(360, 312);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "ConnectionForm";
            this.Text = "Connection";
            this.Load += new System.EventHandler(this.ConnectionForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtLoggerPassword;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.RadioButton rbnLoggerPassword;
        private System.Windows.Forms.RadioButton rbnAdminUser;
        private System.Windows.Forms.ComboBox txtUrl;
        private System.Windows.Forms.CheckBox cbxSaveAuthInfo;
        private System.Windows.Forms.Label lblLogin;
        private System.Windows.Forms.TextBox txtLogin;
    }
}