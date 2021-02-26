using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Another_Centurys_Episode_R
{
    public partial class Pallet : Form
    {
        public Pallet()
        {
            InitializeComponent();
        }

        private void Binks(string[] fname)
        {
            for (int i = 0; i < fname.Length; i++)
            {
                DDSPallet.bink(fname[i]);
            }
        }


        private void Pallet_DragDrop(object sender, DragEventArgs e)
        {
            string[] fname = (string[])((System.Array)e.Data.GetData(DataFormats.FileDrop));
            Binks(fname);
        }

        private void Pallet_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Link;
            else e.Effect = DragDropEffects.None;
        }
    }

    static class DDSPallet
    {
        static public void bink(string fname)
        {
            string palletfile = Path.GetDirectoryName(fname) + "\\" + Path.GetFileNameWithoutExtension(fname) + "_Pallet.dds";
            if (!File.Exists(palletfile))
            {
                return;
            }

            BigEndianReader r = new BigEndianReader(File.OpenRead(palletfile));
            byte[] pallets = new byte[256 * 4];
            r.BaseStream.Position = 0x80;
            for (int i = 0; i < 128; i++)
            {
                pallets[i * 8 + 0] = r.ReadByte();
                pallets[i * 8 + 1] = r.ReadByte();
                pallets[i * 8 + 2] = r.ReadByte();
                pallets[i * 8 + 3] = r.ReadByte();

                pallets[i * 8 + 4] = r.ReadByte();
                pallets[i * 8 + 5] = r.ReadByte();
                pallets[i * 8 + 6] = r.ReadByte();
                pallets[i * 8 + 7] = r.ReadByte();

                r.BaseStream.Position += 8;
            }
            r.Close();

            r = new BigEndianReader(File.OpenRead(fname));
            r.BaseStream.Position = 0xc;
            int pw = r.ReadInt32();
            int ph = r.ReadInt32();
            r.BaseStream.Position = 0x80;
            byte[] idxs = r.ReadBytes(pw * ph);
            r.Close();

            BinaryWriter w = new BinaryWriter(File.Create(fname + ".tga"));
            w.Write(0x20000); w.Write(0); w.Write(0);
            w.Write((UInt16)(pw)); w.Write((UInt16)(ph)); w.Write((UInt16)0x2820);
            for (int y = 0; y < ph; y++)
            {
                for (int x = 0; x < pw; x++)
                {
                    int seek = y * pw + x;
                    //if(seek
                    w.Write((byte)(pallets[idxs[seek] * 4 + 1])); //R
                    w.Write((byte)(pallets[idxs[seek] * 4 + 2])); //G
                    w.Write((byte)(pallets[idxs[seek] * 4 + 3])); //B
                    w.Write((byte)(pallets[idxs[seek] * 4 + 0])); //A

                }
            }
            w.Flush();
            w.Close();
        }
    }
}
