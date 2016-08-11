using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LocalizationTool
{
    public static class Prompt
    {
        public static string ShowDialog(string caption, string okButtonValue, string startValue)
        {
            Form prompt = new Form()
            {
                Width = 455,
                Height = 175,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };
            TextBox textBox = new TextBox() { Left = 20, Top = 20, Width = 400, Height = 73, Text = startValue, Multiline = true };
            Button confirmation = new Button() { Text = okButtonValue, Left = 210, Width = 100, Top = 100, DialogResult = DialogResult.OK };
            Button cancel = new Button() { Text = "Cancel", Left = 320, Width = 100, Top = 100, DialogResult = DialogResult.Cancel };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(cancel);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }
    }
}
