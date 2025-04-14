using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Dashboard
{
    public partial class Form3 : Form
    {
        // Drawing state class to hold the state for each tab's PictureBox
        private class DrawingState
        {
            public Bitmap Bitmap { get; set; }
            public Graphics Graphics { get; set; }
            public Stack<Bitmap> History { get; set; }
            public bool IsPainting { get; set; }
            public Point PrevPoint { get; set; }
            public Point CurrentPoint { get; set; }
            public Point StartPoint { get; set; }

            public DrawingState(int width, int height)
            {
                Bitmap = new Bitmap(width, height);
                Graphics = Graphics.FromImage(Bitmap);
                Graphics.Clear(Color.White);
                History = new Stack<Bitmap>();
                IsPainting = false;
            }

            public void SaveToHistory()
            {
                // Clone the current bitmap and add to history stack
                History.Push(new Bitmap(Bitmap));
            }
        }

        // Only keep global variables that are shared across all tabs
        private Pen p = new Pen(Color.Black, 1); // Pen for drawing
        private int index; // Drawing mode
        private int eraserSize = 10; // Eraser size

        private PictureBox pic_color; // Tambahkan deklarasi ini di kelas Form3

        public Form3()
        {
            InitializeComponent();
            pic_color = new PictureBox(); // Inisialisasi kontrol pic_color
                                          // Tambahkan pengaturan kontrol pic_color sesuai kebutuhan, misalnya ukuran dan lokasi
            pic_color.Size = new Size(50, 50);
            pic_color.Location = new Point(10, 10);
            this.Controls.Add(pic_color); // Tambahkan kontrol pic_color ke form

            this.Load += Form3_Load;
        }
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse
        );

        private void Form3_Load(object sender, EventArgs e)
        {
            panel2.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, panel2.Width, panel2.Height, 30, 30));
            panel4.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, panel4.Width, panel4.Height, 30, 30));
            panel5.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, panel5.Width, panel5.Height, 30, 30));
            panel6.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, panel6.Width, panel6.Height, 30, 30));
            pictureBox5.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, pictureBox5.Width, pictureBox5.Height, 30, 30));
            pictureBox4.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, pictureBox4.Width, pictureBox4.Height, 30, 30));
            pictureBox3.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, pictureBox3.Width, pictureBox3.Height, 30, 30));
            pictureBox2.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, pictureBox2.Width, pictureBox2.Height, 30, 30));
            save1.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, save1.Width, save1.Height, 30, 30));
            btnNewTab.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnNewTab.Width, btnNewTab.Height, 30, 30));
            deletetab.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, deletetab.Width, deletetab.Height, 30, 30));
            tabControl1.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, tabControl1.Width, tabControl1.Height, 30, 30));
            panel1.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, panel1.Width, panel1.Height, 30, 30));


            // Create the first tab when the form loads
            if (tabControl1.TabCount == 0)
            {
                btnNewTab_Click(this, EventArgs.Empty);

            }
            timer1.Tick += timer1_Tick;
            timer1.Interval = 1000; // 1 detik
            timer1.Start();
        }

        private void InitializeGraphics(PictureBox pictureBox)
        {
            // Create new drawing state for this PictureBox
            DrawingState state = new DrawingState(pictureBox.Width, pictureBox.Height);
            pictureBox.Image = state.Bitmap;
            pictureBox.Tag = state;
        }

        private void pic_MouseDown(object sender, MouseEventArgs e)
        {
            PictureBox pictureBox = sender as PictureBox;
            DrawingState state = pictureBox.Tag as DrawingState;

            // Save the current state to history before making changes
            state.SaveToHistory();

            state.IsPainting = true;
            state.PrevPoint = e.Location;
            state.StartPoint = e.Location;
            state.CurrentPoint = e.Location;

            if (index == 6) // Eraser mode
            {
                state.Graphics.FillRectangle(new SolidBrush(Color.White),
                    e.X - (eraserSize / 2), e.Y - (eraserSize / 2), eraserSize, eraserSize);
                pictureBox.Image = state.Bitmap;
            }
        }

        private void pic_MouseMove(object sender, MouseEventArgs e)
        {
            PictureBox pictureBox = sender as PictureBox;
            DrawingState state = pictureBox.Tag as DrawingState;

            if (state.IsPainting) // If drawing is active
            {
                if (index == 1) // Free-drawing mode
                {
                    Point currentPoint = e.Location;
                    state.Graphics.DrawLine(p, state.PrevPoint, currentPoint);
                    state.PrevPoint = currentPoint;
                }
                else if (index == 6) // Eraser mode
                {
                    state.Graphics.FillRectangle(new SolidBrush(Color.White),
                        e.X - (eraserSize / 2), e.Y - (eraserSize / 2), eraserSize, eraserSize);
                }
            }

            pictureBox.Refresh(); // Refresh to show the preview drawing
            state.CurrentPoint = e.Location; // Update current point
        }

        private void pic_MouseUp(object sender, MouseEventArgs e)
        {
            PictureBox pictureBox = sender as PictureBox;
            DrawingState state = pictureBox.Tag as DrawingState;

            state.IsPainting = false;

            int width = Math.Abs(e.X - state.StartPoint.X);
            int height = Math.Abs(e.Y - state.StartPoint.Y);
            int startX = Math.Min(e.X, state.StartPoint.X);
            int startY = Math.Min(e.Y, state.StartPoint.Y);

            // Handle different drawing modes
            if (index == 2) // Circle
            {
                state.Graphics.DrawEllipse(p, startX, startY, width, height);
            }
            else if (index == 3) // Rectangle
            {
                state.Graphics.DrawRectangle(p, startX, startY, width, height);
            }
            else if (index == 4) // Line
            {
                state.Graphics.DrawLine(p, state.StartPoint.X, state.StartPoint.Y, e.X, e.Y);
            }
            else if (index == 5) // Triangle
            {
                Point point1 = state.StartPoint;
                Point point2 = new Point(e.X, e.Y);
                int baseMiddleX = (state.StartPoint.X + e.X) / 2;
                int baseMiddleY = (state.StartPoint.Y + e.Y) / 2;
                int baseVectorX = e.X - state.StartPoint.X;
                int baseVectorY = e.Y - state.StartPoint.Y;
                int perpVectorX = -baseVectorY;
                int perpVectorY = baseVectorX;

                double length = Math.Sqrt(perpVectorX * perpVectorX + perpVectorY * perpVectorY);
                if (length != 0)
                {
                    double baseLengthHalf = Math.Sqrt(baseVectorX * baseVectorX + baseVectorY * baseVectorY) / 2;
                    perpVectorX = (int)(perpVectorX / length * baseLengthHalf);
                    perpVectorY = (int)(perpVectorY / length * baseLengthHalf);
                }

                Point point3 = new Point(baseMiddleX + perpVectorX, baseMiddleY + perpVectorY);
                state.Graphics.DrawPolygon(p, new Point[] { point1, point2, point3 });
            }
            else if (index == 7) // Filled rectangle
            {
                state.Graphics.FillRectangle(new SolidBrush(p.Color), startX, startY, width, height);
            }
            else if (index == 8) // Filled circle
            {
                state.Graphics.FillEllipse(new SolidBrush(p.Color), startX, startY, width, height);
            }
            else if (index == 9) // Filled triangle
            {
                Point point1 = state.StartPoint;
                Point point2 = new Point(e.X, e.Y);
                int baseMiddleX = (state.StartPoint.X + e.X) / 2;
                int baseMiddleY = (state.StartPoint.Y + e.Y) / 2;
                int baseVectorX = e.X - state.StartPoint.X;
                int baseVectorY = e.Y - state.StartPoint.Y;
                int perpVectorX = -baseVectorY;
                int perpVectorY = baseVectorX;

                double length = Math.Sqrt(perpVectorX * perpVectorX + perpVectorY * perpVectorY);
                if (length != 0)
                {
                    double baseLengthHalf = Math.Sqrt(baseVectorX * baseVectorX + baseVectorY * baseVectorY) / 2;
                    perpVectorX = (int)(perpVectorX / length * baseLengthHalf);
                    perpVectorY = (int)(perpVectorY / length * baseLengthHalf);
                }

                Point point3 = new Point(baseMiddleX + perpVectorX, baseMiddleY + perpVectorY);
                state.Graphics.FillPolygon(new SolidBrush(p.Color), new Point[] { point1, point2, point3 });
            }

            pictureBox.Image = state.Bitmap;
        }

        private void pic_Paint(object sender, PaintEventArgs e)
        {
            PictureBox pictureBox = sender as PictureBox;
            DrawingState state = pictureBox.Tag as DrawingState;

            if (!state.IsPainting) return;

            int width = Math.Abs(state.CurrentPoint.X - state.StartPoint.X);
            int height = Math.Abs(state.CurrentPoint.Y - state.StartPoint.Y);
            int startX = Math.Min(state.CurrentPoint.X, state.StartPoint.X);
            int startY = Math.Min(state.CurrentPoint.Y, state.StartPoint.Y);

            // Draw temporary shapes for preview
            if (index == 2) // Circle
            {
                e.Graphics.DrawEllipse(p, startX, startY, width, height);
            }
            else if (index == 3) // Rectangle
            {
                e.Graphics.DrawRectangle(p, startX, startY, width, height);
            }
            else if (index == 4) // Line
            {
                e.Graphics.DrawLine(p, state.StartPoint.X, state.StartPoint.Y, state.CurrentPoint.X, state.CurrentPoint.Y);
            }
            else if (index == 5) // Triangle
            {
                Point point1 = state.StartPoint;
                Point point2 = state.CurrentPoint;
                int baseMiddleX = (state.StartPoint.X + state.CurrentPoint.X) / 2;
                int baseMiddleY = (state.StartPoint.Y + state.CurrentPoint.Y) / 2;
                int baseVectorX = state.CurrentPoint.X - state.StartPoint.X;
                int baseVectorY = state.CurrentPoint.Y - state.StartPoint.Y;
                int perpVectorX = -baseVectorY;
                int perpVectorY = baseVectorX;

                double length = Math.Sqrt(perpVectorX * perpVectorX + perpVectorY * perpVectorY);
                if (length != 0)
                {
                    double baseLengthHalf = Math.Sqrt(baseVectorX * baseVectorX + baseVectorY * baseVectorY) / 2;
                    perpVectorX = (int)(perpVectorX / length * baseLengthHalf);
                    perpVectorY = (int)(perpVectorY / length * baseLengthHalf);
                }

                Point point3 = new Point(baseMiddleX + perpVectorX, baseMiddleY + perpVectorY);
                e.Graphics.DrawPolygon(p, new Point[] { point1, point2, point3 });
            }
            else if (index == 6) // Eraser
            {
                e.Graphics.DrawRectangle(new Pen(Color.LightGray),
                    state.CurrentPoint.X - (eraserSize / 2),
                    state.CurrentPoint.Y - (eraserSize / 2),
                    eraserSize, eraserSize);
            }
            else if (index == 7) // Filled rectangle
            {
                e.Graphics.FillRectangle(new SolidBrush(p.Color), startX, startY, width, height);
            }
            else if (index == 8) // Filled circle
            {
                e.Graphics.FillEllipse(new SolidBrush(p.Color), startX, startY, width, height);
            }
            else if (index == 9) // Filled triangle
            {
                Point point1 = state.StartPoint;
                Point point2 = state.CurrentPoint;
                int baseMiddleX = (state.StartPoint.X + state.CurrentPoint.X) / 2;
                int baseMiddleY = (state.StartPoint.Y + state.CurrentPoint.Y) / 2;
                int baseVectorX = state.CurrentPoint.X - state.StartPoint.X;
                int baseVectorY = state.CurrentPoint.Y - state.StartPoint.Y;
                int perpVectorX = -baseVectorY;
                int perpVectorY = baseVectorX;

                double length = Math.Sqrt(perpVectorX * perpVectorX + perpVectorY * perpVectorY);
                if (length != 0)
                {
                    double baseLengthHalf = Math.Sqrt(baseVectorX * baseVectorX + baseVectorY * baseVectorY) / 2;
                    perpVectorX = (int)(perpVectorX / length * baseLengthHalf);
                    perpVectorY = (int)(perpVectorY / length * baseLengthHalf);
                }

                Point point3 = new Point(baseMiddleX + perpVectorX, baseMiddleY + perpVectorY);
                e.Graphics.FillPolygon(new SolidBrush(p.Color), new Point[] { point1, point2, point3 });
            }
        }

        // Helper method to get the current PictureBox
        private PictureBox GetCurrentPictureBox()
        {
            if (tabControl1.SelectedTab == null)
                return null;

            // Find the PictureBox in the current tab
            foreach (Control control in tabControl1.SelectedTab.Controls)
            {
                if (control is PictureBox)
                    return control as PictureBox;
            }

            return null;
        }

        private void Pen_Click(object sender, EventArgs e)
        {
            index = 1; // Free-drawing mode
        }

        private void circle_Click(object sender, EventArgs e)
        {
            index = 2; // Circle mode
        }

        private void Persegi_Click(object sender, EventArgs e)
        {
            index = 3; // Rectangle mode
        }

        private void garis_Click(object sender, EventArgs e)
        {
            index = 4; // Line mode
        }

        private void segitiga_Click(object sender, EventArgs e)
        {
            index = 5; // Triangle mode
        }

        private void eraser_Click(object sender, EventArgs e)
        {
            index = 6; // Eraser mode
        }

        private void Fullpersegi_Click(object sender, EventArgs e)
        {
            index = 7; // Filled rectangle mode
        }

        private void fullcircle_Click(object sender, EventArgs e)
        {
            index = 8; // Filled circle mode
        }

        private void fullsegitiga_Click(object sender, EventArgs e)
        {
            index = 9; // Filled triangle mode
        }

        private void hapus_Click(object sender, EventArgs e)
        {
            // Clear the current tab's canvas
            PictureBox pictureBox = GetCurrentPictureBox();
            if (pictureBox != null)
            {
                DrawingState state = pictureBox.Tag as DrawingState;
                state.SaveToHistory(); // Save current state before clearing
                state.Graphics.Clear(Color.White);
                pictureBox.Image = state.Bitmap;
            }

            index = 0;
        }

        private void back_Click(object sender, EventArgs e)
        {
            // Undo: Restore the previous state
            PictureBox pictureBox = GetCurrentPictureBox();
            if (pictureBox != null)
            {
                DrawingState state = pictureBox.Tag as DrawingState;
                if (state.History.Count > 0)
                {
                    state.Bitmap = state.History.Pop();
                    state.Graphics = Graphics.FromImage(state.Bitmap);
                    pictureBox.Image = state.Bitmap;
                }
            }
        }



        private void btnNewTab_Click(object sender, EventArgs e)
        {
            // Create a new tab
            TabPage newTab = new TabPage("Whiteboard " + (tabControl1.TabCount + 1));
            tabControl1.TabPages.Add(newTab);

            // Create a PictureBox as canvas
            PictureBox pic = new PictureBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            // Add PictureBox to the new tab
            newTab.Controls.Add(pic);

            // Make sure PictureBox is properly sized
            pic.Width = newTab.Width;
            pic.Height = newTab.Height;

            // Initialize graphics for this PictureBox
            InitializeGraphics(pic);

            // Add event handlers for drawing
            pic.MouseDown += new MouseEventHandler(pic_MouseDown);
            pic.MouseMove += new MouseEventHandler(pic_MouseMove);
            pic.MouseUp += new MouseEventHandler(pic_MouseUp);
            pic.Paint += new PaintEventHandler(pic_Paint);

            // Add and position panel7 - center bottom
            panel7.Anchor = AnchorStyles.Bottom;
            panel7.Location = new Point((newTab.ClientSize.Width - panel7.Width) / 2, newTab.ClientSize.Height - panel7.Height - 10);
            newTab.Controls.Add(panel7);
            panel7.BringToFront();

            // Add and position trackBar1 - right side
            trackBar1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            trackBar1.Location = new Point(newTab.ClientSize.Width - trackBar1.Width - 10, 10);
            newTab.Controls.Add(trackBar1);
            trackBar1.BringToFront();

            // Switch to the new tab
            tabControl1.SelectedTab = newTab;
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            p.Width = trackBar1.Value;
        }
        private void color_Click(object sender, EventArgs e)
        {
            using (ColorDialog cd = new ColorDialog())
            {
                if (cd.ShowDialog() == DialogResult.OK)
                {
                    Color new_color = cd.Color; // Memilih warna
                    pic_color.BackColor = new_color; // Menampilkan warna yang dipilih
                    p.Color = new_color; // Mengganti warna pensil
                }
            }
        }

        private void save1_Click(object sender, EventArgs e)
        {
            PictureBox pictureBox = GetCurrentPictureBox();
            if (pictureBox == null) return;

            DrawingState state = pictureBox.Tag as DrawingState;
            if (state == null || state.Bitmap == null)
            {
                MessageBox.Show("No image to save.");
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "JPEG Image|*.jpg|PNG Image|*.png|Bitmap Image|*.bmp|All Files|*.*";
                sfd.Title = "Save an Image File";
                sfd.FileName = "untitled";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        ImageFormat format = ImageFormat.Jpeg; // Default format

                        // Determine format based on the file extension
                        string extension = System.IO.Path.GetExtension(sfd.FileName).ToLower();
                        switch (extension)
                        {
                            case ".jpg":
                            case ".jpeg":
                                format = ImageFormat.Jpeg;
                                break;
                            case ".png":
                                format = ImageFormat.Png;
                                break;
                            case ".bmp":
                                format = ImageFormat.Bmp;
                                break;
                        }

                        using (Bitmap btm = new Bitmap(state.Bitmap))
                        {
                            btm.Save(sfd.FileName, format);
                        }
                        MessageBox.Show("Image saved successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error saving image: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void deletetab_Click(object sender, EventArgs e)
        {
            if (tabControl1.TabCount > 0)
            {
                TabPage currentTab = tabControl1.SelectedTab;
                if (currentTab != null)
                {
                    tabControl1.TabPages.Remove(currentTab);
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        { 
            DateTime waktu = DateTime.Now;
            label1.Text = waktu.ToString("HH:mm");
            label2.Text = waktu.ToString("dd/MM/yyyy");
            label3.Text = waktu.ToString("dddd");
        }
    }
}