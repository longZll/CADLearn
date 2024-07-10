using System;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using DotNetARX;
namespace Blocks
{
    public class Blocks
    {
        /// <summary>
        /// 制作门
        /// </summary>
        [CommandMethod("MakeDoorBlock")]
        public void MakeDoorBlock()
        {
            Database db=HostApplicationServices.WorkingDatabase;
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                //设置门框的左边线
                Point3d pt1=Point3d.Origin;
                Point3d pt2=new Point3d(0, 1.0, 0);
                Line leftLine=new Line(pt1, pt2);
                //门框的下边线
                Line bottomLine=new Line(pt1, pt1.PolarPoint(0, 0.05));
                
                //设置表示门面的圆弧
                Arc arc=new Arc();
                arc.CreateArc(pt1.PolarPoint(0, 1), pt1, Math.PI / 2);
                //设置门框的右边线
                Line rightLine=new Line(bottomLine.EndPoint, leftLine.EndPoint.PolarPoint(0, 0.05));
                Point3dCollection pts=new Point3dCollection();

                //rightLine.IntersectWith(arc, Intersect.OnBothOperands, pts, 0, 0);
                rightLine.IntersectWith(arc, Intersect.OnBothOperands, pts, IntPtr.Zero, IntPtr.Zero);

                if (pts.Count == 0) return;

                rightLine.EndPoint = pts[0];

                //将表示门的直线和圆弧添加到名为DOOR的块表记录
                db.AddBlockTableRecord("DOOR", leftLine, bottomLine, rightLine, arc);
                trans.Commit();


                // 显示成功消息
                ed.WriteMessage("\n已经成功制作了一个门图块！");
            }
        }

        /// <summary>
        /// 插入门图块
        /// </summary>
        [CommandMethod("InsertDoor")]
        public void InsertDoor()
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;

            Database db=HostApplicationServices.WorkingDatabase;
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            ObjectId spaceId=db.CurrentSpaceId; //获取当前空间（模型空间或图纸空间）

            MakeDoorBlock();

            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                DocumentLock m_DocumentLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument();

                //在当前空间加入块参照
                spaceId.InsertBlockReference("0", "DOOR", Point3d.Origin, new Scale3d(2), 0);

                m_DocumentLock.Dispose();
                trans.Commit();

                // 显示成功消息
                ed.WriteMessage("\n已经成功将门图块插入到本图纸中！");
                //增加范围缩放功能
                doc.SendStringToExecute("_.zoom E ", true, false, true);
            }

          

        }
    }
}
