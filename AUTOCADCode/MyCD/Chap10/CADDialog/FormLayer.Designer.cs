namespace CADDialog
{
    partial class FormLayer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.olvLayerManager = new BrightIdeasSoftware.ObjectListView();
            this.ColumnState = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.ColumnName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.ColumnIsOff = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.ColumnIsFrozen = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.ColumnIsLocked = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.ColumnColor = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.ColumnLinetype = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.ColumnLineWeight = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.ColumnPlotStyleName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.ColumnIsPlottable = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.ColumnViewportVisibilityDefault = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.ColumnDescription = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.buttonNewLayer = new System.Windows.Forms.Button();
            this.buttonNewLayerFrozen = new System.Windows.Forms.Button();
            this.buttonDeleteLayer = new System.Windows.Forms.Button();
            this.buttonSetCurrentLayer = new System.Windows.Forms.Button();
            this.textBoxCurrentLayer = new System.Windows.Forms.TextBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonHelp = new System.Windows.Forms.Button();
            this.buttonApply = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.olvLayerManager)).BeginInit();
            this.SuspendLayout();
            // 
            // olvLayerManager
            // 
            this.olvLayerManager.AllColumns.Add(this.ColumnState);
            this.olvLayerManager.AllColumns.Add(this.ColumnName);
            this.olvLayerManager.AllColumns.Add(this.ColumnIsOff);
            this.olvLayerManager.AllColumns.Add(this.ColumnIsFrozen);
            this.olvLayerManager.AllColumns.Add(this.ColumnIsLocked);
            this.olvLayerManager.AllColumns.Add(this.ColumnColor);
            this.olvLayerManager.AllColumns.Add(this.ColumnLinetype);
            this.olvLayerManager.AllColumns.Add(this.ColumnLineWeight);
            this.olvLayerManager.AllColumns.Add(this.ColumnPlotStyleName);
            this.olvLayerManager.AllColumns.Add(this.ColumnIsPlottable);
            this.olvLayerManager.AllColumns.Add(this.ColumnViewportVisibilityDefault);
            this.olvLayerManager.AllColumns.Add(this.ColumnDescription);
            this.olvLayerManager.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.SingleClick;
            this.olvLayerManager.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColumnState,
            this.ColumnName,
            this.ColumnIsOff,
            this.ColumnIsFrozen,
            this.ColumnIsLocked,
            this.ColumnColor,
            this.ColumnLinetype,
            this.ColumnLineWeight,
            this.ColumnPlotStyleName,
            this.ColumnIsPlottable,
            this.ColumnViewportVisibilityDefault,
            this.ColumnDescription});
            this.olvLayerManager.FullRowSelect = true;
            this.olvLayerManager.Location = new System.Drawing.Point(24, 56);
            this.olvLayerManager.Name = "olvLayerManager";
            this.olvLayerManager.ShowGroups = false;
            this.olvLayerManager.ShowImagesOnSubItems = true;
            this.olvLayerManager.Size = new System.Drawing.Size(779, 292);
            this.olvLayerManager.TabIndex = 0;
            this.olvLayerManager.UseCompatibleStateImageBehavior = false;
            this.olvLayerManager.View = System.Windows.Forms.View.Details;
            this.olvLayerManager.CellClick += new System.EventHandler<BrightIdeasSoftware.CellClickEventArgs>(this.olvLayerManager_CellClick);
            // 
            // ColumnState
            // 
            this.ColumnState.IsEditable = false;
            this.ColumnState.Text = "状态";
            // 
            // ColumnName
            // 
            this.ColumnName.AspectName = "";
            this.ColumnName.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ColumnName.IsEditable = false;
            this.ColumnName.Text = "名称";
            this.ColumnName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // ColumnIsOff
            // 
            this.ColumnIsOff.AspectName = "";
            this.ColumnIsOff.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ColumnIsOff.IsEditable = false;
            this.ColumnIsOff.Text = "开";
            this.ColumnIsOff.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // ColumnIsFrozen
            // 
            this.ColumnIsFrozen.AspectName = "";
            this.ColumnIsFrozen.IsEditable = false;
            this.ColumnIsFrozen.Text = "冻结";
            // 
            // ColumnIsLocked
            // 
            this.ColumnIsLocked.AspectName = "";
            this.ColumnIsLocked.IsEditable = false;
            this.ColumnIsLocked.Text = "锁定";
            // 
            // ColumnColor
            // 
            this.ColumnColor.AspectName = "";
            this.ColumnColor.IsEditable = false;
            this.ColumnColor.Text = "颜色";
            // 
            // ColumnLinetype
            // 
            this.ColumnLinetype.AspectName = "";
            this.ColumnLinetype.IsEditable = false;
            this.ColumnLinetype.Text = "线型";
            // 
            // ColumnLineWeight
            // 
            this.ColumnLineWeight.AspectName = "";
            this.ColumnLineWeight.IsEditable = false;
            this.ColumnLineWeight.Text = "线宽";
            // 
            // ColumnPlotStyleName
            // 
            this.ColumnPlotStyleName.AspectName = "";
            this.ColumnPlotStyleName.IsEditable = false;
            this.ColumnPlotStyleName.Text = "打印样式";
            this.ColumnPlotStyleName.Width = 82;
            // 
            // ColumnIsPlottable
            // 
            this.ColumnIsPlottable.AspectName = "";
            this.ColumnIsPlottable.IsEditable = false;
            this.ColumnIsPlottable.Text = "打印";
            // 
            // ColumnViewportVisibilityDefault
            // 
            this.ColumnViewportVisibilityDefault.AspectName = "";
            this.ColumnViewportVisibilityDefault.IsEditable = false;
            this.ColumnViewportVisibilityDefault.Text = "冻结新视口";
            this.ColumnViewportVisibilityDefault.Width = 87;
            // 
            // ColumnDescription
            // 
            this.ColumnDescription.AspectName = "";
            this.ColumnDescription.IsEditable = false;
            this.ColumnDescription.Text = "说明";
            // 
            // buttonNewLayer
            // 
            this.buttonNewLayer.Image = global::CADDialog.CADResource.ButtonNew;
            this.buttonNewLayer.Location = new System.Drawing.Point(34, 24);
            this.buttonNewLayer.Name = "buttonNewLayer";
            this.buttonNewLayer.Size = new System.Drawing.Size(21, 23);
            this.buttonNewLayer.TabIndex = 1;
            this.buttonNewLayer.UseVisualStyleBackColor = true;
            this.buttonNewLayer.Click += new System.EventHandler(this.buttonNewLayer_Click);
            // 
            // buttonNewLayerFrozen
            // 
            this.buttonNewLayerFrozen.Image = global::CADDialog.CADResource.ButtonNewFreeze;
            this.buttonNewLayerFrozen.Location = new System.Drawing.Point(61, 24);
            this.buttonNewLayerFrozen.Name = "buttonNewLayerFrozen";
            this.buttonNewLayerFrozen.Size = new System.Drawing.Size(25, 23);
            this.buttonNewLayerFrozen.TabIndex = 2;
            this.buttonNewLayerFrozen.UseVisualStyleBackColor = true;
            this.buttonNewLayerFrozen.Click += new System.EventHandler(this.buttonNewLayerFrozen_Click);
            // 
            // buttonDeleteLayer
            // 
            this.buttonDeleteLayer.Image = global::CADDialog.CADResource.ButtonDelete;
            this.buttonDeleteLayer.Location = new System.Drawing.Point(92, 24);
            this.buttonDeleteLayer.Name = "buttonDeleteLayer";
            this.buttonDeleteLayer.Size = new System.Drawing.Size(29, 23);
            this.buttonDeleteLayer.TabIndex = 3;
            this.buttonDeleteLayer.UseVisualStyleBackColor = true;
            this.buttonDeleteLayer.Click += new System.EventHandler(this.buttonDeleteLayer_Click);
            // 
            // buttonSetCurrentLayer
            // 
            this.buttonSetCurrentLayer.Image = global::CADDialog.CADResource.IsCurrentTrue;
            this.buttonSetCurrentLayer.Location = new System.Drawing.Point(127, 24);
            this.buttonSetCurrentLayer.Name = "buttonSetCurrentLayer";
            this.buttonSetCurrentLayer.Size = new System.Drawing.Size(28, 23);
            this.buttonSetCurrentLayer.TabIndex = 4;
            this.buttonSetCurrentLayer.UseVisualStyleBackColor = true;
            this.buttonSetCurrentLayer.Click += new System.EventHandler(this.buttonSetCurrentLayer_Click);
            // 
            // textBoxCurrentLayer
            // 
            this.textBoxCurrentLayer.Location = new System.Drawing.Point(179, 26);
            this.textBoxCurrentLayer.Name = "textBoxCurrentLayer";
            this.textBoxCurrentLayer.ReadOnly = true;
            this.textBoxCurrentLayer.Size = new System.Drawing.Size(624, 21);
            this.textBoxCurrentLayer.TabIndex = 5;
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(380, 355);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 6;
            this.buttonOK.Text = "确定";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(502, 355);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 7;
            this.buttonCancel.Text = "取消";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonHelp
            // 
            this.buttonHelp.Location = new System.Drawing.Point(713, 355);
            this.buttonHelp.Name = "buttonHelp";
            this.buttonHelp.Size = new System.Drawing.Size(75, 23);
            this.buttonHelp.TabIndex = 8;
            this.buttonHelp.Text = "帮助(&H)";
            this.buttonHelp.UseVisualStyleBackColor = true;
            this.buttonHelp.Click += new System.EventHandler(this.buttonHelp_Click);
            // 
            // buttonApply
            // 
            this.buttonApply.Location = new System.Drawing.Point(613, 355);
            this.buttonApply.Name = "buttonApply";
            this.buttonApply.Size = new System.Drawing.Size(75, 23);
            this.buttonApply.TabIndex = 9;
            this.buttonApply.Text = "应用(&A)";
            this.buttonApply.UseVisualStyleBackColor = true;
            this.buttonApply.Click += new System.EventHandler(this.buttonApply_Click);
            // 
            // FormLayer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(812, 390);
            this.Controls.Add(this.buttonApply);
            this.Controls.Add(this.buttonHelp);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.textBoxCurrentLayer);
            this.Controls.Add(this.buttonSetCurrentLayer);
            this.Controls.Add(this.buttonDeleteLayer);
            this.Controls.Add(this.buttonNewLayerFrozen);
            this.Controls.Add(this.buttonNewLayer);
            this.Controls.Add(this.olvLayerManager);
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormLayer";
            this.Text = "图层特性管理器";
            this.Load += new System.EventHandler(this.FormLayer_Load);
            ((System.ComponentModel.ISupportInitialize)(this.olvLayerManager)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private BrightIdeasSoftware.ObjectListView olvLayerManager;
        private BrightIdeasSoftware.OLVColumn ColumnName;
        private BrightIdeasSoftware.OLVColumn ColumnIsOff;
        private BrightIdeasSoftware.OLVColumn ColumnState;
        private BrightIdeasSoftware.OLVColumn ColumnIsFrozen;
        private BrightIdeasSoftware.OLVColumn ColumnIsLocked;
        private BrightIdeasSoftware.OLVColumn ColumnColor;
        private BrightIdeasSoftware.OLVColumn ColumnLinetype;
        private BrightIdeasSoftware.OLVColumn ColumnLineWeight;
        private BrightIdeasSoftware.OLVColumn ColumnPlotStyleName;
        private BrightIdeasSoftware.OLVColumn ColumnIsPlottable;
        private BrightIdeasSoftware.OLVColumn ColumnViewportVisibilityDefault;
        private BrightIdeasSoftware.OLVColumn ColumnDescription;
        private System.Windows.Forms.Button buttonNewLayer;
        private System.Windows.Forms.Button buttonNewLayerFrozen;
        private System.Windows.Forms.Button buttonDeleteLayer;
        private System.Windows.Forms.Button buttonSetCurrentLayer;
        private System.Windows.Forms.TextBox textBoxCurrentLayer;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonHelp;
        private System.Windows.Forms.Button buttonApply;
    }
}