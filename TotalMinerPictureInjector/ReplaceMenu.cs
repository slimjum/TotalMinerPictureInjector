using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace TotalMinerPictureInjector
{
    public partial class ReplaceDialogMenu : Form
    {
        public Bitmap[] Bitmaps;

        public Bitmap img;

        public string Filename = string.Empty;

        public ReplaceDialogMenu()
        {
            InitializeComponent();
        }

        private void ReplaceDialogMenu_Load(object sender, EventArgs e)
        {
            using (FileDialog file = new OpenFileDialog() { Filter = "img file|*.png", })
            {
                var dialog = file.ShowDialog();

                if (dialog == DialogResult.OK)
                {
                   Filename = file.FileName;
                }
                else
                {
                    DialogResult = DialogResult.Abort;
                    Close();
                }
            }
        }

        private void Replace_Click(object sender, EventArgs e)
        {
            var xytext = textBox1.Text.ToLower().Split('x');

            if (xytext.Length < 1 || !int.TryParse(xytext[0], out int x) || !int.TryParse(xytext[1], out int y))
                return;

            if (x < 1 || y < 1)
                return;

            Bitmaps = new Bitmap[x * y];

            img = new Bitmap(Bitmap.FromFile(Filename));

            int dx = img.Width / x;
            int dy = img.Height / y;
            var bitmap = new Bitmap(Bitmap.FromFile(Filename), new Size(dx * x, dy * y));

            int ttt = 0;
            for (int indexX = 0; indexX < x; indexX++)
            {
                for (int indexY = 0; indexY < y; indexY++)
                {
                    Bitmaps[ttt++] = cropAtRect(bitmap, new Rectangle(indexX * dx, indexY * dy, dx, dy));
                }
            }


            DialogResult = DialogResult.OK;
        }

        private void About_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Abort;
        }

        private Bitmap cropAtRect(Bitmap b, Rectangle r)
        {
            return b.Clone(r, PixelFormat.Format24bppRgb);
        }
    }
}
