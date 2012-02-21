namespace LocalizationTool
{
    partial class PrintForm
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
            this.printButton = new System.Windows.Forms.Button();
            this.filesListBox = new System.Windows.Forms.ListBox();
            this.lbFiles = new System.Windows.Forms.Label();
            this.wbPrint = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // printButton
            // 
            this.printButton.Location = new System.Drawing.Point(480, 59);
            this.printButton.Name = "printButton";
            this.printButton.Size = new System.Drawing.Size(75, 23);
            this.printButton.TabIndex = 0;
            this.printButton.Text = "Print";
            this.printButton.UseVisualStyleBackColor = true;
            this.printButton.Click += new System.EventHandler(this.printButton_Click);
            // 
            // filesListBox
            // 
            this.filesListBox.FormattingEnabled = true;
            this.filesListBox.Location = new System.Drawing.Point(52, 13);
            this.filesListBox.Name = "filesListBox";
            this.filesListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.filesListBox.Size = new System.Drawing.Size(422, 69);
            this.filesListBox.TabIndex = 1;
            this.filesListBox.SelectedIndexChanged += new System.EventHandler(this.filesListBox_SelectedIndexChanged);
            // 
            // lbFiles
            // 
            this.lbFiles.AutoSize = true;
            this.lbFiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbFiles.Location = new System.Drawing.Point(13, 13);
            this.lbFiles.Name = "lbFiles";
            this.lbFiles.Size = new System.Drawing.Size(33, 13);
            this.lbFiles.TabIndex = 2;
            this.lbFiles.Text = "Files";
            // 
            // wbPrint
            // 
            this.wbPrint.Location = new System.Drawing.Point(52, 88);
            this.wbPrint.MinimumSize = new System.Drawing.Size(20, 20);
            this.wbPrint.Name = "wbPrint";
            this.wbPrint.Size = new System.Drawing.Size(653, 371);
            this.wbPrint.TabIndex = 3;
            // 
            // PrintForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(726, 471);
            this.Controls.Add(this.wbPrint);
            this.Controls.Add(this.lbFiles);
            this.Controls.Add(this.filesListBox);
            this.Controls.Add(this.printButton);
            this.Name = "PrintForm";
            this.Text = "Print";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PrintForm_FormClosing);
            this.Load += new System.EventHandler(this.PrintForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button printButton;
        private System.Windows.Forms.ListBox filesListBox;
        private System.Windows.Forms.Label lbFiles;
        private System.Windows.Forms.WebBrowser wbPrint;

    }
}