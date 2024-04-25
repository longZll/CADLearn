using System.Collections.Generic;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using DotNetARX;
namespace MLineStyles
{
    public class MLineStyles
    {
        [CommandMethod("AddMLineStyle")]
        public void AddMLineStyle()
        {
            Database db=HostApplicationServices.WorkingDatabase;
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                //MlineStyleElement列表对象，用来存储需要设置多线样式的元素
                List<MlineStyleElement> elements=new List<MlineStyleElement>();
                //第一个元素（红色的中心线）
                Color red=Color.FromColorIndex(ColorMethod.ByAci, 1);
                ObjectId centerId=db.LoadLineType("CENTER");
                elements.Add(new MlineStyleElement(0, red, centerId));
                //第二个元素（蓝色的虚线）
                Color blue=Color.FromColorIndex(ColorMethod.ByAci, 5);
                ObjectId hiddenId=db.LoadLineType("HIDDEN");
                elements.Add(new MlineStyleElement(0.5, blue, hiddenId));
                //第三个元素（蓝色的虚线）
                elements.Add(new MlineStyleElement(-0.5, blue, hiddenId));
                //创建名为NewStyle的多线样式，并将其设置为默认样式
                db.CmlstyleID = db.CreateMLineStyle("NewStyle", elements);
                trans.Commit();
            }
        }

        [CommandMethod("AddMLine")]
        public void AddMLine()
        {
            Database db=HostApplicationServices.WorkingDatabase;
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                Mline mline=new Mline(); // 创建多线对象
                mline.Normal = new Vector3d(0, 0, 1); // 多线的法向量
                mline.Style = db.CmlstyleID; //多线的样式
                mline.Justification = MlineJustification.Zero;// 多线的对齐方式
                // 添加多线的顶点
                mline.AppendSegment(Point3d.Origin);
                mline.AppendSegment(new Point3d(10, 0, 0));
                mline.AppendSegment(new Point3d(5, 5, 0));
                mline.IsClosed = true; // 多线闭合
                db.AddToModelSpace(mline); //将多线添加到模型空间中
                trans.Commit(); //提交事务处理
            }
        }

        [CommandMethod("DelMLineStyle")]
        public void DelMLineStyle()
        {
            Database db=HostApplicationServices.WorkingDatabase;
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                db.RemoveMLineStyle("NewStyle");
                trans.Commit();
            }
        }
    }
}
