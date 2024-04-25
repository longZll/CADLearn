using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using DotNetARX;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;
namespace WBlockTest
{
    public class WBlockTest
    {
        [CommandMethod("CopyEntities")]
        public void CopyEntities()
        {
            Editor ed = AcadApp.DocumentManager.MdiActiveDocument.Editor;
            //新建一个数据库对象
            Database db = new Database(false, true);
            //获取当前数据库对象
            Database curdb = HostApplicationServices.WorkingDatabase;
            //下面的操作选择要复制到新建数据库中的实体
            PromptSelectionOptions opts = new PromptSelectionOptions();
            opts.MessageForAdding = "请输入复制到新文件的实体";
            SelectionSet ss = ed.GetSelection(opts).Value;
            //获取所选实体的ObjectId集合
            ObjectIdCollection ids = new ObjectIdCollection(ss.GetObjectIds());
            //把当前数据库中所选择的实体复制到新建的数据库中，并指定插入点为当前数据库的基点
            db = curdb.Wblock(ids, curdb.Ucsorg);
            //以2004格式保存数据库为Dwg文件
            db.SaveAs(@"C:\test.dwg", DwgVersion.AC1800);
        }

        [CommandMethod("GetBlocksFromDwgs")]
        public void GetBlocksFromDwgs()
        {
            Database curdb=HostApplicationServices.WorkingDatabase;//获取当前数据库对象
            FolderBrowserDialog dlg=new FolderBrowserDialog();//文件夹浏览对话框
            DialogResult result=dlg.ShowDialog();//显示对话框
            if (result == DialogResult.OK)//用户选择了“确定”按钮
            {
                string pathName=dlg.SelectedPath;//用户选择的文件夹
                //获取所选文件夹中的所有文件
                string[] fileNames=Directory.GetFiles(pathName, "*.dwg", SearchOption.AllDirectories);
                using (Transaction trans=curdb.TransactionManager.StartTransaction())
                {
                    foreach (string fileName in fileNames)//遍历文件夹中的文件
                    {
                        //获取去除扩展名后的文件名（不含目录）
                        string destName=SymbolUtilityServices.GetSymbolNameFromPathName(fileName, "dwg");
                        //上一句不一定能得到有效的文件名，必须对于无效的名称进行修正
                        destName = SymbolUtilityServices.RepairSymbolName(destName, false);
                        using (Database db=new Database(false, true))
                        {
                            //读入Dwg文件
                            db.ReadDwgFile(fileName, FileShare.Read, true, null);
                            //为了让插入块的函数在多个图形文件打开的情况下起作用，你必须使用下面的函数把Dwg文件关闭
                            db.CloseInput(true);
                            bool isAnno=db.AnnotativeDwg();//判断文件是否包含可缩放块
                            //把源数据库模型空间中的实体插入到当前数据库的一个新的块表记录中
                            ObjectId btrId=curdb.Insert(destName, db, false);
                            if (isAnno)//如果包含可缩放块，则设置可缩放属性
                            {
                                BlockTableRecord btr=(BlockTableRecord)trans.GetObject(btrId, OpenMode.ForWrite);
                                btr.Annotative = AnnotativeStates.True;
                            }
                        }
                    }
                    trans.Commit();
                }
            }
        }
    }
}
