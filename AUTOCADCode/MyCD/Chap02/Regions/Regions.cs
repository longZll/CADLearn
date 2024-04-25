using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using DotNetARX;
namespace Regions
{
    public class Regions
    {
        [CommandMethod("AddRegion")]
        public void AddRegion()
        {
            Database db=HostApplicationServices.WorkingDatabase;
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                //创建一个正三角形
                Polyline triangle=new Polyline();
                triangle.CreatePolygon(new Point2d(550, 200), 3, 30);
                //根据三角形创建面域
                List<Region> regions=RegionTools.CreateRegion(triangle);
                if (regions.Count == 0) return;//如果面域创建未成功，则返回
                Region region=regions[0];
                db.AddToModelSpace(region);//将创建的面域添加到数据库中
                //打印面域的相关特性
                getAreaProp(region);
                trans.Commit();//提交更改
            }
        }

        /// <summary>
        /// 创建组合面域
        /// </summary>
        [CommandMethod("AddComplexRegion")]
        public void AddComplexRegion()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                //创建一个正六边形
                Polyline polygon=new Polyline();
                polygon.CreatePolygon(new Point2d(500, 200), 6, 30);
                //创建一个圆
                Circle circle=new Circle();
                circle.Center = new Point3d(500, 200, 0);
                circle.Radius = 10;
                //根据正六边形和圆创建面域
                List<Region> regions=RegionTools.CreateRegion(polygon, circle);
                if (regions.Count == 0) return;//如果面域创建未成功，则返回
                //使用LINQ按面积对面域进行排序
                List<Region> orderRegions = (from r in regions
                                             orderby r.Area
                                             select r).ToList();
                //对面域进行布尔操作，获取正六边形减去圆后的部分
                orderRegions[1].BooleanOperation(BooleanOperationType.BoolSubtract, orderRegions[0]);
               
                db.AddToModelSpace(regions[1]);//将上面操作好的面域添加到数据库中            
                trans.Commit();//提交更改
            }
        }
        /// <summary>
        /// 打印输出面域的相关特性
        /// </summary>
        /// <param name="region"></param>
        private void getAreaProp(Region region)
        {
            Editor ed=Application.DocumentManager.MdiActiveDocument.Editor;
            ed.WriteMessage("\n ----------------    面域   ----------------");
            ed.WriteMessage("\n面积:{0} ", region.Area);
            ed.WriteMessage("\n周长:{0} ", region.Perimeter);
            ed.WriteMessage("\n边界框上限:{0} ", region.GetExtentsHigh());
            ed.WriteMessage("\n边界框下限:{0} ", region.GetExtentsLow());
            ed.WriteMessage("\n质心: {0} ", region.GetCentroid());
            ed.WriteMessage("\n惯性矩为: {0}; {1} ", region.GetMomInertia()[0], region.GetMomInertia()[1]);
            ed.WriteMessage("\n惯性积为: {0} ", region.GetProdInertia());
            ed.WriteMessage("\n主力矩为: {0}; {1} ", region.GetPrinMoments()[0], region.GetPrinMoments()[1]);
            ed.WriteMessage("\n主方向为: {0}; {1} ", region.GetPrinAxes()[0], region.GetPrinAxes()[1]);
            ed.WriteMessage("\n旋转半径为: {0}; {1} ", region.GetRadiiGyration()[0], region.GetRadiiGyration()[1]);
        }
    }
}
