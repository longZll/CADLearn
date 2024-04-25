using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;

namespace InitAndOpt
{
    public class OptimizeClass
    {
        [CommandMethod("OptCommand")]
        public void OptCommand()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

            //string fileFirstName = AppDomain.CurrentDomain.BaseDirectory;

            //F:\CADLearn\AUTOCADCode\MyCD\Chap01\outPutPath\Hello.dll
            //Hello.dll程序集的文件路径
            string fileName = "F:\\CADLearn\\AUTOCADcode\\MyCD\\Chap01\\outPutPath\\Hello.dll";    
            
            //。。。。....“” "" ‘’ ''[]  { }+====----()
            try
            {
                ExtensionLoader.Load(fileName); //载入Hello.dll程序集

                ed.WriteMessage(fileName + "路径！");
            }
            catch (System.Exception ex)
            {
                ed.WriteMessage(ex.Message);   //显示异常信息
            }
                
            // 在命令行上显示信息，提示用户Hello.dll程序集已经被载入
            ed.WriteMessage("\n" + fileName + "被载入，请输入Hello进行测试！");
        }
        
        /// <summary>
        /// 改变单个选中对象的颜色
        /// </summary>
        [CommandMethod("ChangeColor")]
        public void ChangeColor()
        {
            //获取当前AutoCAD文档和数据库
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            //获取命令行对象
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

            try
            {
                ObjectId id = ed.GetEntity("\n 请单击选择需要改变颜色的对象").ObjectId;

                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    //锁定文档以确保不被其他操作影响
                    DocumentLock docLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument();

                    Entity ent = tr.GetObject(id, OpenMode.ForWrite) as Entity;
                    ent.ColorIndex = 5;         //5号颜色为蓝色,为测试异常,可以设置对象为不合法的颜色
                    doc.Editor.WriteMessage("已经设置成功!");

                    //提交事务
                    docLock.Dispose();
                    tr.Commit();
                }

            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                switch (ex.ErrorStatus)
                {
                    case ErrorStatus.InvalidIndex:  //输入错误的颜色值
                        ed.WriteMessage("\n 输入的颜色值有误");
                        break;
                    case ErrorStatus.InvalidObjectId:  //未选择对象,无效的对象id
                        ed.WriteMessage("\n 未选择对象,无效的对象id");
                        break;
                    default:
                        ed.WriteMessage(ex.ErrorStatus.ToString());
                        break;
                }

            }

           
        }


        /// <summary>
        /// 改变多个选中对象的颜色
        /// </summary>
        [CommandMethod("ChangeColors")]
        public void ChangeColors()
        {
            //获取当前AutoCAD文档和数据库
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            //获取命令行对象
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

            try
            {
                // 提示用户选择对象
                PromptSelectionResult selectionResult = ed.GetSelection();
                if (selectionResult.Status != PromptStatus.OK)
                {
                    ed.WriteMessage("\n未选择对象。");
                    return;
                }

                //ObjectId id = ed.GetEntity("\n 请单击选择需要改变颜色的对象").ObjectId;

                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    //锁定文档以确保不被其他操作影响
                    DocumentLock docLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument();

                    //Entity ent = tr.GetObject(id, OpenMode.ForWrite) as Entity;
                    //ent.ColorIndex = 5;       //5号颜色为蓝色,为测试异常,可以设置对象为不合法的颜色

                    SelectionSet selectionSet = selectionResult.Value;

                    foreach (SelectedObject selectedObject in selectionSet)
                    {
                        Entity ent = tr.GetObject(selectedObject.ObjectId, OpenMode.ForWrite) as Entity;
                        ent.ColorIndex = 3; //将颜色设置为蓝色（颜色索引为5）
                    }


                    doc.Editor.WriteMessage("已经设置成功!");

                    //提交事务
                    docLock.Dispose();
                    tr.Commit();
                }

            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                switch (ex.ErrorStatus)
                {
                    case ErrorStatus.InvalidIndex:      //输入错误的颜色值
                        ed.WriteMessage("\n 输入的颜色值有误");
                        break;
                    case ErrorStatus.InvalidObjectId:   //未选择对象,无效的对象id
                        ed.WriteMessage("\n 未选择对象,无效的对象id");
                        break;
                    default:
                        ed.WriteMessage(ex.ErrorStatus.ToString());
                        break;
                }

                ed.WriteMessage(ex.Message);

            }

        }

    }
}
