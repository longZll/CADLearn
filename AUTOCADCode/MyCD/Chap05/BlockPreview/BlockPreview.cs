using System.Drawing;
using System.IO;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Internal;
using Autodesk.AutoCAD.Runtime;
using DotNetARX;
namespace BlockPreview
{
    public class BlockPreview
    {
        /// <summary>
        /// 生成块预览
        /// </summary>
        [CommandMethod("GenerateBlockPreview")]
        public void GenerateBlockPreview()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            // 提示用户选择文件
            PromptFileNameResult result = ed.GetFileNameForOpen("请选择需要预览的文件");
            if (result.Status != PromptStatus.OK) return; // 如果未选择，则返回

            string filename = result.StringResult;  //获取带有路径的文件名

            // 在C盘根目录下创建一个临时文件夹，用来存放文件中的块预览图标
            string path = "C:\\Temp";

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                // 导入外部文件中的块
                db.ImportBlocksFromDwg(filename);

                //打开块表
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);

                // 循环遍历块表中的所有块表记录
                foreach (ObjectId blockRecordId in bt)
                {
                    // 打开块表记录对象
                    BlockTableRecord btr = (BlockTableRecord)trans.GetObject(blockRecordId, OpenMode.ForRead);
                    
                    // 如果是匿名块、布局块及没有预览图形的块，则返回
                    if (btr.IsAnonymous || btr.IsLayout || !btr.HasPreviewIcon) continue;
                    
                    // 获取块预览图案（适用于AutoCAD 2008及以下版本）
                    //Bitmap preview = BlockThumbnailHelper.GetBlockThumbanail(btr.ObjectId);

                    Bitmap preview = btr.PreviewIcon; //适用于AutoCAD 2009及以上版本
                    
                    preview.Save(path + "\\" + btr.Name + ".bmp"); // 保存块预览图案
                }
                trans.Commit();
            }
        }
    }
}
