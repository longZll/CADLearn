using System;
using System.IO;
using System.Windows.Forms;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Internal;
using DotNetARX;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;
using AcadWnd = Autodesk.AutoCAD.Windows;
namespace chap10
{
    public partial class ModalForm : Form
    {
        public ModalForm()
        {
            InitializeComponent();
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            Database db = HostApplicationServices.WorkingDatabase;
            //�����ʾ�����������б���е�����
            this.comboBoxBlockName.Items.Clear();
            //�½�һ�����ļ��Ի��򣬲����öԻ���ı������ʾ�ļ�����Ϊdwg����dxf            
            AcadWnd.OpenFileDialog dlg = new AcadWnd.OpenFileDialog("ѡ��ͼ���ļ�", null, "dwg;dxf", null, AcadWnd.OpenFileDialog.OpenFileDialogFlags.AllowMultiple);
            //����򿪶Ի���ɹ�
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //��ʾ��ѡ���ļ���·��
                this.labelPathlabelPath.Text = "·��:  " + dlg.Filename;
                //������ѡ���ļ��еĿ�
                db.ImportBlocksFromDwg(dlg.Filename);
                //��ʼ������
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    //�򿪿��
                    BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
                    //ѭ����������еĿ���¼
                    foreach (ObjectId blockRecordId in bt)
                    {
                        //�򿪿���¼����
                        BlockTableRecord btr = (BlockTableRecord)trans.GetObject(blockRecordId, OpenMode.ForRead);
                        //�������б����ֻ�����������ͷǲ��ֿ������
                        if (!btr.IsAnonymous && !btr.IsLayout)
                            this.comboBoxBlockName.Items.Add(btr.Name);
                    }
                }
                //�������б������ʾ��ĸ˳�����ڵ�һ���Ŀ���
                if (this.comboBoxBlockName.Items.Count > 0)
                    this.comboBoxBlockName.Text = this.comboBoxBlockName.Items[0].ToString();
            }
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
                    this.pictureBoxBlock.Image = BlockThumbnailHelper.GetBlockThumbanail(btr.ObjectId);
                    //this.pictureBoxBlock.Image = btr.PreviewIcon; //������AutoCAD 2009�����ϰ汾
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
            Point3d insertPoint = new Point3d(insertPointX, insertPointY, insertPointZ);
            //��ȡ����յ����ű���
            double scaleX = Convert.ToDouble(this.textBoxScaleX.Text);
            double scaleY = Convert.ToDouble(this.textBoxScaleY.Text);
            double scaleZ = Convert.ToDouble(this.textBoxScaleZ.Text);
            Scale3d scale = new Scale3d(scaleX, scaleY, scaleZ);
            //��ȡ����յ���ת�Ƕ�
            double rotationAngle = Convert.ToDouble(this.textBoxRotateAngle.Text);
            //�رմ���
            this.Close();
            Database db=HostApplicationServices.WorkingDatabase;
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                ObjectId id=db.Clayer; //��ǰ���Id
                // ��ȡ��ǰ��
                LayerTableRecord ltr=(LayerTableRecord)id.GetObject(OpenMode.ForRead);
                ObjectId model=db.GetModelSpaceId(); // ��ȡģ�Ϳռ��Id
                // �ڵ�ǰģ�Ϳռ��������
                model.InsertBlockReference(ltr.Name, this.comboBoxBlockName.Text, insertPoint, scale, rotationAngle);
            }
        }

        private void buttonHelp_Click(object sender, EventArgs e)
        {
            string fullHelpPath = Path.Combine(Tools.GetCurrentPath(), "acad_acr.chm");
            AcadApp.InvokeHelp(fullHelpPath, "d0e53397");
        }
    }
}

