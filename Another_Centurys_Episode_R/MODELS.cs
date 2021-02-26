using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Another_Centurys_Episode_R
{
    class MODELS
    {
        public MODELS()
        { 
        }

        struct MATRIXS
        {
            public float[] row;
        };

        struct FACE
        {
            public int x, y, z;
            public FACE(int a, int b, int c)
            {
                x = a; y = b; z = c;
            }
        }


        struct VT3
        {
            public float x, y, z;
        };

        struct CHUNKS
        {
            public int faceaddr;
            public int facelenth;
            public int vertaddr;
            public int vertlenth;
            public int vertnum;
        };

        double[] juzhen3x3(double[] a, double[] b)
        {
            double[] tmp = new double[9];
            for (int y = 0; y < 3; y++)
                for (int x = 0; x < 3; x++)
                    tmp[y * 3 + x] = a[y * 3 + 0] * b[0 * 3 + x] + a[y * 3 + 1] * b[1 * 3 + x] + a[y * 3 + 2] * b[2 * 3 + x];
            return tmp;
        }

        float[] rotjuzhen(float[] rot, float[] eura)
        {
            double[] x = new double[9];
            double[] y = new double[9];
            double[] z = new double[9];
            x[0 * 3 + 0] = 1;                   x[0 * 3 + 1] = 0;                       x[0 * 3 + 2] = 0;
            x[1 * 3 + 0] = 0;                   x[1 * 3 + 1] = Math.Cos(eura[0]);       x[1 * 3 + 2] = Math.Sin(eura[0]);
            x[2 * 3 + 0] = 0;                   x[2 * 3 + 1] = -Math.Sin(eura[0]);      x[2 * 3 + 2] = Math.Cos(eura[0]);

            y[0 * 3 + 0] = Math.Cos(eura[1]);   y[0 * 3 + 1] = 0;                       y[0 * 3 + 2] = -Math.Sin(eura[1]);
            y[1 * 3 + 0] = 0;                   y[1 * 3 + 1] = 1;                       y[1 * 3 + 2] = 0;
            y[2 * 3 + 0] = Math.Sin(eura[1]);   y[2 * 3 + 1] = 0;                       y[2 * 3 + 2] = Math.Cos(eura[1]);

            z[0 * 3 + 0] = Math.Cos(eura[2]);   z[0 * 3 + 1] = Math.Sin(eura[2]);       z[0 * 3 + 2] = 0;
            z[1 * 3 + 0] = -Math.Sin(eura[2]);  z[1 * 3 + 1] = Math.Cos(eura[2]);       z[1 * 3 + 2] = 0;
            z[2 * 3 + 0] = 0;                   z[2 * 3 + 1] = 0;                       z[2 * 3 + 2] = 1;

            x = juzhen3x3(x, y);
            x = juzhen3x3(x, z);

            for (int yy = 0; yy < 3; yy++)
                for (int xx = 0; xx < 3; xx++)
                    rot[yy * 3 + xx] = (float)x[yy * 3 + xx];

            return rot;
        }

        VT3 juzhen(VT3 vt, float[] rot)
        {
            VT3 tmvt;
            tmvt.x = vt.x * rot[0 * 3 + 0] + vt.y * rot[1 * 3 + 0] + vt.z * rot[2 * 3 + 0];
            tmvt.y = vt.x * rot[0 * 3 + 1] + vt.y * rot[1 * 3 + 1] + vt.z * rot[2 * 3 + 1];
            tmvt.z = vt.x * rot[0 * 3 + 2] + vt.y * rot[1 * 3 + 2] + vt.z * rot[2 * 3 + 2];
            return tmvt;
        }


        VT3 juzhen(VT3 vt, float[] rot,float weigth)
        {
            VT3 tmvt;
            tmvt.x = vt.x * rot[0 * 3 + 0] + vt.y * rot[1 * 3 + 0] + vt.z * rot[2 * 3 + 0];
            tmvt.y = vt.x * rot[0 * 3 + 1] + vt.y * rot[1 * 3 + 1] + vt.z * rot[2 * 3 + 1];
            tmvt.z = vt.x * rot[0 * 3 + 2] + vt.y * rot[1 * 3 + 2] + vt.z * rot[2 * 3 + 2];
            tmvt.x = vt.x * (1.0f - weigth) + tmvt.x * (weigth);
            tmvt.y = vt.y * (1.0f - weigth) + tmvt.y * (weigth);
            tmvt.z = vt.z * (1.0f - weigth) + tmvt.z * (weigth);
            return tmvt;
        }

        public void expModel(string infile, string outpath, bool sdir)
        {
            if (sdir)
            {
                ExpModel(infile, Path.GetDirectoryName(infile));
            }
            else
            {
                ExpModel(infile, outpath);
            }
        }

        public void ExpModel(string infile, string outpath)
        {
            //StreamWriter o=new StreamWriter();

            BigEndianReader r = new BigEndianReader(File.OpenRead(infile));

            //int point[3];

            byte[] head = new byte[4];
            head = r.ReadBytes(4);
            if (!(head[0] == 'F' && head[1] == 'L' && head[2] == 'V' && head[3] == 'E'))
            {
                return;
            }
            int baseseek = 0;
            r.BaseStream.Seek(0xc, SeekOrigin.Begin);

            baseseek = r.ReadBInt32();

            r.BaseStream.Seek(0x14, SeekOrigin.Begin);
            int[] cnnum = new int[5];
            cnnum[0] = r.ReadBInt32(); cnnum[1] = r.ReadBInt32(); cnnum[2] = r.ReadBInt32(); cnnum[3] = r.ReadBInt32(); cnnum[4] = r.ReadBInt32();
            r.BaseStream.Seek(0x80, SeekOrigin.Begin);


            //r.BaseStream.Seek(0x40 * cnnum[0], SeekOrigin.Current);
            //*
            VT3[] dspos = new VT3[cnnum[0]];
            int[] dsposid = new int[cnnum[0]];
            for (int i = 0; i < cnnum[0]; i++)
            {
                dspos[i].x = r.ReadBFloat();
                dspos[i].y = r.ReadBFloat();
                dspos[i].z = r.ReadBFloat();
                r.BaseStream.Seek(0x12, SeekOrigin.Current);
                dsposid[i] = r.ReadBInt16();
                r.BaseStream.Seek(0x20, SeekOrigin.Current);

            }
            //*/

            r.BaseStream.Seek(0x20 * cnnum[1], SeekOrigin.Current);

            VT3[] cpos = new VT3[cnnum[2] + 1];
            MATRIXS[] crot = new MATRIXS[cnnum[2]];
            int[] partnum = new int[cnnum[2] + 1];
            int[] partnameaddr = new int[cnnum[2]];
            int[] tmids = new int[cnnum[2]];
            string[] partname = new string[cnnum[2]];
            for (int i = 0; i < cnnum[2]; i++)
            {

                cpos[i].x = r.ReadBFloat();
                cpos[i].y = r.ReadBFloat();
                cpos[i].z = r.ReadBFloat();

                partnameaddr[i] = r.ReadBInt32();
                Int64 nowaddr = r.BaseStream.Position;
                r.BaseStream.Seek(partnameaddr[i], SeekOrigin.Begin);
                partname[i] = r.ReadFLVString();
                r.BaseStream.Seek(nowaddr, SeekOrigin.Begin);

                float[] tmrot = new float[3];
                tmrot[0] = -r.ReadBFloat();
                tmrot[2] = -r.ReadBFloat();
                tmrot[1] = -r.ReadBFloat();

                crot[i].row = new float[9];
                crot[i].row = rotjuzhen(crot[i].row, tmrot);
                ushort tmid = 0xffff;
                tmid = r.ReadBUInt16();
                r.BaseStream.Seek(0x62, SeekOrigin.Current);

                tmids[i] = tmid;

                if (tmid != 0xffff)
                {
                    cpos[i].x += cpos[tmid].x;
                    cpos[i].y += cpos[tmid].y;
                    cpos[i].z += cpos[tmid].z;
                }

            }
            int[] posixs = new int[7700];
            int[] posggp = new int[cnnum[3] + 1];
            posggp[0] = 0;
            int idxs = 0;
            CHUNKS[] chunk = new CHUNKS[cnnum[3]];
            for (int i = 0; i < cnnum[3]; i++)
            {
                r.BaseStream.Seek(0x8, SeekOrigin.Current);
                chunk[i].vertnum = r.ReadBInt32();

                r.BaseStream.Seek(0x3c-2, SeekOrigin.Current);
                int tccccount = r.ReadBInt16();
                int paddr = (int)r.BaseStream.Position;
                posggp[i + 1] =posggp[i ]+tccccount;

                r.BaseStream.Seek(paddr - 0x3c + 2, SeekOrigin.Begin);
                ushort tmidx = 0;
                tmidx = r.ReadBUInt16();
                int idxscount = 0;
                /*
                while (idxscount == 0 || (tmidx != 0xffff && tmidx != 0))
                {
                    idxscount++;
                    posixs[idxs++] = tmidx;
                    tmidx = r.ReadBUInt16();
                }
                //*/
                for (int ttti = 0; ttti < tccccount; ttti++)
                {
                    idxscount++;
                    posixs[idxs++] = tmidx;
                    tmidx = r.ReadBUInt16();
                }
                r.BaseStream.Seek(paddr, SeekOrigin.Begin);

                chunk[i].facelenth = r.ReadBInt32();
                chunk[i].faceaddr = r.ReadBInt32() + baseseek;
                chunk[i].vertlenth = r.ReadBInt32();
                chunk[i].vertaddr = r.ReadBInt32() + baseseek;

                r.BaseStream.Seek(0xc, SeekOrigin.Current);
            }

            string outfile = (outpath + "\\" + Path.GetFileNameWithoutExtension(infile) + ".exp" + ".obj");

            BinaryWriter ww = new BinaryWriter(File.Create(outfile + ".link"));
            ww.Write(cnnum[0]);
            for (int vi = 0; vi < cnnum[0]; vi++)
            {
                ww.Write(dsposid[vi]);
                ww.Write(dspos[vi].x + cpos[dsposid[vi]].x);
                ww.Write(dspos[vi].y + cpos[dsposid[vi]].y);
                ww.Write(dspos[vi].z + cpos[dsposid[vi]].z);
            }
            

            ww.Write(cnnum[2]);
            for (int vi = 0; vi < cnnum[2]; vi++)
            {
                ww.Write((byte)Encoding.ASCII.GetByteCount(partname[vi]));
                ww.Write(Encoding.ASCII.GetBytes(partname[vi]));
                ww.Write(tmids[vi]);
                ww.Write(cpos[vi].x);
                ww.Write(cpos[vi].y);
                ww.Write(cpos[vi].z);
            }

            ww.Flush();
            ww.Close();

            int potgup = -1;
            int potmax = 0;
            partnum[0] = 0;
            int nowpart = 0;

            StreamWriter o = new StreamWriter(outfile);
            int facebegin = 0;
            int facemax = 0;
            for (int vi = 0; vi < cnnum[3]; vi++)
            //for (int vi = 4; vi <0x5; vi++)
            {
                if (chunk[vi].vertlenth / chunk[vi].vertnum == 0x20)
                {
                    potgup = posggp[vi];
                    potmax = 0;
                    byte debg = 0;

                    r.BaseStream.Seek(chunk[vi].vertaddr, SeekOrigin.Begin);
                    int[] pppatrs = new int[chunk[vi].vertnum];
                    for (int i = 0; i < chunk[vi].vertnum; i++)
                    {

                        VT3 ppos;
                        ppos.x = r.ReadBFloat();
                        ppos.z = r.ReadBFloat();
                        ppos.y = r.ReadBFloat();


                        debg = r.ReadByte();
                        //debg = 0;
                        pppatrs[i] = debg;
                        if (nowpart != debg)
                        {
                            partnum[debg] = i;
                            nowpart = debg;
                        }
                        if (debg > potmax)
                        {
                            potmax = debg ;
                        }

                        r.BaseStream.Seek(0x13, SeekOrigin.Current);


                        ppos = juzhen(ppos, crot[posixs[potgup + debg]].row);

                        o.WriteLine("v {0} {1} {2} ",
                            ((ppos.x) + cpos[posixs[potgup + debg]].x).ToString(),
                            ((ppos.y) + cpos[posixs[potgup + debg]].z).ToString(),
                            ((ppos.z) + cpos[posixs[potgup + debg]].y).ToString());

                    }
                    o.WriteLine("");

                    r.BaseStream.Seek(chunk[vi].vertaddr, SeekOrigin.Begin);

                    float[] vts = new float[2];

                    for (int i = 0; i < chunk[vi].vertnum; i++)
                    {
                        r.BaseStream.Seek(0x18, SeekOrigin.Current);
                        vts[0] = r.ReadBFloat();
                        vts[1] = r.ReadBFloat();
                        r.BaseStream.Seek(0x0, SeekOrigin.Current);
                        o.WriteLine("vt {0} {1} ", vts[0], vts[1]);
                    }
                    o.WriteLine("");

                    ushort[] ts = new ushort[3];
                    int fcot = 0;
                    bool inverse = false;

                    r.BaseStream.Seek(chunk[vi].faceaddr, SeekOrigin.Begin);
                    partnum[nowpart] = -1;
                    nowpart = 0;

                    List<FACE>[] facess = new List<FACE>[potmax+1];
                    for (int fi = 0; fi < potmax+1; fi++)
                    {
                        facess[fi] = new List<FACE>();
                    }

                    //o.WriteLine("g {0} ",partname[posixs[potgup]]);
                    while (r.BaseStream.Position < chunk[vi].faceaddr + chunk[vi].facelenth)
                    {
                        ts[0] = r.ReadBUInt16();
                        if (ts[0] == partnum[nowpart])
                        {
                            //o.WriteLine("g {0} ", partname[posixs[potgup + nowpart]]);
                            nowpart++;
                        }
                        if (ts[0] + 1 > facemax && ts[0] != 0xffff)
                            facemax = ts[0] + 1;

                        fcot++;
                        if (ts[0] == 0xffff)
                        {
                            fcot = 0;
                            inverse = false;
                        }
                        else if (fcot == 3)
                        {
                            if (inverse)
                            {
                                //o.WriteLine("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2} ",
                                    //ts[0] + 1 + facebegin, ts[1] + facebegin, ts[2] + facebegin);
                                facess[pppatrs[ts[0]]].Add(new FACE(ts[0] + 1 + facebegin, ts[1] + facebegin, ts[2] + facebegin));
                            }
                            else
                            {
                                //o.WriteLine("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2} ",
                                        //ts[2] + facebegin, ts[1] + facebegin, ts[0] + 1 + facebegin);
                                facess[pppatrs[ts[0]]].Add(new FACE(ts[2] + facebegin, ts[1] + facebegin, ts[0] + 1 + facebegin));
                            }
                            fcot--;
                            inverse = !inverse;
                        }
                        {
                            if (ts[1] == ts[0] + 1 && fcot != 1)
                            {
                                fcot--;
                                inverse = true;
                            }
                            ts[2] = ts[1];
                            ts[1] = (ushort)(ts[0] + 1);
                        }
                    } 
                    for (int fi = 0; fi < potmax + 1; fi++)
                    {
                        o.WriteLine("g {0} ", partname[posixs[potgup + fi]]);
                        for (int ffi = 0; ffi < facess[fi].Count; ffi++)
                        {
                            o.WriteLine("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2} ",
                                        facess[fi][ffi].x, facess[fi][ffi].y, facess[fi][ffi].z);
                        }
                    }
                    facebegin += facemax;
                    facemax = 0;
                }
                else if (chunk[vi].vertlenth / chunk[vi].vertnum == 0x28)
                {
                    potgup = posggp[vi];
                    potmax = 0;
                    byte debg = 0;

                    r.BaseStream.Seek(chunk[vi].vertaddr, SeekOrigin.Begin);
                    int[] pppatrs = new int[chunk[vi].vertnum];
                    for (int i = 0; i < chunk[vi].vertnum; i++)
                    {

                        VT3 ppos;
                        ppos.x = r.ReadBFloat();
                        ppos.z = r.ReadBFloat();
                        ppos.y = r.ReadBFloat();

                        byte[] tweg = r.ReadBytes(4);
                        float we1 = 0, we2 = 0, we3 = 0, we4 = 0;
                        r.BaseStream.Seek(0xc, SeekOrigin.Current);
                        if (r.ReadBInt32() == -1)
                        {
                            r.BaseStream.Seek(-0x10, SeekOrigin.Current);
                            we1 = r.ReadBUInt16() / 32767.0f;
                            we2 = r.ReadBUInt16() / 32767.0f;
                            we3 = r.ReadBUInt16() / 32767.0f;
                            we4 = r.ReadBUInt16() / 32767.0f;
                            r.BaseStream.Seek(0x18 - 8, SeekOrigin.Current);

                            float debugweigth = we1 + we2 + we3 + we4;
                            if (debugweigth == 0.0f)
                            {
                                we1 = 1.0f;
                            }
                            else
                            {
                                we2 = 0; we3 = 0; we4 = 0; we1 = 0.0f;
                            }
                        }
                        else
                        {
                            we2 = 0;
                            we3 = 0;
                            we4 = 0;
                            we1 = 1;
                            r.BaseStream.Seek(0x8, SeekOrigin.Current);
                        }

                        //debg = r.ReadByte();
                        debg = tweg[0];
                        pppatrs[i] = debg;
                        if (nowpart != debg)
                        {
                            partnum[debg] = i;
                            nowpart = debg;
                        }
                        if (debg > potmax)
                        {
                            potmax = debg;
                        }




                        //if (we2 > 0.0f)
                        {
                           // we2 = 0; we3 = 0; we4 = 0; we1 = 0.0f;
                        }

                        ppos = juzhen(ppos, crot[posixs[potgup + tweg[0]]].row, we1);
                        //ppos = juzhen(ppos, crot[posixs[potgup + tweg[1]]].row, we2);
                        //ppos = juzhen(ppos, crot[posixs[potgup + tweg[2]]].row, we3);
                        //ppos = juzhen(ppos, crot[posixs[potgup + tweg[3]]].row, we4);

                        ppos.x = ppos.x
                            + cpos[posixs[potgup + tweg[0]]].x * we1;
                            //+ cpos[posixs[potgup + tweg[1]]].x * we2
                            //+ cpos[posixs[potgup + tweg[2]]].x * we3
                            //+ cpos[posixs[potgup + tweg[3]]].x * we4;

                        ppos.z = ppos.z
                            + cpos[posixs[potgup + tweg[0]]].y * we1;
                            //+ cpos[posixs[potgup + tweg[1]]].y * we2
                            //+ cpos[posixs[potgup + tweg[2]]].y * we3
                            //+ cpos[posixs[potgup + tweg[3]]].y * we4;

                        ppos.y = ppos.y
                            + cpos[posixs[potgup + tweg[0]]].z * we1;
                            //+ cpos[posixs[potgup + tweg[1]]].z * we2
                            //+ cpos[posixs[potgup + tweg[2]]].z * we3
                           // + cpos[posixs[potgup + tweg[3]]].z * we4;


                        o.WriteLine("v {0} {1} {2} ",
                            ((ppos.x)).ToString(),
                            ((ppos.y)).ToString(),
                            ((ppos.z)).ToString());

                        /*
                        o.WriteLine("v {0} {1} {2} ",
                            ((ppos.x) + cpos[posixs[potgup + debg]].x).ToString(),
                            ((ppos.y) + cpos[posixs[potgup + debg]].z).ToString(),
                            ((ppos.z) + cpos[posixs[potgup + debg]].y).ToString());
                        //*/

                    }
                    o.WriteLine("");

                    r.BaseStream.Seek(chunk[vi].vertaddr, SeekOrigin.Begin);

                    float[] vts = new float[2];

                    r.BaseStream.Seek(0x14, SeekOrigin.Current);
                    if (r.ReadBInt32() == -1)
                    {
                        r.BaseStream.Seek(chunk[vi].vertaddr, SeekOrigin.Begin);

                        for (int i = 0; i < chunk[vi].vertnum; i++)
                        {
                            r.BaseStream.Seek(0x18, SeekOrigin.Current);
                            vts[0] = r.ReadBFloat();
                            vts[1] = r.ReadBFloat();
                            r.BaseStream.Seek(0x8, SeekOrigin.Current);
                            o.WriteLine("vt {0} {1} ", vts[0], vts[1]);
                        }
                    }
                    else
                    {
                        r.BaseStream.Seek(chunk[vi].vertaddr, SeekOrigin.Begin);
                        for (int i = 0; i < chunk[vi].vertnum; i++)
                        {
                            r.BaseStream.Seek(0x20, SeekOrigin.Current);
                            vts[0] = r.ReadBFloat();
                            vts[1] = r.ReadBFloat();
                            r.BaseStream.Seek(0x0, SeekOrigin.Current);
                            o.WriteLine("vt {0} {1} ", vts[0], vts[1]);
                        }
                    }
                    o.WriteLine("");

                    ushort[] ts = new ushort[3];
                    int fcot = 0;
                    bool inverse = false;

                    r.BaseStream.Seek(chunk[vi].faceaddr, SeekOrigin.Begin);
                    partnum[nowpart] = -1;
                    nowpart = 0;

                    List<FACE>[] facess = new List<FACE>[potmax + 1];
                    for (int fi = 0; fi < potmax + 1; fi++)
                    {
                        facess[fi] = new List<FACE>();
                    }

                    //o.WriteLine("g {0} ",partname[posixs[potgup]]);
                    while (r.BaseStream.Position < chunk[vi].faceaddr + chunk[vi].facelenth)
                    {
                        ts[0] = r.ReadBUInt16();
                        if (ts[0] == partnum[nowpart])
                        {
                            //o.WriteLine("g {0} ", partname[posixs[potgup + nowpart]]);
                            nowpart++;
                        }
                        if (ts[0] + 1 > facemax && ts[0] != 0xffff)
                            facemax = ts[0] + 1;

                        fcot++;
                        if (ts[0] == 0xffff)
                        {
                            fcot = 0;
                            inverse = false;
                        }
                        else if (fcot == 3)
                        {
                            if (inverse)
                            {
                                //o.WriteLine("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2} ",
                                //ts[0] + 1 + facebegin, ts[1] + facebegin, ts[2] + facebegin);
                                facess[pppatrs[ts[0]]].Add(new FACE(ts[0] + 1 + facebegin, ts[1] + facebegin, ts[2] + facebegin));
                            }
                            else
                            {
                                //o.WriteLine("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2} ",
                                //ts[2] + facebegin, ts[1] + facebegin, ts[0] + 1 + facebegin);
                                facess[pppatrs[ts[0]]].Add(new FACE(ts[2] + facebegin, ts[1] + facebegin, ts[0] + 1 + facebegin));
                            }
                            fcot--;
                            inverse = !inverse;
                        }
                        {
                            if (ts[1] == ts[0] + 1 && fcot != 1)
                            {
                                fcot--;
                                inverse = true;
                            }
                            ts[2] = ts[1];
                            ts[1] = (ushort)(ts[0] + 1);
                        }
                    }
                    for (int fi = 0; fi < potmax + 1; fi++)
                    {
                        o.WriteLine("g {0} ", partname[posixs[potgup + fi]]);
                        for (int ffi = 0; ffi < facess[fi].Count; ffi++)
                        {
                            o.WriteLine("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2} ",
                                        facess[fi][ffi].x, facess[fi][ffi].y, facess[fi][ffi].z);
                        }
                    }
                    facebegin += facemax;
                    facemax = 0;
                }


                else if (chunk[vi].vertlenth / chunk[vi].vertnum == 0x18)
                {
                    potgup = potgup + potmax + 1;
                    potmax = 0;
                    byte debg = 0;

                    r.BaseStream.Seek(chunk[vi].vertaddr, SeekOrigin.Begin);

                    for (int i = 0; i < chunk[vi].vertnum; i++)
                    {
                        //r.BaseStream.Seek(0x4, SeekOrigin.Current);
                        VT3 ppos;
                        ppos.x = r.ReadBFloat();
                        ppos.z = r.ReadBFloat();
                        ppos.y = r.ReadBFloat();


                        debg = r.ReadByte();
                        if (nowpart != debg && debg < cnnum[2])
                        {
                            partnum[debg] = i;
                            nowpart = debg;
                        }
                        if (debg > potmax && debg < cnnum[2])
                        {
                            potmax = debg;
                        }

                        r.BaseStream.Seek(0xb, SeekOrigin.Current);
                        //while (r.ReadBUInt32() != 0xffffffff) ;

                        ppos = juzhen(ppos, crot[posixs[potgup + debg]].row);

                        o.WriteLine("v {0} {1} {2} ",
                            ((ppos.x) + cpos[posixs[potgup + debg]].x).ToString(),
                            ((ppos.y) + cpos[posixs[potgup + debg]].z).ToString(),
                            ((ppos.z) + cpos[posixs[potgup + debg]].y).ToString());

                    }
                    o.WriteLine("");

                    r.BaseStream.Seek(chunk[vi].vertaddr, SeekOrigin.Begin);

                    short[] vts = new short[2];

                    for (int i = 0; i < chunk[vi].vertnum; i++)
                    {
                        r.BaseStream.Seek(0x14, SeekOrigin.Current);
                        vts[0] = (short)(r.ReadBInt16() & 0xffff);
                        vts[1] = (short)(r.ReadBInt16() & 0xffff);
                        //r.BaseStream.Seek(0x4, SeekOrigin.Current);
                        o.WriteLine("vt {0} {1} ",
                            ((float)(vts[0]) / 2048.0f),
                            (1.0 - (float)(vts[1]) / 2048.0f));
                    }
                    o.WriteLine("");

                    ushort[] ts = new ushort[3];
                    int fcot = 0;
                    bool inverse = false;

                    r.BaseStream.Seek(chunk[vi].faceaddr, SeekOrigin.Begin);
                    partnum[nowpart + 1] = -1;
                    nowpart = 0;
                    o.WriteLine("g {0} ", partname[posixs[potgup]]);
                    while (r.BaseStream.Position < chunk[vi].faceaddr + chunk[vi].facelenth)
                    {
                        ts[0] = r.ReadBUInt16();
                        if (ts[0] == partnum[nowpart])
                        {
                            // o.WriteLine("g {0}_{1}_{2} ", vi, posixs[potgup + nowpart++], partname[posixs[potgup + nowpart - 1]]);
                            o.WriteLine("g {0} ", partname[posixs[potgup + nowpart]]);
                            nowpart++;
                        }
                        if (ts[0] + 1 > facemax && ts[0] != 0xffff)
                            facemax = ts[0] + 1;

                        fcot++;
                        if (ts[0] == 0xffff)
                        {
                            fcot = 0;
                            inverse = false;
                        }
                        else if (fcot == 3)
                        {
                            if (inverse)
                                o.WriteLine("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2} ",
                                    ts[0] + 1 + facebegin, ts[1] + facebegin, ts[2] + facebegin);
                            else
                                o.WriteLine("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2} ",
                                        ts[2] + facebegin, ts[1] + facebegin, ts[0] + 1 + facebegin);
                            fcot--;
                            inverse = !inverse;
                        }
                        {
                            if (ts[1] == ts[0] + 1 && fcot != 1)
                            {
                                fcot--;
                                inverse = true;
                            }
                            ts[2] = ts[1];
                            ts[1] = (ushort)(ts[0] + 1);
                        }
                    }
                    facebegin += facemax;
                    facemax = 0;
                }
                else if (chunk[vi].vertlenth / chunk[vi].vertnum == 0x1c)
                {
                    potgup = potgup + potmax + 1;
                    potmax = 0;
                    byte debg = 0;

                    r.BaseStream.Seek(chunk[vi].vertaddr, SeekOrigin.Begin);

                    for (int i = 0; i < chunk[vi].vertnum; i++)
                    {
                        //r.BaseStream.Seek(0x4, SeekOrigin.Current);
                        VT3 ppos;
                        ppos.x = r.ReadBFloat();
                        ppos.z = r.ReadBFloat();
                        ppos.y = r.ReadBFloat();


                        debg = r.ReadByte();
                        if (nowpart != debg && debg < cnnum[2])
                        {
                            partnum[debg] = i;
                            nowpart = debg;
                        }
                        if (debg > potmax && debg < cnnum[2])
                        {
                            potmax = debg;
                        }

                        r.BaseStream.Seek(0xf, SeekOrigin.Current);
                        //while (r.ReadBUInt32() != 0xffffffff) ;

                        ppos = juzhen(ppos, crot[posixs[potgup + debg]].row);

                        o.WriteLine("v {0} {1} {2} ",
                            ((ppos.x) + cpos[posixs[potgup + debg]].x).ToString(),
                            ((ppos.y) + cpos[posixs[potgup + debg]].z).ToString(),
                            ((ppos.z) + cpos[posixs[potgup + debg]].y).ToString());

                    }
                    o.WriteLine("");

                    r.BaseStream.Seek(chunk[vi].vertaddr, SeekOrigin.Begin);

                    short[] vts = new short[2];

                    for (int i = 0; i < chunk[vi].vertnum; i++)
                    {
                        r.BaseStream.Seek(0x18, SeekOrigin.Current);
                        vts[0] = (short)(r.ReadBInt16() & 0xffff);
                        vts[1] = (short)(r.ReadBInt16() & 0xffff);
                        //r.BaseStream.Seek(0x4, SeekOrigin.Current);
                        o.WriteLine("vt {0} {1} ",
                            ((float)(vts[0]) / 2048.0f),
                            (1.0 - (float)(vts[1]) / 2048.0f));
                    }
                    o.WriteLine("");

                    ushort[] ts = new ushort[3];
                    int fcot = 0;
                    bool inverse = false;

                    r.BaseStream.Seek(chunk[vi].faceaddr, SeekOrigin.Begin);
                    partnum[nowpart + 1] = -1;
                    nowpart = 0;
                    o.WriteLine("g {0} ", partname[posixs[potgup]]);
                    while (r.BaseStream.Position < chunk[vi].faceaddr + chunk[vi].facelenth)
                    {

                        ts[0] = r.ReadBUInt16();
                        if (ts[0] == partnum[nowpart])
                        {
                            //o.WriteLine("g {0}_{1}_{2} ", vi, posixs[potgup + nowpart++], partname[posixs[potgup + nowpart - 1]]);
                            o.WriteLine("g {0} ", partname[posixs[potgup + nowpart]]);
                            nowpart++;
                        }

                        if (ts[0] + 1 > facemax && ts[0] != 0xffff)
                            facemax = ts[0] + 1;

                        fcot++;
                        if (ts[0] == 0xffff)
                        {
                            fcot = 0;
                            inverse = false;
                        }
                        else if (fcot == 3)
                        {
                            if (inverse)
                                o.WriteLine("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2} ",
                                    ts[0] + 1 + facebegin, ts[1] + facebegin, ts[2] + facebegin);
                            else
                                o.WriteLine("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2} ",
                                        ts[2] + facebegin, ts[1] + facebegin, ts[0] + 1 + facebegin);
                            fcot--;
                            inverse = !inverse;
                        }
                        {
                            if (ts[1] == ts[0] + 1 && fcot != 1)
                            {
                                fcot--;
                                inverse = true;
                            }
                            ts[2] = ts[1];
                            ts[1] = (ushort)(ts[0] + 1);
                        }
                    }

                    facebegin += facemax;
                    facemax = 0;
                }
                else
                    break;
                o.WriteLine("");
            }
            o.Flush();
            o.Close();
            r.Close();

        }

    }
}
