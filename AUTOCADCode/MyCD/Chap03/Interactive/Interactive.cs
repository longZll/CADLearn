using System;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using DotNetARX;
namespace NetSelection
{
    public class Interactive
    {
        [CommandMethod("AddPoly")]
        public void AddPoly()
        {
            Document doc=Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            double width = 0;       //初始化线宽
            short colorIndex = 0;   //初始化颜色索引值
            int index = 2;          //初始化多段线顶点数
            ObjectId polyEntId = ObjectId.Null; //声明多段线的ObjectId
            
            //定义第一个点的用户交互类
            PromptPointOptions optPoint = new PromptPointOptions("\n请输入第一个点<100,200>");
            optPoint.AllowNone = true; //允许用户回车响应
            //返回点的用户提示类
            PromptPointResult resPoint = ed.GetPoint(optPoint);
            //用户按下ESC键，退出
            if (resPoint.Status == PromptStatus.Cancel)
                return;
            Point3d ptStart; //声明第一个输入点
            //用户按回车键
            if (resPoint.Status == PromptStatus.None)
                //得到第一个输入点的默认值
                ptStart = new Point3d(100, 200, 0);
            else
                //得到第一个输入点
                ptStart = resPoint.Value;
            Point3d ptPrevious = ptStart;   //保存当前点
            //定义输入下一点的点交互类
            PromptPointOptions optPtKey = new PromptPointOptions("\n请输入下一个点或[线宽(W)/颜色(C)/完成(O)]<O>");
            //为点交互类添加关键字
            optPtKey.Keywords.Add("W");
            optPtKey.Keywords.Add("C");
            optPtKey.Keywords.Add("O");
            optPtKey.Keywords.Default = "O";    //设置默认的关键字
            optPtKey.UseBasePoint = true;       //允许使用基准点
            optPtKey.BasePoint = ptPrevious;    //设置基准点
            optPtKey.AppendKeywordsToMessage = false;//不将关键字列表添加到提示信息中
            //提示用户输入点
            PromptPointResult resKey = ed.GetPoint(optPtKey);
            //如果用户输入点或关键字，则一直循环
            while (resKey.Status == PromptStatus.OK || resKey.Status == PromptStatus.Keyword)
            {
                Point3d ptNext= Point3d.Origin; //声明下一个输入点
                //如果用户输入的是关键字集合对象中的关键字
                if (resKey.Status == PromptStatus.Keyword)
                {
                    switch (resKey.StringResult)
                    {
                        case "W":
                            width = GetWidth();
                            break;
                        case "C":
                            colorIndex = GetColorIndex();
                            break;
                        case "O":
                            return;
                        default:
                            ed.WriteMessage("\n输入了无效关键字");
                            break;
                    }
                }
                else
                {
                    ptNext = resKey.Value;  //得到用户输入的下一点
                    if (index == 2)         //新建多段线,初始状态,多段线只有两个顶点
                    {
                        //提取三维点的X、Y坐标值，转化为二维点
                        Point2d pt1 = new Point2d(ptPrevious[0], ptPrevious[1]);
                        Point2d pt2 = new Point2d(ptNext[0], ptNext[1]);
                        Polyline  polyEnt= new Polyline();//新建一条多段线
                        //给多段线添加顶点，设置线宽
                        polyEnt.AddVertexAt(0, pt1, 0, width, width);
                        polyEnt.AddVertexAt(1, pt2, 0, width, width);
                        //设置多段线的颜色
                        polyEnt.Color = Color.FromColorIndex(ColorMethod.ByColor, colorIndex);
                        //将多段线添加到图形数据库并返回一个ObjectId(在绘图窗口动态显示多段线)
                        polyEntId = db.AddToModelSpace(polyEnt);
                    }
                    else  //修改多段线，添加最后一个顶点
                    {
                        using (Transaction trans = db.TransactionManager.StartTransaction())
                        {
                            //打开多段线的状态为写
                            Polyline  polyEnt=trans.GetObject(polyEntId, OpenMode.ForWrite) as Polyline;
                            if (polyEnt != null)
                            {
                                //继续添加多段线的顶点
                                Point2d ptCurrent = new Point2d(ptNext[0], ptNext[1]);
                                polyEnt.AddVertexAt(index - 1, ptCurrent, 0, width, width);
                                //重新设置多段线的颜色和线宽
                                polyEnt.Color = Color.FromColorIndex(ColorMethod.ByColor, colorIndex);
                                polyEnt.ConstantWidth = width;
                            }
                            trans.Commit();
                        }
                    }
                    index++;
                }
                ptPrevious = ptNext;
                optPtKey.BasePoint = ptPrevious;    //重新设置基准点
                resKey = ed.GetPoint(optPtKey);     //提示用户输入新的顶点
            }
        }

        // 得到用户输入线宽的函数.
        public double GetWidth()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            //定义一个实数的用户交互类.
            PromptDoubleOptions optDou = new PromptDoubleOptions("\n请输入线宽");
            optDou.AllowNegative = false;   //不允许输入负数
            optDou.DefaultValue = 0;        //设置默认值
            PromptDoubleResult resDou = ed.GetDouble(optDou);
            if (resDou.Status == PromptStatus.OK)
            {
                Double width = resDou.Value;
                return width; //得到用户输入的线宽
            }
            else
                return 0;
        }

        // 得到用户输入颜色索引值的函数.
        public short GetColorIndex()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            //定义一个整数的用户交互类
            PromptIntegerOptions optInt = new PromptIntegerOptions("\n请输入颜色索引值(0～256)");
            optInt.DefaultValue = 0;    //设置默认值
            //返回一个整数提示类
            PromptIntegerResult resInt = ed.GetInteger(optInt);
            if (resInt.Status == PromptStatus.OK)
            {
                //得到用户输入的颜色索引值
                short colorIndex = (short)resInt.Value;
                if (colorIndex > 256 | colorIndex < 0)
                    return 0;
                else
                    return colorIndex;
            }
            else
                return 0;
        }
    }
}
