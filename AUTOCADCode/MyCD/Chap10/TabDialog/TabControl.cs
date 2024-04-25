using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Autodesk.AutoCAD.ApplicationServices;

namespace TabDialog
{
    public partial class TabControl : UserControl
    {
        public TabControl()
        {
            InitializeComponent();
        }

        private void propertyGridOwner_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            //当用户在属性网格内修改项目内容后，属性窗口进行更新
            TabbedDialogExtension.SetDirty(this, true);
        }
    }
}
