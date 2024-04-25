using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

namespace ClassLibrary1
{
    public class Class1
    {
        [CommandMethod("Hello")]
        public void Hello()
        {
            // 获取当前活动文档的Editor对象，也就是命令行
            Editor ed=Application.DocumentManager.MdiActiveDocument.Editor;
            // 调用Editor对象的WriteMessage函数在命令行上显示文本
            ed.WriteMessage("Hello,欢迎张三进入.NET开发AutoCAD的世界！");
        }
    }

}
