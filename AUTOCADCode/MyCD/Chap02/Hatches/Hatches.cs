using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using DotNetARX;
using Autodesk.AutoCAD.Colors;
using System.Drawing;


namespace Hatches
{
    public class Hatches
    {
        [CommandMethod("AddHatch")]
        public void AddHatch()
        {
            Database db=HostApplicationServices.WorkingDatabase;
            //创建一个正六边形
            Polyline polygon=new Polyline();
            polygon.CreatePolygon(new Point2d(500, 200), 6, 30);
            //创建一个圆
            Circle circle=new Circle();
            circle.Center = new Point3d(500, 200, 0);
            circle.Radius = 10;
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                //将正六边形和圆添加到模型空间中
                ObjectId polygonId=db.AddToModelSpace(polygon);
                ObjectId circleId=db.AddToModelSpace(circle);

                //创建一个ObjectId集合类对象，用于存储填充边界的ObjectId
                ObjectIdCollection ids=new ObjectIdCollection();
                ids.Add(polygonId);//将正六边形的ObjectId添加到边界集合中

                Hatch hatch=new Hatch();//创建填充对象
                hatch.PatternScale = 0.5;//设置填充图案的比例

                //创建填充图案选项板
                HatchPalletteDialog dlg=new HatchPalletteDialog();
                //显示填充图案选项板
                bool isOK=dlg.ShowDialog();
                //如果用户选择了填充图案，则设置填充图案名为所选的图案名，否则为当前图案名
                string patterName=isOK ? dlg.GetPattern() : HatchTools.CurrentPattern;
                //根据上面的填充图案名创建图案填充，类型为预定义,与边界关联
                hatch.CreateHatch(HatchPatternType.PreDefined, patterName, true);
                //为填充添加外边界（正六边形）
                hatch.AppendLoop(HatchLoopTypes.Outermost, ids);

                ids.Clear();        //清空集合以添加新的边界
                ids.Add(circleId);  //将圆的ObjectId添加到边界集合中
                
                //为填充添加内边界（圆）
                hatch.AppendLoop(HatchLoopTypes.Default, ids);
                hatch.EvaluateHatch(true);//计算并显示填充对象

                trans.Commit();//提交更改
            }
        }
        
        /// <summary>
        /// 添加渐变填充
        /// </summary>
        [CommandMethod("AddGradientHatch")]
        public void AddGradientHatch()
        {
            Database db=HostApplicationServices.WorkingDatabase;
            //创建一个三角形
            Polyline triangle=new Polyline();
            triangle.CreatePolygon(new Point2d(550, 200), 3, 30);
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                //将三角形添加到模型空间中
                ObjectId triangleId=db.AddToModelSpace(triangle);
                //创建一个ObjectId集合类对象，用于存储填充边界的ObjectId
                ObjectIdCollection ids=new ObjectIdCollection();
                ids.Add(triangleId);//将三角形的ObjectId添加到边界集合中
                Hatch hatch=new Hatch();//创建填充对象
                //创建两个Color类变量，分别表示填充的起始颜色（红）和结束颜色（蓝）
                Autodesk.AutoCAD.Colors.Color color1 = Autodesk.AutoCAD.Colors.Color.FromColorIndex(ColorMethod.ByLayer, 1);
                Autodesk.AutoCAD.Colors.Color color2 = Autodesk.AutoCAD.Colors.Color.FromColor(System.Drawing.Color.Blue);
                //创建渐变填充，与边界无关联
                hatch.CreateGradientHatch(HatchGradientName.Cylinder, color1, color2, false);
                //为填充添加边界（三角形）
                hatch.AppendLoop(HatchLoopTypes.Default, ids);
                hatch.EvaluateHatch(true);//计算并显示填充对象
                trans.Commit();//提交更改
            }
        }
    }
}
