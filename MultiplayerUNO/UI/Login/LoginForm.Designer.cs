namespace MultiplayerUNO.UI.Login {
    partial class LoginForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.BtnClient = new System.Windows.Forms.Button();
            this.BtnServer = new System.Windows.Forms.Button();
            this.GrpUserInfo = new System.Windows.Forms.GroupBox();
            this.BtnJoinGame = new System.Windows.Forms.Button();
            this.BtnRechoose = new System.Windows.Forms.Button();
            this.TxtUserName = new System.Windows.Forms.TextBox();
            this.LblUserName = new System.Windows.Forms.Label();
            this.TxtPort = new System.Windows.Forms.TextBox();
            this.LblPort = new System.Windows.Forms.Label();
            this.TxtHost = new System.Windows.Forms.TextBox();
            this.LblHost = new System.Windows.Forms.Label();
            this.GrpReady = new System.Windows.Forms.GroupBox();
            this.BtnCancelReady = new System.Windows.Forms.Button();
            this.LblReadyInfo = new System.Windows.Forms.Label();
            this.BtnReady = new System.Windows.Forms.Button();
            this.BtnStart = new System.Windows.Forms.Button();
            this.GrpUserInfo.SuspendLayout();
            this.GrpReady.SuspendLayout();
            this.SuspendLayout();
            // 
            // BtnClient
            // 
            this.BtnClient.Location = new System.Drawing.Point(14, 340);
            this.BtnClient.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.BtnClient.Name = "BtnClient";
            this.BtnClient.Size = new System.Drawing.Size(132, 124);
            this.BtnClient.TabIndex = 3;
            this.BtnClient.Text = "client";
            this.BtnClient.UseVisualStyleBackColor = true;
            this.BtnClient.Click += new System.EventHandler(this.BtnClient_Click);
            // 
            // BtnServer
            // 
            this.BtnServer.Location = new System.Drawing.Point(14, 472);
            this.BtnServer.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.BtnServer.Name = "BtnServer";
            this.BtnServer.Size = new System.Drawing.Size(132, 124);
            this.BtnServer.TabIndex = 2;
            this.BtnServer.Text = "server";
            this.BtnServer.UseVisualStyleBackColor = true;
            this.BtnServer.Click += new System.EventHandler(this.BtnServer_Click);
            // 
            // GrpUserInfo
            // 
            this.GrpUserInfo.Controls.Add(this.BtnJoinGame);
            this.GrpUserInfo.Controls.Add(this.BtnRechoose);
            this.GrpUserInfo.Controls.Add(this.TxtUserName);
            this.GrpUserInfo.Controls.Add(this.LblUserName);
            this.GrpUserInfo.Controls.Add(this.TxtPort);
            this.GrpUserInfo.Controls.Add(this.LblPort);
            this.GrpUserInfo.Controls.Add(this.TxtHost);
            this.GrpUserInfo.Controls.Add(this.LblHost);
            this.GrpUserInfo.Location = new System.Drawing.Point(247, 55);
            this.GrpUserInfo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.GrpUserInfo.Name = "GrpUserInfo";
            this.GrpUserInfo.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.GrpUserInfo.Size = new System.Drawing.Size(464, 512);
            this.GrpUserInfo.TabIndex = 4;
            this.GrpUserInfo.TabStop = false;
            this.GrpUserInfo.Text = "please enter your information";
            // 
            // BtnJoinGame
            // 
            this.BtnJoinGame.Location = new System.Drawing.Point(44, 396);
            this.BtnJoinGame.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.BtnJoinGame.Name = "BtnJoinGame";
            this.BtnJoinGame.Size = new System.Drawing.Size(141, 91);
            this.BtnJoinGame.TabIndex = 5;
            this.BtnJoinGame.Text = "join the game";
            this.BtnJoinGame.UseVisualStyleBackColor = true;
            this.BtnJoinGame.Click += new System.EventHandler(this.BtnJoinGame_Click);
            // 
            // BtnRechoose
            // 
            this.BtnRechoose.Location = new System.Drawing.Point(276, 396);
            this.BtnRechoose.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.BtnRechoose.Name = "BtnRechoose";
            this.BtnRechoose.Size = new System.Drawing.Size(141, 91);
            this.BtnRechoose.TabIndex = 6;
            this.BtnRechoose.Text = "previous";
            this.BtnRechoose.UseVisualStyleBackColor = true;
            this.BtnRechoose.Click += new System.EventHandler(this.BtnRechoose_Click);
            // 
            // TxtUserName
            // 
            this.TxtUserName.Location = new System.Drawing.Point(210, 268);
            this.TxtUserName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.TxtUserName.Name = "TxtUserName";
            this.TxtUserName.Size = new System.Drawing.Size(188, 26);
            this.TxtUserName.TabIndex = 5;
            this.TxtUserName.Text = "Alice";
            // 
            // LblUserName
            // 
            this.LblUserName.Location = new System.Drawing.Point(27, 251);
            this.LblUserName.Name = "LblUserName";
            this.LblUserName.Size = new System.Drawing.Size(137, 61);
            this.LblUserName.TabIndex = 4;
            this.LblUserName.Text = "player nickname";
            this.LblUserName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TxtPort
            // 
            this.TxtPort.Location = new System.Drawing.Point(210, 171);
            this.TxtPort.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.TxtPort.Name = "TxtPort";
            this.TxtPort.Size = new System.Drawing.Size(188, 26);
            this.TxtPort.TabIndex = 3;
            this.TxtPort.Text = "25564";
            // 
            // LblPort
            // 
            this.LblPort.Location = new System.Drawing.Point(27, 153);
            this.LblPort.Name = "LblPort";
            this.LblPort.Size = new System.Drawing.Size(137, 61);
            this.LblPort.TabIndex = 2;
            this.LblPort.Text = "port infomation";
            this.LblPort.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TxtHost
            // 
            this.TxtHost.Location = new System.Drawing.Point(210, 80);
            this.TxtHost.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.TxtHost.Name = "TxtHost";
            this.TxtHost.Size = new System.Drawing.Size(188, 26);
            this.TxtHost.TabIndex = 1;
            this.TxtHost.Text = "127.0.0.1";
            // 
            // LblHost
            // 
            this.LblHost.Location = new System.Drawing.Point(27, 63);
            this.LblHost.Name = "LblHost";
            this.LblHost.Size = new System.Drawing.Size(137, 61);
            this.LblHost.TabIndex = 0;
            this.LblHost.Text = "domain information";
            this.LblHost.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // GrpReady
            // 
            this.GrpReady.Controls.Add(this.BtnCancelReady);
            this.GrpReady.Controls.Add(this.LblReadyInfo);
            this.GrpReady.Controls.Add(this.BtnReady);
            this.GrpReady.Controls.Add(this.BtnStart);
            this.GrpReady.Location = new System.Drawing.Point(762, 323);
            this.GrpReady.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.GrpReady.Name = "GrpReady";
            this.GrpReady.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.GrpReady.Size = new System.Drawing.Size(464, 512);
            this.GrpReady.TabIndex = 7;
            this.GrpReady.TabStop = false;
            this.GrpReady.Text = "game ready";
            // 
            // BtnCancelReady
            // 
            this.BtnCancelReady.Location = new System.Drawing.Point(255, 233);
            this.BtnCancelReady.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.BtnCancelReady.Name = "BtnCancelReady";
            this.BtnCancelReady.Size = new System.Drawing.Size(141, 91);
            this.BtnCancelReady.TabIndex = 8;
            this.BtnCancelReady.Text = "cancel preparation";
            this.BtnCancelReady.UseVisualStyleBackColor = true;
            this.BtnCancelReady.Click += new System.EventHandler(this.BtnCancelReady_Click);
            // 
            // LblReadyInfo
            // 
            this.LblReadyInfo.Location = new System.Drawing.Point(89, 83);
            this.LblReadyInfo.Name = "LblReadyInfo";
            this.LblReadyInfo.Size = new System.Drawing.Size(307, 61);
            this.LblReadyInfo.TabIndex = 7;
            this.LblReadyInfo.Text = "0/0 Player is ready";
            this.LblReadyInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // BtnReady
            // 
            this.BtnReady.Location = new System.Drawing.Point(62, 233);
            this.BtnReady.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.BtnReady.Name = "BtnReady";
            this.BtnReady.Size = new System.Drawing.Size(141, 91);
            this.BtnReady.TabIndex = 5;
            this.BtnReady.Text = "ready for game";
            this.BtnReady.UseVisualStyleBackColor = true;
            this.BtnReady.Click += new System.EventHandler(this.BtnReady_Click);
            // 
            // BtnStart
            // 
            this.BtnStart.Location = new System.Drawing.Point(162, 368);
            this.BtnStart.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.BtnStart.Name = "BtnStart";
            this.BtnStart.Size = new System.Drawing.Size(141, 91);
            this.BtnStart.TabIndex = 6;
            this.BtnStart.Text = "start the game";
            this.BtnStart.UseVisualStyleBackColor = true;
            this.BtnStart.Click += new System.EventHandler(this.BtnStart_Click);
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 600);
            this.Controls.Add(this.GrpUserInfo);
            this.Controls.Add(this.GrpReady);
            this.Controls.Add(this.BtnClient);
            this.Controls.Add(this.BtnServer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoginForm";
            this.Text = "LoginForm";
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.GrpUserInfo.ResumeLayout(false);
            this.GrpUserInfo.PerformLayout();
            this.GrpReady.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button BtnClient;
        private System.Windows.Forms.Button BtnServer;
        private System.Windows.Forms.GroupBox GrpUserInfo;
        private System.Windows.Forms.Label LblHost;
        private System.Windows.Forms.TextBox TxtHost;
        private System.Windows.Forms.TextBox TxtPort;
        private System.Windows.Forms.Label LblPort;
        private System.Windows.Forms.TextBox TxtUserName;
        private System.Windows.Forms.Label LblUserName;
        private System.Windows.Forms.Button BtnJoinGame;
        private System.Windows.Forms.Button BtnRechoose;
        private System.Windows.Forms.GroupBox GrpReady;
        private System.Windows.Forms.Button BtnReady;
        private System.Windows.Forms.Button BtnStart;
        private System.Windows.Forms.Label LblReadyInfo;
        private System.Windows.Forms.Button BtnCancelReady;
    }
}