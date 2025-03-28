﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TotalMinerPictureInjector
{
    public partial class Form1 : Form
    {
        public List<TmPic> pics = new List<TmPic>();

        public List<GlobalPic> GlobalPics = new List<GlobalPic>();

        public string Selected_File;

        public Form1()
        {
            InitializeComponent();

            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            checkBox1.Enabled = false;
        }

        private void Extract_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count < 0 && pics.Count < 0)
                return;

            var index = listBox1.SelectedIndex;

            using (SaveFileDialog file = new SaveFileDialog() { Filter = "img file|*.png", })
            {
                var dialog = file.ShowDialog();

                if (dialog == DialogResult.OK)
                    pics[index].Extract(pics[index].Hd, file.FileName);
            }
        }

        private void Replace_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count < 0 && pics.Count < 0)
                return;

            var index = listBox1.SelectedIndex;

            using (FileDialog file = new OpenFileDialog() { Filter = "img file|*.png",})
            {
                var dialog = file.ShowDialog();

                if (dialog == DialogResult.OK)
                {
                    pics[index].Replace(ref pics[index].Hd, file.FileName);
                    pics[index].Replace(ref pics[index].Sd, file.FileName);
                    pictureBox1.Image = pics[index].Hd;
                }
            }
        }

        private void Load_Click(object sender, EventArgs e)
        {
            pics.Clear();
            listBox1.Items.Clear();
            pictureBox1.Image = null;

            if (listBox1.Items.Count < 0 && pics.Count < 0)
                return;


            string mygames = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"My Games\TotalMiner\Maps\");
            using (FileDialog file =  new OpenFileDialog(){ FileName = "photos.dat", Filter = "TM Photo File|*.dat", InitialDirectory = mygames, Title = "Select photos.dat" })
            {
                var dialog = file.ShowDialog();

                if (string.IsNullOrEmpty(file.FileName) || Path.GetFileName(file.FileName) != "photos.dat")
                {
                    MessageBox.Show($"{Path.GetFileName(file.FileName)} is not a valid file. Please select \"photos.dat\".", "Invalid File");
                    return;
                }

                if (dialog == DialogResult.OK)
                    LoadPics(file.FileName);

                return;
            }
        }

        private void LoadPics(string path)
        {
            Selected_File = path;

            using (BinaryReader reader = new BinaryReader(File.OpenRead(path)))
            {
                int count = reader.ReadInt32();

                if (count < 0)
                    return;

                for (int i = 0; i < count; i++)
                {
                    listBox1.Items.Add($"Index: {i + 1}");
                    pics.Add(new TmPic(reader));
                }
            }
            checkBox1.Enabled = listBox1.Items.Count > 0;
        }

        private void clear_Click(object sender, EventArgs e)
        {
            pics.Clear();
            listBox1.Items.Clear();
            pictureBox1.Image = null;
            Selected_File = "";
            checkBox1.Enabled = false;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var index = listBox1.SelectedIndex;

            if (index == -1)
                return;

            pictureBox1.Image = checkBox1.Checked ? pics[index].Sd : pics[index].Hd;
        }

        private void Save_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Selected_File))
                return;

            using (BinaryWriter Writer = new BinaryWriter(File.Open(Selected_File, FileMode.Open)))
            {
                Writer.Write(pics.Count);
                for (int index = 0; index < pics.Count; index++)
                {
                    Writer.Write(pics[index].index);

                    Writer.Write(64 * 64);
                    Writer.Write(Helper.ToColorDataAll(new Bitmap(pics[index].Hd, new Size(64, 64)), out int sroot1));

                    Writer.Write(16 * 16);
                    Writer.Write(Helper.ToColorDataAll(new Bitmap(pics[index].Sd, new Size(16, 16)), out int sroot2));
                }
            }

            clear_Click(this, EventArgs.Empty);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            var index = listBox1.SelectedIndex;

            if (index == -1)
            {
                MessageBox.Show("Please select a photo first.");
                return;
            }

            pictureBox1.Image = checkBox1.Checked ? pics[index].Sd : pics[index].Hd;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            string mygames = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"My Games\TotalMinerDev\Photo\");

            if (!Directory.Exists(mygames))
                return;

            var files = Directory.GetDirectories(mygames);
            for (int index = 0; index < files.Length; index++)
            {
                var temp = new GlobalPic(files[index]);
                if (!temp.IsValid)
                    continue;

                GlobalPics.Add(temp);
                listBox2.Items.Add($"Index: {Path.GetFileName(files[index])}");
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            foreach (var gp in GlobalPics)
            {
                if (!gp.Edited)
                    continue;

                gp.Save();
            }
            button10_Click(null, null);
        }



        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            var index = listBox2.SelectedIndex;

            if (index == -1)
                return;

            pictureBox2.Image = GlobalPics[index].Org;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (listBox2.Items.Count < 0 && GlobalPics.Count < 0)
                return;

            var index = listBox2.SelectedIndex;

            if (index == -1) 
                return;

            using (FileDialog file = new OpenFileDialog() { Filter = "img file|*.png", })
            {
                var dialog = file.ShowDialog();

                if (dialog == DialogResult.OK)
                {
                    GlobalPics[index].Replace(file.FileName);
                    pictureBox2.Image = GlobalPics[index].Org;
                }
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            GlobalPics.Clear();
            listBox2.Items.Clear();
            pictureBox2.Image = null;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            using (ReplaceDialogMenu replaceMenu = new ReplaceDialogMenu())
            {
                var dialog = replaceMenu.ShowDialog();

                if (dialog == DialogResult.OK)
                {
                    foreach (var pic in replaceMenu.Bitmaps)
                    {
                        var path = GetFreePhoto(out int index);
                        Directory.CreateDirectory(path);

                        var gp = new GlobalPic(path, true, index, pic);
                        gp.Save();
                        GlobalPics.Add(gp);
                        listBox2.Items.Add($"Index: {index.ToString().PadLeft(6, '0')}");
                    }
                }
            }
        }

        private string GetFreePhoto(out int Photoindex)
        {
            Photoindex = -1;
            string mygames = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"My Games\TotalMinerDev\Photo\");

            for (int index = 1; index < 255; index++)
            {
                var dir = Path.Combine(mygames, index.ToString().PadLeft(6, '0'));

                if (Directory.Exists(dir))
                    continue;

                Photoindex = index;
                return dir;
            }

            return string.Empty;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            using (FileDialog file = new OpenFileDialog() { Filter = "img file|*.png", })
            {
                var dialog = file.ShowDialog();

                if (dialog == DialogResult.OK)
                {
                    var path = GetFreePhoto(out int index);
                    Directory.CreateDirectory(path);

                    var gp = new GlobalPic(path, true, index, new Bitmap(Bitmap.FromFile(file.FileName)));
                    GlobalPics.Add(gp);
                    listBox2.Items.Add($"Index: {index.ToString().PadLeft(6, '0')}");

                    GlobalPics.Last().Replace(file.FileName);
                    pictureBox2.Image = GlobalPics.Last().Org;
                }
            }
        }
    }
}
