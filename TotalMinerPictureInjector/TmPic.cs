using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TotalMinerPictureInjector
{
    public class TmPic
    {
        public Bitmap Lod, Hd;

        public int index { private set; get; } = 0;

        public TmPic(BinaryReader reader)
        {
            index = reader.ReadInt32();

            Hd = Load(reader);
            Lod = Load(reader);
        }

        public TmPic(int index)
        {
            this.index = index;
            var placeholder = Bitmap.FromFile(@"PlaceholderImage.png");
            Hd = new Bitmap(placeholder, new Size(64, 64));
            Lod = new Bitmap(placeholder, new Size(16, 16));
        }

        private Bitmap Load(BinaryReader reader)
        {
            int ColorCount = reader.ReadInt32();

            if (ColorCount <= 0)
                return null;

            long pos = reader.BaseStream.Position;
            byte[] array = reader.ReadBytes(ColorCount * 3);
            var colorData = array.ToColorData();
            var size = (int)Math.Sqrt(colorData.Length);

            return Helper.FromTMData(size, size, colorData);
        }
        
        public void Replace(ref Bitmap bitmap, string path)
        {
            bitmap = new Bitmap(Bitmap.FromFile(path), bitmap.Size);
        }

        public void Extract(Bitmap bitmap, string path)
        {
            bitmap.Save(path, ImageFormat.Png);
        }
    }
}
