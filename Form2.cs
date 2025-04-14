using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Dashboard
{
    public partial class Form2 : Form
    {
        private MySqlConnection koneksi;

        public Form2()
        {
            InitializeComponent();
            this.Load += Form2_Load;

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
        private void FetchData()
        {
            MySqlConnection connection = null;
            MySqlCommand command = null;
            MySqlDataReader reader = null;

            try
            {
                connection = OpenConnection();

                if (connection != null && connection.State == System.Data.ConnectionState.Open)
                {
                    string query = "SELECT * FROM agenda";
                    command = new MySqlCommand(query, connection);
                    reader = command.ExecuteReader();

                    flowLayoutPanel1.Controls.Clear(); // Kosongkan dulu

                    int nomor = 1;

                    while (reader.Read())
                    {
                        string namaAgenda = reader["Name_Agenda"].ToString();
                        string jammulai = reader["Jam_Mulai"].ToString();
                        string jamselesai = reader["Jam_Selesai"].ToString();

                        Panel itemPanel = new Panel();
                        itemPanel.Width = flowLayoutPanel1.Width - 10;
                        itemPanel.Height = 50;
                        itemPanel.BackColor = Color.LightGray;
                        itemPanel.Margin = new Padding(5);
                        itemPanel.Padding = new Padding(10);
                        itemPanel.Anchor = AnchorStyles.Left | AnchorStyles.Right;

                        Label lblNo = new Label();
                        lblNo.Text = $"{nomor}";
                        lblNo.ForeColor = Color.White;
                        lblNo.BackColor = Color.RoyalBlue;
                        lblNo.Font = new Font("Segoe UI", 10, FontStyle.Regular);
                        lblNo.TextAlign = ContentAlignment.MiddleCenter;
                        lblNo.Size = new Size(30, 30); // Ukuran persegi
                        lblNo.Location = new Point(10, 10); // Sesuaikan posisi

                        // Membuat label berbentuk lingkaran
                        GraphicsPath path = new GraphicsPath();
                        path.AddEllipse(0, 0, lblNo.Width, lblNo.Height);
                        lblNo.Region = new Region(path);

                        // Tambahkan ke panel
                        itemPanel.Controls.Add(lblNo);

                        Label lblAgenda = new Label();
                        lblAgenda.Text = $"{namaAgenda}";
                        lblAgenda.ForeColor = Color.Black;
                        lblAgenda.Font = new Font("Segoe UI", 10, FontStyle.Regular);
                        lblAgenda.AutoSize = true;
                        lblAgenda.Location = new Point(50, 10);

                        Label lblJam = new Label();
                        lblJam.Text = $"{jammulai} - {jamselesai}";
                        lblJam.Font = new Font("Segoe UI", 9);
                        lblJam.ForeColor = Color.Black;
                        lblJam.AutoSize = true;
                        lblJam.Anchor = AnchorStyles.Right;
                        lblJam.Location = new Point(itemPanel.Width - 200, 15);

                        itemPanel.Controls.Add(lblAgenda);
                        itemPanel.Controls.Add(lblJam);

                        flowLayoutPanel1.Controls.Add(itemPanel);

                        nomor++;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }

                if (command != null)
                {
                    command.Dispose();
                }

                if (connection != null && connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse
        );

        private void Form2_Load(object sender, EventArgs e)
        {
            panel1.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, panel1.Width, panel1.Height, 30, 30));
            panel2.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, panel2.Width, panel2.Height, 30, 30));

            panel4.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, panel4.Width, panel4.Height, 30, 30));
            panel5.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, panel5.Width, panel5.Height, 30, 30));
            panel6.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, panel6.Width, panel6.Height, 30, 30));

            panel18.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, panel18.Width, panel18.Height, 30, 30));
            pictureBox1.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, pictureBox1.Width, pictureBox1.Height, 30, 30));
            pictureBox2.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, pictureBox2.Width, pictureBox2.Height, 30, 30));
            pictureBox3.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, pictureBox3.Width, pictureBox3.Height, 30, 30));
            pictureBox4.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, pictureBox4.Width, pictureBox4.Height, 30, 30));
            pictureBox5.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, pictureBox5.Width, pictureBox5.Height, 30, 30));
            FetchData();
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

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {

        }

        private void panel11_Paint(object sender, PaintEventArgs e)
        {
            Form4 f4 = new Form4();
            f4.Show();
        }

        private void label13_Click(object sender, EventArgs e)
        {
            Form4 f4 = new Form4();
            f4.Show();
        }

        private void panel13_Paint(object sender, PaintEventArgs e)
        {
            Form5 f5 = new Form5();
            f5.Show();
        }

        private void label15_Click(object sender, EventArgs e)
        {
            Form5 f5 = new Form5();
            f5.Show();
        }
    }
}
