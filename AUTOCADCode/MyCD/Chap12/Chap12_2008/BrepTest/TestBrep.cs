using ahlzl;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.DatabaseServices;

namespace BrepTest
{
    public class TestBrep
    {
        [CommandMethod("BrepNum")]
        public void BrepNumTest()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

            int complexCount, shellCount, faceCount, edgeCount, vertexCount;
            Point3d[] ptArr;

            bool isOK = Express3D.GetBrepVertex(out complexCount, out shellCount, out faceCount,
                out edgeCount, out vertexCount, out ptArr);

            if (!isOK)
            {
                return;
            }

            ed.WriteMessage("\ncomplex(组合体)数: {0}", complexCount);
            ed.WriteMessage("\nshell(壳)数: {0}", shellCount);
            ed.WriteMessage("\nface(面)数: {0}", faceCount);
            ed.WriteMessage("\nedge(边)数: {0}", edgeCount);
            ed.WriteMessage("\nvertex(顶点)数: {0}", vertexCount);
            ed.WriteMessage("\n");

            if (ptArr.Length > 0)
            {
                for (int i = 0; i < ptArr.Length; i++)
                {
                    ed.WriteMessage("\n第{0}个顶点的坐标：{1},{2},{3}", i + 1, ptArr[i].X, ptArr[i].Y, ptArr[i].Z);
                }
            }
        }

        [CommandMethod("GetCylRadius")]
        public void GetCylRadiusTest()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

            double[] radiusArr = Express3D.GetCylRadius();

            if (radiusArr != null && radiusArr.Length > 0)
            {
                for (int i = 0; i < radiusArr.Length; i++)
                {
                    ed.WriteMessage("\n第{0}个圆柱面的半径：{1}", i + 1, radiusArr[i]);
                }
            }
            else
            {
                ed.WriteMessage("\n选择的不是圆柱面!");
            }
        }

        [CommandMethod("test2")]
        public void MyTest2()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            Database db = HostApplicationServices.WorkingDatabase;

            PromptEntityOptions optEnt = new PromptEntityOptions("\n选择三维实体");
            optEnt.SetRejectMessage("您选择的不是三维实体，请重新选择!");
            optEnt.AddAllowedClass(typeof(Solid3d), true);

            PromptEntityResult resEnt = ed.GetEntity(optEnt);
            if (resEnt.Status != PromptStatus.OK)
            {
                return;
            }
            ObjectId entId = resEnt.ObjectId;

            PromptPointOptions optPoint = new PromptPointOptions("\n请指定点");
            PromptPointResult resPoint = ed.GetPoint(optPoint);

            if (resPoint.Status != PromptStatus.OK)
            {
                return;
            }
            Point3d pt = resPoint.Value; 

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                Solid3d solid3d = (Solid3d)trans.GetObject(resEnt.ObjectId, OpenMode.ForRead);

                switch (Express3D.PttoSolid3d(pt, solid3d))
                {
                    case 0:
                        ed.WriteMessage("\n该点在三维实体的边界上!"); 
                        break;
                    case 1:
                        ed.WriteMessage("\n该点在三维实体的外部!");
                        break;
                    case -1:
                        ed.WriteMessage("\n该点在三维实体的内部!");
                        break;
                    case 99:
                        ed.WriteMessage("\n错误!");
                        break;
                }
                trans.Commit();
            }
        }

        [CommandMethod("test")]
        public void MyTest()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            Database db = HostApplicationServices.WorkingDatabase;

            PromptEntityOptions optEnt = new PromptEntityOptions("\n选择三维实体");
            optEnt.SetRejectMessage("您选择的不是三维实体，请重新选择!");
            optEnt.AddAllowedClass(typeof(Region), true);

            PromptEntityResult resEnt = ed.GetEntity(optEnt);
            if (resEnt.Status != PromptStatus.OK)
            {
                return;
            }
            ObjectId entId = resEnt.ObjectId;


            PromptEntityOptions optEnt2 = new PromptEntityOptions("\n选择直线");
            optEnt2.SetRejectMessage("您选择的不是直线，请重新选择!");
            optEnt2.AddAllowedClass(typeof(Line), true);

            PromptEntityResult resEnt2 = ed.GetEntity(optEnt2);
            if (resEnt2.Status != PromptStatus.OK)
            {
                return;
            }
            ObjectId entId2 = resEnt2.ObjectId;

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                Region solid3d = (Region)trans.GetObject(resEnt.ObjectId, OpenMode.ForRead);
                Line line = (Line)trans.GetObject(resEnt2.ObjectId, OpenMode.ForRead);

               Point3d[] ss = Express3D.ABC(line, solid3d);
        
                trans.Commit();
            }
        }
    }
}
