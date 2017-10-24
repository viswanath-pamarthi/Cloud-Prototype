namespace Client
{
    partial class PostLogin
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
            this.txtContent = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnLogOut = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtContent
            // 
            this.txtContent.Location = new System.Drawing.Point(3, 39);
            this.txtContent.Multiline = true;
            this.txtContent.Name = "txtContent";
            this.txtContent.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtContent.Size = new System.Drawing.Size(906, 532);
            this.txtContent.TabIndex = 0;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(338, 592);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 27);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnLogOut
            // 
            this.btnLogOut.Location = new System.Drawing.Point(533, 594);
            this.btnLogOut.Name = "btnLogOut";
            this.btnLogOut.Size = new System.Drawing.Size(75, 23);
            this.btnLogOut.TabIndex = 2;
            this.btnLogOut.Text = "Exit";
            this.btnLogOut.UseVisualStyleBackColor = true;
            this.btnLogOut.Click += new System.EventHandler(this.btnLogOut_Click);
            // 
            // PostLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(912, 644);
            this.Controls.Add(this.btnLogOut);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtContent);
            this.Name = "PostLogin";
            this.Text = "PostLogin";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.PostLogin_FormClosed);
            this.Load += new System.EventHandler(this.PostLogin_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Client.clientCloud clientObject;
        private System.Windows.Forms.TextBox txtContent;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnLogOut;
    }
}