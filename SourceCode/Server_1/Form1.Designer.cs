namespace Server
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
            this.components = new System.ComponentModel.Container();
            this.tcpServer = new Server.ServerCloud(this.components);
            this.txtBoxPort = new System.Windows.Forms.TextBox();
            this.lblClientPort = new System.Windows.Forms.Label();
            this.txtBoxPortSync = new System.Windows.Forms.TextBox();
            this.startServer = new System.Windows.Forms.Button();
            this.lblServerPort = new System.Windows.Forms.Label();
            this.txtIPServ = new System.Windows.Forms.TextBox();
            this.lblIPServer = new System.Windows.Forms.Label();
            this.lblOtherServerPort = new System.Windows.Forms.Label();
            this.txtOtherServerPort = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // tcpServer
            // 
            this.tcpServer.IpOtherServer = null;
            this.tcpServer.Port = -1;
            this.tcpServer.PortSync = 0;
            // 
            // txtBoxPort
            // 
            this.txtBoxPort.Location = new System.Drawing.Point(441, 100);
            this.txtBoxPort.Name = "txtBoxPort";
            this.txtBoxPort.Size = new System.Drawing.Size(100, 22);
            this.txtBoxPort.TabIndex = 0;
            // 
            // lblClientPort
            // 
            this.lblClientPort.AutoSize = true;
            this.lblClientPort.Location = new System.Drawing.Point(206, 103);
            this.lblClientPort.Name = "lblClientPort";
            this.lblClientPort.Size = new System.Drawing.Size(198, 17);
            this.lblClientPort.TabIndex = 1;
            this.lblClientPort.Text = "Port number to client requests";
            // 
            // txtBoxPortSync
            // 
            this.txtBoxPortSync.Location = new System.Drawing.Point(441, 159);
            this.txtBoxPortSync.Name = "txtBoxPortSync";
            this.txtBoxPortSync.Size = new System.Drawing.Size(100, 22);
            this.txtBoxPortSync.TabIndex = 2;
            // 
            // startServer
            // 
            this.startServer.Location = new System.Drawing.Point(315, 289);
            this.startServer.Name = "startServer";
            this.startServer.Size = new System.Drawing.Size(134, 28);
            this.startServer.TabIndex = 3;
            this.startServer.Text = "Start Server";
            this.startServer.UseVisualStyleBackColor = true;
            this.startServer.Click += new System.EventHandler(this.startServer_Click);
            // 
            // lblServerPort
            // 
            this.lblServerPort.AutoSize = true;
            this.lblServerPort.Location = new System.Drawing.Point(209, 159);
            this.lblServerPort.Name = "lblServerPort";
            this.lblServerPort.Size = new System.Drawing.Size(207, 17);
            this.lblServerPort.TabIndex = 4;
            this.lblServerPort.Text = "Port number to Server requests";
            // 
            // txtIPServ
            // 
            this.txtIPServ.Location = new System.Drawing.Point(441, 211);
            this.txtIPServ.Name = "txtIPServ";
            this.txtIPServ.Size = new System.Drawing.Size(190, 22);
            this.txtIPServ.TabIndex = 5;
            // 
            // lblIPServer
            // 
            this.lblIPServer.AutoSize = true;
            this.lblIPServer.Location = new System.Drawing.Point(209, 211);
            this.lblIPServer.Name = "lblIPServer";
            this.lblIPServer.Size = new System.Drawing.Size(197, 17);
            this.lblIPServer.TabIndex = 6;
            this.lblIPServer.Text = "IP Address of the other server";
            // 
            // lblOtherServerPort
            // 
            this.lblOtherServerPort.AutoSize = true;
            this.lblOtherServerPort.Location = new System.Drawing.Point(209, 252);
            this.lblOtherServerPort.Name = "lblOtherServerPort";
            this.lblOtherServerPort.Size = new System.Drawing.Size(207, 17);
            this.lblOtherServerPort.TabIndex = 7;
            this.lblOtherServerPort.Text = "Port number of the other server";
            // 
            // txtOtherServerPort
            // 
            this.txtOtherServerPort.Location = new System.Drawing.Point(441, 249);
            this.txtOtherServerPort.Name = "txtOtherServerPort";
            this.txtOtherServerPort.Size = new System.Drawing.Size(190, 22);
            this.txtOtherServerPort.TabIndex = 8;
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(809, 408);
            this.Controls.Add(this.txtOtherServerPort);
            this.Controls.Add(this.lblOtherServerPort);
            this.Controls.Add(this.lblIPServer);
            this.Controls.Add(this.txtIPServ);
            this.Controls.Add(this.lblServerPort);
            this.Controls.Add(this.startServer);
            this.Controls.Add(this.txtBoxPortSync);
            this.Controls.Add(this.lblClientPort);
            this.Controls.Add(this.txtBoxPort);
            this.Name = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        
        
        private System.Windows.Forms.Label lblPort;
        private Server.ServerCloud tcpServer;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox txtBoxPort;
        private System.Windows.Forms.Label lblClientPort;
        private System.Windows.Forms.TextBox txtBoxPortSync;
        private System.Windows.Forms.Button startServer;
        private System.Windows.Forms.Label lblServerPort;
        private System.Windows.Forms.TextBox txtIPServ;
        private System.Windows.Forms.Label lblIPServer;
        private System.Windows.Forms.Label lblOtherServerPort;
        private System.Windows.Forms.TextBox txtOtherServerPort;
    }
}

