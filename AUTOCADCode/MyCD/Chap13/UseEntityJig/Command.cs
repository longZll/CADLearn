using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

namespace UseEntityJig
{
    public class Command
    {
        [CommandMethod("JigCircle")]
        public void JigCircleTest()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            Matrix3d mt = ed.CurrentUserCoordinateSystem;
            Vector3d normal = mt.CoordinateSystem3d.Zaxis;

            CircleJig circleJig = new CircleJig(normal);

            for (; ; )
            {
                // 拖动
                PromptResult resJig = ed.Drag(circleJig);
                // 放弃, 则退出.
                if (resJig.Status == PromptStatus.Cancel)
                {
                    return;
                }
                // 确定, 则将圆添加到数据库
                if (resJig.Status == PromptStatus.OK)
                {
                    AppendEntity(circleJig.GetEntity());
                    break;
                }
            }
        }

        [CommandMethod("JigEllipse")]
        public void JigEllipseTest()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            Database db = Application.DocumentManager.MdiActiveDocument.Database;

            // 备份系统变量“ANGBASE”.
            object oldAngBase = Application.GetSystemVariable("ANGBASE");

            // 普通的点交互操作.
            PromptPointOptions optPoint = new PromptPointOptions("\n请指定椭圆弧的圆心:");
            PromptPointResult resPoint = ed.GetPoint(optPoint);
            if (resPoint.Status != PromptStatus.OK)
            {
                return;
            }

            // 定义一个EntityJig派生类的实例.
            EllipseJig myJig = new EllipseJig(resPoint.Value, Vector3d.ZAxis);

            // 第一次拖拽.
            myJig.setPromptCounter(0);
            PromptResult resJig = ed.Drag(myJig);
            if (resJig.Status != PromptStatus.OK)
            {
                return;
            }

            // 第二次拖拽.
            myJig.setPromptCounter(1);
            resJig = ed.Drag(myJig);
            if (resJig.Status != PromptStatus.OK)
            {
                return;
            }

            // 第三次拖拽.
            myJig.setPromptCounter(2);
            resJig = ed.Drag(myJig);
            if (resJig.Status != PromptStatus.OK)
            {
                return;
            }

            // 第四次拖拽.
            myJig.setPromptCounter(3);
            resJig = ed.Drag(myJig);
            if (resJig.Status != PromptStatus.OK)
            {
                return;
            }

            AppendEntity(myJig.GetEntity());
 
            // 还原系统变量“ANGBASE”.
            Application.SetSystemVariable("ANGBASE", oldAngBase);
        }

        private ObjectId AppendEntity(Entity ent)
        {
            ObjectId entId;
            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId,
                    OpenMode.ForRead);
                BlockTableRecord btr = (BlockTableRecord)trans.GetObject
                    (bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                entId = btr.AppendEntity(ent);
                trans.AddNewlyCreatedDBObject(ent, true);
                trans.Commit();
            }
            return entId;
        }
    }
}
