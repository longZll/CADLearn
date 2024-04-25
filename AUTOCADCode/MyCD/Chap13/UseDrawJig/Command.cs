using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Geometry;

namespace UseDrawJig
{
    public class Command
    {
[CommandMethod("JigElRec")]
public void JigElRecTest()
{
    Database db = HostApplicationServices.WorkingDatabase;
    Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            
    PromptPointOptions optPoint = new PromptPointOptions
        ("\n请指定椭圆外切矩形的一个角点");
    PromptPointResult resPoint = ed.GetPoint(optPoint);
    if (resPoint.Status != PromptStatus.OK)
    {
        return;
    }

    // 得到第一个角点的UCS坐标.
    Point3d pt1 = resPoint.Value;

    // 加载线型.
    try
    {
        db.LoadLineTypeFile("DASHED", "acadiso.lin");
    }
    catch{}

    // 初始化矩形
    Polyline polyLine = new Polyline();
    for (int i = 0; i < 4; i++)
    {
        polyLine.AddVertexAt(i, new Point2d(0, 0), 0, 0, 0);
    }
    polyLine.Closed = true;
    polyLine.Linetype = "DASHED";

    // 初始化椭圆.
    Ellipse ellipse = new Ellipse(Point3d.Origin, Vector3d.ZAxis,
        new Vector3d(0.000001, 0, 0), 1, 0, 0);


    ElRecJig elRecJig = new ElRecJig(pt1, ellipse, polyLine);

    PromptResult resJig = ed.Drag(elRecJig);
    if (resJig.Status == PromptStatus.OK)
    {
        AppendEntity(elRecJig.m_Ellipse);
    }
}


        [CommandMethod("JigMirror")]
        public void JigMirrorTest()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

            // 选择集操作
            PromptSelectionOptions selOpt = new PromptSelectionOptions();
            selOpt.MessageForAdding = "选择对象";
            PromptSelectionResult selRes = ed.GetSelection(selOpt);

            if (selRes.Status != PromptStatus.OK)
            {
                return;
            }
            SelectionSet sSet = selRes.Value;
            ObjectId[] ids = sSet.GetObjectIds();

            Entity[] entArr = new Entity[ids.Length];
            Entity[] entCopyArr = new Entity[ids.Length];

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                // 源对象高亮，并复制.
                for (int i = 0; i < ids.Length; i++)
                {
                    entArr[i] = (Entity)trans.GetObject(ids[i], OpenMode.ForWrite);
                    entArr[i].Highlight();
                    entCopyArr[i] = (Entity)entArr[i].Clone();
                }

                PromptPointResult resPoint = ed.GetPoint("\n请指定镜像线第一点");
                if (resPoint.Status != PromptStatus.OK)
                {
                    // 取消源对象的高亮
                    for (int i = 0; i <= ids.Length - 1; i++)
                    {
                        entArr[i].Unhighlight();
                    }
                    return;
                }

                // 得到镜像线的第一点(GetPoint函数得到的是UCS点）
                Point3d mirrorPt1 = resPoint.Value;
                // 实例化MirrorJig
                MirrorJig mirrorJig = new MirrorJig(mirrorPt1, entArr, entCopyArr);
                // 拖拽.
                PromptResult jigRes = ed.Drag(mirrorJig);
                if (jigRes.Status == PromptStatus.OK)
                {
                    PromptKeywordOptions keyOpt = new PromptKeywordOptions
                        ("\n是否删除源对象[是(Y)/否(N)]<N>");
                    keyOpt.Keywords.Add("Y", "Y", "Y", false, true);
                    keyOpt.Keywords.Add("N", "N", "N", false, true);
                    keyOpt.Keywords.Default = "N";
                    PromptResult keyRes = ed.GetKeywords(keyOpt);

                    if (keyRes.Status == PromptStatus.OK)
                    {
                        if (keyRes.StringResult == "Y")
                        {
                            // 删除源对象.
                            for (int i = 0; i <= ids.Length - 1; i++)
                            {
                                entArr[i].Erase();
                            }
                        }
                        else
                        {
                            mirrorJig.Unhighlight();
                        }

                        for (int i = 0; i < ids.Length; i++)
                        {
                            AppendEntity(entCopyArr[i]);
                        }
                    }
                    else
                    {
                        mirrorJig.Unhighlight();
                    }
                }
                else
                {
                    mirrorJig.Unhighlight();
                }
                trans.Commit();
            }
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
