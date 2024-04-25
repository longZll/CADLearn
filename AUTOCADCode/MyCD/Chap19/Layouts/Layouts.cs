using System.IO;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using DotNetARX;
namespace Layouts
{
    public class Layouts
    {
        [CommandMethod("CopyLayout")]
        public void CopyLayout()
        {
            // 打开模板图形
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            string fileName = Tools.GetCurrentPath() + "\\布局模板图形.dwg";
            if (!File.Exists(fileName))
            {
                ed.WriteMessage("\n未找到需要的模板图形.");
                return;
            }
            Database templateDb = new Database(false, false);
            templateDb.ReadDwgFile(fileName, FileShare.ReadWrite, true, "");
            Database db = HostApplicationServices.WorkingDatabase;
            string layoutName = "新建打印布局";
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                // 创建新布局
                LayoutManager lm = LayoutManager.Current;
                ObjectId layoutId = lm.GetLayoutId(layoutName);
                if (layoutId != ObjectId.Null) lm.DeleteLayout(layoutName);
                layoutId = lm.CreateLayout(layoutName);
                Layout layout = (Layout)layoutId.GetObject(OpenMode.ForWrite);
                layout.Initialize(); 
                lm.CurrentLayout = layoutName;
                // 删除布局中的所有实体
                ObjectIdCollection entIds = layout.GetEntsInLayout(true);
                foreach (ObjectId entId in entIds)
                {
                    Entity ent = entId.GetObject(OpenMode.ForWrite) as Entity;
                    ent.Erase();
                }
                // 获得模板图中的布局ID
                ObjectIdCollection entIdsInLayout = new ObjectIdCollection();
                ObjectId srcLayoutId = templateDb.GetLayoutId("模板布局", ref entIdsInLayout);
                ObjectId newBtrId = new ObjectId();
                if (srcLayoutId.IsValid)
                {
                    // 复制布局中的所有实体
                    Layout srcLayout = (Layout)srcLayoutId.GetObject(OpenMode.ForRead);
                    layout.CopyFrom(srcLayout);
                    newBtrId = layout.BlockTableRecordId;
                }
                // 将模板布局中的实体复制到新布局
                if (entIdsInLayout.Count > 0)
                {
                    IdMapping idMap = new IdMapping();
                    templateDb.WblockCloneObjects(entIdsInLayout, newBtrId, idMap, DuplicateRecordCloning.Replace, false);
                }
                trans.Commit();
            }
            templateDb.Dispose();
            // 布局和视口居中显示
            db.CenterLayoutViewport(layoutName);
        }
    }
}
