using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace LocalizationTool
{
    public partial class CredentialForm : Form
    {
        public CredentialForm()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CredentialHelper.SaveInRegistery(CredentialHelper.SourceRepoUserName, SourceRepoUserName.Text);
            CredentialHelper.SaveInRegistery(CredentialHelper.SourceRepoPassword, SourceRepoPassword.Text);
            CredentialHelper.SaveInRegistery(CredentialHelper.TargetRepoUserName, TargetRepoUserName.Text);
            CredentialHelper.SaveInRegistery(CredentialHelper.TargetRepoPassword, TargetRepoPassword.Text);
            
            this.Close();
        }

        private void CredentialForm_Load(object sender, EventArgs e)
        {
            SourceRepoUserName.Text = CredentialHelper.GetFormRegistery(CredentialHelper.SourceRepoUserName);
            SourceRepoPassword.Text = CredentialHelper.GetFormRegistery(CredentialHelper.SourceRepoPassword);
            TargetRepoUserName.Text = CredentialHelper.GetFormRegistery(CredentialHelper.TargetRepoUserName);
            TargetRepoPassword.Text = CredentialHelper.GetFormRegistery(CredentialHelper.TargetRepoPassword);
            
        }
    }
}
