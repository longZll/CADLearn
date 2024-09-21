using System;
using System.Collections.Generic;
//using System.Linq;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System.Windows.Forms;
using Autodesk.AutoCAD.Internal;
using DotNetARX;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;

public class SmallestEnclosingCircleCommand
{

    // 定义二维点的类（AutoCAD中的点使用 Point3d）
    public class Point
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// 计算当前点到另一个点的距离
        /// </summary>
        /// <param name="otherPoint"></param>
        /// <returns></returns>
        public double DistanceTo(Point otherPoint)
        {
            //使用勾股定理计算两点之间的距离
            return Math.Sqrt(Math.Pow(this.X - otherPoint.X, 2) + Math.Pow(this.Y - otherPoint.Y, 2));
        }

    }

    // 定义圆的类
    public class CircleInfo
    {
        public Point Center { get; set; }
        public double Radius { get; set; }

        public CircleInfo(Point center, double radius)
        {
            Center = center;
            Radius = radius;
        }

        /// <summary>
        /// 判断点p是否在该圆内或者圆边线上,如果在返回真,不在返回假
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool Contains(Point p)
        {
            return Math.Pow(p.X - Center.X, 2) + Math.Pow(p.Y - Center.Y, 2) <= Math.Pow(Radius, 2);
        }
    }

    /// <summary>
    /// 最终的包围圆,最终的包围圆应该包含所有的点
    /// </summary>
    public class ResCircleInfo
    {
        public Point Center { get; set; }
        public double Radius { get; set; }

        public ResCircleInfo(Point center, double radius)
        {
            Center = center;
            Radius = radius;
        }

        /// <summary>
        /// 判断点p是否在该圆内或者圆边线上,如果在返回真,不在返回假
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool Contains(Point p)
        {
            return Math.Pow(p.X - Center.X, 2) + Math.Pow(p.Y - Center.Y, 2) <= Math.Pow(Radius, 2);
        }
    }



    /// <summary>
    /// 创建最小包围圆
    /// </summary>
    [CommandMethod("CreatSmallestEnclosingCircle")]
    public void CreatSmallestEnclosingCircle()
    {
        Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
        Database db = doc.Database;
        Editor ed = doc.Editor;

        try
        {
            // 提示用户选择点
            PromptSelectionOptions opts = new PromptSelectionOptions
            {
                MessageForAdding = "\n 请框选多条直线段以生成最小包围圆: "
            };
            PromptSelectionResult res = ed.GetSelection(opts);

            //如果用户选择了多条线段
            if (res.Status == PromptStatus.OK)
            {
                //提取选择的点
                SelectionSet selSet = res.Value;

                List<Point> points = new List<Point>();

                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    foreach (SelectedObject selObj in selSet)
                    {
                        // 获取实体
                        Entity ent = tr.GetObject(selObj.ObjectId, OpenMode.ForRead) as Entity;

                        //只处理直线的端点
                        if (ent is Line line)
                        {
                            //提取直线的两个端点
                            Point3d startPoint = line.StartPoint;
                            Point3d endPoint = line.EndPoint;

                            //刚开始的时候添加第一条直线的两个端点进来
                            if (points.Count == 0)
                            {
                                points.Add(new Point(startPoint.X, startPoint.Y));
                                points.Add(new Point(endPoint.X, endPoint.Y));
                            }

                            AddPoint(points, new Point(startPoint.X, startPoint.Y));
                            AddPoint(points, new Point(endPoint.X, endPoint.Y));

                        }
                    }

                    //检查是否选择了足够的点, 计算最小包围圆
                    if (points.Count >= 2)
                    {
                        ed.WriteMessage("\n点的数量为:" + points.Count);

                        //至少选择了1条线段，2个点
                        CircleInfo resCircle = GetSmallestEnclosingCircle(points);

                        //在图形中绘制圆和圆心
                        DrawCircleInDwg(resCircle, db, tr);
                        ed.WriteMessage("\n最小包围圆生成成功!");
                    }
                    else
                    {
                        ed.WriteMessage("\n请选择至少1条直线段。");
                    }

                    tr.Commit();
                }
            }
            else
            {
                ed.WriteMessage("\n未选择任何线段,无法创建最小包围圆!");
            }
        }
        catch (System.Exception ex)
        {
            ed.WriteMessage("\n出错: " + ex.Message);
        }
    }


    /// <summary>
    /// 设置点的颜色为绿色,并显示进度条
    /// </summary>
    [CommandMethod("ProgressManager")]
    public void TestProgressManager()
    {
        Database db = HostApplicationServices.WorkingDatabase;
        Editor ed = AcadApp.DocumentManager.MdiActiveDocument.Editor;
        //创建消息过滤类
        MessageFilter filter = new MessageFilter();
        //为程序添加消息过滤
        System.Windows.Forms.Application.AddMessageFilter(filter);
        bool esc = false;//标识是否按下了Esc键
                         //开始事务处理并新建一个进度条管理类
        using (Transaction trans = db.TransactionManager.StartTransaction())
        using (ProgressManager manager = new ProgressManager("更改点的颜色"))
        {
            //获取模型空间中所有的点对象
            var points = db.GetEntsInModelSpace<DBPoint>();

            //设置进度条需要更新的次数，一般为循环次数
            manager.SetTotalOperations(points.Count);

            foreach (var point in points)//遍历点
            {
                manager.Tick();     //进度条更新进度
                point.UpgradeOpen();    //切换点对象的状态为写
                point.ColorIndex = 3;   //设置点对象的颜色为绿色

                //如果用户按下了Esc键，则结束遍历
                if (filter.KeyName == Keys.Escape)
                {
                    esc = true;
                    break;
                }
            }
            if (esc) trans.Abort(); //如果按下了Esc键，则放弃所有的更改
            else trans.Commit();    //否则程序能完成所有点的变色工作，提交事务处理
        }
        //移除对按键消息的过滤
        System.Windows.Forms.Application.RemoveMessageFilter(filter);
    }

    /// <summary>
    /// 根据是否接近一个点集中的任何点 来确定是否把新的点加入原点集中
    /// </summary>
    /// <param name="points"></param>
    /// <param name="startPoint"></param>
    public void AddPoint(List<Point> points, Point startPoint)
    {
        bool temp_isNear = false;

        if (points.Count > 0)
        {
            for (int i = 0; i < points.Count; i++)
            {
                bool resAreTwoPointsClose = AreTwoPointsClose(points[i], new Point(startPoint.X, startPoint.Y));
                if (resAreTwoPointsClose)
                {
                    temp_isNear = true;
                    break;  //切断for循环,提高程序执行效率
                }
            }
        }

        //如果到最后仍然是false,也就是跟数组中所有的点都不接近的时候才加入这个点进来
        if (!temp_isNear)
        {
            //将端点加入点集
            points.Add(new Point(startPoint.X, startPoint.Y));
        }
    }


    public void TestProgressManager_General(string name, int num, Action func)
    {
        //TODO:通用进度条

        Database db = HostApplicationServices.WorkingDatabase;
        Editor ed = AcadApp.DocumentManager.MdiActiveDocument.Editor;
        //创建消息过滤类
        MessageFilter filter = new MessageFilter();
        //为程序添加消息过滤
        System.Windows.Forms.Application.AddMessageFilter(filter);
        bool esc = false;//标识是否按下了Esc键
                         //开始事务处理并新建一个进度条管理类
        using (Transaction trans = db.TransactionManager.StartTransaction())
        using (ProgressManager manager = new ProgressManager(name))
        {
            //获取模型空间中所有的点对象
            var points = db.GetEntsInModelSpace<DBPoint>();

            //设置进度条需要更新的次数，一般为循环次数
            manager.SetTotalOperations(num);

            foreach (var point in points)//遍历点
            {
                manager.Tick();     //进度条更新进度

                func();
                //point.UpgradeOpen();    //切换点对象的状态为写
                //point.ColorIndex = 3;   //设置点对象的颜色为绿色

                //如果用户按下了Esc键，则结束遍历
                if (filter.KeyName == Keys.Escape)
                {
                    esc = true;
                    break;
                }
            }
            if (esc) trans.Abort(); //如果按下了Esc键，则放弃所有的更改
            else trans.Commit();    //否则程序能完成所有点的变色工作，提交事务处理
        }
        //移除对按键消息的过滤
        System.Windows.Forms.Application.RemoveMessageFilter(filter);
    }



    /// <summary>
    /// 一个函数来判断两个点是否非常接近，以便认为它们是同一个点
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="Threshold"></param>
    /// <returns></returns>
    private bool AreTwoPointsClose(Point p1, Point p2, double Threshold = 0.001)
    {
        double distance = Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));

        //判断距离是否小于或等于阈值
        return distance <= Threshold;
    }


    //绘制最小包围圆和圆心
    private void DrawCircleInDwg(CircleInfo circle, Database db, Transaction tr)
    {
        //获取当前空间块表记录（模型空间）
        BlockTable blockTable = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
        BlockTableRecord blockTableRecord = tr.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

        //创建圆心点
        Point3d centerPoint = new Point3d(circle.Center.X, circle.Center.Y, 0);
        DBPoint dbCenterPoint = new DBPoint(centerPoint);
        dbCenterPoint.ColorIndex = 1;   //红色
        blockTableRecord.AppendEntity(dbCenterPoint);
        tr.AddNewlyCreatedDBObject(dbCenterPoint, true);

        // 创建圆
        Circle dbCircle = new Circle(centerPoint, Vector3d.ZAxis, circle.Radius);
        dbCircle.ColorIndex = 2;        //黄色
        blockTableRecord.AppendEntity(dbCircle);
        tr.AddNewlyCreatedDBObject(dbCircle, true);
    }

    // 最小包围圆算法实现
    private static CircleInfo GetSmallestEnclosingCircle(List<Point> points)
    {
        Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
        Database db = doc.Database;
        Editor ed = doc.Editor;

        var shuffledPoints = new List<Point>(points);
        var random = new Random();
        for (int i = 0; i < shuffledPoints.Count; i++)
        {
            int j = random.Next(i, shuffledPoints.Count);
            var temp = shuffledPoints[i];
            shuffledPoints[i] = shuffledPoints[j];
            shuffledPoints[j] = temp;
        }

        //1.点的数量小于2的情况
        if (points.Count < 2)
        {
            ed.WriteMessage("/n点的数量小于2,无法形成包围圆!");
            return null;
        }

        //2.点的数量等于2的情况
        if (points.Count == 2)
        {
            return CircleFromTwoPoints(points[0], points[1]);
        }

        //3.点的数量等于3的情况
        if (points.Count == 3)
        {
            return CircleFromThreePoints(points[0], points[1], points[2]);
        }

        if (points.Count > 3)
        {
            return GetCircleFromBoundaryPoints_FourOrMorePoint(points);
        }

        //3.点的数量大于3的情况
        CircleInfo c1 = GetCircleFromBoundaryPoints_TwoPoint(points);
        PrintCircleInfo(c1, "c1");
        CircleInfo c2 = GetCircleFromBoundaryPoints_ThreePoint(shuffledPoints);
        PrintCircleInfo(c2, "c2");

        if (c1 == null && c2 == null)
        {
            ed.WriteMessage("/n无法形成包围圆!");
            return null;
        }

        //返回满足要求的半径较小的圆圈
        if (c1 != null && c2 != null)
        {
            if (c2.Radius < c1.Radius)
            {
                return c2;
            }
            else
            {
                return c1;
            }
        }

        if (c1 == null)
        {
            return c2;
        }

        if (c2 == null)
        {
            return c1;
        }

        return c2;
    }


    private static void PrintCircleInfo(CircleInfo circle, string name)
    {
        Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
        Database db = doc.Database;
        Editor ed = doc.Editor;

        ed.WriteMessage("\n name:" + name);
        ed.WriteMessage("\n X:" + circle.Center.X);
        ed.WriteMessage("\n Y:" + circle.Center.Y);
        ed.WriteMessage("\n R:" + circle.Radius);
    }


    private static CircleInfo WelzlAlgorithm(List<Point> points, List<Point> boundary, int n)
    {
        // 基本条件：没有点时返回特定圆
        if (n == 0)
        {
            // 返回根据边界点数量生成的圆
            if (boundary.Count == 0) return new CircleInfo(new Point(0, 0), 0);
            else if (boundary.Count == 1) return new CircleInfo(boundary[0], 0);
            else if (boundary.Count == 2) return CircleFromTwoPoints(boundary[0], boundary[1]);
            else
            {
                return GetCircleFromBoundaryPoints_ThreePoint(boundary);
            }
        }

        //获取当前要处理的点,也就是最后一个点
        Point p = points[n - 1];
        //递归调用，处理剩余的 n-1 个点
        CircleInfo d = WelzlAlgorithm(points, boundary, n - 1);

        CircleInfo resCircleInfo = new CircleInfo(d.Center, d.Radius);

        //检查当前点是否在圆内
        if (d.Contains(p))
        {
            return d; //如果在圆内，则返回该圆
        }

        //下面处理不在圆圈内的情况
        double new_Radi = resCircleInfo.Center.DistanceTo(p);
        CircleInfo resCircleInfo2 = new CircleInfo(resCircleInfo.Center, new_Radi);

        //如果点p不在圆内，将点p加入边界集合
        boundary.Add(p);

        //递归继续处理剩下的点
        return resCircleInfo2;
    }

    /// <summary>
    /// 检查列表中所有点是否都在生成的圆内或者圆上,如果有出现在圆圈外的则返回false
    /// </summary>
    /// <param name="points">列表中所有点</param>
    /// <param name="circle">最终的圆</param>
    /// <returns></returns>
    private static bool AllPointsInsideCircle(List<Point> points, CircleInfo circle)
    {
        foreach (var p in points)
        {
            if (!circle.Contains(p))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 通过两个点形成的半径最小的圆,并且该圆能覆盖其余所有的点
    /// </summary>
    /// <param name="boundary">点的集合</param>
    /// <returns></returns>
    private static CircleInfo GetCircleFromBoundaryPoints_TwoPoint(List<Point> boundary)
    {
        Database db = HostApplicationServices.WorkingDatabase;
        Editor ed = AcadApp.DocumentManager.MdiActiveDocument.Editor;
        //创建消息过滤类
        MessageFilter filter = new MessageFilter();
        //为程序添加消息过滤
        System.Windows.Forms.Application.AddMessageFilter(filter);

        //遍历所有可能的两点组合并返回能覆盖所有点的最小包围圆
        CircleInfo bestCircle = null;

        List<CircleInfo> cirList = new List<CircleInfo>();

        CircleInfo cir_base = CircleFromTwoPoints(boundary[0], boundary[1]);

        PrintCircleInfo(cir_base, "cir_base");


        bool esc = false;//标识是否按下了Esc键
                         //开始事务处理并新建一个进度条管理类
        using (Transaction trans = db.TransactionManager.StartTransaction())
        using (ProgressManager manager = new ProgressManager("阶段1-选取任意两点生成圆,请等待..."))
        {
            int n = boundary.Count;
            int num = n * (n - 1) / 2;
            //设置进度条需要更新的次数，一般为循环次数
            manager.SetTotalOperations(num);

            for (int i = 0; i < boundary.Count; i++)
            {
                for (int j = i + 1; j < boundary.Count; j++)
                {
                    //加入进度条
                    manager.Tick();     //进度条更新进度

                    CircleInfo cir = CircleFromTwoPoints(boundary[i], boundary[j]);

                    //检查所有点是否都在生成的圆内或圆圈上并且cir不为空
                    if (cir != null && AllPointsInsideCircle(boundary, cir))
                    {
                        cirList.Add(cir);
                    }

                    if (filter.KeyName == Keys.Escape)
                    {
                        esc = true;
                        break;
                    }

                }
            }

            if (esc) trans.Abort(); //如果按下了Esc键，则放弃所有的更改
            else trans.Commit();    //否则程序能完成所有点的变色工作，提交事务处理
        }

        //如果没有符合要求的圆圈
        if (cirList.Count == 0)
        {

            ed.WriteMessage("\n 没有符合要求的圆圈");

            //就采用半径最大的圆圈
            bestCircle = cir_base;

            //选出半径最大的圆圈
            for (int i = 0; i < boundary.Count; i++)
            {
                for (int j = i + 1; j < boundary.Count; j++)
                {
                    CircleInfo cir_temp = CircleFromTwoPoints(boundary[i], boundary[j]);
                    //检查所有点是否都在生成的圆内
                    if (cir_temp.Radius > bestCircle.Radius)
                    {
                        bestCircle = cir_temp;
                    }
                }
            }

            //获取最终目标圆圈,对有的点不在目标圆圈中的情况 圆心不变对半径进行扩大处理,以包含外围的点
            bestCircle = GetBestCircle(boundary, bestCircle);

            PrintCircleInfo(bestCircle, "bestCircle11111111111");
        }


        if (cirList.Count > 0)
        {
            ed.WriteMessage("\n 有符合要求的圆圈");

            //有符合要求的圆圈(所有点是否都在生成的圆内),就选出半径最小的那各作为最终的目标圆圈
            bestCircle = GetMinCircle(cirList);
        }

        //移除对按键消息的过滤
        System.Windows.Forms.Application.RemoveMessageFilter(filter);

        return bestCircle; //默认值
    }

    /// <summary>
    /// 以包围盒的中心点作为圆心,以圆心到定点的最大距离作为半径生成圆---算法效率很高,算法的时间复杂度仅仅为n(n为顶点的的个数),生成的圆的效果还不错
    /// </summary>
    /// <param name="boundary"></param>
    /// <returns></returns>
    private static CircleInfo GetCircleFromBoundaryPoints_FourOrMorePoint(List<Point> boundary)
    {
        //求出所有点的包围盒
        double xmin = GetMinXCor(boundary);
        double xmax = GetMaxXCor(boundary);

        double ymin = GetMinYCor(boundary);
        double ymax = GetMaxYCor(boundary);

        //求出圆心坐标---以包围盒的中心点作为圆心
        Point center_new = new Point(0.5 * (xmin + xmax), 0.5 * (ymin + ymax));

        //求出最大半径
        double r_new = 0.5 * Math.Max(xmax - xmin, ymax - ymin);

        double r_ToVertex = r_new;

        //通过循环找出距离顶点距离的最大值 作为最终的半径
        foreach (var item in boundary)
        {
            if (center_new.DistanceTo(item) > r_ToVertex)
            {
                r_ToVertex = center_new.DistanceTo(item);
            }
        }

        CircleInfo cir_res = new CircleInfo(center_new, r_ToVertex);
        return cir_res; //默认值
    }

    /// <summary>
    /// 获取最小的X坐标
    /// </summary>
    /// <param name="boundary"></param>
    /// <returns></returns>
    private static double GetMinXCor(List<Point> boundary)
    {
        double xmin = boundary[0].X;
        foreach (var item in boundary)
        {
            if (item.X < xmin)
            {
                xmin = item.X;
            }
        }
        return xmin;
    }


    private static double GetMinYCor(List<Point> boundary)
    {
        double ymin = boundary[0].Y;
        foreach (var item in boundary)
        {
            if (item.Y < ymin)
            {
                ymin = item.Y;
            }
        }
        return ymin;
    }


    private static double GetMaxXCor(List<Point> boundary)
    {
        double xmax = boundary[0].X;
        foreach (var item in boundary)
        {
            if (item.X > xmax)
            {
                xmax = item.X;
            }
        }
        return xmax;
    }

    private static double GetMaxYCor(List<Point> boundary)
    {
        double ymax = boundary[0].Y;
        foreach (var item in boundary)
        {
            if (item.Y > ymax)
            {
                ymax = item.Y;
            }
        }
        return ymax;
    }


    /// <summary>
    /// 通过三个点形成的半径最小的圆,并且该圆能覆盖其余所有的点
    /// </summary>
    /// <param name="boundary"></param>
    /// <returns></returns>
    private static CircleInfo GetCircleFromBoundaryPoints_ThreePoint(List<Point> boundary)
    {
        Database db = HostApplicationServices.WorkingDatabase;
        Editor ed = AcadApp.DocumentManager.MdiActiveDocument.Editor;
        //创建消息过滤类
        MessageFilter filter = new MessageFilter();
        //为程序添加消息过滤
        System.Windows.Forms.Application.AddMessageFilter(filter);

        //遍历所有可能的三点组合并返回能覆盖所有点的最小包围圆
        CircleInfo bestCircle = null;

        List<CircleInfo> cirList_tem = new List<CircleInfo>();

        bool esc = false;//标识是否按下了Esc键
        //开始事务处理并新建一个进度条管理类
        using (Transaction trans = db.TransactionManager.StartTransaction())
        using (ProgressManager manager = new ProgressManager("阶段2-选取任意三点生成圆,请等待..."))
        {
            int n = boundary.Count;
            int num = n * (n - 1) * (n - 2) / 6;
            //设置进度条需要更新的次数，一般为循环次数
            manager.SetTotalOperations(num);

            CircleInfo cir_base = CircleFromThreePoints(boundary[0], boundary[1], boundary[2]);

            for (int i = 0; i < boundary.Count; i++)
            {
                for (int j = i + 1; j < boundary.Count; j++)
                {
                    for (int k = j + 1; k < boundary.Count; k++)
                    {
                        //加入进度条
                        manager.Tick();     //进度条更新进度

                        CircleInfo cir = CircleFromThreePoints(boundary[i], boundary[j], boundary[k]);
                        if (cir != null && AllPointsInsideCircle(boundary, cir))
                        {
                            cirList_tem.Add(cir);
                        }

                        if (filter.KeyName == Keys.Escape)
                        {
                            esc = true;
                            break;
                        }
                    }
                }
            }

            if (cirList_tem.Count == 0)
            {
                bestCircle = cir_base;

                List<CircleInfo> cirList_tem2 = new List<CircleInfo>();

                for (int i = 0; i < boundary.Count; i++)
                {
                    for (int j = i + 1; j < boundary.Count; j++)
                    {
                        for (int k = j + 1; k < boundary.Count; k++)
                        {
                            CircleInfo cir = CircleFromThreePoints(boundary[i], boundary[j], boundary[k]);
                            if (cir != null)
                            {
                                cirList_tem2.Add(cir);
                            }
                        }
                    }
                }


                if (cirList_tem2.Count > 0)
                {
                    bestCircle = GetMaxCircle(cirList_tem2);
                }

                bestCircle = GetBestCircle(boundary, bestCircle);

            }

            if (cirList_tem.Count > 0)
            {
                //就取半径最小的圆
                bestCircle = cirList_tem[0];
                foreach (var item in cirList_tem)
                {
                    //如果半径更小
                    if (item.Radius < bestCircle.Radius)
                    {
                        bestCircle = item;
                    }
                }
            }

            if (esc) trans.Abort(); //如果按下了Esc键，则放弃所有的更改
            else trans.Commit();    //否则程序能完成所有点的变色工作，提交事务处理
        }

        //移除对按键消息的过滤
        System.Windows.Forms.Application.RemoveMessageFilter(filter);

        return bestCircle;
    }

    /// <summary>
    /// 获取最终目标圆圈,对有的点不在目标圆圈中的情况 圆心不变对半径进行扩大处理,以包含外围的点
    /// </summary>
    /// <param name="boundary"></param>
    /// <param name="bestCircle"></param>
    /// <returns></returns>
    private static CircleInfo GetBestCircle(List<Point> boundary, CircleInfo bestCircle)
    {
        List<CircleInfo> cirList_tem3 = new List<CircleInfo>();
        for (int i = 0; i < boundary.Count; i++)
        {
            //有的点不在目标圆圈中的情况,外围点的情况
            if (!bestCircle.Contains(boundary[i]))
            {
                //对半径进行扩大
                double newRadius = Math.Sqrt(bestCircle.Center.DistanceTo(boundary[i]));
                //半径采用扩大之后的半径

                if (newRadius > bestCircle.Radius)
                {
                    cirList_tem3.Add(new CircleInfo(bestCircle.Center, newRadius));
                }

            }
        }

        //从集合中选出半径最大的圆作为最终的目标圆圈
        if (cirList_tem3.Count > 0)
        {
            bestCircle = GetMaxCircle(cirList_tem3);
        }


        return bestCircle;

    }

    /// <summary>
    /// 从圆圈集合中获取半径最大的圆圈
    /// </summary>
    /// <param name="cirList"></param>
    /// <returns></returns>
    private static CircleInfo GetMaxCircle(List<CircleInfo> cirList)
    {
        CircleInfo ci_temp = cirList[0];
        foreach (var item in cirList)
        {
            if (item.Radius > ci_temp.Radius)
            {
                ci_temp = item;
            }
        }
        return ci_temp;
    }


    /// <summary>
    /// 从圆圈集合中获取半径最大的圆圈
    /// </summary>
    /// <param name="cirList"></param>
    /// <returns></returns>
    private static CircleInfo GetMinCircle(List<CircleInfo> cirList)
    {
        CircleInfo ci_temp = cirList[0];
        foreach (var item in cirList)
        {
            if (item.Radius < ci_temp.Radius)
            {
                ci_temp = item;
            }
        }
        return ci_temp;
    }



    /// <summary>
    /// 生成最大半径圆
    /// </summary>
    /// <param name="boundary"></param>
    /// <returns></returns>
    private static CircleInfo GetMaxRadiusCircle(List<Point> boundary)
    {
        CircleInfo maxCircle = CircleFromThreePoints(boundary[0], boundary[1], boundary[2]);

        if (maxCircle != null)
        {
            for (int i = 0; i < boundary.Count; i++)
            {
                for (int j = i + 1; j < boundary.Count; j++)
                {
                    for (int k = j + 1; k < boundary.Count; k++)
                    {
                        CircleInfo currentCircle = CircleFromThreePoints(boundary[i], boundary[j], boundary[k]);
                        if (currentCircle.Radius > maxCircle.Radius)
                        {
                            maxCircle = currentCircle;
                        }
                    }
                }
            }

        }
        else
        {
            //初始的三个点在一条直线上,无法形成圆的情况
            for (int i = 0; i < boundary.Count; i++)
            {
                for (int j = i + 1; j < boundary.Count; j++)
                {
                    for (int k = j + 1; k < boundary.Count; k++)
                    {
                        CircleInfo currentCircle = CircleFromThreePoints(boundary[i], boundary[j], boundary[k]);

                        if (currentCircle != null)
                        {
                            maxCircle = currentCircle;
                            break; //退出循环,确保能找到一个圆
                        }
                    }
                }
            }

            //获取最大圆
            for (int i = 0; i < boundary.Count; i++)
            {
                for (int j = i + 1; j < boundary.Count; j++)
                {
                    for (int k = j + 1; k < boundary.Count; k++)
                    {
                        CircleInfo currentCircle = CircleFromThreePoints(boundary[i], boundary[j], boundary[k]);
                        if (currentCircle.Radius > maxCircle.Radius)
                        {
                            maxCircle = currentCircle;
                        }
                    }
                }
            }

        }

        return maxCircle; // 返回半径最大的圆
    }


    /// <summary>
    /// 获取圆集合中半径最大的圆
    /// </summary>
    /// <param name="circles"></param>
    /// <returns></returns>
    public static CircleInfo GetCircleWithMaxRadius(List<CircleInfo> circles)
    {
        // 检查圆集合是否为空
        if (circles == null || circles.Count == 0)
        {
            return null; // 返回 null 表示没有找到圆
        }

        // 初始化最大半径的圆
        CircleInfo maxCircle = circles[0];

        // 遍历圆集合
        foreach (var circle in circles)
        {
            // 如果当前圆的半径大于最大圆的半径，则更新最大圆
            if (circle.Radius > maxCircle.Radius)
            {
                maxCircle = circle;
            }
        }

        return maxCircle; //返回半径最大的圆
    }


    /// <summary>
    /// 两点创建圆
    /// </summary>
    /// <param name="A"></param>
    /// <param name="B"></param>
    /// <returns></returns>
    private static CircleInfo CircleFromTwoPoints(Point A, Point B)
    {
        Point center = new Point((A.X + B.X) / 2, (A.Y + B.Y) / 2);
        double radius = Math.Sqrt(Math.Pow(A.X - B.X, 2) + Math.Pow(A.Y - B.Y, 2)) / 2;
        return new CircleInfo(center, radius);
    }

    /// <summary>
    /// 三点创建圆,如果三点共线就返回null,三点不共线才返回通过这三个点的圆
    /// </summary>
    /// <param name="A"></param>
    /// <param name="B"></param>
    /// <param name="C"></param>
    /// <returns></returns>
    private static CircleInfo CircleFromThreePoints(Point A, Point B, Point C, double thr = 0.001)
    {
        double d = 2 * (A.X * (B.Y - C.Y) + B.X * (C.Y - A.Y) + C.X * (A.Y - B.Y));

        bool isZero = Math.Abs(d) <= thr;

        if (isZero)
        {
            //针对三点共线的情况 ,用距离较远的两个点形成一个圆
            return CircleFromTwoFarthestPoints(A, B, C);
        }

        //判断两点形成的圆能否包含第三个点
        CircleInfo c1 = CircleFromTwoPoints(A, B);
        if (c1.Contains(C))
        {
            return c1;
        }

        CircleInfo c2 = CircleFromTwoPoints(A, C);
        if (c2.Contains(B))
        {
            return c2;
        }

        CircleInfo c3 = CircleFromTwoPoints(B, C);
        if (c3.Contains(A))
        {
            return c3;
        }

        //针对三点不共线的情况,且上面三种情况都不满足的情况, 创建一个同时通过三个点的外接圆
        double centerX = ((Math.Pow(A.X, 2) + Math.Pow(A.Y, 2)) * (B.Y - C.Y) +
                         (Math.Pow(B.X, 2) + Math.Pow(B.Y, 2)) * (C.Y - A.Y) +
                         (Math.Pow(C.X, 2) + Math.Pow(C.Y, 2)) * (A.Y - B.Y)) / d;

        double centerY = ((Math.Pow(A.X, 2) + Math.Pow(A.Y, 2)) * (C.X - B.X) +
                         (Math.Pow(B.X, 2) + Math.Pow(B.Y, 2)) * (A.X - C.X) +
                         (Math.Pow(C.X, 2) + Math.Pow(C.Y, 2)) * (B.X - A.X)) / d;

        Point center = new Point(centerX, centerY);
        double radius = Math.Sqrt(Math.Pow(centerX - A.X, 2) + Math.Pow(centerY - A.Y, 2));

        return new CircleInfo(center, radius);
    }


    /// <summary>
    /// 三点共线的情况,用距离较远的两个点形成一个圆
    /// </summary>
    /// <param name="A"></param>
    /// <param name="B"></param>
    /// <param name="C"></param>
    /// <returns></returns>
    private static CircleInfo CircleFromTwoFarthestPoints(Point A, Point B, Point C)
    {
        // 计算三点之间的距离
        double distanceAB = A.DistanceTo(B);
        double distanceAC = A.DistanceTo(C);
        double distanceBC = B.DistanceTo(C);

        // 找到距离较远的两个点
        Point point1, point2;
        if (distanceAB >= distanceAC && distanceAB >= distanceBC)
        {
            point1 = A;
            point2 = B;
        }
        else if (distanceAC >= distanceAB && distanceAC >= distanceBC)
        {
            point1 = A;
            point2 = C;
        }
        else
        {
            point1 = B;
            point2 = C;
        }

        // 计算圆心（两个点的中点）
        double centerX = (point1.X + point2.X) / 2;
        double centerY = (point1.Y + point2.Y) / 2;
        Point center = new Point(centerX, centerY);

        // 计算半径（半径为两点之间的距离的一半）
        double radius = point1.DistanceTo(point2) / 2;

        return new CircleInfo(center, radius);
    }





}
