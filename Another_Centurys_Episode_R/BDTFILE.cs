using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace Another_Centurys_Episode_R
{
    struct BHDCHUNK
    {
        public int tag;
        public int ffff;
        public Int64 chunksize;
        public Int64 rawsize;
        public Int64 pos;
        public int unknow;
        public string name;
    }

    struct DCXCHUNK
    {
        public Int64 seek;
        public int length, tag;
    }

    class BDTFILE
    {
        
        int filecount;
        int BHDChunksize;
        bool r4gb;
        BHDCHUNK[] chunksinfo;

        public void init(string bdtf, string opth, bool cdir)
        {
            if (loadBHD(bdtf.ToLower().Replace(".bdt", ".bhd")))
            {
                ExpBDT(bdtf, opth, cdir);
            }
        }

        private bool loadBHD(string bhdf)
        {
            BigEndianReader r = new BigEndianReader(File.OpenRead(bhdf));
            if (r.ReadBInt32() != 1112032820) //BHF4
            {
                r.Close();
                return false;
            }
            r.BaseStream.Position = 0xc;
            filecount = r.ReadBInt32();
            r.BaseStream.Position = 0x24;
            BHDChunksize=r.ReadBInt32();
            if (BHDChunksize == 0x28)
            {
                r4gb = true;
            }
            else
            {
                r4gb = false;
            }

            chunksinfo = new BHDCHUNK[filecount];

            r.BaseStream.Position = 0x40;

            for (int i = 0; i < filecount; i++)
            {
                chunksinfo[i].tag = r.ReadInt32();
                chunksinfo[i].ffff = r.ReadBInt32();
                chunksinfo[i].chunksize = r.ReadBInt64();
                chunksinfo[i].rawsize = r.ReadBInt64();
                if (r4gb)
                {
                    chunksinfo[i].pos = r.ReadBInt64();
                }
                else
                {
                    chunksinfo[i].pos = r.ReadBInt32();
                }
                chunksinfo[i].unknow = r.ReadBInt32();
                Int64 nowadr = r.BaseStream.Position + 4;
                r.BaseStream.Position = r.ReadBInt32();
                chunksinfo[i].name = r.ReadBString();
                r.BaseStream.Position = nowadr;
            }
            r.Close();
            return true;
        }

        private byte[] unzip(byte[] datas,int bufsize)
        {
            DeflateStream dzip = new DeflateStream(new MemoryStream(datas), CompressionMode.Decompress);
            int offset = 0;
            int totalCount = 0;
            MemoryStream rt = new MemoryStream();

            byte[] buf = new byte[bufsize + 0x777];

            while (true)
            {
                int bytesRead = dzip.Read(buf, offset, 0x100);
                if (bytesRead == 0)
                {
                    break;
                }
                offset += bytesRead;
                totalCount += bytesRead;
            }

            rt.Write(buf, 0, totalCount);
            rt.Flush();
            dzip.Close();

            return rt.ToArray();

        }

        private void ExpBDT(string bdtf, string opath, bool cdir)
        {
            BigEndianReader r = new BigEndianReader(File.OpenRead(bdtf));
            if (r.ReadBInt32() != 1111770676) //BDF4
            {
                r.Close();
                return;
            }
            for (int i = 0; i < filecount; i++)
            {
                if (chunksinfo[i].tag != 3)
                {
                    continue;
                }
                MemoryStream uzip = new MemoryStream();
                BinaryWriter w = new BinaryWriter(uzip);

                r.BaseStream.Position = chunksinfo[i].pos + 0x5c;
                int rawbufsize = r.ReadBInt32();
                r.BaseStream.Position += 8;
                int bufchunknum = r.ReadBInt32();
                r.BaseStream.Position += 4;

                DCXCHUNK[] dcxs = new DCXCHUNK[bufchunknum];
                for (int ti = 0; ti < bufchunknum; ti++)
                {
                    dcxs[ti].seek = r.ReadBInt64();
                    dcxs[ti].length = r.ReadBInt32();
                    dcxs[ti].tag = r.ReadBInt32();
                }
                Int64 baseseek = r.BaseStream.Position;

                for (int ti = 0; ti < bufchunknum; ti++)
                {
                    r.BaseStream.Position = dcxs[ti].seek + baseseek;
                    byte[] tbuf = r.ReadBytes(dcxs[ti].length);
                    if (dcxs[ti].tag == 1)
                    {
                        w.Write(unzip(tbuf, rawbufsize));
                    }
                    else
                    {
                        w.Write(tbuf);
                    }
                }
                w.Flush();
                string oname = opath + "\\";
                if (cdir)
                {
                    oname = oname + Path.GetFileNameWithoutExtension(bdtf) + "\\";
                }
                oname = oname + chunksinfo[i].name;
                Directory.CreateDirectory(Path.GetDirectoryName(oname));
                File.WriteAllBytes(oname, uzip.ToArray());
                w.Close();
            }
            r.Close();


        }
    }
}
