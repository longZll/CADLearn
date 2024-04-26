using System.Linq;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using DotNetARX;
namespace MyXData
{
    public class MyXData
    {
        /// <summary>
        /// 增加扩展数据
        /// </summary>
        [CommandMethod("AddX")]
        public void AddX()
        {
            Document doc=Application.DocumentManager.MdiActiveDocument;
            Database db=doc.Database;
            Editor ed=doc.Editor;
            //提示用户选择一个表示董事长的多行文本
            PromptEntityOptions opt=new PromptEntityOptions("\n请选择表示董事长的多行文本");
            opt.SetRejectMessage("\n您选择的不是多行文本，请重新选择");
            opt.AddAllowedClass(typeof(MText), true);

            PromptEntityResult entResult=ed.GetEntity(opt);

            if (entResult.Status != PromptStatus.OK) return;

            ObjectId id=entResult.ObjectId; //用户选择的多行文本的ObjectId

            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                TypedValueList values=new TypedValueList(); //定义一个TypedValue列表
                //添加整型（表示员工编号）和字符串（表示职位）扩展数据项
                values.Add(DxfCode.ExtendedDataInteger32, 1002);
                values.Add(DxfCode.ExtendedDataAsciiString, "董事长");
                
                //为实体添加应用程序名为"EMPLOYEE"的扩展数据
                id.AddXData("EMPLOYEE", values);
                trans.Commit();
            }
        }

        /// <summary>
        /// 修改多行文本的扩展数据
        /// </summary>
        [CommandMethod("ModX")]
        public void ModX()
        {
            Document doc=Application.DocumentManager.MdiActiveDocument;
            Database db=doc.Database;
            Editor ed=doc.Editor;
            //提示用户选择一个多行文本
            PromptEntityOptions opt=new PromptEntityOptions("\n请选择多行文本");
            opt.SetRejectMessage("\n您选择的不是多行文本，请重新选择");
            opt.AddAllowedClass(typeof(MText), true);
            PromptEntityResult entResult=ed.GetEntity(opt);
            if (entResult.Status != PromptStatus.OK) return;

            ObjectId id=entResult.ObjectId;//用户选择的多行文本的ObjectId
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                //如果扩展数据项（员工编号）为1002，则将其修改为1001
                id.ModXData("EMPLOYEE", DxfCode.ExtendedDataInteger32, 1002, 1001);
                trans.Commit();
            }
        }


        /// <summary>
        /// 删除多行文本的扩展数据
        /// </summary>
        [CommandMethod("DelX")]
        public void DelX()
        {
            Document doc=Application.DocumentManager.MdiActiveDocument;
            Database db=doc.Database;
            Editor ed=doc.Editor;
            //提示用户选择一个多行文本
            PromptEntityOptions opt=new PromptEntityOptions("\n请选择多行文本");
            opt.SetRejectMessage("\n您选择的不是多行文本，请重新选择");
            opt.AddAllowedClass(typeof(MText), true);
            PromptEntityResult entResult=ed.GetEntity(opt);
            if (entResult.Status != PromptStatus.OK) return;

            ObjectId id=entResult.ObjectId;//用户选择的多行文本的ObjectId
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                id.RemoveXData("EMPLOYEE"); //删除EMPLOYEE扩展数据
                trans.Commit();
            }
        }

        [CommandMethod("StartMonitor")]
        public void StartMonitor()
        {
            Editor ed=Application.DocumentManager.MdiActiveDocument.Editor;
            //开始鼠标监控事件
            ed.PointMonitor += new PointMonitorEventHandler(ed_PointMonitor);
        }

        void ed_PointMonitor(object sender, PointMonitorEventArgs e)
        {
            string employeeInfo=""; //用于存储员工的信息：编号和职位

            //获取命令行对象（鼠标监视事件的发起者），用于获取文档对象
            Editor ed=(Editor)sender;
            Document doc=ed.Document;
            
            //获取鼠标停留处的实体
            FullSubentityPath[] paths=e.Context.GetPickedEntities();
            using (Transaction trans=doc.TransactionManager.StartTransaction())
            {
                //如果鼠标停留处有实体
                if (paths.Length > 0)
                {
                    //获取鼠标停留处的实体
                    FullSubentityPath path=paths[0];
                    MText mtext=trans.GetObject(path.GetObjectIds()[0], OpenMode.ForRead) as MText;
                    if (mtext != null)//如果鼠标停留处为多行文本
                    {
                        //获取多行文本中应用程序名为“EMPLOYEE”的扩展数据
                        TypedValueList xdata=mtext.ObjectId.GetXData("EMPLOYEE");
                        if (xdata != null)
                        {
                            employeeInfo += "员工编号：" + xdata[1].Value.ToString() + "\n职位：" + xdata[2].Value.ToString();
                        }
                    }
                }
                trans.Commit();
            }

            if (employeeInfo != "")
            {
                e.AppendToolTipText(employeeInfo);        //在鼠标停留处显示提示信息
            }
        }

        [CommandMethod("StopMonitor")]
        public void StopMonitor()
        {
            Editor ed=Application.DocumentManager.MdiActiveDocument.Editor;
            //停止鼠标监控事件
            ed.PointMonitor -= new PointMonitorEventHandler(ed_PointMonitor);
        }
    }
}
