using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.ApplicationServices;
using DotNetARX;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Internal;
using System.Drawing;
using System.IO;
namespace EditAndPreviewBlock
{
    public class EditAndPreviewBlock
    {
        [CommandMethod("GenerateBlockPreview")]
        public void GenerateBlockPreviews()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            PromptFileNameResult result = ed.GetFileNameForOpen("请选择需要预览的文件");
            if (result.Status != PromptStatus.OK) return;
            string filename=result.StringResult;
            string path = Path.GetDirectoryName(filename)+"\\"+Path.GetFileNameWithoutExtension(filename) + " icons";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                db.ImportBlocksFromDwg(result.StringResult);
                //打开块表
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
                //循环遍历块表中的块表记录
                foreach (ObjectId blockRecordId in bt)
                {
                    //打开块表记录对象
                    BlockTableRecord btr = (BlockTableRecord)trans.GetObject(blockRecordId, OpenMode.ForRead);
                    //在组合框中只加入非匿名块和非布局块的名称
                    if (btr.IsAnonymous || btr.IsLayout || !btr.HasPreviewIcon) continue;
                    Bitmap preview=BlockThumbnailHelper.GetBlockThumbanail(btr.ObjectId);                    
                    preview.Save(path+"\\"+btr.Name + ".bmp");
                }                
                trans.Commit();
            }
        }
    }
}
