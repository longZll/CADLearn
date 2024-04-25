using System.Linq;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using DotNetARX;
namespace Groups
{
    public class Groups
    {
        [CommandMethod("MakeGroup")]
        public void MakeGroup()
        {
            Database db=HostApplicationServices.WorkingDatabase;
            Editor ed=Application.DocumentManager.MdiActiveDocument.Editor;
            //提示用户选择需要加入到组中的对象
            PromptSelectionOptions opt=new PromptSelectionOptions();
            opt.MessageForAdding = "\n请选择需要加入到组中的对象";
            PromptSelectionResult result=ed.GetSelection(opt);
            if (result.Status != PromptStatus.OK) return;
            SelectionSet ss=result.Value;//用户选择的对象
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                //创建一个名为MyGroup的组，并添加用户选择的实体
                db.CreateGroup("MyGroup", ss.GetObjectIds());
                trans.Commit();
            }
        }

        [CommandMethod("AddEntity")]
        public void AddEntity()
        {
            Database db=HostApplicationServices.WorkingDatabase;
            Editor ed=Application.DocumentManager.MdiActiveDocument.Editor;
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                //提示用户选择组对象
                PromptEntityResult entResult=ed.GetEntity("\n请选择组");
                if (entResult.Status != PromptStatus.OK) return;
                //根据选择的对象获得其所在的组
                var groups=entResult.ObjectId.GetGroups();
                if (groups == null) return;
                ObjectId groupId=groups.First();
                //提示用户选择要加入到组中的对象
                PromptSelectionOptions opt=new PromptSelectionOptions();
                opt.MessageForAdding = "\n请选择需要加入到组中的对象";
                PromptSelectionResult result=ed.GetSelection(opt);
                if (result.Status != PromptStatus.OK) return;
                SelectionSet ss=result.Value;
                //添加用户选择的实体到前面选择的组中
                groupId.AppendEntityToGroup(ss.GetObjectIds());
                trans.Commit();
            }
        }

        [CommandMethod("RemoveAllButLines")]
        public void RemoveAllButLines()
        {
            Database db=HostApplicationServices.WorkingDatabase;
            Editor ed=Application.DocumentManager.MdiActiveDocument.Editor;
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                //提示用户选择组
                PromptEntityResult entResult=ed.GetEntity("\n请选择组");
                if (entResult.Status != PromptStatus.OK) return;
                //根据选择的对象获得其所在的组
                var groups=entResult.ObjectId.GetGroups();
                if (groups == null) return;
                ObjectId groupId=groups.First();
                //由于要在组中进行去除对象的操作，因此以写的方式打开找到的组对象
                Group group=(Group)trans.GetObject(groupId, OpenMode.ForWrite);
                //获取组对象中的所有实体的ObjectId并进行循环遍历
                ObjectIdList ids=group.GetAllEntityIds();
                foreach (var id in ids)
                {
                    //打开组中的当前对象
                    DBObject obj=trans.GetObject(id, OpenMode.ForRead);
                    if (obj is Line) continue;//如果对象为直线则不进行处理
                    group.Remove(id);//如果对象不是直线，则在组中移除它
                }
                group.SetColorIndex(1);//设置组中所有实体的颜色为红色
                trans.Commit();
            }
        }
    }
}