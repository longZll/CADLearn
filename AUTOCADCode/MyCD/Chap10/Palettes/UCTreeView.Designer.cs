namespace Palettes
{
    partial class UCTreeView
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCTreeView));
            this.treeViewEnts = new System.Windows.Forms.TreeView();
            this.dataGridViewEnts = new System.Windows.Forms.DataGridView();
            this.imageListNode = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewEnts)).BeginInit();
            this.SuspendLayout();
            // 
            // treeViewEnts
            // 
            this.treeViewEnts.Dock = System.Windows.Forms.DockStyle.Top;
            this.treeViewEnts.Location = new System.Drawing.Point(0, 0);
            this.treeViewEnts.Name = "treeViewEnts";
            this.treeViewEnts.ShowNodeToolTips = true;
            this.treeViewEnts.Size = new System.Drawing.Size(322, 177);
            this.treeViewEnts.TabIndex = 0;
            this.treeViewEnts.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewEnts_AfterSelect);
            // 
            // dataGridViewEnts
            // 
            this.dataGridViewEnts.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridViewEnts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewEnts.Dock = System.Windows.Forms.DockStyle.Top;
            this.dataGridViewEnts.Location = new System.Drawing.Point(0, 177);
            this.dataGridViewEnts.Name = "dataGridViewEnts";
            this.dataGridViewEnts.RowTemplate.Height = 23;
            this.dataGridViewEnts.Size = new System.Drawing.Size(322, 208);
            this.dataGridViewEnts.TabIndex = 1;
            // 
            // imageListNode
            // 
            this.imageListNode.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListNode.ImageStream")));
            this.imageListNode.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListNode.Images.SetKeyName(0, "acad.ico");
            this.imageListNode.Images.SetKeyName(1, "door.ico");
            this.imageListNode.Images.SetKeyName(2, "window.ico");
            // 
            // UCTreeView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataGridViewEnts);
            this.Controls.Add(this.treeViewEnts);
            this.Name = "UCTreeView";
            this.Size = new System.Drawing.Size(322, 391);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewEnts)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.TreeView treeViewEnts;
        internal System.Windows.Forms.DataGridView dataGridViewEnts;
        internal System.Windows.Forms.ImageList imageListNode;

    }
}
