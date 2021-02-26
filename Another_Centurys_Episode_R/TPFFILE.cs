using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace Another_Centurys_Episode_R
{
    class TPFFILE
    {
        int fcount;
        IMGINFO[] imginfo;
        struct IMGINFO
        {
            public int pos, length, unknow3, nameadr;
            public ushort dxt, unknow1, w, h;
            public Int64 unknow2;
            public string name;
        }

        public void init(string fname, string opth,bool cdir)
        {
            if (cdir)
            {
                expfile(fname, Path.GetDirectoryName(fname));
            }
            else
            {
                expfile(fname, opth);
            }
        }

        private void expfile(string fname, string opth)
        {
            BigEndianReader r = new BigEndianReader(File.OpenRead(fname));
            if (r.ReadBInt32() != 1414546944) //TPF
            {
                r.Close();
                return;
            }

            r.BaseStream.Position = 8;
            fcount = r.ReadBInt32();
            imginfo = new IMGINFO[fcount];
            r.BaseStream.Position = 0x10;

            for (int i = 0; i < fcount; i++)
            {
                imginfo[i].pos = r.ReadBInt32();
                imginfo[i].length = r.ReadBInt32();
                imginfo[i].dxt = r.ReadUInt16();
                imginfo[i].unknow1 = r.ReadBUInt16();
                imginfo[i].w = r.ReadBUInt16();
                imginfo[i].h = r.ReadBUInt16();
                imginfo[i].unknow2 = r.ReadBInt64();
                imginfo[i].nameadr = r.ReadBInt32();
                imginfo[i].unknow3 = r.ReadBInt32();
            }


            for (int i = 0; i < fcount; i++)
            {
                r.BaseStream.Position = imginfo[i].nameadr;
                imginfo[i].name = r.ReadBString();
            }

            for (int i = 0; i < fcount; i++)
            {
                MemoryStream uzip = new MemoryStream();
                BinaryWriter w = new BinaryWriter(uzip);

                int[] DXTheader = new int[32];
                for (int di = 0; di < 32; di++)
                    DXTheader[di] = 0;

                DXTheader[0] = 542327876;
                DXTheader[1] = 124;
                DXTheader[2] = 135175;
                DXTheader[3] = imginfo[i].h;
                DXTheader[4] = imginfo[i].w;
                /*
                if(type==1)
                    DXTheader[5] = 131072;
                else
                    DXTheader[5] = 262144;
                //(=*/
                //DXTheader[7] = 10;
                DXTheader[19] = 32;
                DXTheader[20] = 4;

                if (imginfo[i].dxt != 5)
                    DXTheader[21] = 827611204;  //DXT1
                else// if (imginfo[i].dxt == 5)
                    DXTheader[21] = 894720068;  //DXT5
                DXTheader[27] = 4198408;


                for (int di = 0; di < 32; di++)
                    w.Write(DXTheader[di]);

                r.BaseStream.Position = imginfo[i].pos;
                if (r.ReadBInt32() != 1145262080)
                {
                    r.BaseStream.Position = imginfo[i].pos;
                    w.Write(r.ReadBytes(imginfo[i].length));
                }
                else
                {
                    r.BaseStream.Position = imginfo[i].pos + 0x28;
                    Int64 baseseek = r.BaseStream.Position + 8;
                    r.BaseStream.Position = r.ReadBInt32() + baseseek + 0x18;
                    int rawbufsize = r.ReadBInt32();
                    r.BaseStream.Position += 4;
                    int bufchunknum = r.ReadBInt32();
                    r.BaseStream.Position += 4;

                    DCXCHUNK[] dcxs = new DCXCHUNK[bufchunknum];
                    for (int ti = 0; ti < bufchunknum; ti++)
                    {
                        dcxs[ti].seek = r.ReadBInt64();
                        dcxs[ti].length = r.ReadBInt32();
                        dcxs[ti].tag = r.ReadBInt32();
                    }

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
                }
                w.Flush();
                string oname = opth + "\\" + imginfo[i].name + ".dds";
                Directory.CreateDirectory(Path.GetDirectoryName(oname));
                File.WriteAllBytes(oname, uzip.ToArray());
                w.Close();
            }
            r.Close();
        }


        private byte[] unzip(byte[] datas, int bufsize)
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
    }
}
