using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using DotNetARX;
using Polenter.Serialization;
namespace FileCAD
{
    public class FileCAD
    {
        //写入文本文件
        [CommandMethod("WriteTextFile")]
        public static void WriteTextFile()
        {
            Database db=HostApplicationServices.WorkingDatabase;
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                //设置文件名
                string fileName=Tools.GetCurrentPath() + "\\Points.txt";
                //获取AutoCAD中所有的点
                var pointsInCAD=db.GetEntsInModelSpace<DBPoint>();
                //将点的坐标值遍历进文本文件，若文件不存在则先会创建文件
                foreach (var point in pointsInCAD)
                {
                    File.AppendAllText(fileName, point.Position.ToString() + Environment.NewLine);
                }
                trans.Commit();
            }
        }

        //读取文本文件
        [CommandMethod("ReadTextFile")]
        public static void ReadTextFile()
        {
            List<DBPoint> ptsInCAD=new List<DBPoint>(); // 创建CAD点列表
            string fileName=Tools.GetCurrentPath() + "\\Points.txt"; // 打开文本文件
            if (File.Exists(fileName)) // 如果文件存在
            {
                string[] strLines=File.ReadAllLines(fileName); // 读取文本文件的所有行
                foreach (string strLine in strLines) // 遍历文本
                {
                    Point3d pt=strLine.StringToPoint3d(); // 将文本行的数据转换为Point3d            
                    ptsInCAD.Add(new DBPoint(pt)); // 创建AutoCAD点并添加到列表中
                }
            }
            Database db=HostApplicationServices.WorkingDatabase;
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                db.AddToModelSpace(ptsInCAD.ToArray()); // 添加点到AutoCAD中
                trans.Commit();
            }
        }

        //二进制序列化
        [CommandMethod("Serilize")]
        public static void Serilize()
        {
            //定义一个可序列化类SerilizePoint的列表
            List<SerilizePoint> points=new List<SerilizePoint>();
            Database db=HostApplicationServices.WorkingDatabase;
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                //获取AutoCAD中所有的点
                var pointsInCAD=db.GetEntsInModelSpace<DBPoint>();
                //对点的坐标值进行遍历并添加到SerilizePoint类列表
                foreach (var point in pointsInCAD)
                {
                    SerilizePoint sp=new SerilizePoint { X = point.Position.X, Y = point.Position.Y, Z = point.Position.Z };
                    points.Add(sp);
                }
                //声明一个二进制序列化对象
                SharpSerializer serializer = new SharpSerializer(true);
                //将SerilizePoint类列表序列化到一个二进制文件
                serializer.Serialize(points, Tools.GetCurrentPath() + "\\Points.bin");
                trans.Commit();
            }
        }

        // 二进制反序列化
        [CommandMethod("DeSerilize")]
        public static void DeSerilize()
        {
            // 定义一个可序列化类SerilizePoint的列表
            List<SerilizePoint> points=new List<SerilizePoint>();
            List<DBPoint> ptsInCAD=new List<DBPoint>();
            // 声明一个二进制序列化对象
            SharpSerializer serializer = new SharpSerializer(true);
            // 将Points.bin文件中保存的点坐标反序列化为SerilizePoint列表
            points = serializer.Deserialize(Tools.GetCurrentPath() + "\\Points.bin") as List<SerilizePoint>;

            foreach (var point in points) //遍历SerilizePoint列表以创建AutoCAD点
            {
                ptsInCAD.Add(new DBPoint(new Point3d(point.X, point.Y, point.Z)));
            }
            Database db=HostApplicationServices.WorkingDatabase;
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                db.AddToModelSpace(ptsInCAD.ToArray()); // 添加点到AutoCAD中
                trans.Commit();
            }
        }

        //写入XML文件
        [CommandMethod("WriteXML")]
        public static void WriteXML()
        {
            //创建一个XML根元素
            XElement  xroot=new XElement("Root");
            Database db=HostApplicationServices.WorkingDatabase;
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                //获取AutoCAD中所有的点
                var pointsInCAD=db.GetEntsInModelSpace<DBPoint>();
                //打开层表并进行遍历
                LayerTable lt=(LayerTable)trans.GetObject(db.LayerTableId, OpenMode.ForRead);
                foreach (ObjectId id in lt)
                {
                    //获取每个层的层名
                    LayerTableRecord ltr=(LayerTableRecord)trans.GetObject(id, OpenMode.ForRead);
                    string layerName=ltr.Name;
                    //创建一个名为Layer的新元素
                    XElement xLayer=new XElement("Layer");
                    //在Layer元素下添加表示层名、层颜色、层颜色索引值的属性
                    xLayer.Add(new XAttribute("Name", layerName));
                    xLayer.Add(new XAttribute("Color", ltr.Color.ToString()));
                    xLayer.Add(new XAttribute("ColorIndex", ltr.Color.ColorIndex.ToString()));
                    //获取点的坐标
                    var pointsOnLayer=from pt in pointsInCAD
                                      where pt.Layer == layerName
                                      select pt.Position;
                    var pointsToXML=new XElement("Points", //新建一个Points元素
                    from pt in pointsOnLayer //将点坐标遍历成Points元素下新的Point元素
                    select new XElement("Point",
                                new XElement("X", pt.X.ToString()),
                                new XElement("Y", pt.Y.ToString()),
                                new XElement("Z", pt.Z.ToString())
                                )
                            );
                    xLayer.Add(pointsToXML);//添加Points元素到Layer元素中
                    xroot.Add(xLayer);//添加Layer元素到根元素中
                }
                //在当前目录下新建一个XML文件，将从AutoCAD中获取的点信息添加到XML文件中
                string fileName=Tools.GetCurrentPath() + "\\Points.XML";
                xroot.Save(fileName);
                trans.Commit();
            }
        }
        //读取XML文件
        [CommandMethod("ReadXML")]
        public static void ReadXML()
        {
            //载入当前目录下的PointsLinq.XML文件
            string fileName=Tools.GetCurrentPath() + "\\Points.XML";
            XElement xroot=XElement.Load(fileName);
            Database db=HostApplicationServices.WorkingDatabase;
            //创建CAD点列表
            List<DBPoint> ptsInCAD=new List<DBPoint>();
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                //打开层表，因为可能要加入新的层，所以打开方式为写
                LayerTable lt=(LayerTable)trans.GetObject(db.LayerTableId, OpenMode.ForWrite);
                //遍历根目录下的Layer元素
                foreach (var layer in xroot.Elements())
                {
                    //通过Layer元素的属性分别获取层名和层的颜色索引值
                    string layerName=layer.Attribute("Name").Value;
                    short colorIndex=Convert.ToInt16(layer.Attribute("ColorIndex").Value);
                    //如果图形中不存在指定的层，则创建新层
                    if (!lt.Has(layerName))
                    {
                        LayerTableRecord ltr=new LayerTableRecord();
                        ltr.Name = layerName;
                        ltr.Color = Color.FromColorIndex(ColorMethod.ByAci, colorIndex);
                        lt.Add(ltr);
                        trans.AddNewlyCreatedDBObject(ltr, true);
                    }
                    //遍历Layer元素的Points子元素下的Point元素
                    foreach (var point in layer.Element("Points").Elements())
                    {
                        //通过Point元素的X、Y、Z子元素获取点的坐标值
                        double x=Convert.ToDouble(point.Element("X").Value);
                        double y=Convert.ToDouble(point.Element("Y").Value);
                        double z=Convert.ToDouble(point.Element("Z").Value);
                        //创建CAD点，设置层并添加到点列表中以一次性添加到AutoCAD中
                        DBPoint dbPoint=new DBPoint(new Point3d(x, y, z));
                        dbPoint.Layer = layerName;
                        ptsInCAD.Add(dbPoint);
                    }
                }
                //将XML文件中获取的点添加到AutoCAD中
                db.AddToModelSpace(ptsInCAD.ToArray());
                trans.Commit();
            }
        }
    }
}
