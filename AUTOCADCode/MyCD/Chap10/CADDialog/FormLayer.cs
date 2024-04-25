using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using DotNetARX;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;
using AcadWnd = Autodesk.AutoCAD.Windows;
namespace CADDialog
{
    public partial class FormLayer : Form
    {
        Document doc;//文档
        Database db;//数据库
        //在对话框存在期间，必须保持事务处理为打开状态
        Transaction trans;//事务处理
        public FormLayer()
        {
            InitializeComponent();
            InitializeListView();//初始化列表视图控件
        }
        private void InitializeListView()
        {
            doc = AcadApp.DocumentManager.MdiActiveDocument;
            db = doc.Database;
            trans = db.TransactionManager.StartTransaction();
            //必须锁定文档，以防止不可预知的错误
            using (DocumentLock loc=doc.LockDocument())
            {
                //定义一个图像列表对象，用于加入列表视图中使用的各种图片
                ImageList imgList=new ImageList();
                //向图像列表添加图片
                imgList.Images.Add(CADResource.IsCurrentTrue);
                imgList.Images.Add(CADResource.IsCurrentFalse);
                imgList.Images.Add(CADResource.IsOffTrue);
                imgList.Images.Add(CADResource.IsOffFalse);
                imgList.Images.Add(CADResource.IsFrozenTrue);
                imgList.Images.Add(CADResource.IsFrozenFalse);
                imgList.Images.Add(CADResource.IsLockedTrue);
                imgList.Images.Add(CADResource.IsLockedFalse);
                imgList.Images.Add(CADResource.IsPlottableTrue);
                imgList.Images.Add(CADResource.IsPlottableFalse);
                imgList.Images.Add(CADResource.ViewportVisibilityDefaultTrue);
                imgList.Images.Add(CADResource.ViewportVisibilityDefaultFalse);
                //为图像列表中的图标设置索引键
                imgList.Images.SetKeyName(0, "IsCurrentTrue");
                imgList.Images.SetKeyName(1, "IsCurrentFalse");
                imgList.Images.SetKeyName(2, "IsOffTrue");
                imgList.Images.SetKeyName(3, "IsOffFalse");
                imgList.Images.SetKeyName(4, "IsFrozenTrue");
                imgList.Images.SetKeyName(5, "IsFrozenFalse");
                imgList.Images.SetKeyName(6, "IsLockedTrue");
                imgList.Images.SetKeyName(7, "IsLockedFalse");
                imgList.Images.SetKeyName(8, "IsPlottableTrue");
                imgList.Images.SetKeyName(9, "IsPlottableFalse");
                imgList.Images.SetKeyName(10, "ViewportVisibilityDefaultTRUE");
                imgList.Images.SetKeyName(11, "ViewportVisibilityDefaultFalse");
                //设置列表视图的小图标
                this.olvLayerManager.SmallImageList = imgList;
                //设置层状态列（图像表示）
                this.ColumnState.ImageGetter = delegate(object o)
                {
                    ObjectId layerId=(ObjectId)o;
                    if (layerId == db.Clayer)
                        return "IsCurrentTrue";//图层为当前层时的图像
                    else
                        return "IsCurrentFalse";//图层不是当前层时的图像
                };
                //设置层名列
                this.ColumnName.AspectGetter = delegate(object o)
                {
                    //获取列表视图中的模型对象:层的ObjectId
                    ObjectId layerId=(ObjectId)o;
                    LayerTableRecord ltr=(LayerTableRecord)layerId.GetObject(OpenMode.ForRead);
                    return ltr.Name;//返回图层名为本列内容
                };

                //设置图层打开状态列（图像表示）
                this.ColumnIsOff.ImageGetter = delegate(object o)
                {
                    ObjectId layerId=(ObjectId)o;
                    LayerTableRecord ltr=(LayerTableRecord)layerId.GetObject(OpenMode.ForRead);
                    if (ltr.IsOff)
                        return "IsOffTrue";//图层关闭的图像
                    else
                        return "IsOffFalse";//图层打开的图像
                };
                //设置图层冻结状态列（图像表示）
                this.ColumnIsFrozen.ImageGetter = delegate(object o)
                {
                    ObjectId layerId=(ObjectId)o;
                    LayerTableRecord ltr=(LayerTableRecord)layerId.GetObject(OpenMode.ForRead);
                    if (ltr.IsFrozen)
                        return "IsFrozenTrue";//图层冻结的图像
                    else
                        return "IsFrozenFalse";//图层未冻结的图像
                };

                //设置图层锁定状态列（图像表示）
                this.ColumnIsLocked.ImageGetter = delegate(object o)
                {
                    ObjectId layerId=(ObjectId)o;
                    LayerTableRecord ltr=(LayerTableRecord)layerId.GetObject(OpenMode.ForRead);
                    if (ltr.IsLocked)
                        return "IsLockedTrue";//图层锁定的图像
                    else
                        return "IsLockedFalse";//图层未锁定的图像
                };
                //设置图层颜色列
                this.ColumnColor.AspectGetter = delegate(object o)
                {
                    ObjectId layerId=(ObjectId)o;
                    LayerTableRecord ltr=(LayerTableRecord)layerId.GetObject(OpenMode.ForRead);
                    return ltr.Color.ColorNameForDisplay;//返回颜色名为本列的显示内容
                };
                //设置线型列
                this.ColumnLinetype.AspectGetter = delegate(object o)
                {
                    ObjectId layerId=(ObjectId)o;
                    LayerTableRecord ltr=(LayerTableRecord)layerId.GetObject(OpenMode.ForRead);
                    //获取图层的线型
                    LinetypeTableRecord lttr=(LinetypeTableRecord)ltr.LinetypeObjectId.GetObject(OpenMode.ForRead);
                    return lttr.Name;//返回图层线型名为本列的显示内容
                };
                //设置线宽列
                this.ColumnLineWeight.AspectGetter = delegate(object o)
                {
                    ObjectId layerId=(ObjectId)o;
                    LayerTableRecord ltr=(LayerTableRecord)layerId.GetObject(OpenMode.ForRead);
                    //获取图层的线宽值(LineWeight枚举)，并将其强制转换为double型
                    double lineWeight=(double)ltr.LineWeight / 100;
                    //设置本列显示的内容
                    if (lineWeight < 0)//线型类型为ByLayer、ByBlock、
                        return "默认";
                    else
                        return lineWeight.ToString("0.00") + " 毫米";
                };
                //设置图层可打印列（图像表示）
                this.ColumnIsPlottable.ImageGetter = delegate(object o)
                {
                    ObjectId layerId=(ObjectId)o;
                    LayerTableRecord ltr=(LayerTableRecord)trans.GetObject(layerId, OpenMode.ForRead);
                    if (ltr.IsPlottable)
                        return "IsPlottableTrue";//图层可打印的图像
                    else
                        return "IsPlottableFalse";//图层不可打印的图像
                };
                //设置打印样式列
                this.ColumnPlotStyleName.AspectGetter = delegate(object o)
                {
                    ObjectId layerId=(ObjectId)o;
                    LayerTableRecord ltr=(LayerTableRecord)layerId.GetObject(OpenMode.ForRead);
                    return ltr.PlotStyleName;//返回图层的打印样式名为本列的显示内容
                };
                //设置冻结新视口列（图像表示）
                this.ColumnViewportVisibilityDefault.ImageGetter = delegate(object o)
                {
                    ObjectId layerId=(ObjectId)o;
                    LayerTableRecord ltr=(LayerTableRecord)trans.GetObject(layerId, OpenMode.ForRead);
                    if (ltr.ViewportVisibilityDefault)
                        return "ViewportVisibilityDefaultTrue";//图层冻结新视口的图像
                    else
                        return "ViewportVisibilityDefaultFalse";//图层不冻结新视口的图像
                };
                //获取所有层表的ObjectId（DotNetARX中LayerTools类的自定义函数）
                List<ObjectId> layers = db.GetAllLayerObjectIds();
                //设置列表视图中需要显示的模型对象
                this.olvLayerManager.SetObjects(layers);
            }
        }

        private void FormLayer_Load(object sender, EventArgs e)
        {
            //获取当前层
            LayerTableRecord layer=db.Clayer.GetObject(OpenMode.ForRead) as LayerTableRecord;
            //设置文本框的内容为当前图层层
            this.textBoxCurrentLayer.Text = "当前图层：" + layer.Name;
        }

        private void olvLayerManager_CellClick(object sender, BrightIdeasSoftware.CellClickEventArgs e)
        {
            if (e.Item == null) return;//如果点击的单元格无内容，则返回
            //为了防止错误，锁定文档
            using (DocumentLock loc=doc.LockDocument())
            {
                //当前列表视图中使用的对象模型（图层的ObjectId）
                ObjectId layerId=(ObjectId)e.Model;
                //打开层表为写的状态
                LayerTableRecord ltr=(LayerTableRecord)layerId.GetObject(OpenMode.ForWrite);
                switch (e.Column.Text)//当前点击的单元格列名
                {
                    case "开"://点击的是图层打开列
                        //如果点击的是当前图层且图层为打开状态
                        if (layerId == db.Clayer && ltr.IsOff == false)
                        {
                            //显示一个警告对象框，提示用户是否关闭当前图层
                            DialogResult dlgRes=MessageBox.Show("当前图层将被关闭。\n是否要使当前图层保持打开状态？", "AutoCAD", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                            //如果用户选择“是”按钮，则关闭当前图层
                            if (dlgRes == DialogResult.Yes)
                            {
                                ltr.IsOff = false;
                            }
                        }
                        else
                            ltr.IsOff = !ltr.IsOff;//切换图层的打开状态
                        break;
                    case "冻结"://点击的是图层冻结列
                        if (layerId == db.Clayer)//如果点击的是当前图层
                            //显示一个警告对象框，提示用户不能冻结当前图层
                            MessageBox.Show("不能冻结当前图层。", "AutoCAD", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        else
                            ltr.IsFrozen = !ltr.IsFrozen;//切换图层的冻结状态
                        break;
                    case "锁定"://点击的是图层锁定列
                        ltr.IsLocked = !ltr.IsLocked;//切换图层的锁定状态
                        break;
                    case "颜色"://点击的是图层颜色列
                        //打开一个AutoCAD颜色对话框
                        AcadWnd.ColorDialog colorDialog=new AcadWnd.ColorDialog();
                        colorDialog.ShowDialog();//显示颜色对话框
                        //设置图层的颜色为用户在颜色对话框中选择的颜色
                        ltr.Color = colorDialog.Color;
                        break;
                    case "线型"://点击的是图层线型列
                        //打开一个AutoCAD线型对话框
                        AcadWnd.LinetypeDialog linetypeDialog=new AcadWnd.LinetypeDialog();
                        linetypeDialog.ShowDialog();//显示线型对话框
                        //设置图层的线型为用户在线型对话框中选择的线型
                        ltr.LinetypeObjectId = linetypeDialog.Linetype;
                        break;
                    case "线宽"://点击的是图层线宽列
                        //打开一个AutoCAD线宽对话框
                        AcadWnd.LineWeightDialog lineWeightDialog=new AcadWnd.LineWeightDialog();
                        lineWeightDialog.ShowDialog();//显示线宽对话框
                        //设置图层的线宽为用户在线宽对话框中选择的线宽
                        ltr.LineWeight = lineWeightDialog.LineWeight;
                        break;
                    case "打印"://点击的是图层可打印列
                        ltr.IsPlottable = !ltr.IsPlottable;//切换图层的可打印状态
                        break;
                    case "冻结新视口"://点击的是图层是否冻结新视口列
                        //切换图层的是否冻结新视口状态
                        ltr.ViewportVisibilityDefault = !ltr.ViewportVisibilityDefault;
                        break;
                }
                //更新当前列表视图中使用的对象模型（图层的ObjectId）
                this.olvLayerManager.RefreshObject(e.Model);
            }
        }

        //设置当前层
        private void buttonSetCurrentLayer_Click(object sender, EventArgs e)
        {
            using (DocumentLock loc=doc.LockDocument())
            {
                //列表实视图中被选择的层
                ObjectId layerId=(ObjectId)this.olvLayerManager.SelectedObject;
                db.Clayer = layerId;//设置该层为当前层
                this.olvLayerManager.BuildList();//更新列表视图的状态
            }
        }
        //新建图层
        private void buttonNewLayer_Click(object sender, EventArgs e)
        {
            using (DocumentLock loc=doc.LockDocument())
            {
                LayerTable lt=(LayerTable)db.LayerTableId.GetObject(OpenMode.ForRead);
                //声明一个列表对象，用于存储当前图形中图层名类似于“图层
                List<int> layerNums=new List<int>();
                foreach (ObjectId layerId in lt)//遍历层表记录
                {
                    LayerTableRecord ltr=(LayerTableRecord)layerId.GetObject(OpenMode.ForRead);
                    //如果图层名以“图层”字样开头
                    if (ltr.Name.StartsWith("图层"))
                    {
                        //将图层名去除“图层”字样后的字符串值
                        string layerNum=ltr.Name.Substring(2);
                        if (layerNum.IsInt())
                            layerNums.Add(Convert.ToInt32(layerNum));
                    }
                }
                //要创建的新图层名的数字编号
                int newLayerNum=layerNums.Count > 0 ? layerNums.Max() + 1 : 1;
                //添加新图层
                ObjectId newLayerId=db.AddLayer("图层" + newLayerNum);
                //更新当前列表视图
                this.olvLayerManager.SetObjects(db.GetAllLayerObjectIds());
            }
        }

        //删除图层
        private void buttonDeleteLayer_Click(object sender, EventArgs e)
        {
            using (DocumentLock loc=doc.LockDocument())
            {
                //获取当前选择的图层名
                string layerName=this.olvLayerManager.SelectedItem.Text;
                //删除选择的图层，并判断是否可以删除
                if (db.DeleteLayer(layerName) == false)
                {
                    AcadApp.ShowAlertDialog("未能删除选定的图层。");
                }
            }
        }
        //新建冻结新视口
        private void buttonNewLayerFrozen_Click(object sender, EventArgs e)
        {
            using (DocumentLock loc=doc.LockDocument())
            {
                LayerTable lt=(LayerTable)db.LayerTableId.GetObject(OpenMode.ForRead);
                List<int> layerNums=new List<int>();
                foreach (ObjectId layerId in lt)
                {
                    LayerTableRecord ltr=(LayerTableRecord)layerId.GetObject(OpenMode.ForRead);
                    if (ltr.Name.StartsWith("图层"))
                    {
                        string layerNum=ltr.Name.Substring(2);
                        if (layerNum.IsInt())
                            layerNums.Add(Convert.ToInt32(layerNum));
                    }
                }
                int newLayerNum=layerNums.Count > 0 ? layerNums.Max() + 1 : 1;
                ObjectId newLayerId=db.AddLayer("图层" + newLayerNum);
                LayerTableRecord newLayer=(LayerTableRecord)newLayerId.GetObject(OpenMode.ForWrite);
                newLayer.ViewportVisibilityDefault = true;//冻结视口
                this.olvLayerManager.SetObjects(db.GetAllLayerObjectIds());
            }
        }
        //确定按钮
        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (trans != null)//如果有事务处理需要提交
            {
                trans.Commit();//提交事务处理
                trans = null;
            }
            this.Dispose();//关闭对话框
        }
        //应用按钮
        private void buttonApply_Click(object sender, EventArgs e)
        {
            if (trans != null)
            {
                trans.Commit();//提交事务处理
                //再次开启事务处理
                trans = db.TransactionManager.StartTransaction();
            }
        }
        //帮助按钮
        private void buttonHelp_Click(object sender, EventArgs e)
        {
            string fullHelpPath = Path.Combine(Tools.GetCurrentPath(), "acad_acr.chm");
            AcadApp.InvokeHelp(fullHelpPath, "d0e55330");
        }
        //取消按钮
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            if (trans != null)
            {
                trans.Abort();//放弃事务处理
                trans = null;
            }
            this.Dispose();
        }
    }
}
