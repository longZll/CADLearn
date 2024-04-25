using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace AttachingExternalReference
{
    public class AttachDwg
    {
        [CommandMethod("AttachingExternalReference")]
        public static void AttachingExternalReferenceDwg()
        {
            try
            {
                //获取当前文档和数据库
                Document acDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
                Database acCurDb = acDoc.Database;
                using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
                {
                    //创建对DWG文件的引用
                    //C: \Users\user\Desktop\dwgTest\Draw2.dwg  修改文件路径为双斜杠
                    //string PathName = "C:\\AutoCAD\\Sample\\Sheet Sets\\Architectural\\Res\\Exterior Elevations.dwg";

                    string PathName = "C: \\Users\\user\\Desktop\\dwgTest\\Draw2.dwg";

                    ObjectId acXrefId = acCurDb.AttachXref(PathName, "Draw2");

                    // If a valid reference is created then continue 如果创建了有效的引用，则继续
                    if (!acXrefId.IsNull)
                    {
                        // Attach the DWG reference to the current space
                        // 将DWG参照附加到当前空间
                        Point3d insPt = new Point3d(1, 1, 0);
                        using (BlockReference acBlkRef = new BlockReference(insPt, acXrefId))
                        {
                            BlockTableRecord acBlkTblRec;
                            acBlkTblRec = acTrans.GetObject(acCurDb.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;

                            acBlkTblRec.AppendEntity(acBlkRef);
                            acTrans.AddNewlyCreatedDBObject(acBlkRef, true);
                        }
                    }

                    // Save the new objects to the database 保存新对象到数据库
                    acTrans.Commit();

                    // 执行缩放操作
                    acDoc.SendStringToExecute("._zoom _e ", true, false, true);
                }
            }
            catch (Autodesk.AutoCAD.Runtime.Exception e)
            {
                // 捕获 eFileAccessErr 异常
                if (e.ErrorStatus == ErrorStatus.FileAccessErr)
                {
                    // 处理文件访问错误
                    // 在这里添加您的处理代码
                    Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("文件访问错误,请将文件路径加入CAD信任路径: " + e.Message);
                }
                else
                {
                    // 如果不是eFileAccessErr，重新抛出异常
                    throw;
                }
            }

        }

    }
}
