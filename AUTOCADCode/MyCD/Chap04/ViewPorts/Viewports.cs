using System;
using System.Linq;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using DotNetARX;
namespace Viewports
{
    public class Viewports
    {
        /// <summary>
        /// 在图纸空间创建一个矩形浮动视口并设置为当前视口
        /// </summary>
        [CommandMethod("CreateVP")]
        public void CreateVP()
        {
            Document doc=Application.DocumentManager.MdiActiveDocument;
            Database db=doc.Database;
            Editor ed=doc.Editor;
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                //将活动空间切换到图纸空间
                db.TileMode = false;
                ed.SwitchToPaperSpace();
                //新建一个常规矩形视口
                Viewport vport=new Viewport();
                //设置视口的中点
                vport.CenterPoint = new Point3d(100, 50, 0);
                vport.Width = 120;//视口的宽度
                vport.Height = 80;//视口的高度                
                db.AddToPaperSpace(vport);//将视口添加到图纸空间 
                //修改视口的观察方向
                vport.ViewDirection = new Vector3d(1, 1, 1);
                vport.On = true;//启用新建的视口  
                ed.SwitchToModelSpace();//切换到模型空间              
                ed.SetCurrentVPort(vport);//将新视口置为当前
                trans.Commit();
            }
        }

        /// <summary>
        /// 用于在图纸空间创建一个圆形的浮动视口并设置其为当前视口
        /// </summary>
        [CommandMethod("CreateCirVP")]
        public void CreateCirVP()
        {
            Document doc=Application.DocumentManager.MdiActiveDocument;
            Database db=doc.Database;
            Editor ed=doc.Editor;
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                db.TileMode = false;
                ed.SwitchToPaperSpace();
                //在图纸空间中创建一个圆用来表示视口的范围
                Circle cir=new Circle(new Point3d(5, 2, 0), Vector3d.ZAxis, 10);
                db.AddToPaperSpace(cir);
                //圆形视口
                Viewport vportCir=new Viewport();
                db.AddToPaperSpace(vportCir);//将视口添加到图纸空间
                //设置非矩形视口的边界对象
                vportCir.NonRectClipEntityId = cir.ObjectId;
                vportCir.NonRectClipOn = true;  //启用非矩形视口
                vportCir.On = true;             //启用新建的圆形视口
                ed.SetCurrentVPort(vportCir);   //将新视口置为当前
                trans.Commit();
            }
        }

        /// <summary>
        /// 在模型空间创建四个大小相同的视口
        /// </summary>
        [CommandMethod("Create4VPorts")]
        public void Create4VPorts()
        {
            Document doc=Application.DocumentManager.MdiActiveDocument;
            Database db=doc.Database;
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                //打开视口表
                ViewportTable vt=(ViewportTable)trans.GetObject(db.ViewportTableId, OpenMode.ForRead);
                string vname="4VP";//视口名称
                if (vt.Has(vname)) return; //如果已经存在名为4VP的视口，则返回
                vt.UpgradeOpen(); //切换视口表为写的状态
                
                //新建视口表记录数组
                ViewportTableRecord[] vtrs=new ViewportTableRecord[4];
                for (int i = 0; i < vtrs.Count(); i++)
                {
                    vtrs[i] = new ViewportTableRecord();//新建视口表记录
                    vtrs[i].Name = vname;//设置视口表记录的名称
                    vt.Add(vtrs[i]);//将视口表记录添加到视口表
                    //把视口表记录添加到事务处理中
                    trans.AddNewlyCreatedDBObject(vtrs[i], true);
                }

                //设置视口表记录的角点
                vtrs[0].LowerLeftCorner = new Point2d(0, 0);
                vtrs[0].UpperRightCorner = new Point2d(0.5, 0.5);
                vtrs[1].LowerLeftCorner = new Point2d(0.5, 0);
                vtrs[1].UpperRightCorner = new Point2d(1, 0.5);
                vtrs[2].LowerLeftCorner = new Point2d(0, 0.5);
                vtrs[2].UpperRightCorner = new Point2d(0.5, 1);
                vtrs[3].LowerLeftCorner = new Point2d(0.5, 0.5);
                vtrs[3].UpperRightCorner = new Point2d(1, 1);
                
                //分析当前空间是模型空间还是图纸空间
                if (db.TileMode == true) //模型空间
                    doc.SendStringToExecute("-VPORTS 4 ", false, false, false);
                else //图纸空间
                    doc.SendStringToExecute("-VPORTS 4  ", false, false, false);
                trans.Commit();
            }
        }

        /// <summary>
        /// 模型空间与图纸空间切换
        /// </summary>
        [CommandMethod("ToggleSpace")]
        public void ToggleSpace()
        {
            Document doc=Application.DocumentManager.MdiActiveDocument;
            Database db=doc.Database;
            Editor ed=doc.Editor;
            //获取CVPORT系统变量的值
            int cvport=Convert.ToInt32(Application.GetSystemVariable("CVPORT"));
            if (db.TileMode == false)//图纸空间
            {
                //CVPORT=2，表示模型状态被激活
                if (cvport == 2)
                    ed.SwitchToPaperSpace();//切换到图纸状态
                else//CVPORT不为2，表示模型状态未被激活
                    ed.SwitchToModelSpace();//切换到模型状态
            }
            else//模型空间
                db.TileMode = false;
        }
    }
}
