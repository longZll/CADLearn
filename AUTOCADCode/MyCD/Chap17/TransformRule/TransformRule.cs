using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using DotNetARX;
namespace TransformRule
{
public class TransformRule : TransformOverrule
{
    internal static ObjectId curveId;
    public override void TransformBy(Entity entity, Matrix3d transform)
    {
        bool found=false;//在曲线上是否能找到
        DBPoint pt=entity as DBPoint;// 点对象
        if (pt != null)
        {
            Database db=HostApplicationServices.WorkingDatabase;
            Document doc=db.GetDocument();
            using (OpenCloseTransaction trans=db.TransactionManager.StartOpenCloseTransaction())
            {
                Curve curve=trans.GetObject(curveId, OpenMode.ForRead) as Curve;
                if (curve != null)
                {
                    //将点的位置限制在曲线上
                    Point3d ptLoc=pt.Position.TransformBy(transform);
                    Point3d ptOnCurve=curve.GetClosestPointTo(ptLoc, false);
                    pt.Position = ptOnCurve;
                    found = true;//已经在曲线上找到点
                }
            }
        }
        if (!found) base.TransformBy(entity, transform);
    }
}
}
