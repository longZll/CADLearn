namespace Plots
{
    partial class PlotForm
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
            this.comboBoxDevice = new System.Windows.Forms.ComboBox();
            this.comboBoxMedia = new System.Windows.Forms.ComboBox();
            this.buttonPreview = new System.Windows.Forms.Button();
            this.buttonPlot = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBoxOrientation = new System.Windows.Forms.GroupBox();
            this.checkBoxReverse = new System.Windows.Forms.CheckBox();
            this.radioButtonHorizontal = new System.Windows.Forms.RadioButton();
            this.radioButtonVertical = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.checkBoxPlotStyles = new System.Windows.Forms.CheckBox();
            this.comboBoxStyleSheet = new System.Windows.Forms.ComboBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.buttonPlotWindow = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBoxPlotArea = new System.Windows.Forms.ComboBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.checkBoxPlotCentered = new System.Windows.Forms.CheckBox();
            this.labelOffsetYUnit = new System.Windows.Forms.Label();
            this.labelOffsetXUnit = new System.Windows.Forms.Label();
            this.textBoxOffsetY = new System.Windows.Forms.TextBox();
            this.textBoxOffsetX = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.comboBoxPlotPaperUnits = new System.Windows.Forms.ComboBox();
            this.comboBoxStdScaleType = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.textBoxDenominator = new System.Windows.Forms.TextBox();
            this.textBoxNumerator = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.checkBoxExtent = new System.Windows.Forms.CheckBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.numericCopies = new System.Windows.Forms.NumericUpDown();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.checkBoxToFile = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonApply = new System.Windows.Forms.Button();
            this.buttonPlotLayouts = new System.Windows.Forms.Button();
            this.groupBoxOrientation.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericCopies)).BeginInit();
            this.groupBox9.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBoxDevice
            // 
            this.comboBoxDevice.FormattingEnabled = true;
            this.comboBoxDevice.Location = new System.Drawing.Point(58, 22);
            this.comboBoxDevice.Name = "comboBoxDevice";
            this.comboBoxDevice.Size = new System.Drawing.Size(237, 20);
            this.comboBoxDevice.TabIndex = 0;
            this.comboBoxDevice.SelectedIndexChanged += new System.EventHandler(this.comboBoxDevice_SelectedIndexChanged);
            // 
            // comboBoxMedia
            // 
            this.comboBoxMedia.FormattingEnabled = true;
            this.comboBoxMedia.Location = new System.Drawing.Point(11, 19);
            this.comboBoxMedia.Name = "comboBoxMedia";
            this.comboBoxMedia.Size = new System.Drawing.Size(284, 20);
            this.comboBoxMedia.TabIndex = 3;
            this.comboBoxMedia.SelectedIndexChanged += new System.EventHandler(this.comboBox_UpdateDataBinding);
            // 
            // buttonPreview
            // 
            this.buttonPreview.Location = new System.Drawing.Point(20, 398);
            this.buttonPreview.Name = "buttonPreview";
            this.buttonPreview.Size = new System.Drawing.Size(87, 23);
            this.buttonPreview.TabIndex = 4;
            this.buttonPreview.Text = "预览当前布局";
            this.buttonPreview.UseVisualStyleBackColor = true;
            this.buttonPreview.Click += new System.EventHandler(this.buttonPreview_Click);
            // 
            // buttonPlot
            // 
            this.buttonPlot.Location = new System.Drawing.Point(238, 398);
            this.buttonPlot.Name = "buttonPlot";
            this.buttonPlot.Size = new System.Drawing.Size(93, 23);
            this.buttonPlot.TabIndex = 5;
            this.buttonPlot.Text = "打印当前布局";
            this.buttonPlot.UseVisualStyleBackColor = true;
            this.buttonPlot.Click += new System.EventHandler(this.buttonPlot_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(479, 398);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 6;
            this.buttonCancel.Text = "取消";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // groupBoxOrientation
            // 
            this.groupBoxOrientation.Controls.Add(this.checkBoxReverse);
            this.groupBoxOrientation.Controls.Add(this.radioButtonHorizontal);
            this.groupBoxOrientation.Controls.Add(this.radioButtonVertical);
            this.groupBoxOrientation.Location = new System.Drawing.Point(348, 103);
            this.groupBoxOrientation.Name = "groupBoxOrientation";
            this.groupBoxOrientation.Size = new System.Drawing.Size(97, 100);
            this.groupBoxOrientation.TabIndex = 7;
            this.groupBoxOrientation.TabStop = false;
            this.groupBoxOrientation.Text = "图形方向";
            // 
            // checkBoxReverse
            // 
            this.checkBoxReverse.AutoSize = true;
            this.checkBoxReverse.Location = new System.Drawing.Point(7, 67);
            this.checkBoxReverse.Name = "checkBoxReverse";
            this.checkBoxReverse.Size = new System.Drawing.Size(72, 16);
            this.checkBoxReverse.TabIndex = 2;
            this.checkBoxReverse.Text = "反向打印";
            this.checkBoxReverse.UseVisualStyleBackColor = true;
            this.checkBoxReverse.CheckedChanged += new System.EventHandler(this.control_ValueChanged);
            // 
            // radioButtonHorizontal
            // 
            this.radioButtonHorizontal.AutoSize = true;
            this.radioButtonHorizontal.Location = new System.Drawing.Point(7, 40);
            this.radioButtonHorizontal.Name = "radioButtonHorizontal";
            this.radioButtonHorizontal.Size = new System.Drawing.Size(47, 16);
            this.radioButtonHorizontal.TabIndex = 1;
            this.radioButtonHorizontal.TabStop = true;
            this.radioButtonHorizontal.Text = "横向";
            this.radioButtonHorizontal.UseVisualStyleBackColor = true;
            this.radioButtonHorizontal.CheckedChanged += new System.EventHandler(this.control_ValueChanged);
            // 
            // radioButtonVertical
            // 
            this.radioButtonVertical.AutoSize = true;
            this.radioButtonVertical.Checked = true;
            this.radioButtonVertical.Location = new System.Drawing.Point(7, 18);
            this.radioButtonVertical.Name = "radioButtonVertical";
            this.radioButtonVertical.Size = new System.Drawing.Size(47, 16);
            this.radioButtonVertical.TabIndex = 0;
            this.radioButtonVertical.TabStop = true;
            this.radioButtonVertical.Text = "纵向";
            this.radioButtonVertical.UseVisualStyleBackColor = true;
            this.radioButtonVertical.CheckedChanged += new System.EventHandler(this.control_ValueChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.checkBoxPlotStyles);
            this.groupBox3.Controls.Add(this.comboBoxStyleSheet);
            this.groupBox3.Location = new System.Drawing.Point(348, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(250, 85);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "打印样式表";
            // 
            // checkBoxPlotStyles
            // 
            this.checkBoxPlotStyles.AutoSize = true;
            this.checkBoxPlotStyles.Location = new System.Drawing.Point(7, 54);
            this.checkBoxPlotStyles.Name = "checkBoxPlotStyles";
            this.checkBoxPlotStyles.Size = new System.Drawing.Size(84, 16);
            this.checkBoxPlotStyles.TabIndex = 0;
            this.checkBoxPlotStyles.Text = "按样式打印";
            this.checkBoxPlotStyles.UseVisualStyleBackColor = true;
            this.checkBoxPlotStyles.CheckedChanged += new System.EventHandler(this.control_ValueChanged);
            // 
            // comboBoxStyleSheet
            // 
            this.comboBoxStyleSheet.FormattingEnabled = true;
            this.comboBoxStyleSheet.Location = new System.Drawing.Point(6, 27);
            this.comboBoxStyleSheet.Name = "comboBoxStyleSheet";
            this.comboBoxStyleSheet.Size = new System.Drawing.Size(226, 20);
            this.comboBoxStyleSheet.TabIndex = 0;
            this.comboBoxStyleSheet.SelectedIndexChanged += new System.EventHandler(this.comboBox_UpdateDataBinding);
            this.comboBoxStyleSheet.TabIndexChanged += new System.EventHandler(this.control_ValueChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.buttonPlotWindow);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Controls.Add(this.comboBoxPlotArea);
            this.groupBox4.Location = new System.Drawing.Point(18, 210);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(259, 76);
            this.groupBox4.TabIndex = 10;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "打印区域";
            // 
            // buttonPlotWindow
            // 
            this.buttonPlotWindow.Location = new System.Drawing.Point(155, 39);
            this.buttonPlotWindow.Name = "buttonPlotWindow";
            this.buttonPlotWindow.Size = new System.Drawing.Size(75, 23);
            this.buttonPlotWindow.TabIndex = 2;
            this.buttonPlotWindow.Text = "窗口<";
            this.buttonPlotWindow.UseVisualStyleBackColor = true;
            this.buttonPlotWindow.Click += new System.EventHandler(this.buttonPlotWindow_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 1;
            this.label3.Text = "打印范围：";
            // 
            // comboBoxPlotArea
            // 
            this.comboBoxPlotArea.FormattingEnabled = true;
            this.comboBoxPlotArea.Location = new System.Drawing.Point(8, 41);
            this.comboBoxPlotArea.Name = "comboBoxPlotArea";
            this.comboBoxPlotArea.Size = new System.Drawing.Size(121, 20);
            this.comboBoxPlotArea.TabIndex = 0;
            this.comboBoxPlotArea.SelectedIndexChanged += new System.EventHandler(this.comboBoxPlotArea_SelectedIndexChanged);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.checkBoxPlotCentered);
            this.groupBox5.Controls.Add(this.labelOffsetYUnit);
            this.groupBox5.Controls.Add(this.labelOffsetXUnit);
            this.groupBox5.Controls.Add(this.textBoxOffsetY);
            this.groupBox5.Controls.Add(this.textBoxOffsetX);
            this.groupBox5.Controls.Add(this.label5);
            this.groupBox5.Controls.Add(this.label4);
            this.groupBox5.Location = new System.Drawing.Point(16, 292);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(261, 79);
            this.groupBox5.TabIndex = 11;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "打印偏移（原点设置在可打印区域）";
            // 
            // checkBoxPlotCentered
            // 
            this.checkBoxPlotCentered.AutoSize = true;
            this.checkBoxPlotCentered.Location = new System.Drawing.Point(174, 29);
            this.checkBoxPlotCentered.Name = "checkBoxPlotCentered";
            this.checkBoxPlotCentered.Size = new System.Drawing.Size(72, 16);
            this.checkBoxPlotCentered.TabIndex = 6;
            this.checkBoxPlotCentered.Text = "居中打印";
            this.checkBoxPlotCentered.UseVisualStyleBackColor = true;
            this.checkBoxPlotCentered.CheckedChanged += new System.EventHandler(this.checkBoxPlotCentered_CheckedChanged);
            // 
            // labelOffsetYUnit
            // 
            this.labelOffsetYUnit.AutoSize = true;
            this.labelOffsetYUnit.Location = new System.Drawing.Point(137, 50);
            this.labelOffsetYUnit.Name = "labelOffsetYUnit";
            this.labelOffsetYUnit.Size = new System.Drawing.Size(29, 12);
            this.labelOffsetYUnit.TabIndex = 5;
            this.labelOffsetYUnit.Text = "毫米";
            // 
            // labelOffsetXUnit
            // 
            this.labelOffsetXUnit.AutoSize = true;
            this.labelOffsetXUnit.Location = new System.Drawing.Point(138, 29);
            this.labelOffsetXUnit.Name = "labelOffsetXUnit";
            this.labelOffsetXUnit.Size = new System.Drawing.Size(29, 12);
            this.labelOffsetXUnit.TabIndex = 4;
            this.labelOffsetXUnit.Text = "毫米";
            // 
            // textBoxOffsetY
            // 
            this.textBoxOffsetY.Location = new System.Drawing.Point(31, 47);
            this.textBoxOffsetY.Name = "textBoxOffsetY";
            this.textBoxOffsetY.Size = new System.Drawing.Size(100, 21);
            this.textBoxOffsetY.TabIndex = 3;
            this.textBoxOffsetY.Leave += new System.EventHandler(this.control_ValueChanged);
            // 
            // textBoxOffsetX
            // 
            this.textBoxOffsetX.Location = new System.Drawing.Point(31, 21);
            this.textBoxOffsetX.Name = "textBoxOffsetX";
            this.textBoxOffsetX.Size = new System.Drawing.Size(100, 21);
            this.textBoxOffsetX.TabIndex = 2;
            this.textBoxOffsetX.Leave += new System.EventHandler(this.control_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 47);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(17, 12);
            this.label5.TabIndex = 1;
            this.label5.Text = "Y:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "X:";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.label10);
            this.groupBox6.Controls.Add(this.comboBoxPlotPaperUnits);
            this.groupBox6.Controls.Add(this.comboBoxStdScaleType);
            this.groupBox6.Controls.Add(this.label9);
            this.groupBox6.Controls.Add(this.textBoxDenominator);
            this.groupBox6.Controls.Add(this.textBoxNumerator);
            this.groupBox6.Controls.Add(this.label8);
            this.groupBox6.Controls.Add(this.checkBoxExtent);
            this.groupBox6.Location = new System.Drawing.Point(348, 210);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(250, 162);
            this.groupBox6.TabIndex = 12;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "打印比例";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(209, 86);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(11, 12);
            this.label10.TabIndex = 10;
            this.label10.Text = "=";
            // 
            // comboBoxPlotPaperUnits
            // 
            this.comboBoxPlotPaperUnits.FormattingEnabled = true;
            this.comboBoxPlotPaperUnits.Location = new System.Drawing.Point(131, 83);
            this.comboBoxPlotPaperUnits.Name = "comboBoxPlotPaperUnits";
            this.comboBoxPlotPaperUnits.Size = new System.Drawing.Size(72, 20);
            this.comboBoxPlotPaperUnits.TabIndex = 9;
            this.comboBoxPlotPaperUnits.SelectedIndexChanged += new System.EventHandler(this.comboBox_UpdateDataBinding);
            // 
            // comboBoxStdScaleType
            // 
            this.comboBoxStdScaleType.FormattingEnabled = true;
            this.comboBoxStdScaleType.Location = new System.Drawing.Point(75, 46);
            this.comboBoxStdScaleType.Name = "comboBoxStdScaleType";
            this.comboBoxStdScaleType.Size = new System.Drawing.Size(157, 20);
            this.comboBoxStdScaleType.TabIndex = 8;
            this.comboBoxStdScaleType.SelectedIndexChanged += new System.EventHandler(this.comboBox_UpdateDataBinding);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(129, 112);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(29, 12);
            this.label9.TabIndex = 6;
            this.label9.Text = "单位";
            // 
            // textBoxDenominator
            // 
            this.textBoxDenominator.Location = new System.Drawing.Point(75, 109);
            this.textBoxDenominator.Name = "textBoxDenominator";
            this.textBoxDenominator.Size = new System.Drawing.Size(49, 21);
            this.textBoxDenominator.TabIndex = 5;
            this.textBoxDenominator.Leave += new System.EventHandler(this.control_ValueChanged);
            // 
            // textBoxNumerator
            // 
            this.textBoxNumerator.Location = new System.Drawing.Point(75, 82);
            this.textBoxNumerator.Name = "textBoxNumerator";
            this.textBoxNumerator.Size = new System.Drawing.Size(49, 21);
            this.textBoxNumerator.TabIndex = 3;
            this.textBoxNumerator.Leave += new System.EventHandler(this.control_ValueChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(17, 49);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(41, 12);
            this.label8.TabIndex = 1;
            this.label8.Text = "比例：";
            // 
            // checkBoxExtent
            // 
            this.checkBoxExtent.AutoSize = true;
            this.checkBoxExtent.Location = new System.Drawing.Point(17, 23);
            this.checkBoxExtent.Name = "checkBoxExtent";
            this.checkBoxExtent.Size = new System.Drawing.Size(72, 16);
            this.checkBoxExtent.TabIndex = 0;
            this.checkBoxExtent.Text = "布满图纸";
            this.checkBoxExtent.UseVisualStyleBackColor = true;
            this.checkBoxExtent.CheckedChanged += new System.EventHandler(this.checkBoxExtent_CheckedChanged);
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.comboBoxMedia);
            this.groupBox7.Location = new System.Drawing.Point(18, 127);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(313, 48);
            this.groupBox7.TabIndex = 13;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "图纸尺寸";
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.numericCopies);
            this.groupBox8.Location = new System.Drawing.Point(473, 103);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(119, 48);
            this.groupBox8.TabIndex = 14;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "打印份数";
            // 
            // numericCopies
            // 
            this.numericCopies.Location = new System.Drawing.Point(22, 17);
            this.numericCopies.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericCopies.Name = "numericCopies";
            this.numericCopies.Size = new System.Drawing.Size(57, 21);
            this.numericCopies.TabIndex = 0;
            this.numericCopies.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.checkBoxToFile);
            this.groupBox9.Controls.Add(this.label2);
            this.groupBox9.Controls.Add(this.comboBoxDevice);
            this.groupBox9.Location = new System.Drawing.Point(18, 12);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(313, 85);
            this.groupBox9.TabIndex = 15;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "打印机/绘图仪";
            // 
            // checkBoxToFile
            // 
            this.checkBoxToFile.AutoSize = true;
            this.checkBoxToFile.Location = new System.Drawing.Point(13, 54);
            this.checkBoxToFile.Name = "checkBoxToFile";
            this.checkBoxToFile.Size = new System.Drawing.Size(84, 16);
            this.checkBoxToFile.TabIndex = 2;
            this.checkBoxToFile.Text = "打印到文件";
            this.checkBoxToFile.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "名称：";
            // 
            // buttonApply
            // 
            this.buttonApply.Enabled = false;
            this.buttonApply.Location = new System.Drawing.Point(119, 398);
            this.buttonApply.Name = "buttonApply";
            this.buttonApply.Size = new System.Drawing.Size(100, 23);
            this.buttonApply.TabIndex = 16;
            this.buttonApply.Text = "应用到当前布局";
            this.buttonApply.UseVisualStyleBackColor = true;
            this.buttonApply.Click += new System.EventHandler(this.buttonApply_Click);
            // 
            // buttonPlotLayouts
            // 
            this.buttonPlotLayouts.Location = new System.Drawing.Point(354, 398);
            this.buttonPlotLayouts.Name = "buttonPlotLayouts";
            this.buttonPlotLayouts.Size = new System.Drawing.Size(95, 23);
            this.buttonPlotLayouts.TabIndex = 11;
            this.buttonPlotLayouts.Text = "打印所有布局";
            this.buttonPlotLayouts.UseVisualStyleBackColor = true;
            this.buttonPlotLayouts.Click += new System.EventHandler(this.buttonPlotLayouts_Click);
            // 
            // PlotForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(611, 428);
            this.Controls.Add(this.buttonPlotLayouts);
            this.Controls.Add(this.buttonApply);
            this.Controls.Add(this.groupBox9);
            this.Controls.Add(this.groupBox8);
            this.Controls.Add(this.groupBox7);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.buttonPlot);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBoxOrientation);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonPreview);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PlotForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PlotForm";
            this.Load += new System.EventHandler(this.PlotForm_Load);
            this.groupBoxOrientation.ResumeLayout(false);
            this.groupBoxOrientation.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox8.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericCopies)).EndInit();
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxDevice;
        private System.Windows.Forms.ComboBox comboBoxMedia;
        private System.Windows.Forms.Button buttonPreview;
        private System.Windows.Forms.Button buttonPlot;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.GroupBox groupBoxOrientation;
        private System.Windows.Forms.CheckBox checkBoxReverse;
        private System.Windows.Forms.RadioButton radioButtonHorizontal;
        private System.Windows.Forms.RadioButton radioButtonVertical;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox comboBoxStyleSheet;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBoxPlotArea;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.CheckBox checkBoxPlotCentered;
        private System.Windows.Forms.Label labelOffsetYUnit;
        private System.Windows.Forms.Label labelOffsetXUnit;
        private System.Windows.Forms.TextBox textBoxOffsetY;
        private System.Windows.Forms.TextBox textBoxOffsetX;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox comboBoxPlotPaperUnits;
        private System.Windows.Forms.ComboBox comboBoxStdScaleType;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBoxDenominator;
        private System.Windows.Forms.TextBox textBoxNumerator;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox checkBoxExtent;
        private System.Windows.Forms.Button buttonPlotWindow;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.NumericUpDown numericCopies;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.CheckBox checkBoxToFile;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonApply;
        private System.Windows.Forms.CheckBox checkBoxPlotStyles;
        private System.Windows.Forms.Button buttonPlotLayouts;
    }
}