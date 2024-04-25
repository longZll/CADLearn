using System;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using DotNetARX;
using ahlzl;

namespace DwawSolid3d
{
    public class Dwaw3dSolid
    {
        [CommandMethod("CreateBase3D")]
        public void CreateBase3DTest()
        {
            // 创建长方体
            
            Draw3DTools.AddBox(new Point3d(2.0, 3.0, 0.0), -30.0, 20.0, 10.0);
            // 创建球体
            Draw3DTools.AddSphere(new Point3d(25.0, -35.0, 0.0), 15.0);
            // 创建圆柱体
            Draw3DTools.AddCylinder(new Point3d(50.0, 60.0, 0.0), 15.0, 30.0);
            // 创建圆锥体 
            Draw3DTools.AddCone(new Point3d(-20.0, -45.0, 0.0), 15.0, 30.0);
            // 创建圆环体
            Draw3DTools.AddTorus(new Point3d(80.0, 0.0, 0.0), 20.0, 3.0);
            // 创建楔体
            Draw3DTools.AddWedge(new Point3d(20.0, 50.0, 0.0), -30.0, 20.0, 10.0);
            // 创建棱柱
            Draw3DTools.AddPrism(new Point3d(90.0, 60.0, 0.0), 20, 5, 10.0);
            // 创建棱锥
            Draw3DTools.AddPyramid(new Point3d(90.0, -60.0, 0.0), -20, 5, 10.0);
        }

        // 普通拉伸
        [CommandMethod("CreateExt1")]
        public void CreateExtTest1()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            Database db = HostApplicationServices.WorkingDatabase;
            Matrix3d mt = ed.CurrentUserCoordinateSystem;

            Line lineEnt = new Line(new Point3d(10.0, 10.0, 0), new Point3d(20.0, 10.0,
        0.0));
            Arc arcEnt = new Arc(new Point3d(15.0, 10.0, 0.0), Vector3d.ZAxis, 5.0, 0.0,
        Math.PI);
            lineEnt.TransformBy(mt);
            arcEnt.TransformBy(mt);

            DBObjectCollection ents = new DBObjectCollection();
            ents.Add(lineEnt);
            ents.Add(arcEnt);

            DBObjectCollection regions = Region.CreateFromCurves(ents);
            Region regionEnt = (Region)regions[0];

            Draw3DTools.AddExtrudedSolid(regionEnt, 20.0, 5 * Math.PI / 180);
            lineEnt.Dispose();
            arcEnt.Dispose();
            regionEnt.Dispose();
        }

        // 沿路径拉伸
        [CommandMethod("CreateExt2")]
        public void CreateExtTest2()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            Database db = HostApplicationServices.WorkingDatabase;
            Matrix3d mt = ed.CurrentUserCoordinateSystem;

            Line lineEnt = new Line(new Point3d(0.0, 0.0, 0.0), new Point3d(10.0, 0.0,
        0.0));
            Arc arcEnt = new Arc(new Point3d(5.0, 0.0, 0.0), Vector3d.ZAxis, 5.0, 0.0,
        Math.PI);
            Arc pathEnt = new Arc(new Point3d(50.0, 0, 0), new Vector3d(0.0, -1.0, 0.0),
                50.0, 0.0, Math.PI);

            lineEnt.TransformBy(mt);
            arcEnt.TransformBy(mt);
            pathEnt.TransformBy(mt);

            DBObjectCollection ents = new DBObjectCollection();
            ents.Add(lineEnt);
            ents.Add(arcEnt);

            DBObjectCollection regions = Region.CreateFromCurves(ents);
            Region regionEnt = (Region)regions[0];

            Draw3DTools.AddExtrudedSolid(regionEnt, pathEnt, 2 * Math.PI / 180);

            lineEnt.Dispose();
            arcEnt.Dispose();
            pathEnt.Dispose();
            regionEnt.Dispose();
        }

        // 旋转体
        [CommandMethod("CreateRev")]
        public void CreateRevTest()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            Database db = HostApplicationServices.WorkingDatabase;
            Matrix3d mt = ed.CurrentUserCoordinateSystem;

            Line lineEnt = new Line(new Point3d(20.0, 0.0, 0.0), new Point3d(30.0, 0.0,
        0.0));
            Arc arcEnt = new Arc(new Point3d(25.0, 0.0, 0.0), Vector3d.ZAxis, 5.0, 0.0,
        Math.PI);

            lineEnt.TransformBy(mt);
            arcEnt.TransformBy(mt);

            DBObjectCollection ents = new DBObjectCollection();
            ents.Add(lineEnt);
            ents.Add(arcEnt);

            DBObjectCollection regions = Region.CreateFromCurves(ents);
            Region regionEnt = (Region)regions[0];

            Draw3DTools.AddRevolvedSolid(regionEnt, Point3d.Origin.TransformBy(mt),
                (new Point3d(10.0, 0.0, 0.0)).TransformBy(mt), 0.5 * Math.PI);

            lineEnt.Dispose();
            arcEnt.Dispose();
            regionEnt.Dispose();
        }

        // 布尔运算
        [CommandMethod("NetBool")]
        public void NetBoolTest()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

            Solid3d ent1 = new Solid3d();
            ent1.CreateBox(30.0, 20.0, 10.0);
            Point3d cenPt1 = new Point3d(15.0, 40.0, 5.0);
            Matrix3d mt1 = ed.CurrentUserCoordinateSystem;
            mt1 = mt1 * Matrix3d.Displacement(cenPt1 - Point3d.Origin);
            ent1.TransformBy(mt1);

            Solid3d ent2 = new Solid3d();
            ent2.CreateSphere(8.0);
            Point3d cenPt2 = new Point3d(30.0, 40.0, 10.0);
            Matrix3d mt2 = ed.CurrentUserCoordinateSystem;
            mt2 = mt2 * Matrix3d.Displacement(cenPt2 - Point3d.Origin);
            ent2.TransformBy(mt2);

            ent1.BooleanOperation(BooleanOperationType.BoolSubtract, ent2);

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                db.AddToModelSpace(ent1);
                tr.Commit();
            }
        }

        // 扫琼(麦比乌斯环)
        [CommandMethod("CreateSweep")]
        public void CreateSweep()
        {
            Database db = HostApplicationServices.WorkingDatabase;

            // 路径曲线
            Arc pathEnt1 = new Arc(new Point3d(0, 0, 0), 5, 0, Math.PI);
            Arc pathEnt2 = new Arc(new Point3d(0, 0, 0), 5, Math.PI, 2 * Math.PI);

            // 截面实体
            Polyline sectionEnt1 = new Polyline();
            sectionEnt1.AddVertexAt(0, new Point2d(5.1, 1.0), 0, 0, 0);
            sectionEnt1.AddVertexAt(1, new Point2d(4.9, 1.0), 0, 0, 0);
            sectionEnt1.AddVertexAt(2, new Point2d(4.9, -1.0), 0, 0, 0);
            sectionEnt1.AddVertexAt(3, new Point2d(5.1, -1.0), 0, 0, 0);
            sectionEnt1.Closed = true;

            Polyline sectionEnt2 = new Polyline();
            sectionEnt2.AddVertexAt(0, new Point2d(-4.0, 0.1), 0, 0, 0);
            sectionEnt2.AddVertexAt(1, new Point2d(-6.0, 0.1), 0, 0, 0);
            sectionEnt2.AddVertexAt(2, new Point2d(-6.0, -0.1), 0, 0, 0);
            sectionEnt2.AddVertexAt(3, new Point2d(-4.0, -0.1), 0, 0, 0);
            sectionEnt2.Closed = true;

            // 旋转两个矩形
            Matrix3d mt = Matrix3d.Rotation(Math.PI / 2, Vector3d.XAxis,
                Point3d.Origin);
            sectionEnt1.TransformBy(mt);
            sectionEnt2.TransformBy(mt);

            // 扫琼选项
            SweepOptionsBuilder sweepOptBu = new SweepOptionsBuilder();
            sweepOptBu.TwistAngle = Math.PI / 2;
            SweepOptions sweepOpt = sweepOptBu.ToSweepOptions();

            // 扫琼
            Solid3d sweepEnt1 = Express3D.createSweptSolid(sectionEnt1, pathEnt1, sweepOpt);
            Solid3d sweepEnt2 = Express3D.createSweptSolid(sectionEnt2, pathEnt2, sweepOpt);

            // 并集
            sweepEnt1.BooleanOperation(BooleanOperationType.BoolUnite, sweepEnt2);
            sweepEnt1.ColorIndex = 1;

            // 添加到数据库
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                db.AddToModelSpace(sweepEnt1);
                tr.Commit();
            }
        }

        // 放样(天圆地方)
        [CommandMethod("CreateLoft1")]
        public void CreateLoft1()
        {
            Database db = HostApplicationServices.WorkingDatabase;

            // 地方
            Polyline ent1 = new Polyline();
            ent1.AddVertexAt(0, new Point2d(50, 50), 0, 0, 0);
            ent1.AddVertexAt(1, new Point2d(-50, 50), 0, 0, 0);
            ent1.AddVertexAt(2, new Point2d(-50, -50), 0, 0, 0);
            ent1.AddVertexAt(3, new Point2d(50, -50), 0, 0, 0);
            ent1.Closed = true;

            // 天圆 
            Circle ent2 = new Circle(new Point3d(0, 0, 200), new Vector3d(0, 0, 1), 30);

            // 添加到实体数组
            Entity[] crossEnts = new Entity[2];
            crossEnts.SetValue(ent1, 0);
            crossEnts.SetValue(ent2, 1);

            // 空的引导线实体数组
            Entity[] guideCurs = new Entity[0];

            // 放样选项
            LoftOptions loftOpt = new LoftOptions();

            Solid3d loftEnt = Express3D.CreateLoftedSolid(crossEnts, guideCurs, null, loftOpt);
            // 添加到数据库
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                db.AddToModelSpace(loftEnt);
                tr.Commit();
            }
        }

        // 放样(方圆90度弯头过渡管)
        [CommandMethod("CreateLoft2")]
        public void CreateLoft2()
        {
            Database db = HostApplicationServices.WorkingDatabase;

            Polyline ent1 = new Polyline();
            ent1.AddVertexAt(0, new Point2d(10.0, 10.0), 0.0, 0.0, 0.0);
            ent1.AddVertexAt(1, new Point2d(-10.0, 10.0), 0.0, 0.0, 0.0);
            ent1.AddVertexAt(2, new Point2d(-10.0, -10.0), 0.0, 0.0, 0.0);
            ent1.AddVertexAt(3, new Point2d(10.0, -10.0), 0.0, 0.0, 0.0);
            ent1.Closed = true;
            Circle ent2 = new Circle(new Point3d(30.0, 0.0, 30.0), new Vector3d(1.0, 0.0, 0.0), 6.0);

            Polyline ent3 = new Polyline();
            ent3.AddVertexAt(0, new Point2d(9.5, 9.5), 0.0, 0.0, 0.0);
            ent3.AddVertexAt(1, new Point2d(-9.5, 9.5), 0.0, 0.0, 0.0);
            ent3.AddVertexAt(2, new Point2d(-9.5, -9.5), 0.0, 0.0, 0.0);
            ent3.AddVertexAt(3, new Point2d(9.5, -9.5), 0.0, 0.0, 0.0);
            ent3.Closed = true;
            Circle ent4 = new Circle(new Point3d(30.0, 0.0, 30.0), new Vector3d(1.0, 0.0, 0.0), 5.5);

            Entity[] crossEnts1 = new Entity[2];
            crossEnts1.SetValue(ent1, 0);
            crossEnts1.SetValue(ent2, 1);

            Entity[] crossEnts2 = new Entity[2];
            crossEnts2.SetValue(ent3, 0);
            crossEnts2.SetValue(ent4, 1);

            Entity[] guideCurs = new Entity[0];

            Arc pathCur = new Arc(new Point3d(30.0, 0.0, 0.0), new Vector3d(0.0, -1.0, 0.0), 30.0,
                Math.PI / 2, Math.PI);

            LoftOptions loftOpt = new LoftOptions();

            Solid3d loftEnt1 = Express3D.CreateLoftedSolid(crossEnts1, guideCurs, pathCur, loftOpt);
            Solid3d loftEnt2 = Express3D.CreateLoftedSolid(crossEnts2, guideCurs, pathCur, loftOpt);

            loftEnt1.BooleanOperation(BooleanOperationType.BoolSubtract, loftEnt2);

            // 添加到数据库
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                db.AddToModelSpace(loftEnt1);
                tr.Commit();
            }
        }
    }
}
