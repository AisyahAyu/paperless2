using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Dashboard
{
    public partial class Form4 : Form
    {
        // Flag untuk memastikan panel hanya dibuat sekali
        private bool isPanelCreated = false;
        private Button selectedButton = null;
        private TextBox txtRemarks = null;

        public Form4()
        {
            InitializeComponent();
            this.Load += Form4_Load;
        }

        private MySqlConnection OpenConnection()
        {
            string mysqlCon = "Server=127.0.0.1; user=root; database=paperless_user; Password=";
            MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon);

            try
            {
                mySqlConnection.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            return mySqlConnection;
        }

        void CreateEvaluationPanel()
        {
            // Main panel with light gray background
            Panel evaluationPanel = new Panel();
            evaluationPanel.Width = flowLayoutPanel1.Width - 10; // Adjust width to fit flow panel
            evaluationPanel.Height = 100;
            evaluationPanel.BackColor = Color.FromArgb(230, 230, 230); // Lighter gray
            evaluationPanel.Margin = new Padding(0, 0, 0, 0);

            // Blue circular number
            Label lblNumber = new Label();
            lblNumber.Text = "①";
            lblNumber.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblNumber.ForeColor = Color.RoyalBlue;
            lblNumber.AutoSize = true;
            lblNumber.Location = new Point(15, 20);
            evaluationPanel.Controls.Add(lblNumber);

            // "Please evaluate our service" label
            Label lblTitle = new Label();
            lblTitle.Text = "Please evaluate our service";
            lblTitle.Font = new Font("Segoe UI", 12, FontStyle.Regular);
            lblTitle.ForeColor = Color.Black;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(50, 20);
            evaluationPanel.Controls.Add(lblTitle);

            // Create rounded buttons
            Button btnAgree = CreateRoundedButton("agree", Color.Green, 80, 30);
            btnAgree.Location = new Point(75, 50);
            btnAgree.Click += VoteButton_Click;
            evaluationPanel.Controls.Add(btnAgree);

            Button btnOppose = CreateRoundedButton("oppose", Color.Red, 80, 30);
            btnOppose.Location = new Point(165, 50);
            btnOppose.Click += VoteButton_Click;
            evaluationPanel.Controls.Add(btnOppose);

            Button btnWaiver = CreateRoundedButton("waiver", Color.Orange, 80, 30);
            btnWaiver.Location = new Point(255, 50);
            btnWaiver.Click += VoteButton_Click;
            evaluationPanel.Controls.Add(btnWaiver);

            // Remarks label
            Label lblRemarks = new Label();
            lblRemarks.Text = "Remarks:";
            lblRemarks.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            lblRemarks.ForeColor = Color.Black;
            lblRemarks.AutoSize = true;
            lblRemarks.Location = new Point(345, 55);
            evaluationPanel.Controls.Add(lblRemarks);

            // Remarks text field with rounded corners
            txtRemarks = new TextBox();
            txtRemarks.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            txtRemarks.Size = new Size(400, 25);
            txtRemarks.Location = new Point(450, 52);
            txtRemarks.BorderStyle = BorderStyle.FixedSingle;
            evaluationPanel.Controls.Add(txtRemarks);

            // Add panel to flowLayoutPanel1 instead of directly to the form
            flowLayoutPanel1.Controls.Add(evaluationPanel);
        }

        private void VoteButton_Click(object sender, EventArgs e)
        {
            // Reset the background color and text color of previously selected button
            if (selectedButton != null)
            {
                selectedButton.BackColor = Color.White;
                selectedButton.ForeColor = selectedButton.FlatAppearance.BorderColor;
            }

            // Set the current button as selected
            selectedButton = (Button)sender;
            selectedButton.BackColor = selectedButton.FlatAppearance.BorderColor;
            selectedButton.ForeColor = Color.White;
        }

        private Button CreateRoundedButton(string text, Color color, int width, int height)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.BorderColor = color;
            btn.BackColor = Color.White;
            btn.ForeColor = color;

            // Menambahkan panjang tombol ke bawah
            int additionalHeight = 10; // Jumlah tambahan panjang ke bawah
            int finalHeight = height + additionalHeight;
            btn.Size = new Size(width, finalHeight);

            // Add rounded corners with GraphicsPath
            GraphicsPath path = new GraphicsPath();
            int radius = 10; // Corner radius

            // Update koordinat untuk mencakup tambahan panjang ke bawah
            path.AddArc(0, 0, radius, radius, 180, 90); // Top-left corner
            path.AddArc(width - radius, 0, radius, radius, 270, 90); // Top-right corner
            path.AddArc(width - radius, finalHeight - radius, radius, radius, 0, 90); // Bottom-right corner
            path.AddArc(0, finalHeight - radius, radius, radius, 90, 90); // Bottom-left corner
            path.CloseAllFigures();

            btn.Region = new Region(path);

            return btn;
        }

        private void Submit_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if a button is selected
                if (selectedButton == null)
                {
                    MessageBox.Show("Please select an option: agree, oppose, or waiver.");
                    return;
                }

                string vote = selectedButton.Text;
                string remarks = txtRemarks != null ? txtRemarks.Text : "";
                string title = "Service Evaluation"; // You can customize this or make it dynamic

                // Open database connection
                using (MySqlConnection connection = OpenConnection())
                {
                    if (connection.State == System.Data.ConnectionState.Open)
                    {
                        // Prepare SQL command with parameters to prevent SQL injection
                        string sql = "INSERT INTO voting (Tittle_Vote, Pilihan, Remarks) VALUES (@title, @vote, @remarks)";
                        using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                        {
                            // Add parameters
                            cmd.Parameters.AddWithValue("@title", title);
                            cmd.Parameters.AddWithValue("@vote", vote);
                            cmd.Parameters.AddWithValue("@remarks", remarks);

                            // Execute the command
                            int result = cmd.ExecuteNonQuery();

                            if (result > 0)
                            {
                                MessageBox.Show("Data has been submitted successfully!");

                                // Clear the flowLayoutPanel
                                flowLayoutPanel1.Controls.Clear();

                                // Reset selection
                                selectedButton = null;
                                txtRemarks = null;

                                // Important: Keep isPanelCreated as true to prevent auto-recreation
                                // This prevents the panel from being recreated by other events
                                isPanelCreated = true;
                            }
                            else
                            {
                                MessageBox.Show("Failed to submit data. Please try again.");
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Database connection failed. Please check your connection.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse
        );

        private void Form4_Load(object sender, EventArgs e)
        {
            // Buat panel evaluasi saat form dimuat
            if (!isPanelCreated)
            {
                CreateEvaluationPanel();
                isPanelCreated = true;
            }

            // Menerapkan efek rounded corners untuk beberapa panel
            int radius = 30;
            panel1.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, panel1.Width, panel1.Height, radius, radius));
            panel2.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, panel2.Width, panel2.Height, radius, radius));
            panel4.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, panel4.Width, panel4.Height, radius, radius));
            panel5.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, panel5.Width, panel5.Height, radius, radius));

            // Set up and start the timer
            timer1.Tick += Timer1_Tick;
            timer1.Interval = 1000; // 1 detik
            timer1.Start();
        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {
            // Pastikan panel hanya dibuat sekali
            if (!isPanelCreated)
            {
                CreateEvaluationPanel();
                isPanelCreated = true;
            }
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            DateTime waktu = DateTime.Now;
            label1.Text = waktu.ToString("HH:mm");
            label2.Text = waktu.ToString("dd/MM/yyyy");
            label3.Text = waktu.ToString("dddd");
        }

        private void pictureBox5_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void pictureBox3_Click_1(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            // Method body empty in original code - kept as is
        }

        private void Form4_Load_1(object sender, EventArgs e)
        {

        }
    }
}