using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using DotNetARX;



namespace Texts
{
    public class Texts
    {
        /// <summary>
        /// 创建单行文字
        /// </summary>
        [CommandMethod("AddText")]
        public void AddText()
        {
            Database db=HostApplicationServices.WorkingDatabase;

            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                DBText textFirst=new DBText(); // 创建第一个单行文字
                textFirst.Position = new Point3d(50, 50, 0);//文字位置
                textFirst.Height = 5;//文字高度 
                //设置文字内容，特殊格式为≈、下划线和平方
                textFirst.TextString = "面积" + TextSpecialSymbol.AlmostEqual + TextSpecialSymbol.Underline + "2000" + TextSpecialSymbol.Underline + "m" + TextSpecialSymbol.Square;
                //设置文字的水平对齐方式为居中
                textFirst.HorizontalMode = TextHorizontalMode.TextCenter;
                //设置文字的垂直对齐方式为居中
                textFirst.VerticalMode = TextVerticalMode.TextVerticalMid;
                //设置文字的对齐点
                textFirst.AlignmentPoint = textFirst.Position;

                DBText textSecond=new DBText();// 创建第二个单行文字
                textSecond.Height = 5; //文字高度
                //设置文字内容，特殊格式为角度、希腊字母和度数
                textSecond.TextString = TextSpecialSymbol.Angle + TextSpecialSymbol.Belta + "=45" + TextSpecialSymbol.Degree;
                //设置文字的对齐方式为居中对齐
                textSecond.HorizontalMode = TextHorizontalMode.TextCenter;
                textSecond.VerticalMode = TextVerticalMode.TextVerticalMid;
                //设置文字的对齐点
                textSecond.AlignmentPoint = new Point3d(50, 40, 0);

                DBText textLast=new DBText();//创建第三个单行文字
                textLast.Height = 5;// 文字高度
                //设置文字的内容，特殊格式为直径和公差
                textLast.TextString = TextSpecialSymbol.Diameter + "30的直径偏差为" + TextSpecialSymbol.Tolerance + "0.01";
                //设置文字的对齐方式为居中对齐
                textLast.HorizontalMode = TextHorizontalMode.TextCenter;
                textLast.VerticalMode = TextVerticalMode.TextVerticalMid;
                //设置文字的对齐点
                textLast.AlignmentPoint = new Point3d(50, 30, 0);

                //添加文本到模型空间
                db.AddToModelSpace(textFirst, textSecond, textLast);
                trans.Commit();//提交事务处理
            }
        }
        /// <summary>
        /// 创建多行文本对象/创建堆叠文字对象
        /// </summary>
        [CommandMethod("AddStackText")]
        public void AddStackText()
        {
            Database db=HostApplicationServices.WorkingDatabase;

            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                MText mtext=new MText();//创建多行文本对象
                mtext.Location = new Point3d(100, 40, 0);//位置
                //创建水平分数形式的堆叠文字
                string firstLine=TextTools.StackText(TextSpecialSymbol.Diameter + "20", "H7", "P7", StackType.HorizontalFraction, 0.5);
                //创建斜分数形式的堆叠文字
                string secondLine=TextTools.StackText(TextSpecialSymbol.Diameter + "20", "H7", "P7", StackType.ItalicFraction, 0.5);
                //创建公差形式的堆叠文字
                string lastLine=TextTools.StackText(TextSpecialSymbol.Diameter + "20", "+0.020", "-0.010", StackType.Tolerance, 0.5);
                
                //将前面定义的堆叠文字合并，作为多行文本的内容
                mtext.Contents = firstLine + MText.ParagraphBreak + secondLine + "\n" + lastLine;
                mtext.TextHeight = 5;//文本高度
                mtext.Width = 0;//文本宽度，设为0表示不会自动换行
                //设置多行文字的对齐方式正中
                mtext.Attachment = AttachmentPoint.MiddleCenter;

                db.AddToModelSpace(mtext);//添加文本到模型空间中
                trans.Commit();//提交事务处理
            }
        }
    }
}
