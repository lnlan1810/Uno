namespace MultiplayerUNO.UI {
    partial class MainForm {
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
            this.components = new System.ComponentModel.Container();
            this.LblGetCard = new System.Windows.Forms.Label();
            this.TmrCheckLeftTime = new System.Windows.Forms.Timer(this.components);
            this.LblLeftTime = new System.Windows.Forms.Label();
            this.TmrControlGame = new System.Windows.Forms.Timer(this.components);
            this.LblFirstShowCard = new System.Windows.Forms.Label();
            this.PnlChooseColor = new System.Windows.Forms.Panel();
            this.LblQuestion = new System.Windows.Forms.Label();
            this.PnlQuestion = new System.Windows.Forms.Panel();
            this.LblNoQuestion = new System.Windows.Forms.Label();
            this.PnlDisplayCard = new System.Windows.Forms.Panel();
            this.LblDisplayCardsPlayerName = new System.Windows.Forms.Label();
            this.TmrDisplayCard = new System.Windows.Forms.Timer(this.components);
            this.LblGameOver = new System.Windows.Forms.Label();
            this.PnlPlus2 = new System.Windows.Forms.Panel();
            this.LblPlayPlus2 = new System.Windows.Forms.Label();
            this.LblDonotPlayPlus2 = new System.Windows.Forms.Label();
            this.TxtDebug = new System.Windows.Forms.TextBox();
            this.PnlAfterGetOne = new System.Windows.Forms.Panel();
            this.LblShowAfterGetOne = new System.Windows.Forms.Label();
            this.LblDonotShowAfterGetOne = new System.Windows.Forms.Label();
            this.PnlNormalShowCardorNot = new System.Windows.Forms.Panel();
            this.LblShowCard = new System.Windows.Forms.Label();
            this.PnlShowResultWhenGameOver = new System.Windows.Forms.Panel();
            this.LblColor = new System.Windows.Forms.Label();
            this.LblDirection = new System.Windows.Forms.Label();
            this.LblMsg = new System.Windows.Forms.Label();
            this.LblPlus2Total = new System.Windows.Forms.Label();
            this.LblGameOverShowInForm = new System.Windows.Forms.Label();
            this.PnlQuestion.SuspendLayout();
            this.PnlPlus2.SuspendLayout();
            this.PnlAfterGetOne.SuspendLayout();
            this.PnlNormalShowCardorNot.SuspendLayout();
            this.PnlShowResultWhenGameOver.SuspendLayout();
            this.SuspendLayout();
            // 
            // LblGetCard
            // 
            this.LblGetCard.AutoSize = true;
            this.LblGetCard.BackColor = System.Drawing.Color.LightCyan;
            this.LblGetCard.Font = new System.Drawing.Font("Microsoft YaHei", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LblGetCard.Location = new System.Drawing.Point(351, 72);
            this.LblGetCard.Name = "LblGetCard";
            this.LblGetCard.Padding = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.LblGetCard.Size = new System.Drawing.Size(172, 50);
            this.LblGetCard.TabIndex = 12;
            this.LblGetCard.Text = "draw cards";
            this.LblGetCard.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LblGetCard.Click += new System.EventHandler(this.LblGetCard_Click);
            // 
            // TmrCheckLeftTime
            // 
            this.TmrCheckLeftTime.Interval = 500;
            this.TmrCheckLeftTime.Tick += new System.EventHandler(this.TmrCheckLeftTime_Tick);
            // 
            // LblLeftTime
            // 
            this.LblLeftTime.BackColor = System.Drawing.Color.LightCyan;
            this.LblLeftTime.Font = new System.Drawing.Font("Microsoft YaHei", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LblLeftTime.Location = new System.Drawing.Point(22, 27);
            this.LblLeftTime.Name = "LblLeftTime";
            this.LblLeftTime.Padding = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.LblLeftTime.Size = new System.Drawing.Size(68, 80);
            this.LblLeftTime.TabIndex = 14;
            this.LblLeftTime.Text = "60";
            this.LblLeftTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LblLeftTime.Visible = false;
            // 
            // TmrControlGame
            // 
            this.TmrControlGame.Interval = 500;
            this.TmrControlGame.Tick += new System.EventHandler(this.TmrControlGame_Tick);
            // 
            // LblFirstShowCard
            // 
            this.LblFirstShowCard.AutoSize = true;
            this.LblFirstShowCard.BackColor = System.Drawing.Color.LightCyan;
            this.LblFirstShowCard.Font = new System.Drawing.Font("Microsoft YaHei", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LblFirstShowCard.Location = new System.Drawing.Point(623, 64);
            this.LblFirstShowCard.Name = "LblFirstShowCard";
            this.LblFirstShowCard.Padding = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.LblFirstShowCard.Size = new System.Drawing.Size(257, 50);
            this.LblFirstShowCard.TabIndex = 17;
            this.LblFirstShowCard.Text = "random first card";
            this.LblFirstShowCard.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LblFirstShowCard.Visible = false;
            // 
            // PnlChooseColor
            // 
            this.PnlChooseColor.BackColor = System.Drawing.Color.Azure;
            this.PnlChooseColor.Location = new System.Drawing.Point(29, 149);
            this.PnlChooseColor.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.PnlChooseColor.Name = "PnlChooseColor";
            this.PnlChooseColor.Size = new System.Drawing.Size(350, 127);
            this.PnlChooseColor.TabIndex = 18;
            // 
            // LblQuestion
            // 
            this.LblQuestion.AutoSize = true;
            this.LblQuestion.BackColor = System.Drawing.Color.LightCyan;
            this.LblQuestion.Font = new System.Drawing.Font("Microsoft YaHei", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LblQuestion.Location = new System.Drawing.Point(10, 37);
            this.LblQuestion.Name = "LblQuestion";
            this.LblQuestion.Padding = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.LblQuestion.Size = new System.Drawing.Size(143, 50);
            this.LblQuestion.TabIndex = 20;
            this.LblQuestion.Text = "question";
            this.LblQuestion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LblQuestion.Click += new System.EventHandler(this.LblQuestion_Click);
            // 
            // PnlQuestion
            // 
            this.PnlQuestion.BackColor = System.Drawing.Color.OliveDrab;
            this.PnlQuestion.Controls.Add(this.LblQuestion);
            this.PnlQuestion.Controls.Add(this.LblNoQuestion);
            this.PnlQuestion.Location = new System.Drawing.Point(876, 27);
            this.PnlQuestion.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.PnlQuestion.Name = "PnlQuestion";
            this.PnlQuestion.Size = new System.Drawing.Size(490, 114);
            this.PnlQuestion.TabIndex = 21;
            // 
            // LblNoQuestion
            // 
            this.LblNoQuestion.AutoSize = true;
            this.LblNoQuestion.BackColor = System.Drawing.Color.LightCyan;
            this.LblNoQuestion.Font = new System.Drawing.Font("Microsoft YaHei", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LblNoQuestion.Location = new System.Drawing.Point(203, 37);
            this.LblNoQuestion.Name = "LblNoQuestion";
            this.LblNoQuestion.Padding = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.LblNoQuestion.Size = new System.Drawing.Size(240, 50);
            this.LblNoQuestion.TabIndex = 21;
            this.LblNoQuestion.Text = "do not question";
            this.LblNoQuestion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LblNoQuestion.Click += new System.EventHandler(this.LblNoQuestion_Click);
            // 
            // PnlDisplayCard
            // 
            this.PnlDisplayCard.AutoSize = true;
            this.PnlDisplayCard.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.PnlDisplayCard.BackColor = System.Drawing.Color.DodgerBlue;
            this.PnlDisplayCard.Location = new System.Drawing.Point(441, 223);
            this.PnlDisplayCard.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.PnlDisplayCard.Name = "PnlDisplayCard";
            this.PnlDisplayCard.Size = new System.Drawing.Size(0, 0);
            this.PnlDisplayCard.TabIndex = 22;
            this.PnlDisplayCard.VisibleChanged += new System.EventHandler(this.PnlDisplayCard_VisibleChanged);
            // 
            // LblDisplayCardsPlayerName
            // 
            this.LblDisplayCardsPlayerName.AutoSize = true;
            this.LblDisplayCardsPlayerName.BackColor = System.Drawing.Color.GhostWhite;
            this.LblDisplayCardsPlayerName.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LblDisplayCardsPlayerName.Location = new System.Drawing.Point(413, 201);
            this.LblDisplayCardsPlayerName.Name = "LblDisplayCardsPlayerName";
            this.LblDisplayCardsPlayerName.Size = new System.Drawing.Size(0, 18);
            this.LblDisplayCardsPlayerName.TabIndex = 0;
            this.LblDisplayCardsPlayerName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LblDisplayCardsPlayerName.Click += new System.EventHandler(this.LblDisplayCardsPlayerName_Click);
            // 
            // TmrDisplayCard
            // 
            this.TmrDisplayCard.Interval = 5000;
            this.TmrDisplayCard.Tick += new System.EventHandler(this.TmrDisplayCard_Tick);
            // 
            // LblGameOver
            // 
            this.LblGameOver.AutoSize = true;
            this.LblGameOver.BackColor = System.Drawing.Color.Silver;
            this.LblGameOver.Dock = System.Windows.Forms.DockStyle.Top;
            this.LblGameOver.Font = new System.Drawing.Font("Microsoft YaHei", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LblGameOver.Location = new System.Drawing.Point(0, 0);
            this.LblGameOver.Name = "LblGameOver";
            this.LblGameOver.Padding = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.LblGameOver.Size = new System.Drawing.Size(490, 50);
            this.LblGameOver.TabIndex = 22;
            this.LblGameOver.Text = "The game is over and the winner is";
            this.LblGameOver.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // PnlPlus2
            // 
            this.PnlPlus2.AutoSize = true;
            this.PnlPlus2.BackColor = System.Drawing.Color.OliveDrab;
            this.PnlPlus2.Controls.Add(this.LblPlayPlus2);
            this.PnlPlus2.Controls.Add(this.LblDonotPlayPlus2);
            this.PnlPlus2.Location = new System.Drawing.Point(34, 315);
            this.PnlPlus2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.PnlPlus2.Name = "PnlPlus2";
            this.PnlPlus2.Size = new System.Drawing.Size(435, 160);
            this.PnlPlus2.TabIndex = 22;
            this.PnlPlus2.VisibleChanged += new System.EventHandler(this.PnlPlus2_VisibleChanged);
            // 
            // LblPlayPlus2
            // 
            this.LblPlayPlus2.AutoSize = true;
            this.LblPlayPlus2.BackColor = System.Drawing.Color.LightCyan;
            this.LblPlayPlus2.Font = new System.Drawing.Font("Microsoft YaHei", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LblPlayPlus2.Location = new System.Drawing.Point(16, 60);
            this.LblPlayPlus2.Name = "LblPlayPlus2";
            this.LblPlayPlus2.Padding = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.LblPlayPlus2.Size = new System.Drawing.Size(106, 50);
            this.LblPlayPlus2.TabIndex = 20;
            this.LblPlayPlus2.Text = "hit +2";
            this.LblPlayPlus2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LblPlayPlus2.Click += new System.EventHandler(this.LblPlayPlus2_Click);
            // 
            // LblDonotPlayPlus2
            // 
            this.LblDonotPlayPlus2.AutoSize = true;
            this.LblDonotPlayPlus2.BackColor = System.Drawing.Color.LightCyan;
            this.LblDonotPlayPlus2.Font = new System.Drawing.Font("Microsoft YaHei", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LblDonotPlayPlus2.Location = new System.Drawing.Point(205, 60);
            this.LblDonotPlayPlus2.Name = "LblDonotPlayPlus2";
            this.LblDonotPlayPlus2.Padding = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.LblDonotPlayPlus2.Size = new System.Drawing.Size(227, 50);
            this.LblDonotPlayPlus2.TabIndex = 21;
            this.LblDonotPlayPlus2.Text = "Do not play +2";
            this.LblDonotPlayPlus2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LblDonotPlayPlus2.Click += new System.EventHandler(this.LblDonotPlayPlus2_Click);
            // 
            // TxtDebug
            // 
            this.TxtDebug.Location = new System.Drawing.Point(1242, 149);
            this.TxtDebug.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.TxtDebug.Multiline = true;
            this.TxtDebug.Name = "TxtDebug";
            this.TxtDebug.ReadOnly = true;
            this.TxtDebug.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TxtDebug.Size = new System.Drawing.Size(579, 340);
            this.TxtDebug.TabIndex = 24;
            this.TxtDebug.WordWrap = false;
            this.TxtDebug.Click += new System.EventHandler(this.TxtDebug_Click);
            // 
            // PnlAfterGetOne
            // 
            this.PnlAfterGetOne.AutoSize = true;
            this.PnlAfterGetOne.BackColor = System.Drawing.Color.OliveDrab;
            this.PnlAfterGetOne.Controls.Add(this.LblShowAfterGetOne);
            this.PnlAfterGetOne.Controls.Add(this.LblDonotShowAfterGetOne);
            this.PnlAfterGetOne.Location = new System.Drawing.Point(34, 497);
            this.PnlAfterGetOne.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.PnlAfterGetOne.Name = "PnlAfterGetOne";
            this.PnlAfterGetOne.Size = new System.Drawing.Size(421, 191);
            this.PnlAfterGetOne.TabIndex = 25;
            // 
            // LblShowAfterGetOne
            // 
            this.LblShowAfterGetOne.AutoSize = true;
            this.LblShowAfterGetOne.BackColor = System.Drawing.Color.LightCyan;
            this.LblShowAfterGetOne.Font = new System.Drawing.Font("Microsoft YaHei", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LblShowAfterGetOne.Location = new System.Drawing.Point(63, 80);
            this.LblShowAfterGetOne.Name = "LblShowAfterGetOne";
            this.LblShowAfterGetOne.Padding = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.LblShowAfterGetOne.Size = new System.Drawing.Size(82, 50);
            this.LblShowAfterGetOne.TabIndex = 20;
            this.LblShowAfterGetOne.Text = "play";
            this.LblShowAfterGetOne.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LblShowAfterGetOne.Click += new System.EventHandler(this.LblShowAfterGetOne_Click);
            // 
            // LblDonotShowAfterGetOne
            // 
            this.LblDonotShowAfterGetOne.AutoSize = true;
            this.LblDonotShowAfterGetOne.BackColor = System.Drawing.Color.LightCyan;
            this.LblDonotShowAfterGetOne.Font = new System.Drawing.Font("Microsoft YaHei", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LblDonotShowAfterGetOne.Location = new System.Drawing.Point(239, 80);
            this.LblDonotShowAfterGetOne.Name = "LblDonotShowAfterGetOne";
            this.LblDonotShowAfterGetOne.Padding = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.LblDonotShowAfterGetOne.Size = new System.Drawing.Size(179, 50);
            this.LblDonotShowAfterGetOne.TabIndex = 21;
            this.LblDonotShowAfterGetOne.Text = "do not play";
            this.LblDonotShowAfterGetOne.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LblDonotShowAfterGetOne.Click += new System.EventHandler(this.LblDonotShowAfterGetOne_Click);
            // 
            // PnlNormalShowCardorNot
            // 
            this.PnlNormalShowCardorNot.AutoSize = true;
            this.PnlNormalShowCardorNot.BackColor = System.Drawing.SystemColors.HotTrack;
            this.PnlNormalShowCardorNot.Controls.Add(this.LblShowCard);
            this.PnlNormalShowCardorNot.Controls.Add(this.LblGetCard);
            this.PnlNormalShowCardorNot.Location = new System.Drawing.Point(700, 223);
            this.PnlNormalShowCardorNot.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.PnlNormalShowCardorNot.Name = "PnlNormalShowCardorNot";
            this.PnlNormalShowCardorNot.Size = new System.Drawing.Size(548, 191);
            this.PnlNormalShowCardorNot.TabIndex = 26;
            // 
            // LblShowCard
            // 
            this.LblShowCard.AutoSize = true;
            this.LblShowCard.BackColor = System.Drawing.Color.LightCyan;
            this.LblShowCard.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.LblShowCard.Font = new System.Drawing.Font("Microsoft YaHei", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LblShowCard.Location = new System.Drawing.Point(6, 72);
            this.LblShowCard.Name = "LblShowCard";
            this.LblShowCard.Padding = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.LblShowCard.Size = new System.Drawing.Size(203, 50);
            this.LblShowCard.TabIndex = 11;
            this.LblShowCard.Text = "playing cards";
            this.LblShowCard.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LblShowCard.Click += new System.EventHandler(this.LblShowCard_Click);
            // 
            // PnlShowResultWhenGameOver
            // 
            this.PnlShowResultWhenGameOver.BackColor = System.Drawing.Color.DodgerBlue;
            this.PnlShowResultWhenGameOver.Controls.Add(this.LblGameOver);
            this.PnlShowResultWhenGameOver.Location = new System.Drawing.Point(587, 472);
            this.PnlShowResultWhenGameOver.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.PnlShowResultWhenGameOver.Name = "PnlShowResultWhenGameOver";
            this.PnlShowResultWhenGameOver.Size = new System.Drawing.Size(532, 216);
            this.PnlShowResultWhenGameOver.TabIndex = 27;
            // 
            // LblColor
            // 
            this.LblColor.BackColor = System.Drawing.Color.Black;
            this.LblColor.Font = new System.Drawing.Font("Microsoft YaHei", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LblColor.Location = new System.Drawing.Point(197, 27);
            this.LblColor.Name = "LblColor";
            this.LblColor.Padding = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.LblColor.Size = new System.Drawing.Size(68, 80);
            this.LblColor.TabIndex = 16;
            this.LblColor.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LblColor.Visible = false;
            // 
            // LblDirection
            // 
            this.LblDirection.BackColor = System.Drawing.Color.LightGoldenrodYellow;
            this.LblDirection.Font = new System.Drawing.Font("Microsoft YaHei", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LblDirection.Location = new System.Drawing.Point(110, 27);
            this.LblDirection.Name = "LblDirection";
            this.LblDirection.Padding = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.LblDirection.Size = new System.Drawing.Size(68, 80);
            this.LblDirection.TabIndex = 15;
            this.LblDirection.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LblDirection.Visible = false;
            // 
            // LblMsg
            // 
            this.LblMsg.AutoSize = true;
            this.LblMsg.BackColor = System.Drawing.Color.LightCyan;
            this.LblMsg.Font = new System.Drawing.Font("Microsoft YaHei", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LblMsg.Location = new System.Drawing.Point(410, 149);
            this.LblMsg.Name = "LblMsg";
            this.LblMsg.Padding = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.LblMsg.Size = new System.Drawing.Size(576, 50);
            this.LblMsg.TabIndex = 28;
            this.LblMsg.Text = "Some hints will disappear after 2 seconds";
            this.LblMsg.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LblMsg.Visible = false;
            // 
            // LblPlus2Total
            // 
            this.LblPlus2Total.AutoSize = true;
            this.LblPlus2Total.BackColor = System.Drawing.Color.LightCyan;
            this.LblPlus2Total.Font = new System.Drawing.Font("Microsoft YaHei", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LblPlus2Total.Location = new System.Drawing.Point(299, 9);
            this.LblPlus2Total.Name = "LblPlus2Total";
            this.LblPlus2Total.Padding = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.LblPlus2Total.Size = new System.Drawing.Size(567, 50);
            this.LblPlus2Total.TabIndex = 22;
            this.LblPlus2Total.Text = "Current +2 cumulative number of sheets";
            this.LblPlus2Total.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LblGameOverShowInForm
            // 
            this.LblGameOverShowInForm.AutoSize = true;
            this.LblGameOverShowInForm.BackColor = System.Drawing.Color.LightCyan;
            this.LblGameOverShowInForm.Font = new System.Drawing.Font("Microsoft YaHei", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LblGameOverShowInForm.Location = new System.Drawing.Point(475, 418);
            this.LblGameOverShowInForm.Name = "LblGameOverShowInForm";
            this.LblGameOverShowInForm.Padding = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.LblGameOverShowInForm.Size = new System.Drawing.Size(490, 50);
            this.LblGameOverShowInForm.TabIndex = 29;
            this.LblGameOverShowInForm.Text = "The game is over and the winner is";
            this.LblGameOverShowInForm.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LblGameOverShowInForm.Visible = false;
            this.LblGameOverShowInForm.Click += new System.EventHandler(this.LblGameOverShowInForm_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DimGray;
            this.ClientSize = new System.Drawing.Size(1904, 771);
            this.Controls.Add(this.LblDisplayCardsPlayerName);
            this.Controls.Add(this.LblGameOverShowInForm);
            this.Controls.Add(this.LblMsg);
            this.Controls.Add(this.LblPlus2Total);
            this.Controls.Add(this.PnlShowResultWhenGameOver);
            this.Controls.Add(this.PnlNormalShowCardorNot);
            this.Controls.Add(this.PnlAfterGetOne);
            this.Controls.Add(this.TxtDebug);
            this.Controls.Add(this.PnlPlus2);
            this.Controls.Add(this.PnlDisplayCard);
            this.Controls.Add(this.PnlQuestion);
            this.Controls.Add(this.PnlChooseColor);
            this.Controls.Add(this.LblFirstShowCard);
            this.Controls.Add(this.LblColor);
            this.Controls.Add(this.LblDirection);
            this.Controls.Add(this.LblLeftTime);
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Text = "UNO Battle";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.PnlQuestion.ResumeLayout(false);
            this.PnlQuestion.PerformLayout();
            this.PnlPlus2.ResumeLayout(false);
            this.PnlPlus2.PerformLayout();
            this.PnlAfterGetOne.ResumeLayout(false);
            this.PnlAfterGetOne.PerformLayout();
            this.PnlNormalShowCardorNot.ResumeLayout(false);
            this.PnlNormalShowCardorNot.PerformLayout();
            this.PnlShowResultWhenGameOver.ResumeLayout(false);
            this.PnlShowResultWhenGameOver.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LblShowCard;
        private System.Windows.Forms.Label LblGetCard;
        private System.Windows.Forms.Timer TmrCheckLeftTime;
        private System.Windows.Forms.Label LblLeftTime;
        private System.Windows.Forms.Label LblDirection;
        private System.Windows.Forms.Label LblColor;
        private System.Windows.Forms.Timer TmrControlGame;
        private System.Windows.Forms.Label LblFirstShowCard;
        private System.Windows.Forms.Panel PnlChooseColor;
        private System.Windows.Forms.Label LblQuestion;
        private System.Windows.Forms.Panel PnlQuestion;
        private System.Windows.Forms.Label LblNoQuestion;
        private System.Windows.Forms.Panel PnlDisplayCard;
        private System.Windows.Forms.Label LblDisplayCardsPlayerName;
        private System.Windows.Forms.Timer TmrDisplayCard;
        private System.Windows.Forms.Label LblGameOver;
        private System.Windows.Forms.Panel PnlPlus2;
        private System.Windows.Forms.Label LblDonotPlayPlus2;
        private System.Windows.Forms.Label LblPlayPlus2;
        private System.Windows.Forms.TextBox TxtDebug;
        private System.Windows.Forms.Panel PnlAfterGetOne;
        private System.Windows.Forms.Label LblDonotShowAfterGetOne;
        private System.Windows.Forms.Label LblShowAfterGetOne;
        private System.Windows.Forms.Panel PnlNormalShowCardorNot;
        private System.Windows.Forms.Panel PnlShowResultWhenGameOver;
        private System.Windows.Forms.Label LblMsg;
        private System.Windows.Forms.Label LblPlus2Total;
        private System.Windows.Forms.Label LblGameOverShowInForm;
    }
}