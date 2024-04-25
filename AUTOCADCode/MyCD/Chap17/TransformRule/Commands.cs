using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using DotNetARX;

namespace TransformRule
{
    public class Commands : IExtensionApplication
    {
        private static TransformRule theOverrule;//变形重定义对象
        [CommandMethod("MovePoint")]
        public void MovePoint()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            //提示用户选择一条曲线，点会限制在其上移动
            PromptEntityOptions opt=new PromptEntityOptions("请选择曲线");
            opt.SetRejectMessage("必须是曲线");
            opt.AddAllowedClass(typeof(Curve), false);
            PromptEntityResult result=ed.GetEntity(opt);
            if (result.Status != PromptStatus.OK) return;
            //获取曲线的ObjectId并传递给变形重定义，以实现点沿曲线移动
            TransformRule.curveId = result.ObjectId;
            db.Pdmode = 35;//设置点的样式
            db.Pdsize = -10;//设置点的大小
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                Curve curve=trans.GetObject(TransformRule.curveId, OpenMode.ForRead) as Curve;
                if (curve != null)
                {
                    //获取曲线上离选取点最近的一个点
                    Point3d position=curve.GetClosestPointTo(result.PickedPoint, false);
                    //将几何点转化为实体点并添加到模型空间
                    DBPoint pt=new DBPoint(position);
                    db.AddToModelSpace(pt);
                }
                trans.Commit();
            }
        }
        public void Initialize()
        {
            if (theOverrule == null)
            {
                theOverrule = new TransformRule();
                //为点添加变形重定义
                Overrule.AddOverrule(RXObject.GetClass(typeof(DBPoint)), theOverrule, false);
            }
            Overrule.Overruling = true;//开启规则重定义
            //刷新屏幕
            Application.DocumentManager.MdiActiveDocument.Editor.Regen();
        }
        public void Terminate()
        {

        }
    }
}
