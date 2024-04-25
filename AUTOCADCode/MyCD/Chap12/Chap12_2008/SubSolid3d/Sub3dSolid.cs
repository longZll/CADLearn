using System;
using ahlzl;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;

namespace SubSolid3d
{
    public class Sub3dSolid
    {
        // 认识gs标记值
        [CommandMethod("GetGs")]
        public void GetGsTest()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

            ObjectId solid3dId = ObjectId.Null;
            int gsMarker = 0;
            string prompt = "\n请选择三维实体的边:";

            // 得到ObjectId和边的gs标记
            bool isOK = Express3D.GetgsMarker(prompt, ref solid3dId, ref gsMarker);
            if (!isOK || solid3dId == ObjectId.Null || gsMarker == 0)
            {
                return;
            }
            ed.WriteMessage("\ngs = {0}", gsMarker);
        }

        // 着色边
        [CommandMethod("EdgeColor")]
        public void EdgeColorTest()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

            ObjectId solid3dId = ObjectId.Null;
            int gsMarker = 0;
            string prompt = "\n请选择三维实体的边:";

            // 得到ObjectId和边的gs标记
            bool isOK = Express3D.GetgsMarker(prompt, ref solid3dId, ref gsMarker);
            if (!isOK || solid3dId == ObjectId.Null || gsMarker == 0)
            {
                return;
            } 
        
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                Solid3d solid3d = tr.GetObject(solid3dId, OpenMode.ForWrite) as Solid3d;
                if (solid3d == null)
                {
                    return;
                }

                // 得到边子实体全路径集合
                Point3d pickPoint = new Point3d(0, 0, 0);
                Matrix3d xform = Matrix3d.Identity;
                IntPtr resbuf = IntPtr.Zero;
                FullSubentityPath[] edgePathes = null;
                try
                {
                    edgePathes = solid3d.GetSubentityPathsAtGraphicsMarker
                        (SubentityType.Edge, gsMarker, pickPoint, xform, 0, null);
                }
                catch
                {
                    return;
                }

                // 高亮边.
                solid3d.Highlight(edgePathes[0], false);
                ed.GetString("\n子实体选择成功, 按回车键继续...");
                solid3d.Unhighlight(edgePathes[0], false);

                // 打开颜色对话框
                ColorDialog dialogObj = new ColorDialog();
                if (dialogObj.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                {
                    return;
                }
                Color newColor = dialogObj.Color;

                // 着色边.
                SubentityId edgeId = edgePathes[0].SubentId;
                solid3d.SetSubentityColor(edgeId, newColor);
                tr.Commit();
            }    
        }

        // 着色面
        [CommandMethod("FaceColor")]
        public void FaceColorTest()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

            ObjectId solid3dId;
            SubentityId[] subIdArr;
            bool isOK = Express3D.SelSolid3dFace(out solid3dId, out subIdArr);

            if (!isOK || solid3dId == ObjectId.Null || subIdArr.Length == 0)
            {
                return;
            }

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                Solid3d solid3d = (Solid3d)tr.GetObject(solid3dId, OpenMode.ForWrite);

                // 打开颜色对话框
                ColorDialog dialogObj = new ColorDialog();
                if (dialogObj.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                {
                    return;
                }
                Color newColor = dialogObj.Color;

                for (int i = 0; i < subIdArr.Length; i++)
                {
                    solid3d.SetSubentityColor(subIdArr[i], newColor);
                }
                tr.Commit();
            }
        }

        // 拉伸面
        [CommandMethod("FaceExtrude")]
        public void FaceExtrudeTest()
        {
            Express3D.Solid3dExtrudeFaces(200.0, 0.0);
        }

        // 三维实体圆角
        [CommandMethod("EdgeFillet")]
        public void FilletEdgeTest()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

            string prompt = "\n请选择三维实体的边";
            ObjectId solid3dId = ObjectId.Null;
            int gsMarker = 0;

            // 得到ObjectId和gs标记
            Express3D.GetgsMarker(prompt, ref solid3dId, ref gsMarker);

            if (solid3dId == ObjectId.Null || gsMarker == 0)
            {
                ed.WriteMessage("\n选择失败!");
            }

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                Solid3d solid3dEnt = trans.GetObject(solid3dId, OpenMode.ForWrite) as Solid3d;
                if (solid3dEnt == null)
                {
                    ed.WriteMessage("\n没用选择三维实体!");
                    return;
                }

                Point3d pickPoint = new Point3d(0, 0, 0);
                Matrix3d xform = Matrix3d.Identity;
                IntPtr resbuf = IntPtr.Zero;
                FullSubentityPath[] edgePathes = null;
                try
                {
                    edgePathes = solid3dEnt.GetSubentityPathsAtGraphicsMarker
                        (SubentityType.Edge, gsMarker, pickPoint, xform, 0, null);
                }
                catch
                {
                    ed.WriteMessage("\n选择失败!");
                    return;
                }

                // 高亮边.
                solid3dEnt.Highlight(edgePathes[0], false);


                // 输入圆角半径
                PromptDoubleOptions pdo = new PromptDoubleOptions("\n输入圆角半径");
                pdo.DefaultValue = 50.0;
                pdo.AllowZero = false;
                pdo.AllowNegative = false;
                PromptDoubleResult res = ed.GetDouble(pdo);

                if (res.Status != PromptStatus.OK)
                {
                    solid3dEnt.Unhighlight(edgePathes[0], false);
                    return;
                }
                double rauidsValue = res.Value;

                // 圆角
                Express3D.FilletEdge(solid3dId, gsMarker, rauidsValue);
                solid3dEnt.Unhighlight(edgePathes[0], false);
                trans.Commit();
            }       
        }

        // 三维实体倒角
        [CommandMethod("EdgeChamfer")]
        public void ChamferEdgeTest()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

            string prompt = "\n请选择三维实体的边";
            ObjectId solid3dId = ObjectId.Null;
            int gsMarker = 0;
            bool isOK = Express3D.GetgsMarker(prompt, ref solid3dId, ref gsMarker);

            if (!isOK || solid3dId == ObjectId.Null || gsMarker == 0)
            {
                ed.WriteMessage("\n选择失败!");
            }

            Express3D.ChamferEdge(solid3dId, gsMarker, 20.0, 50.0);
        }
    }
}


