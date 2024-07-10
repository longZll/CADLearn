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
    /// <summary>
    /// 输出块参照测试
    /// </summary>
    public class WBlockTest
    {
        /// <summary>
        /// 用于用户选定的实体以形式 复制到我的文档中的test.dwg中(相当于跨文件复制)
        /// </summary>
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

            //把当前数据库中所选择的实体复制到新建的数据库中，并指定插入点为当前图形数据库的基点
            db = curdb.Wblock(ids, curdb.Ucsorg);


            //保存文件,代码将尝试将文件保存到我的文档目录,文件名为 test.dwg
            try
            {
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "test.dwg");
                db.SaveAs(filePath, DwgVersion.AC1800);
                ed.WriteMessage($"实体已成功复制并保存到 {filePath}。\n");
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                ed.WriteMessage($"保存文件时发生错误: {ex.Message}\n");
            }

            //以2004格式保存数据库为Dwg文件
            //db.SaveAs(@"C:\test.dwg", DwgVersion.AC1800);
        }

        /// <summary>
        /// 从指定文件夹中读取所有DWG文件，并将每个DWG文件中的模型空间实体插入到当前AutoCAD数据库的一个新块表记录中。如果这些DWG文件包含可缩放块，则在插入后设置其可缩放属性,该功能有问题,会报致命错误
        /// </summary>
        [CommandMethod("GetBlocksFromDwgs")]
        public void GetBlocksFromDwgs()
        {
            Database curdb=HostApplicationServices.WorkingDatabase; //获取当前数据库对象
            
            FolderBrowserDialog dlg=new FolderBrowserDialog();      //文件夹浏览对话框
            DialogResult result=dlg.ShowDialog();   //显示对话框
            
            if (result == DialogResult.OK)          //用户选择了“确定”按钮
            {
                string pathName=dlg.SelectedPath;   //用户选择的文件夹的路径
                
                //获取所选文件夹中的后缀名为dwg的所有文件的完整文件路径 数组
                string[] fileNames=Directory.GetFiles(pathName, "*.dwg", SearchOption.AllDirectories);
                
                using (Transaction trans=curdb.TransactionManager.StartTransaction())
                {
                    foreach (string fileName in fileNames) //遍历文件路径数组
                    {
                        //获取去除扩展名后的文件名（不含目录）,从路径名中获取符号名
                        string destName =SymbolUtilityServices.GetSymbolNameFromPathName(fileName, "dwg");
                        
                        //上一句不一定能得到有效的文件名，必须对于无效的名称进行修正
                        destName = SymbolUtilityServices.RepairSymbolName(destName, false);
                        
                        using (Database db=new Database(false, true))
                        {
                            //读入Dwg文件
                            db.ReadDwgFile(fileName, FileShare.Read, true, null);
                            
                            //为了让插入块的函数在多个图形文件打开的情况下起作用，你必须使用下面的函数把Dwg文件关闭
                            db.CloseInput(true);
                            
                            bool isAnno=db.AnnotativeDwg();         //判断文件是否包含可缩放块

                            //bool isAnno = db.AnnotativeDwg;       //判断文件是否包含可缩放块

                            //把源数据库模型空间中的实体插入到当前数据库的一个新的块表记录中
                            ObjectId btrId=curdb.Insert(destName, db, false);
                            
                            if (isAnno)     //如果包含可缩放块，则设置可缩放属性
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
