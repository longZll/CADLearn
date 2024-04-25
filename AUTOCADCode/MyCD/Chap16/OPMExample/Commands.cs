using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNetARX;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
namespace OPMExample
{
    public class Commands : IExtensionApplication
    {
        //设置标识扩展数据的注册应用程序名
        public static readonly string appName="EMPLOYEE";
        //放置员工职位和年薪的Dictionary类变量
        public static Dictionary<string,double> employees=new Dictionary<string, double>();
        [CommandMethod("AddX")]
        public static void AddX()
        {
            Document doc=Application.DocumentManager.MdiActiveDocument;
            Database db=doc.Database;
            Editor ed=doc.Editor;
            //设置缺省的员工类型
            string empType="普通员工";
            //定义输入员工类型的关键字交互类
            PromptKeywordOptions optK=new PromptKeywordOptions("\n请选择员工类型[普通员工(P)/管理人员(G)/董事长(D)]<P>");
            optK.Keywords.Add("P");
            optK.Keywords.Add("G");
            optK.Keywords.Add("D");
            optK.Keywords.Default = "P";//设置默认的关键字
            //提示用户输入员工类型
            PromptResult result=ed.GetKeywords(optK);
            if (result.Status != PromptStatus.OK) return;
            //根据用户输入的关键字，设置对应的员工类型
            switch (result.StringResult)
            {
                case "P":
                    break;
                case "G":
                    empType = "管理人员";
                    break;
                case "D":
                    empType = "董事长";
                    break;
            }
            //提示用户选择一个表示员工的多行文本
            PromptEntityOptions optE=new PromptEntityOptions("\n请选择表示员工的多行文本");
            //设置用户能选择的对象类型为多行文本
            optE.SetRejectMessage("\n您选择的不是多行文本，请重新选择");
            optE.AddAllowedClass(typeof(MText), true);
            PromptEntityResult entResult=ed.GetEntity(optE);
            if (entResult.Status != PromptStatus.OK) return;
            ObjectId id=entResult.ObjectId;//用户选择的多行文本的ObjectId
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                //定义一个TypedValue列表用来定义扩展数据
                TypedValueList values=new TypedValueList();
                //添加字符串（表示职位）和实型（表示年薪）扩展数据项
                values.Add(DxfCode.ExtendedDataAsciiString, empType);
                values.Add(DxfCode.ExtendedDataReal, employees[empType]);
                //为实体添加应用程序名为"EMPLOYEE"的扩展数据
                id.AddXData(appName, values);
                trans.Commit();
            }
        }
        public void Initialize()
        {
            //为多行文本类型添加OPM支持，以在属性面板上显示其附加信息
            Masterhe.OPM.OPMManager.Add(typeof(MText), new EmployeeAtt());
            //设置与职位相对应的年薪
            employees.Add("董事长", 500000);
            employees.Add("管理人员", 200000);
            employees.Add("普通员工", 100000);
        }
        public void Terminate()
        {

        }
    }
}
