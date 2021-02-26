using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Another_Centurys_Episode_R
{
    class BigEndianReader : BinaryReader
    {
        Stream readnow;
        public BigEndianReader(FileStream InputStream)
            : base(InputStream)
        {
            readnow = InputStream;
        }

        public Int16 ReadBInt16()
        {
            byte[] data = new byte[2];
            readnow.Read(data, 0, 2);
            byte swt;
            
            swt = data[0];
            data[0] = data[1];
            data[1] = swt;

            return BitConverter.ToInt16(data, 0);
        }

        public Int32 ReadBInt32()
        {
            byte[] data = new byte[4];
            readnow.Read(data, 0, 4);
            byte swt;

            swt = data[0];
            data[0] = data[3];
            data[3] = swt;

            swt = data[1];
            data[1] = data[2];
            data[2] = swt;

            return BitConverter.ToInt32(data, 0);
        }
        public UInt16 ReadBUInt16()
        {
            byte[] data = new byte[2];
            readnow.Read(data, 0, 2);
            byte swt;

            swt = data[0];
            data[0] = data[1];
            data[1] = swt;

            return BitConverter.ToUInt16(data, 0);
        }

        public UInt32 ReadBUInt32()
        {
            byte[] data = new byte[4];
            readnow.Read(data, 0, 4);
            byte swt;

            swt = data[0];
            data[0] = data[3];
            data[3] = swt;

            swt = data[1];
            data[1] = data[2];
            data[2] = swt;

            return BitConverter.ToUInt32(data, 0);
        }

        public UInt64 ReadBUInt64()
        {
            byte[] data = new byte[8];
            readnow.Read(data, 0, 8);
            byte swt;

            swt = data[0];
            data[0] = data[7];
            data[7] = swt;

            swt = data[1];
            data[1] = data[6];
            data[6] = swt;

            swt = data[2];
            data[2] = data[5];
            data[5] = swt;

            swt = data[3];
            data[3] = data[4];
            data[4] = swt;

            return BitConverter.ToUInt64(data, 0);
        }

        public Int64 ReadBInt64()
        {
            byte[] data = new byte[8];
            readnow.Read(data, 0, 8);
            byte swt;

            swt = data[0];
            data[0] = data[7];
            data[7] = swt;

            swt = data[1];
            data[1] = data[6];
            data[6] = swt;

            swt = data[2];
            data[2] = data[5];
            data[5] = swt;

            swt = data[3];
            data[3] = data[4];
            data[4] = swt;

            return BitConverter.ToInt64(data, 0);
        }


        public float ReadBFloat()
        {
            byte[] data = new byte[4];
            readnow.Read(data, 0, 4);
            byte swt;

            swt = data[0];
            data[0] = data[3];
            data[3] = swt;

            swt = data[1];
            data[1] = data[2];
            data[2] = swt;

            return BitConverter.ToSingle(data, 0);
        }

        public byte[] ReadXPR16()
        {
            byte[] data = new byte[16];
            readnow.Read(data, 0, 16);
            byte swt;

            for (int i = 0; i < 8; i++)
            {
                swt = data[i * 2];
                data[i * 2] = data[i * 2 + 1];
                data[i * 2 + 1] = swt;
            }

            return data;
        }


        public string ReadBString()
        {
            int count = 1;
            while (this.ReadByte() != 0)
            {
                count++;
            }
            this.BaseStream.Position -= count;

            return Encoding.ASCII.GetString(this.ReadBytes(count - 1));
        }

        public string ReadFLVString()
        {
            int count = 1;
            while (this.ReadBUInt16() != 0)
            {
                count++;
            }
            this.BaseStream.Position -= count * 2;
            char[] t = new char[count - 1];
            for (int i = 0; i < count - 1; i++)
            {
                t[i] = (char)this.ReadBUInt16();
            }

            return new string(t);
        }

    }
}
