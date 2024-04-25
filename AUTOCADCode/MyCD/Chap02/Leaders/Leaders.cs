using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using DotNetARX;
namespace Leaders
{
    public class Leaders
    {
        /// <summary>
        /// 添加引线
        /// </summary>
        [CommandMethod("AddLeader")]
        public void AddLeader()
        {
            Database db=HostApplicationServices.WorkingDatabase;
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                //创建一个在原点直径为0.219的圆。
                Circle circle=new Circle();
                circle.Center = Point3d.Origin;
                circle.Diameter = 0.219;
                //创建一个多行文本并设置其内容为4Xφd±0.005（其中d为圆的直径）
                MText txt=new MText();
                txt.Contents = "4X" + TextSpecialSymbol.Diameter + circle.Diameter + TextSpecialSymbol.Tolerance + "0.005";
                txt.Location = new Point3d(1, 1, 0);//文本位置
                txt.TextHeight = 0.2;//文本高度 
                db.AddToModelSpace(circle, txt);//将圆和文本添加到模型空间中                                
                Leader leader=new Leader();//创建一个引线对象
                //将圆上一点及文本位置作为引线的顶点
                leader.AppendVertex(circle.Center.PolarPoint(Math.PI / 3, circle.Radius));
                leader.AppendVertex(txt.Location);
                db.AddToModelSpace(leader);//将引线添加到模型空间中
                leader.Dimgap = 0.1;//设置引线的文字偏移为0.1
                leader.Dimasz = 0.1;//设置引线的箭头大小为0.1
                leader.Annotation = txt.ObjectId;//设置引线的注释对象为文本
                leader.EvaluateLeader();//计算引线及其关联注释之间的关系
                trans.Commit();//提交更改
            }
        }


        [CommandMethod("AddCoordMLeader")]
        public void AddCoordMLeader()
        {
            Database db=HostApplicationServices.WorkingDatabase;
            //获取符号为无的箭头块的ObjectId
            ObjectId arrowId=db.GetArrowObjectId(DimArrowBlock.None);
            //如果当前图形中还未加入上述箭头块，则加入并获取其ObjectId
            if (arrowId == ObjectId.Null)
            {
                DimTools.ArrowBlock = DimArrowBlock.None;
                arrowId = db.GetArrowObjectId(DimArrowBlock.None);
            }
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                Point3d ptCoord=new Point3d(80, 30, 0);//要标注的坐标点
                MText mtext=new MText();//新建多行文本
                //设置多行文本的内容为点的坐标值，并且分两行表示
                mtext.Contents = "X:" + ptCoord.X.ToString("0.000") + @"\PY:" + ptCoord.Y.ToString("0.000");
                mtext.LineSpacingFactor = 0.8;  //多行文本的行间距
                MLeader leader=new MLeader();   //创建多重引线
                //为多重引线添加引线束，引线束由基线和一些单引线构成
                int leaderIndex=leader.AddLeader();
                //在引线束中添加单引线
                int lineIndex=leader.AddLeaderLine(leaderIndex);
                //在单引线中添加引线头点（引线箭头所指向的点），位置为要进行标注的点
                leader.AddFirstVertex(lineIndex, ptCoord);
                //在单引线中添加引线终点
                leader.AddLastVertex(lineIndex, ptCoord.PolarPoint(Math.PI / 4, 10));
                //设置单引线的注释类型为多行文本
                leader.ContentType = ContentType.MTextContent;
                leader.MText = mtext;//设置单引线的注释文字
                //将多重引线添加到模型空间
                db.AddToModelSpace(leader);
                leader.ArrowSymbolId = arrowId;//设置单引线的箭头块ObjectId
                leader.DoglegLength = 0;//设置单引线的基线长度为0
                //将基线连接到引线文字的下方并且绘制下划线
                leader.TextAttachmentType = TextAttachmentType.AttachmentBottomOfTopLine;
                trans.Commit();
            }
        }
        [CommandMethod("AddMLeader")]
        public void AddMLeader()
        {
            Database db=HostApplicationServices.WorkingDatabase;
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                //创建3个点，分别表示引线的终点和两个头点
                Point3d ptEnd=new Point3d(90, 0, 0);
                Point3d pt1=new Point3d(80, 20, 0);
                Point3d pt2=new Point3d(100, 20, 0);
                MText mtext=new MText();//新建多行文本
                mtext.Contents = "多重引线示例";//文本内容
                MLeader mleader=new MLeader();//创建多重引线
                //为多重引线添加引线束，引线束由基线和一些单引线构成
                int leaderIndex=mleader.AddLeader();
                //在引线束中添加一单引线
                int lineIndex=mleader.AddLeaderLine(leaderIndex);
                mleader.AddFirstVertex(lineIndex, pt1); //在单引线中添加引线头点

                mleader.AddLastVertex(lineIndex, ptEnd); //在单引线中添加引线终点
                //在引线束中再添加一单引线，并只设置引线头点
                lineIndex = mleader.AddLeaderLine(leaderIndex);
                mleader.AddFirstVertex(lineIndex, pt2);
                //设置多重引线的注释为多行文本
                mleader.ContentType = ContentType.MTextContent;
                mleader.MText = mtext;
                //将多重引线添加到模型空间
                db.AddToModelSpace(mleader);
                trans.Commit();
            }
        }
        [CommandMethod("AddRoadMLeader")]
        public void AddRoadMLeader()
        {
            Database db=HostApplicationServices.WorkingDatabase;
            //获取符号为点的箭头块的ObjectId
            ObjectId arrowId=db.GetArrowObjectId(DimArrowBlock.Dot);
            //如果当前图形中还未加入上述箭头块，则加入并获取其ObjectId
            if (arrowId == ObjectId.Null)
            {
                DimTools.ArrowBlock = DimArrowBlock.Dot;
                arrowId = db.GetArrowObjectId(DimArrowBlock.Dot);
            }
            //创建一个点列表，在其中添加4个要标注的点
            List<Point3d> pts=new List<Point3d>();
            pts.Add(new Point3d(150, 0, 0));
            pts.Add(new Point3d(150, 15, 0));
            pts.Add(new Point3d(150, 18, 0));
            pts.Add(new Point3d(150, 20, 0));
            //各标注点对应的文字
            List<string> contents=new List<string> { "道路中心线", "机动车道", "人行道", "绿化带" };
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                for (int i = 0; i < pts.Count; i++)//遍历标注点
                {
                    MText txt=new MText();//创建多行文本
                    txt.Contents = contents[i];//文本内容
                    MLeader mleader=new MLeader();//创建多重引线
                    //为多重引线添加引线束，引线束由基线和一些单引线构成
                    int leaderIndex=mleader.AddLeader();
                    //在引线束中添加一单引线，并设置引线头点和终点
                    int lineIndex=mleader.AddLeaderLine(leaderIndex);
                    mleader.AddFirstVertex(lineIndex, pts[i]);
                    mleader.AddLastVertex(lineIndex, pts[0].PolarPoint(Math.PI / 2, 20 + (i + 1) * 5));
                    mleader.ArrowSymbolId = arrowId;//设置单引线的箭头块ObjectId
                    //设置多重引线的注释为多行文本
                    mleader.ContentType = ContentType.MTextContent;
                    mleader.MText = txt;
                    db.AddToModelSpace(mleader);
                    mleader.ArrowSize = 1;//多重引线箭头大小
                    mleader.DoglegLength = 0;//多重引线基线长度设为0
                    //将基线连接到引线文字的下方并且绘制下划线
                    mleader.TextAttachmentType = TextAttachmentType.AttachmentBottomLine;
                }
                trans.Commit();
            }
        }
        [CommandMethod("AddTolerance")]
        public void AddTolerance()
        {
            Database db=HostApplicationServices.WorkingDatabase;
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                //创建一个形位公差特征控制框
                FeatureControlFrame frame=new FeatureControlFrame();
                //形位公差的几何特征为位置
                string geometricSym=DimFormatCode.Position;
                //形位公差值为0.20，且带直径符号，包容条件为最大实体要求
                string torlerance=DimFormatCode.Diameter + "0.20" + DimFormatCode.CircleM;
                //形位公差的第一级基准符号,包容条件为最大实体要求
                string firstDatum="A" + DimFormatCode.CircleM;
                //形位公差的第二级基准符号,包容条件为不考虑特征尺寸
                string secondDatum="B" + DimFormatCode.CircleS;
                //形位公差的第三级基准符号,包容条件为最小实体要求
                string thirdDatum="C" + DimFormatCode.CircleL;
                //设置公差特征控制框的内容为形位公差
                frame.CreateTolerance(geometricSym, torlerance, firstDatum, secondDatum, thirdDatum);
                frame.Location = new Point3d(1, 0.5, 0);//控制框的位置
                frame.Dimscale = 0.05;//控制框的大小
                db.AddToModelSpace(frame);//控制框添加到模型空间中
                trans.Commit();//提交更改
            }
        }

    }
}
