using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Dashboard
{
    public partial class Form1 : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse
        );

        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Menerapkan efek rounded corners untuk beberapa panel
            int radius = 30;
            panel3.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, panel3.Width, panel3.Height, radius, radius));
            panel4.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, panel4.Width, panel4.Height, radius, radius));
            panel5.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, panel5.Width, panel5.Height, radius, radius));
            panel6.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, panel6.Width, panel6.Height, radius, radius));
            gambar1.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, gambar1.Width, gambar1.Height, radius, radius));
            gambar2.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, gambar2.Width, gambar2.Height, radius, radius));
            gambar3.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, gambar3.Width, gambar3.Height, radius, radius));
            gambar4.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, gambar4.Width, gambar4.Height, radius, radius));
            gambar5.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, gambar5.Width, gambar5.Height, radius, radius));
            gambar6.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, gambar6.Width, gambar6.Height, radius, radius));
            gambar7.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, gambar7.Width, gambar7.Height, radius, radius));
            gambar8.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, gambar8.Width, gambar8.Height, radius, radius));
            gambar9.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, gambar9.Width, gambar9.Height, radius, radius));
            gambar10.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, gambar10.Width, gambar10.Height, radius, radius));
            pictureBox1.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, pictureBox1.Width, pictureBox1.Height, radius, radius));
            pictureBox2.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, pictureBox2.Width, pictureBox2.Height, radius, radius));
            pictureBox3.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, pictureBox3.Width, pictureBox3.Height, radius, radius));
            pictureBox4.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, pictureBox4.Width, pictureBox4.Height, radius, radius));
            pictureBox5.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, pictureBox5.Width, pictureBox5.Height, radius, radius));
            button1.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, button1.Width, button1.Height, radius, radius));

            // Mengaktifkan timer
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

        private void gambar2_Click_1(object sender, EventArgs e)
        {
            Form2 f2 = new Form2();
            f2.Show();
        }

        private void Agenda_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form2 f2 = new Form2();
            f2.Show();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void gambar10_Click(object sender, EventArgs e)
        {
            Form3 f3 = new Form3();
            f3.Show();
        }

        private void linkLabel10_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form3 f3 = new Form3();
            f3.Show();
        }
    }
}
