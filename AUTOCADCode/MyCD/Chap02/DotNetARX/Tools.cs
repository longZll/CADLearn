using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;


namespace DotNetARXMIne
{
    public static class Tools
    {/// <summary>
     /// 将实体添加到模型空间
     /// </summary>
     /// <param name="db">数据库对象</param>
     /// <param name="ent">要添加的实体</param>
     /// <returns></returns>
        public static ObjectId AddToModelSpace(this Database db, Entity ent)
        {

            ObjectId entId;

            //定义一个指向当前数据库的事务处理，以添加直线
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                //3.打开图形数据库的块表
                BlockTable bt = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;           //以读方式打开块表.

                //4.打开存储实体的块表记录
                BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);   //以写方式打开模型空间块表记录.

                //锁定文档以确保不被其他操作影响
                DocumentLock docLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument();

                //5.将图形对象的信息添加到块表记录中.
                entId = btr.AppendEntity(ent);

                trans.AddNewlyCreatedDBObject(ent, true); //把对象添加到事务处理中.

                docLock.Dispose();
                trans.Commit(); //提交事务处理

            }

            return entId;

        }


    }
}
