using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using DotNetARX;

namespace HighlightRule
{
    public class HightLightRule : HighlightOverrule
    {
        public static string GetDoorType(Entity entity)
        {
            string doorType=string.Empty;
            TypedValueList xdata=entity.GetXDataForApplication(Commands.appName);
            if (xdata != null)
            {
                doorType = xdata[1].Value.ToString();
            }
            return doorType;
        }
        public override void Highlight(Entity entity, FullSubentityPath subId, bool highlightAll)
        {
            //获取门的类型
            string doorType=GetDoorType(entity);
            Line line = entity as Line;
            if (line == null || doorType.IsNullOrWhiteSpace()) return;
            Database db= entity.Database;
            Document doc=db.GetDocument();
            //开启事务处理，注意锁定文档
            using (OpenCloseTransaction tran = db.TransactionManager.StartOpenCloseTransaction())
            using (DocumentLock loc=doc.LockDocument())
            {
                line.UpgradeOpen();
                //根据门的类型设置颜色
                switch (doorType)
                {
                    case "木门":
                        line.ColorIndex = 1;//红色
                        break;
                    case "铁门":
                        line.ColorIndex = 2;//黄色
                        break;
                    case "铝合金门":
                        line.ColorIndex = 3;//蓝色
                        break;
                }
                line.DowngradeOpen();
            }
            base.Highlight(entity, subId, highlightAll);
        }
        public override void Unhighlight(Entity entity, FullSubentityPath subId, bool highlightAll)
        {
            Database db= entity.Database;
            Document doc=db.GetDocument();
            string doorType=GetDoorType(entity);
            Line line = entity as Line;
            if (line == null || doorType.IsNullOrWhiteSpace()) return;
            using (OpenCloseTransaction tran = db.TransactionManager.StartOpenCloseTransaction())
            using (DocumentLock loc=doc.LockDocument())
            {
                line.UpgradeOpen();
                line.ColorIndex = 7;//还原直线的颜色为黑色
                line.DowngradeOpen();
            }
            base.Unhighlight(entity, subId, highlightAll);
        }
    }
}
