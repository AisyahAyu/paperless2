// menggunakan System, System.Drawing, System.IO, System.Windows.Forms
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Dashboard
{
    public partial class Form7 : Form
    {
        private int ownFilesCount = 1;
        private int userFilesCount = 0;
        private bool showingOwnFiles = true;

        public Form7()
        {
            InitializeComponent();
            this.Load += Form7_Load;
        }

        private void Form7_Load(object sender, EventArgs e)
        {
            timer1.Interval = 1000;
            timer1.Tick += Timer1_Tick;
            timer1.Start();

            UpdateDateTime();
            InitializeFileContainer();
            SetupNavigationButtons();
            ShowOwnFiles();
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            UpdateDateTime();
        }

        private void UpdateDateTime()
        {
            label1.Text = DateTime.Now.ToString("HH:mm");
            label2.Text = DateTime.Now.ToString("dd MMMM yyyy");
            label3.Text = DateTime.Now.ToString("dddd");
        }

        private void InitializeFileContainer()
        {
            panel2.Controls.Clear();

            Panel tabsPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                BackColor = Color.White
            };

            Button ownButton = CreateTabButton("Own", ownFilesCount);
            Button userButton = CreateTabButton("User", userFilesCount);

            ownButton.Click += (s, e) =>
            {
                ActivateTabButton(ownButton);
                DeactivateTabButton(userButton);
                ShowOwnFiles();
            };

            userButton.Click += (s, e) =>
            {
                ActivateTabButton(userButton);
                DeactivateTabButton(ownButton);
                ShowUserFiles();
            };

            tabsPanel.Controls.Add(ownButton);
            tabsPanel.Controls.Add(userButton);

            ActivateTabButton(ownButton);

            Panel filesContainer = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.WhiteSmoke,
                Padding = new Padding(10)
            };

            // Gunakan TableLayoutPanel agar layout status bar rapi
            TableLayoutPanel statusBar = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 40,
                BackColor = Color.White,
                ColumnCount = 3,
                AutoSize = false,
                Padding = new Padding(10),
                ColumnStyles = {
                    new ColumnStyle(SizeType.Percent, 60),
                    new ColumnStyle(SizeType.Absolute, 140),
                    new ColumnStyle(SizeType.Absolute, 110)
                }
            };

            Label statusLabel = new Label
            {
                Text = "Upload completed: Paperless v5.0.0 Client Operation Instructions.docx",
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                TextAlign = ContentAlignment.MiddleLeft
            };

            ProgressBar progressBar = new ProgressBar
            {
                Value = 100,
                Height = 20,
                Anchor = AnchorStyles.Left,
                Width = 120
            };

            Button importButton = new Button
            {
                Text = "Import",
                BackColor = Color.FromArgb(59, 130, 246),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Width = 100,
                Height = 30,
                Anchor = AnchorStyles.Right
            };
            importButton.FlatAppearance.BorderSize = 0;
            importButton.Click += ImportFile_Click;

            statusBar.Controls.Add(statusLabel, 0, 0);
            statusBar.Controls.Add(progressBar, 1, 0);
            statusBar.Controls.Add(importButton, 2, 0);

            panel2.Controls.Add(filesContainer);
            panel2.Controls.Add(statusBar);
            panel2.Controls.Add(tabsPanel);
            panel2.Tag = new object[] { filesContainer, statusLabel };
        }

        private Button CreateTabButton(string text, int count)
        {
            Button button = new Button
            {
                Text = text,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                Size = new Size(100, 40),
                TextAlign = ContentAlignment.MiddleCenter,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 9)
            };
            button.FlatAppearance.BorderSize = 0;

            Label countLabel = new Label
            {
                Text = count.ToString(),
                Size = new Size(20, 20),
                Location = new Point(70, 10),
                BackColor = Color.FromArgb(59, 130, 246),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 8, FontStyle.Bold)
            };

            button.Controls.Add(countLabel);
            return button;
        }

        private void ActivateTabButton(Button button)
        {
            button.BackColor = Color.White;
            button.ForeColor = Color.FromArgb(59, 130, 246);
            button.Font = new Font(button.Font, FontStyle.Bold);
        }

        private void DeactivateTabButton(Button button)
        {
            button.BackColor = Color.Transparent;
            button.ForeColor = Color.Black;
            button.Font = new Font(button.Font, FontStyle.Regular);
        }

        private void ShowOwnFiles()
        {
            showingOwnFiles = true;
            var filesContainer = ((object[])panel2.Tag)[0] as Panel;
            filesContainer.Controls.Clear();
            AddFileItem(filesContainer, "Paperless v5.0.0 Client Operation Instructions.docx");
        }

        private void ShowUserFiles()
        {
            showingOwnFiles = false;
            var filesContainer = ((object[])panel2.Tag)[0] as Panel;
            filesContainer.Controls.Clear();
            AddMeetingItem(filesContainer, "Test Meeting");
        }

        private void AddFileItem(Panel container, string fileName)
        {
            Panel filePanel = new Panel
            {
                Width = container.Width - 40,
                Height = 50,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(0, 0, 0, 10),
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };

            PictureBox icon = new PictureBox
            {
                Size = new Size(24, 24),
                Location = new Point(10, 13),
                BackColor = Color.FromArgb(59, 130, 246)
            };

            Label nameLabel = new Label
            {
                Text = fileName,
                AutoSize = false,
                Size = new Size(filePanel.Width - 180, 30),
                Location = new Point(44, 15),
                Font = new Font("Segoe UI", 9)
            };

            Button downloadButton = new Button
            {
                Size = new Size(30, 30),
                Location = new Point(filePanel.Width - 80, 10),
                FlatStyle = FlatStyle.Flat,
                Text = "↓",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(59, 130, 246),
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            downloadButton.FlatAppearance.BorderSize = 0;

            Button deleteButton = new Button
            {
                Size = new Size(30, 30),
                Location = new Point(filePanel.Width - 40, 10),
                FlatStyle = FlatStyle.Flat,
                Text = "×",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 53, 69),
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            deleteButton.FlatAppearance.BorderSize = 0;
            deleteButton.Click += (s, e) =>
            {
                container.Controls.Remove(filePanel);
                if (showingOwnFiles)
                {
                    ownFilesCount--;
                    UpdateTabCount(0, ownFilesCount);
                }
                else
                {
                    userFilesCount--;
                    UpdateTabCount(1, userFilesCount);
                }
            };

            filePanel.Controls.Add(icon);
            filePanel.Controls.Add(nameLabel);
            filePanel.Controls.Add(downloadButton);
            filePanel.Controls.Add(deleteButton);
            container.Controls.Add(filePanel);
        }

        private void AddMeetingItem(Panel container, string meetingName)
        {
            Panel meetingPanel = new Panel
            {
                Width = container.Width - 40,
                Height = 50,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(0, 0, 0, 10),
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };

            PictureBox icon = new PictureBox
            {
                Size = new Size(24, 24),
                Location = new Point(10, 13),
                BackColor = Color.FromArgb(59, 130, 246)
            };

            Label nameLabel = new Label
            {
                Text = meetingName,
                AutoSize = false,
                Size = new Size(meetingPanel.Width - 100, 30),
                Location = new Point(44, 15),
                Font = new Font("Segoe UI", 9)
            };

            meetingPanel.Controls.Add(icon);
            meetingPanel.Controls.Add(nameLabel);
            container.Controls.Add(meetingPanel);
        }

        private void UpdateTabCount(int tabIndex, int count)
        {
            if (panel2.Controls.Count < 3) return;
            Panel tabsPanel = panel2.Controls[2] as Panel;
            if (tabsPanel == null || tabsPanel.Controls.Count <= tabIndex) return;

            Button tabButton = tabsPanel.Controls[tabIndex] as Button;
            foreach (Control c in tabButton.Controls)
            {
                if (c is Label lbl)
                {
                    lbl.Text = count.ToString();
                    break;
                }
            }
        }

        private void ImportFile_Click(object sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "All files (*.*)|*.*",
                Title = "Select a file to import"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = Path.GetFileName(openFileDialog.FileName);
                Label statusLabel = ((object[])panel2.Tag)[1] as Label;
                statusLabel.Text = $"Upload completed: {fileName}";

                Panel filesContainer = ((object[])panel2.Tag)[0] as Panel;
                AddFileItem(filesContainer, fileName);

                if (showingOwnFiles)
                {
                    ownFilesCount++;
                    UpdateTabCount(0, ownFilesCount);
                }
                else
                {
                    userFilesCount++;
                    UpdateTabCount(1, userFilesCount);
                }
            }
        }

        private void SetupNavigationButtons()
        {
            Submit.Click += (s, e) =>
            {
                MessageBox.Show("File import operation completed.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            panel16.Click += (s, e) =>
            {
                InitializeFileContainer();
                ShowOwnFiles();
            };

            panel16.Cursor = Cursors.Hand;
        }
    }
}
