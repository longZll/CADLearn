using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;


namespace DragDropHandler
{
    /// <summary>
    /// 拖放处理程序
    /// </summary>
    public class DragDropHandler
    {
        [CommandMethod("EnableDragDrop")]
        public void EnableDragDrop()
        {
            Application.DocumentManager.DocumentCreated += OnDocumentCreated;
        }

        private void OnDocumentCreated(object sender, DocumentCollectionEventArgs e)
        {
            Document doc = e.Document;
          
            doc.CommandWillStart += OnCommandWillStart;
        }

        private void OnCommandWillStart(object sender, CommandEventArgs e)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;

            // 注册一个拖拽事件
            //DragWindow dragWindow = new DragWindow(new Point3d(0, 0, 0), new Point3d(100, 100, 0));
            //ed.Drag(dragWindow);


            if (doc != null && e.GlobalCommandName == "OPEN")
            {
                // 获取最后一个打开的文档，即新打开的文档
                Document newDoc = Application.DocumentManager.Cast<Document>().Last();

                // 检查新文档是否是DWG文件
                if (System.IO.Path.GetExtension(newDoc.Name).Equals(".dwg", System.StringComparison.OrdinalIgnoreCase))
                {
                    Database db = newDoc.Database;

                    using (Transaction trans = db.TransactionManager.StartTransaction())
                    {
                        // 以可写模式打开拖入的DWG文件
                        newDoc.Editor.Command(new object[] { "_.OPEN", newDoc.Name });

                        trans.Commit();
                    }
                }
            }
        }


    }


    //public class DragWindow : Autodesk.AutoCAD.Windows.DragWindow
    //{
    //    public DragWindow(Point3d pt1, Point3d pt2)
    //        : base(pt1, pt2)
    //    {
    //    }
    //    protected override void OnDragBegin()
    //    {
    //        // 拖拽开始时执行的操作
    //    }
    //    protected override void OnDragEnd()
    //    {
    //        // 拖拽结束时执行的操作
    //    }
    //    protected override void OnDragMove()
    //    {
    //        // 拖拽移动时执行的操作
    //    }
    //}


}
