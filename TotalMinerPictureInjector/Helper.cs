using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TotalMinerPictureInjector
{
    public static class Helper
    {
        public static byte[] ToColorDataAll(Bitmap pic, out int sroot)
        {
            var Carry = ToColorData(pic, out sroot, pic.Height * pic.Width);

            return ToColorData(Carry, sroot);
        }

        public static byte[] ToColorData(Color[] Carry, int sroot)
        {
            var array = new byte[(sroot * sroot) * 3];
            for (int i = 0; i < array.Length;)
            {
                var obj = Carry[i / 3];
                array[i++] = obj.R;
                array[i++] = obj.G;
                array[i++] = obj.B;
            }

            return array;
        }

        public static Color[] ToColorData(Bitmap pic, out int sroot, int array_size = 4096)
        {
            var Carry = new Color[array_size];
            sroot = (int)Math.Sqrt(array_size);
            for (int x = 0; x < sroot; x++)
            {
                for (int y = 0; y < sroot; y++)
                {
                    Carry[(y * sroot) + x] = pic.GetPixel(x, y);
                }
            }

            return Carry;
        }

        public static Color[] ToColorData( this byte[] array)
        {
            var data = new Color[array.Length / 3];

            for (int i = 0; i < array.Length;)
            {
                var rgb_r = array[i];
                var rgb_g = array[i + 1];
                var rgb_b = array[i + 2];

                data[i / 3] = Color.FromArgb(rgb_r, rgb_g, rgb_b);

                i += 3;
            }

            return data;
        }

        public static Bitmap FromTMData(int w, int h, Color[] data)
        {
            Bitmap pic = new Bitmap(w, h, PixelFormat.Format32bppArgb);

            for (int i = 0; i < data.Length; i++)
            {
                pic.SetPixel(i % w, i / w, data[i]);
            }

            return pic;
        }
    }


}
