using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using Autodesk.AutoCAD.Runtime;
using DotNetARX;

namespace CustomFilter
{
    public class DrawRule : DrawableOverrule
    {       
        short blockColor=1;//设置亮显的颜色为红色
        //需要亮显的块名
        public static string BlockName { get; set; }
        public override bool WorldDraw(Drawable drawable, WorldDraw wd)
        {
            BlockReference bref=drawable as BlockReference;
            //获取块参照的边界框
            Extents3d ext=bref.Bounds.Value;
            //获取边界框的最值点
            Point3d maxPoint=ext.MaxPoint;
            Point3d minPoint=ext.MinPoint;
            //根据边界框的最值点设置亮显的范围框
            Point3dCollection pts=new Point3dCollection();
            pts.Add(new Point3d(minPoint.X, minPoint.Y, minPoint.Z));
            pts.Add(new Point3d(minPoint.X, maxPoint.Y, minPoint.Z));
            pts.Add(new Point3d(maxPoint.X, maxPoint.Y, minPoint.Z));
            pts.Add(new Point3d(maxPoint.X, minPoint.Y, minPoint.Z));
            //存储当前的填充类型值，设置新的填充类型为总是填充图形
            FillType oldFillType=wd.SubEntityTraits.FillType;
            wd.SubEntityTraits.FillType = FillType.FillAlways;
            //存储当前的颜色值，设置新的颜色为红色
            short oldColor=wd.SubEntityTraits.Color;
            wd.SubEntityTraits.Color = blockColor;
            //绘制包围块参照的红色填充矩形
            wd.Geometry.Polygon(pts);
            //块参照还原为原来的填充类型和颜色
            wd.SubEntityTraits.FillType = oldFillType;
            wd.SubEntityTraits.Color = oldColor;
            //调用基类函数，绘制块参照本身的图形
            return base.WorldDraw(drawable, wd);
        }
        public override bool IsApplicable(RXObject overruledSubject)
        {
            //获取块参照及其ObjectId
            BlockReference bref=overruledSubject as BlockReference;
            ObjectId id=bref.ObjectId;
            //如果是块参照且已指定需要亮显的块名
            if (bref != null && BlockName!=null)
            {
                Database db=id.Database;
                using (OpenCloseTransaction trans=db.TransactionManager.StartOpenCloseTransaction())                
                {
                    string blockName=id.GetBlockName();//获取块名
                    if (blockName.Contains(BlockName))
                    {
                        //如果块参照的块名是否与指定的块名一致，则对其进行规则重定义
                        return true;
                    }
                }                
            }
            return false;//如果不是块参照或块名与指定的名字不一致则不进行规则重定义
        }
        
    }
}
