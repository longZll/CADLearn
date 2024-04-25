using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using DotNetARX;
namespace ObjectRule
{
    public class ObjectRule : ObjectOverrule
    {
        public override void Erase(DBObject dbObject, bool erasing)
        {
            base.Erase(dbObject, erasing);//调用基类函数删除对象
            Editor ed=Application.DocumentManager.MdiActiveDocument.Editor;
            ed.WriteMessage("\n不能删除门");//提示用户不能删除重定义的门
            //抛出异常，阻止门被删除
            throw new Autodesk.AutoCAD.Runtime.Exception(ErrorStatus.NotApplicable);
        }

        public override DBObject DeepClone(DBObject dbObject, DBObject ownerObject, IdMapping idMap, bool isPrimary)
        {
            //调用基类函数复制对象
            DBObject obj=base.DeepClone(dbObject, ownerObject, idMap, isPrimary);
            Database db=obj.Database;
            Document doc=db.GetDocument();
            Editor ed=doc.Editor;
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                string doorType="木门";//门类型
                //提示用户输入表示门类型的关键字
                PromptKeywordOptions opt=new PromptKeywordOptions("\n请输入门的类型[木门(M)/铁门(T)/铝合金门(L)]<M>");
                opt.Keywords.Add("M", "M", "M", false, true);
                opt.Keywords.Add("T", "T", "T", false, true);
                opt.Keywords.Add("L", "L", "L", false, true);
                opt.Keywords.Default = "M";//缺省关键字
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
                TypedValueList values=new TypedValueList();
                values.Add(DxfCode.ExtendedDataAsciiString, doorType);//门类型
                values.Add(DxfCode.ExtendedDataReal, Commands.doors[doorType]);//门价格
                obj.ObjectId.AddXData(Commands.appName, values);//添加扩展数据
                trans.Commit();
            }
            return obj;
        }
    }
}
