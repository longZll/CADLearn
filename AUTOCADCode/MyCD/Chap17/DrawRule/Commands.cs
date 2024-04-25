using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;

namespace DrawRule
{
    public class Commands
    {
        //规则重定义对象
        private static DrawRule theOverrule;
        [CommandMethod("Door")]
        public static void DrawDoor()
        {
            //初始化重定义对象
            if (theOverrule == null)
            {
                theOverrule = new DrawRule();
                //为直线类添加重定义
                Overrule.AddOverrule(RXObject.GetClass(typeof(Line)), theOverrule, false);
            }
            Overrule.Overruling = true;//开启规则重定义
            //刷新屏幕，直线被更新为门
            Application.DocumentManager.MdiActiveDocument.Editor.Regen();
        }
    }
}
