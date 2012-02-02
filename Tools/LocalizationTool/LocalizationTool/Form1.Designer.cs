namespace LocalizationTool
{
    partial class Form1
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
			this.filesListBox = new System.Windows.Forms.ListBox();
			this.label1 = new System.Windows.Forms.Label();
			this.stringKeysListBox = new System.Windows.Forms.ListBox();
			this.label2 = new System.Windows.Forms.Label();
			this.sourceLanguageTextBox = new System.Windows.Forms.TextBox();
			this.sourceLanguageLabel = new System.Windows.Forms.Label();
			this.targetLanguageTextBox = new System.Windows.Forms.TextBox();
			this.targetLanguageLabel = new System.Windows.Forms.Label();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
			this.button1 = new System.Windows.Forms.Button();
			this.enterSavesAndMovesCheckBox = new System.Windows.Forms.CheckBox();
			this.cbFromGoogleTranslate = new System.Windows.Forms.CheckBox();
			this.cbRegisterInCompositeConfig = new System.Windows.Forms.CheckBox();
			this.statusStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// filesListBox
			// 
			this.filesListBox.FormattingEnabled = true;
			this.filesListBox.Location = new System.Drawing.Point(171, 12);
			this.filesListBox.Name = "filesListBox";
			this.filesListBox.Size = new System.Drawing.Size(391, 95);
			this.filesListBox.TabIndex = 0;
			this.filesListBox.Click += new System.EventHandler(this.filesListBox_Click);
			this.filesListBox.SelectedIndexChanged += new System.EventHandler(this.filesListBox_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 12);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(28, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Files";
			// 
			// stringKeysListBox
			// 
			this.stringKeysListBox.FormattingEnabled = true;
			this.stringKeysListBox.Location = new System.Drawing.Point(171, 114);
			this.stringKeysListBox.Name = "stringKeysListBox";
			this.stringKeysListBox.Size = new System.Drawing.Size(391, 95);
			this.stringKeysListBox.TabIndex = 2;
			this.stringKeysListBox.Click += new System.EventHandler(this.stringKeysListBox_Click);
			this.stringKeysListBox.SelectedIndexChanged += new System.EventHandler(this.stringKeysListBox_SelectedIndexChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 114);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(60, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "String Keys";
			// 
			// sourceLanguageTextBox
			// 
			this.sourceLanguageTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.sourceLanguageTextBox.Location = new System.Drawing.Point(171, 244);
			this.sourceLanguageTextBox.Multiline = true;
			this.sourceLanguageTextBox.Name = "sourceLanguageTextBox";
			this.sourceLanguageTextBox.ReadOnly = true;
			this.sourceLanguageTextBox.Size = new System.Drawing.Size(391, 73);
			this.sourceLanguageTextBox.TabIndex = 4;
			// 
			// sourceLanguageLabel
			// 
			this.sourceLanguageLabel.AutoSize = true;
			this.sourceLanguageLabel.Location = new System.Drawing.Point(12, 244);
			this.sourceLanguageLabel.Name = "sourceLanguageLabel";
			this.sourceLanguageLabel.Size = new System.Drawing.Size(92, 13);
			this.sourceLanguageLabel.TabIndex = 5;
			this.sourceLanguageLabel.Text = "Source Language";
			// 
			// targetLanguageTextBox
			// 
			this.targetLanguageTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.targetLanguageTextBox.Location = new System.Drawing.Point(171, 323);
			this.targetLanguageTextBox.Multiline = true;
			this.targetLanguageTextBox.Name = "targetLanguageTextBox";
			this.targetLanguageTextBox.Size = new System.Drawing.Size(391, 73);
			this.targetLanguageTextBox.TabIndex = 6;
			this.targetLanguageTextBox.TextChanged += new System.EventHandler(this.targetLanguageTextBox_TextChanged);
			this.targetLanguageTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
			// 
			// targetLanguageLabel
			// 
			this.targetLanguageLabel.AutoEllipsis = true;
			this.targetLanguageLabel.AutoSize = true;
			this.targetLanguageLabel.BackColor = System.Drawing.Color.Transparent;
			this.targetLanguageLabel.Location = new System.Drawing.Point(15, 323);
			this.targetLanguageLabel.MaximumSize = new System.Drawing.Size(150, 0);
			this.targetLanguageLabel.Name = "targetLanguageLabel";
			this.targetLanguageLabel.Size = new System.Drawing.Size(89, 13);
			this.targetLanguageLabel.TabIndex = 7;
			this.targetLanguageLabel.Text = "Target Language";
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
			this.statusStrip1.Location = new System.Drawing.Point(0, 502);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(600, 22);
			this.statusStrip1.TabIndex = 8;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// toolStripStatusLabel1
			// 
			this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
			this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(487, 215);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 9;
			this.button1.Text = "Find missing";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// enterSavesAndMovesCheckBox
			// 
			this.enterSavesAndMovesCheckBox.AutoSize = true;
			this.enterSavesAndMovesCheckBox.Checked = true;
			this.enterSavesAndMovesCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.enterSavesAndMovesCheckBox.Location = new System.Drawing.Point(171, 402);
			this.enterSavesAndMovesCheckBox.Name = "enterSavesAndMovesCheckBox";
			this.enterSavesAndMovesCheckBox.Size = new System.Drawing.Size(222, 17);
			this.enterSavesAndMovesCheckBox.TabIndex = 10;
			this.enterSavesAndMovesCheckBox.Text = "Pressing Enter will save and move to next";
			this.enterSavesAndMovesCheckBox.UseVisualStyleBackColor = true;
			// 
			// cbFromGoogleTranslate
			// 
			this.cbFromGoogleTranslate.AutoSize = true;
			this.cbFromGoogleTranslate.Checked = true;
			this.cbFromGoogleTranslate.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbFromGoogleTranslate.Location = new System.Drawing.Point(171, 425);
			this.cbFromGoogleTranslate.Name = "cbFromGoogleTranslate";
			this.cbFromGoogleTranslate.Size = new System.Drawing.Size(198, 17);
			this.cbFromGoogleTranslate.TabIndex = 14;
			this.cbFromGoogleTranslate.Text = "Get translation from GoogleTranslate";
			this.cbFromGoogleTranslate.UseVisualStyleBackColor = true;
			// 
			// cbRegisterInCompositeConfig
			// 
			this.cbRegisterInCompositeConfig.AutoSize = true;
			this.cbRegisterInCompositeConfig.Checked = true;
			this.cbRegisterInCompositeConfig.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbRegisterInCompositeConfig.Location = new System.Drawing.Point(171, 448);
			this.cbRegisterInCompositeConfig.Name = "cbRegisterInCompositeConfig";
			this.cbRegisterInCompositeConfig.Size = new System.Drawing.Size(261, 17);
			this.cbRegisterInCompositeConfig.TabIndex = 15;
			this.cbRegisterInCompositeConfig.Text = "Automatically register updates in Composite.config";
			this.cbRegisterInCompositeConfig.UseVisualStyleBackColor = true;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(600, 524);
			this.Controls.Add(this.cbRegisterInCompositeConfig);
			this.Controls.Add(this.cbFromGoogleTranslate);
			this.Controls.Add(this.enterSavesAndMovesCheckBox);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.targetLanguageLabel);
			this.Controls.Add(this.targetLanguageTextBox);
			this.Controls.Add(this.sourceLanguageLabel);
			this.Controls.Add(this.sourceLanguageTextBox);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.stringKeysListBox);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.filesListBox);
			this.Name = "Form1";
			this.Text = "C1 Localization Tool";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox filesListBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox stringKeysListBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox sourceLanguageTextBox;
        private System.Windows.Forms.Label sourceLanguageLabel;
        private System.Windows.Forms.TextBox targetLanguageTextBox;
        private System.Windows.Forms.Label targetLanguageLabel;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Button button1;
		private System.Windows.Forms.CheckBox enterSavesAndMovesCheckBox;
		private System.Windows.Forms.CheckBox cbFromGoogleTranslate;
		private System.Windows.Forms.CheckBox cbRegisterInCompositeConfig;
    }
}

