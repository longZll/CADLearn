using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using DotNetARX;
namespace XRefs
{
    public class XRefs
    {
        [CommandMethod("AttachExternalReference")]
        public void AttachExternalReference()
        {
            Database db=HostApplicationServices.WorkingDatabase;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            //获取文档管理器对象以打开Dwg文件
            DocumentCollection docs = Application.DocumentManager;
            //设置打开文件对话框的有关选项
            PromptOpenFileOptions opt = new PromptOpenFileOptions("\n请输入文件名：");
            opt.Filter = "图形(*.dwg)|*.dwg|图形(*.dxf)|*.dxf";
            opt.FilterIndex = 0;
            //根据打开文件对话框中用户的选择，获取文件名
            string filename = ed.GetFileNameForOpen(opt).StringResult;
            Point3d pt=Point3d.Origin;//外部参照的插入点
            //提示用户输入外部参照的类型，“A”为附着型，“O”为覆盖型
            PromptKeywordOptions keyOpt=new PromptKeywordOptions("\n请输入外部参照的类型[附着(A)/覆盖(O)]<A>");
            keyOpt.Keywords.Add("A", "A", "A", false, true);
            keyOpt.Keywords.Add("O", "O", "O", false, true);
            keyOpt.Keywords.Default = "A";//缺省为附着型
            PromptResult keyResult=ed.GetKeywords(keyOpt);
            bool isOverlay=keyResult.StringResult == "A" ? false : true;
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                //将外部参照插入到当前图形并在模型空间添加参照块
                ObjectId xrefId = db.AttachXref(filename, System.IO.Path.GetFileNameWithoutExtension(filename), pt, new Scale3d(1, 1, 1), 0, isOverlay);
                trans.Commit();
            }
        }
        [CommandMethod("UnloadExternalReference")]
        public void UnloadExternalReference()
        {
            Database db=HostApplicationServices.WorkingDatabase;
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                BlockTable bt=(BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
                foreach (ObjectId id in bt)//遍历块表
                {
                    BlockTableRecord btr=(BlockTableRecord)trans.GetObject(id, OpenMode.ForRead);
                    //如果是名为“Annotation - Metric”的外部参照，则进行卸载
                    if (btr.IsFromExternalReference && btr.Name == "Annotation - Metric")
                    {
                        db.UnloadXrefs(new ObjectIdList(id));
                        break;//完成卸载指定的外部参照，跳出循环
                    }
                }
                trans.Commit();
            }
        }

        [CommandMethod("ListXrefs")]
        public static void ListXrefs()
        {
            Database db=HostApplicationServices.WorkingDatabase;
            //获取XrefGraph对象，该对象包含当前文档中的所有外部参照节点
            XrefGraph xgraph=db.GetHostDwgXrefGraph(true);
            //创建一个节点集合对象，用来保存当前XrefGraph中的节点
            using (var nodes=new GraphNodeCollection())
            {
                //遍历XrefGraph中的节点，并将其添加到节点集合中
                for (int i = 0; i < xgraph.NumNodes; i++)
                    nodes.Add(xgraph.GetXrefNode(i));
                //第一个为当前文档本身，不属于外部参照，跳过
                for (int i = 1; i < xgraph.NumNodes; i++)
                {
                    //获取外部参照节点对象
                    XrefGraphNode node=xgraph.GetXrefNode(i);
                    //如果为嵌套节点，则访问其子节点
                    if (!node.IsNested)
                        getNestedNodes(xgraph, node, nodes, string.Empty);
                }
            }
        }
        private static void getNestedNodes(XrefGraph xgraph, XrefGraphNode node, GraphNodeCollection nodes, string parent)
        {
            Document doc=Application.DocumentManager.MdiActiveDocument;
            //输出外部参照节点的名称（含其父节点名称）和状态
            doc.Editor.WriteMessage("\n{0}({1}）", parent + node.Name, node.XrefStatus);
            //递归调用，以完成对嵌套节点的访问
            for (int i = 0; i < node.NumOut; i++)
            {
                getNestedNodes(xgraph, xgraph.GetXrefNode(nodes.IndexOf(node.Out(i))), nodes, parent + node.Name + "|");
            }
        }
        [CommandMethod("AttachImage")]
        public void AttachImage()
        {
            Database db=HostApplicationServices.WorkingDatabase;
            Point3d pt=Point3d.Origin;//图像插入点
            //图像文件路径
            string fileName=Tools.GetCurrentPath() + "\\child.jpg";
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                //访问当前文档的图像目录Id
                ObjectId dictId=RasterImageDef.GetImageDictionary(db);
                //如果图像目录不存在，则新建
                if (dictId.IsNull) dictId = RasterImageDef.CreateImageDictionary(db);
                //获取图像目录，它是一个字典对象
                DBDictionary dict=(DBDictionary)trans.GetObject(dictId, OpenMode.ForRead);
                RasterImageDef def=new RasterImageDef();//新建一个光栅图像定义对象
                def.SourceFileName = fileName;//光栅图像的文件名
                def.Load();//装载光栅图像定义对象
                dict.UpgradeOpen();//切换图像目录对象为写的状态
                //将光栅图像定义添加到图像目录
                ObjectId defId=dict.SetAt("child", def);
                trans.AddNewlyCreatedDBObject(def, true);//通知事务处理
                RasterImage image=new RasterImage();//新建光栅图像
                image.ImageDefId = defId;//设置光栅图像的定义
                image.ShowImage = true;//显示光栅图像
                //将光栅图像放置在原点，大小为图像的原尺寸
                image.Orientation = new CoordinateSystem3d(
                    Point3d.Origin, new Vector3d(image.Width, 0, 0), new Vector3d(0, image.Height, 0));
                db.AddToModelSpace(image);//将光栅图像添加到模型空间
                //在光栅图像和光栅图像定义之间创建联系，防止在外部参照管理器中图像的状态为“未参照”
                RasterImage.EnableReactors(true);
                image.AssociateRasterDef(def);
                trans.Commit();//提交事务处理
            }
        }
    }
}
