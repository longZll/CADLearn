using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using DotNetARX;
using Excel = NetOffice.ExcelApi;

namespace ExcelCAD
{
    public class ExcelCAD
    {
        //提取Excel数据到CAD
        [CommandMethod("ExcelToCAD")]
        public void ExcelToCAD()
        {
            Document doc=Application.DocumentManager.MdiActiveDocument;
            Database db=doc.Database;
            Editor ed=doc.Editor;
            // 初始化对COM对象的支持
            LateBindingApi.Core.Factory.Initialize();
            // 声明一个Excel对象
            Excel.Application excelApp = new Excel.Application();
            //定义打开文件对话框选项
            PromptOpenFileOptions opt=new PromptOpenFileOptions("请选择Excel文件");
            //文件对话框的标题
            opt.DialogCaption = "选择Excel文件";
            //文件对话框的文件过滤器列表，可以是各版本的Excel
            opt.Filter = "Excel 97-2003工作簿(*.xls)|*.xls|Excel工作簿(*.xlsx)|*.xlsx";
            //文件过滤器列表中缺省显示的文件扩展名
            opt.FilterIndex = 0;
            //根据用户的选择，获取文件名
            PromptFileNameResult fileResult=ed.GetFileNameForOpen(opt);
            //若没有成功，则返回
            if (fileResult.Status != PromptStatus.OK) return;
            //设置文件名变量
            string filename=fileResult.StringResult;
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                //打开选择的Excel文件，获得指定的页
                Excel.Workbook book= excelApp.Workbooks.Open(filename);
                Excel.Worksheet sheet=(Excel.Worksheet)book.Worksheets["Sheet1"];
                //用于保存点位名字和坐标的列表
                List<DBText> txts=new List<DBText>();
                List<DBPoint> points=new List<DBPoint>();
                //设置AutoCAD中点的形状和大小
                Application.SetSystemVariable("PDMODE", 35);
                Application.SetSystemVariable("PDSIZE", 2);
                int i=0;                
                //对单元格进行循环，若为空或空格则结束循环
                while (sheet.Cells[i + 1, 1].Value!= null && sheet.Cells[i + 1, 1].Value.ToString().Trim() != "")
                {
                    //设置临时变量用于保存点的位置和名称
                    DBPoint pt=new DBPoint();
                    DBText txt=new DBText();
                    //获取点的名称，位于第1列
                    txt.TextString = sheet.Cells[i + 1, 1].Value.ToString();
                    //获取点的X、Y、Z值，分别位于第2、3、4列
                    double x=Convert.ToDouble(sheet.Cells[i + 1, 2].Value);
                    double y=Convert.ToDouble(sheet.Cells[i + 1, 3].Value);
                    double z=Convert.ToDouble(sheet.Cells[i + 1, 4].Value);
                    //设置点的位置
                    pt.Position = new Point3d(x, y, z);
                    //设置文本的位置,位于点的右侧
                    txt.Position = pt.Position.Add(new Vector3d(3, 0, 0));
                    //添加点和点名称到对应的列表
                    points.Add(pt);
                    txts.Add(txt);
                    i++;
                }
                //添加点和点名称到当前模型空间
                db.AddToModelSpace(txts.ToArray());
                db.AddToModelSpace(points.ToArray());
                trans.Commit();
            }
            // 关闭Excel程序并销毁引用的对象
            excelApp.Quit();
            excelApp.Dispose();
        }
    }
}
