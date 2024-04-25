using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using DotNetARX;
namespace CircleAndArc
{   /// <summary>
    /// 创建扇形
    /// </summary>
    public class CircleAndArc
    {
        [CommandMethod("Fan")]
        public static void Fan()
        {
            Database db=HostApplicationServices.WorkingDatabase;
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                //定义圆弧上的三个点
                Point3d startPoint=new Point3d(1000, 0, 0);
                Point3d pointOnArc=new Point3d(500, 250, 0);
                Point3d endPoint=new Point3d();

                //调用三点法画圆弧的扩展函数创建扇形的圆弧
                Arc arc=new Arc();
                arc.CreateArc(startPoint, pointOnArc, endPoint);

                //创建扇形的两条半径
                Line line1= new Line(arc.Center, startPoint);
                Line line2= new Line(arc.Center, endPoint);
                //一次性添加实体到模型空间，完成扇形的创建
                db.AddToModelSpace(line1, line2, arc);
                trans.Commit();
            }
        }
    }
}
