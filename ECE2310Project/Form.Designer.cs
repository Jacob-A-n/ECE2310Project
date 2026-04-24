namespace ECE2310Project
{
    partial class Form
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
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageCalendar = new System.Windows.Forms.TabPage();
            this.textBoxMonth = new System.Windows.Forms.TextBox();
            this.tabControl.SuspendLayout();
            this.tabPageCalendar.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Alignment = System.Windows.Forms.TabAlignment.Left;
            this.tabControl.Controls.Add(this.tabPageCalendar);
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Multiline = true;
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(989, 593);
            this.tabControl.TabIndex = 0;
            // 
            // tabPageCalendar
            // 
            this.tabPageCalendar.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tabPageCalendar.Controls.Add(this.textBoxMonth);
            this.tabPageCalendar.Location = new System.Drawing.Point(23, 4);
            this.tabPageCalendar.Margin = new System.Windows.Forms.Padding(0);
            this.tabPageCalendar.Name = "tabPageCalendar";
            this.tabPageCalendar.Size = new System.Drawing.Size(962, 585);
            this.tabPageCalendar.TabIndex = 0;
            this.tabPageCalendar.Text = "Calendar";
            this.tabPageCalendar.UseVisualStyleBackColor = true;
            // 
            // textBoxMonth
            // 
            this.textBoxMonth.Font = new System.Drawing.Font("Noto Sans JP Black", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxMonth.Location = new System.Drawing.Point(0, -4);
            this.textBoxMonth.Name = "textBoxMonth";
            this.textBoxMonth.Size = new System.Drawing.Size(713, 31);
            this.textBoxMonth.TabIndex = 0;
            this.textBoxMonth.Text = "April, 2026";
            this.textBoxMonth.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 586);
            this.Controls.Add(this.tabControl);
            this.Name = "Form";
            this.Text = "Form1";
            this.tabControl.ResumeLayout(false);
            this.tabPageCalendar.ResumeLayout(false);
            this.tabPageCalendar.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageCalendar;
        private System.Windows.Forms.TextBox textBoxMonth;
    }
}