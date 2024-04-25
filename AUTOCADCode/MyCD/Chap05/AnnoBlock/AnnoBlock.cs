using System.Collections.Generic;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using DotNetARX;

namespace AnnoBlock
{
    public class AnnoBlock
    {
        /// <summary>
        /// 为当前图形添加一个新的注释比例，图纸单位为1，图形单位为22
        /// </summary>
        [CommandMethod("AddNewAnnotationScale")]
        public void AddNewAnnotationScale()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            //为当前图形添加一个新的注释比例，图纸单位为1，图形单位为22
            AnnotationScale scale = db.AddScale("Myscale 1:22", 1, 22);

            db.Cannoscale = scale;//设置图形的默认注释比例

            Application.ShowAlertDialog(scale.CollectionName);
        }

        /// <summary>
        /// 为选中的对象的附加三种注释比例
        /// </summary>
        [CommandMethod("AttachScale")]
        public void AttachScale()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            //Editor ed=db.GetEditor();
            //提示用户选择需要添加比例的对象
            PromptEntityOptions opt = new PromptEntityOptions("\n请选择需要添加比例的对象：");
            PromptEntityResult result = ed.GetEntity(opt);

            if (result.Status != PromptStatus.OK) return;

            ObjectId id = result.ObjectId;

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                //为对象添加“1:1”、“1:2”、“Myscale 1:22”三种比例
                id.AttachScale("1:1", "1:2", "Myscale 1:22");
                trans.Commit();
            }
        }
    }
}
