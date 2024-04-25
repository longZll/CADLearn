using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;

using System;

namespace load
{
    public class Class1
    {
        private bool ev = false;
        /// <summary>
        /// 以下代码是一个AutoCAD的命令程序，主要功能是加载指定路径下的一个或多个DLL文件
        /// </summary>
        [CommandMethod("ww")]
        public void ww()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;

            // 创建一个AssemblyDependent实例，并传入dll文件的路径
            //var ad = new AssemblyDependent("E:\\cad\\bin\\Debug\\demo.dll");  //写上你dll的路径
            //var ad = new AssemblyDependent(@"F:\cadDevelopment\Install\bin\CADDevelopment.dll");  //写上你dll的路径

            //F:\CADLearn\AUTOCADCode\MyCD\Chap06\outPutPath\Xreocrd.dll

            var ad = new AssemblyDependent(@"F:\CADLearn\AUTOCADCode\MyCD\Chap06\outPutPath\Xreocrd.dll");  //写上你dll的路径

            // 调用Load方法加载dll，并返回加载信息
            var msg = ad.Load();

            // 初始化一个布尔变量allyes，表示所有dll是否都加载成功
            bool allyes = true;

            foreach (var item in msg)
            {
                if (!item.LoadYes)
                {
                    //如果某个dll未加载成功，则在编辑器中输出加载失败的信息，并将allyes标记为false
                    ed.WriteMessage(Environment.NewLine + "**" + item.Path +
                                    Environment.NewLine + "**此文件已加载过,重复名称,重复版本号,本次不加载!" +
                                    Environment.NewLine);
                    allyes = false;
                }
            }

            //如果所有dll都加载成功，则在编辑器中输出加载成功的信息
            if (allyes)
            {
                ed.WriteMessage(Environment.NewLine + "dll文件链式加载成功! 请输入命令启动插件" + Environment.NewLine);
            }
            //如果ev为false，则注册AssemblyResolve事件处理程序
            if (!ev) 
            { 
                AppDomain.CurrentDomain.AssemblyResolve += RunTimeCurrentDomain.DefaultAssemblyResolve; 
                ev = true; 
            }

        }

    }
}
