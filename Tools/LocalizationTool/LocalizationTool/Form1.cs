using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Management.Automation;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

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
					MessageBox.Show(string.Format("The strings with invalid keys have been found and cleaned up. The strings have been moved to {0}", Settings.UnknownStringsFilePath));
				}

                if (!Directory.Exists(Settings.LocalizationDirectory))
                {
                    Directory.CreateDirectory(Settings.LocalizationDirectory);
                }

                if (!Directory.Exists(Settings.TargetLocalizationDirectory))
				{
					Directory.CreateDirectory(Settings.TargetLocalizationDirectory);
				}

				FileHandler.AutoTranslateEmptyStrings();

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
					MessageBox.Show(string.Format("No localization files ending with {0} found in {1}, Please Check Out", FileHandler.SourceFileEnding, Settings.LocalizationDirectory));
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

			//re-save all changed target files to have the same structure as source files

			FileHandler.SaveTargetFilesStructureAsSource();

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

		private void PrintForComparisonClick(object sender, EventArgs e)
		{
			var print = new StringBuilder();
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
			var sInfo = new ProcessStartInfo(filePath);
			Process.Start(sInfo);
		}

		private void TxtSearchTextChanged(object sender, EventArgs e)
		{
            if (_selectedKey != null)
            {
                SaveString();
            }
            FileHandler.FilterDataSource(txtSearch.Text, chbShowFlaggedOnly.Checked);
			_filesListSource.Clear();
			foreach (var s in FileHandler.StringsCurrentDataSource)
			{
				_filesListSource.Add(new FilesListItem(s.Key));
			}
			_selectedFile = (FilesListItem)filesListBox.SelectedValue;
			PopulateStringKeysListBox();
		}

		private void BtnFlagThisClick(object sender, EventArgs e)
		{
			var flagThis = new FlagWindow();

			if (flagThis.ShowDialog(this) == DialogResult.OK)
			{
				FileHandler.AddFlag(_selectedFile.Name, _selectedKey.Name, flagThis.FlagComment);
				RefreshKeysListSelectedItem();
				UpdateFlagsUI();
			}
		}

		protected void BtnEditRemoveFlagClick(object sender, EventArgs e)
		{
			var flagThis = new FlagWindow { FlagComment = _selectedKey.Comment, CancelButtonText = "Remove" };
			if (flagThis.ShowDialog(this) == DialogResult.Abort)
			{
				FileHandler.RemoveFlag(_selectedFile.Name, _selectedKey.Name, flagThis.FlagComment);
				if (chbShowFlaggedOnly.Checked)
				{
					var itemToRemove = _keysListSource.FirstOrDefault(k => k.File == _selectedFile.Name && k.Name == _selectedKey.Name);
					_keysListSource.Remove(itemToRemove);
					FileHandler.StringsCurrentDataSource[_selectedFile.Name].Remove(_selectedKey.Name);
					if (!_keysListSource.Any())
					{
						FileHandler.StringsCurrentDataSource.Remove(_selectedFile.Name);
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

		private void ChbShowFlaggedOnlyCheckedChanged(object sender, EventArgs e)
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
				keysListBox.SelectedItem = _keysListSource.FirstOrDefault(item => item.Name == nextKey && item.File == _selectedFile.Name);
				missingStringFound = true;
			}
			else
			{
				foreach (var fileStem in FileHandler.StringsCurrentDataSource.Keys)
				{
					nextKey = FileHandler.FindNextMissingKey(fileStem);

					if (nextKey != null)
					{
						filesListBox.SelectedItem = _filesListSource.FirstOrDefault(s => s.Name == fileStem);
						keysListBox.SelectedItem = _keysListSource.FirstOrDefault(it => it.Name == nextKey && it.File == _selectedFile.Name);
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

			isSaved = FileHandler.SaveTargetString(_selectedFile?.Name, _selectedKey?.Name, targetString);

			//if a Source string gets translated, and this string exists for other keys, those keys gets automatically updated (saving the user time)
			if (!string.IsNullOrEmpty(sourceLanguageTextBox.Text))
			{
				foreach (var sourceFile in FileHandler.SourceFilesBySterm)
				{
					var keysWithTheSameText = sourceFile.Value.Root.Elements("string").Where(e => e.Attribute("value").Value == sourceLanguageTextBox.Text).Select(e => e.Attribute("key").Value).ToList();
					keysWithTheSameText.ForEach(key =>
					{
						if (!(sourceFile.Key == _selectedFile.Name && key == _selectedKey.Name))
							FileHandler.SaveTargetString(sourceFile.Key, key, targetString);
					});
				}
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
			var totalCountOfMissingStrings = FileHandler.TotalCountOfMissingStrings();
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
				toolStripStatusLabel1.Text = string.Format("Flag: {0}", _selectedKey.Comment);
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

        private void sourceLanguageTextBox_DoubleClick(object sender, EventArgs e)
        {
            string updatedString = Prompt.ShowDialog("Update source string", "Update string", sourceLanguageTextBox.Text);
            if (!string.IsNullOrEmpty(updatedString) && updatedString != sourceLanguageTextBox.Text)
            {
                var activeKey = (KeysListItem)keysListBox.SelectedValue;
                FileHandler.UpdateSourceString(activeKey.File, activeKey.Name, updatedString);
                UpdateStringTextBoxes();
            }
        }

	    private PowerShell PowerShellInstance;

	    private async Task<bool> RunScript(string path)
	    {
            PowerShellInstance = PowerShell.Create();

            PowerShellInstance.AddScript(LoadScript(path));

            PowerShellInstance.AddArgument(CredentialHelper.GetFormRegistery(CredentialHelper.SourceRepoUserName));
            PowerShellInstance.AddArgument(CredentialHelper.GetFormRegistery(CredentialHelper.SourceRepoPassword));
            PowerShellInstance.AddArgument(CredentialHelper.GetFormRegistery(CredentialHelper.TargetRepoUserName));
            PowerShellInstance.AddArgument(CredentialHelper.GetFormRegistery(CredentialHelper.TargetRepoPassword));

            PSDataCollection<PSObject> outputCollection = new PSDataCollection<PSObject>();
            outputCollection.DataAdded += outputCollection_DataAdded;

            PowerShellInstance.Streams.Error.DataAdded += Error_DataAdded;

            IAsyncResult result = PowerShellInstance.BeginInvoke<PSObject,PSObject>(null, outputCollection);

	        while (result.IsCompleted == false)
	        {
                await Task.Delay(100);
	        }
	        return PowerShellInstance.HadErrors;
	    }

	    private async void checkoutButton_Click(object sender, EventArgs e)
	    {
	        var script = GetScriptName();

	        toolStripStatusLabel1.Text = "Wait For check out/Update to complete";
            DisableSourceControlButtons(false);
            var result = await RunScript(script);
            DisableSourceControlButtons(true);
            toolStripStatusLabel1.Text = "";
            MessageBox.Show(result? "Check Out/Update Finished with errors":"Check Out/Update Finished");
	        FileHandler.UpdateFileHandler();
            FileHandler.FilterDataSource("", false);
            _filesListSource.Clear();
            foreach (var s in FileHandler.StringsCurrentDataSource)
            {
                _filesListSource.Add(new FilesListItem(s.Key));
            }
            _selectedFile = (FilesListItem)filesListBox.SelectedValue;
            PopulateStringKeysListBox();

	    }

	    private static string GetScriptName()
	    {
	        string script;
	        SHA1 sha = new SHA1CryptoServiceProvider();
	        using (var stream = File.OpenRead(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile))
	        {
	            var hash = BitConverter.ToString(sha.ComputeHash(stream));

	            if (hash == CredentialHelper.GetFormRegistery("ConfigHash"))
	            {
	                script = "UpdateSource.ps1";
	            }
	            else
	            {
	                CredentialHelper.SaveInRegistery("ConfigHash", hash);
	                script = "CheckoutSource.ps1";
	            }
	        }
	        return script;
	    }

	    private async void commitButton_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "Wait For Commit to complete";
            DisableSourceControlButtons(false);
            var result = await RunScript("CommitChanges.ps1");
            DisableSourceControlButtons(true);
            toolStripStatusLabel1.Text = "";
            MessageBox.Show(result ? "Commit Finished with errors":"Commit Finished");
        }

        void outputCollection_DataAdded(object sender, DataAddedEventArgs e)
        {
            using (StreamWriter w = File.AppendText("log.txt"))
            {
                w.WriteLine(((PSDataCollection<PSObject>)sender)[e.Index].ToString());
            }
        }

        void Error_DataAdded(object sender, DataAddedEventArgs e)
        {
            using (StreamWriter w = File.AppendText("log.txt"))
            {
                w.WriteLine(((PSDataCollection<ErrorRecord>)sender)[e.Index].ToString());
            }
        }

        private string LoadScript(string filename)
        {
            try
            {
                using (StreamReader sr = new StreamReader(filename))
                {
                    StringBuilder fileContents = new StringBuilder();

                    string curLine;

                    while ((curLine = sr.ReadLine()) != null)
                    {
                        fileContents.Append(curLine + "\n");
                    }

                    return fileContents.ToString();
                }
            }
            catch (Exception e)
            {
                string errorText = "The file could not be read:";
                errorText += e.Message + "\n";
                return errorText;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            CredentialForm credentialForm = new CredentialForm();
            credentialForm.Show();
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "Copying files to Live Website";
            DisableSourceControlButtons(false);
            var result = await RunScript("CopyToLiveWebsite.ps1");
            DisableSourceControlButtons(true);
            toolStripStatusLabel1.Text = "";
            MessageBox.Show(result ? "Copying Finished with errors" : "Copying Finished");
        }

	    private void DisableSourceControlButtons(bool b)
	    {
	        commitButton.Enabled = b;
	        checkoutButton.Enabled = b;
	        CredentialButton.Enabled = b;
	        LiveButton.Enabled = b;
	    }
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
				var count = FileHandler.CountOfMissingStrings(Name);
				return count != 0 ? String.Format("{0} ({1} strings missing)", Name, count) : Name;
			}

		}
	}

	public class KeysListItem : ListItemNotificationObject
	{
		private string _text;
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
				Comment = string.Empty;
				IsFlagged = false;
				_text = Name;
				if (System.IO.File.Exists(Settings.FlagsFilePath))
				{
					var flagDoc = XDocument.Load(Settings.FlagsFilePath);
					var flag = FileHandler.GetFlag(flagDoc, File, Name);
					if (flag != null)
					{
						var commentAttr = flag.Attribute("Comment");
						if (commentAttr != null) Comment = commentAttr.Value;
						IsFlagged = true;
						_text = String.Format("{0} (flagged)", Name);
					}
				}
				return _text;
			}
		}
		public string Comment { get; set; }
		public bool IsFlagged { get; set; }
	}
	#endregion
}
