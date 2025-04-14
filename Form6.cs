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
using System.IO;

namespace Dashboard
{
    public partial class Form6 : Form
    {
        public Form6()
        {
            InitializeComponent();
            this.Load += Form6_Load;
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

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse
        );

        private void Form6_Load(object sender, EventArgs e)
        {
            panel4.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, panel4.Width, panel4.Height, 30, 30));
            panel2.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, panel2.Width, panel2.Height, 30, 30));
            SetupUILayout(panel2); // Pass panel2 as the container
        }

            void SetupUILayout(Panel container)
            {
                // === TabControl untuk User ===
                TabControl tabUsers = new TabControl();
                tabUsers.Name = "tabUsers";
                tabUsers.Dock = DockStyle.Top;
                tabUsers.Height = 60; // Tinggi TabControl untuk memberikan jarak lebih banyak
                container.Controls.Add(tabUsers);

                // === TableLayoutPanel untuk Tata Letak Utama ===
                TableLayoutPanel mainLayout = new TableLayoutPanel();
                mainLayout.Dock = DockStyle.Fill;
                mainLayout.ColumnCount = 2;
                mainLayout.RowCount = 1;
                mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150)); // Sidebar width

                container.Controls.Add(mainLayout);

                // === Panel kiri untuk kategori (Sidebar) ===
                Panel sidebar = new Panel();
                sidebar.Dock = DockStyle.Fill;
                sidebar.BackColor = Color.LightGray;
                mainLayout.Controls.Add(sidebar, 0, 0); // Tambahkan ke kolom pertama

                // === FlowLayoutPanel untuk isi konten (Thumbnail Preview) ===
                FlowLayoutPanel contentPanel = new FlowLayoutPanel();
                contentPanel.Name = "flowContent";
                contentPanel.Dock = DockStyle.Fill;
                contentPanel.AutoScroll = true;
                contentPanel.Padding = new Padding(10, 50, 10, 20); // Padding untuk menurunkan konten
                mainLayout.Controls.Add(contentPanel, 1, 0); // Tambahkan ke kolom kedua

                // === Daftar kategori ===
                string[] categories = { "DOC", "Picture", "Board", "video", "Other" };
                int y = 20;
                foreach (string cat in categories)
                {
                    Button btn = new Button();
                    btn.Text = $"{cat} (0)";
                    btn.Width = 130;
                    btn.Height = 40;
                    btn.Location = new Point(10, y + 50);
                    btn.Tag = cat;
                    btn.Click += (s, e) =>
                    {
                        string selectedCat = ((Button)s).Tag.ToString();
                        string selectedUser = ((TabControl)container.Controls.Find("tabUsers", true)[0]).SelectedTab.Tag.ToString();
                        LoadCategoryContent(selectedCat, contentPanel, selectedUser);
                    };

                    sidebar.Controls.Add(btn);
                    y += 50;
                }

                // === Simulasi isi tab user ===
                using (var conn = OpenConnection())
                {
                    MySqlCommand cmd = new MySqlCommand("SELECT DISTINCT user FROM materialmeeting", conn);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string username = reader["user"].ToString();
                        TabPage tab = new TabPage(username);
                        tab.Tag = username;
                        tabUsers.TabPages.Add(tab);
                    }
                }

                tabUsers.SelectedIndexChanged += (s, e) =>
                {
                    // Reset isi konten saat user berubah
                    contentPanel.Controls.Clear();
                };
            }

        void LoadCategoryContent(string category, FlowLayoutPanel contentPanel, string username)
        {
            contentPanel.Controls.Clear();

            using (var conn = OpenConnection())
            {
                string query = "";
                if (category == "DOC")
                    query = "SELECT tempat_menyimpan_file_dok AS path FROM materialmeeting WHERE user = @user";
                else if (category == "Picture")
                    query = "SELECT tempat_menyimpan_file_pic AS path FROM materialmeeting WHERE user = @user";
                else if (category == "Board")
                    query = "SELECT tempat_menyimpan_file_board AS path FROM materialmeeting WHERE user = @user";
                else if (category == "video")
                    query = "SELECT tempat_menyimpan_file_video AS path FROM materialmeeting WHERE user = @user";
                else if (category == "Other")
                    query = "SELECT tempat_menyimpan_file_other AS path FROM materialmeeting WHERE user = @user";
                else
                    return;

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@user", username);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string filePath = reader["path"].ToString();
                    if (string.IsNullOrEmpty(filePath)) continue;

                    // Buat tombol untuk file
                    Button btnFile = new Button();
                    btnFile.Text = System.IO.Path.GetFileName(filePath);
                    btnFile.Width = 300;
                    btnFile.Height = 40;
                    btnFile.Margin = new Padding(10);
                    btnFile.Tag = filePath;

                    btnFile.Click += (s, e) =>
                    {
                        try
                        {
                            string relativePath = (string)((Button)s).Tag;
                            string fullPath = Path.Combine(@"C:\xampp\htdocs\material_meeting", relativePath);

                            // Periksa apakah file ada
                            if (!File.Exists(fullPath))
                            {
                                MessageBox.Show("File tidak ditemukan: " + fullPath);
                                return;
                            }

                            // Gunakan ProcessStartInfo untuk membuka file dengan lebih baik
                            var psi = new System.Diagnostics.ProcessStartInfo
                            {
                                FileName = fullPath,
                                UseShellExecute = true
                            };
                            System.Diagnostics.Process.Start(psi);
                        }
                        catch (Win32Exception ex)
                        {
                            MessageBox.Show("Tidak dapat membuka file: " + ex.Message);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Gagal membuka file: " + ex.Message);
                        }
                    };

                    contentPanel.Controls.Add(btnFile);
                }
            }
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form6_Load_1(object sender, EventArgs e)
        {

        }
    }
}