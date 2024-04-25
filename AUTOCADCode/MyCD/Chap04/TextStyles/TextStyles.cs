using System;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using DotNetARX;
namespace TextStyles
{
    public class TextStyles
    {
        [CommandMethod("NewStyle")]
        public void NewStyle()
        {
            Database db=HostApplicationServices.WorkingDatabase;
            Editor ed=Application.DocumentManager.MdiActiveDocument.Editor;
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                //设置TrueType字体(仿宋体）
                ObjectId styleId=db.AddTextStyle("仿宋体", "simfang.ttf");
                DBText txt1=new DBText();
                txt1.TextString = "仿宋体";
                txt1.TextStyleId = styleId;//文字样式
                //设置SHX字体(romans）
                styleId = db.AddTextStyle("罗马字体", "romans", "gbcbig");

                DBText txt2=new DBText();
                txt2.TextString = "罗马字体";
                txt2.TextStyleId = styleId;//文字样式
                txt2.Position = txt1.Position.PolarPoint(Math.PI / 2, 5);
                //设置SHX字体(romans）
                styleId = db.AddTextStyle("垂直罗马字体", "romans", "gbcbig");
                //设置文字样式的各种属性
                styleId.SetTextStyleProp(2.0, 0.67, 15 * Math.PI / 180, true, true, true, AnnotativeStates.True, true);
                
                DBText txt3=new DBText();
                txt3.TextString = "垂直罗马字体";
                txt3.TextStyleId = styleId;
                txt3.Position = txt1.Position.PolarPoint(0, 20);
                //设置TrueType字体(宋体）并且有加粗、倾斜效果
                styleId = db.AddTextStyle("加粗斜宋体", "宋体", true, true, 0, 0);
                
                DBText txt4=new DBText();
                txt4.TextString = "加粗斜宋体";
                txt4.TextStyleId = styleId;
                txt4.Position = txt2.Position.PolarPoint(Math.PI / 2, 5);

                db.AddToModelSpace(txt1, txt2, txt3, txt4);


                //设置单行文本的属性为当前文字样式的属性
                //txt1.SetFromTextStyle();//必须将文本加入到数据库后再匹配文字样式的属性
                //txt2.SetFromTextStyle();//必须将文本加入到数据库后再匹配文字样式的属性
                txt3.SetFromTextStyle();//必须将文本加入到数据库后再匹配文字样式的属性
                //txt4.SetFromTextStyle();//必须将文本加入到数据库后再匹配文字样式的属性                    

                trans.Commit();
            }
        }
    }
}
