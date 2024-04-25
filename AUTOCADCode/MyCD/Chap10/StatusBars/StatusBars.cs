using System;
using System.Linq;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;
using Autodesk.AutoCAD.DatabaseServices;
using System.Collections.Generic;
using Autodesk.AutoCAD.EditorInput;
namespace StatusBars
{
    public class StatusBars
    {
        [CommandMethod("CreateAppPane")]
        public void AddApplicationPane()
        {
            //����һ�����򴰸����
            Pane appPaneButton = new Pane();
            //���ô��������
            appPaneButton.Enabled = true;
            appPaneButton.Visible = true;
            //���ô����ʼ״̬�ǵ�����
            appPaneButton.Style = PaneStyles.Normal;
            //���ô���ı���
            appPaneButton.Text = "���򴰸�";
            //��ʾ�������ʾ��Ϣ
            appPaneButton.ToolTipText = "��ӭ������.net�����磡";
            //���MouseDown�¼�������걻����ʱ����
            appPaneButton.MouseDown += OnAppMouseDown;
            //�Ѵ�����ӵ�AutoCAD��״̬������
            Application.StatusBar.Panes.Add(appPaneButton);
        }
        void OnAppMouseDown(object sender, StatusBarMouseDownEventArgs e)
        {
            //��ȡ����ť����
            Pane paneButton = (Pane)sender;
            string alertMessage;
            //�������Ĳ������������򷵻�
            if (e.Button != System.Windows.Forms.MouseButtons.Left)
            {
                return;
            }
            //�л�����ť��״̬
            if (paneButton.Style == PaneStyles.PopOut)//�������ť�ǵ����ģ����л�Ϊ����
            {
                paneButton.Style = PaneStyles.Normal;
                alertMessage = "���򴰸�ť������";
            }
            else
            {
                paneButton.Style = PaneStyles.PopOut;
                alertMessage = "���򴰸�ťû�б�����";
            }
            //����״̬���Է�ӳ����ť��״̬�仯
            Application.StatusBar.Update();
            //��ʾ��ӳ����ť�仯����Ϣ
            Application.ShowAlertDialog(alertMessage);
        }
        [CommandMethod("StatusBarBalloon")]
        public void StatusBarBalloon()
        {
            Document doc=Application.DocumentManager.MdiActiveDocument;
            Editor ed=doc.Editor;
            //��ʾ�û�������Ҫ�ı���ɫ�Ķ���
            ObjectId id=ed.GetEntity("��ѡ����Ҫ�ı���ɫ�Ķ���").ObjectId;
            TrayItem trayItem=new TrayItem();//�½�һ��������Ŀ
            trayItem.ToolTipText = "�ı�������ɫ";//������Ŀ����ʾ�ַ�
            //������Ŀ��ͼ��
            trayItem.Icon = doc.StatusBar.TrayItems[0].Icon;
            //��������Ŀ��ӵ�AutoCAD��״̬������
            Application.StatusBar.TrayItems.Add(trayItem);
            //�½�һ������֪ͨ����
            TrayItemBubbleWindow window=new TrayItemBubbleWindow();
            window.Title = "�ı�������ɫ";//���ݴ��ڵı���
            window.HyperText = "������ɫ��Ϊ��ɫ";//���ݴ��ڵ������ı�
            window.Text = "����ı�������ɫ";//���ݴ��ڵ������ı�
            window.IconType = IconType.Information;//���ݴ��ڵ�ͼ������
            trayItem.ShowBubbleWindow(window);//����������ʾ���ݴ���
            Application.StatusBar.Update();//����״̬��
            //ע�����ݴ��ڹر��¼�
            window.Closed += (sender, e) =>
            {
                //����û���������ݴ����е����ӣ������ö������ɫΪ��ɫ
                if (e.CloseReason == TrayItemBubbleWindowCloseReason.HyperlinkClicked)
                {
                    using (doc.LockDocument())
                    using (Transaction trans=doc.TransactionManager.StartTransaction())
                    {
                        Entity ent=(Entity)trans.GetObject(id, OpenMode.ForWrite);
                        ent.ColorIndex = 1;
                        trans.Commit();
                    }
                }
                //���ݴ��ڹرպ󣬽����̴�״̬��ɾ��
                Application.StatusBar.TrayItems.Remove(trayItem);
                Application.StatusBar.Update();//����״̬��
            };
        }
    }
}
