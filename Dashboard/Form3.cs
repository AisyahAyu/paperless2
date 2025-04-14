using System;
using System.Drawing;
using System.Windows.Forms;

namespace Dashboard
{
    public partial class Form3 : Form
    {
        Bitmap bm; // Bitmap untuk menyimpan gambar permanen
        Graphics g; // Graphics untuk menggambar pada Bitmap
        bool paint = false; // Status apakah pengguna sedang menggambar
        Point px, py; // Titik sebelumnya untuk mode garis
        Pen p = new Pen(Color.Black, 1); // Pensil untuk menggambar
        int index; // Mode menggambar (1: garis, 2: lingkaran)
        int x, y, cx, cy; // Koordinat saat ini dan awal

        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            // Menginisialisasi PictureBox
            pic.Width = 1404;
            pic.Height = 545;
            pic.BackColor = Color.White;

            // Inisialisasi Bitmap dan Graphics
            InitializeGraphics();
        }

        private void InitializeGraphics()
        {
            // Pastikan Bitmap dan Graphics diinisialisasi
            if (bm == null || g == null)
            {
                bm = new Bitmap(pic.Width, pic.Height);
                g = Graphics.FromImage(bm);
                g.Clear(Color.White);
                pic.Image = bm;
            }
        }

        private void pic_MouseDown(object sender, MouseEventArgs e)
        {
            paint = true; // Mulai menggambar
            cx = e.X; // Titik awal X
            cy = e.Y; // Titik awal Y

            // Pastikan Graphics sudah diinisialisasi
            InitializeGraphics();
        }

        private void pic_MouseMove(object sender, MouseEventArgs e)
        {
            if (!paint) return; // Jika tidak sedang menggambar, keluar

            x = e.X; // Update koordinat X saat ini
            y = e.Y; // Update koordinat Y saat ini

            if (index == 1) // Mode menggambar garis
            {
                px = e.Location; // Titik saat ini
                g.DrawLine(p, py, px); // Gambar garis dari titik sebelumnya ke titik saat ini
                py = px; // Update titik sebelumnya
                pic.Invalidate(); // Refresh PictureBox
            }
            else if (index == 2) // Mode menggambar lingkaran
            {
                pic.Invalidate(); // Pemicu event Paint untuk menggambar lingkaran sementara
            }
        }

        private void pic_MouseUp(object sender, MouseEventArgs e)
        {
            paint = false; // Berhenti menggambar

            if (index == 2) // Mode menggambar lingkaran
            {
                // Gambar lingkaran permanen pada Bitmap
                int width = e.X - cx; // Lebar lingkaran
                int height = e.Y - cy; // Tinggi lingkaran
                g.DrawEllipse(p, cx, cy, width, height);

                // Simpan hasil ke Bitmap
                pic.Image = bm;
            }
        }

        private void pic_Paint(object sender, PaintEventArgs e)
        {
            if (paint && index == 2) // Hanya jika mode lingkaran dan sedang menggambar
            {
                // Gambar lingkaran sementara selama drag mouse
                int width = x - cx; // Lebar lingkaran sementara
                int height = y - cy; // Tinggi lingkaran sementara
                e.Graphics.DrawEllipse(p, cx, cy, width, height);
            }
        }

        private void Pen_Click(object sender, EventArgs e)
        {
            // Aktifkan mode menggambar garis
            index = 1;

            // Pastikan Bitmap dan Graphics sudah diinisialisasi
            InitializeGraphics();
        }

        private void circle_Click(object sender, EventArgs e)
        {
            // Aktifkan mode menggambar lingkaran
            index = 2;

            // Pastikan Bitmap dan Graphics sudah diinisialisasi
            InitializeGraphics();
        }
    }
}