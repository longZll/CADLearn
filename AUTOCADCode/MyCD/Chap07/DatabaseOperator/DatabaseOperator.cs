using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.DatabaseServices.Filters;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using DotNetARX;
namespace DatabaseOperator
{
    public class DatabaseOperator
    {
        static string filename;//文件名
        [CommandMethod("CreateAndSaveDwg")]
        public void CreateAndSaveDwg()
        {
            //新建一个数据库对象以创建新的Dwg文件
            using (Database db=new Database())
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                //新建两个圆
                Circle cir1=new Circle(new Point3d(1, 1, 0), Vector3d.ZAxis, 1.0);
                Circle cir2=new Circle(new Point3d(5, 5, 0), Vector3d.ZAxis, 2.0);
                db.AddToModelSpace(cir1, cir2);//将新建的圆加入模型空间
                //定义保存文件对话框
                PromptSaveFileOptions opt=new PromptSaveFileOptions("请输入文件名：");
                //保存文件对话框的文件扩展名列表
                opt.Filter = "图形(*.dwg)|*.dwg|图形(*.dxf)|*.dxf";
                opt.FilterIndex = 0;//文件过滤器列表中缺省显示的文件扩展名
                opt.DialogCaption = "图形另存为";//保存文件对话框的标题
                opt.InitialDirectory = @"C:\";//缺省保存目录
                opt.InitialFileName = "MyDwg";//缺省保存文件名
                Editor ed=Application.DocumentManager.MdiActiveDocument.Editor;
                //根据保存对话框中用户的选择，获取保存文件名
                PromptFileNameResult result=ed.GetFileNameForSave(opt);
                if (result.Status != PromptStatus.OK) return;
                filename = result.StringResult;
                db.SaveAs(filename, DwgVersion.Current);//保存文件为当前AutoCAD版本
                trans.Commit();//提交事务处理
            }
        }

        [CommandMethod("ReadDwg")]
        public void ReadDwg()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            //新建一个数据库对象以读取Dwg文件
            using (Database db=new Database(false, true))
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                //如果文件存在
                if (System.IO.File.Exists(filename))
                {
                    //把文件读入到数据库中
                    db.ReadDwgFile(filename, System.IO.FileShare.Read, true, null);
                    PartialOpenDatabase(db);//执行局部加载
                    //获取数据库的块表对象
                    BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
                    //打开数据库的模型空间块表记录对象
                    BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForRead);
                    //循环遍历模型空间中的实体
                    foreach (ObjectId id in btr)
                    {
                        Entity ent = trans.GetObject(id, OpenMode.ForRead) as Entity;
                        //显示实体的类名
                        if (ent != null) ed.WriteMessage("\n" + ent.GetType().ToString());
                    }
                }
                else ed.WriteMessage("文件不存在！");//文件不存在
                trans.Commit();
            }
        }

        public bool PartialOpenDatabase(Database db)
        {
            if (db == null) return false;//数据库未赋值，返回
            //指定局部加载的范围
            Point2d pt1=Point2d.Origin;
            Point2d pt2=new Point2d(100, 100);
            Point2dCollection pts=new Point2dCollection(2) { pt1, pt2 };
            //创建局部加载范围过滤器
            SpatialFilterDefinition filterDef=new SpatialFilterDefinition(
                pts, Vector3d.ZAxis, 0.0, 0.0, 0.0, true);
            SpatialFilter sFilter=new SpatialFilter();
            sFilter.Definition = filterDef;
            //创建图层过滤器，只加载Circle和Line层
            LayerFilter layerFilter=new LayerFilter();
            layerFilter.Add("Circle");
            layerFilter.Add("Line");
            //对图形数据库应用局部加载
            db.ApplyPartialOpenFilters(sFilter, layerFilter);
            if (db.IsPartiallyOpened)//判断图形数据库是否已局部加载
            {
                db.CloseInput(true);//关闭文件输入
                return true;//局部加载成功
            }
            else return false;//局部加载失败
        }
    }
}
