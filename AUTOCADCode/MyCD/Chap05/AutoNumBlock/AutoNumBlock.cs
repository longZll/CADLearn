using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using DotNetARX;
namespace AutoNumBlock
{
    public class AutoNumBlock : IExtensionApplication
    {
        public static NumManager manager;//自动编号管理类
        //设置自动编号的格式
        [CommandMethod("NumOption")]
        public void SetNumOption()
        {
            Editor ed=Application.DocumentManager.MdiActiveDocument.Editor;
            //存储原来的前后缀，用于判断后面的操作是否对其进行了改变
            string oldPrefix=manager.Prefix;
            string oldPostfix=manager.Suffix;
            //定义一个整型交互类，提示用户对编号的格式进行设置
            PromptIntegerOptions opt=new PromptIntegerOptions("\n请输入编号基数或[前缀(P)/后缀(S)/增量(I)/编号方向(D)]<P>");
            //为整型交互类添加关键字
            opt.Keywords.Add("P");
            opt.Keywords.Add("S");
            opt.Keywords.Add("I");
            opt.Keywords.Add("D");
            opt.Keywords.Default = "P";//设置默认的关键字
            opt.AppendKeywordsToMessage = false;// 不将关键字列表添加到提示中
            //提示用户输入表示自动编号的基数
            PromptIntegerResult result=ed.GetInteger(opt);
            //如果用户输入整数或关键字，则一直循环进行编号的格式设置
            while (result.Status == PromptStatus.OK || result.Status == PromptStatus.Keyword)
            {
                //如果用户输入的是关键字集合对象中的关键字
                if (result.Status == PromptStatus.Keyword)
                {
                    switch (result.StringResult)
                    {
                        case "P"://编号的前缀
                            //提示用户输入编号的前缀
                            PromptResult prResult= ed.GetString("请输入前缀");
                            if (prResult.Status != PromptStatus.OK) return;
                            //设置编号的前缀
                            manager.Prefix = prResult.StringResult;
                            break;
                        case "S"://编号的后缀
                            PromptResult poResult= ed.GetString("请输入后缀");
                            if (poResult.Status != PromptStatus.OK) return;
                            manager.Suffix = poResult.StringResult;
                            break;
                        case "I":
                            PromptIntegerResult iResult= ed.GetInteger("编号增量");
                            if (iResult.Status != PromptStatus.OK) return;
                            manager.Increment = iResult.Value;
                            break;
                        case "D"://编号的增量
                            //定义一个关键字交互类，提示用户对编号的方向进行设置
                            PromptKeywordOptions keyOpt=new PromptKeywordOptions("编号方向[行(R)/列(C)]<R>");
                            keyOpt.Keywords.Add("R", "R", "R", false, true);
                            keyOpt.Keywords.Add("C", "C", "C", false, true);
                            keyOpt.Keywords.Default = "R";
                            //提示用户输入关键字以确定是按行还是列进行编号
                            PromptResult keyResult= ed.GetKeywords(keyOpt);
                            if (keyResult.Status != PromptStatus.OK) return;
                            //如果用户输入R关键字，则按行编号，否则按列编号
                            manager.IsRowDirection = keyResult.StringResult == "R" ? true : false;
                            break;
                    }
                }
                else//如果用户输入的整数值
                {
                    manager.BaseNum = result.Value;//设置编号的基数
                }
                result = ed.GetInteger(opt);//提示用户输入新的整数
            }
            //编号的前缀或后缀已经被用户设置成新的值，应更新图形中的编号
            if (manager.Prefix != oldPrefix || manager.Suffix != oldPostfix)
            {
                if (manager.Numbers.Count > 0)//如果图形中存在编号
                {
                    UpdateNumbers();
                }
            }
        }

        public void UpdateNumbers()
        {
            Database db=HostApplicationServices.WorkingDatabase;
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                //遍历图形中的编号
                foreach (var dup in manager.Numbers)
                {
                    //定义一个字典，用于编号所表示的块属性及属性值
                    Dictionary<string,string> atts=new Dictionary<string, string>();
                    //对编号进行更新
                    atts.Add(manager.AttName, manager.Prefix + dup.Key + manager.Suffix);
                    //更新编号所表示的块
                    dup.Value.UpdateAttributesInBlock(atts);
                }
                trans.Commit();

            }
        }

        //在指定位置进行自动编号
        [CommandMethod("NumsAtPoints")]
        public void NumsAtPoints()
        {
            Document doc=Application.DocumentManager.MdiActiveDocument;
            using (Transaction trans=doc.Database.TransactionManager.StartTransaction())
            {
                bool isOver=false;//是否结束编号操作
                while (!isOver)//如果没有结束，则一直编号
                {
                    //提示用户输入需要在编号的位置
                    PromptPointOptions opt=new PromptPointOptions("\n请输入点");
                    PromptPointResult res=doc.Editor.GetPoint(opt);
                    //如果用户按Esc键终止操作，则结束循环
                    if (res.Status != PromptStatus.OK) isOver = true;
                    else insertNumBlock(res.Value);//在指定点插入表示编号的块
                }
                trans.Commit();
            }
        }

        //根据选择的圆进行自动编号
        [CommandMethod("NumsInCircles")]
        public void NumsInCircles()
        {
            Database db=HostApplicationServices.WorkingDatabase;
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                var cirs=db.GetSelection<Circle>();//用户选择圆
                if (cirs.Count == 0) return;//若未选中圆，则返回
                //如果只选中一个圆，则在该圆的圆心位置插入表示编号的块
                if (cirs.Count == 1) insertNumBlock(cirs.First().Center);
                //获取所选择的最大圆的半径
                double maxRadius=cirs.Max(c => c.Radius);
                //按行自动编号
                if (manager.IsRowDirection)
                {
                    //获取圆心的最小与最大Y值
                    double bottomY=cirs.Min(c => c.Center.Y);
                    double topY=cirs.Max(c => c.Center.Y);
                    //根据最大圆的半径及圆之间的最大竖直间距，计算可以将圆分成几行
                    int row=Convert.ToInt32(Math.Ceiling((topY - bottomY) / maxRadius));
                    for (int i = 0; i < row; i++)
                    {
                        //使用LINQ语句筛选在当前行内的圆
                        var cirPos=from c in cirs
                                   where c.Center.Y > topY - (2 * i + 1) * maxRadius
                                           && c.Center.Y <= topY + (1 - 2 * i) * maxRadius
                                   orderby c.Center.X //根据圆心的X值进行排序
                                   select c.Center;   //返回符合条件的圆心
                        //遍历当前行内的圆
                        foreach (var pos in cirPos)
                        {
                            insertNumBlock(pos);//在圆心位置插入表示编号的块
                        }
                    }
                }
                else //按列自动编号
                {
                    //获取圆心的最小与最大X值
                    double leftX=cirs.Min(c => c.Center.X);
                    double rightY=cirs.Max(c => c.Center.X);
                    //根据最大圆的半径及圆之间的最大水平间距，计算可以将圆分成几列
                    int col=Convert.ToInt32(Math.Ceiling((rightY - leftX) / maxRadius));
                    for (int i = 0; i < col; i++)
                    {
                        //使用LINQ语句筛选在当前列内的圆
                        var cirPos=from c in cirs
                                   where c.Center.X > leftX + (2 * i - 1) * maxRadius
                                           && c.Center.X <= leftX + (2 * i + 1) * maxRadius
                                   orderby c.Center.Y descending //根据圆心的Y值进行倒序排列
                                   select c.Center;   //返回符合条件的圆心
                        //遍历当前列内的圆
                        foreach (var pos in cirPos)
                        {
                            insertNumBlock(pos);//在圆心位置插入表示编号的块
                        }
                    }
                }
                trans.Commit();
            }
        }

        //在指定位置插入表示编号的块
        private void insertNumBlock(Point3d pt)
        {
            Database db=HostApplicationServices.WorkingDatabase;
            ObjectId model=db.GetModelSpaceId();//模型空间的Id
            //获取当前可用的编号，如果最大编号小于基数值，则表示无自动编号，应从基数值开始编号；
            //否则，可用编号应设定为当前最大编号+自动编号增量
            int num=manager.MaxNum < manager.BaseNum ? manager.BaseNum : manager.MaxNum + manager.Increment;
            manager.MaxNum = num;//最大编号已改变，应更新
            //定义一个字典，用于编号所表示的块属性及属性值
            Dictionary<string,string> atts=new Dictionary<string, string>();
            //设置含前后缀的编号值
            atts.Add(manager.AttName, manager.Prefix + num + manager.Suffix);
            //在模型空间插入新编号块
            ObjectId brId=model.InsertBlockReference(manager.LayerName, manager.BlockName, pt, new Scale3d(), 0, atts);
            manager.Numbers.Add(num, brId);//在编号管理器中添加新的编号值及对应的块参照Id
        }

        //将编号及其坐标输出到文本文件
        [CommandMethod("ExportToFile")]
        public void ExportToFile()
        {
            Database db=HostApplicationServices.WorkingDatabase;
            //设置文件名            
            string fileName=Tools.GetCurrentPath() + "\\Points.txt";
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                //将点的坐标值遍历进文本文件，若文件不存在则先会创建文件
                foreach (var num in manager.Numbers)
                {
                    BlockReference br=(BlockReference)trans.GetObject(num.Value, OpenMode.ForRead);
                    string numInfo=manager.Prefix + num.Key + manager.Suffix + "," + br.Position.X + "," + br.Position.Y + "," + br.Position.Z;
                    File.AppendAllText(fileName, numInfo + Environment.NewLine);
                }
                trans.Commit();
            }
        }
        public void Initialize()
        {
            //初始化编号管理器，设置编号使用的块名与块属性
            manager = new NumManager { BlockName = "BUBBLE", AttName = "NUMBER" };
        }
        public void Terminate()
        {

        }
    }
}
