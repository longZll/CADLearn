using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace ReadSummaryInfo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ReadSummary("C:\\test.dwg");
        }
        public static void ReadSummary(string filename)
        {
            using (FileStream stream=new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using(BinaryReader reader=new BinaryReader(stream))
            {
                string versionId=new string(reader.ReadChars(6));                
                bool isUnicode=true;
                switch (versionId)
                {
                    case "AC1018":
                        isUnicode = false;
                        break;
                    case "AC1021":
                    case "AC1024":
                        isUnicode = true;
                        break;
                }
                stream.Seek(0x20, SeekOrigin.Begin);
                uint address=reader.ReadUInt32();
                stream.Seek(address, SeekOrigin.Begin);
                for (int i = 0; i < 7; i++)
                {
                    string temp;
                    if (isUnicode)
                    {
                        temp = ReadStringW(reader);
                    }
                    else
                        temp = ReadStringA(reader);
                    System.Diagnostics.Debug.Print(temp);
                }
                uint tet1=reader.ReadUInt32();
                uint tet2=reader.ReadUInt32();
                uint cdt1=reader.ReadUInt32();
                uint cdt2=reader.ReadUInt32();
                uint mdt1=reader.ReadUInt32();
                uint mdt2=reader.ReadUInt32();
                short pcount=reader.ReadInt16();
                for (int i = 0; i < pcount-15; i++)
                {
                    string name,value;
                    if (isUnicode)
                    {
                        name = ReadStringW(reader);
                        value = ReadStringW(reader);
                    }
                    else
                    {
                        name = ReadStringA(reader);
                        value = ReadStringA(reader);
                    }
                    System.Diagnostics.Debug.Print(name);
                    System.Diagnostics.Debug.Print(value);
                }
            }
        }

        private static string ReadStringA(BinaryReader reader)
        {
            int len=reader.ReadInt16();
            if (len > 0)
            {
                string tmp=Encoding.ASCII.GetString(reader.ReadBytes(len));
                return tmp.TrimEnd(Convert.ToChar(0));
            }
            else return string.Empty;
        }

        private static string ReadStringW(BinaryReader reader)
        {
            int len=reader.ReadInt16();
            if (len > 0)
            {
                char[] buffer=new char[len - 1];
                for (int i = 0; i < len - 1; i++)
                {
                    short n=reader.ReadInt16();
                    buffer[i] = Convert.ToChar(n);
                }
                return new string(buffer).TrimEnd(Convert.ToChar(0));
            }
            else return string.Empty;
        }
    }
}
