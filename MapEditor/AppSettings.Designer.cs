namespace MapEditor
{
    partial class AppSettings
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
            this.okbtn = new System.Windows.Forms.Button();
            this.cancelbtn = new System.Windows.Forms.Button();
            this.AppSettingValue = new System.Windows.Forms.PropertyGrid();
            this.SuspendLayout();
            // 
            // okbtn
            // 
            this.okbtn.Location = new System.Drawing.Point(79, 222);
            this.okbtn.Name = "okbtn";
            this.okbtn.Size = new System.Drawing.Size(60, 25);
            this.okbtn.TabIndex = 5;
            this.okbtn.Text = "OK";
            this.okbtn.UseVisualStyleBackColor = true;
            this.okbtn.Click += new System.EventHandler(this.okbtn_Click);
            // 
            // cancelbtn
            // 
            this.cancelbtn.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cancelbtn.Location = new System.Drawing.Point(139, 222);
            this.cancelbtn.Name = "cancelbtn";
            this.cancelbtn.Size = new System.Drawing.Size(62, 25);
            this.cancelbtn.TabIndex = 4;
            this.cancelbtn.Text = "Cancel";
            this.cancelbtn.UseVisualStyleBackColor = true;
            this.cancelbtn.Click += new System.EventHandler(this.cancelbtn_Click);
            // 
            // AppSettingValue
            // 
            this.AppSettingValue.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.AppSettingValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AppSettingValue.HelpVisible = false;
            this.AppSettingValue.Location = new System.Drawing.Point(20, 20);
            this.AppSettingValue.Name = "AppSettingValue";
            this.AppSettingValue.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
            this.AppSettingValue.Size = new System.Drawing.Size(194, 192);
            this.AppSettingValue.TabIndex = 3;
            this.AppSettingValue.ToolbarVisible = false;
            // 
            // AppSettings
            // 
            this.AcceptButton = this.okbtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(235, 259);
            this.Controls.Add(this.okbtn);
            this.Controls.Add(this.cancelbtn);
            this.Controls.Add(this.AppSettingValue);
            this.Name = "AppSettings";
            this.Text = "AppSettings";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button okbtn;
        private System.Windows.Forms.Button cancelbtn;
        private System.Windows.Forms.PropertyGrid AppSettingValue;

    }
}