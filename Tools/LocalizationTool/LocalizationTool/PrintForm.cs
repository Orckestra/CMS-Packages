using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace LocalizationTool
{
    public partial class PrintForm : Form
    {
        private WebBrowser _webBrowserForPrinting = new WebBrowser();

        public PrintForm()
        {
            InitializeComponent();
        }

        #region Events
        private void PrintForm_Load(object sender, EventArgs e)
        {
            filesListBox.DataSource = FileHandler.SourceFileStems.ToList();
        }

        private void PrintForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            wbPrint.Dispose();
        }
        private void printButton_Click(object sender, EventArgs e)
        {
            Print();
        }
        #endregion

        #region Private methods
        private void Print()
        {
            wbPrint.ShowPrintDialog();
        }

        #endregion

        private void filesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            StringBuilder print = new StringBuilder();
            print.Append(@"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd""><html><head><title></title>
                <style type='text/css'>
                  body {font-size: 11px; font-family: Arial;}
                  b {font-size: 11px; padding-left: 5px;}
                  table.report {width:100%; border: solid 1px silver; margin-bottom: 10px; }
                  table.report td {padding: 2px; width: 50%; font-size: 11px; font-family: Arial;}
                  table.report td.first {border-right: solid 1px silver;}
                
                  table {page-break-after: avoid; page-break-before: avoid;}
                  tr, td { page-break-inside:avoid; page-break-after: avoid; page-break-before: avoid;}
                  b     { page-break-before:auto; display: block;}
                   
                </style>
                </head><body>");
            foreach (var file in filesListBox.SelectedItems)
            {
                var fileSterm = file.ToString();
                print.AppendFormat("<h3>{0} ({1},{2})</h3>", fileSterm, Settings.SourceCulture, Settings.TargetCulture);

                var sourcefileElements = FileHandler.SourceFilesBySterm[fileSterm].Root.Elements("string").ToDictionary(k => k.Attribute("key").Value, v => v.Attribute("value").Value);
                var targetFileElements = new Dictionary<string, string>();
               
                var targetFile = FileHandler.GetTargetDocument(fileSterm);
                if(targetFile!=null) 
                    targetFileElements = targetFile.Root.Elements("string").ToDictionary(k => k.Attribute("key").Value, v => v.Attribute("value").Value);
                foreach (var el in sourcefileElements)
                {
                    print.AppendFormat("<b>{0}</b>", el.Key);
                    var targetValue = string.Empty;
                    if (!targetFileElements.TryGetValue(el.Key, out targetValue))
                        targetValue = "<br/>";
                    print.AppendFormat("<table class='report'><tr><td class='first'>{0}</td><td>{1}</td></tr></table>", el.Value, targetValue).AppendLine();
                }
            }
            print.Append("</body></html>");
            wbPrint.DocumentText = print.ToString();
        }

 
    }
}
