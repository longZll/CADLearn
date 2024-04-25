using System.Collections.Generic;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.BoundaryRepresentation;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using AcBr = Autodesk.AutoCAD.BoundaryRepresentation;
using AcGe = Autodesk.AutoCAD.Geometry;

namespace BrepTest
{
    public class TestBrep
    {
        // 组合体、壳、面、边、顶点的数量及所有顶点的坐标
        [CommandMethod("BrepNum")]
        public void BrepNnmTest()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            Database db = HostApplicationServices.WorkingDatabase;

            PromptEntityOptions opt = new PromptEntityOptions("\n选择三维实体或面域");
            opt.SetRejectMessage("您选择的不是三维实体和面域，请重新选择!");
            opt.AddAllowedClass(typeof(Solid3d), true);
            opt.AddAllowedClass(typeof(Region), true);
            PromptEntityResult res = ed.GetEntity(opt);
            if (res.Status != PromptStatus.OK)
            {
                return;
            }

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                Entity ent = (Entity)trans.GetObject(res.ObjectId, OpenMode.ForRead);
                Brep brepEnt = new Brep(ent);

                int i = 0;
                // 组合体集合
                BrepComplexCollection complexCol = brepEnt.Complexes;
                // 组合体遍历器
                BrepComplexEnumerator complexEnumerator = complexCol.GetEnumerator();
                while (complexEnumerator.MoveNext())
                {
                    i++;
                }
                ed.WriteMessage("\n组合体(Complex)数: {0}", i);

                i = 0;
                // 壳集合
                BrepShellCollection shellCol = brepEnt.Shells;
                // 壳遍历器
                BrepShellEnumerator shellEnumerator = shellCol.GetEnumerator();
                while (shellEnumerator.MoveNext())
                {
                    i++;
                }
                ed.WriteMessage("\n壳(Shell)数: " + i);

                i = 0;
                // 面集合
                BrepFaceCollection faceCol = brepEnt.Faces;
                // 面遍历器
                BrepFaceEnumerator faceEnumerator = faceCol.GetEnumerator();
                while (faceEnumerator.MoveNext())
                {
                    i++;
                }
                ed.WriteMessage("\n面(Face)数: " + i);

                i = 0;
                try
                {
                    // 边集合
                    BrepEdgeCollection edgeCol = brepEnt.Edges;
                    // 边遍历器
                    BrepEdgeEnumerator edgeEnumerator = edgeCol.GetEnumerator();
                    while (edgeEnumerator.MoveNext())
                    {
                        i++;
                    }
                    ed.WriteMessage("\n边(Edge)数: " + i);
                }
                catch
                {
                    ed.WriteMessage("\n退化的拓扑结构, 边不存在!");
                }

                i = 0;
                List<Point3d> pts = new List<Point3d>();
                try
                {
                    // 顶点集合
                    BrepVertexCollection VertexCol = brepEnt.Vertices;
                    // 顶点遍历器
                    BrepVertexEnumerator vertexEnumerator = VertexCol.GetEnumerator();
                    while (vertexEnumerator.MoveNext())
                    {
                        i++;
                        // 得到顶点
                        AcBr.Vertex vertex = vertexEnumerator.Current;
                        // 得到顶点坐标
                        Point3d pt = vertex.Point;
                        pts.Add(pt);
                    }
                    ed.WriteMessage("\n顶点(Vertex)数: " + i);
                    ed.WriteMessage("\n");

                    for (int j = 0; j < pts.Count; j++)
                    {
                        ed.WriteMessage("\n第{0}个顶点(Vertex)坐标: {1},{2},{3}", j + 1,
                            pts[j].X, pts[j].Y, pts[j].Z);
                    }
                }
                catch
                {
                    ed.WriteMessage("\n退化的拓扑结构, 顶点不存在!");
                }
                trans.Commit();
            }
        }

        [CommandMethod("GetCylRadius")]
        public void GetCylRadiusTest()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            Database db = HostApplicationServices.WorkingDatabase;

            // 选择三维实体的面
            PromptSelectionOptions pso = new PromptSelectionOptions();
            pso.MessageForAdding = "\n请选择三维实体的面";
            pso.SingleOnly = true;
            pso.ForceSubSelections = true;
            pso.SinglePickInSpace = true;

            PromptSelectionResult psr = ed.GetSelection(pso);
            if (psr.Status != PromptStatus.OK)
            {
                return;
            }

            SelectionSet sSet = psr.Value;
            SelectedSubObject[] sSubObjArr = psr.Value[0].GetSubentities();
            FullSubentityPath facePath = sSubObjArr[0].FullSubentityPath;
            SubentityId faceSubId = facePath.SubentId;

            if (faceSubId.Type != SubentityType.Face)
            {
                ed.WriteMessage("\n没有选择三维实体的面!");
                return;
            }

            AcBr.Face face = new AcBr.Face(facePath);
            AcGe.Surface surf = face.Surface;
            ExternalBoundedSurface ebSurf = surf as ExternalBoundedSurface;

            if (ebSurf != null && ebSurf.IsCylinder)
            {
                Cylinder cyl = ebSurf.BaseSurface as Cylinder;

                if (cyl != null)
                {
                    double radius = cyl.Radius;
                    ed.WriteMessage("\n圆柱面半径 = {0}", radius);
                }
            }
            else
            {
                ed.WriteMessage("\n选择的不是圆柱面!");
            }
        }

        [CommandMethod("GetAllCylRadius")]
        public void GetAllCylRadiusTest()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            Database db = HostApplicationServices.WorkingDatabase;

            PromptEntityOptions opt = new PromptEntityOptions("\n选择三维实体");
            opt.SetRejectMessage("您选择的不是三维实体，请重新选择!");
            opt.AddAllowedClass(typeof(Solid3d), true);

            PromptEntityResult res = ed.GetEntity(opt);
            if (res.Status != PromptStatus.OK)
            {
                return;
            }

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                Entity ent = (Entity)trans.GetObject(res.ObjectId, OpenMode.ForWrite);
                Brep brepEnt = new Brep(ent);

                int i = 0;
                BrepFaceEnumerator faceEnumerator = brepEnt.Faces.GetEnumerator();
                while (faceEnumerator.MoveNext())
                {
                    AcGe.Surface surf = faceEnumerator.Current.Surface;
                    ExternalBoundedSurface ebSurf = surf as ExternalBoundedSurface;

                    if (ebSurf != null && ebSurf.IsCylinder)
                    {
                        Cylinder cyl = ebSurf.BaseSurface as Cylinder;

                        if (cyl != null)
                        {
                            double radius = cyl.Radius;
                            ed.WriteMessage("\n圆柱面半径 = {0}", radius);
                        }
                    }
                    i++;
                }
            }
        }

        [CommandMethod("test2")]
        public void MyTest2()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            Database db = HostApplicationServices.WorkingDatabase;

            PromptEntityOptions optEnt = new PromptEntityOptions("\n选择面域");
            optEnt.SetRejectMessage("您选择的不是面域，请重新选择!");
            optEnt.AddAllowedClass(typeof(Region), true);

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
                Region regEnt = (Region)trans.GetObject(resEnt.ObjectId, OpenMode.ForRead);
                Brep brepEnt = new Brep(regEnt);

                BrepFaceEnumerator faceEnumerator = brepEnt.Faces.GetEnumerator();
                while (faceEnumerator.MoveNext())
                {
                    PointContainment containment;
                    faceEnumerator.Current.GetPointContainment(pt, out containment);

                    if (containment == PointContainment.Inside)
                    {
                        ed.WriteMessage("\n该点在面域的内部!");
                        return;
                    }

                    if (containment == PointContainment.OnBoundary)
                    {
                        ed.WriteMessage("\n该点在面域的边界上!");
                        return;
                    }
                } // end while
                ed.WriteMessage("\n该点在面域的外部!");
                trans.Commit();
            }
        }

        [CommandMethod("Test")]
        public void Test()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

            PromptEntityOptions opt1 = new PromptEntityOptions("\n请选择面域: ");
            opt1.SetRejectMessage("\n必须选择面域!");
            opt1.AddAllowedClass(typeof(Region), true);

            PromptEntityResult res1 = ed.GetEntity(opt1);
            if (res1.Status != PromptStatus.OK)
            {
                return;
            }
            ObjectId regId = res1.ObjectId;

            PromptEntityOptions opt2 = new PromptEntityOptions("\n请选择直线: ");
            opt2.SetRejectMessage("\n必须选择直线!");
            opt2.AddAllowedClass(typeof(Line), true);

            PromptEntityResult res2 = ed.GetEntity(opt2);
            if (res2.Status != PromptStatus.OK)
            {
                return;
            }
            ObjectId lineId = res2.ObjectId;

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                Region regEnt = (Region)trans.GetObject(res1.ObjectId, OpenMode.ForRead);
                Line lineEnt = (Line)trans.GetObject(res2.ObjectId, OpenMode.ForRead);

                Brep brp = new Brep(regEnt);
                LineSegment3d ln = new LineSegment3d(lineEnt.StartPoint, lineEnt.EndPoint);
                int maxNum = 10;
                Hit[] hits = brp.GetLineContainment(ln, maxNum);
                if (hits != null && hits.Length > 0)
                {
                    foreach (Hit hit in hits)
                    {
                        ed.WriteMessage("\n面域和直线的交点坐标: {0}", hit.Point);
                    }
                }
                else
                {
                    ed.WriteMessage("\n面域和直线无交点!");
                }
                trans.Commit();
            }
        }

    }
}

