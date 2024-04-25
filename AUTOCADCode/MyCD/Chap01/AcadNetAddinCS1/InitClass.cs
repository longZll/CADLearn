#region Namespaces
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
#endregion

[assembly: ExtensionApplication(typeof(AcadNetAddinCS1.InitClass))]
[assembly: CommandClass(typeof(AcadNetAddinCS1.OptimizeClass))]
namespace AcadNetAddinCS1
{
    public class InitClass : IExtensionApplication
    {
        #region IExtensionApplication Members
        public void Initialize()
        {
            Editor ed= Application.DocumentManager.MdiActiveDocument.Editor;
            // 在AutoCAD命令行上显示一些信息，它们会在程序载入时被显示
            ed.WriteMessage("程序开始初始化。");
        }

        public void Terminate()
        {
            // 在Visual Studio 2010的输出窗口上显示程序结束的信息
            System.Diagnostics.Debug.WriteLine(
        "程序结束，你可以在里做一些程序的清理工作，如关闭AutoCAD文档");
        }
        #endregion

        [CommandMethod("InitCommand")]
        public void InitCommand()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            ed.WriteMessage("Test");

            ed.WriteMessage("请输入InitCommand启动命令");
        }
    }
}
