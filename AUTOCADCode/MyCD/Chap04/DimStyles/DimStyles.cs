using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using DotNetARX;

namespace DimStyles
{
    public class DimStyles
    {
        /// <summary>
        /// 创建一个新的标注样式,并且将该标注样式设置为当前标注样式
        /// </summary>
        [CommandMethod("NewDim")]
        public void NewDim()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            string dimName = "";//存储标注样式名
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                while (dimName == "")//如果用户没有输入标注样式名，则循环
                {
                    //提示用户输入标注样式名
                    PromptResult pr = ed.GetString("请输入标注样式名称");
                    if (pr.Status != PromptStatus.OK) return;
                    try
                    {
                        //验证输入的标注样式名是否合法
                        SymbolUtilityServices.ValidateSymbolName(pr.StringResult, false);
                        dimName = pr.StringResult;//标注样式名
                        //添加名为dimName的标注样式记录
                        ObjectId dimId = db.AddDimStyle(dimName);
                        //以写的方式打开新建的标注样式记录
                        DimStyleTableRecord dstr = (DimStyleTableRecord)trans.GetObject(dimId, OpenMode.ForWrite);
                        dstr.Dimasz = 3;//设置箭头大小
                        dstr.Dimexe = 3;//超出尺寸线的大小
                        dstr.Dimtad = 1;//文字位于标注线的上方
                        dstr.Dimtxt = 3;//标注文字的高度
                        //设置新添加的标注样式为当前标注样式
                        db.Dimstyle = dimId;

                        //为防止出现替代样式的问题，请添加下面的语句
                        db.SetDimstyleData(dstr);

                        break;//添加标注样式成功，跳出while循环
                    }
                    catch
                    {
                        ed.WriteMessage("标注样式名称不合法\n,请输入合理的标注样式名称");
                    }
                }
                trans.Commit();
            }
        }


        /// <summary>
        /// 通过标注对象、已有的标注样式及数据库的标注样式来创建新的标注样式
        /// </summary>
        [CommandMethod("CopyDim")]
        public void CopyDim()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            //提示用户选择标注对象
            PromptEntityOptions opt = new PromptEntityOptions("请选择标注对象");
            opt.SetRejectMessage("你选择的不是标注对象");
            opt.AddAllowedClass(typeof(Dimension), false);

            PromptEntityResult result = ed.GetEntity(opt);
            if (result.Status != PromptStatus.OK) return;

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                //获取标注对象
                Dimension dim = (Dimension)trans.GetObject(result.ObjectId, OpenMode.ForRead);
                //新建标注样式表记录
                DimStyleTableRecord dstr1 = new DimStyleTableRecord();
                //复制标注对象的标注样式表记录
                dstr1.CopyFrom(dim.GetDimstyleData());
                //设置标注样式表记录名称
                dstr1.Name = "标注对象样式";

                DimStyleTableRecord dstr2 = new DimStyleTableRecord();
                //复制当前的标注样式表记录
                dstr2.CopyFrom(db.GetDimstyleData());
                dstr2.Name = "数据库样式";

                DimStyleTableRecord dstr3 = new DimStyleTableRecord();
                //复制已有的标注样式表记录
                dstr3.CopyFrom(dstr2);
                dstr3.Name = "复制样式";

                //打开标注样式表并添加新建的标注样式表记录
                DimStyleTable dst = (DimStyleTable)trans.GetObject(db.DimStyleTableId, OpenMode.ForWrite);
                dst.Add(dstr1);
                dst.Add(dstr2);
                dst.Add(dstr3);
                trans.AddNewlyCreatedDBObject(dstr1, true);
                trans.AddNewlyCreatedDBObject(dstr2, true);
                trans.AddNewlyCreatedDBObject(dstr3, true);
                trans.Commit();
            }
        }
    }
}
