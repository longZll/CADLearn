using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;

namespace TranlateCoordinate
{
    public class TranlateCoordinate 
    {
        /// <summary>
        /// 绘制矩形管道
        /// </summary>
        [CommandMethod("DrawRectPipe")]
        public void DrawRectPipe()
        {
            // 输入起点、终点
            Point3d startPoint = new Point3d();
            Point3d endPoint = new Point3d();
            if (GetPoint("\n输入起点:", out startPoint) && GetPoint("\n输入终点:", startPoint, out endPoint))
            {
                // 绘制管道
                DrawPipe(startPoint, endPoint, 100, 70);
            }            
        }

        /// <summary>
        /// 绘制管道
        /// </summary>
        /// <param name="startPoint">起点坐标</param>
        /// <param name="endPoint">终点坐标</param>
        /// <param name="width">管道横截面宽度</param>
        /// <param name="height">管道横截面高度</param>
        private void DrawPipe(Point3d startPoint, Point3d endPoint, double width, double height)
        {
            //获得变换矩阵
            Vector3d inVector = endPoint - startPoint;      //入口向量
            Vector3d normal = GetNormalByInVector(inVector);      //根据入口向量计算出法向量
            Matrix3d mat = GetTranslateMatrix(startPoint, inVector, normal); //据这两个向量获得从WCS到UCS的变换矩阵

            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {                
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
                BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                
                // 顶面
                double z = 0.5 * height;
                double length = startPoint.DistanceTo(endPoint);
                Face fTop = new Face(new Point3d(0, -0.5 * width, z), new Point3d(length, -0.5 * width, z), new Point3d(length, 0.5 * width, z), 
                    new Point3d(0, 0.5 * width, z), true, true, true, true);
                fTop.TransformBy(mat);

                btr.AppendEntity(fTop);
                trans.AddNewlyCreatedDBObject(fTop, true);

                // 底面
                z = -0.5 * height;
                Face fBottom = new Face(new Point3d(0, -0.5 * width, z), new Point3d(length, -0.5 * width, z), new Point3d(length, 0.5 * width, z),
                    new Point3d(0, 0.5 * width, z), true, true, true, true);
                fBottom.TransformBy(mat);

                btr.AppendEntity(fBottom);
                trans.AddNewlyCreatedDBObject(fBottom, true);

                // 左侧面
                double y = 0.5 * width;
                Face fLeftSide = new Face(new Point3d(0, y, 0.5 * height), new Point3d(length, y, 0.5 * height), new Point3d(length, y, -0.5 * height),
                    new Point3d(0, y, -0.5 * height), true, true, true, true);
                fLeftSide.TransformBy(mat);

                btr.AppendEntity(fLeftSide);
                trans.AddNewlyCreatedDBObject(fLeftSide, true);

                // 右侧面
                y = -0.5 * width;
                Face fRightSide = new Face(new Point3d(0, y, 0.5 * height), new Point3d(length, y, 0.5 * height), new Point3d(length, y, -0.5 * height),
                    new Point3d(0, y, -0.5 * height), true, true, true, true);
                fRightSide.TransformBy(mat);

                btr.AppendEntity(fRightSide);
                trans.AddNewlyCreatedDBObject(fRightSide, true);

                trans.Commit();
            }
        }


        /// <summary>
        ///  根据入口向量、法向量获得变换矩阵
        /// </summary>
        /// <param name="inPoint">起点坐标</param>
        /// <param name="inVector">入口向量</param>
        /// <param name="normal">根据入口向量计算所得法向量</param>
        /// <returns>变换矩阵</returns>
        Matrix3d GetTranslateMatrix(Point3d inPoint, Vector3d inVector, Vector3d normal)
        {
            Vector3d xAxis = inVector;
            xAxis = xAxis.GetNormal();
            Vector3d zAxis = normal;
            zAxis = zAxis.GetNormal();
            Vector3d yAxis = new Vector3d(xAxis.X, xAxis.Y, xAxis.Z);
            yAxis = yAxis.RotateBy(Math.PI * 0.5, zAxis);

            return Matrix3d.AlignCoordinateSystem(Point3d.Origin, Vector3d.XAxis, Vector3d.YAxis, Vector3d.ZAxis, inPoint, xAxis, yAxis, zAxis);
        }

        
        /// <summary>
        /// 提示用户拾取点
        /// </summary>
        /// <param name="prompt">提示字符串</param>
        /// <param name="pt">点的坐标</param>
        /// <returns>是否获取到点</returns>
        public bool GetPoint(string prompt, out Point3d pt)
        {
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            PromptPointResult ppr = ed.GetPoint(prompt);
            if (ppr.Status == PromptStatus.OK)
            {
                pt = ppr.Value;

                //变换到世界坐标系
                Matrix3d mat = ed.CurrentUserCoordinateSystem;
                pt.TransformBy(mat);
                return true;
            }
            else
            {
                pt = new Point3d();
                return false;
            }
        }

        /// <summary>
        /// 拾取第二个点
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="basePoint">基准点</param>
        /// <param name="pt"></param>
        /// <returns></returns>
        public bool GetPoint(string prompt, Point3d basePoint, out Point3d pt)
        {
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            PromptPointOptions ppo = new PromptPointOptions(prompt);
            ppo.BasePoint = basePoint;
            ppo.UseBasePoint = true;  //显示基准线
            PromptPointResult ppr = ed.GetPoint(ppo);
            if (ppr.Status == PromptStatus.OK)
            {
                pt = ppr.Value;

                //变换到世界坐标系
                Matrix3d mat = ed.CurrentUserCoordinateSystem;
                pt.TransformBy(mat);
                return true;
            }
            else
            {
                pt = new Point3d();
                return false;
            }
        }

        
        /// <summary>
        /// 根据用户指定的入口向量计算法向量
        /// </summary>
        /// <param name="inVector"></param>
        /// <returns></returns>
        private Vector3d GetNormalByInVector(Vector3d inVector)
        {
            double tol = 1.0E-7;        //设定容差
            if (Math.Abs(inVector.X) < tol && Math.Abs(inVector.Y) < tol)
            {
                //1.原向量X和Y分量都等于0的情况,也就是Z方向
                if (inVector.Z >= 0)
                {
                    //1.1-Z轴正方向的情况
                    return new Vector3d(-1, 0, 0);  //返回X轴负方向
                }
                else
                {
                    //1.2-Z轴负方向的情况
                    return Vector3d.XAxis;
                }
            }
            else
            {
                //2.原向量X和Y分量不都等于0的情况,通常情况
           
                Vector2d yAxis2d = new Vector2d(inVector.X, inVector.Y);  //先获得UCS的X轴
                yAxis2d = yAxis2d.RotateBy(Math.PI * 0.5);  //平面旋转90度 得到Y轴
                Vector3d yAxis = new Vector3d(yAxis2d.X, yAxis2d.Y, 0);
                Vector3d normal = yAxis;
                normal = normal.RotateBy(Math.PI * 0.5, inVector); //将Y轴沿UCS的x轴旋转90°得到Z轴。 Z轴方向也就是所要求的法向量的方向
                return normal;
            }
        }
    }
}
