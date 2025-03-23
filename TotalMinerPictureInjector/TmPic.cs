using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TotalMinerPictureInjector
{
    //Sd = 16 SDThumbnail.dat
    //hd = 64 HDThumbnail.dat
    //org = 864 PhotoImage.dat

    public class GlobalPic
    {
        public Bitmap Sd, Hd, Org;
        public int Version;
        public bool Edited, IsValid;
        public string Filename, OrgPAth, HdPath, SdPAth;

        public GlobalPic(string Dir ,bool create, int index, Bitmap bitmap)
        {
            var photoinfo = Path.Combine(Dir, "PhotoInfo.dat");
            File.Copy("PhotoInfo.dat", photoinfo);

            using (var writer = new BinaryWriter(File.OpenWrite(photoinfo)))
            {
                writer.Seek(sizeof(Int32), SeekOrigin.Current);
                writer.Write(index);
            }

            Org = Sd = Hd = bitmap;
            OrgPAth = Path.Combine(Dir, @"PhotoImage.dat");
            HdPath = Path.Combine(Dir, @"HDThumbnail.dat");
            SdPAth = Path.Combine(Dir, @"SDThumbnail.dat");
        }

        public GlobalPic(string Dir)
        {

            OrgPAth = Path.Combine(Dir, @"PhotoImage.dat");
            if (File.Exists(OrgPAth))
            {
                using (BinaryReader reader = new BinaryReader(File.OpenRead(OrgPAth)))
                {
                    Org = Load(reader);
                }
            }
            else
            {
                IsValid = false;
                return;
            }

            HdPath = Path.Combine(Dir, @"HDThumbnail.dat");
            if (File.Exists(HdPath))
            {
                using (BinaryReader reader = new BinaryReader(File.OpenRead(HdPath)))
                {
                    Hd = Load(reader);
                }
            }
            else if(File.Exists(OrgPAth))
            {
                Hd = new Bitmap(Org, new Size(64, 64));
            }
            else
            {
                IsValid = false;
                return;
            }

            SdPAth = Path.Combine(Dir, @"SDThumbnail.dat");
            if (File.Exists(SdPAth))
            {
                using (BinaryReader reader = new BinaryReader(File.OpenRead(SdPAth)))
                {
                    Sd = Load(reader);
                }
            }
            else if (File.Exists(OrgPAth))
            {
                Sd = new Bitmap(Org, new Size(16, 16));
            }
            else if (File.Exists(HdPath))
            {
                Sd = new Bitmap(Hd, new Size(16, 16));
            }
            else
            {
                IsValid = false;
                return;
            }

            Edited = false;
            IsValid = true;
            Filename = Dir;
        }

        private Bitmap Load(BinaryReader reader)
        {
            Version = reader.ReadInt32();

            int ColorCount = (int)(reader.BaseStream.Length - reader.BaseStream.Position);

            if (ColorCount <= 0)
                return null;

            long pos = reader.BaseStream.Position;
            byte[] array = reader.ReadBytes(ColorCount * 3);
            var colorData = array.ToColorData();
            var size = (int)Math.Sqrt(colorData.Length);

            return Helper.FromTMData(size, size, colorData);
        }

        public void Replace(string path)
        {
            var bitmap = Bitmap.FromFile(path);
            Org = new Bitmap(bitmap, new Size(864, 864));
            Hd = new Bitmap(bitmap, new Size(64, 64));
            Sd = new Bitmap(bitmap, new Size(16, 16));
            Edited = true;
        }

        public void Save()
        {
            using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(OrgPAth)))
            {
                writer.Write(Version);
                writer.Write(Helper.ToColorDataAll(new Bitmap(Org, new Size(864, 864)), out _));
            }

            using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(HdPath)))
            {
                writer.Write(Version);
                writer.Write(Helper.ToColorDataAll(new Bitmap(Hd, new Size(64, 64)), out _));
            }

            using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(SdPAth)))
            {
                writer.Write(Version);
                writer.Write(Helper.ToColorDataAll(new Bitmap(Sd, new Size(16, 16)), out _));
            }
        }
    }

    public class TmPic
    {
        public Bitmap Sd, Hd;

        public int index { private set; get; } = 0;

        public TmPic(BinaryReader reader)
        {
            index = reader.ReadInt32();

            Hd = Load(reader);
            Sd = Load(reader);
        }

        public TmPic(int index)
        {
            this.index = index;
            var placeholder = Bitmap.FromFile(@"PlaceholderImage.png");
            Hd = new Bitmap(placeholder, new Size(64, 64));
            Sd = new Bitmap(placeholder, new Size(16, 16));
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
