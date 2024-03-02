
namespace MerryTest
{
    partial class RFCalibration
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
            this.components = new System.ComponentModel.Container();
            this.rtbTestValue = new System.Windows.Forms.RichTextBox();
            this.rtbCalibrValue = new System.Windows.Forms.RichTextBox();
            this.btnNo = new System.Windows.Forms.Button();
            this.btnYes = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.rtbCompensation = new System.Windows.Forms.RichTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.txbLimitCompensation = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // rtbTestValue
            // 
            this.rtbTestValue.Font = new System.Drawing.Font("SimSun", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rtbTestValue.Location = new System.Drawing.Point(27, 95);
            this.rtbTestValue.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rtbTestValue.Name = "rtbTestValue";
            this.rtbTestValue.ReadOnly = true;
            this.rtbTestValue.Size = new System.Drawing.Size(418, 457);
            this.rtbTestValue.TabIndex = 0;
            this.rtbTestValue.Text = "";
            // 
            // rtbCalibrValue
            // 
            this.rtbCalibrValue.Font = new System.Drawing.Font("SimSun", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rtbCalibrValue.Location = new System.Drawing.Point(452, 95);
            this.rtbCalibrValue.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rtbCalibrValue.Name = "rtbCalibrValue";
            this.rtbCalibrValue.Size = new System.Drawing.Size(418, 457);
            this.rtbCalibrValue.TabIndex = 0;
            this.rtbCalibrValue.Text = "";
            this.rtbCalibrValue.TextChanged += new System.EventHandler(this.rtbCalibrValue_TextChanged);
            // 
            // btnNo
            // 
            this.btnNo.BackColor = System.Drawing.Color.Red;
            this.btnNo.Font = new System.Drawing.Font("SimSun", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnNo.Location = new System.Drawing.Point(906, 575);
            this.btnNo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnNo.Name = "btnNo";
            this.btnNo.Size = new System.Drawing.Size(191, 60);
            this.btnNo.TabIndex = 1;
            this.btnNo.Text = "取消";
            this.btnNo.UseVisualStyleBackColor = false;
            this.btnNo.Click += new System.EventHandler(this.btnNo_Click);
            // 
            // btnYes
            // 
            this.btnYes.BackColor = System.Drawing.Color.Lime;
            this.btnYes.Font = new System.Drawing.Font("SimSun", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnYes.Location = new System.Drawing.Point(1104, 575);
            this.btnYes.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnYes.Name = "btnYes";
            this.btnYes.Size = new System.Drawing.Size(191, 60);
            this.btnYes.TabIndex = 1;
            this.btnYes.Text = "校准";
            this.btnYes.UseVisualStyleBackColor = false;
            this.btnYes.Click += new System.EventHandler(this.btnYes_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("SimSun", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(22, 59);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(264, 28);
            this.label1.TabIndex = 2;
            this.label1.Text = "测试值/ Test Value";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("SimSun", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(448, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(362, 28);
            this.label2.TabIndex = 2;
            this.label2.Text = "校准值/ Calibration Value";
            // 
            // rtbCompensation
            // 
            this.rtbCompensation.Font = new System.Drawing.Font("SimSun", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rtbCompensation.Location = new System.Drawing.Point(878, 95);
            this.rtbCompensation.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rtbCompensation.Name = "rtbCompensation";
            this.rtbCompensation.ReadOnly = true;
            this.rtbCompensation.Size = new System.Drawing.Size(418, 457);
            this.rtbCompensation.TabIndex = 0;
            this.rtbCompensation.Text = "";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("SimSun", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(873, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(376, 28);
            this.label3.TabIndex = 2;
            this.label3.Text = "补偿值/ Compensation Value";
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // txbLimitCompensation
            // 
            this.txbLimitCompensation.Font = new System.Drawing.Font("Microsoft YaHei", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txbLimitCompensation.Location = new System.Drawing.Point(217, 2);
            this.txbLimitCompensation.Name = "txbLimitCompensation";
            this.txbLimitCompensation.Size = new System.Drawing.Size(88, 44);
            this.txbLimitCompensation.TabIndex = 3;
            this.txbLimitCompensation.Text = "50";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("SimSun", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(3, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(208, 28);
            this.label4.TabIndex = 4;
            this.label4.Text = "补偿值 Limit：";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.MediumAquamarine;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.txbLimitCompensation);
            this.panel1.Location = new System.Drawing.Point(27, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(329, 51);
            this.panel1.TabIndex = 5;
            // 
            // RFCalibration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1304, 645);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnYes);
            this.Controls.Add(this.btnNo);
            this.Controls.Add(this.rtbCompensation);
            this.Controls.Add(this.rtbCalibrValue);
            this.Controls.Add(this.rtbTestValue);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "RFCalibration";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "RFCalibration";
            this.Load += new System.EventHandler(this.RFCalibration_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.RFCalibration_KeyDown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtbTestValue;
        private System.Windows.Forms.RichTextBox rtbCalibrValue;
        private System.Windows.Forms.Button btnNo;
        private System.Windows.Forms.Button btnYes;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RichTextBox rtbCompensation;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TextBox txbLimitCompensation;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel1;
    }
}