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

		public Form1()
		{
			InitializeComponent();

		}

		private void PopulateFilesListBox()
		{
			List<string> filePaths = FileHandler.GetSourceFileStems().ToList();
			filesListBox.DataSource = filePaths;
			_selectedFileStem = (string)filesListBox.SelectedValue;
		}

		private void PopulateStringKeysListBox()
		{
			List<string> stingKeys = FileHandler.GetStringKeys(_selectedFileStem).ToList();
			stringKeysListBox.DataSource = stingKeys;
			_selectedStringKey = (string)stringKeysListBox.SelectedValue;
		}

		private void filesListBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			_selectedFileStem = (string)filesListBox.SelectedValue;
			PopulateStringKeysListBox();
		}

		private void stringKeysListBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			_selectedStringKey = (string)stringKeysListBox.SelectedValue;
			UpdateStringTextBoxes();
		}

		private void UpdateStringTextBoxes()
		{
			sourceLanguageTextBox.Text = FileHandler.GetSourceString(_selectedFileStem, _selectedStringKey);
			targetLanguageTextBox.Text = FileHandler.GetTargetString(_selectedFileStem, _selectedStringKey);
			if (string.IsNullOrEmpty(targetLanguageTextBox.Text) && cbFromGoogleTranslate.Checked)
			{
				targetLanguageTextBox.Text = Translator.Translate(sourceLanguageTextBox.Text, Settings.SourceCulture, Settings.TargetCulture);
				toolStripStatusLabel1.Text = string.Format("Translated with GoogleTranslate {0}", _selectedStringKey);
			}
		}

		private void textBox1_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyValue == 13)
			{
				e.Handled = true;
				if (enterSavesAndMovesCheckBox.Checked == true)
				{
					SaveString();
					MoveToNextMissing();
				}
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

		private void filesListBox_Click(object sender, EventArgs e)
		{
			SaveString();
		}

		private void stringKeysListBox_Click(object sender, EventArgs e)
		{
			SaveString();
		}

		void Form1_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
		{
			SaveString();
		}

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
				foreach (string fileStem in FileHandler.GetSourceFileStems())
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

		private void targetLanguageTextBox_TextChanged(object sender, EventArgs e)
		{
			if (targetLanguageTextBox.Text.Contains("\n") == true)
			{
				targetLanguageTextBox.Text = targetLanguageTextBox.Text.Replace("\n", "");
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			MoveToNextMissing();
			targetLanguageTextBox.Focus();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			SaveString();
			this.Close();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			sourceLanguageLabel.Text = Settings.SourceCulture.DisplayName;
			targetLanguageLabel.Text = Settings.TargetCulture.DisplayName;

			try
			{
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

	}
}
