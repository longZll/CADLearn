using System.Linq;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using DotNetARX;
namespace NetSelection
{
    public class NetSelection
    {
        [CommandMethod("TestGetSelect")]
        public static void TestGetSelect()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            //生成三个同心圆并添加到当前模型空间
            //Circle cir1=new Circle(Point3d.Origin, Vector3d.ZAxis, 10);
            //Circle cir2=new Circle(Point3d.Origin, Vector3d.ZAxis, 20);
            //Circle cir3=new Circle(Point3d.Origin, Vector3d.ZAxis, 30);
            //db.AddToModelSpace(new Circle[] { cir1, cir2, cir3 });

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                // 获取模型空间
                BlockTable bt = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord modelSpace = trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                // 生成三个同心圆并添加到当前模型空间
                Circle cir1 = new Circle(Point3d.Origin, Vector3d.ZAxis, 10);
                Circle cir2 = new Circle(Point3d.Origin, Vector3d.ZAxis, 20);
                Circle cir3 = new Circle(Point3d.Origin, Vector3d.ZAxis, 30);
                //modelSpace.AppendEntity(cir1);
                //modelSpace.AppendEntity(cir2);
                //modelSpace.AppendEntity(cir3);
                //trans.AddNewlyCreatedDBObject(cir1, true);
                //trans.AddNewlyCreatedDBObject(cir2, true);
                //trans.AddNewlyCreatedDBObject(cir3, true);

                db.AddToModelSpace(new Circle[] { cir1, cir2, cir3 });

                trans.Commit(); // 提交事务
            }

            ed.WriteMessage("\n三个同心圆已添加到模型空间。");

            //提示用户选择对象
            PromptSelectionResult psr = ed.GetSelection();
            if (psr.Status != PromptStatus.OK) return;//如果未选择，则返回

            //获取选择集
            SelectionSet ss = psr.Value;
            //信息提示框，给出选择集中包含实体个数的提示
            Application.ShowAlertDialog("选择集中实体的数量：" + ss.Count.ToString());
        }

        [CommandMethod("MergeSelection")]
        public static void MergeSelection()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            //第一次选择
            PromptSelectionResult ss1 = ed.GetSelection();
            if (ss1.Status != PromptStatus.OK) return; //若选择不成功，返回
            Application.ShowAlertDialog("第一个选择集中实体的数量：" + ss1.Value.Count.ToString());
            //第二次选择
            PromptSelectionResult ss2 = ed.GetSelection();
            if (ss2.Status != PromptStatus.OK) return;
            Application.ShowAlertDialog("第二个选择集中实体的数量：" + ss2.Value.Count.ToString());

            //将第二个选择集的ObjectId加入到第一个选择集中
            var ss3 = ss1.Value.GetObjectIds().Union(ss2.Value.GetObjectIds());
            Application.ShowAlertDialog("合并后选择集中实体的数量：" + ss3.Count().ToString());
        }
        /// <summary>
        /// 从第一个选择集中删除第二个选择集
        /// </summary>
        [CommandMethod("DelFromSelection")]
        public static void DelFromSelection()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            //第一次选择
            PromptSelectionResult ss1 = ed.GetSelection();
            if (ss1.Status != PromptStatus.OK) return; //若选择不成功，返回
            Application.ShowAlertDialog("第一个选择集中实体的数量：" + ss1.Value.Count.ToString());
            //第二次选择
            PromptSelectionResult ss2 = ed.GetSelection();
            if (ss2.Status != PromptStatus.OK) return;
            Application.ShowAlertDialog("第二个选择集中实体的数量：" + ss2.Value.Count.ToString());

            //若第二次选择的实体位于第一个选择集中，则删除该实体的ObjectId
            var ss3 = ss1.Value.GetObjectIds().Except(ss2.Value.GetObjectIds());
            Application.ShowAlertDialog("删除第二个选择集后第一个选择集中实体的数量：" + ss3.Count().ToString());
        }
        /// <summary>
        /// 测试先选择后执行
        /// </summary>
        [CommandMethod("TestPickFirst", CommandFlags.UsePickSet)]
        public static void TestPickFirst()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            //获取当前已选择的实体
            PromptSelectionResult psr = ed.SelectImplied();
            //在命令发出前已有实体被选中
            if (psr.Status == PromptStatus.OK)
            {
                SelectionSet ss1 = psr.Value; //获取选择集
                //显示当前已选择的实体个数
                Application.ShowAlertDialog("PickFirst示例：当前已选择的实体个数：" + ss1.Count.ToString());
                //清空当前选择集
                ed.SetImpliedSelection(new ObjectId[0]);
                psr = ed.GetSelection();      //提示用户进行新的选择
                if (psr.Status == PromptStatus.OK)
                {
                    //设置当前已选择的实体
                    ed.SetImpliedSelection(psr.Value.GetObjectIds());
                    SelectionSet ss2 = psr.Value;
                    Application.ShowAlertDialog("PickFirst示例：当前已选择的实体个数：" + ss2.Count.ToString());
                }
            }
            else
            {
                Application.ShowAlertDialog("PickFirst示例：当前已选择的实体个数：0");
            }
        }

        /// <summary>
        /// 多边形作为边界的选择
        /// </summary>
        [CommandMethod("TestPolygonSelect")]
        public static void TestPolygonSelect()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            //声明一个Point3d类列表对象，用于存储多段线的顶点
            Point3dList pts = new Point3dList();
            //提示用户选择多段线
            PromptEntityResult per = ed.GetEntity("请选择多段线");
            if (per.Status != PromptStatus.OK) return;//选择错误，返回

            using (Transaction trans = doc.TransactionManager.StartTransaction())
            {
                //转换为Polyline对象
                Polyline pline = trans.GetObject(per.ObjectId, OpenMode.ForRead) as Polyline;
                if (pline != null)
                {
                    //遍历所选多段线的顶点并添加到Point3d类列表
                    for (int i = 0; i < pline.NumberOfVertices; i++)
                    {
                        Point3d point = pline.GetPoint3dAt(i);
                        pts.Add(point);
                    }
                    //窗口选择，仅选择完全位于多边形区域中的对象
                    PromptSelectionResult psr = ed.SelectWindowPolygon(pts);
                    if (psr.Status == PromptStatus.OK)
                    {
                        Application.ShowAlertDialog("选择集中实体的数量：" + psr.Value.Count.ToString());
                    }
                }
                trans.Commit();
            }
        }

        [CommandMethod("TestSelectException")]
        public static void TestSelectException()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            Point3d pt1 = Point3d.Origin;
            Point3d pt2 = new Point3d(100, 100, 0);
            //交叉窗口选择，选择由pt1和pt2组成的矩形窗口包围的或相交的对象
            PromptSelectionResult psr = ed.SelectCrossingWindow(pt1, pt2);
            //注意:SelectCrossingWindow/SelectCrossingPolygon/SelectWindow/SelectWindowPolygon方法同样存在这个问题--仅能获得图形视口内部的实体,不能获得视口外部的实体
            if (psr.Status == PromptStatus.OK)
            {
                Application.ShowAlertDialog("选择集中实体的数量：" + psr.Value.Count.ToString());
            }
        }
        /// <summary>
        /// 获得"图层1"上面的 直线类型 实体的数量
        /// </summary>
        [CommandMethod("TestFilter")]
        public static void TestFilter()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            //创建一个自定义的TypedValue列表对象，用于构建过滤器列表
            TypedValueList values = new TypedValueList();
            //选择图层1上的直线对象
            values.Add(DxfCode.LayerName, "图层1");
            values.Add(typeof(Line));

            //构建过滤器列表，注意这里使用自定义类型转换
            SelectionFilter filter = new SelectionFilter(values);
            //选择图形中所有满足过滤器的对象，即位于图层1上的直线

            PromptSelectionResult psr = ed.SelectAll(filter);
            if (psr.Status == PromptStatus.OK)
            {
                Application.ShowAlertDialog("选择集中实体的数量：" + psr.Value.Count.ToString());
            }
        }

        /// <summary>
        /// 构建过滤器，选择图形中圆心在pt1和pt2两点构成的矩形范围内的 所有圆
        /// </summary>
        [CommandMethod("TestFilter2")]
        public static void TestFilter2()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            Point3d pt1 = new Point3d(0, 0, 0);
            Point3d pt2 = new Point3d(100, 100, 0);
            TypedValueList values = new TypedValueList();
            values.Add(typeof(Circle));
            values.Add(DxfCode.Operator, ">,>,*");
            values.Add(DxfCode.XCoordinate, pt1);
            values.Add(DxfCode.Operator, "<,<,*");
            values.Add(DxfCode.XCoordinate, pt2);
            SelectionFilter filter = new SelectionFilter(values);

            ////创建一个自定义的TypedValue列表对象，用于构建过滤器列表
            //TypedValueList values = new TypedValueList();
            ////选择图层1上的直线对象
            //values.Add(DxfCode.LayerName, "图层1");
            //values.Add(typeof(Line));

            ////构建过滤器列表，注意这里使用自定义类型转换
            //SelectionFilter filter = new SelectionFilter(values);
            ////选择图形中所有满足过滤器的对象，即位于图层1上的直线

            PromptSelectionResult psr = ed.SelectAll(filter);
            if (psr.Status == PromptStatus.OK)
            {
                Application.ShowAlertDialog("选择集中实体的数量：" + psr.Value.Count.ToString());
            }


            if (psr.Status == PromptStatus.OK)
            {
                using (Transaction trans = doc.TransactionManager.StartTransaction())
                {
                    BlockTableRecord currentSpace = trans.GetObject(doc.Database.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;

                    foreach (SelectedObject selectedObject in psr.Value)
                    {
                        if (selectedObject != null)
                        {
                            Entity ent = trans.GetObject(selectedObject.ObjectId, OpenMode.ForRead) as Entity;
                            Circle circle = ent as Circle;
                            if (circle != null)
                            {
                                // 创建代替圆的多段线
                                Polyline pline = new Polyline();
                                pline.AddVertexAt(0, new Point2d(circle.Center.X + circle.Radius, circle.Center.Y), 0, 0, 0);
                                pline.AddVertexAt(1, new Point2d(circle.Center.X, circle.Center.Y + circle.Radius), 0, 0, 0);
                                pline.AddVertexAt(2, new Point2d(circle.Center.X - circle.Radius, circle.Center.Y), 0, 0, 0);
                                pline.AddVertexAt(3, new Point2d(circle.Center.X, circle.Center.Y - circle.Radius), 0, 0, 0);
                                pline.Closed = true;

                                // 设置多段线的属性
                                pline.ConstantWidth = 5;
                                pline.Color = Color.FromColorIndex(ColorMethod.ByAci, 1); // 设置为红色

                                // 添加多段线到模型空间
                                currentSpace.AppendEntity(pline);
                                trans.AddNewlyCreatedDBObject(pline, true);
                            }
                        }
                    }

                    trans.Commit();
                }


            }


        }

        /// <summary>
        /// 构建过滤器，选择用户框选范围内的 所有圆
        /// </summary>
        [CommandMethod("TestFilter3")]
        public static void TestFilter3()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            // 提示用户框选范围
            PromptSelectionResult psr = ed.GetSelection();
            if (psr.Status != PromptStatus.OK)
            {
                ed.WriteMessage("\n未选择有效区域。");
                return;
            }

            // 获取用户框选的范围
            SelectionSet ss = psr.Value;

            // 构建过滤器
            TypedValueList values = new TypedValueList();
            values.Add(typeof(Circle));
            foreach (SelectedObject selectedObject in ss)
            {
                values.Add(DxfCode.Start, selectedObject.ObjectId);
            }
            SelectionFilter filter = new SelectionFilter(values);


            //PromptSelectionResult psr = ed.SelectAll(filter);
            if (psr.Status == PromptStatus.OK)
            {
                Application.ShowAlertDialog("选择集中实体的数量：" + psr.Value.Count.ToString());

                using (Transaction trans = doc.TransactionManager.StartTransaction())
                {
                    BlockTableRecord currentSpace = trans.GetObject(doc.Database.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;

                    foreach (SelectedObject selectedObject in ss)
                    {
                        if (selectedObject != null)
                        {
                            Entity ent = trans.GetObject(selectedObject.ObjectId, OpenMode.ForRead) as Entity;
                            Circle circle = ent as Circle;
                            if (circle != null)
                            {
                                // 创建代替圆的多段线
                                Polyline pline = new Polyline();
                                pline.AddVertexAt(0, new Point2d(circle.Center.X + circle.Radius, circle.Center.Y), 0, 0, 0);
                                pline.AddVertexAt(1, new Point2d(circle.Center.X, circle.Center.Y + circle.Radius), 0, 0, 0);
                                pline.AddVertexAt(2, new Point2d(circle.Center.X - circle.Radius, circle.Center.Y), 0, 0, 0);
                                pline.AddVertexAt(3, new Point2d(circle.Center.X, circle.Center.Y - circle.Radius), 0, 0, 0);
                                pline.Closed = true;

                                // 设置多段线的属性
                                pline.ConstantWidth = 5;        //设置多段线的线宽为5
                                pline.Color = Color.FromColorIndex(ColorMethod.ByAci, 1); // 设置为红色

                                // 添加多段线到模型空间
                                currentSpace.AppendEntity(pline);
                                trans.AddNewlyCreatedDBObject(pline, true);
                            }
                        }
                    }

                    trans.Commit();
                }

                ed.WriteMessage("\n已将选择范围内的圆转换为多段线并设置属性。");


            }


        }

    }
}
