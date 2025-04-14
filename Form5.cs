using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Dashboard
{
    public partial class Form5 : Form
    {
        public Form5()
        {
            InitializeComponent();
            this.Load += Form5_Load;
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

        void Evaluation()
        {
            Panel evaluationPanel = new Panel();
            evaluationPanel.Width = flowLayoutPanel1.Width - 10;
            evaluationPanel.Height = 130;
            evaluationPanel.BackColor = Color.FromArgb(230, 230, 230);
            //rounded corners
            int radius = 30;
            evaluationPanel.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, evaluationPanel.Width, evaluationPanel.Height, radius, radius));

            // Label ①
            Label lblNumber = new Label();
            lblNumber.Text = "①";
            lblNumber.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblNumber.ForeColor = Color.RoyalBlue;
            lblNumber.AutoSize = true;
            lblNumber.Location = new Point(10, 10);
            evaluationPanel.Controls.Add(lblNumber);

            // Label judul
            Label lblTitle = new Label();
            lblTitle.Text = "Please evaluate our service (50points)";
            lblTitle.Font = new Font("Segoe UI", 12);
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(50, 20);
            evaluationPanel.Controls.Add(lblTitle);

            // TextBox nilai (readonly)
            TextBox txtValue = new TextBox();
            txtValue.ReadOnly = true;
            txtValue.Text = "10";
            txtValue.Font = new Font("Segoe UI", 10);
            txtValue.Size = new Size(40, 25);

            // Tombol kiri <
            Button btnLeft = new Button();
            btnLeft.Text = "<";
            btnLeft.Size = new Size(30, 30);
            btnLeft.BackColor = Color.White;
            var pathLeft = new System.Drawing.Drawing2D.GraphicsPath();
            pathLeft.AddEllipse(0, 0, 30, 30);
            btnLeft.Region = new Region(pathLeft);

            // Tombol kanan >
            Button btnRight = new Button();
            btnRight.Text = ">";
            btnRight.Size = new Size(30, 30);
            btnRight.BackColor = Color.White;
            var pathRight = new System.Drawing.Drawing2D.GraphicsPath();
            pathRight.AddEllipse(0, 0, 30, 30);
            btnRight.Region = new Region(pathRight);

            // TrackBar
            TrackBar trackBar = new TrackBar();
            trackBar.Minimum = 0;
            trackBar.Maximum = 50;
            trackBar.Value = 10;
            trackBar.TickFrequency = 10;
            trackBar.Size = new Size(200, 30);

            // Kotak input text (TextBox biasa)
            TextBox txtInput = new TextBox();
            txtInput.Font = new Font("Segoe UI", 10);
            txtInput.Size = new Size(800, 25);

            // Tombol submit bulat
            Button btnSubmit = new Button();
            btnSubmit.Text = "Submit";
            btnSubmit.Size = new Size(50, 50);
            btnSubmit.FlatStyle = FlatStyle.Flat;
            btnSubmit.FlatAppearance.BorderSize = 0;
            btnSubmit.BackColor = Color.RoyalBlue;
            btnSubmit.ForeColor = Color.White;
            var pathSubmit = new System.Drawing.Drawing2D.GraphicsPath();
            pathSubmit.AddEllipse(0, 0, 50, 50);
            btnSubmit.Region = new Region(pathSubmit);

            // Event
            btnLeft.Click += (s, e) =>
            {
                if (trackBar.Value > 0)
                {
                    trackBar.Value--;
                    txtValue.Text = trackBar.Value.ToString();
                }
            };

            btnRight.Click += (s, e) =>
            {
                if (trackBar.Value < 50)
                {
                    trackBar.Value++;
                    txtValue.Text = trackBar.Value.ToString();
                }
            };
            btnSubmit.Click += (s, e) =>
            {
                string connStr = "server=127.0.0.1;user=root;database=paperless_user;password=;";
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    try
                    {
                        conn.Open();
                        string query = "INSERT INTO evaluation (Tittle_Evaluate, Score, Tambahan) VALUES (@title, @score, @note)";
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@title", lblTitle.Text);
                        cmd.Parameters.AddWithValue("@score", trackBar.Value);
                        cmd.Parameters.AddWithValue("@note", txtInput.Text);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Data berhasil disimpan ke database!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error saat menyimpan: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            };

            trackBar.Scroll += (s, e) =>
            {
                txtValue.Text = trackBar.Value.ToString();
            };

            btnSubmit.Click += (s, e) =>
            {
                MessageBox.Show($"Submitted value: {trackBar.Value}, note: {txtInput.Text}");
            };

            // Atur posisi elemen horizontal
            int startX = 20;
            int startY = 60;
            int spacing = 10;

            btnLeft.Location = new Point(startX, startY);
            txtValue.Location = new Point(btnLeft.Right + spacing, startY + 3);
            btnRight.Location = new Point(txtValue.Right + spacing, startY);
            trackBar.Location = new Point(btnRight.Right + spacing, startY);
            txtInput.Location = new Point(trackBar.Right + spacing, startY + 5);
            btnSubmit.Location = new Point(txtInput.Right + spacing + 25, startY - 10);

            evaluationPanel.Controls.Add(btnLeft);
            evaluationPanel.Controls.Add(txtValue);
            evaluationPanel.Controls.Add(btnRight);
            evaluationPanel.Controls.Add(trackBar);
            evaluationPanel.Controls.Add(txtInput);
            evaluationPanel.Controls.Add(btnSubmit);

            flowLayoutPanel1.Controls.Add(evaluationPanel);
        }

        private void Submit_Click(object sender, EventArgs e)
        {
            // This method is empty in the original code
        }
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse
        );
        private void Form5_Load(object sender, EventArgs e)
        {
            Evaluation();
            int radius = 30;
            panel1.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, panel1.Width, panel1.Height, radius, radius));
            panel2.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, panel2.Width, panel2.Height, radius, radius));
            panel4.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, panel4.Width, panel4.Height, radius, radius));
            panel5.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, panel5.Width, panel5.Height, radius, radius));
            pictureBox5.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, pictureBox5.Width, pictureBox5.Height, radius, radius));
            pictureBox3.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, pictureBox3.Width, pictureBox3.Height, radius, radius));
            pictureBox4.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, pictureBox4.Width, pictureBox4.Height, radius, radius));
            pictureBox2.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, pictureBox2.Width, pictureBox2.Height, radius, radius));

            // Set up and start the timer
            timer1.Tick += Timer1_Tick;
            timer1.Interval = 1000; // 1 detik
            timer1.Start();
        }
        private void Timer1_Tick(object sender, EventArgs e)
        {
            DateTime waktu = DateTime.Now;
            label1.Text = waktu.ToString("HH:mm");
            label2.Text = waktu.ToString("dd/MM/yyyy");
            label3.Text = waktu.ToString("dddd");
        }


        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {
            // Removed the Evaluation() call here to prevent creating duplicate controls
            // The original code had this which would cause multiple evaluation panels to be created
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            
            Application.Exit();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }
    }
}