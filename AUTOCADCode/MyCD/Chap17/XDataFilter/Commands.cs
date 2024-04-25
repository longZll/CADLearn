using System.Collections.Generic;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using DotNetARX;
namespace XDataFilter
{
    public class Commands : IExtensionApplication
    {
        private static DrawRule theOverrule;//重定义对象        
        public static readonly string appName="Door";//扩展数据应用程序名
        //定义门的类型和价格
        public static Dictionary<string,double> doors=new Dictionary<string, double>();
        [CommandMethod("Door")]
        public static void DrawDoor()
        {
            Document doc=Application.DocumentManager.MdiActiveDocument;
            Database db=doc.Database;
            Editor ed=doc.Editor;
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                string doorType="木门";//门类型
                //提示用户输入表示门类型的关键字
                PromptKeywordOptions opt=new PromptKeywordOptions("\n请输入门的类型[木门(M)/铁门(T)/铝合金门(L)]<M>");
                opt.Keywords.Add("M");
                opt.Keywords.Add("T");
                opt.Keywords.Add("L");
                opt.Keywords.Default = "M";//缺省关键字
                opt.AppendKeywordsToMessage = false; //不将关键字列表添加到提示信息
                PromptResult result=ed.GetKeywords(opt);
                //根据用户输入的关键字，设置门的类型
                if (result.Status == PromptStatus.OK)
                {
                    switch (result.StringResult)
                    {
                        case "M":
                            break;
                        case "T":
                            doorType = "铁门";
                            break;
                        case "L":
                            doorType = "铝合金门";
                            break;
                    }
                }
                //选择需要转化门的直线
                ed.WriteMessage("\n请选择要转化为门的直线");
                var lines=db.GetSelection<Line>();
                if (lines.Count == 0) return;
                //遍历选择的直线，为其添加Door扩展数据
                foreach (var line in lines)
                {
                    TypedValueList values=new TypedValueList();
                    values.Add(DxfCode.ExtendedDataAsciiString, doorType);//门类型
                    values.Add(DxfCode.ExtendedDataReal, doors[doorType]);//门价格
                    line.ObjectId.AddXData(appName, values);//添加扩展数据
                }
                trans.Commit();
            }
        }
        public void Initialize()
        {
            //为直线类型添加OPM支持，以在属性面板上显示其扩展数据
            Masterhe.OPM.OPMManager.Add(typeof(Line), new DoorAtt());
            //添加各种类型的门及其价格
            doors.Add("木门", 1000);
            doors.Add("铁门", 2000);
            doors.Add("铝合金门", 3000);
            //初始化重定义对象
            if (theOverrule == null)
            {
                theOverrule = new DrawRule();
                //为直线类添加显示重定义
                Overrule.AddOverrule(RXObject.GetClass(typeof(Line)), theOverrule, false);
                //添加扩展数据过滤，只重定义有Door扩展数据的直线
                theOverrule.SetXDataFilter(appName);
            }
            Overrule.Overruling = true;//开启规则重定义
            //刷新屏幕，直线被更新为门
            Application.DocumentManager.MdiActiveDocument.Editor.Regen();
        }
        public void Terminate()
        {

        }
    }
}
