using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.GraphicsInterface;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using DotNetARX;

namespace HighlightBlock
{
    using Autodesk.AutoCAD.Runtime;
    using Autodesk.AutoCAD.ApplicationServices;
    using Autodesk.AutoCAD.DatabaseServices;
    using Autodesk.AutoCAD.Geometry;

    namespace HighlightOverruleSample
    {
        public class EmployeeOverrule : HighlightOverrule
        {
            private int _colorIndex = 1;
            private int _oldColorIndex;
            public static EmployeeOverrule theOverrule = new EmployeeOverrule();
            const string regAppName="EMPLOYEE";
            public EmployeeOverrule()
            {
                SetXDataFilter(regAppName);
            }
            public static string GetXData(ObjectId id)
            {
                string position=string.Empty;
                TypedValueList xdata= id.GetXData(regAppName);
                if (xdata != null)
                {
                    position = xdata[2].Value.ToString();
                }
                return position;
            }
            public int ColorIndex
            {
                set { _colorIndex = value; }
                get { return _colorIndex; }
            }

            public override void Highlight(
                Entity entity, FullSubentityPath subId, bool highlightAll)
            {
                Database db= entity.Database;
                Document doc=Application.DocumentManager.MdiActiveDocument;
                Editor ed=doc.Editor;
                string position=GetXData(entity.ObjectId);
                MText txt = entity as MText;
                if (txt == null || position.IsNullOrWhiteSpace()) return;                
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    txt.UpgradeOpen();
                    _oldColorIndex = txt.ColorIndex;
                    txt.ColorIndex = _colorIndex;
                    //trans.Commit();
                }
                base.Highlight(entity, subId, highlightAll);                
            }

            public override void Unhighlight(
                Entity entity, FullSubentityPath subId, bool highlightAll)
            {
                string position=GetXData(entity.ObjectId);
                MText txt = entity as MText;
                if (txt == null && position.IsNullOrWhiteSpace()) return;
                Database db= entity.Database;
                Document doc=Application.DocumentManager.MdiActiveDocument;
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    txt.UpgradeOpen();
                    txt.ColorIndex = _oldColorIndex;
                    //trans.Commit();
                }
                base.Unhighlight(entity, subId, highlightAll);
            }
        }
        public class Command
        {
            [CommandMethod("Overrule1")]
            public void StartOverrule()
            {
                ObjectOverrule.AddOverrule(RXClass.GetClass(typeof(MText)), EmployeeOverrule.theOverrule, true);
            }
            [CommandMethod("Overrule0")]
            public void EndOverrule()
            {
                ObjectOverrule.RemoveOverrule(RXClass.GetClass(typeof(MText)), EmployeeOverrule.theOverrule);
                Document doc = Application.DocumentManager.MdiActiveDocument;
                doc.SendStringToExecute("REGEN3\n", true, false, false);
                doc.Editor.Regen();
            }
            [CommandMethod("Employee", CommandFlags.UsePickSet)]
            public void SetHighlightColor()
            {
                Document dwg = Autodesk.AutoCAD.ApplicationServices.
                    Application.DocumentManager.MdiActiveDocument;

                Editor ed = dwg.Editor;

                PromptIntegerOptions opt = new PromptIntegerOptions(
                    "\nEnter color index (a number form 0 to 7):");

                PromptIntegerResult res = ed.GetInteger(opt);

                if (res.Status == PromptStatus.OK)
                {
                    EmployeeOverrule.theOverrule.ColorIndex = res.Value;
                }
            }
            [CommandMethod("AddX")]
            public static void AddX()
            {
                Document doc=Application.DocumentManager.MdiActiveDocument;
                Database db=doc.Database;
                Editor ed=doc.Editor;
                //提示用户选择一个表示董事长的多行文本
                PromptEntityOptions opt=new PromptEntityOptions("\n请选择表示董事长的多行文本");
                opt.SetRejectMessage("\n您选择的不是多行文本，请重新选择");
                opt.AddAllowedClass(typeof(MText), true);
                PromptEntityResult entResult=ed.GetEntity(opt);
                if (entResult.Status != PromptStatus.OK) return;
                ObjectId id=entResult.ObjectId;//用户选择的多行文本的ObjectId
                using (Transaction trans=db.TransactionManager.StartTransaction())
                {
                    TypedValueList values=new TypedValueList();//定义一个TypedValue列表
                    //添加整型（表示员工编号）和字符串（表示职位）扩展数据项
                    values.Add(DxfCode.ExtendedDataInteger32, 1002);
                    values.Add(DxfCode.ExtendedDataAsciiString, "董事长");
                    //为实体添加应用程序名为"EMPLOYEE"的扩展数据
                    id.AddXData("EMPLOYEE", values);
                    trans.Commit();
                }
            }
        }
        //public class MyHighlightOverrule : HighlightOverrule
        //{
        //    private int _colorIndex = 1;
        //    private int _oldColorIndex;

        //    public MyHighlightOverrule()
        //    {
        //        AddOverrule(RXClass.GetClass(typeof(MText)), this, true);
        //    }

        //    public int ColorIndex
        //    {
        //        set { _colorIndex = value; }
        //        get { return _colorIndex; }
        //    }

        //    public override void Highlight(
        //        Entity entity, FullSubentityPath subId, bool highlightAll)
        //    {
        //        Polyline pline = entity as Polyline;
        //        if (pline == null) return;

        //        Database db= entity.Database;
        //        Document dwg=Application.DocumentManager.MdiActiveDocument;

        //        using (DocumentLock dl = dwg.LockDocument())
        //        {
        //            using (Transaction tran = db.TransactionManager.StartTransaction())
        //            {
        //                pline.UpgradeOpen();
        //                _oldColorIndex = pline.ColorIndex;
        //                pline.ColorIndex = _colorIndex;
        //                pline.DowngradeOpen();
        //            }
        //        }

        //        base.Highlight(entity, subId, highlightAll);
        //    }

        //    public override void Unhighlight(
        //        Entity entity, FullSubentityPath subId, bool highlightAll)
        //    {
        //        Polyline pline = entity as Polyline;
        //        if (pline == null) return;

        //        Database db = entity.Database;
        //        Document dwg=Application.DocumentManager.MdiActiveDocument;

        //        using (DocumentLock dl = dwg.LockDocument())
        //        {
        //            using (Transaction tran = db.TransactionManager.StartTransaction())
        //            {
        //                pline.UpgradeOpen();
        //                pline.ColorIndex = _oldColorIndex;
        //                pline.DowngradeOpen();
        //            }
        //        }

        //        base.Unhighlight(entity, subId, highlightAll);
        //    }
        //}
    }

}
