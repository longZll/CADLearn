using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.BoundaryRepresentation;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;
using AcBr = Autodesk.AutoCAD.BoundaryRepresentation;

namespace SubSolid3d
{
    public class Sub3dSolid
    {
        // 着色边
        [CommandMethod("EdgeColor")]
        public void EdgeColorTest()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

            PromptSelectionOptions pso = new PromptSelectionOptions();
            pso.MessageForAdding = "\n请选择三维实体的边:";
            pso.SingleOnly = true;          // 单选
            pso.ForceSubSelections = true;  // 强制子实体选择

            PromptSelectionResult psr = ed.GetSelection(pso);
            if (psr.Status != PromptStatus.OK || psr.Value.Count == 0)
            {
                return;
            }

            SelectionSet sSet = psr.Value;    // 得到选择集
            SelectedObject sObj = sSet[0];    // 得到选择集的唯一对象
            // 得到选择集唯一对象的子实体集合
            SelectedSubObject[] sSubObjs = sObj.GetSubentities();
            // 得到子实体Id
            SubentityId edgeSubId = sSubObjs[0].FullSubentityPath.SubentId;

            // 检查子实体类型
            if (edgeSubId.Type != SubentityType.Edge)
            {
                ed.WriteMessage("\n没有选择三维实体的边!");
                return;
            }

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                Solid3d solid3d = tr.GetObject(sObj.ObjectId, OpenMode.ForWrite) as Solid3d;
                if (solid3d == null)
                {
                    return;
                }

                // 得到边的子实体全路径
                ObjectId[] solid3dIds = new ObjectId[] { solid3d.ObjectId };
                FullSubentityPath edgePath = new FullSubentityPath(solid3dIds, edgeSubId);

                
                // 子实体高亮
                solid3d.Highlight(edgePath, false);
                ed.GetString("\n子实体选择成功, 按回车键继续...");

                // 打开颜色对话框
                ColorDialog dialogObj = new ColorDialog();
                if (dialogObj.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                {
                    solid3d.Unhighlight(edgePath, false);
                    return;
                }
                Color newColor = dialogObj.Color;

                // 对子实体进行着色
                solid3d.SetSubentityColor(edgeSubId, newColor);
                solid3d.Unhighlight(edgePath, false);
                tr.Commit();
            }
        }

        // 拉伸面
        [CommandMethod("FaceExtrude")]
        public void FaceExtrudeTest()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

            PromptSelectionOptions pso = new PromptSelectionOptions();
            pso.MessageForAdding = "\n请选择三维实体的面:";
            pso.SingleOnly = true;            // 单选
            pso.ForceSubSelections = true;    // 强制子实体选择
            pso.SinglePickInSpace = true;     // 允许在一个“空白”处拾取
            PromptSelectionResult psr = ed.GetSelection(pso);

            if (psr.Status != PromptStatus.OK)
            {
                return;
            }

            SelectionSet sSet = psr.Value;
            SelectedObject sObj = sSet[0];
            SelectedSubObject[] sSubObjs = sObj.GetSubentities();
            SubentityId faceSubId = sSubObjs[0].FullSubentityPath.SubentId;

            if (faceSubId.Type != SubentityType.Face)
            {
                ed.WriteMessage("\n没有选择三维实体的面!");
                return;
            }

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                Solid3d solid3d = tr.GetObject(sObj.ObjectId, OpenMode.ForWrite) as Solid3d;
                if (solid3d == null)
                {
                    return;
                }
                SubentityId[] faceSubIds = new SubentityId[] { faceSubId };
                double height = 100.0;   // 拉伸高度
                double taper = 0.0;      // 拉伸角度
                solid3d.ExtrudeFaces(faceSubIds, height, taper);
                tr.Commit();
            }
        }

        // 三维实体圆角
        [CommandMethod("EdgeFillet")]
        public void FilletEdgeTest()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

            PromptSelectionOptions pso = new PromptSelectionOptions();
            pso.MessageForAdding = "\n请选择三维实体的边:";
            pso.SingleOnly = true;
            pso.ForceSubSelections = true;   // 强制进行子实体选择

            PromptSelectionResult psr = ed.GetSelection(pso);
            if (psr.Status != PromptStatus.OK || psr.Value.Count == 0)
            {
                return;
            }

            SelectionSet sSet = psr.Value;    // 得到选择集
            SelectedObject sObj = sSet[0];    // 得到选择集的唯一对象
            // 得到选择集唯一对象的子实体集合
            SelectedSubObject[] sSubObjs = sObj.GetSubentities();
            // 得到子实体Id
            SubentityId edgeSubId = sSubObjs[0].FullSubentityPath.SubentId;

            if (edgeSubId.Type != SubentityType.Edge)
            {
                ed.WriteMessage("\n没有选择三维实体的边!");
                return;
            }

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                // 得到三维实体
                Solid3d solid3dEnt = (Solid3d)tr.GetObject(sObj.ObjectId,
                    OpenMode.ForWrite);

                // 得到边的子实体全路径
                ObjectId[] objIds = new ObjectId[] { solid3dEnt.ObjectId };
                FullSubentityPath edgePath = new FullSubentityPath(objIds, edgeSubId);
                // 子实体高亮
                solid3dEnt.Highlight(edgePath, false);

                // 输入圆角半径
                PromptDoubleOptions pdo = new PromptDoubleOptions("\n输入圆角半径");
                pdo.DefaultValue = 50.0;
                pdo.AllowZero = false;
                pdo.AllowNegative = false;
                PromptDoubleResult res = ed.GetDouble(pdo);

                if (res.Status != PromptStatus.OK)
                {
                    solid3dEnt.Unhighlight(edgePath, false);
                    return;
                }
                double rauidsValue = res.Value;

                DoubleCollection rauidsArr = new DoubleCollection();
                rauidsArr.Add(rauidsValue);
                DoubleCollection startDouArr = new DoubleCollection();
                startDouArr.Add(0.0);
                DoubleCollection endDouArr = new DoubleCollection();
                endDouArr.Add(0.0);

                SubentityId[] edgeIds = new SubentityId[1] { edgeSubId };
                try
                {
                    // 三维实体圆角
                    solid3dEnt.FilletEdges(edgeIds, rauidsArr, startDouArr, endDouArr);
                }
                catch
                {
                    solid3dEnt.Unhighlight(edgePath, false); // 取消子实体高亮
                    ed.WriteMessage("\n圆角失败!");
                }

                solid3dEnt.Unhighlight(edgePath, false);     // 取消子实体高亮
                tr.Commit();
            }
        }

        // 三维实体倒角
        [CommandMethod("EdgeChamfer")]
        public void ChamferEdgeTest()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

            PromptSelectionOptions pso = new PromptSelectionOptions();
            pso.MessageForAdding = "\n请选择三维实体的边:";
            pso.SingleOnly = true;
            pso.ForceSubSelections = true; // 强制进行子实体选择

            PromptSelectionResult psr = ed.GetSelection(pso);
            if (psr.Status != PromptStatus.OK || psr.Value.Count == 0)
            {
                return;
            }

            SelectionSet sSet = psr.Value;    // 得到选择集
            SelectedObject sObj = sSet[0];    // 得到选择集的唯一对象
            // 得到选择集唯一对象的子实体集合
            SelectedSubObject[] sSubObjs = sObj.GetSubentities();
            // 得到子实体Id
            SubentityId edgeSubId = sSubObjs[0].FullSubentityPath.SubentId;

            if (edgeSubId.Type != SubentityType.Edge)
            {
                ed.WriteMessage("\n没有选择三维实体的边!");
                return;
            }

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                Solid3d solid3dEnt = (Solid3d)tr.GetObject(sObj.ObjectId, OpenMode.ForWrite);

                // 得到边的子实体全路径
                ObjectId[] objIds = new ObjectId[] { solid3dEnt.ObjectId };
                FullSubentityPath edgePath = new FullSubentityPath(objIds, edgeSubId);

                // 得到“边”
                Edge edgeSubEnt = new Edge(edgePath);
                // 得到“边环”集合
                EdgeLoopCollection loops = edgeSubEnt.Loops;
                // 得到“边环”遍历器
                EdgeLoopEnumerator edgeLoopEnumerator = loops.GetEnumerator();
                // 声明“面”子实体全路径
                FullSubentityPath facePath = FullSubentityPath.Null;

                bool isNext = false;
                while (!isNext)
                {
                    // “边环”遍历器复位
                    edgeLoopEnumerator.Reset();
                    // “边环”遍历器跳到下一个节点
                    bool moveNext = edgeLoopEnumerator.MoveNext();

                    while (moveNext)
                    {
                        // 得到“边环”遍历器的当前节点
                        BoundaryLoop loopSubEnt = edgeLoopEnumerator.Current;
                        // 由“边环”得到“面”
                        AcBr.Face faceSubEnt = loopSubEnt.Face;
                        // 得到“面”子实体全路径
                        facePath = faceSubEnt.SubentityPath;
                        // 高亮面
                        solid3dEnt.Highlight(facePath, false);

                        PromptKeywordOptions optKey = new PromptKeywordOptions
                            ("\n请指定曲面选项[下一个(N)/当前(OK)]<OK>");
                        optKey.Keywords.Add("N", "N", "N", false, true);
                        optKey.Keywords.Add("O", "O", "O", false, true);
                        optKey.Keywords.Default = "O";
                        PromptResult resKey = ed.GetKeywords(optKey);

                        if (resKey.Status == PromptStatus.Cancel)
                        {
                            // 取消, 则退出高亮.
                            solid3dEnt.Unhighlight(facePath, false);
                            return;
                        }

                        if (resKey.Status == PromptStatus.OK)
                        {
                            switch (resKey.StringResult)
                            {
                                case "N":
                                    // 选择“下一个”, 则当前面退出高亮.
                                    solid3dEnt.Unhighlight(facePath, false);
                                    // 并将“边环”遍历器指向下一个节点.
                                    moveNext = edgeLoopEnumerator.MoveNext();
                                    break;
                                case "O":
                                    // 选择“当前”, 则跳出循环.
                                    moveNext = false;
                                    isNext = true;
                                    break;
                            }
                        }
                    }
                }

                // 输入基面倒角距离
                PromptDoubleOptions pdo = new PromptDoubleOptions("\n指定基面倒角距离");
                pdo.DefaultValue = 10.0;
                pdo.AllowZero = false;
                pdo.AllowNegative = false;
                PromptDoubleResult res = ed.GetDouble(pdo);

                if (res.Status != PromptStatus.OK)
                {
                    solid3dEnt.Unhighlight(facePath, false);
                    return;
                }
                double baseDisValue = res.Value;

                // 输入其他曲面倒角距离
                pdo = new PromptDoubleOptions("\n指定其他曲面倒角距离");
                pdo.DefaultValue = 50.0;
                pdo.AllowZero = false;
                pdo.AllowNegative = false;
                res = ed.GetDouble(pdo);

                if (res.Status != PromptStatus.OK)
                {
                    solid3dEnt.Unhighlight(facePath, false);
                    return;
                }
                double otherDisValue = res.Value;

                // 倒角边的子实体Id数组
                SubentityId[] edgeIds = new SubentityId[1] { edgeSubId };
                // 基面的面子实体Id
                SubentityId baseFaceId = facePath.SubentId;
                try
                {
                    // 三维实体倒角
                    solid3dEnt.ChamferEdges(edgeIds, baseFaceId, baseDisValue, otherDisValue);
                }
                catch
                {
                    ed.WriteMessage("\n倒角失败!");
                    solid3dEnt.Unhighlight(facePath, false);
                }

                solid3dEnt.Unhighlight(facePath, false);
                tr.Commit();
            }
        }
    }
}



