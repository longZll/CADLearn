using System;
using Autodesk.ADN.AutoCAD;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using DotNetARX;
namespace DimConstraints
{
    public class DimConstraints
    {
        [CommandMethod("SameArea")]
        public void SameArea()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                // 创建一个角点为（0，0）、（30，30）的矩形（多段线表示）
                Polyline rect = new Polyline();
                rect.CreateRectangle(Point2d.Origin, new Point2d(30, 30));
                // 创建一个圆心为(15, 15, 0)、半径为1的圆
                Circle cir = new Circle { Center = new Point3d(15, 15, 0), Radius = 1 };
                db.AddToModelSpace(rect, cir); // 将矩形及圆添加到模型空间
                // 声明一个点集对象，用来存储矩形的顶点及中点
                Point3dCollection pts = new Point3dCollection();
                for (int i = 0; i < 8; i++)
                {
                    pts.Add(rect.GetPointAtParameter(i * 0.5));
                }
                // 为多段线添加平行约束，使其成为平行四边形
                AssocUtil.CreateParallelConstraint(rect.ObjectId, rect.ObjectId, pts[1], pts[5]);
                AssocUtil.CreateParallelConstraint(rect.ObjectId, rect.ObjectId, pts[3], pts[7]);
                // 为多段线添加水平约束，使其平行于X轴
                AssocUtil.CreateHorizontalConstraint(rect.ObjectId, pts[3]);
                // 为多段线添加垂直约束，使其平行于Y轴
                AssocUtil.CreateVerticalConstraint(rect.ObjectId, pts[1]);
                // 为多段线添加固定约束，使其一条边保持不动
                AssocUtil.CreateFixConstraint(rect.ObjectId, pts[7]);
                // 为矩形添加对齐标注约束，约束矩形的宽度
                //ObjectId widthId = AssocUtil.CreateAlignedDimConstraint(rect.ObjectId, pts[0], pts[2], pts[1].PolarPoint(Math.PI, 10));
                ObjectId widthId = AssocUtil.Create2LineAngularDimConstraint(rect.ObjectId, rect.ObjectId,pts[0], pts[2], pts[1].PolarPoint(Math.PI, 10));
                // 为矩形添加对齐标注约束，约束矩形的长度
                ObjectId lengthId = AssocUtil.CreateAlignedDimConstraint(rect.ObjectId, pts[0], pts[6], pts[7].PolarPoint(-Math.PI / 2, 10));
                // 将对齐标注约束参数重新命名
                AssocUtil.RenameVariable(widthId, "Width", true);
                AssocUtil.RenameVariable(lengthId, "Length", true);
                // 获取当前图形中名为Area的用户参数，如果不存在，则创建
                ObjectId areaId = AssocUtil.GetVariableByName(db.CurrentSpaceId, "Area", true);
                string errMsg; // 存储错误信息的字符串
                // 设置Area参数的表达式为Width*Length，即矩形的面积
                AssocUtil.SetVariableValue(areaId, null, "Width*Length", "", out errMsg);
                // 为圆添加半径约束
                ObjectId radiusId = AssocUtil.CreateRadialDimConstraint(cir.ObjectId, cir.GetPointAtParameter(Math.PI / 4), cir.GetPointAtParameter(Math.PI / 4));
                // 设置半径约束的表达式为sqrt(Area/PI)，根据矩形的面积获取圆的半径
                AssocUtil.SetVariableValue(radiusId, null, "sqrt(Area/PI)", "", out errMsg);
                // 将半径约束参数重新命名
                AssocUtil.RenameVariable(radiusId, "Radius", true);
                // 添加水平标注约束，约束矩形角点与圆心之间的距离
                ObjectId centerXId = AssocUtil.CreateHorizontalDimConstraint(rect.ObjectId, cir.ObjectId, pts[2], cir.Center, pts[3].PolarPoint(Math.PI / 2, 10));
                // 设置水平标注约束的表达式为Length/2，即矩形长度的一半
                AssocUtil.SetVariableValue(centerXId, null, "Length/2", "", out errMsg);
                // 添加垂直标注约束，约束矩形角点与圆心之间的距离
                ObjectId centerYId = AssocUtil.CreateVerticalDimConstraint(rect.ObjectId, cir.ObjectId, pts[6], cir.Center, pts[5].PolarPoint(0, 10));
                // 设置垂直标注约束的表达式为Width/2，即矩形宽度的一半
                AssocUtil.SetVariableValue(centerYId, null, "Width/2", "", out errMsg);
                trans.Commit();
            }
        }
    }
}
