using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;



namespace MoveTexts
{
    /// <summary>
    /// 拉移随心,同时移动文字类
    /// </summary>
    public class MoveTexts
    {
        Database db = HostApplicationServices.WorkingDatabase;
        Document doc = Application.DocumentManager.MdiActiveDocument;
        Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

        bool bMove;
        Point3d startPoint;     //用来存储选中文字的起始位置
        Point3d endPoint;       //用来存储选中文字的最终位置
        Double YSpacingCoefficient = 1.6;       //修正之后的Y方向文本间距系数
        Double TextHigh = 250;
        Double TextSpacingCoefficient = 3.0;    //文本搜索间距系数

        void commandWillStart(object sender, CommandEventArgs e)
        {
            //如果AutoCAD命令为MOVE
            if (e.GlobalCommandName == "MOVE")
            {
                //设置全局变量bMove为True，表示移动命令开始
                bMove = true;
            }
        }

        //打开对象进行修改 事件
        void objectOpenedForModify(object sender, ObjectEventArgs e)
        {
            //判断AutoCAD命令是否为移动
            if (bMove == false)
                //如果AutoCAD命令为非移动，则返回
                return;
            DBText dbtext = e.DBObject as DBText;
            //判断将要移动的对象是否为圆
            if (dbtext != null)
            {
                //存储起始位置
                startPoint = dbtext.Position;
            }
        }

        void objectModified(object sender, ObjectEventArgs e)
        {
            //判断AutoCAD命令是否为移动
            if (bMove == false)
                //如果AutoCAD命令为非移动，则返回
                return;

            //断开所有的事件处理函数
            //removeEvents();

            //判断移动过的对象是否为圆
            DBText startText = e.DBObject as DBText;
            if (startText == null) {

                ed.WriteMessage("没有选中单行文字!");
                return;
            }

            if (startText != null)
            {
                //存储最终位置
                endPoint = startText.Position;
            }


            ed.WriteMessage("选中了单行文字!\n");

            try
            {
                //设置选择集过滤器，只选择dwg图形数据库中所有的单行文字
                ed.WriteMessage("请选择单行文字对象!\n");

                TypedValue[] values = { new TypedValue((int)DxfCode.Start, "Text") };
                SelectionFilter filter = new SelectionFilter(values);
                PromptSelectionResult resSel = ed.SelectAll(filter);

                if (resSel.Status == PromptStatus.OK)
                {
                    //获取选择集中的圆对象
                    SelectionSet sSet = resSel.Value;
                    ObjectId[] ids = sSet.GetObjectIds();

                    ed.WriteMessage("这张图纸中单行文字的个数为:"+ ids.Length.ToString()+ "\n");

                    //开始事务处理
                    using (Transaction trans = db.TransactionManager.StartTransaction())
                    {
                        //循环遍历 选择集中的所有的单行文字
                        foreach (ObjectId id in ids)
                        {
                            //以读的方式打开对象
                            DBText followedText = (DBText)trans.GetObject(id, OpenMode.ForRead);

                            Point3d followedText_startPoint = followedText.Position;
                            Point3d followedText_endPoint = followedText_startPoint;

                            //通过判断选中的文字跟图纸中所有的文字是否满足对应的位置关系，如果满足在附近一定范围内
                            bool isTextsdisAdjoin = isTextsdistanceAdjoin(startPoint, followedText_startPoint, 1.0e-2, TextHigh * TextSpacingCoefficient);

                            if (isTextsdisAdjoin)
                            {
                                ed.WriteMessage("是否邻近选中的文字:" + isTextsdisAdjoin.ToString() + "\n");
                            }
                       
                            if (isTextsdisAdjoin)
                            {
                                //Point3d point3D = new Point3d(0, 0, 0);

                                //X坐标更改为相同,Y坐标更改为相差一定高度,Z坐标都更改为0
                                if (followedText_startPoint.Y > startText.Position.Y)
                                {
                                    ed.WriteMessage("目标文字在上面" + "\n");
                                    followedText_endPoint = new Point3d(endPoint.X, endPoint.Y + YSpacingCoefficient * TextHigh, 0);

                                }
                                else
                                {
                                    ed.WriteMessage("目标文字在下面" + "\n");
                                    followedText_endPoint = new Point3d(endPoint.X, endPoint.Y- YSpacingCoefficient * TextHigh, 0);
                                }

                                //改变为写
                                followedText.UpgradeOpen();

                                //改变要移动的单行文字的位置，以达到移动的目的
                                followedText.Position = followedText_endPoint;
                            }
                        }
                        //提交事务处理
                        trans.Commit();
                    }
                }
                else
                {
                    ed.WriteMessage("\n选择操作不成功\n");
                }


            }
            catch (System.Exception ex)
            {
                ed.WriteMessage($"\nException: {ex.Message}");
            }

        
            //连接所有的事件处理函数
            //AddEvents();
        }

        void commandEnded(object sender, CommandEventArgs e)
        {
            if (bMove == true)//判断AutoCAD命令是否为移动
                bMove = false;//设置全局变量bMove为False，表示移动命令结束
        }


        [CommandMethod("AddEvents")]
        public void AddEvents()
        {
            //把事件处理函数与相应的事件进行连接
            db.ObjectOpenedForModify += objectOpenedForModify;
            db.ObjectModified += objectModified;
            doc.CommandWillStart += commandWillStart;
            doc.CommandEnded += commandEnded;
            ed.WriteMessage("已增加移动事件监听!\n");
        }


        [CommandMethod("RemoveEvents")]
        public void removeEvents()
        {
            //断开所有的事件处理函数
            db.ObjectOpenedForModify -= objectOpenedForModify;
            db.ObjectModified -= objectModified;
            doc.CommandWillStart -= commandWillStart;
            doc.CommandEnded -= commandEnded;
            ed.WriteMessage("事件处理函数与事件已断开!\n");
        }

        /// <summary>
        /// 判断两个单行文字是否X坐标相同
        /// </summary>
        /// <param name="t1">文字1</param>
        /// <param name="t2">文字2</param>
        /// <param name="tol">容差</param>
        /// <param name="hDifference">位置Y坐标的高差容差范围</param>
        /// <returns></returns>
        public bool isTextsdistanceAdjoin(DBText t1, DBText t2, Double tol, Double hDifference)
        {
            Point3d p1 = t1.Position;
            Point3d p2 = t2.Position;
            bool b1 = isEqual(p1.X, p2.X, tol + hDifference);
            bool b2 = isEqual(p1.Z, p2.Z, tol);
            bool b3 = isEqual(p1.Y, p2.Y, tol + hDifference);   //Y方向
            return b1 && b2 && b3;
        }

        /// <summary>
        /// 比较两个点的X和Y坐标是否在容差范围内
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="tol"></param>
        /// <param name="hDifference"></param>
        /// <returns></returns>
        public bool isTextsdistanceAdjoin(Point3d p1, Point3d p2, Double tol, Double hDifference)
        {
            bool b1 = isEqual(p1.X, p2.X, tol + hDifference);
            bool b2 = isEqual(p1.Z, p2.Z, tol);
            bool b3 = isEqual(p1.Y, p2.Y, tol + hDifference);   //Y方向
            return b1 && b2 && b3;
        }



        /// <summary>
        /// 判断两个数是相等
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <param name="tol"></param>
        /// <returns></returns>
        public bool isEqual(Double d1, Double d2, Double tol)
        {
            return Math.Abs(d1 - d2) <= tol;
        }


    }

}
