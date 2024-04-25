using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using DotNetARX;

//using DotNetARXMine.Tools;


namespace Lines
{
    public class Lines
    {
        [CommandMethod("FirstLine")]
        public static void FirstLine()
        {
            //1.获取当前活动图形数据库
            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            //获取命令行对象
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            //Database db= HostApplicationServices.WorkingDatabase;

            //2.在内存中创建实体类的对象
            Point3d startPoint = new Point3d(0, 0, 0);            //直线起点
            Point3d endPoint = new Point3d(1000, 1000, 0);        //直线终点
            Line line = new Line(startPoint, endPoint);           //新建一个直线对象   

            //定义一个指向当前数据库的事务处理，以添加直线
            using (OpenCloseTransaction trans = new OpenCloseTransaction())
            {
                try
                {
                    //3.打开图形数据库的块表
                    BlockTable bt = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;           //以读方式打开块表.

                    //4.打开存储实体的块表记录
                    BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);   //以写方式打开模型空间块表记录.

                    //锁定文档以确保不被其他操作影响
                    DocumentLock docLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument();


                    //5.将图形对象的信息添加到块表记录中.
                    btr.AppendEntity(line);

                    trans.AddNewlyCreatedDBObject(line, true); //把对象添加到事务处理中.

                    docLock.Dispose();
                    trans.Commit();     //提交事务处理
                }
                catch (System.Exception ex)
                {

                    trans.Abort();      //如有异常放弃事务处理

                    // 处理异常
                    Application.ShowAlertDialog("发生错误：" + ex.Message);

                }

            }


        }

        [CommandMethod("SecondLine")]
        public static void SecondLine()
        {
            Database db = HostApplicationServices.WorkingDatabase;

            Point3d startPoint = new Point3d(0, 3000, 0);
            double angle = GeTools.DegreeToRadian(90);
            Line line = new Line(startPoint, startPoint.PolarPoint(angle, 100));

            db.AddToModelSpace(line);
        }


        [CommandMethod("ThridLine")]
        public static void ThridLine()
        {
            //1.获取当前活动图形数据库
            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            //获取命令行对象
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            //Database db= HostApplicationServices.WorkingDatabase;

            //2.在内存中创建实体类的对象
            Point3d startPoint = new Point3d(1000, 0, 0);         //直线起点
            Point3d endPoint = new Point3d(1000, 1000, 0);        //直线终点
            Line line = new Line(startPoint, endPoint);           //新建一个直线对象   

            //定义一个指向当前数据库的事务处理，以添加直线
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                //3.打开图形数据库的块表
                BlockTable bt = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;           //以读方式打开块表.

                //4.打开存储实体的块表记录
                BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);   //以写方式打开模型空间块表记录.

                //锁定文档以确保不被其他操作影响
                DocumentLock docLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument();

                try
                {
                    //5.将图形对象的信息添加到块表记录中.
                    btr.AppendEntity(line);

                    trans.AddNewlyCreatedDBObject(line, true); //把对象添加到事务处理中.

                    docLock.Dispose();
                    trans.Commit(); //提交事务处理
                }
                catch (System.Exception ex)
                {
                    trans.Abort();  //如有异常放弃事务处理,终止事务

                    // 处理异常
                    Application.ShowAlertDialog("发生错误：" + ex.Message);
                }

            }

        }



        [CommandMethod("FourthLine")]
        public static void FourthLine()
        {
            //1.获取当前活动图形数据库
            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            //获取命令行对象
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            //Database db= HostApplicationServices.WorkingDatabase;

            //2.在内存中创建实体类的对象
            Point3d startPoint = new Point3d(0, 500, 0);            //直线起点
            Point3d endPoint = new Point3d(1000, 3000, 0);        //直线终点
            Line line = new Line(startPoint, endPoint);           //新建一个直线对象   

            //静态函数可以直接通过类的名字来调用
            //Tools.AddToModelSpace(db, line);

            db.AddToModelSpace(line);

        }

        [CommandMethod("FifthLine")]
        public static void FifthLine()
        {
            Database db = HostApplicationServices.WorkingDatabase;

            Point3d startPoint = new Point3d(1500, 3000, 0);
            Point3d endPoint = new Point3d(5000, 3000, 0);

            Line line = new Line(startPoint, endPoint);

            //using ()
            //{
            //   // db.DotNetARXMine.Tools.AddToModelSpace(line);

            //    db.AddToModelSpace(line);
            //}

            db.AddToModelSpace(line);

        }

    }
}
