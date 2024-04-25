using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using DotNetARX;
namespace DocManager
{
    public class DocManager
    {
        [CommandMethod("OpenDwg")]
        public void OpenDwg()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            //获取文档管理器对象以打开Dwg文件
            DocumentCollection docs = Application.DocumentManager;
            //设置打开文件对话框的有关选项
            PromptOpenFileOptions opt = new PromptOpenFileOptions("\n请输入文件名：");
            opt.Filter = "图形(*.dwg)|*.dwg|图形(*.dxf)|*.dxf";
            opt.FilterIndex = 0;
            //根据打开文件对话框中用户的选择，获取文件名
            string filename = ed.GetFileNameForOpen(opt).StringResult;
            //打开所选择的Dwg文件
            Document doc = docs.Open(filename, true);
            //设置当前的活动文档为新打开的Dwg文件
            Application.DocumentManager.MdiActiveDocument = doc;
        }
        [CommandMethod("SaveDwg")]
        public void SaveDwg()
        {
            Document doc=Application.DocumentManager.MdiActiveDocument;
            //获取DWGTITLED系统变量，它指示当前图形是否已命名。
            object tiled=Application.GetSystemVariable("DWGTITLED");
            if (!doc.Saved()) return;//如果图形没有未保存的修改，则返回
            if (Convert.ToInt16(tiled) == 0)//如果图形没有被命名
                doc.Database.SaveAs("C:\\test.dwg", DwgVersion.Current);
            else
                doc.Save();//保存当前文档
        }
        [CommandMethod("CreateNewDwg", CommandFlags.Session)]
        public void CreateNewDwg()
        {
            string template="acad.dwt";//确定要使用的文档模板
            DocumentCollection docs=Application.DocumentManager;
            Document doc=docs.Add(template);//根据模板创建文档
            Database db=doc.Database;
            using (doc.LockDocument())//锁定文档
            {
                Transaction trans=db.TransactionManager.StartTransaction();
                Circle cir=new Circle();
                cir.Center = new Point3d(5, 5, 0);
                cir.Radius = 5;
                db.AddToModelSpace(cir);//将圆添加到文档的模型空间
                trans.Commit();
            }
        }
        [CommandMethod("CloseAllDwgs", CommandFlags.Session)]
        public void CloseAllDwgs()
        {
            DocumentCollection docs=Application.DocumentManager;
            foreach (Document doc in docs)//遍历打开的文档
            {
                //如果文档中有运行的命令（不包括CloseAllDwgs命令）
                if (doc.CommandInProgress != "" && doc.CommandInProgress != "CloseAllDwgs")
                    doc.SendCommand("\x03\x03");//向命令行发出两个Esc命令，终止命令的运行
                if (doc.IsReadOnly) doc.CloseAndDiscard();//如果是只读文件，直接关闭
                else //不是只读文件
                {
                    if (docs.MdiActiveDocument != doc)//如果不是当前文档，则切换成当前文档
                        docs.MdiActiveDocument = doc;
                    //如果文档有未保存的修改，则保存后关闭文档
                    if (doc.Saved()) doc.CloseAndSave(doc.Name);
                    else doc.CloseAndDiscard();//如果文档没有未保存的修改，则直接关闭
                }
            }
        }
    }
}
