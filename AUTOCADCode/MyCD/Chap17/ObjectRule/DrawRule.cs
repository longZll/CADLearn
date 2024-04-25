using System;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using DotNetARX;

namespace ObjectRule
{
    public class DrawRule : DrawableOverrule
    {
        public override bool WorldDraw(Drawable drawable, WorldDraw wd)
        {
            //要绘制的对象强制转化为直线
            Line line=drawable as Line;
            //获取直线的起点、终点、长度
            Point3d startPoint=line.StartPoint;
            Point3d endPoint=line.EndPoint;
            double len=line.Length;
            //获取表示直线的方向向量
            Vector3d delta=line.Delta;
            //定义一个点集合，用于存储构成门框（矩形）的角点
            Point3dCollection pts=new Point3dCollection();
            Point3d pt1=startPoint - 0.05 * delta;
            Point3d pt2=pt1 - delta.Negate().RotateBy(Math.PI / 2, Vector3d.ZAxis);
            Point3d pt3=startPoint + delta.RotateBy(Math.PI / 2, Vector3d.ZAxis);
            pts.Add(startPoint);
            pts.Add(pt1);
            pts.Add(pt2);
            pts.Add(pt3);
            //绘制门框            
            wd.Geometry.Polygon(pts);
            //绘制表示门面的圆弧
            wd.Geometry.CircularArc(startPoint, len, Vector3d.ZAxis, delta, Math.PI / 2, ArcType.ArcSimple);
            //绘制结束，返回。若要绘制直线本身，则请调用下面一行代码
            return true;
            //return base.WorldDraw(drawable, wd);
        }
        public override int SetAttributes(Drawable drawable, DrawableTraits traits)
        {
            //由于我们只是改变直线的颜色，因此需要调用基类函数正常设置其它的属性
            int b=base.SetAttributes(drawable, traits);
            Line line=drawable as Line;
            ObjectId id=line.ObjectId;
            //获取直线的扩展数据
            TypedValueList xdata=line.GetXDataForApplication(Commands.appName);
            if (xdata != null)
            {
                string doorType=xdata[1].Value.ToString();
                switch (doorType)
                {
                    case "木门":
                        traits.Color = 1;//红色
                        break;
                    case "铁门":
                        traits.Color = 2;//黄色
                        break;
                    case "铝合金门":
                        traits.Color = 3;//绿色
                        break;
                }
            }
            return b;
        }
    }
}
