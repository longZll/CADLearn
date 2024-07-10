using System;
using System.IO;
using System.Windows.Forms;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
//using Autodesk.AutoCAD.Internal;
using DotNetARX;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;
using AcadWnd = Autodesk.AutoCAD.Windows;
namespace chap10
{
    /// <summary>
    /// 模态对话框
    /// </summary>
    public partial class ModalForm : Form
    {

        string filePath = "";       //外部参照完整文件路径
        string xrefName = "";       //外部参照的文件名
        Point3d insertPoint;        //插入点坐标
        Scale3d scaleFactors;       //缩放因子
        double rotation;            //旋转角度
        bool isOverlay = false;     //是否覆盖型外部参照(否的时候表示附着型外部参照),默认为附着型外部参照
        Document acDoc = AcadApp.DocumentManager.MdiActiveDocument;
        Database acCurDb = AcadApp.DocumentManager.MdiActiveDocument.Database;
        Editor ed = AcadApp.DocumentManager.MdiActiveDocument.Editor;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ModalForm()
        {
            InitializeComponent();
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            
            //清空显示块名的下拉列表框中的内容
            this.comboBoxBlockName.Items.Clear();

            //新建一个打开文件对话框，并设置对话框的标题和显示文件类型为dwg或者dxf
            //TODO:需要修改为打开文件结构树,并选定 文件结构树中的文件
            //AcadWnd.OpenFileDialog dlg = new AcadWnd.OpenFileDialog("选择图形文件", null, "dwg;dxf", null, AcadWnd.OpenFileDialog.OpenFileDialogFlags.AllowMultiple);

            //提示用户输入外部参照文件路径
            PromptOpenFileOptions opt = new PromptOpenFileOptions("请选择外部参照文件:");
            opt.Filter = "图形(*.dwg)|*.dwg|图形(*.dxf)|*.dxf";
            opt.FilterIndex = 0;
            this.filePath = ed.GetFileNameForOpen(opt).StringResult;
            ed.WriteMessage("文件路径:" + this.filePath + "\n");

            //获取外部参照块名，默认为文件名（不含扩展名）
            this.xrefName = System.IO.Path.GetFileNameWithoutExtension(this.filePath);
            ed.WriteMessage("文件名:" + this.xrefName + "\n");

            //在下拉列表框中外部参照名        
            this.comboBoxBlockName.Text = this.xrefName;
            this.labelPathlabelPath.Text = "路径:" + this.filePath;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void comboBoxBlockName_SelectedIndexChanged(object sender, EventArgs e)
        {
            Database db = HostApplicationServices.WorkingDatabase;
            //获取下拉列表框对象
            ComboBox blockNames = sender as ComboBox;
            //获取下拉列表框中当前选择的块名
            string blockName = blockNames.SelectedItem.ToString();
            //开始事务处理
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                //打开块表
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
                //打开名字为下拉列表框中当前选择的块名的块
                BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[blockName], OpenMode.ForRead);

                //如果获取块预览图案成功，则设置图片框中的图案为该预览图案
                if (btr.HasPreviewIcon)
                {
                    //适用于AutoCAD 2008及以下版本
                    //this.pictureBoxBlock.Image = BlockThumbnailHelper.GetBlockThumbanail(btr.ObjectId);
                    this.pictureBoxBlock.Image = btr.PreviewIcon; //适用于AutoCAD 2009及以上版本
                }

                //根据当前选择的块，设置块的单位和比例
                switch (btr.Units)
                {
                    case UnitsValue.Inches:
                        this.textBoxBlockUnit.Text = "英寸";
                        this.textBoxBlockRatio.Text = "25.4";
                        break;
                    case UnitsValue.Meters:
                        this.textBoxBlockUnit.Text = "米";
                        this.textBoxBlockRatio.Text = "1000";
                        break;
                    case UnitsValue.Millimeters:
                        this.textBoxBlockUnit.Text = "毫米";
                        this.textBoxBlockRatio.Text = "1";
                        break;
                    case UnitsValue.Undefined:
                        this.textBoxBlockUnit.Text = "无单位";
                        this.textBoxBlockRatio.Text = "1";
                        break;
                    default:
                        this.textBoxBlockUnit.Text = btr.Units.ToString();
                        this.textBoxBlockRatio.Text = "";
                        break;
                }
            }
        }

        private void checkBoxSameScale_CheckedChanged(object sender, EventArgs e)
        {
            //获取检查框对象
            CheckBox uniformScale = sender as CheckBox;
            //如果检查框对象处于选中状态
            if (uniformScale.Checked == true)
            {
                //Y、Z方向的缩放比例与X一致
                this.textBoxScaleY.Text = this.textBoxScaleX.Text;
                this.textBoxScaleZ.Text = this.textBoxScaleX.Text;
                //Y、Z方向的缩放比例设置文本框不能输入
                this.textBoxScaleY.Enabled = false;
                this.textBoxScaleZ.Enabled = false;
            }
            else
            {
                //Y、Z方向的缩放比例设置文本框可以输入
                this.textBoxScaleY.Enabled = true;
                this.textBoxScaleZ.Enabled = true;
            }
        }

        private void textBoxScaleX_TextChanged(object sender, EventArgs e)
        {
            //如果检查框处于选中状态，则Y、Z方向的缩放比例与X方向一致
            if (this.checkBoxSameScale.Checked == true)
            {
                this.textBoxScaleY.Text = this.textBoxScaleX.Text;
                this.textBoxScaleZ.Text = this.textBoxScaleX.Text;
            }
        }

        private void checkBoxScale_CheckedChanged(object sender, EventArgs e)
        {
            //获取检查框对象
            CheckBox bchecked = sender as CheckBox;
            //如果检查框对象处于选中状态，则不能在设置缩放比例的文本框输入
            if (bchecked.Checked == true)
            {
                this.textBoxScaleX.Enabled = false;
                this.textBoxScaleY.Enabled = false;
                this.textBoxScaleZ.Enabled = false;
            }
            //否则，可以在设置缩放比例的文本框输入
            else
            {
                this.textBoxScaleX.Enabled = true;
                this.textBoxScaleY.Enabled = true;
                this.textBoxScaleZ.Enabled = true;
            }
        }

        private void checkBoxInsertPoint_CheckedChanged(object sender, EventArgs e)
        {
            //获取检查框对象
            CheckBox bchecked = sender as CheckBox;
            //如果检查框对象处于选中状态，则不能在设置插入点的文本框输入
            if (bchecked.Checked == true)
            {
                this.textBoxInsertPointX.Enabled = false;
                this.textBoxInsertPointY.Enabled = false;
                this.textBoxInsertPointZ.Enabled = false;
            }
            //否则，可以在设置插入点的文本框输入
            else
            {
                this.textBoxInsertPointX.Enabled = true;
                this.textBoxInsertPointY.Enabled = true;
                this.textBoxInsertPointZ.Enabled = true;
            }
        }


        private void checkBoxRotate_CheckedChanged(object sender, EventArgs e)
        {
            //获取检查框对象
            CheckBox bchecked = sender as CheckBox;
            //如果检查框对象处于选中状态，则不能在设置旋转角度的文本框输入
            if (bchecked.Checked == true)
            {
                this.textBoxRotateAngle.Enabled = false;
            }
            //否则，可以在设置旋转角度的文本框输入
            else
            {
                this.textBoxRotateAngle.Enabled = true;
            }
        }

        private void ModalForm_Load(object sender, EventArgs e)
        {
            //初始状态下图形中没有块，所以显示为无单位字样和比例为1
            this.textBoxBlockUnit.Text = "无单位";
            this.textBoxBlockRatio.Text = "1";
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            //获取块参照插入点坐标
            double insertPointX = Convert.ToDouble(this.textBoxInsertPointX.Text);
            double insertPointY = Convert.ToDouble(this.textBoxInsertPointY.Text);
            double insertPointZ = Convert.ToDouble(this.textBoxInsertPointZ.Text);
            this.insertPoint = new Point3d(insertPointX, insertPointY, insertPointZ);
            //获取块参照的缩放比例
            double scaleX = Convert.ToDouble(this.textBoxScaleX.Text);
            double scaleY = Convert.ToDouble(this.textBoxScaleY.Text);
            double scaleZ = Convert.ToDouble(this.textBoxScaleZ.Text);
            this.scaleFactors = new Scale3d(scaleX, scaleY, scaleZ);

            //获取块参照的旋转角度
            this.rotation = Convert.ToDouble(this.textBoxRotateAngle.Text);
            //关闭窗体
            this.Close();

            Database db = HostApplicationServices.WorkingDatabase;

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                //检查外部参照块表记录是否已存在
                XrefGraph xrefGraph = acCurDb.GetHostDwgXrefGraph(false);
                bool xrefExists = false;

                for (int i = 0; i < xrefGraph.NumNodes; i++)
                {
                    XrefGraphNode xrefNode = xrefGraph.GetXrefNode(i);
                    if (xrefNode.Name == xrefName)
                    {
                        xrefExists = true;
                        break;
                    }
                }

                //检查是否已经存在一个具有相同名称的外部参照。如果外部参照不存在 则会附加外部参照并创建一个块参照以将其添加到模型空间中
                if (!xrefExists)
                {
                    ed.WriteMessage("不存在同名参照文件!");
                    AttachXref(db, filePath, xrefName, insertPoint, scaleFactors, rotation, isOverlay);
                }
                else
                {
                    ed.WriteMessage("存在同名参照文件,附加外部参照失败!");
                }

                trans.Commit();
            }
        }

        private void buttonHelp_Click(object sender, EventArgs e)
        {
            string fullHelpPath = Path.Combine(Tools.GetCurrentPath(), "acad_acr.chm");
            AcadApp.InvokeHelp(fullHelpPath, "d0e53397");
        }

        private void pictureBoxBlock_Click(object sender, EventArgs e)
        {

        }

        private ObjectId AttachXref(Database db, string fileName, string blockName, Point3d insertionPoint, Scale3d scaleFactors, double rotation, bool isOverlay)
        {
            ObjectId xrefId = ObjectId.Null;        //外部参照的Id

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                try
                {
                    //选择以覆盖的方式插入外部参照
                    if (isOverlay) xrefId = db.OverlayXref(fileName, blockName);
                    //选择以附着的方式插入外部参照
                    else xrefId = db.AttachXref(fileName, blockName);

                    // 检查是否成功附加
                    if (!xrefId.IsNull)
                    {
                        //创建块参照
                        BlockReference bref = new BlockReference(insertionPoint, xrefId)
                        {
                            ScaleFactors = scaleFactors,
                            Rotation = rotation
                        };

                        //将块参照添加到模型空间
                        BlockTable bt = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;
                        BlockTableRecord btr = acTrans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                        btr.AppendEntity(bref);
                        acTrans.AddNewlyCreatedDBObject(bref, true);

                        //提交事务
                        acTrans.Commit();

                        ed.WriteMessage("\n外部参照附加成功!");
                    }
                    else
                    {
                        ed.WriteMessage("\n附加外部参照失败");
                    }

                }
                catch (Autodesk.AutoCAD.Runtime.Exception ex)
                {
                    //捕获AutoCAD运行时异常
                    ed.WriteMessage($"\n添加外部参照出现异常: {ex.Message}");
                    acTrans.Abort(); //回滚事务
                }

            }
            return xrefId;
        }




    }
}

