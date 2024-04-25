#region Namespaces
using System;
using System.Diagnostics;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using MgdAcApplication = Autodesk.AutoCAD.ApplicationServices.Application;
#endregion

namespace AcadNetAddinCS1
{
    public class OptimizeClass
    {
        [CommandMethod("CmdGroup1", "OptCommand", null, CommandFlags.Modal, null, "AcadNetAddinCS1", "OptCommand")]
        public void OptCommand_Method()
        {
            //Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = MgdAcApplication.DocumentManager.MdiActiveDocument.Editor;            
            try
            {
                //string fileFirstName = AppDomain.CurrentDomain.BaseDirectory;

                string fileName ="F:\\CADLearn\\AUTOCAD VBA&VB.NET开发基础与实例教程第2版(C#版) 配套源码\\我的光盘\\Chap01\\outPutPath\\Hello.dll";// 指定外部程序集Hello.dll的文件路径

                ExtensionLoader.Load(fileName); // 载入Hello.dll程序集

                // 在命令行上显示信息，提示用户Hello.dll程序集已经被载入
                ed.WriteMessage("\n" + fileName + "被载入，请输入Hello进行测试！");
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                ed.WriteMessage(ex.ToString());
            }
        }
    }
}
