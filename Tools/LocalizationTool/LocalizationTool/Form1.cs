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
using System.Drawing.Printing;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Threading;

namespace LocalizationTool
{
	public partial class Form1 : Form
	{
		#region Variables
		private FilesListItem _selectedFile = null;
		private KeysListItem _selectedKey = null;
		private string _progressLabelFormat = "{0}% done – {1} out of {2} strings are missing translation.";
		private BindingList<FilesListItem> _filesListSource;
		private BindingList<KeysListItem> _keysListSource;
		#endregion

		#region Form Events
		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			try
			{
				//On startup – locate strings in the translation that has “file+key” that do not exist in the English version (i.e. unused keys). If any are found, dump them to a single “UnknownStrings.xml” file and do a message box “Strings with invalid keys was found and has been cleaned up. Strings have been moved to UnknownStrings.xml
				if (FileHandler.CleanUnknownStrings())
				{
					MessageBox.Show("The strings with invalid keys have been found and cleaned up. The strings have been moved to " + Settings.UnknownStringsFilePath);
				}

				if (!Directory.Exists(Settings.TargetLocalizationDirectory))
				{
					Directory.CreateDirectory(Settings.TargetLocalizationDirectory);
				}

				FileHandler.AutoTranslateEmptyStrings();

				//re-save all target files to have the same structure as source files
				ThreadPool.QueueUserWorkItem((_state) =>
				{
					FileHandler.SaveTargetFilesStructureAsSource();
				});

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
					MoveToNextMissing();
					FillProgressInfo();
					targetLanguageTextBox.Focus();
					sourceLanguageLabel.Text = Settings.SourceCulture.DisplayName;
					targetLanguageLabel.Text = Settings.TargetCulture.DisplayName;
				}
				else
				{
					MessageBox.Show(string.Format("No localization files ending with {0} found in {1}", FileHandler.SourceFileEnding, Settings.LocalizationDirectory));
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(string.Format("Error: {0}. Inner: {1}", ex.Message, ex.InnerException != null ? ex.InnerException.Message : ""));
			}
		}

		void Form1_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
		{
			SaveString();
		}

		#endregion

		#region Events

		private void filesListBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			var newSelectedFile = (FilesListItem)filesListBox.SelectedValue;
			if (newSelectedFile != null && newSelectedFile != _selectedFile)
			{
				_selectedFile = newSelectedFile;
				PopulateStringKeysListBox();
			}
		}

		private void filesListBox_Click(object sender, EventArgs e)
		{
			if (_selectedKey != null)
			{
				SaveString();
			}
		}

		private void stringKeysListBox_Click(object sender, EventArgs e)
		{
			if (_selectedKey != null)
			{
				SaveString();
				if (_selectedKey.IsFlagged)
					UpdateFlagsUI();
			}
		}

		private void stringKeysListBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			var newSelectedKey = (KeysListItem)keysListBox.SelectedValue;
			if (newSelectedKey != null && newSelectedKey != _selectedKey)
			{
				_selectedKey = newSelectedKey;
				UpdateStringTextBoxes();
				UpdateFlagsUI();
			}
		}

		private void targetLanguageTextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyValue == 13)
			{
				e.Handled = true;
				if (enterSavesAndMovesCheckBox.Checked == true && _selectedKey != null)
				{
					SaveString();
					MoveToNextMissing();
				}
			}
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

		private void printForComparison_Click(object sender, EventArgs e)
		{
			StringBuilder print = new StringBuilder();
			print.Append(@"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd""><html><head><title></title>
                <style type='text/css'>
                  body {font-size: 11px; font-family: Arial;}
                  b {font-size: 11px; padding-left: 5px;}
                  table.report {width:100%; border: solid 1px silver; margin-bottom: 10px; }
                  table.report td {padding: 2px; width: 50%; font-size: 11px; font-family: Arial;}
                  table.report td.first {border-right: solid 1px silver;}
                
                  table {page-break-after: auto; page-break-before: avoid;}
                  tr, td { page-break-inside:avoid; page-break-after: avoid; page-break-before: avoid;}
                  b      { page-break-before:auto; page-break-after:avoid; display:block;}
                </style>
                </head><body>");

			var fileSterm = _selectedFile.Name;
			print.AppendFormat("<h3>{0} ({1},{2})</h3>", fileSterm, Settings.SourceCulture, Settings.TargetCulture);

			var targetFile = FileHandler.GetTargetDocument(fileSterm);

			var sourcefileElements = FileHandler.SourceFilesBySterm[fileSterm].Root.Elements("string").ToDictionary(k => k.Attribute("key").Value, v => v.Attribute("value").Value);
			var targetFileElements = targetFile != null ? targetFile.Root.Elements("string").ToDictionary(k => k.Attribute("key").Value, v => v.Attribute("value").Value) : new Dictionary<string, string>();

			foreach (var el in sourcefileElements)
			{
				print.AppendFormat("<b>{0}</b>", el.Key);
				var targetValue = "<br/>";
				if (targetFileElements.ContainsKey(el.Key))
					targetValue = targetFileElements[el.Key];

				print.AppendFormat("<table class='report'><tr><td class='first'>{0}</td><td>{1}</td></tr></table>", el.Value, targetValue).AppendLine();
			}

			print.Append("</body></html>");
			if (!Directory.Exists(Settings.ReportsDirectory))
				Directory.CreateDirectory(Settings.ReportsDirectory);
			var filePath = Path.Combine(Settings.ReportsDirectory, fileSterm + ".html");
			File.WriteAllText(filePath, print.ToString(), Encoding.UTF8);
			ProcessStartInfo sInfo = new ProcessStartInfo(filePath);
			Process.Start(sInfo);
		}

		private void txtSearch_TextChanged(object sender, EventArgs e)
		{
			FileHandler.FilterDataSource(txtSearch.Text, chbShowFlaggedOnly.Checked);
			_filesListSource.Clear();
			foreach (var s in FileHandler.StringsCurrentDataSource)
			{
				_filesListSource.Add(new FilesListItem(s.Key));
			}
			_selectedFile = (FilesListItem)filesListBox.SelectedValue;
			PopulateStringKeysListBox();
		}

		private void btnFlagThis_Click(object sender, EventArgs e)
		{
			FlagWindow flagThis = new FlagWindow();

			if (flagThis.ShowDialog(this) == DialogResult.OK)
			{
				FileHandler.AddFlag(_selectedFile.Name, _selectedKey.Name, flagThis.FlagComment);
				RefreshKeysListSelectedItem();
				UpdateFlagsUI();
			}
		}

		private void btnEditRemoveFlag_Click(object sender, EventArgs e)
		{
			FlagWindow flagThis = new FlagWindow();
			flagThis.FlagComment = _selectedKey.Comment;
			flagThis.CancelButtonText = "Remove";
			if (flagThis.ShowDialog(this) == DialogResult.Abort)
			{
				FileHandler.RemoveFlag(_selectedFile.Name, _selectedKey.Name, flagThis.FlagComment);
				if (chbShowFlaggedOnly.Checked)
				{
					var itemToRemove = _keysListSource.Where(k => k.File == _selectedFile.Name && k.Name == _selectedKey.Name).FirstOrDefault();
					_keysListSource.Remove(itemToRemove);
					if (_keysListSource.Count() == 0)
					{
						_filesListSource.Remove(_selectedFile);
					}
				}
			}
			else
			{
				FileHandler.UpdateFlag(_selectedFile.Name, _selectedKey.Name, flagThis.FlagComment);
			}
			RefreshKeysListSelectedItem();
			UpdateFlagsUI();

		}

		private void chbShowFlaggedOnly_CheckedChanged(object sender, EventArgs e)
		{
			FileHandler.FilterDataSource(txtSearch.Text, chbShowFlaggedOnly.Checked);
			_filesListSource.Clear();
			foreach (var s in FileHandler.StringsCurrentDataSource)
			{
				_filesListSource.Add(new FilesListItem(s.Key));
			}
			_selectedFile = (FilesListItem)filesListBox.SelectedValue;
			PopulateStringKeysListBox();
		}

		#endregion

		#region Private Methods
		private void MoveToNextMissing()
		{
			bool missingStringFound = false;

			string nextKey = FileHandler.FindNextMissingKey(_selectedFile.Name);

			if (nextKey != null)
			{
				keysListBox.SelectedItem = _keysListSource.Where(item => item.Name == nextKey && item.File == _selectedFile.Name).FirstOrDefault();
				missingStringFound = true;
			}
			else
			{
				foreach (string fileStem in FileHandler.StringsCurrentDataSource.Keys)
				{
					nextKey = FileHandler.FindNextMissingKey(fileStem);

					if (nextKey != null)
					{
						filesListBox.SelectedItem = _filesListSource.Where(s => s.Name == fileStem).FirstOrDefault();
						keysListBox.SelectedItem = _keysListSource.Where(it => it.Name == nextKey && it.File == _selectedFile.Name).FirstOrDefault();
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
			bool isSaved = false;
			toolStripStatusLabel1.Text = "Saving...";
			string targetString = targetLanguageTextBox.Text;
			//  targetString = targetString.Replace("\n", "");
			//targetString = targetString.Replace("\r", "");
			targetString = targetString.Trim();
			if (string.IsNullOrEmpty(targetString) && !string.IsNullOrEmpty(sourceLanguageTextBox.Text))
			{
				toolStripStatusLabel1.Text = string.Format("Empty string not saved: {0}, {1}", _selectedFile.Name, _selectedKey.Name);
				return;
			}

			isSaved = FileHandler.SaveTargetString(_selectedFile.Name, _selectedKey.Name, targetString);

			//if a Source string gets translated, and this string exists for other keys, those keys gets automatically updated (saving the user time)
			if (!string.IsNullOrEmpty(sourceLanguageTextBox.Text))
			{
				var keysWithTheSameText = FileHandler.SourceFilesBySterm[_selectedFile.Name].Root.Elements("string").Where(e => e.Attribute("value").Value == sourceLanguageTextBox.Text && e.Attribute("key").Value != _selectedKey.Name).Select(e => e.Attribute("key").Value).ToList();
				keysWithTheSameText.ForEach(key =>
					 {
						 isSaved = FileHandler.SaveTargetString(_selectedFile.Name, key, targetString) || isSaved;
					 });
			}

			if (isSaved)
			{
				toolStripStatusLabel1.Text = string.Format("Saved {0}, {1}", _selectedFile.Name, _selectedKey.Name);
				FillProgressInfo();
				RefreshFilesListSelectedItem();
				if (cbRegisterInCompositeConfig.Checked)
				{
					FileHandler.RegisterInCompositeConfig(_selectedFile.Name);
				}
			}
			else
				toolStripStatusLabel1.Text = string.Empty;
		}

		private void UpdateStringTextBoxes()
		{
			if (_selectedFile != null && _selectedKey != null)
			{
				sourceLanguageTextBox.Text = FileHandler.GetSourceString(_selectedFile.Name, _selectedKey.Name);
				targetLanguageTextBox.Text = FileHandler.GetTargetString(_selectedFile.Name, _selectedKey.Name);
				if (string.IsNullOrEmpty(targetLanguageTextBox.Text) && cbFromGoogleTranslate.Checked)
				{
					targetLanguageTextBox.Text = Translator.Translate(sourceLanguageTextBox.Text, Settings.SourceCulture, Settings.TargetCulture);
					toolStripStatusLabel1.Text = string.Format("Translated with GoogleTranslate {0}", _selectedKey.Name);
				}
			}
			else
			{
				sourceLanguageTextBox.Text = string.Empty;
				targetLanguageTextBox.Text = string.Empty;
			}
		}

		private void PopulateFilesListBox()
		{
			_filesListSource = new BindingList<FilesListItem>(
				FileHandler.SourceFileStems
				.Select(name => new FilesListItem(name))
				.ToList());
			filesListBox.DisplayMember = "Text";
			filesListBox.DataSource = _filesListSource;

			_selectedFile = (FilesListItem)filesListBox.SelectedValue;
		}

		private void PopulateStringKeysListBox()
		{
			_keysListSource = new BindingList<KeysListItem>();
			if (_selectedFile != null)
			{
				_keysListSource = new BindingList<KeysListItem>(
					FileHandler.StringsCurrentDataSource[_selectedFile.Name]
					.Select(key => new KeysListItem(_selectedFile.Name, key))
					.ToList());
			}
			keysListBox.DisplayMember = "Text";
			keysListBox.DataSource = _keysListSource;

			_selectedKey = (KeysListItem)keysListBox.SelectedValue;

		}

		private void FillProgressInfo()
		{
			// There is an info area at the top, “Progress” with the text “NN% done – NNN1 strings out of NNN2 missing translation.” where NNN1 is number of strings in translation, NNN2 is number of strings in English.
			int totalCountOfMissingStrings = FileHandler.TotalCountOfMissingStrings();
			double percentDone = 100.0 - (totalCountOfMissingStrings * 100.0) / FileHandler.TotalCountOfSourceTranstations;
			progressValue.Text = String.Format(_progressLabelFormat, String.Format("{0:0.0}", percentDone), totalCountOfMissingStrings, FileHandler.TotalCountOfSourceTranstations);
		}

		private void RefreshFilesListSelectedItem()
		{
			var selectedItem = filesListBox.SelectedItem as ListItemNotificationObject;
			if (selectedItem != null)
				selectedItem.Refresh(); // making listbox reread the text of the item
		}

		private void RefreshKeysListSelectedItem()
		{
			var selectedItem = keysListBox.SelectedItem as ListItemNotificationObject;
			if (selectedItem != null)
				selectedItem.Refresh(); // making listbox reread the text of the item
		}

		private void UpdateFlagsUI()
		{
			if (_selectedKey == null)
			{
				tooltipFlaged.Active = false;
				btnFlagThis.Visible = false;
				btnEditRemoveFlag.Visible = false;
				return;
			}

			if (_selectedKey.IsFlagged)
			{
				tooltipFlaged.SetToolTip(keysListBox, _selectedKey.Comment);
				toolStripStatusLabel1.Text = "Flag: " + _selectedKey.Comment;
				tooltipFlaged.Active = true;
				btnFlagThis.Visible = false;
				btnEditRemoveFlag.Visible = true;
			}
			else
			{
				btnFlagThis.Visible = true;
				btnEditRemoveFlag.Visible = false;
				tooltipFlaged.Active = false;
			}
		}

		#endregion

	}

	#region ListBoxItem Classes
	public class ListItemNotificationObject : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public void Refresh()
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs("Text"));
			}
		}
	}

	public class FilesListItem : ListItemNotificationObject
	{
		public FilesListItem() { }
		public FilesListItem(string name)
		{
			Name = name;
		}
		public string Name { get; set; }
		public string Text
		{
			get
			{
				int count = FileHandler.CountOfMissingStrings(Name);
				return count != 0 ? String.Format("{0} ({1} strings missing)", Name, count) : Name;
			}

		}
	}

	public class KeysListItem : ListItemNotificationObject
	{
		public KeysListItem() { }
		public KeysListItem(string file, string name)
		{
			Name = name;
			File = file;

		}
		public string File { get; set; }
		public string Name { get; set; }
		public string Text
		{
			get
			{
				XElement flag = FileHandler.GetFlag(File, Name);
				if (flag != null)
				{
					Comment = flag.Attribute("Comment").Value;
					IsFlagged = true;
					return String.Format("{0} (flagged)", Name);
				}
				else
				{
					Comment = string.Empty;
					IsFlagged = false;
					return Name;
				}
			}
		}
		public string Comment { get; set; }
		public bool IsFlagged { get; set; }
	}
	#endregion
}
