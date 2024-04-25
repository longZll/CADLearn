using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using DotNetARX;
namespace DynBlock
{
    /// <summary>
    /// 动态块案例
    /// </summary>
    public class DynBlock
    {
        /// <summary>
        /// 利用直线和圆弧制作门
        /// </summary>
        /// <returns></returns>
        
        //[CommandMethod("MakeDoor")]
        public ObjectId MakeDoor()
        {
            ObjectId blockId;
            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                //设置门框的左边线
                Point3d pt1 = Point3d.Origin;
                Point3d pt2 = new Point3d(0, 1.0, 0);
                Line leftLine = new Line(pt1, pt2);

                //门框的下边线
                Line bottomLine = new Line(pt1, pt1.PolarPoint(0, 0.05));

                //设置表示门面的圆弧
                Arc arc = new Arc();
                arc.CreateArc(pt1.PolarPoint(0, 1), pt1, Math.PI / 2);
                //设置门框的右边线
                Line rightLine = new Line(bottomLine.EndPoint, leftLine.EndPoint.PolarPoint(0, 0.05));

                Point3dCollection pts = new Point3dCollection();

                //获取直线与arc圆弧的交点,并且存储在pts中
                //rightLine.IntersectWith(arc, Intersect.OnBothOperands, pts, 0, 0);

                rightLine.IntersectWith(arc, Intersect.OnBothOperands, pts, IntPtr.Zero, IntPtr.Zero);

                if (pts.Count == 0) return ObjectId.Null;

                rightLine.EndPoint = pts[0];

                //将表示门的直线和圆弧 添加到BlockTableRecor,块的名字为设置为"Door"
                blockId = db.AddBlockTableRecord("Door", leftLine, bottomLine, rightLine, arc);

                trans.Commit();
            }
            return blockId;
        }

        /// <summary>
        /// 统计向右边开的门的数量
        /// </summary>
        [CommandMethod("CountDynBlock")]
        public void CountDynBlock()
        {
            Database db=HostApplicationServices.WorkingDatabase;

            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

            try
            {
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    //选取模型空间中的Door动态块，并限定其Flip vertical属性为1（向右开的门）
                    var doors = from d in db.GetEntsInModelSpace<BlockReference>()
                                where d.GetBlockName() == "Door" && d.ObjectId.GetDynBlockValue("Flip vertical") == "1"
                                select d;
                    //显示向右开的门的个数
                    Application.ShowAlertDialog("向右开的门共有" + doors.Count() + "个");

                    trans.Commit();
                }

            }
            catch (System.Exception ex)
            {
                ed.WriteMessage(ex.Message);   //显示异常信息
               
            }
        
        }

        /// <summary>
        /// 插入动态块
        /// </summary>
        [CommandMethod("InsertDynBlock")]
        public void InsertDynBlock()
        {
            Database db=HostApplicationServices.WorkingDatabase;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

            try
            {
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    ObjectId model = db.GetModelSpaceId();//获取模型空间的ObjectId
                                                          //表示块静态属性的字典对象
                    Dictionary<string, string> atts = new Dictionary<string, string>();

                    //添加门的各种静态属性
                    atts.Add("SYM.", "2");
                    atts.Add("WIDTH", "0.8m");
                    atts.Add("HEIGHT", "2m");
                    atts.Add("STYLE", "ONE PANEL");
                    atts.Add("REF#", "TS 1040");
                    atts.Add("MANUFACTURE", "TUR STYLE");
                    atts.Add("COST", "500.00");

                    //在模型空间的Doors层上插入表示门的Door块，插入点为原点，不旋转，不缩放
                    ObjectId blockId = model.InsertBlockReference("Doors", "Door", Point3d.Origin, new Scale3d(), 0, atts);

                    //设置门的动态属性，宽度为0.8个单位，垂直方向翻转
                    blockId.SetDynBlockValue("Door Width", 0.8);
                    blockId.SetDynBlockValue("Flip vertical", 1);

                    trans.Commit();
                }

            }
            catch (System.Exception ex)
            {
                ed.WriteMessage(ex.Message);   //显示异常信息
               
            }

           
            //执行缩放命令
            Application.DocumentManager.MdiActiveDocument.SendStringToExecute("zoom e\n", true, false, true);
        }
    }
}
