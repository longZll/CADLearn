using Autodesk.AutoCAD.Runtime;
using System.Reflection;
using System.Windows.Forms;
using Autodesk.AutoCAD;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.DatabaseServices.Filters;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.PlottingServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using cadapp = Autodesk.AutoCAD.ApplicationServices.Application;


[assembly: CommandClass(typeof(netload.netloadx))]

namespace netload
{
    public class netloadx
    {

        [CommandMethod("netloadx")]
        public void Netloadx()
        {
            Editor ed = cadapp.DocumentManager.MdiActiveDocument.Editor;
            string file_dir = "";
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "dll文件(*.dll)|*.dll";
            ofd.Title = "打开dll文件";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                file_dir = ofd.FileName;
            }
            else return;
            AssemblyDependent ad = new AssemblyDependent(file_dir); 
            bool allyes = true;
            foreach (var item in ad.Load())
            {
                if (!item.LoadYes)
                {
                    ed.WriteMessage("\n" + item.Path + "没加载成功");
                    allyes = false;
                }
            }
            if (allyes)
            {
                ed.WriteMessage("\n加载成功");
            }
        }

        [CommandMethod("netloadzhy")]
        public void netloadzhy()
        {
            string file_dir2 = @"D:\文档\zhy\我的文档\我的程序\我的netdll2014\zhy.dll";
            Editor ed = cadapp.DocumentManager.MdiActiveDocument.Editor;
            AssemblyDependent ad = new AssemblyDependent(file_dir2); 
            bool allyes = true;
            foreach (var item in ad.Load())
            {
                if (!item.LoadYes)
                {
                    ed.WriteMessage("\n" + item.Path + "...");
                    allyes = false;
                }
            }
            if (allyes)
            {
                ed.WriteMessage("\n。。。");
            }  

        }




        //[CommandMethod("netloadx")]
        public void Netloadx000()
        {
            string file_dir = "";
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "dll文件(*.dll)|*.dll";
            ofd.Title = "打开dll文件";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                file_dir = ofd.FileName;
            }
            else return;
            //打开文件，将文件以二进制方式复制到内存，自动关闭文件
            byte[] buffer = System.IO.File.ReadAllBytes(file_dir);
            //加载内存中的文件
            Assembly assembly = Assembly.Load(buffer);
        }

        //[CommandMethod("netloadzhy")]
        public void netloadzhy000()
        {
            string file_dir2 = @"D:\文档\zhy\我的文档\我的程序\我的netdll2014\zhy.dll";
            byte[] buffer2 = System.IO.File.ReadAllBytes(file_dir2);
            Assembly assembly2 = Assembly.Load(buffer2);
        }

    }
}
