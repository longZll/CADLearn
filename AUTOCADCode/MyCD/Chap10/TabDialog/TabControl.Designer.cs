namespace TabDialog
{
    partial class TabControl
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
            this.propertyGridOwner = new System.Windows.Forms.PropertyGrid();
            this.SuspendLayout();
            // 
            // propertyGridOwner
            // 
            this.propertyGridOwner.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGridOwner.Location = new System.Drawing.Point(0, 0);
            this.propertyGridOwner.Name = "propertyGridOwner";
            this.propertyGridOwner.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.propertyGridOwner.Size = new System.Drawing.Size(213, 258);
            this.propertyGridOwner.TabIndex = 0;
            this.propertyGridOwner.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGridOwner_PropertyValueChanged);
            // 
            // TabControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.propertyGridOwner);
            this.Name = "TabControl";
            this.Size = new System.Drawing.Size(213, 258);
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.PropertyGrid propertyGridOwner;

    }
}
