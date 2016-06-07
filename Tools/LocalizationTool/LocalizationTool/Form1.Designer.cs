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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.filesListBox = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.keysListBox = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.sourceLanguageTextBox = new System.Windows.Forms.TextBox();
            this.sourceLanguageLabel = new System.Windows.Forms.Label();
            this.targetLanguageTextBox = new System.Windows.Forms.TextBox();
            this.targetLanguageLabel = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.findFirstMissing = new System.Windows.Forms.Button();
            this.enterSavesAndMovesCheckBox = new System.Windows.Forms.CheckBox();
            this.cbFromGoogleTranslate = new System.Windows.Forms.CheckBox();
            this.cbRegisterInCompositeConfig = new System.Windows.Forms.CheckBox();
            this.progressLabel = new System.Windows.Forms.Label();
            this.progressValue = new System.Windows.Forms.Label();
            this.printForComparison = new System.Windows.Forms.Button();
            this.lbSearch = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.tooltipFlaged = new System.Windows.Forms.ToolTip(this.components);
            this.btnFlagThis = new System.Windows.Forms.Button();
            this.btnEditRemoveFlag = new System.Windows.Forms.Button();
            this.chbShowFlaggedOnly = new System.Windows.Forms.CheckBox();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // filesListBox
            // 
            this.filesListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.filesListBox.FormattingEnabled = true;
            this.filesListBox.Location = new System.Drawing.Point(171, 71);
            this.filesListBox.Name = "filesListBox";
            this.filesListBox.Size = new System.Drawing.Size(394, 82);
            this.filesListBox.TabIndex = 0;
            this.filesListBox.Click += new System.EventHandler(this.filesListBox_Click);
            this.filesListBox.SelectedIndexChanged += new System.EventHandler(this.filesListBox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 71);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Files";
            // 
            // keysListBox
            // 
            this.keysListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.keysListBox.FormattingEnabled = true;
            this.keysListBox.Location = new System.Drawing.Point(172, 187);
            this.keysListBox.Name = "keysListBox";
            this.keysListBox.Size = new System.Drawing.Size(394, 95);
            this.keysListBox.TabIndex = 2;
            this.keysListBox.Click += new System.EventHandler(this.stringKeysListBox_Click);
            this.keysListBox.SelectedIndexChanged += new System.EventHandler(this.stringKeysListBox_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 187);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "String Keys";
            // 
            // sourceLanguageTextBox
            // 
            this.sourceLanguageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sourceLanguageTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sourceLanguageTextBox.Location = new System.Drawing.Point(171, 317);
            this.sourceLanguageTextBox.Multiline = true;
            this.sourceLanguageTextBox.Name = "sourceLanguageTextBox";
            this.sourceLanguageTextBox.ReadOnly = true;
            this.sourceLanguageTextBox.Size = new System.Drawing.Size(394, 73);
            this.sourceLanguageTextBox.TabIndex = 4;
            this.sourceLanguageTextBox.DoubleClick += new System.EventHandler(this.sourceLanguageTextBox_DoubleClick);
            // 
            // sourceLanguageLabel
            // 
            this.sourceLanguageLabel.AutoSize = true;
            this.sourceLanguageLabel.Location = new System.Drawing.Point(15, 317);
            this.sourceLanguageLabel.Name = "sourceLanguageLabel";
            this.sourceLanguageLabel.Size = new System.Drawing.Size(92, 13);
            this.sourceLanguageLabel.TabIndex = 5;
            this.sourceLanguageLabel.Text = "Source Language";
            // 
            // targetLanguageTextBox
            // 
            this.targetLanguageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.targetLanguageTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.targetLanguageTextBox.Location = new System.Drawing.Point(171, 396);
            this.targetLanguageTextBox.Multiline = true;
            this.targetLanguageTextBox.Name = "targetLanguageTextBox";
            this.targetLanguageTextBox.Size = new System.Drawing.Size(394, 73);
            this.targetLanguageTextBox.TabIndex = 6;
            this.targetLanguageTextBox.TextChanged += new System.EventHandler(this.targetLanguageTextBox_TextChanged);
            this.targetLanguageTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.targetLanguageTextBox_KeyDown);
            // 
            // targetLanguageLabel
            // 
            this.targetLanguageLabel.AutoEllipsis = true;
            this.targetLanguageLabel.AutoSize = true;
            this.targetLanguageLabel.BackColor = System.Drawing.Color.Transparent;
            this.targetLanguageLabel.Location = new System.Drawing.Point(15, 396);
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
            this.statusStrip1.Location = new System.Drawing.Point(0, 558);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(581, 22);
            this.statusStrip1.TabIndex = 8;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // findFirstMissing
            // 
            this.findFirstMissing.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.findFirstMissing.Location = new System.Drawing.Point(458, 288);
            this.findFirstMissing.Name = "findFirstMissing";
            this.findFirstMissing.Size = new System.Drawing.Size(107, 23);
            this.findFirstMissing.TabIndex = 9;
            this.findFirstMissing.Text = "Find first missing";
            this.findFirstMissing.UseVisualStyleBackColor = true;
            this.findFirstMissing.Click += new System.EventHandler(this.findFirstMissing_Click);
            // 
            // enterSavesAndMovesCheckBox
            // 
            this.enterSavesAndMovesCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.enterSavesAndMovesCheckBox.AutoSize = true;
            this.enterSavesAndMovesCheckBox.Checked = true;
            this.enterSavesAndMovesCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.enterSavesAndMovesCheckBox.Location = new System.Drawing.Point(171, 486);
            this.enterSavesAndMovesCheckBox.Name = "enterSavesAndMovesCheckBox";
            this.enterSavesAndMovesCheckBox.Size = new System.Drawing.Size(222, 17);
            this.enterSavesAndMovesCheckBox.TabIndex = 10;
            this.enterSavesAndMovesCheckBox.Text = "Pressing Enter will save and move to next";
            this.enterSavesAndMovesCheckBox.UseVisualStyleBackColor = true;
            // 
            // cbFromGoogleTranslate
            // 
            this.cbFromGoogleTranslate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbFromGoogleTranslate.AutoSize = true;
            this.cbFromGoogleTranslate.Checked = true;
            this.cbFromGoogleTranslate.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbFromGoogleTranslate.Location = new System.Drawing.Point(171, 504);
            this.cbFromGoogleTranslate.Name = "cbFromGoogleTranslate";
            this.cbFromGoogleTranslate.Size = new System.Drawing.Size(198, 17);
            this.cbFromGoogleTranslate.TabIndex = 14;
            this.cbFromGoogleTranslate.Text = "Get translation from GoogleTranslate";
            this.cbFromGoogleTranslate.UseVisualStyleBackColor = true;
            // 
            // cbRegisterInCompositeConfig
            // 
            this.cbRegisterInCompositeConfig.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbRegisterInCompositeConfig.AutoSize = true;
            this.cbRegisterInCompositeConfig.Checked = true;
            this.cbRegisterInCompositeConfig.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbRegisterInCompositeConfig.Location = new System.Drawing.Point(171, 521);
            this.cbRegisterInCompositeConfig.Name = "cbRegisterInCompositeConfig";
            this.cbRegisterInCompositeConfig.Size = new System.Drawing.Size(261, 17);
            this.cbRegisterInCompositeConfig.TabIndex = 15;
            this.cbRegisterInCompositeConfig.Text = "Automatically register updates in Composite.config";
            this.cbRegisterInCompositeConfig.UseVisualStyleBackColor = true;
            // 
            // progressLabel
            // 
            this.progressLabel.AutoSize = true;
            this.progressLabel.Location = new System.Drawing.Point(12, 7);
            this.progressLabel.Name = "progressLabel";
            this.progressLabel.Size = new System.Drawing.Size(48, 13);
            this.progressLabel.TabIndex = 17;
            this.progressLabel.Text = "Progress";
            // 
            // progressValue
            // 
            this.progressValue.AutoSize = true;
            this.progressValue.Location = new System.Drawing.Point(171, 7);
            this.progressValue.Name = "progressValue";
            this.progressValue.Size = new System.Drawing.Size(0, 13);
            this.progressValue.TabIndex = 18;
            // 
            // printForComparison
            // 
            this.printForComparison.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.printForComparison.Location = new System.Drawing.Point(415, 158);
            this.printForComparison.Name = "printForComparison";
            this.printForComparison.Size = new System.Drawing.Size(150, 23);
            this.printForComparison.TabIndex = 19;
            this.printForComparison.Text = "Generate comparison report";
            this.printForComparison.UseVisualStyleBackColor = true;
            this.printForComparison.Click += new System.EventHandler(this.PrintForComparisonClick);
            // 
            // lbSearch
            // 
            this.lbSearch.AutoSize = true;
            this.lbSearch.Location = new System.Drawing.Point(12, 25);
            this.lbSearch.Name = "lbSearch";
            this.lbSearch.Size = new System.Drawing.Size(78, 13);
            this.lbSearch.TabIndex = 20;
            this.lbSearch.Text = "Filter by search";
            // 
            // txtSearch
            // 
            this.txtSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearch.Location = new System.Drawing.Point(171, 25);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(394, 20);
            this.txtSearch.TabIndex = 21;
            this.txtSearch.TextChanged += new System.EventHandler(this.TxtSearchTextChanged);
            // 
            // tooltipFlaged
            // 
            this.tooltipFlaged.IsBalloon = true;
            // 
            // btnFlagThis
            // 
            this.btnFlagThis.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFlagThis.Location = new System.Drawing.Point(490, 475);
            this.btnFlagThis.Name = "btnFlagThis";
            this.btnFlagThis.Size = new System.Drawing.Size(75, 23);
            this.btnFlagThis.TabIndex = 22;
            this.btnFlagThis.Text = "Flag this...";
            this.btnFlagThis.UseVisualStyleBackColor = true;
            this.btnFlagThis.Click += new System.EventHandler(this.BtnFlagThisClick);
            // 
            // btnEditRemoveFlag
            // 
            this.btnEditRemoveFlag.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEditRemoveFlag.Location = new System.Drawing.Point(454, 475);
            this.btnEditRemoveFlag.Name = "btnEditRemoveFlag";
            this.btnEditRemoveFlag.Size = new System.Drawing.Size(111, 23);
            this.btnEditRemoveFlag.TabIndex = 23;
            this.btnEditRemoveFlag.Text = "Edit/remove flag";
            this.btnEditRemoveFlag.UseVisualStyleBackColor = true;
            this.btnEditRemoveFlag.Click += new System.EventHandler(this.BtnEditRemoveFlagClick);
            // 
            // chbShowFlaggedOnly
            // 
            this.chbShowFlaggedOnly.AutoSize = true;
            this.chbShowFlaggedOnly.Location = new System.Drawing.Point(172, 50);
            this.chbShowFlaggedOnly.Name = "chbShowFlaggedOnly";
            this.chbShowFlaggedOnly.Size = new System.Drawing.Size(113, 17);
            this.chbShowFlaggedOnly.TabIndex = 24;
            this.chbShowFlaggedOnly.Text = "Show flagged only";
            this.chbShowFlaggedOnly.UseVisualStyleBackColor = true;
            this.chbShowFlaggedOnly.CheckedChanged += new System.EventHandler(this.ChbShowFlaggedOnlyCheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(581, 580);
            this.Controls.Add(this.filesListBox);
            this.Controls.Add(this.chbShowFlaggedOnly);
            this.Controls.Add(this.btnFlagThis);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.lbSearch);
            this.Controls.Add(this.printForComparison);
            this.Controls.Add(this.progressValue);
            this.Controls.Add(this.progressLabel);
            this.Controls.Add(this.cbRegisterInCompositeConfig);
            this.Controls.Add(this.cbFromGoogleTranslate);
            this.Controls.Add(this.enterSavesAndMovesCheckBox);
            this.Controls.Add(this.findFirstMissing);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.targetLanguageLabel);
            this.Controls.Add(this.targetLanguageTextBox);
            this.Controls.Add(this.sourceLanguageLabel);
            this.Controls.Add(this.sourceLanguageTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.keysListBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnEditRemoveFlag);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(594, 618);
            this.Name = "Form1";
            this.Text = "Composite C1 Localization Tool";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox filesListBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox keysListBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox sourceLanguageTextBox;
        private System.Windows.Forms.Label sourceLanguageLabel;
        private System.Windows.Forms.TextBox targetLanguageTextBox;
        private System.Windows.Forms.Label targetLanguageLabel;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Button findFirstMissing;
		private System.Windows.Forms.CheckBox enterSavesAndMovesCheckBox;
		private System.Windows.Forms.CheckBox cbFromGoogleTranslate;
        private System.Windows.Forms.CheckBox cbRegisterInCompositeConfig;
        private System.Windows.Forms.Label progressLabel;
        private System.Windows.Forms.Label progressValue;
        private System.Windows.Forms.Button printForComparison;
        private System.Windows.Forms.Label lbSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.ToolTip tooltipFlaged;
        private System.Windows.Forms.Button btnFlagThis;
        private System.Windows.Forms.Button btnEditRemoveFlag;
		private System.Windows.Forms.CheckBox chbShowFlaggedOnly;
    }
}

