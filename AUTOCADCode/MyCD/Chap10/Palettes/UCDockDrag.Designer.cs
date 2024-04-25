namespace Palettes
{
    partial class UCDockDrag
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.comboBoxDock = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxDrag = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBoxDock
            // 
            this.comboBoxDock.FormattingEnabled = true;
            this.comboBoxDock.Items.AddRange(new object[] {
            "底部",
            "左侧",
            "右侧",
            "顶部",
            "浮动"});
            this.comboBoxDock.Location = new System.Drawing.Point(69, 21);
            this.comboBoxDock.Name = "comboBoxDock";
            this.comboBoxDock.Size = new System.Drawing.Size(121, 20);
            this.comboBoxDock.TabIndex = 0;
            this.comboBoxDock.DropDown += new System.EventHandler(this.comboBoxDock_DropDown);
            this.comboBoxDock.SelectedIndexChanged += new System.EventHandler(this.comboBoxDock_SelectedIndexChanged);
            this.comboBoxDock.DropDownClosed += new System.EventHandler(this.comboBoxDock_DropDownClosed);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "停靠";
            this.label1.UseMnemonic = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBoxDrag);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(26, 71);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(164, 134);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "拖放演示";
            // 
            // textBoxDrag
            // 
            this.textBoxDrag.Location = new System.Drawing.Point(19, 62);
            this.textBoxDrag.Name = "textBoxDrag";
            this.textBoxDrag.Size = new System.Drawing.Size(139, 21);
            this.textBoxDrag.TabIndex = 1;
            this.textBoxDrag.MouseMove += new System.Windows.Forms.MouseEventHandler(this.textBoxDrag_MouseMove);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(113, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "输入需要拖放的文字";
            // 
            // UCDockDrag
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBoxDock);
            this.Name = "UCDockDrag";
            this.Size = new System.Drawing.Size(217, 230);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxDock;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        internal System.Windows.Forms.TextBox textBoxDrag;
    }
}
