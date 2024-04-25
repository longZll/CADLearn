using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNetARX;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using System.Reflection;

[assembly: ExtensionApplication(typeof(CustomFilter.Commands))]
namespace CustomFilter
{
    public class Commands:IExtensionApplication
    {
        private static DrawRule theOverrule;
        [CommandMethod("SHOWBLOCKS")]
        public static void ShowBlocks()
        {
            Document doc=Application.DocumentManager.MdiActiveDocument;
            Database db=doc.Database;
            Editor ed=doc.Editor;
            //提示用户选择需要亮显的块
            PromptEntityOptions opt=new PromptEntityOptions("\n请输入需要亮显的块");
            opt.SetRejectMessage("选择的不是块");
            opt.AddAllowedClass(typeof(BlockReference),false);
            PromptEntityResult result=ed.GetEntity(opt);
            if (result.Status != PromptStatus.OK) return;
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                ObjectId id=result.ObjectId;
                //设置需要重定义的块名，此语句会调用DrawRule重定义以亮显块
                DrawRule.BlockName = id.GetBlockName();
                trans.Commit();
            }
            //刷新屏幕，亮显指定名称的块
            ed.Regen();
        }
        public void Initialize()
        {
            if (theOverrule == null)
            {
                theOverrule = new DrawRule();
                //为块参照添加重定义
                Overrule.AddOverrule(RXObject.GetClass(typeof(BlockReference)), theOverrule, false);
                //设置自定义过滤
                theOverrule.SetCustomFilter();
            }
            Overrule.Overruling = true;//开启规则重定义            
        }
        public void Terminate()
        {
            
        }
    }
}
