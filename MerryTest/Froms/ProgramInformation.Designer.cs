using System.Drawing;

namespace MerryTest.Froms
{
    partial class ProgramInformation
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProgramInformation));
            this.lv_ProgramInformation = new System.Windows.Forms.ListView();
            this.参数名称 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.参数值 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pb_TypeNameImage = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pb_TypeNameImage)).BeginInit();
            this.SuspendLayout();
            // 
            // lv_ProgramInformation
            // 
            this.lv_ProgramInformation.BackColor = System.Drawing.SystemColors.Window;
            this.lv_ProgramInformation.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.参数名称,
            this.参数值});
            this.lv_ProgramInformation.Font = new System.Drawing.Font("微软雅黑", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lv_ProgramInformation.ForeColor = System.Drawing.SystemColors.MenuText;
            this.lv_ProgramInformation.HideSelection = false;
            this.lv_ProgramInformation.Location = new System.Drawing.Point(4, 5);
            this.lv_ProgramInformation.Name = "lv_ProgramInformation";
            this.lv_ProgramInformation.Size = new System.Drawing.Size(361, 216);
            this.lv_ProgramInformation.TabIndex = 0;
            this.lv_ProgramInformation.UseCompatibleStateImageBehavior = false;
            this.lv_ProgramInformation.View = System.Windows.Forms.View.Details;
            // 
            // 参数名称
            // 
            this.参数名称.Text = "参数名称";
            this.参数名称.Width = 130;
            // 
            // 参数值
            // 
            this.参数值.Text = "参数值";
            this.参数值.Width = 292;
            // 
            // pb_TypeNameImage
            // 
            this.pb_TypeNameImage.ErrorImage = null;
            this.pb_TypeNameImage.Image = ((System.Drawing.Image)(resources.GetObject("pb_TypeNameImage.Image")));
            this.pb_TypeNameImage.Location = new System.Drawing.Point(371, 5);
            this.pb_TypeNameImage.Name = "pb_TypeNameImage";
            this.pb_TypeNameImage.Size = new System.Drawing.Size(250, 216);
            this.pb_TypeNameImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pb_TypeNameImage.TabIndex = 1;
            this.pb_TypeNameImage.TabStop = false;
            // 
            // ProgramInformation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(634, 228);
            this.Controls.Add(this.pb_TypeNameImage);
            this.Controls.Add(this.lv_ProgramInformation);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ProgramInformation";
            this.Text = "ProgramInformation";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.ProgramInformation_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pb_TypeNameImage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lv_ProgramInformation;
        private System.Windows.Forms.ColumnHeader 参数名称;
        private System.Windows.Forms.ColumnHeader 参数值;
        private System.Windows.Forms.PictureBox pb_TypeNameImage;
    }
}