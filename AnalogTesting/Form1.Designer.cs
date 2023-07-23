namespace AnalogTesting
{
    partial class Form1
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
            this.button1 = new System.Windows.Forms.Button();
            this.plotView1 = new OxyPlot.WindowsForms.PlotView();
            this.comboBoxRecordings = new System.Windows.Forms.ComboBox();
            this.radioButtonA = new System.Windows.Forms.RadioButton();
            this.radioButtonB = new System.Windows.Forms.RadioButton();
            this.radioButtonC = new System.Windows.Forms.RadioButton();
            this.radioButtonZ = new System.Windows.Forms.RadioButton();
            this.buttonRunGraph = new System.Windows.Forms.Button();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxSampleSize = new System.Windows.Forms.ComboBox();
            this.comboBoxWindowMode = new System.Windows.Forms.ComboBox();
            this.button2 = new System.Windows.Forms.Button();
            this.buttonSaveAllGraphs = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(123, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Record 30 seconds";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // plotView1
            // 
            this.plotView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.plotView1.Location = new System.Drawing.Point(12, 86);
            this.plotView1.Name = "plotView1";
            this.plotView1.PanCursor = System.Windows.Forms.Cursors.Hand;
            this.plotView1.Size = new System.Drawing.Size(949, 487);
            this.plotView1.TabIndex = 1;
            this.plotView1.Text = "plotView1";
            this.plotView1.ZoomHorizontalCursor = System.Windows.Forms.Cursors.SizeWE;
            this.plotView1.ZoomRectangleCursor = System.Windows.Forms.Cursors.SizeNWSE;
            this.plotView1.ZoomVerticalCursor = System.Windows.Forms.Cursors.SizeNS;
            // 
            // comboBoxRecordings
            // 
            this.comboBoxRecordings.FormattingEnabled = true;
            this.comboBoxRecordings.Location = new System.Drawing.Point(12, 42);
            this.comboBoxRecordings.Name = "comboBoxRecordings";
            this.comboBoxRecordings.Size = new System.Drawing.Size(225, 21);
            this.comboBoxRecordings.TabIndex = 2;
            this.comboBoxRecordings.SelectedValueChanged += new System.EventHandler(this.comboBoxRecordings_SelectedValueChanged);
            // 
            // radioButtonA
            // 
            this.radioButtonA.AutoSize = true;
            this.radioButtonA.Checked = true;
            this.radioButtonA.Location = new System.Drawing.Point(6, 15);
            this.radioButtonA.Name = "radioButtonA";
            this.radioButtonA.Size = new System.Drawing.Size(32, 17);
            this.radioButtonA.TabIndex = 3;
            this.radioButtonA.TabStop = true;
            this.radioButtonA.Text = "A";
            this.radioButtonA.UseVisualStyleBackColor = true;
            // 
            // radioButtonB
            // 
            this.radioButtonB.AutoSize = true;
            this.radioButtonB.Location = new System.Drawing.Point(6, 30);
            this.radioButtonB.Name = "radioButtonB";
            this.radioButtonB.Size = new System.Drawing.Size(32, 17);
            this.radioButtonB.TabIndex = 4;
            this.radioButtonB.Text = "B";
            this.radioButtonB.UseVisualStyleBackColor = true;
            // 
            // radioButtonC
            // 
            this.radioButtonC.AutoSize = true;
            this.radioButtonC.Location = new System.Drawing.Point(6, 45);
            this.radioButtonC.Name = "radioButtonC";
            this.radioButtonC.Size = new System.Drawing.Size(32, 17);
            this.radioButtonC.TabIndex = 5;
            this.radioButtonC.Text = "C";
            this.radioButtonC.UseVisualStyleBackColor = true;
            // 
            // radioButtonZ
            // 
            this.radioButtonZ.AutoSize = true;
            this.radioButtonZ.Location = new System.Drawing.Point(6, 60);
            this.radioButtonZ.Name = "radioButtonZ";
            this.radioButtonZ.Size = new System.Drawing.Size(32, 17);
            this.radioButtonZ.TabIndex = 6;
            this.radioButtonZ.Text = "Z";
            this.radioButtonZ.UseVisualStyleBackColor = true;
            // 
            // buttonRunGraph
            // 
            this.buttonRunGraph.Location = new System.Drawing.Point(788, 12);
            this.buttonRunGraph.Name = "buttonRunGraph";
            this.buttonRunGraph.Size = new System.Drawing.Size(75, 23);
            this.buttonRunGraph.TabIndex = 7;
            this.buttonRunGraph.Text = "Graph it";
            this.buttonRunGraph.UseVisualStyleBackColor = true;
            this.buttonRunGraph.Click += new System.EventHandler(this.buttonRunGraph_Click);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.DecimalPlaces = 4;
            this.numericUpDown1.Location = new System.Drawing.Point(526, 12);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(120, 20);
            this.numericUpDown1.TabIndex = 10;
            this.numericUpDown1.Value = new decimal(new int[] {
            46,
            0,
            0,
            131072});
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButtonA);
            this.groupBox2.Controls.Add(this.radioButtonB);
            this.groupBox2.Controls.Add(this.radioButtonC);
            this.groupBox2.Controls.Add(this.radioButtonZ);
            this.groupBox2.Location = new System.Drawing.Point(243, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(83, 80);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Db Weight";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(485, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Alpha";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(652, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 56);
            this.label2.TabIndex = 14;
            this.label2.Tag = "";
            this.label2.Text = "Hamming - 0.46\r\nHann - 0.5\r\nRectangular - 0.0\r\nBlackman - 0.16\r\n";
            // 
            // comboBoxSampleSize
            // 
            this.comboBoxSampleSize.FormattingEnabled = true;
            this.comboBoxSampleSize.Location = new System.Drawing.Point(345, 30);
            this.comboBoxSampleSize.Name = "comboBoxSampleSize";
            this.comboBoxSampleSize.Size = new System.Drawing.Size(121, 21);
            this.comboBoxSampleSize.TabIndex = 15;
            // 
            // comboBoxWindowMode
            // 
            this.comboBoxWindowMode.FormattingEnabled = true;
            this.comboBoxWindowMode.Location = new System.Drawing.Point(345, 6);
            this.comboBoxWindowMode.Name = "comboBoxWindowMode";
            this.comboBoxWindowMode.Size = new System.Drawing.Size(121, 21);
            this.comboBoxWindowMode.TabIndex = 16;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(788, 40);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 17;
            this.button2.Text = "Out to wav";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // buttonSaveAllGraphs
            // 
            this.buttonSaveAllGraphs.Location = new System.Drawing.Point(869, 12);
            this.buttonSaveAllGraphs.Name = "buttonSaveAllGraphs";
            this.buttonSaveAllGraphs.Size = new System.Drawing.Size(98, 23);
            this.buttonSaveAllGraphs.TabIndex = 18;
            this.buttonSaveAllGraphs.Text = "Save All Graphs";
            this.buttonSaveAllGraphs.UseVisualStyleBackColor = true;
            this.buttonSaveAllGraphs.Click += new System.EventHandler(this.buttonSaveAllGraphs_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(973, 585);
            this.Controls.Add(this.buttonSaveAllGraphs);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.comboBoxWindowMode);
            this.Controls.Add(this.comboBoxSampleSize);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.buttonRunGraph);
            this.Controls.Add(this.comboBoxRecordings);
            this.Controls.Add(this.plotView1);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private OxyPlot.WindowsForms.PlotView plotView1;
        private System.Windows.Forms.ComboBox comboBoxRecordings;
        private System.Windows.Forms.RadioButton radioButtonA;
        private System.Windows.Forms.RadioButton radioButtonB;
        private System.Windows.Forms.RadioButton radioButtonC;
        private System.Windows.Forms.RadioButton radioButtonZ;
        private System.Windows.Forms.Button buttonRunGraph;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxSampleSize;
        private System.Windows.Forms.ComboBox comboBoxWindowMode;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button buttonSaveAllGraphs;
    }
}