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
        public Tuple<Bitmap, long> Lod;

        public Tuple<Bitmap, long> Hd;

        public int index { private set; get; } = 0;

        public bool Edited { private set; get; } = false;

        public TmPic(BinaryReader reader)
        {
            index = reader.ReadInt32();

            Hd = Load(reader);
            Lod = Load(reader);
        }

        private Tuple<Bitmap, long> Load(BinaryReader reader)
        {
            int ColorCount = reader.ReadInt32();

            if (ColorCount <= 0)
                return null;

            long pos = reader.BaseStream.Position;
            byte[] array = reader.ReadBytes(ColorCount * 3);
            var colorData = array.ToColorData();
            var size = (int)Math.Sqrt(colorData.Length);

            return new Tuple<Bitmap, long>(Helper.FromTMData(size, size, colorData), pos);
        }
        
        public void Replace(ref Tuple<Bitmap, long> data, string path)
        {
           var pic = new Bitmap(Bitmap.FromFile(path), data.Item1.Size);
           var temp_index = data.Item2;

            data = new Tuple<Bitmap, long>(pic, temp_index);
            Edited = true;
        }

        public void Extract(Tuple<Bitmap, long> data, string path)
        {
            data.Item1.Save(path, ImageFormat.Png);
        }
    }
}
