using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using System.IO;

namespace LocalizationTool
{
    public partial class Form1 : Form
    {
        private string _selectedFileStem = null;
        private string _selectedStringKey = null;
        private string _progressLabelFormat = "{0}% done – {1} strings out of {2} missing translation.";

        #region Form Events
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //On startup – locate strings in the translation that has “file+key” that do not exist in the English version (i.e. unused keys). If any are found, dump them to a single “UnknownStrings.xml” file and do a message box “Strings with invalid keys was found and has been cleaned up. Strings have been moved to UnknownStrings.xml
            if (FileHandler.LocateUnknownFilesKeys())
            {
                MessageBox.Show("Strings with invalid keys was found and has been cleaned up. Strings have been moved to " + Settings.UnknownStringsFilePath);
            }

            sourceLanguageLabel.Text = Settings.SourceCulture.DisplayName;
            targetLanguageLabel.Text = Settings.TargetCulture.DisplayName;

            try
            {
                FillProgressInfo();
                PopulateFilesListBox();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to fetch source files - check .config file for this application. Error was: " + ex.Message, "Error locating files for translation....");
                this.Close();
                return;
            }

            if (filesListBox.Items.Count > 0)
            {
                PopulateStringKeysListBox();

                MoveToNextMissing();

                targetLanguageTextBox.Focus();
            }
            else
            {
                MessageBox.Show(string.Format("No localization files ending with {0} found in {1}", FileHandler.SourceFileEnding, Settings.LocalizationDirectory));
            }
        }

        void Form1_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            SaveString();
        }

        #endregion

        private void filesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedFileStem = (string)filesListBox.SelectedValue;
            PopulateStringKeysListBox();
            FillProgressInfo();
        }

        private void filesListBox_Click(object sender, EventArgs e)
        {
            if (_selectedStringKey != null)
            {
                SaveString();
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                e.Handled = true;
                if (enterSavesAndMovesCheckBox.Checked == true && _selectedStringKey != null)
                {
                    SaveString();
                    MoveToNextMissing();
                }
            }
        }

        private void stringKeysListBox_Click(object sender, EventArgs e)
        {
            if (_selectedStringKey != null)
            {
                SaveString();
            }
        }

        private void stringKeysListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedStringKey = (string)stringKeysListBox.SelectedValue;
            UpdateStringTextBoxes();
        }

        private void targetLanguageTextBox_TextChanged(object sender, EventArgs e)
        {
            if (targetLanguageTextBox.Text.Contains("\n") == true)
            {
                targetLanguageTextBox.Text = targetLanguageTextBox.Text.Replace("\n", "");
            }
        }

        private void findFirstMissing_Click(object sender, EventArgs e)
        {
            MoveToNextMissing();
            targetLanguageTextBox.Focus();
        }

        //private void button2_Click(object sender, EventArgs e)
        //{
        //    SaveString();
        //    this.Close();
        //}

        private void filesListBox_Format(object sender, ListControlConvertEventArgs e)
        {
            string value = e.ListItem as string;
            int count = FileHandler.CountOfMissingStrings(value);
            if (count != 0)
            {
                e.Value = String.Format("{0} ({1} strings missing)", value, count);
            }
        }

        private void refreshProgressInfo_Click(object sender, EventArgs e)
        {
            PopulateFilesListBox();
            MoveToNextMissing();
            FillProgressInfo();
        }

        #region Private Methods
        private void MoveToNextMissing()
        {
            bool missingStringFound = false;

            string nextKey = FileHandler.FindNextMissingKey(_selectedFileStem);

            if (nextKey != null)
            {
                stringKeysListBox.SelectedItem = nextKey;
                missingStringFound = true;
            }
            else
            {
                foreach (string fileStem in FileHandler.SourceFileStems)
                {
                    nextKey = FileHandler.FindNextMissingKey(fileStem);

                    if (nextKey != null)
                    {
                        filesListBox.SelectedItem = fileStem;
                        stringKeysListBox.SelectedItem = nextKey;
                        missingStringFound = true;
                        break;
                    }
                }
            }

            if (missingStringFound == false)
            {
                toolStripStatusLabel1.Text = "No missing strings found";
            }
        }

        private void SaveString()
        {
            toolStripStatusLabel1.Text = "Saving...";

            string targetString = targetLanguageTextBox.Text;
            targetString = targetString.Replace("\n", "");
            targetString = targetString.Replace("\r", "");
            targetString = targetString.Trim();

            FileHandler.SetTargetString(_selectedFileStem, _selectedStringKey, targetString);

            //if a Source string gets translated, and this string exists for other keys, those keys gets automatically updated (saving the user time)
            var keysWithTheSameText = FileHandler.SourceFilesBySterm[_selectedFileStem].Root.Elements("string").Where(e => e.Attribute("value").Value == sourceLanguageTextBox.Text).Select(e => e.Attribute("key").Value).ToList();
            foreach (var key in keysWithTheSameText)
            {
                FileHandler.SetTargetString(_selectedFileStem, key, targetString);
            }
            //

            if (string.IsNullOrEmpty(targetString) == false)
            {
                toolStripStatusLabel1.Text = string.Format("Saved {0}, {1}", _selectedFileStem, _selectedStringKey);

            }
            else
            {
                toolStripStatusLabel1.Text = string.Format("Empty string not saved: {0}, {1}", _selectedFileStem, _selectedStringKey);
            }

            if (cbRegisterInCompositeConfig.Checked)
            {
                FileHandler.RegisterInCompositeConfig(_selectedFileStem);
            }
        }

        private void UpdateStringTextBoxes()
        {
            sourceLanguageTextBox.Text = FileHandler.GetSourceString(_selectedFileStem, _selectedStringKey);
            targetLanguageTextBox.Text = FileHandler.GetTargetString(_selectedFileStem, _selectedStringKey);
            if ((string.IsNullOrEmpty(targetLanguageTextBox.Text) || targetLanguageTextBox.Text == Settings.NotTranslatedStringValue) && cbFromGoogleTranslate.Checked)
            {
                targetLanguageTextBox.Text = Translator.Translate(sourceLanguageTextBox.Text, Settings.SourceCulture, Settings.TargetCulture);
                toolStripStatusLabel1.Text = string.Format("Translated with GoogleTranslate {0}", _selectedStringKey);
            }
        }

        private void PopulateFilesListBox()
        {
            List<string> filePaths = FileHandler.SourceFileStems.ToList();
            filesListBox.DataSource = filePaths;

            _selectedFileStem = (string)filesListBox.SelectedValue;
        }

        private void PopulateStringKeysListBox()
        {
            List<string> stingKeys = FileHandler.GetStringKeys(_selectedFileStem).ToList();
            stringKeysListBox.DataSource = stingKeys;
            _selectedStringKey = (string)stringKeysListBox.SelectedValue;
        }

        private void FillProgressInfo()
        {
            // There is an info area at the top, “Progress” with the text “NN% done – NNN1 strings out of NNN2 missing translation.” where NNN1 is number of strings in translation, NNN2 is number of strings in English.
            int totalCountOfMissingStrings = FileHandler.TotalCountOfMissingStrings();
            int percentDone = 100 - (totalCountOfMissingStrings * 100) / FileHandler.TotalCountOfSourceTranstations;
            progressValue.Text = String.Format(_progressLabelFormat, percentDone, FileHandler.TotalCountOfSourceTranstations - FileHandler.TotalCountOfMissingStrings(), FileHandler.TotalCountOfSourceTranstations);
        }

        #endregion
    }
}
