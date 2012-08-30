using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using LogViewer.C1LogService;

namespace LogViewer
{
	public partial class mainForm : Form
	{
		private static readonly string DateFormatString = "yyyy MM dd";
		private static readonly string TimeFormatString = "HH:mm:ss.ff";
		private static readonly string DateTimeFormat = "yyyy MM dd, HH:mm:ss.ff";

		private static readonly string VerbosePriority = "Verbose";
		private static readonly string InformationPriority = "Information";
		private static readonly string WarningPriority = "Warning";
		private static readonly string ErrorPriority = "Error";
		private static readonly string CriticalPriority = "Critical";

		private string _title = "C1 Log Viewer v1.0";

        private AboutBox _aboutBox = new AboutBox();
		private ConnectionForm _connectionForm = new ConnectionForm();
		private C1Connection _connection;

		private static readonly int MaximimLogLinesCount = 5000;


		private readonly object _syncRoot = new object();

		private bool _downloadVerboseLogEntries = true;
		private bool _isBlinking = false;

		// Auto refreshing
		private DateTime _lastRefreshingDate = DateTime.MinValue;
		private Thread _autoRefreshingThread;
		private List<LogEntry> _loadedEntries;
		private Exception _autoRefreshingException;

		// Paging
		private DateTime _viewedDate;
		private int _pagesCount;
		private int _currentPage;
		private List<DateTime> _pageTimeSeparators;


		public class LogEntryGridRow
		{
			public string Date { get; set; }
			public string Time { get; set; }
			public string AppDomainId { get; set; }
			public string ThreadId { get; set; }
			public string Severity { get; set; }
			public string Title { get; set; }
			public string Message { get; set; }
		}

		private DateTime _currentlyShowFrom;
		private DateTime _currentlyShowTo = DateTime.MinValue;

		private int _entriesShown = 0;
		List<LogEntry> _currentLogEntriesSet;


		public mainForm()
		{
			InitializeComponent();
		}

		private void btnStart_Click(object sender, EventArgs e)
		{
			DisablePaging();

			if (_connection == null)
			{
				MessageBox.Show("Not connected");
				return;
			}

			// Checking whether we were showing entries before
			if (_lastRefreshingDate == DateTime.MinValue)
			{
				LogEntry[] entries;
				DateTime serverTime;
				try
				{
					using (var client = _connection.Connect())
					{
						DateTime lastStartUpTime = client.Channel.GetLastStartupTime();
						serverTime = client.Channel.GetServerTime();

						_currentlyShowFrom = lastStartUpTime;
						_currentlyShowTo = DateTime.MaxValue;

						entries = client.Channel.GetLogEntries(lastStartUpTime, serverTime, true, MaximimLogLinesCount);
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show("Error message: " + ex.Message, "Failed to get data");
					return;
				}

				if (entries != null)
				{
					PopulateGrid(entries);

					// Auto scrolling
					if (cbxAutoScroll.Checked)
					{
						int rowsPerPage = tblLogEntries.DisplayedRowCount(false);
						int lastRow = tblLogEntries.Rows.Count - 1;

						if (lastRow > rowsPerPage && tblLogEntries.FirstDisplayedScrollingRowIndex < lastRow - rowsPerPage + 1)
						{
							tblLogEntries.FirstDisplayedScrollingRowIndex = lastRow - rowsPerPage + 1;
						}
					}
				}

				_lastRefreshingDate = serverTime;
			}

			refreshTimer.Enabled = true;
			btnStart.Enabled = false;
			btnPause.Enabled = true;
			btnClear.Enabled = true;

			_loadedEntries = null;
			_autoRefreshingThread = new Thread(() => AutoRefreshThreadStart(_connection, chkIgnoreTimeouts.Checked, cbxStayConnected.Checked));
			_autoRefreshingThread.Start();
		}

		private void PopulateGrid(LogEntry[] entries)
		{
			_currentLogEntriesSet = new List<LogEntry>(entries);

			RefreshGrid();
		}

		private void RefreshGrid()
		{
			var entries = _currentLogEntriesSet;

			if (entries == null)
			{
				return;
			}

			_entriesShown = 0;

			tblLogEntries.Rows.Clear();

			foreach (var entry in entries)
			{
				if (ShouldBeFiltered(entry)) continue;

				AddToGrid(entry);

				_entriesShown++;
			}

			UpdateShownRecordsLabel();

			lblShownFrom.Text = _currentlyShowFrom == DateTime.MinValue
				? "-" : _currentlyShowFrom.ToString(DateTimeFormat);

			string shownTo;
			bool maximumRecordsCountReached = _currentLogEntriesSet.Count == MaximimLogLinesCount;
			if (maximumRecordsCountReached)
			{
				shownTo = _currentLogEntriesSet[MaximimLogLinesCount - 1].TimeStamp.ToString(DateTimeFormat);
			}
			else
			{
				shownTo = _currentlyShowTo == DateTime.MaxValue ? "-" : _currentlyShowTo.ToString(DateTimeFormat);
			}

			lblShownTo.Text = shownTo;
		}

		private void UpdateShownRecordsLabel()
		{
			lblRecordsCount.Text = string.Format("{0} ({1})", _entriesShown, _currentLogEntriesSet.Count);
		}

		private void button1_Click(object sender, EventArgs e)
		{
			var entry1 = new LogEntry
							 {
								 Severity = "Information",
								 TimeStamp = DateTime.Now,
								 DisplayOptions = "RGB(250,50,50)",
								 Message = "uiosdfkljsdfl; sdl;fksd;f,",
								 Title = "Hello world!!!"
							 };

			var entry2 = new LogEntry
							 {
								 Severity = "Critical",
								 Message = "[iosdfkljsdfl; ",
								 DisplayOptions = "RGB(50,250,150)",
								 TimeStamp = DateTime.Now.AddDays(-1).AddSeconds(2.5),
								 Title = "ERsdfkljsdlfj asiofje!!!"
							 };

			var entry3 = new LogEntry
							 {
								 Severity = "Verbose",
								 Message = "sdfklsdfsdfjsdfl; ",
								 DisplayOptions = "RGB(50,150,150)",
								 TimeStamp = DateTime.Now.AddDays(-1).AddSeconds(2.7),
								 Title = "ERsdfkljssdfsdfdlfj asiofje!!!"
							 };

			var entry4 = new LogEntry
			{
				Severity = "Warning",
				Message = "bvnvsdfkljsdfl; ",
				DisplayOptions = "RGB(170,20,10)",
				TimeStamp = DateTime.Now.AddDays(-1).AddSeconds(2.9),
				Title = "ERsdfssdfkljsdlfj asiot565fje!!!"
			};

			PopulateGrid(new[] { entry1, entry2, entry3, entry4, entry2, entry1, entry3 });
		}

		private void mainForm_Load(object sender, EventArgs e)
		{
			tblLogEntries.AutoGenerateColumns = false;

			lstMode.SelectedIndex = 0;

			_title = Text;
			Text = _title + " - Not connected";


			ShowConnectionDialog();
		}

		private void tblLogEntries_SizeChanged(object sender, EventArgs e)
		{
			// Resizing the last column
			int totalWidh = tblLogEntries.Size.Width - 3;
			for (int i = 0; i < tblLogEntries.Columns.Count - 1; i++)
			{
				totalWidh -= tblLogEntries.Columns[i].Width;
			}

			if (totalWidh > 3500)
			{
				tblLogEntries.Columns[tblLogEntries.Columns.Count - 1].Width = totalWidh;
			}
		}

		private void chkHideVerbose_CheckedChanged(object sender, EventArgs e)
		{
			RefreshGrid();
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void connectToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ShowConnectionDialog();
		}

		private void ShowConnectionDialog()
		{
			var dialogResult = _connectionForm.ShowDialog();

			if (dialogResult != DialogResult.OK)
			{
				return;
			}

			_connection = ConnectionForm.CurrentConnection;

			string connectionTitle = _connection.Title;
			this.Text = (connectionTitle == string.Empty ? string.Empty : (connectionTitle + " - ")) + _title;

			_currentLogEntriesSet = new List<LogEntry>();
			RefreshGrid();

			btnStart.Enabled = true;
			btnShowByDate.Enabled = true;

			lstDates.Enabled = true;
			DateTime[] logginDates;
			using (var client = _connection.Connect())
			{
				logginDates = client.Channel.GetLoggingDates();
			}

			lstDates.Items.Clear();

			lstDates.Items.Add("Today");
			foreach (DateTime date in logginDates)
			{
				lstDates.Items.Add(date.ToString(DateFormatString));
			}

			if (lstDates.Items.Count > 0)
			{
				lstDates.SelectedIndex = 0;
			}
		}

		private void AddToGrid(LogEntry entry)
		{
			Color? displayOptions = ParseDisplayOptions(entry);

			string[] messageLines = entry.Message.Split(new[] { '\n' });
			string firstLine = messageLines.Length > 0 ? messageLines[0] : string.Empty;

			AddToGrid(new LogEntryGridRow
				 {
					 Date = entry.TimeStamp.ToString(DateFormatString),
					 Time = entry.TimeStamp.ToString(TimeFormatString),
					 AppDomainId = entry.ApplicationDomainId.ToString(),
					 ThreadId = entry.ThreadId.ToString(),
					 Severity = entry.Severity,
					 Title = entry.Title,
					 Message = firstLine
				 }, displayOptions);

			for (int i = 1; i < messageLines.Length; i++)
			{
				AddToGrid(new LogEntryGridRow
								 {
									 Message = messageLines[i]
								 }, displayOptions);
			}

		}

		private void AddToGrid(LogEntryGridRow gridRow, Color? color)
		{
			tblLogEntries.Rows.Add(new[]
                                       {
                                           gridRow.Date,
                                           gridRow.Time,
                                           gridRow.AppDomainId,
                                           gridRow.ThreadId,
                                           gridRow.Severity,
                                           gridRow.Title,
                                           gridRow.Message
                                       });
			if (color != null)
			{
				tblLogEntries.Rows[tblLogEntries.Rows.Count - 1].DefaultCellStyle.ForeColor = (Color)color;
			}
		}

		private Color? ParseDisplayOptions(LogEntry entry)
		{
			string displayOptions = entry.DisplayOptions;

			if (displayOptions != null
				&& displayOptions.Length > 8
				&& displayOptions.StartsWith("RGB(")
				&& displayOptions.EndsWith(")"))
			{
				string[] colors = displayOptions.Substring(4, displayOptions.Length - 5).Split(new[] { ',' });

				byte R, G, B;
				if (colors.Length == 3
					&& byte.TryParse(colors[0], out R)
					&& byte.TryParse(colors[1], out G)
					&& byte.TryParse(colors[2], out B))
				{
					return Color.FromArgb(R, G, B);
				}
			}

			if (entry.Severity == InformationPriority)
			{
				return Color.LightGreen;
			}

			if (entry.Severity == WarningPriority)
			{
				return Color.Yellow;
			}

			if (entry.Severity == CriticalPriority || entry.Severity == ErrorPriority)
			{
				return Color.Red;
			}
			return null;
		}


		private void StopFeedingThread(bool pause)
		{
			if (_autoRefreshingThread != null)
			{
				try
				{
					_autoRefreshingThread.Abort();
				}
				catch
				{
				}

				_autoRefreshingThread = null;
			}
			_autoRefreshingException = null;

			btnStart.Enabled = _connection != null;
			btnPause.Enabled = false;
			btnClear.Enabled = _connection != null;

			refreshTimer.Enabled = false;

			if (!pause)
			{
				_lastRefreshingDate = DateTime.MinValue;
			}
		}

		private void btnPause_Click(object sender, EventArgs e)
		{
			StopFeedingThread(true);
		}

		private void refreshTimer_Tick(object sender, EventArgs e)
		{
			lock (_syncRoot)
			{
				LogEntry[] newEntries;

				if (_loadedEntries != null && _loadedEntries.Count > 0)
				{
					newEntries = _loadedEntries.ToArray();
					_loadedEntries.Clear();


					_currentLogEntriesSet.AddRange(newEntries);

					bool flashOnCritical = cbxFlashOnCritical.Checked;
					bool flashOnRestart = cbxFlashOnRestart.Checked;

					bool doFlashing = false;

					foreach (LogEntry entry in newEntries)
					{
						if (!ShouldBeFiltered(entry))
						{
							_entriesShown++;

							AddToGrid(entry);
						}

						if (!doFlashing &&
							((flashOnCritical && (entry.Severity == CriticalPriority))
							 || (flashOnRestart && entry.Message.StartsWith("--- Web Application Start"))))
						{
							doFlashing = true;
						}
					}

					if (doFlashing) Flashing(true);

					// Auto scrolling
					if (cbxAutoScroll.Checked)
					{
						int rowsPerPage = tblLogEntries.DisplayedRowCount(false);
						int lastRow = tblLogEntries.Rows.Count - 1;

						if (lastRow > rowsPerPage && tblLogEntries.FirstDisplayedScrollingRowIndex < lastRow - rowsPerPage + 1)
						{
							tblLogEntries.FirstDisplayedScrollingRowIndex = lastRow - rowsPerPage + 1;
						}
					}

					UpdateShownRecordsLabel();
				}

				if (_autoRefreshingException != null)
				{
					string exceptionText = _autoRefreshingException.Message;
					StopFeedingThread(true);

					MessageBox.Show("Error while fetching data from server, auto refreshing is turned-off.\nException text: " + exceptionText);
				}
			}
		}

		private bool ShouldBeFiltered(LogEntry entry)
		{
			IEnumerable<string> searchTexts = tb_Search.Text.Split(',').Select(f => f.Trim()).Where(f => f != string.Empty);

			bool containingSearchText = false;
			foreach (string searchText in searchTexts)
			{
				if (entry.Title.IndexOf(searchText, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
					entry.Message.IndexOf(searchText, StringComparison.InvariantCultureIgnoreCase) >= 0)
				{
					containingSearchText = true;
				}
			}

			if (searchTexts.Count() > 0 && !containingSearchText) return true;

			return (!chkShowVerbose.Checked && entry.Severity == VerbosePriority)
				   || (!chkShowInfo.Checked && entry.Severity == InformationPriority)
				   || (!chkShowWarnings.Checked && entry.Severity == WarningPriority)
				   || (!chkShowCritical.Checked && entry.Severity == CriticalPriority);
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (openFileDialog.ShowDialog() != DialogResult.OK)
			{
				return;
			}

			StopFeedingThread(false);
			btnStart.Enabled = false;
			btnPause.Enabled = false;
			btnClear.Enabled = false;
			btnShowByDate.Enabled = false;

			DisablePaging();

			PopulateGrid(LogFile.Load(openFileDialog.FileName));
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LogEntry[] toSave = null;

			lock (_syncRoot)
			{
				if (_currentLogEntriesSet != null && _currentLogEntriesSet.Count > 0)
				{
					toSave = new LogEntry[_currentLogEntriesSet.Count];
					_currentLogEntriesSet.CopyTo(toSave);
				}
			}

			if (toSave == null)
			{
				MessageBox.Show("No log entries has been loaded");
				return;
			}

			if (saveFileDialog1.ShowDialog() == DialogResult.OK)
			{
				LogFile.Save(saveFileDialog1.FileName, toSave);
			}
		}

		private void DisablePaging()
		{
			btnNextPage.Enabled = false;
			btnPreviousPage.Enabled = false;
			lblCurrentPage.Text = "0/0";
		}

		private void EnablePaging(DateTime dateTime)
		{
			int recordsTotal, pagesCount;
			using (var client = _connection.Connect())
			{
				recordsTotal = client.Channel.GetLogEntriesCountByDate(dateTime, _downloadVerboseLogEntries);
				pagesCount = (recordsTotal + MaximimLogLinesCount - 1) / MaximimLogLinesCount;
			}
			_viewedDate = dateTime;
			_pageTimeSeparators = new List<DateTime>();
			_pagesCount = pagesCount;

			ShowRecordsFromPage(1);
		}

		private void ShowRecordsFromPage(int pageNumber)
		{
			DateTime from;
			if (pageNumber == 1)
			{
				from = _viewedDate;
			}
			else
			{
				if (_pageTimeSeparators.Count < pageNumber)
				{
					from = _currentLogEntriesSet[_currentLogEntriesSet.Count - 1].TimeStamp.AddTicks(1);
					_pageTimeSeparators.Add(from);
				}
				else
				{
					from = _pageTimeSeparators[pageNumber - 2];
				}
			}

			LogEntry[] entries;
			using (var client = _connection.Connect())
			{
				entries = client.Channel.GetLogEntries(from, _viewedDate.AddDays(1.0), _downloadVerboseLogEntries, MaximimLogLinesCount);
			}

			_currentlyShowFrom = entries.Length > 0 ? entries[0].TimeStamp : DateTime.MinValue;
			_currentlyShowTo = entries.Length > 0 ? entries[entries.Length - 1].TimeStamp : DateTime.MaxValue;

			PopulateGrid(entries);

			_currentPage = pageNumber;
			btnPreviousPage.Enabled = pageNumber > 1;
			btnNextPage.Enabled = pageNumber < _pagesCount;

			lblCurrentPage.Text = pageNumber + "/" + _pagesCount;
		}

		private void btnPreviousPage_Click(object sender, EventArgs e)
		{
			ShowRecordsFromPage(_currentPage - 1);
		}

		private void btnShowByDate_Click(object sender, EventArgs e)
		{
			StopFeedingThread(false);
			btnClear.Enabled = false;

			string dateStr = lstDates.SelectedItem as string;
			if (string.IsNullOrEmpty(dateStr))
			{
				return;
			}

			DateTime date;

			if (dateStr == "Today")
			{
				try
				{
					using (var client = _connection.Connect())
					{
						date = client.Channel.GetServerTime().Date;
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show("Failed to connect to server. Exception text: " + ex.Message);
					return;
				}
			}
			else
			{
				date = DateTime.ParseExact(dateStr, DateFormatString, CultureInfo.InvariantCulture.DateTimeFormat);
			}


			EnablePaging(date);
		}

		private void btnNextPage_Click(object sender, EventArgs e)
		{
			ShowRecordsFromPage(_currentPage + 1);
		}

		private void UpdateToolbarButtons()
		{
			bool isLiveFeed = lstMode.SelectedIndex == 0;
			btnStart.Visible = isLiveFeed;
			btnPause.Visible = isLiveFeed;
			btnClear.Visible = isLiveFeed;

			lblShowByDate.Visible = !isLiveFeed;
			lstDates.Visible = !isLiveFeed;
			btnShowByDate.Visible = !isLiveFeed;
		}

		private void lstMode_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateToolbarButtons();
		}

		private void AutoRefreshThreadStart(C1Connection connection, bool ignoreTimeouts, bool stayAlwaysConnected)
		{
			while (true) // Supposed to be stopped by ThreadAbortException
			{
				Thread.Sleep(600);

				LogEntry[] newEntries;

				try
				{
					DateTime serverTime;

					using (var client = connection.Connect())
					{
						serverTime = client.Channel.GetServerTime();
						newEntries = client.Channel.GetLogEntries(_lastRefreshingDate.AddTicks(1), serverTime, true,
																  MaximimLogLinesCount);
					}

					if (newEntries.Length > 0)
					{
						_lastRefreshingDate = serverTime;
					}
				}
				catch (Exception ex)
				{
					if (stayAlwaysConnected || (ignoreTimeouts && ex is TimeoutException))
					{
						Thread.Sleep(5000);
						continue;
					}

					_autoRefreshingException = ex;
					return;
				}

				lock (_syncRoot)
				{
					if (_loadedEntries == null)
					{
						_loadedEntries = new List<LogEntry>();
					}
					_loadedEntries.AddRange(newEntries);
				}
			}
		}

		protected void Flashing(bool enable)
		{
			var info = new Win32.FLASHWINFO();

			info.cbSize = Convert.ToUInt32(Marshal.SizeOf(info));
			info.hwnd = this.Handle;
			info.dwFlags = enable ? Win32.FLASHW_TRAY : Win32.FLASHW_STOP;
			info.uCount = UInt32.MaxValue;
			info.dwTimeout = 0;

			Win32.FlashWindowEx(ref info);

			_isBlinking = enable;
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);

			StopFeedingThread(false);
		}

		private void mainForm_Activated(object sender, EventArgs e)
		{
			Flashing(false);
		}

		private void mainForm_Enter(object sender, EventArgs e)
		{
			Flashing(false);
		}

		private void chkIgnoreTimeouts_CheckedChanged(object sender, EventArgs e)
		{

		}

		private void cbxAutoScroll_CheckedChanged(object sender, EventArgs e)
		{

		}

		private void btnClear_Click(object sender, EventArgs e)
		{
			_currentLogEntriesSet = new List<LogEntry>();

			RefreshGrid();
		}

		private void lstDates_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (_connection == null || lstMode.SelectedIndex == 0) return;

			btnShowByDate_Click(null, null);
		}

		private void cbxStayConnected_CheckedChanged(object sender, EventArgs e)
		{
			chkIgnoreTimeouts.Enabled = !cbxStayConnected.Checked;
		}

		private void tblLogEntries_Click(object sender, EventArgs e)
		{
			if (_isBlinking)
			{
				Flashing(false);
			}
		}

		private void tblLogEntries_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			if (_isBlinking)
			{
				Flashing(false);
			}
		}

		private void tb_Search_TextChanged(object sender, EventArgs e)
		{
			RefreshGrid();
		}

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _aboutBox.ShowDialog();
        }
	}
}