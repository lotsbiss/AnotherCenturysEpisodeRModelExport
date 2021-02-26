using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Another_Centurys_Episode_R
{
    static class BNDFILE
    {

        static public bool loadBND(string bhdf, string opath)
        {
            BigEndianReader r = new BigEndianReader(File.OpenRead(bhdf));
            if (r.ReadBInt32() != 1112425524) //BND4
            {
                r.Close();
                return false;
            }
            r.BaseStream.Position = 0xc;
            int filecount = r.ReadBInt32();
            r.BaseStream.Position = 0x24;
            int BHDChunksize = r.ReadBInt32();
            bool r4gb = false;
            if (BHDChunksize == 0x28)
            {
                r4gb = true;
            }

            BHDCHUNK[] chunksinfo = new BHDCHUNK[filecount];

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

            for (int i = 0; i < filecount; i++)
            {
                if (chunksinfo[i].tag != 2)
                {
                    continue;
                }
                r.BaseStream.Position = chunksinfo[i].pos;
                byte[] buf = r.ReadBytes((int)chunksinfo[i].chunksize);
                string oname = opath + "\\" + chunksinfo[i].name;
                Directory.CreateDirectory(Path.GetDirectoryName(oname));
                File.WriteAllBytes(oname, buf);
            }


            r.Close();
            return true;
        }

    }
}
