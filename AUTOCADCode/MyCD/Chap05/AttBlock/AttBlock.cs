using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using DotNetARX;


namespace AttBlock
{
    public class AttBlock
    {
        public ObjectId MakeDoor()
        {
            ObjectId blockId;
            Database db=HostApplicationServices.WorkingDatabase;

            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                //设置门框的左边线
                Point3d pt1=Point3d.Origin;
                Point3d pt2=new Point3d(0, 1.0, 0);
                Line leftLine=new Line(pt1, pt2);

                //门框的下边线
                Line bottomLine=new Line(pt1, pt1.PolarPoint(0, 0.05));

                //设置表示门面的圆弧
                Arc arc=new Arc();
                arc.CreateArc(pt1.PolarPoint(0, 1), pt1, Math.PI / 2);
                //设置门框的右边线
                Line rightLine=new Line(bottomLine.EndPoint, leftLine.EndPoint.PolarPoint(0, 0.05));
                
                Point3dCollection pts=new Point3dCollection();
                
                //获取直线与arc圆弧的交点,并且存储在pts中
                //rightLine.IntersectWith(arc, Intersect.OnBothOperands, pts, 0, 0);

                rightLine.IntersectWith(arc, Intersect.OnBothOperands, pts, IntPtr.Zero, IntPtr.Zero);

                if (pts.Count == 0) return ObjectId.Null;

                rightLine.EndPoint = pts[0];
                

                //将表示门的直线和圆弧合成为块,并设置块名为"DOOR",最后添加该图块到图形数据库中
                blockId = db.AddBlockTableRecord("DOOR", leftLine, bottomLine, rightLine, arc);

                trans.Commit();
            }
            return blockId;
        }

        /// <summary>
        /// 制作门图块,为门的添加属性定义
        /// </summary>
        [CommandMethod("AddDoorAtt")]
        public void AddDoorAtt()
        {
            Database db=HostApplicationServices.WorkingDatabase;

            //调用上面的制作门的方法
            ObjectId blockId=MakeDoor();    //创建表示门的DOOR块定义

            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                //表示门符号的属性定义
                AttributeDefinition attSYM=new AttributeDefinition(Point3d.Origin, "1", "SYM", "输入门的符号", ObjectId.Null);
                
                //设置属性定义的通用样式
                SetStyleForAtt(attSYM, false);
                //设置属性的对齐点
                attSYM.AlignmentPoint = new Point3d(0.32, 0.28, 0.0);
                
                //表示门宽度的属性定义
                AttributeDefinition attWidth=new AttributeDefinition(Point3d.Origin, "1m", "WIDTH", "输入门的宽度", ObjectId.Null);
                SetStyleForAtt(attWidth, true);
                
                //表示门高度的属性定义
                AttributeDefinition attHeight=new AttributeDefinition(Point3d.Origin, "2m", "HEIGHT", "输入门的高度", ObjectId.Null);
                SetStyleForAtt(attHeight, true);
                
                //表示门宽式样的属性定义
                AttributeDefinition attStyle=new AttributeDefinition(Point3d.Origin, "TWO PANEL", "STYLE", "输入门的式样", ObjectId.Null);
                SetStyleForAtt(attStyle, true);
                
                //表示门参考图集编号的属性定义
                AttributeDefinition attRef=new AttributeDefinition(Point3d.Origin, "TS 3010", "REF", "输入门的参考图集编号", ObjectId.Null);
                SetStyleForAtt(attRef, true);
                
                //表示生产产家的属性定义
                AttributeDefinition attManufacturer=new AttributeDefinition(Point3d.Origin, "TRU STYLE", "MANUFACTURER", "输入生产产家", ObjectId.Null);
                SetStyleForAtt(attManufacturer, true);
                
                //表示门单价的属性定义
                AttributeDefinition attCost=new AttributeDefinition(Point3d.Origin, "189.00", "COST", "输入门的单价", ObjectId.Null);
                SetStyleForAtt(attCost, true);
                
                //为DOOR图块添加上面定义的属性
                blockId.AddAttsToBlock(attSYM, attWidth, attHeight, attStyle, attRef, attManufacturer, attCost);

                trans.Commit();
            }
        }

        private void SetStyleForAtt(AttributeDefinition att, bool invisible)
        {
            att.Height = 0.15;  //属性文字的高度

            //属性文字的水平对齐方式为居中
            att.HorizontalMode = TextHorizontalMode.TextCenter;

            //属性文字的垂直对齐方式为居中
            att.VerticalMode = TextVerticalMode.TextVerticalMid;

            att.Invisible = invisible; //属性文字的可见性
        }


        /// <summary>
        /// 插入 门属性块
        /// </summary>
        [CommandMethod("InsertDoor")]
        public void InsertDoor()
        {
            Database db=HostApplicationServices.WorkingDatabase;
            ObjectId spaceId=db.CurrentSpaceId;//获取当前空间（模型空间或图纸空间）
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                //表示属性的字典对象
                Dictionary<string,string> atts=new Dictionary<string, string>();

                atts.Add("SYM", "1");       //门的符号
                atts.Add("WIDTH", "0.90m"); //门的宽度
                atts.Add("HEIGHT", "2.2m"); //门的高度
                atts.Add("COST", "200.0");  //门的单价
                
                //在当前空间0图形中 加入一个表示门的带属性块参照
                spaceId.InsertBlockReference("0", "DOOR", Point3d.Origin, new Scale3d(3), 0, atts);
                
                trans.Commit();
            }
        }

        /// <summary>
        /// 更新的图块门的相关属性
        /// </summary>
        [CommandMethod("UpdateDoorAtt")]
        public void UpdateDoorAtt()
        {
            Database db=HostApplicationServices.WorkingDatabase;
            Editor ed=Application.DocumentManager.MdiActiveDocument.Editor;
            
            //提示用户选择要更新的块
            PromptEntityOptions opt=new PromptEntityOptions("请选择一个块参照");

            opt.SetRejectMessage("你选择的不是块");

            opt.AddAllowedClass(typeof(BlockReference), true);

            PromptEntityResult result=ed.GetEntity(opt);

            if (result.Status != PromptStatus.OK) return;

            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                //要更新的门属性
                Dictionary<string,string> atts=new Dictionary<string, string>();

                atts.Add("SYM", "2");
                atts.Add("COST", "500");
                atts.Add("WIDTH","1.0");
                atts.Add("HEIGHT", "2.5");

                //更新门的属性
                result.ObjectId.UpdateAttributesInBlock(atts);

                trans.Commit();
            }
        }
    }
}
