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
    /// ģ̬�Ի���
    /// </summary>
    public partial class ModalForm : Form
    {

        string filePath = "";       //�ⲿ���������ļ�·��
        string xrefName = "";       //�ⲿ���յ��ļ���
        Point3d insertPoint;        //���������
        Scale3d scaleFactors;       //��������
        double rotation;            //��ת�Ƕ�
        bool isOverlay = false;     //�Ƿ񸲸����ⲿ����(���ʱ���ʾ�������ⲿ����),Ĭ��Ϊ�������ⲿ����
        Document acDoc = AcadApp.DocumentManager.MdiActiveDocument;
        Database acCurDb = AcadApp.DocumentManager.MdiActiveDocument.Database;
        Editor ed = AcadApp.DocumentManager.MdiActiveDocument.Editor;

        /// <summary>
        /// ���캯��
        /// </summary>
        public ModalForm()
        {
            InitializeComponent();
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            
            //�����ʾ�����������б���е�����
            this.comboBoxBlockName.Items.Clear();

            //�½�һ�����ļ��Ի��򣬲����öԻ���ı������ʾ�ļ�����Ϊdwg����dxf
            //TODO:��Ҫ�޸�Ϊ���ļ��ṹ��,��ѡ�� �ļ��ṹ���е��ļ�
            //AcadWnd.OpenFileDialog dlg = new AcadWnd.OpenFileDialog("ѡ��ͼ���ļ�", null, "dwg;dxf", null, AcadWnd.OpenFileDialog.OpenFileDialogFlags.AllowMultiple);

            //��ʾ�û������ⲿ�����ļ�·��
            PromptOpenFileOptions opt = new PromptOpenFileOptions("��ѡ���ⲿ�����ļ�:");
            opt.Filter = "ͼ��(*.dwg)|*.dwg|ͼ��(*.dxf)|*.dxf";
            opt.FilterIndex = 0;
            this.filePath = ed.GetFileNameForOpen(opt).StringResult;
            ed.WriteMessage("�ļ�·��:" + this.filePath + "\n");

            //��ȡ�ⲿ���տ�����Ĭ��Ϊ�ļ�����������չ����
            this.xrefName = System.IO.Path.GetFileNameWithoutExtension(this.filePath);
            ed.WriteMessage("�ļ���:" + this.xrefName + "\n");

            //�������б�����ⲿ������        
            this.comboBoxBlockName.Text = this.xrefName;
            this.labelPathlabelPath.Text = "·��:" + this.filePath;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void comboBoxBlockName_SelectedIndexChanged(object sender, EventArgs e)
        {
            Database db = HostApplicationServices.WorkingDatabase;
            //��ȡ�����б�����
            ComboBox blockNames = sender as ComboBox;
            //��ȡ�����б���е�ǰѡ��Ŀ���
            string blockName = blockNames.SelectedItem.ToString();
            //��ʼ������
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                //�򿪿��
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
                //������Ϊ�����б���е�ǰѡ��Ŀ����Ŀ�
                BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[blockName], OpenMode.ForRead);

                //�����ȡ��Ԥ��ͼ���ɹ���������ͼƬ���е�ͼ��Ϊ��Ԥ��ͼ��
                if (btr.HasPreviewIcon)
                {
                    //������AutoCAD 2008�����°汾
                    //this.pictureBoxBlock.Image = BlockThumbnailHelper.GetBlockThumbanail(btr.ObjectId);
                    this.pictureBoxBlock.Image = btr.PreviewIcon; //������AutoCAD 2009�����ϰ汾
                }

                //���ݵ�ǰѡ��Ŀ飬���ÿ�ĵ�λ�ͱ���
                switch (btr.Units)
                {
                    case UnitsValue.Inches:
                        this.textBoxBlockUnit.Text = "Ӣ��";
                        this.textBoxBlockRatio.Text = "25.4";
                        break;
                    case UnitsValue.Meters:
                        this.textBoxBlockUnit.Text = "��";
                        this.textBoxBlockRatio.Text = "1000";
                        break;
                    case UnitsValue.Millimeters:
                        this.textBoxBlockUnit.Text = "����";
                        this.textBoxBlockRatio.Text = "1";
                        break;
                    case UnitsValue.Undefined:
                        this.textBoxBlockUnit.Text = "�޵�λ";
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
            //��ȡ�������
            CheckBox uniformScale = sender as CheckBox;
            //������������ѡ��״̬
            if (uniformScale.Checked == true)
            {
                //Y��Z��������ű�����Xһ��
                this.textBoxScaleY.Text = this.textBoxScaleX.Text;
                this.textBoxScaleZ.Text = this.textBoxScaleX.Text;
                //Y��Z��������ű��������ı���������
                this.textBoxScaleY.Enabled = false;
                this.textBoxScaleZ.Enabled = false;
            }
            else
            {
                //Y��Z��������ű��������ı����������
                this.textBoxScaleY.Enabled = true;
                this.textBoxScaleZ.Enabled = true;
            }
        }

        private void textBoxScaleX_TextChanged(object sender, EventArgs e)
        {
            //���������ѡ��״̬����Y��Z��������ű�����X����һ��
            if (this.checkBoxSameScale.Checked == true)
            {
                this.textBoxScaleY.Text = this.textBoxScaleX.Text;
                this.textBoxScaleZ.Text = this.textBoxScaleX.Text;
            }
        }

        private void checkBoxScale_CheckedChanged(object sender, EventArgs e)
        {
            //��ȡ�������
            CheckBox bchecked = sender as CheckBox;
            //������������ѡ��״̬���������������ű������ı�������
            if (bchecked.Checked == true)
            {
                this.textBoxScaleX.Enabled = false;
                this.textBoxScaleY.Enabled = false;
                this.textBoxScaleZ.Enabled = false;
            }
            //���򣬿������������ű������ı�������
            else
            {
                this.textBoxScaleX.Enabled = true;
                this.textBoxScaleY.Enabled = true;
                this.textBoxScaleZ.Enabled = true;
            }
        }

        private void checkBoxInsertPoint_CheckedChanged(object sender, EventArgs e)
        {
            //��ȡ�������
            CheckBox bchecked = sender as CheckBox;
            //������������ѡ��״̬�����������ò������ı�������
            if (bchecked.Checked == true)
            {
                this.textBoxInsertPointX.Enabled = false;
                this.textBoxInsertPointY.Enabled = false;
                this.textBoxInsertPointZ.Enabled = false;
            }
            //���򣬿��������ò������ı�������
            else
            {
                this.textBoxInsertPointX.Enabled = true;
                this.textBoxInsertPointY.Enabled = true;
                this.textBoxInsertPointZ.Enabled = true;
            }
        }


        private void checkBoxRotate_CheckedChanged(object sender, EventArgs e)
        {
            //��ȡ�������
            CheckBox bchecked = sender as CheckBox;
            //������������ѡ��״̬��������������ת�Ƕȵ��ı�������
            if (bchecked.Checked == true)
            {
                this.textBoxRotateAngle.Enabled = false;
            }
            //���򣬿�����������ת�Ƕȵ��ı�������
            else
            {
                this.textBoxRotateAngle.Enabled = true;
            }
        }

        private void ModalForm_Load(object sender, EventArgs e)
        {
            //��ʼ״̬��ͼ����û�п飬������ʾΪ�޵�λ�����ͱ���Ϊ1
            this.textBoxBlockUnit.Text = "�޵�λ";
            this.textBoxBlockRatio.Text = "1";
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            //��ȡ����ղ��������
            double insertPointX = Convert.ToDouble(this.textBoxInsertPointX.Text);
            double insertPointY = Convert.ToDouble(this.textBoxInsertPointY.Text);
            double insertPointZ = Convert.ToDouble(this.textBoxInsertPointZ.Text);
            this.insertPoint = new Point3d(insertPointX, insertPointY, insertPointZ);
            //��ȡ����յ����ű���
            double scaleX = Convert.ToDouble(this.textBoxScaleX.Text);
            double scaleY = Convert.ToDouble(this.textBoxScaleY.Text);
            double scaleZ = Convert.ToDouble(this.textBoxScaleZ.Text);
            this.scaleFactors = new Scale3d(scaleX, scaleY, scaleZ);

            //��ȡ����յ���ת�Ƕ�
            this.rotation = Convert.ToDouble(this.textBoxRotateAngle.Text);
            //�رմ���
            this.Close();

            Database db = HostApplicationServices.WorkingDatabase;

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                //����ⲿ���տ���¼�Ƿ��Ѵ���
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

                //����Ƿ��Ѿ�����һ��������ͬ���Ƶ��ⲿ���ա�����ⲿ���ղ����� ��ḽ���ⲿ���ղ�����һ��������Խ�����ӵ�ģ�Ϳռ���
                if (!xrefExists)
                {
                    ed.WriteMessage("������ͬ�������ļ�!");
                    AttachXref(db, filePath, xrefName, insertPoint, scaleFactors, rotation, isOverlay);
                }
                else
                {
                    ed.WriteMessage("����ͬ�������ļ�,�����ⲿ����ʧ��!");
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
            ObjectId xrefId = ObjectId.Null;        //�ⲿ���յ�Id

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                try
                {
                    //ѡ���Ը��ǵķ�ʽ�����ⲿ����
                    if (isOverlay) xrefId = db.OverlayXref(fileName, blockName);
                    //ѡ���Ը��ŵķ�ʽ�����ⲿ����
                    else xrefId = db.AttachXref(fileName, blockName);

                    // ����Ƿ�ɹ�����
                    if (!xrefId.IsNull)
                    {
                        //���������
                        BlockReference bref = new BlockReference(insertionPoint, xrefId)
                        {
                            ScaleFactors = scaleFactors,
                            Rotation = rotation
                        };

                        //���������ӵ�ģ�Ϳռ�
                        BlockTable bt = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;
                        BlockTableRecord btr = acTrans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                        btr.AppendEntity(bref);
                        acTrans.AddNewlyCreatedDBObject(bref, true);

                        //�ύ����
                        acTrans.Commit();

                        ed.WriteMessage("\n�ⲿ���ո��ӳɹ�!");
                    }
                    else
                    {
                        ed.WriteMessage("\n�����ⲿ����ʧ��");
                    }

                }
                catch (Autodesk.AutoCAD.Runtime.Exception ex)
                {
                    //����AutoCAD����ʱ�쳣
                    ed.WriteMessage($"\n����ⲿ���ճ����쳣: {ex.Message}");
                    acTrans.Abort(); //�ع�����
                }

            }
            return xrefId;
        }




    }
}

