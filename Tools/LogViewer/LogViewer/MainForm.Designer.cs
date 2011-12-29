namespace LogViewer
{
    partial class mainForm
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(mainForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnClear = new System.Windows.Forms.Button();
            this.lblMode = new System.Windows.Forms.Label();
            this.btnShowByDate = new System.Windows.Forms.Button();
            this.lblShowByDate = new System.Windows.Forms.Label();
            this.lstDates = new System.Windows.Forms.ComboBox();
            this.btnPause = new System.Windows.Forms.Button();
            this.lstMode = new System.Windows.Forms.ComboBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gridTest = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tblLogEntries = new System.Windows.Forms.DataGridView();
            this.Date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clAppDomainId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clThreadId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clPriority = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Priority = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clMessage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel3 = new System.Windows.Forms.Panel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cbxStayConnected = new System.Windows.Forms.CheckBox();
            this.cbxAutoScroll = new System.Windows.Forms.CheckBox();
            this.chkIgnoreTimeouts = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cbxFlashOnCritical = new System.Windows.Forms.CheckBox();
            this.cbxFlashOnRestart = new System.Windows.Forms.CheckBox();
            this.lblCurrentPage = new System.Windows.Forms.Label();
            this.lblPage = new System.Windows.Forms.Label();
            this.btnPreviousPage = new System.Windows.Forms.Button();
            this.btnNextPage = new System.Windows.Forms.Button();
            this.lblShownTo = new System.Windows.Forms.Label();
            this.lblShownFrom = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblRecordsCount = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkShowVerbose = new System.Windows.Forms.CheckBox();
            this.chkShowCritical = new System.Windows.Forms.CheckBox();
            this.chkShowInfo = new System.Windows.Forms.CheckBox();
            this.chkShowWarnings = new System.Windows.Forms.CheckBox();
            this.refreshTimer = new System.Windows.Forms.Timer(this.components);
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.tb_Search = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tblLogEntries)).BeginInit();
            this.panel3.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.tb_Search);
            this.panel1.Controls.Add(this.btnClear);
            this.panel1.Controls.Add(this.lblMode);
            this.panel1.Controls.Add(this.btnShowByDate);
            this.panel1.Controls.Add(this.lblShowByDate);
            this.panel1.Controls.Add(this.lstDates);
            this.panel1.Controls.Add(this.btnPause);
            this.panel1.Controls.Add(this.lstMode);
            this.panel1.Controls.Add(this.btnStart);
            this.panel1.Controls.Add(this.menuStrip1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(942, 60);
            this.panel1.TabIndex = 0;
            // 
            // btnClear
            // 
            this.btnClear.Enabled = false;
            this.btnClear.FlatAppearance.BorderSize = 0;
            this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClear.Image = global::LogViewer.Properties.Resources.cleanup_16;
            this.btnClear.Location = new System.Drawing.Point(237, 29);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(30, 28);
            this.btnClear.TabIndex = 12;
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // lblMode
            // 
            this.lblMode.AutoSize = true;
            this.lblMode.Location = new System.Drawing.Point(9, 36);
            this.lblMode.Name = "lblMode";
            this.lblMode.Size = new System.Drawing.Size(34, 13);
            this.lblMode.TabIndex = 11;
            this.lblMode.Text = "Mode";
            // 
            // btnShowByDate
            // 
            this.btnShowByDate.Enabled = false;
            this.btnShowByDate.FlatAppearance.BorderSize = 0;
            this.btnShowByDate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnShowByDate.Image = global::LogViewer.Properties.Resources.ref_16;
            this.btnShowByDate.Location = new System.Drawing.Point(335, 28);
            this.btnShowByDate.Name = "btnShowByDate";
            this.btnShowByDate.Size = new System.Drawing.Size(36, 27);
            this.btnShowByDate.TabIndex = 10;
            this.btnShowByDate.UseVisualStyleBackColor = true;
            this.btnShowByDate.Click += new System.EventHandler(this.btnShowByDate_Click);
            // 
            // lblShowByDate
            // 
            this.lblShowByDate.AutoSize = true;
            this.lblShowByDate.Location = new System.Drawing.Point(177, 35);
            this.lblShowByDate.Name = "lblShowByDate";
            this.lblShowByDate.Size = new System.Drawing.Size(33, 13);
            this.lblShowByDate.TabIndex = 9;
            this.lblShowByDate.Text = "Date:";
            // 
            // lstDates
            // 
            this.lstDates.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lstDates.Enabled = false;
            this.lstDates.FormattingEnabled = true;
            this.lstDates.Location = new System.Drawing.Point(216, 32);
            this.lstDates.Name = "lstDates";
            this.lstDates.Size = new System.Drawing.Size(113, 21);
            this.lstDates.TabIndex = 8;
            this.lstDates.SelectedIndexChanged += new System.EventHandler(this.lstDates_SelectedIndexChanged);
            // 
            // btnPause
            // 
            this.btnPause.Enabled = false;
            this.btnPause.FlatAppearance.BorderSize = 0;
            this.btnPause.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPause.Image = global::LogViewer.Properties.Resources.pause_16;
            this.btnPause.Location = new System.Drawing.Point(204, 29);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(32, 27);
            this.btnPause.TabIndex = 7;
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // lstMode
            // 
            this.lstMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lstMode.FormattingEnabled = true;
            this.lstMode.Items.AddRange(new object[] {
            "Live feed",
            "History"});
            this.lstMode.Location = new System.Drawing.Point(49, 32);
            this.lstMode.Name = "lstMode";
            this.lstMode.Size = new System.Drawing.Size(118, 21);
            this.lstMode.TabIndex = 5;
            this.lstMode.SelectedIndexChanged += new System.EventHandler(this.lstMode_SelectedIndexChanged);
            // 
            // btnStart
            // 
            this.btnStart.Enabled = false;
            this.btnStart.FlatAppearance.BorderSize = 0;
            this.btnStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStart.Image = global::LogViewer.Properties.Resources.play_16;
            this.btnStart.Location = new System.Drawing.Point(170, 29);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(36, 27);
            this.btnStart.TabIndex = 2;
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.connectToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(942, 24);
            this.menuStrip1.TabIndex = 6;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(111, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(111, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(108, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(111, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // connectToolStripMenuItem
            // 
            this.connectToolStripMenuItem.Name = "connectToolStripMenuItem";
            this.connectToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
            this.connectToolStripMenuItem.Text = "Connect";
            this.connectToolStripMenuItem.Click += new System.EventHandler(this.connectToolStripMenuItem_Click);
            // 
            // gridTest
            // 
            this.gridTest.Location = new System.Drawing.Point(349, 16);
            this.gridTest.Name = "gridTest";
            this.gridTest.Size = new System.Drawing.Size(52, 49);
            this.gridTest.TabIndex = 6;
            this.gridTest.Text = "Grid Test";
            this.gridTest.UseVisualStyleBackColor = true;
            this.gridTest.Visible = false;
            this.gridTest.Click += new System.EventHandler(this.button1_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tblLogEntries);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 60);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(942, 320);
            this.panel2.TabIndex = 1;
            // 
            // tblLogEntries
            // 
            this.tblLogEntries.AllowUserToAddRows = false;
            this.tblLogEntries.AllowUserToDeleteRows = false;
            this.tblLogEntries.AllowUserToResizeRows = false;
            this.tblLogEntries.BackgroundColor = System.Drawing.Color.Black;
            this.tblLogEntries.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.tblLogEntries.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Date,
            this.clTime,
            this.clAppDomainId,
            this.clThreadId,
            this.clPriority,
            this.Priority,
            this.clMessage});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Courier New", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(130)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.tblLogEntries.DefaultCellStyle = dataGridViewCellStyle1;
            this.tblLogEntries.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblLogEntries.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.tblLogEntries.Location = new System.Drawing.Point(0, 0);
            this.tblLogEntries.Name = "tblLogEntries";
            this.tblLogEntries.ReadOnly = true;
            this.tblLogEntries.RowHeadersVisible = false;
            this.tblLogEntries.Size = new System.Drawing.Size(942, 320);
            this.tblLogEntries.TabIndex = 1;
            this.tblLogEntries.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.tblLogEntries_CellContentClick);
            this.tblLogEntries.SizeChanged += new System.EventHandler(this.tblLogEntries_SizeChanged);
            this.tblLogEntries.Click += new System.EventHandler(this.tblLogEntries_Click);
            // 
            // Date
            // 
            this.Date.DataPropertyName = "Date";
            this.Date.HeaderText = "Date";
            this.Date.Name = "Date";
            this.Date.ReadOnly = true;
            this.Date.Width = 90;
            // 
            // clTime
            // 
            this.clTime.DataPropertyName = "Time";
            this.clTime.HeaderText = "Time";
            this.clTime.Name = "clTime";
            this.clTime.ReadOnly = true;
            this.clTime.Width = 110;
            // 
            // clAppDomainId
            // 
            this.clAppDomainId.DataPropertyName = "AppDomainId";
            this.clAppDomainId.HeaderText = "AD Id.";
            this.clAppDomainId.Name = "clAppDomainId";
            this.clAppDomainId.ReadOnly = true;
            this.clAppDomainId.ToolTipText = "Application Domain Id";
            this.clAppDomainId.Width = 35;
            // 
            // clThreadId
            // 
            this.clThreadId.DataPropertyName = "ThreadId";
            this.clThreadId.FillWeight = 50F;
            this.clThreadId.HeaderText = "Th. ID";
            this.clThreadId.Name = "clThreadId";
            this.clThreadId.ReadOnly = true;
            this.clThreadId.ToolTipText = "Thread ID";
            this.clThreadId.Width = 35;
            // 
            // clPriority
            // 
            this.clPriority.DataPropertyName = "Severity";
            this.clPriority.HeaderText = "Priority";
            this.clPriority.Name = "clPriority";
            this.clPriority.ReadOnly = true;
            // 
            // Priority
            // 
            this.Priority.DataPropertyName = "Title";
            this.Priority.HeaderText = "Title";
            this.Priority.Name = "Priority";
            this.Priority.ReadOnly = true;
            this.Priority.Width = 250;
            // 
            // clMessage
            // 
            this.clMessage.DataPropertyName = "Message";
            this.clMessage.HeaderText = "Message";
            this.clMessage.Name = "clMessage";
            this.clMessage.ReadOnly = true;
            this.clMessage.Width = 3500;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.groupBox3);
            this.panel3.Controls.Add(this.groupBox2);
            this.panel3.Controls.Add(this.lblCurrentPage);
            this.panel3.Controls.Add(this.lblPage);
            this.panel3.Controls.Add(this.btnPreviousPage);
            this.panel3.Controls.Add(this.btnNextPage);
            this.panel3.Controls.Add(this.lblShownTo);
            this.panel3.Controls.Add(this.lblShownFrom);
            this.panel3.Controls.Add(this.label3);
            this.panel3.Controls.Add(this.label2);
            this.panel3.Controls.Add(this.lblRecordsCount);
            this.panel3.Controls.Add(this.label1);
            this.panel3.Controls.Add(this.groupBox1);
            this.panel3.Controls.Add(this.gridTest);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 380);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(942, 86);
            this.panel3.TabIndex = 2;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cbxStayConnected);
            this.groupBox3.Controls.Add(this.cbxAutoScroll);
            this.groupBox3.Controls.Add(this.chkIgnoreTimeouts);
            this.groupBox3.Location = new System.Drawing.Point(720, 6);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(213, 70);
            this.groupBox3.TabIndex = 27;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Live feed mode";
            // 
            // cbxStayConnected
            // 
            this.cbxStayConnected.AutoSize = true;
            this.cbxStayConnected.Location = new System.Drawing.Point(106, 19);
            this.cbxStayConnected.Name = "cbxStayConnected";
            this.cbxStayConnected.Size = new System.Drawing.Size(101, 17);
            this.cbxStayConnected.TabIndex = 26;
            this.cbxStayConnected.Text = "Stay connected";
            this.cbxStayConnected.UseVisualStyleBackColor = true;
            this.cbxStayConnected.CheckedChanged += new System.EventHandler(this.cbxStayConnected_CheckedChanged);
            // 
            // cbxAutoScroll
            // 
            this.cbxAutoScroll.AutoSize = true;
            this.cbxAutoScroll.Checked = true;
            this.cbxAutoScroll.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbxAutoScroll.Location = new System.Drawing.Point(6, 19);
            this.cbxAutoScroll.Name = "cbxAutoScroll";
            this.cbxAutoScroll.Size = new System.Drawing.Size(72, 17);
            this.cbxAutoScroll.TabIndex = 23;
            this.cbxAutoScroll.Text = "Autoscroll";
            this.cbxAutoScroll.UseVisualStyleBackColor = true;
            this.cbxAutoScroll.CheckedChanged += new System.EventHandler(this.cbxAutoScroll_CheckedChanged);
            // 
            // chkIgnoreTimeouts
            // 
            this.chkIgnoreTimeouts.AutoSize = true;
            this.chkIgnoreTimeouts.Checked = true;
            this.chkIgnoreTimeouts.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkIgnoreTimeouts.Location = new System.Drawing.Point(6, 41);
            this.chkIgnoreTimeouts.Name = "chkIgnoreTimeouts";
            this.chkIgnoreTimeouts.Size = new System.Drawing.Size(98, 17);
            this.chkIgnoreTimeouts.TabIndex = 25;
            this.chkIgnoreTimeouts.Text = "Ignore timeouts";
            this.chkIgnoreTimeouts.UseVisualStyleBackColor = true;
            this.chkIgnoreTimeouts.CheckedChanged += new System.EventHandler(this.chkIgnoreTimeouts_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cbxFlashOnCritical);
            this.groupBox2.Controls.Add(this.cbxFlashOnRestart);
            this.groupBox2.Location = new System.Drawing.Point(599, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(115, 70);
            this.groupBox2.TabIndex = 26;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Notifications";
            // 
            // cbxFlashOnCritical
            // 
            this.cbxFlashOnCritical.AutoSize = true;
            this.cbxFlashOnCritical.Checked = true;
            this.cbxFlashOnCritical.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbxFlashOnCritical.Location = new System.Drawing.Point(9, 19);
            this.cbxFlashOnCritical.Name = "cbxFlashOnCritical";
            this.cbxFlashOnCritical.Size = new System.Drawing.Size(99, 17);
            this.cbxFlashOnCritical.TabIndex = 22;
            this.cbxFlashOnCritical.Text = "Flash on critical";
            this.cbxFlashOnCritical.UseVisualStyleBackColor = true;
            // 
            // cbxFlashOnRestart
            // 
            this.cbxFlashOnRestart.AutoSize = true;
            this.cbxFlashOnRestart.Location = new System.Drawing.Point(9, 40);
            this.cbxFlashOnRestart.Name = "cbxFlashOnRestart";
            this.cbxFlashOnRestart.Size = new System.Drawing.Size(98, 17);
            this.cbxFlashOnRestart.TabIndex = 24;
            this.cbxFlashOnRestart.Text = "Flash on restart";
            this.cbxFlashOnRestart.UseVisualStyleBackColor = true;
            // 
            // lblCurrentPage
            // 
            this.lblCurrentPage.AutoSize = true;
            this.lblCurrentPage.Location = new System.Drawing.Point(57, 16);
            this.lblCurrentPage.Name = "lblCurrentPage";
            this.lblCurrentPage.Size = new System.Drawing.Size(24, 13);
            this.lblCurrentPage.TabIndex = 21;
            this.lblCurrentPage.Text = "0/0";
            // 
            // lblPage
            // 
            this.lblPage.AutoSize = true;
            this.lblPage.Location = new System.Drawing.Point(13, 16);
            this.lblPage.Name = "lblPage";
            this.lblPage.Size = new System.Drawing.Size(38, 13);
            this.lblPage.TabIndex = 20;
            this.lblPage.Text = "Page: ";
            // 
            // btnPreviousPage
            // 
            this.btnPreviousPage.Enabled = false;
            this.btnPreviousPage.Location = new System.Drawing.Point(12, 45);
            this.btnPreviousPage.Name = "btnPreviousPage";
            this.btnPreviousPage.Size = new System.Drawing.Size(56, 27);
            this.btnPreviousPage.TabIndex = 19;
            this.btnPreviousPage.Text = "<< Prev";
            this.btnPreviousPage.UseVisualStyleBackColor = true;
            this.btnPreviousPage.Click += new System.EventHandler(this.btnPreviousPage_Click);
            // 
            // btnNextPage
            // 
            this.btnNextPage.Enabled = false;
            this.btnNextPage.Location = new System.Drawing.Point(74, 45);
            this.btnNextPage.Name = "btnNextPage";
            this.btnNextPage.Size = new System.Drawing.Size(56, 27);
            this.btnNextPage.TabIndex = 18;
            this.btnNextPage.Text = "Next >>";
            this.btnNextPage.UseVisualStyleBackColor = true;
            this.btnNextPage.Click += new System.EventHandler(this.btnNextPage_Click);
            // 
            // lblShownTo
            // 
            this.lblShownTo.AutoSize = true;
            this.lblShownTo.Location = new System.Drawing.Point(278, 52);
            this.lblShownTo.Name = "lblShownTo";
            this.lblShownTo.Size = new System.Drawing.Size(0, 13);
            this.lblShownTo.TabIndex = 17;
            // 
            // lblShownFrom
            // 
            this.lblShownFrom.AutoSize = true;
            this.lblShownFrom.Location = new System.Drawing.Point(278, 34);
            this.lblShownFrom.Name = "lblShownFrom";
            this.lblShownFrom.Size = new System.Drawing.Size(0, 13);
            this.lblShownFrom.TabIndex = 16;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(151, 52);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "Shown to:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(151, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Shown from:";
            // 
            // lblRecordsCount
            // 
            this.lblRecordsCount.AutoSize = true;
            this.lblRecordsCount.Location = new System.Drawing.Point(278, 16);
            this.lblRecordsCount.Name = "lblRecordsCount";
            this.lblRecordsCount.Size = new System.Drawing.Size(28, 13);
            this.lblRecordsCount.TabIndex = 13;
            this.lblRecordsCount.Text = "0 (0)";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(151, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(121, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Records shown/loaded:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkShowVerbose);
            this.groupBox1.Controls.Add(this.chkShowCritical);
            this.groupBox1.Controls.Add(this.chkShowInfo);
            this.groupBox1.Controls.Add(this.chkShowWarnings);
            this.groupBox1.Location = new System.Drawing.Point(420, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(173, 70);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Show";
            // 
            // chkShowVerbose
            // 
            this.chkShowVerbose.AutoSize = true;
            this.chkShowVerbose.Checked = true;
            this.chkShowVerbose.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkShowVerbose.Location = new System.Drawing.Point(15, 19);
            this.chkShowVerbose.Name = "chkShowVerbose";
            this.chkShowVerbose.Size = new System.Drawing.Size(65, 17);
            this.chkShowVerbose.TabIndex = 7;
            this.chkShowVerbose.Text = "Verbose";
            this.chkShowVerbose.UseVisualStyleBackColor = true;
            this.chkShowVerbose.CheckedChanged += new System.EventHandler(this.chkHideVerbose_CheckedChanged);
            // 
            // chkShowCritical
            // 
            this.chkShowCritical.AutoSize = true;
            this.chkShowCritical.Checked = true;
            this.chkShowCritical.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkShowCritical.Location = new System.Drawing.Point(98, 42);
            this.chkShowCritical.Name = "chkShowCritical";
            this.chkShowCritical.Size = new System.Drawing.Size(57, 17);
            this.chkShowCritical.TabIndex = 10;
            this.chkShowCritical.Text = "Critical";
            this.chkShowCritical.UseVisualStyleBackColor = true;
            this.chkShowCritical.CheckedChanged += new System.EventHandler(this.chkHideVerbose_CheckedChanged);
            // 
            // chkShowInfo
            // 
            this.chkShowInfo.AutoSize = true;
            this.chkShowInfo.Checked = true;
            this.chkShowInfo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkShowInfo.Location = new System.Drawing.Point(15, 42);
            this.chkShowInfo.Name = "chkShowInfo";
            this.chkShowInfo.Size = new System.Drawing.Size(78, 17);
            this.chkShowInfo.TabIndex = 8;
            this.chkShowInfo.Text = "Information";
            this.chkShowInfo.UseVisualStyleBackColor = true;
            this.chkShowInfo.CheckedChanged += new System.EventHandler(this.chkHideVerbose_CheckedChanged);
            // 
            // chkShowWarnings
            // 
            this.chkShowWarnings.AutoSize = true;
            this.chkShowWarnings.Checked = true;
            this.chkShowWarnings.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkShowWarnings.Location = new System.Drawing.Point(98, 19);
            this.chkShowWarnings.Name = "chkShowWarnings";
            this.chkShowWarnings.Size = new System.Drawing.Size(71, 17);
            this.chkShowWarnings.TabIndex = 9;
            this.chkShowWarnings.Text = "Warnings";
            this.chkShowWarnings.UseVisualStyleBackColor = true;
            this.chkShowWarnings.CheckedChanged += new System.EventHandler(this.chkHideVerbose_CheckedChanged);
            // 
            // refreshTimer
            // 
            this.refreshTimer.Interval = 1000;
            this.refreshTimer.Tick += new System.EventHandler(this.refreshTimer_Tick);
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "(*.txt)|*.txt|All Files|*.*";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.Filter = "(*.txt)|*.txt|All Files|*.*";
            // 
            // tb_Search
            // 
            this.tb_Search.Location = new System.Drawing.Point(428, 33);
            this.tb_Search.Name = "tb_Search";
            this.tb_Search.Size = new System.Drawing.Size(217, 20);
            this.tb_Search.TabIndex = 13;
            this.tb_Search.TextChanged += new System.EventHandler(this.tb_Search_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(393, 35);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "Filter";
            // 
            // mainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(942, 466);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "mainForm";
            this.Text = "C1 Log Viewer";
            this.Activated += new System.EventHandler(this.mainForm_Activated);
            this.Load += new System.EventHandler(this.mainForm_Load);
            this.Enter += new System.EventHandler(this.mainForm_Enter);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tblLogEntries)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.DataGridView tblLogEntries;
        private System.Windows.Forms.ComboBox lstMode;
        private System.Windows.Forms.Button gridTest;
        private System.Windows.Forms.CheckBox chkShowInfo;
        private System.Windows.Forms.CheckBox chkShowVerbose;
        private System.Windows.Forms.CheckBox chkShowCritical;
        private System.Windows.Forms.CheckBox chkShowWarnings;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblShownTo;
        private System.Windows.Forms.Label lblShownFrom;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblRecordsCount;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectToolStripMenuItem;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.Timer refreshTimer;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Label lblShowByDate;
        private System.Windows.Forms.ComboBox lstDates;
        private System.Windows.Forms.Button btnPreviousPage;
        private System.Windows.Forms.Button btnNextPage;
        private System.Windows.Forms.Button btnShowByDate;
        private System.Windows.Forms.Label lblCurrentPage;
        private System.Windows.Forms.Label lblPage;
        private System.Windows.Forms.Label lblMode;
        private System.Windows.Forms.CheckBox cbxAutoScroll;
        private System.Windows.Forms.CheckBox cbxFlashOnCritical;
        private System.Windows.Forms.CheckBox cbxFlashOnRestart;
        private System.Windows.Forms.DataGridViewTextBoxColumn Date;
        private System.Windows.Forms.DataGridViewTextBoxColumn clTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn clAppDomainId;
        private System.Windows.Forms.DataGridViewTextBoxColumn clThreadId;
        private System.Windows.Forms.DataGridViewTextBoxColumn clPriority;
        private System.Windows.Forms.DataGridViewTextBoxColumn Priority;
        private System.Windows.Forms.DataGridViewTextBoxColumn clMessage;
        private System.Windows.Forms.CheckBox chkIgnoreTimeouts;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.CheckBox cbxStayConnected;
        private System.Windows.Forms.TextBox tb_Search;
        private System.Windows.Forms.Label label4;

    }
}

