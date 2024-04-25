using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.PlottingServices;
using DotNetARX;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;
namespace Plots
{
    public partial class PlotForm : Form
    {
        PlotSettingsEx ps=null;//声明增强型打印设置对象
        Layout layout=null;//当前布局对象
        public PlotForm()
        {
            InitializeComponent();
            Database  db = HostApplicationServices.WorkingDatabase;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                LayoutManager lm=LayoutManager.Current;//获取当前布局管理器
                //获取当前布局
                ObjectId layoutId=lm.GetLayoutId(lm.CurrentLayout);
                layout = (Layout)layoutId.GetObject(OpenMode.ForRead);
                //获取当前布局名称
                string layoutName=layout.ModelType ? "模型" : layout.LayoutName;
                this.Text = "打印 - " + layoutName;//设置窗口标题
                ps = new PlotSettingsEx(layout);//创建增强型打印设置对象
                trans.Commit();
            }
        }
        private void PlotForm_Load(object sender, EventArgs e)
        {
            //绑定打印机列表框
            this.comboBoxDevice.DataSource = ps.DeviceList;
            this.comboBoxDevice.DataBindings.Add("SelectedItem", ps, "PlotConfigurationName", true, DataSourceUpdateMode.OnPropertyChanged);
            //绑定打印到文件复选框
            this.checkBoxToFile.DataBindings.Add("Checked", ps, "IsPlotToFile", true, DataSourceUpdateMode.OnPropertyChanged);
            this.checkBoxToFile.DataBindings.Add("Enabled", ps, "NoPlotToFile", true, DataSourceUpdateMode.OnPropertyChanged);
            //绑定图纸尺寸组合框
            this.comboBoxMedia.DataBindings.Add("SelectedItem", ps, "CanonicalMediaName", true, DataSourceUpdateMode.OnPropertyChanged);
            //绑定打印范围组合框
            this.comboBoxPlotArea.DataSource = ps.PlotTypeList;
            this.comboBoxPlotArea.DataBindings.Add("SelectedItem", ps, "PlotTypeLocal", true, DataSourceUpdateMode.OnPropertyChanged);
            //绑定X方向打印偏移文本框
            this.textBoxOffsetX.DataBindings.Add("Text", ps, "PlotOriginX", true, DataSourceUpdateMode.OnPropertyChanged);
            this.textBoxOffsetX.DataBindings[0].FormatString = "0.00";
            //绑定Y方向打印偏移文本框
            this.textBoxOffsetY.DataBindings.Add("Text", ps, "PlotOriginY", true, DataSourceUpdateMode.OnPropertyChanged);
            this.textBoxOffsetY.DataBindings[0].FormatString = "0.00";
            //绑定X方向打印偏移单位标签
            this.labelOffsetXUnit.DataBindings.Add("Text", ps, "PlotPaperUnitsLocal", true, DataSourceUpdateMode.OnPropertyChanged);
            //绑定X方向打印偏移单位标签
            this.labelOffsetYUnit.DataBindings.Add("Text", ps, "PlotPaperUnitsLocal", true, DataSourceUpdateMode.OnPropertyChanged);
            //绑定居中打印复选框
            this.checkBoxPlotCentered.DataBindings.Add("Checked", ps, "PlotCentered", true, DataSourceUpdateMode.OnPropertyChanged);
            //绑定打印样式表组合框
            this.comboBoxStyleSheet.DataSource = ps.ColorDependentPlotStyles;
            this.comboBoxStyleSheet.DataBindings.Add("SelectedItem", ps, "CurrentStyleSheet", true, DataSourceUpdateMode.OnPropertyChanged);
            //绑定按样式打印复选框
            this.checkBoxPlotStyles.DataBindings.Add("Checked", ps, "PlotPlotStyles");
            //绑定图形方向为横向的单选按钮
            this.radioButtonHorizontal.DataBindings.Add("Checked", ps, "PlotHorizontal", true, DataSourceUpdateMode.OnPropertyChanged);
            //绑定图形方向为反向打印的单选按钮
            this.checkBoxReverse.DataBindings.Add("Checked", ps, "PlotReverse", true, DataSourceUpdateMode.OnPropertyChanged);
            //绑定布满图纸复选框
            this.checkBoxExtent.DataBindings.Add("Checked", ps, "PlotExtent", true, DataSourceUpdateMode.OnPropertyChanged);
            //绑定打印比例组合框
            this.comboBoxStdScaleType.DataSource = ps.StdScaleTypeList.Values.ToList();
            this.comboBoxStdScaleType.DataBindings.Add("SelectedItem", ps, "StdScaleTypeLocal", true, DataSourceUpdateMode.OnPropertyChanged);
            //绑定图形单位文本框
            this.textBoxDenominator.DataBindings.Add("Text", ps, "Denominator", true, DataSourceUpdateMode.OnPropertyChanged);
            this.textBoxDenominator.DataBindings[0].FormatString = "#.##";
            //绑定图纸单位文本框
            this.textBoxNumerator.DataBindings.Add("Text", ps, "Numerator", true, DataSourceUpdateMode.OnPropertyChanged);
            this.textBoxNumerator.DataBindings[0].FormatString = "#.##";
            //绑定打印单位组合框
            this.comboBoxPlotPaperUnits.DataBindings.Add("SelectedItem", ps, "PlotPaperUnitsLocal", true, DataSourceUpdateMode.OnPropertyChanged);
            this.buttonApply.Enabled = false;//“应用到布局”按钮刚开始为不可用状态
        }

        private void control_ValueChanged(object sender, EventArgs e)
        {
            //当控件的值改变，表示用户修改了打印设置，则“应用到布局”按钮变为可用状态
            this.buttonApply.Enabled = true;
        }
        private void checkBoxPlotCentered_CheckedChanged(object sender, EventArgs e)
        {
            //根据居中打印复选框的选定状态设置打印偏移文本框的可用状态
            this.textBoxOffsetX.Enabled = !this.checkBoxPlotCentered.Checked;
            this.textBoxOffsetY.Enabled = !this.checkBoxPlotCentered.Checked;
            this.buttonApply.Enabled = true;//内容有改变，“应用到布局”按钮可用
        }

        private void checkBoxExtent_CheckedChanged(object sender, EventArgs e)
        {
            //根据布满图纸复选框的选定状态，设置与打印比例有关按钮的可用状态
            this.comboBoxStdScaleType.Enabled = !this.checkBoxExtent.Checked;
            this.textBoxDenominator.Enabled = !this.checkBoxExtent.Checked;
            this.textBoxNumerator.Enabled = !this.checkBoxExtent.Checked;
            this.buttonApply.Enabled = true;//内容有改变，应用到布局按钮可用
        }
        private void buttonPlotWindow_Click(object sender, EventArgs e)
        {
            setPlotWindowArea();//如果打印范围为窗口，则设置窗口的范围
        }
        private void setPlotWindowArea()
        {
            this.Hide();//隐藏窗体
            Editor ed=AcadApp.DocumentManager.MdiActiveDocument.Editor;
            //提示用户输入打印窗口的两个角点
            PromptPointResult result=ed.GetPoint("\n指定第一个角点");
            if (result.Status != PromptStatus.OK) return;
            Point3d basePt=result.Value;
            result = ed.GetCorner("指定对角点", basePt);
            if (result.Status != PromptStatus.OK) return;
            Point3d cornerPt=result.Value;
            //将用户输入的点从UCS坐标转换为DCS坐标
            basePt = basePt.TranslateCoordinates(UCSTools.CoordSystem.UCS, UCSTools.CoordSystem.DCS);
            cornerPt = cornerPt.TranslateCoordinates(UCSTools.CoordSystem.UCS, UCSTools.CoordSystem.DCS);
            //根据DCS坐标下的点创建Extents2d对象，并赋值给表示打印范围对象
            ps.PlotWindowArea = new Extents2d(basePt.X, basePt.Y, cornerPt.X, cornerPt.Y);
            this.Show();//显示窗体
        }

        private void comboBoxDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox_UpdateDataBinding(sender, e);//必须手动调用组合框的绑定程序
            this.comboBoxMedia.DataSource = ps.MediaList;//设置“图纸尺寸列表”组合框的数据源
            //设置“图纸单位列表”组合框的数据源
            this.comboBoxPlotPaperUnits.DataSource = ps.PlotUnitList;
            this.buttonApply.Enabled = true;//内容有改变，“应用到布局”按钮可用
        }

        private void comboBox_UpdateDataBinding(object sender, EventArgs e)
        {
            ComboBox comboBox=sender as ComboBox;
            if (comboBox == null) return;//若控件不是复选框，则返回
            //使用LINQ筛选复选框绑定属性为SelectedItem的绑定对象
            var binds=from Binding b in comboBox.DataBindings
                      where b.PropertyName == "SelectedItem"
                      select b;
            foreach (var bind in binds)
            {
                //读取复选框的“当前选定项”的属性值，将其写入绑定数据源
                bind.WriteValue();
            }
        }

        private void comboBoxPlotArea_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBoxPlotArea.SelectedItem.ToString() == "窗口")//如果打印范围是窗口
            {
                //如果“窗口”按钮不可见
                if (this.buttonPlotWindow.Visible == false)
                {
                    setPlotWindowArea();//设置窗口的范围
                    this.buttonPlotWindow.Visible = true;//“窗口”按钮可见
                }
            }
            else this.buttonPlotWindow.Visible = false;//“窗口”按钮不可见
            //如果打印范围不是布局，则“居中打印”复选框不可用
            this.checkBoxPlotCentered.Enabled = (this.comboBoxPlotArea.SelectedItem.ToString() != "布局");
            comboBox_UpdateDataBinding(sender, e);//必须手动调用组合框的绑定程序
            this.buttonApply.Enabled = true;//内容有改变，“应用到布局”按钮可用
        }
        private void buttonApply_Click(object sender, EventArgs e)
        {
            LayoutManager lm=LayoutManager.Current;//获取当前布局管理器
            //获取当前布局的ObjectId
            ObjectId layoutId=lm.GetLayoutId(lm.CurrentLayout);
            ps.UpdatePlotSettings(layoutId);//更新当前布局的打印设置
            this.buttonApply.Enabled = false;//应用到布局按钮为不可用状态
        }
        private void buttonPreview_Click(object sender, EventArgs e)
        {
            //如果有打印进程在运行，则返回
            if (PlotFactory.ProcessPlotState != ProcessPlotState.NotPlotting) return;
            this.Hide();
            //创建一个打印预览引擎
            PlotEngine previewEngine=PlotFactory.CreatePreviewEngine((int)PreviewEngineFlags.Plot);
            //打印预览当前布局
            PreviewEndPlotStatus status=previewEngine.Plot(layout, ps, null, 1, true, true, false);
            previewEngine.Destroy(); //销毁打印预览引擎
            //如果用户在打印预览对话框中选择了打印
            if (status == PreviewEndPlotStatus.Plot)
            {
                Plot();
            }
            else
                this.Show();
        }

        private void buttonPlot_Click(object sender, EventArgs e)
        {
            Plot();
        }
        public void Plot()
        {
            PlotConfig config=PlotConfigManager.CurrentConfig;
            Document doc=AcadApp.DocumentManager.MdiActiveDocument;
            Editor ed=doc.Editor;
            //获取去除扩展名后的文件名（不含路径）
            string fileName=SymbolUtilityServices.GetSymbolNameFromPathName(doc.Name, "dwg");
            //定义保存文件对话框
            PromptSaveFileOptions opt=new PromptSaveFileOptions("文件名");
            //保存文件对话框的文件扩展名列表
            opt.Filter = "*" + config.DefaultFileExtension + "|*" + config.DefaultFileExtension;
            opt.DialogCaption = "浏览打印文件";//保存文件对话框的标题
            opt.InitialDirectory = @"C:\";//缺省保存目录
            opt.InitialFileName = fileName + "-" + layout.LayoutName;//缺省保存文件名
            //根据保存对话框中用户的选择，获取保存文件名
            PromptFileNameResult result=ed.GetFileNameForSave(opt);
            if (result.Status != PromptStatus.OK) return;
            fileName = result.StringResult;
            this.Hide();
            //为了防止后台打印问题，必须在调用打印API时设置BACKGROUNDPLOT系统变量为0
            short backPlot=(short)AcadApp.GetSystemVariable("BACKGROUNDPLOT");
            AcadApp.SetSystemVariable("BACKGROUNDPLOT", 0);
            PlotEngine plotEngine=PlotFactory.CreatePublishEngine();//创建一个打印引擎            
            //打印当前布局为一个文件，以“图形文件名-布局名”的形式作为文件名
            plotEngine.Plot(layout, ps, fileName, (int)this.numericCopies.Value, false, true, ps.IsPlotToFile);
            plotEngine.Destroy();//销毁打印引擎 
            //恢复BACKGROUNDPLOT系统变量的值
            AcadApp.SetSystemVariable("BACKGROUNDPLOT",backPlot);
            this.Dispose(); //关闭窗体
        }
        private void buttonPlotLayouts_Click(object sender, EventArgs e)
        {
            Document doc=AcadApp.DocumentManager.MdiActiveDocument;
            Database db=HostApplicationServices.WorkingDatabase;
            using (doc.LockDocument())
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {

                List<Layout> layouts=db.GetAllLayouts();
                //如果有打印进程在运行，则返回
                if (PlotFactory.ProcessPlotState != ProcessPlotState.NotPlotting) return;
                this.Hide();
                bool isReadyForPlot=MultiPlotPreView(layouts);
                if (isReadyForPlot)
                {
                    MultiPlot(layouts);
                    this.Dispose();
                }
                else
                    this.Show();
                trans.Commit();
            }
        }
        public bool MultiPlotPreView(List<Layout> layouts)
        {
            int layoutNum=1;//布局序号
            bool isFinished=false;//打印预览是否完成
            bool isReadyForPlot=false;//是否准备打印
            bool isFirst=true;//是否第一次进行打印预览
            while (!isFinished)//如果预览未完成，则一直处于预览状态
            {
                //设置预览状态为打印
                PreviewEngineFlags flags=PreviewEngineFlags.Plot;
                //如果不是第一个布局，则预览状态可以为打印或下一页
                if (layoutNum > 1) flags |= PreviewEngineFlags.PreviousSheet;
                //如果不是最后一个布局，则预览状态可以为打印或下一页
                if (layoutNum < layouts.Count)
                    flags |= PreviewEngineFlags.NextSheet;
                //创建一个打印预览引擎
                PlotEngine previewEngine=PlotFactory.CreatePreviewEngine((int)flags);
                //打印预览当前布局，如果为第一次预览，则显示打印进度框
                PreviewEndPlotStatus status=previewEngine.MPlot(layouts, ps, "", layoutNum, 1, isFirst, false);
                switch (status)//判断打印预览结束时的状态，以切换布局
                {
                    case PreviewEndPlotStatus.Normal://结束预览
                    case PreviewEndPlotStatus.Cancel://取消预览
                        isFinished = true;//预览结束
                        break;
                    case PreviewEndPlotStatus.Previous://上一页
                        layoutNum--;//切换到上一个布局
                        break;
                    case PreviewEndPlotStatus.Next://下一页
                        layoutNum++;//切换到下一个布局
                        break;
                    case PreviewEndPlotStatus.Plot://打印
                        isFinished = true;//预览结束
                        isReadyForPlot = true;//可以打印
                        break;
                }
                previewEngine.Destroy();//销毁打印预览引擎
                isFirst = false;//已经不是第一次预览
            }
            return isReadyForPlot;
        }
        private void MultiPlot(List<Layout> layouts)
        {
            PlotConfig config=PlotConfigManager.CurrentConfig;
            Document doc=AcadApp.DocumentManager.MdiActiveDocument;
            Editor ed=doc.Editor;
            //获取去除扩展名后的文件名（不含路径）
            string fileName=SymbolUtilityServices.GetSymbolNameFromPathName(doc.Name, "dwg");
            //定义保存文件对话框
            PromptSaveFileOptions opt=new PromptSaveFileOptions("文件名");
            //保存文件对话框的文件扩展名列表
            opt.Filter = "*" + config.DefaultFileExtension + "|*" + config.DefaultFileExtension;
            opt.DialogCaption = "浏览打印文件";//保存文件对话框的标题
            opt.InitialDirectory = @"C:\";//缺省保存目录
            opt.InitialFileName = fileName;//缺省保存文件名
            //根据保存对话框中用户的选择，获取保存文件名
            PromptFileNameResult result=ed.GetFileNameForSave(opt);
            if (result.Status != PromptStatus.OK) return;
            fileName = result.StringResult;
            //创建一个打印引擎
            PlotEngine plotEngine=PlotFactory.CreatePublishEngine();
            //打印所有布局为一个多页文件，并以图形文件名的形式作为文件名
            plotEngine.MPlot(layouts, ps, fileName, 0, (int)this.numericCopies.Value, true, ps.IsPlotToFile);
            plotEngine.Destroy();//销毁打印引擎
        }
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();//销毁窗体
        }
    }
}
