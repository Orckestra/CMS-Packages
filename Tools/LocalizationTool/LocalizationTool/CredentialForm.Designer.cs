namespace LocalizationTool
{
    partial class CredentialForm
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
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.SourceRepoPassword = new System.Windows.Forms.TextBox();
            this.SourceRepoUserName = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.TargetRepoPassword = new System.Windows.Forms.TextBox();
            this.TargetRepoUserName = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 39);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 13);
            this.label5.TabIndex = 35;
            this.label5.Text = "User Name";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(182, 39);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 13);
            this.label4.TabIndex = 34;
            this.label4.Text = "Password";
            // 
            // SourceRepoPassword
            // 
            this.SourceRepoPassword.Location = new System.Drawing.Point(241, 35);
            this.SourceRepoPassword.Name = "SourceRepoPassword";
            this.SourceRepoPassword.PasswordChar = '*';
            this.SourceRepoPassword.Size = new System.Drawing.Size(100, 20);
            this.SourceRepoPassword.TabIndex = 33;
            // 
            // SourceRepoUserName
            // 
            this.SourceRepoUserName.Location = new System.Drawing.Point(76, 35);
            this.SourceRepoUserName.Name = "SourceRepoUserName";
            this.SourceRepoUserName.Size = new System.Drawing.Size(100, 20);
            this.SourceRepoUserName.TabIndex = 32;
            // 
            // groupBox1
            // 
            this.groupBox1.Location = new System.Drawing.Point(7, 9);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(340, 67);
            this.groupBox1.TabIndex = 40;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Source Credential";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 112);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 44;
            this.label1.Text = "User Name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(182, 112);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 43;
            this.label2.Text = "Password";
            // 
            // TargetRepoPassword
            // 
            this.TargetRepoPassword.Location = new System.Drawing.Point(241, 108);
            this.TargetRepoPassword.Name = "TargetRepoPassword";
            this.TargetRepoPassword.PasswordChar = '*';
            this.TargetRepoPassword.Size = new System.Drawing.Size(100, 20);
            this.TargetRepoPassword.TabIndex = 42;
            // 
            // TargetRepoUserName
            // 
            this.TargetRepoUserName.Location = new System.Drawing.Point(76, 108);
            this.TargetRepoUserName.Name = "TargetRepoUserName";
            this.TargetRepoUserName.Size = new System.Drawing.Size(100, 20);
            this.TargetRepoUserName.TabIndex = 41;
            // 
            // groupBox2
            // 
            this.groupBox2.Location = new System.Drawing.Point(7, 82);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(340, 67);
            this.groupBox2.TabIndex = 45;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Target Credential";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(185, 156);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 46;
            this.button1.Text = "Save";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(267, 155);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 47;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // CredentialForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(359, 188);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.TargetRepoPassword);
            this.Controls.Add(this.TargetRepoUserName);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.SourceRepoPassword);
            this.Controls.Add(this.SourceRepoUserName);
            this.Controls.Add(this.groupBox1);
            this.Name = "CredentialForm";
            this.Text = "CredentialForm";
            this.Load += new System.EventHandler(this.CredentialForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox SourceRepoPassword;
        private System.Windows.Forms.TextBox SourceRepoUserName;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TargetRepoPassword;
        private System.Windows.Forms.TextBox TargetRepoUserName;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}