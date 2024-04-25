using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using DotNetARX;
namespace LineTypes
{
    public class LineTypes
    {
        [CommandMethod("NewLineType")]
        public void NewLineType()
        {
            Database db=HostApplicationServices.WorkingDatabase;
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                //1.添加虚线线型
                ObjectId ltId=db.AddLineType("DashLines");
                LinetypeTableRecord ltrDashLine=(LinetypeTableRecord)trans.GetObject(ltId, OpenMode.ForWrite);
                ltrDashLine.AsciiDescription = "虚线";    //线型说明
                ltrDashLine.PatternLength = 0.95;        //组成线型的图案长度（划线、空格、点）
                ltrDashLine.NumDashes = 4;      //组成线型的图案数目
                ltrDashLine.SetDashLengthAt(0, 0.5);    //0.5个单位的划线
                ltrDashLine.SetDashLengthAt(1, -0.25);  //0.25个单位的空格
                ltrDashLine.SetDashLengthAt(2, 0);      //一个点
                ltrDashLine.SetDashLengthAt(3, -0.25);  //0.25个单位的空格
                //打开文字样式表，用于文字类型的线型
                TextStyleTable tst=(TextStyleTable)trans.GetObject(db.TextStyleTableId, OpenMode.ForRead);
                
                //2.添加带文字的线型
                ObjectId ltTextId=db.AddLineType("Texts");
                LinetypeTableRecord ltrText=(LinetypeTableRecord)trans.GetObject(ltTextId, OpenMode.ForWrite);
                
                ltrText.AsciiDescription = "文字";//线型说明
                ltrText.PatternLength = 0.9;    //组成线型的图案长度（划线、空格、点）
                ltrText.NumDashes = 3;          //组成线型的图案数目

                ltrText.SetDashLengthAt(0, 0.5);            //0.5个单位的划线
                ltrText.SetDashLengthAt(1, -0.2);           //0.2个单位的空格
                ltrText.SetShapeStyleAt(1, tst["Standard"]);    //设置文字的文字样式
                
                //文字在线型的 X 轴方向上向左移动0.1个单位，在Y轴方向向下移动0.05个单位。
                ltrText.SetShapeOffsetAt(1, new Vector2d(-0.1, -0.05));
                ltrText.SetShapeScaleAt(1, 0.1);    //文字的缩放比例
                ltrText.SetShapeRotationAt(1, 0);   //文字的旋转角度为0（不旋转）
                ltrText.SetTextAt(1, "CAD");        //文字内容
                ltrText.SetDashLengthAt(2, -0.2);   //0.2个单位的空格
                
                //3.将ltypeshp.shx文件添加到当前数据库，该文件包含圆形图案
                ObjectId txtStyleId=db.AddShapeTextStyle("ShpText", "ltypeshp.shx");
                //添加圆型线型
                ObjectId ltCirId=db.AddLineType("Circles");
                LinetypeTableRecord ltrCir=(LinetypeTableRecord)trans.GetObject(ltCirId, OpenMode.ForWrite);
                ltrCir.AsciiDescription = "圆";//线型说明
                ltrCir.PatternLength = 1.45;//组成线型的图案长度（划线、空格、点）
                ltrCir.NumDashes = 4;//组成线型的图案数目
                ltrCir.SetDashLengthAt(0, 0.25);//0.25个单位的划线
                ltrCir.SetDashLengthAt(1, -0.1);//0.1个单位的空格
                ltrCir.SetShapeStyleAt(1, txtStyleId);//设置空格处的图形文件
                //设置空格处要包含的图形为圆形
                ltrCir.SetShapeNumberAt(1, (int)LineTypeTools.Shape.Circle);
                //图形在线型的 X 轴方向上向左移动0.1个单位，在Y轴方向不移动。
                ltrCir.SetShapeOffsetAt(1, new Vector2d(-0.1, 0.0));
                ltrCir.SetShapeScaleAt(1, 0.1);//图形的缩放比例
                ltrCir.SetShapeRotationAt(1, 0);//文字的旋转角度为0（不旋转）
                ltrCir.SetDashLengthAt(2, -0.1);//0.1个单位的空格
                ltrCir.SetDashLengthAt(3, 1.0);//1个单位的划线 

                db.Celtype = ltId;//设置当前线型为虚线
                trans.Commit();
            }
        }
        
        
        [CommandMethod("AddLines")]
        public void AddLines()
        {
            Database db=HostApplicationServices.WorkingDatabase;
            //Editor ed=db.GetEditor();
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                //点列表用于各直线的起点和终点
                Point3dList startPts=new Point3dList();
                Point3dList endPts=new Point3dList();
                for (int i = 0; i < 5; i++)
                {
                    startPts.Add(new Point3d(0, 0.5 * i, 0));
                    endPts.Add(new Point3d(5, 0.5 * i, 0));
                }
                //虚线
                Line dashLine1=new Line(startPts[0], endPts[0]);
                dashLine1.Linetype = "DashLines";
                //设置线型比例为2的虚线（未指定线型，则设置为当前线型：虚线）
                Line dashLine2=new Line(startPts[1], endPts[1]);
                dashLine2.LinetypeScale = 2;
                //文字线型
                Line textLine=new Line(startPts[2], endPts[2]);
                textLine.Linetype = "Texts";
                Line circleLine=new Line(startPts[3], endPts[3]);
                //圆形线型，并设置线宽为0.3
                circleLine.Linetype = "Circles";
                circleLine.LineWeight = LineWeight.LineWeight030;
                //加载系统自带的Center线型
                Line centerLine=new Line(startPts[4], endPts[4]);
                ObjectId centerId=db.LoadLineType("Center");
                if (centerId != null) centerLine.LinetypeId = centerId;
                //将所有的直线添加到模型空间
                db.AddToModelSpace(dashLine1, dashLine2, textLine, circleLine, centerLine);
                trans.Commit();
            }
            db.LineWeightDisplay = true;//显示线宽
        }
    }
}
