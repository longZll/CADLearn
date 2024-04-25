//===================================================================================
//  创建时间  :  2008/03/20
//  类名      :  FreeFly.ARX.CommandClass
//  功能      :  在这个类中初始化，并注册所有要定义的命令
//  测试目录  :   
//  创建者    : 龙城木头
//  测试阶段  : 测试通过
//===================================================================================

using System;
using System.Windows.Forms;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;

[assembly: ExtensionApplication(typeof(FreeFly.ARX.CommandClass))]
[assembly: CommandClass(typeof(FreeFly.ARX.CommandClass))]

namespace FreeFly.ARX
{
    class CommandClass : Autodesk.AutoCAD.Runtime.IExtensionApplication
    {
        public void Initialize()
        {
            DocStatusBar.myStatusBar();
          
        }
        public void Terminate()
        {
        }
        #region 这里添加自己的命令定义

        #endregion
    }
    public class DocStatusBar
    {
        /// <summary>
        /// 用于存储文档与按钮的对应关系
        /// </summary>

        private class DocListNode
        {
            private int mIndex;       //按钮的顺序
            private Document mDoc;    //对应的文档
            public int Index
            {
                get { return mIndex; }
                set { mIndex = value; }
            }
            public Document Doc
            {
                get { return mDoc; }
                set { mDoc = value; }
            }
            public DocListNode(int index, Document doc)
            {
                mIndex = index;
                mDoc = doc;
            }
        }
        /// <summary>
        /// 用于存储文档集合的状态
        /// </summary>
        private class DocCollectionStatus
        {
            public static int DocCount = 0;                //已经打开的文档数目
            public static int ActiveDoc = 0;               //当前活动的文档
        }
        private static bool IsFirst = true;                 //记录该状态栏按钮是否第一次加载
        //private static int MaxCount = 7;                  //允许显示的最多细节数目,最后修改，增加锁定以保证稳定。
        private static string FirstPaneName = "散步的木头";       //标志位显示的文本
        private static int FirstPaneWidth = 30;             //标志位的宽度
        private static int PaneMaxWidth = 200;              //按钮的最大宽度
        private static int PaneMinWidth = 50;               //按钮的最小宽度
        private static int WillDestroyedDoc = 0;            //即将要关闭的文档
        private static LinkedList<DocListNode> doclist = new LinkedList<DocListNode>();//将来修改为ArrayList类型试试
        /// <summary>
        /// 增加每个文档的按钮
        /// </summary>
        //应该在最前面那个标志按钮上加标志图像。
        public static void myStatusBar()
        {
            if (IsFirst)
            {
                Autodesk.AutoCAD.ApplicationServices.DocumentCollection DocCollection = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager;
                try
                {
                    InsertStatusBarItem(0, FirstPaneName, "", Autodesk.AutoCAD.Windows.PaneStyles.NoBorders, FirstPaneWidth, FirstPaneWidth);//修改以使界面明朗
                    int i = 1;
                    foreach (Document Doc in DocCollection)
                    {
                        Autodesk.AutoCAD.Windows.PaneStyles myStyle;
                        if (Doc.IsActive == true)
                        {
                            myStyle = Autodesk.AutoCAD.Windows.PaneStyles.Normal;
                            DocCollectionStatus.ActiveDoc = i;
                        }
                        else
                        {
                            myStyle = Autodesk.AutoCAD.Windows.PaneStyles.PopOut;
                        }
                        InsertStatusBarItem(i, GetFileName(Doc.Name), Doc.Name, myStyle, PaneMaxWidth, PaneMinWidth);
                        doclist.AddLast(new DocListNode(i, Doc));//doclist.AddLast(CurDocListNode);//传递静态的变量会出错。
                        i += 1;
                        DocCollectionStatus.DocCount += 1;
                        Doc.BeginDocumentClose += new DocumentBeginCloseEventHandler(Doc_BeginDocumentClose);
                        Doc.Database.SaveComplete += new DatabaseIOEventHandler(Database_SaveComplete);
                    }
                    DocCollection.DocumentActivated += new DocumentCollectionEventHandler(DocCollection_DocumentActivated);
                    DocCollection.DocumentCreated += new DocumentCollectionEventHandler(DocCollection_DocumentCreated);
                    DocCollection.DocumentDestroyed += new DocumentDestroyedEventHandler(DocCollection_DocumentDestroyed);
                    IsFirst = false;
                }
                catch
                {
                    MessageBox.Show("AddStatusBarToDoc方法中出错！");
                }
            }
        }

        /// <summary>
        /// 在每个文档中增加一个按钮
        /// </summary>
        private static void InsertStatusBarItem(int index, string paneText, string paneToolTipText, Autodesk.AutoCAD.Windows.PaneStyles myStyle, int MaxWidth, int MinWidth)
        {
            Autodesk.AutoCAD.ApplicationServices.DocumentCollection DocCollection = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager;
            try
            {
                foreach (Document Doc in DocCollection)
                {
                    Autodesk.AutoCAD.Windows.StatusBar myStatus = Doc.StatusBar;
                    Autodesk.AutoCAD.Windows.PaneCollection DocPaneColletion = myStatus.Panes;
                    Autodesk.AutoCAD.Windows.Pane myPane = new Autodesk.AutoCAD.Windows.Pane();
                    myPane.Style = myStyle;
                    myPane.MaximumWidth = MaxWidth;
                    myPane.MinimumWidth = MinWidth;
                    myPane.Text = paneText;
                    myPane.ToolTipText = paneToolTipText;
                    myPane.MouseDown += new Autodesk.AutoCAD.Windows.StatusBarMouseDownEventHandler(StatusBar_MouseDown);
                    DocPaneColletion.Insert(index, myPane);
                    myStatus.Update();
                }
            }
            catch
            {
                MessageBox.Show("InsertStatusBarItem方法中出错！");
            }
        }
        /// <summary>
        /// 新建打开文档
        /// </summary>
        private static void DocCollection_DocumentCreated(object sender, DocumentCollectionEventArgs e)
        {
            //MessageBox.Show("sender = " + sender.ToString());
            Autodesk.AutoCAD.ApplicationServices.DocumentCollection DocCollection = (Autodesk.AutoCAD.ApplicationServices.DocumentCollection)sender;
            Document CurDoc = DocCollection.MdiActiveDocument;
            try
            {
                #region 给新文档增加之前的按钮。
                Autodesk.AutoCAD.Windows.StatusBar myStatus = CurDoc.StatusBar;
                Autodesk.AutoCAD.Windows.PaneCollection DocPaneColletion = myStatus.Panes;
                Autodesk.AutoCAD.Windows.Pane myPane = new Autodesk.AutoCAD.Windows.Pane();
                myPane.Style = Autodesk.AutoCAD.Windows.PaneStyles.NoBorders;
                //先添加前面那个标志位按钮
                myPane.MaximumWidth = FirstPaneWidth;
                myPane.MinimumWidth = FirstPaneWidth;
                myPane.Text = FirstPaneName;
                myPane.ToolTipText = "";
                DocPaneColletion.Insert(0, myPane);
                foreach (DocListNode mDoc in doclist)
                {
                    //注意要重新初始变量，不能与前面共用一个变量，会引起引用同一个对象
                    myPane = new Autodesk.AutoCAD.Windows.Pane();
                    if (mDoc.Index != DocCollectionStatus.ActiveDoc)
                    {
                        myPane.Style = Autodesk.AutoCAD.Windows.PaneStyles.PopOut;
                    }
                    else
                    {
                        myPane.Style = Autodesk.AutoCAD.Windows.PaneStyles.Normal;
                    }
                    myPane.MaximumWidth = PaneMaxWidth;
                    myPane.MinimumWidth = PaneMinWidth;
                    myPane.Text = GetFileName(mDoc.Doc.Name);
                    myPane.ToolTipText = mDoc.Doc.Name;
                    myPane.MouseDown += new Autodesk.AutoCAD.Windows.StatusBarMouseDownEventHandler(StatusBar_MouseDown);
                    DocPaneColletion.Insert(mDoc.Index, myPane);
                }
                myStatus.Update();
                #endregion
                #region 在各文档增加新文档的按钮，并修改文档链表数据，设置新文档为当前文档。
                DocCollectionStatus.DocCount += 1;
                InsertStatusBarItem(DocCollectionStatus.DocCount, GetFileName(CurDoc.Name), CurDoc.Name, Autodesk.AutoCAD.Windows.PaneStyles.PopOut, PaneMaxWidth, PaneMinWidth);
                doclist.AddLast(new DocListNode(DocCollectionStatus.DocCount, CurDoc));
                int LastDoc = DocCollectionStatus.ActiveDoc;
                DocCollectionStatus.ActiveDoc = DocCollectionStatus.DocCount;
                foreach (Document Doc in DocCollection)
                {
                    myStatus = Doc.StatusBar;
                    DocPaneColletion = myStatus.Panes;
                    DocPaneColletion[LastDoc].Style = Autodesk.AutoCAD.Windows.PaneStyles.PopOut;
                    DocPaneColletion[DocCollectionStatus.ActiveDoc].Style = Autodesk.AutoCAD.Windows.PaneStyles.Normal;
                    myStatus.Update();
                }
                CurDoc.BeginDocumentClose += new DocumentBeginCloseEventHandler(Doc_BeginDocumentClose);
                CurDoc.Database.SaveComplete += new DatabaseIOEventHandler(Database_SaveComplete);
                #endregion
            }
            catch
            {
                MessageBox.Show("DocCollection_DocumentCreated事件中出错！");
            }
        }

        /// <summary>
        /// 由选取菜单、打开、关闭文档而改变活动文档引起的事件处理，必须保证发生这个事件以前文档状态链表更新过。
        /// </summary>
        private static void DocCollection_DocumentActivated(object sender, DocumentCollectionEventArgs e)
        {
            Autodesk.AutoCAD.ApplicationServices.DocumentCollection DocCollection = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager;
            Document CurDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            try
            {
                foreach (DocListNode mDoc in doclist)
                {
                    if (mDoc.Doc.Equals(CurDoc))
                    {
                        if (mDoc.Index != DocCollectionStatus.ActiveDoc)
                        {
                            int LastDoc = DocCollectionStatus.ActiveDoc;
                            DocCollectionStatus.ActiveDoc = mDoc.Index;
                            foreach (Document Doc1 in DocCollection)
                            {
                                Autodesk.AutoCAD.Windows.StatusBar myStatus = Doc1.StatusBar;
                                Autodesk.AutoCAD.Windows.PaneCollection DocPaneColletion = myStatus.Panes;
                                DocPaneColletion[LastDoc].Style = Autodesk.AutoCAD.Windows.PaneStyles.PopOut;
                                DocPaneColletion[DocCollectionStatus.ActiveDoc].Style = Autodesk.AutoCAD.Windows.PaneStyles.Normal;
                                myStatus.Update();
                            }
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show("DocCollection_DocumentActivated事件中出错！");
            }
        }
        /// <summary>
        /// 包括正常关闭文档的事件，以及打开文档引起临时文档的关闭事件，注意在关闭最后一个文档时候的正确性。
        /// 必须在这个事件中释放对要关闭的文档的引用，否则发生错误，另外注意索引的修改，防止地址越界。
        /// </summary>
        private static void Doc_BeginDocumentClose(object sender, DocumentBeginCloseEventArgs e)
        {
            Autodesk.AutoCAD.ApplicationServices.DocumentCollection DocCollection = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager;
            try
            {
                Document CurDoc = (Document)sender;
                DocListNode BeDelNode = new DocListNode(0, null);
                bool FindIt = false;
                foreach (DocListNode mDoc in doclist)
                {
                    if (mDoc.Doc.Equals(CurDoc))
                    {
                        WillDestroyedDoc = mDoc.Index;
                        mDoc.Doc = null;
                        BeDelNode = mDoc;
                        FindIt = true;
                    }
                    else
                    {
                        if (FindIt)
                        {
                            mDoc.Index -= 1;
                        }
                    }
                }
                doclist.Remove(BeDelNode);
                Autodesk.AutoCAD.Windows.StatusBar myStatus;
                foreach (Document Doc in DocCollection)
                {
                    myStatus = Doc.StatusBar;
                    Autodesk.AutoCAD.Windows.PaneCollection DocPaneColletion = myStatus.Panes;
                    Autodesk.AutoCAD.Windows.Pane myPane = DocPaneColletion[WillDestroyedDoc];
                    myPane.MouseDown -= new Autodesk.AutoCAD.Windows.StatusBarMouseDownEventHandler(StatusBar_MouseDown);
                    myPane = null;
                    DocPaneColletion.RemoveAt(WillDestroyedDoc);
                    myStatus.Update();
                }
                DocCollectionStatus.DocCount -= 1;
                myStatus = CurDoc.StatusBar;
                for (int i = 0; i <= DocCollectionStatus.DocCount; i++)
                {
                    myStatus.Panes.RemoveAt(0);
                }
                CurDoc.Database.SaveComplete -= new DatabaseIOEventHandler(Database_SaveComplete);
                CurDoc.BeginDocumentClose -= new DocumentBeginCloseEventHandler(Doc_BeginDocumentClose);
            }
            catch//不带任何参数，这种情况下它捕获任何类型的异常，并被称为一般 catch 子句。
            {
                MessageBox.Show("Doc_BeginDocumentClose事件中出错！");
            }
        }
        /// <summary>
        /// 文档关闭已完成事件
        /// </summary>
        private static void DocCollection_DocumentDestroyed(object sender, DocumentDestroyedEventArgs e)
        {
            Autodesk.AutoCAD.ApplicationServices.DocumentCollection DocCollection = (DocumentCollection)sender;
            Autodesk.AutoCAD.ApplicationServices.Document CurDoc = DocCollection.MdiActiveDocument;
            try
            {
                foreach (DocListNode mDoc in doclist)
                {
                    if (mDoc.Doc.Equals(CurDoc))
                    {
                        DocCollectionStatus.ActiveDoc = mDoc.Index;
                        foreach (Document Doc in DocCollection)
                        {
                            Doc.StatusBar.Panes[mDoc.Index].Style = Autodesk.AutoCAD.Windows.PaneStyles.Normal;
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show("DocCollection_DocumentDestroyed事件中出错！");
            }
        }
        /// <summary>
        /// 鼠标命中状态栏文档的按钮引起的事件
        /// </summary>
        private static void StatusBar_MouseDown(object sender, Autodesk.AutoCAD.Windows.StatusBarMouseDownEventArgs e)
        {
            Autodesk.AutoCAD.ApplicationServices.DocumentCollection DocCollection = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager;
            Document CurDoc = DocCollection.MdiActiveDocument;
            Autodesk.AutoCAD.Windows.Pane WillActivePane = (Autodesk.AutoCAD.Windows.Pane)sender;
            Autodesk.AutoCAD.Windows.PaneCollection mdiPaneColletion = CurDoc.StatusBar.Panes;
            try
            {
                int LastDoc = 0;
                for (int i = 1; i <= DocCollectionStatus.DocCount; i++)
                    if (mdiPaneColletion[i].Equals(WillActivePane))
                    {
                        if (DocCollectionStatus.ActiveDoc != i)
                        {
                            LastDoc = DocCollectionStatus.ActiveDoc;
                            DocCollectionStatus.ActiveDoc = i;
                        }
                    }
                if (LastDoc != 0)
                {
                    foreach (DocListNode var in doclist)
                    {
                        if (var.Index == DocCollectionStatus.ActiveDoc)
                        {
                            DocCollection.MdiActiveDocument = var.Doc;//文档激活
                        }
                    }
                    foreach (Document Doc in DocCollection)
                    {
                        Autodesk.AutoCAD.Windows.StatusBar myStatus = Doc.StatusBar;
                        Autodesk.AutoCAD.Windows.PaneCollection DocPaneColletion = myStatus.Panes;
                        DocPaneColletion[LastDoc].Style = Autodesk.AutoCAD.Windows.PaneStyles.PopOut;
                        DocPaneColletion[DocCollectionStatus.ActiveDoc].Style = Autodesk.AutoCAD.Windows.PaneStyles.Normal;
                        myStatus.Update();
                    }
                }
            }
            catch
            {
                MessageBox.Show("StatusBar_MouseDown事件中出错！");
            }
        }
        /// <summary>
        /// 文档保存事件
        /// </summary>
        private static void Database_SaveComplete(object sender, DatabaseIOEventArgs e)
        {
            try
            {
                Autodesk.AutoCAD.ApplicationServices.DocumentCollection DocCollection = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager;
                Autodesk.AutoCAD.DatabaseServices.Database CurDatabase = (Autodesk.AutoCAD.DatabaseServices.Database)sender;
                Autodesk.AutoCAD.Windows.Pane myPane;
                //string FileName1 = Autodesk.AutoCAD.ApplicationServices.Application.GetSystemVariable("SAVEFILE");
                if (CurDatabase.Filename == (string)Autodesk.AutoCAD.ApplicationServices.Application.GetSystemVariable("SAVEFILE"))
                {
                    return;
                }
                foreach (DocListNode mDoc in doclist)
                {
                    if (mDoc.Doc.Database.Equals(CurDatabase))
                    {
                        foreach (Document Doc in DocCollection)
                        {
                            myPane = Doc.StatusBar.Panes[mDoc.Index];
                            myPane.Text = GetFileName(CurDatabase.Filename);
                            myPane.ToolTipText = CurDatabase.Filename;
                            Doc.StatusBar.Update();
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show("Database_SaveComplete事件中出错！");
            }
        }
        /// <summary>
        /// 提取文件名方法
        /// </summary>
        private static string GetFileName(string FileName)
        {
            //string asb = FileName.Substring(FileName.LastIndexOf("\\") + 1);
            return FileName.Substring(FileName.LastIndexOf("\\") + 1);
        }
    }
}
