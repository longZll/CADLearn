using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using DotNetARX;
namespace Dimensions
{
    public class Dimensions
    {
        [CommandMethod("DimTest")]
        public void DimTest()
        {
            Database db=HostApplicationServices.WorkingDatabase;
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                // 创建要标注的图形
                Line line1=new Line(new Point3d(30, 20, 0), new Point3d(120, 20, 0));
                Line line2=new Line(new Point3d(120, 20, 0), new Point3d(120, 40, 0));
                Line line3=new Line(new Point3d(120, 40, 0), new Point3d(90, 80, 0));
                Line line4=new Line(new Point3d(90, 80, 0), new Point3d(30, 80, 0));
                Arc arc=new Arc(new Point3d(30, 50, 0), 30, Math.PI / 2, Math.PI * 3 / 2);
                Circle cir1=new Circle(new Point3d(30, 50, 0), Vector3d.ZAxis, 15);
                Circle cir2=new Circle(new Point3d(70, 50, 0), Vector3d.ZAxis, 10);
                //将图形添加到模型空间中
                db.AddToModelSpace(line1, line2, line3, line4, arc, cir1, cir2);
                //创建一个列表，用于存储标注对象
                List<Dimension> dims=new List<Dimension>();
                // 创建转角标注（水平）
                RotatedDimension dimRotated1=new RotatedDimension();
                //指定第一条尺寸界线的附着位置
                dimRotated1.XLine1Point = line1.StartPoint;
                //指定第二条尺寸界线的附着位置
                dimRotated1.XLine2Point = line1.EndPoint;
                //指定尺寸线的位置
                dimRotated1.DimLinePoint = GeTools.MidPoint(line1.StartPoint, line1.EndPoint).PolarPoint(-Math.PI / 2, 10);
                dimRotated1.DimensionText = "<>mm";//设置标注的文字为标注值+后缀mm
                dims.Add(dimRotated1);//将水平转角标注添加到列表中
                //创建转角标注(垂直）
                RotatedDimension dimRotated2=new RotatedDimension();
                dimRotated2.Rotation = Math.PI / 2;//转角标注角度为90度，表示垂直方向
                //指定两条尺寸界线的附着位置和尺寸线的位置
                dimRotated2.XLine1Point = line2.StartPoint;
                dimRotated2.XLine2Point = line2.EndPoint;
                dimRotated2.DimLinePoint = GeTools.MidPoint(line2.StartPoint, line2.EndPoint).PolarPoint(0, 10);
                dims.Add(dimRotated2);//将垂直转角标注添加到列表中
                //创建转角标注（尺寸公差标注）
                RotatedDimension dimRotated3=new RotatedDimension();
                //指定两条尺寸界线的附着位置和尺寸线的位置
                dimRotated3.XLine1Point = line4.StartPoint;
                dimRotated3.XLine2Point = line4.EndPoint;
                dimRotated3.DimLinePoint = GeTools.MidPoint(line4.StartPoint, line4.EndPoint).PolarPoint(Math.PI / 2, 10);
                //设置标注的文字为标注值+堆叠文字
                dimRotated3.DimensionText = TextTools.StackText("<>", "+0.026", "-0.025", StackType.Tolerance, 0.7);
                dims.Add(dimRotated3);//将尺寸公差标注添加到列表中
                // 创建对齐标注
                AlignedDimension dimAligned=new AlignedDimension();
                //指定两条尺寸界线的附着位置和尺寸线的位置
                dimAligned.XLine1Point = line3.StartPoint;
                dimAligned.XLine2Point = line3.EndPoint;
                dimAligned.DimLinePoint = GeTools.MidPoint(line3.StartPoint, line3.EndPoint).PolarPoint(Math.PI / 2, 10);
                //设置标注的文字为标注值+公差符号
                dimAligned.DimensionText = "<>" + TextSpecialSymbol.Tolerance + "0.2";
                dims.Add(dimAligned);//将对齐标注添加到列表中
                // 创建半径标注
                RadialDimension dimRadial=new RadialDimension();
                dimRadial.Center = cir1.Center;//圆或圆弧的圆心
                //用于附着引线的圆或圆弧上的点
                dimRadial.ChordPoint = cir1.Center.PolarPoint(GeTools.DegreeToRadian(30), 15);
                dimRadial.LeaderLength = 10;//引线长度
                dims.Add(dimRadial);//将半径标注添加到列表中
                // 创建直径标注
                DiametricDimension dimDiametric=new DiametricDimension();
                //圆或圆弧上第一个直径点的坐标
                dimDiametric.ChordPoint = cir2.Center.PolarPoint(GeTools.DegreeToRadian(45), 10);
                //圆或圆弧上第二个直径点的坐标
                dimDiametric.FarChordPoint = cir2.Center.PolarPoint(GeTools.DegreeToRadian(-135), 10);
                dimDiametric.LeaderLength = 0;//从 ChordPoint 到注解文字或折线处的长度
                dims.Add(dimDiametric);//将直径标注添加到列表中
                // 创建角度标注
                Point3AngularDimension  dimLineAngular=new Point3AngularDimension();
                //圆或圆弧的圆心、或两尺寸界线间的共有顶点的坐标
                dimLineAngular.CenterPoint = line2.StartPoint;                
                //指定两条尺寸界线的附着位置
                dimLineAngular.XLine1Point = line1.StartPoint;
                dimLineAngular.XLine2Point = line2.EndPoint;
                //设置角度标志圆弧线上的点
                dimLineAngular.ArcPoint = line2.StartPoint.PolarPoint(GeTools.DegreeToRadian(135), 10);
                dims.Add(dimLineAngular);//将角度标注添加到列表中
                // 创建弧长标注,标注文字取为默认值
                ArcDimension dimArc=new ArcDimension(arc.Center, arc.StartPoint, arc.EndPoint, arc.Center.PolarPoint(Math.PI, arc.Radius + 10), "<>", db.Dimstyle);
                dims.Add(dimArc);//将弧长标注添加到列表中
                // 创建显示X轴值的坐标标注
                OrdinateDimension dimX=new OrdinateDimension();
                dimX.UsingXAxis = true;//显示 X 轴值
                dimX.DefiningPoint = cir2.Center;//标注点
                //指定引线终点，即标注文字显示的位置
                dimX.LeaderEndPoint = cir2.Center.PolarPoint(-Math.PI / 2, 20);
                dims.Add(dimX);//将坐标标注添加到列表中
                // 创建显示Y轴值的坐标标注
                OrdinateDimension dimY=new OrdinateDimension();
                dimY.UsingXAxis = false;//显示Y轴                
                dimY.DefiningPoint = cir2.Center;//标注点
                //指定引线终点，即标注文字显示的位置
                dimY.LeaderEndPoint = cir2.Center.PolarPoint(0, 20);
                dims.Add(dimY);//将坐标标注添加到列表中
                foreach (Dimension dim in dims)//遍历标注列表
                {
                    dim.DimensionStyle = db.Dimstyle;//设置标注样式为当前样式
                    db.AddToModelSpace(dim);//将标注添加到模型空间中
                }
                trans.Commit();//提交更改
            }
        }
    }
}
