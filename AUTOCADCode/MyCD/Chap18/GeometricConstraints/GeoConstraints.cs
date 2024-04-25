using Autodesk.ADN.AutoCAD;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using DotNetARX;
namespace GeoConstraints
{
    public class GeoConstraints
    {
        [CommandMethod("CoinCenter")]
        public void CoinCenter()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                // 提示用户选择一个圆，作为所有同心圆的圆心
                ed.WriteMessage("\n请选择第一个圆");
                var cirs = db.GetSelection<Circle>();
                if (cirs.Count != 1) return; // 如果选择的圆数量不是1，则返回
                Circle firstCir = cirs[0]; // 获取选择的圆对象
                cirs.Clear(); // 清空圆对象列表
                // 提示用户选择需要同心的圆
                ed.WriteMessage("\n请选择需要同心的圆");
                cirs = db.GetSelection<Circle>();
                foreach (Circle cir in cirs) // 遍历圆列表
                {
                    // 添加同心约束，让每个圆与第一个圆同心
                    AssocUtil.CreateConcentricConstraint(firstCir.ObjectId, cir.ObjectId, firstCir.Center, cir.Center);
                }
                trans.Commit();
            }
        }

        [CommandMethod("MakeRect")]
        public void MakeRect()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            Database db = HostApplicationServices.WorkingDatabase;
            // 提示用户选择一个表示四边形的多段线
            PromptEntityOptions opt = new PromptEntityOptions("请选择一个四边形");
            opt.SetRejectMessage("必须是多段线");
            opt.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult result = ed.GetEntity(opt);
            if (result.Status != PromptStatus.OK) return; // 如果用户未选择，则返回
            ObjectId id = result.ObjectId; // 获取四边形的Id
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                // 获取多段线对象
                Polyline pline = (Polyline)id.GetObject(OpenMode.ForRead);
                // 如果多段线不是四边形（拥有4个顶点且封闭）
                if (pline.NumberOfVertices != 4 && !pline.Closed) return;
                // 声明一个点集对象，用来存储多段线的顶点及中点
                Point3dCollection pts = new Point3dCollection();
                for (int i = 0; i < 8; i++)
                {
                    pts.Add(pline.GetPointAtParameter(i * 0.5));
                }
                // 为多段线添加平行约束，使其成为平行四边形
                AssocUtil.CreateParallelConstraint(id, id, pts[1], pts[5]);
                AssocUtil.CreateParallelConstraint(id, id, pts[3], pts[7]);
                // 为多段线添加水平约束，使其平行于X轴
                AssocUtil.CreateHorizontalConstraint(id, pts[3]);
                // 为多段线添加垂直约束，使其平行于Y轴
                AssocUtil.CreateVerticalConstraint(id, pts[1]);
                // 为多段线添加固定约束，使其一条边保持不动
                AssocUtil.CreateFixConstraint(id, pts[7]);
                trans.Commit();
            }
        }
    }
}
