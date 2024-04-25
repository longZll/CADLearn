using System;
using System.Windows.Forms;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using DotNetARX;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;
namespace AppAndDocEvent
{
    public class AppAndDocEvent : IExtensionApplication
    {
        ObjectId lineLayerId;       // 直线层Id
        ObjectId circleLayerId;     // 圆层Id
        ObjectId arcLayerId;        // 弧线层Id
        ObjectId polylineLayerId;   // 多段线层Id

        public void Initialize()
        {
            Document doc = AcadApp.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            DocumentCollection docs = AcadApp.DocumentManager;

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                lineLayerId = db.AddLayer("直线");        //添加'直线'图层
                circleLayerId = db.AddLayer("圆");        //添加'圆'图层
                arcLayerId = db.AddLayer("圆弧");         //添加'圆弧'图层
                polylineLayerId = db.AddLayer("多段线");  //添加'多段线'图层
                trans.Commit();
            }

            docs.DocumentLockModeChanged += new DocumentLockModeChangedEventHandler(docs_DocumentLockModeChanged);

            //阻止文档的关闭
            doc.BeginDocumentClose += delegate (object sender, DocumentBeginCloseEventArgs e)
            {
                //提示用户是否真的需要关闭文档
                DialogResult result = MessageBox.Show("文档将被关闭\n是否继续？", "关闭文档", MessageBoxButtons.YesNo);
                if (result == DialogResult.No)//如果不选择关闭文档，则放弃文档关闭事件
                { e.Veto(); }

            };


            //AutoCAD系统变量变化时通知用户
            AcadApp.SystemVariableChanged += (sender, e) =>
            {
                if (e.Name == "ORTHOMODE")  //如果ORTHOMODE系统变量发生变化
                {
                    //获取ORTHOMODE系统变量的值
                    bool isOrtho = Convert.ToBoolean(AcadApp.GetSystemVariable(e.Name));
                    //根据ORTHOMODE系统变量的值，来判断是否启用正交模式
                    if (isOrtho) AcadApp.ShowAlertDialog("启用正交模式！");
                    else AcadApp.ShowAlertDialog("关闭正交模式！");
                }
            };
        }

        void docs_DocumentLockModeChanged(object sender, DocumentLockModeChangedEventArgs e)
        {
            Database db = HostApplicationServices.WorkingDatabase;
            switch (e.GlobalCommandName)  //根据命令名执行不同的动作
            {
                case "ERASE"://如果是删除对象
                    e.Veto();//撤销删除动作,在AutoCAD中禁用了"删除"命令
                    Editor ed = AcadApp.DocumentManager.MdiActiveDocument.Editor;
                    ed.WriteMessage("\nERASE命令已经被禁止，无法删除对象！");
                    break;
                case "LINE":    //如果是绘制直线，则当前图层设为直线层. 实现按照实体类型的分层，也就是在创建实体时将不同类型的实体放置在不同的图层中
                    db.Clayer = lineLayerId;
                    break;
                case "CIRCLE":  //如果是绘制圆，则当前图层设为圆层
                    db.Clayer = circleLayerId;
                    break;
                case "ARC":     //如果是绘制圆弧，则当前图层设为圆弧层
                    db.Clayer = arcLayerId;
                    break;
                case "PLINE":   //如果是绘制多段线，则当前图层设为多段线层
                    db.Clayer = polylineLayerId;
                    break;
            }
        }

        public void Terminate()
        {
            DocumentCollection docs = AcadApp.DocumentManager;
            //断开事件处理函数
            docs.DocumentLockModeChanged -= docs_DocumentLockModeChanged;
        }
    }
}
