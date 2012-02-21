using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LocalizationTool
{
    public partial class FlagWindow : Form
    {

        public FlagWindow()
        {
            InitializeComponent();
            
        }

        public string FlagComment
        {
            get { return txtComment.Text; }
            set { txtComment.Text = value; }
        }

        public string CancelButtonText
        {
            set { btnCancel.Text = value; }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Abort;
            this.Close();
        }

        private void FlagWindow_Load(object sender, EventArgs e)
        {
         
        }

        private void txtComment_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                e.Handled = true;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

    }
}
