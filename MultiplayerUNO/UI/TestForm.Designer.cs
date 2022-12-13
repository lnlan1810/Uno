namespace MultiplayerUNO.UI {
    partial class TestForm {
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
            this.label1 = new System.Windows.Forms.Label();
            this.PnlInfo = new System.Windows.Forms.Panel();
            this.LblInfo = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.BtnZIndex = new System.Windows.Forms.Button();
            this.LblTestAlign = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.LblChooseCard = new System.Windows.Forms.Label();
            this.BtnAddAlpha = new System.Windows.Forms.Button();
            this.LblAlpha = new System.Windows.Forms.Label();
            this.LblTest = new System.Windows.Forms.Label();
            this.btnTest = new System.Windows.Forms.Button();
            this.PnlTest = new System.Windows.Forms.Panel();
            this.PnlVisibleFalse = new System.Windows.Forms.Panel();
            this.PnlInfo.SuspendLayout();
            this.PnlTest.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 57);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(145, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "Please enter cardid";
            // 
            // PnlInfo
            // 
            this.PnlInfo.Controls.Add(this.LblInfo);
            this.PnlInfo.Controls.Add(this.label1);
            this.PnlInfo.Controls.Add(this.textBox1);
            this.PnlInfo.Location = new System.Drawing.Point(585, 16);
            this.PnlInfo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.PnlInfo.Name = "PnlInfo";
            this.PnlInfo.Size = new System.Drawing.Size(435, 281);
            this.PnlInfo.TabIndex = 3;
            // 
            // LblInfo
            // 
            this.LblInfo.AutoSize = true;
            this.LblInfo.Location = new System.Drawing.Point(58, 183);
            this.LblInfo.Name = "LblInfo";
            this.LblInfo.Size = new System.Drawing.Size(51, 20);
            this.LblInfo.TabIndex = 3;
            this.LblInfo.Text = "label2";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(184, 53);
            this.textBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(112, 26);
            this.textBox1.TabIndex = 0;
            this.textBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(219, 293);
            this.button2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(134, 89);
            this.button2.TabIndex = 5;
            this.button2.Text = "add mobile";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // BtnZIndex
            // 
            this.BtnZIndex.Location = new System.Drawing.Point(955, 349);
            this.BtnZIndex.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.BtnZIndex.Name = "BtnZIndex";
            this.BtnZIndex.Size = new System.Drawing.Size(134, 92);
            this.BtnZIndex.TabIndex = 6;
            this.BtnZIndex.Text = "change level";
            this.BtnZIndex.UseVisualStyleBackColor = true;
            this.BtnZIndex.Click += new System.EventHandler(this.BtnZIndex_Click);
            // 
            // LblTestAlign
            // 
            this.LblTestAlign.AutoSize = true;
            this.LblTestAlign.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LblTestAlign.Location = new System.Drawing.Point(450, 400);
            this.LblTestAlign.Name = "LblTestAlign";
            this.LblTestAlign.Size = new System.Drawing.Size(53, 22);
            this.LblTestAlign.TabIndex = 7;
            this.LblTestAlign.Text = "label2";
            // 
            // label3
            // 
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.Font = new System.Drawing.Font("Microsoft YaHei", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(450, 400);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(112, 133);
            this.label3.TabIndex = 8;
            this.label3.Text = "label3";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(611, 599);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(126, 88);
            this.button1.TabIndex = 9;
            this.button1.Text = "move (0,0)";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // LblChooseCard
            // 
            this.LblChooseCard.AutoSize = true;
            this.LblChooseCard.Location = new System.Drawing.Point(70, 318);
            this.LblChooseCard.Name = "LblChooseCard";
            this.LblChooseCard.Size = new System.Drawing.Size(101, 20);
            this.LblChooseCard.TabIndex = 10;
            this.LblChooseCard.Text = "playing cards";
            this.LblChooseCard.Click += new System.EventHandler(this.LblChooseCard_Click);
            // 
            // BtnAddAlpha
            // 
            this.BtnAddAlpha.Location = new System.Drawing.Point(790, 492);
            this.BtnAddAlpha.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.BtnAddAlpha.Name = "BtnAddAlpha";
            this.BtnAddAlpha.Size = new System.Drawing.Size(134, 89);
            this.BtnAddAlpha.TabIndex = 11;
            this.BtnAddAlpha.Text = "add transparency change";
            this.BtnAddAlpha.UseVisualStyleBackColor = true;
            this.BtnAddAlpha.Click += new System.EventHandler(this.BtnAddAlpha_Click);
            // 
            // LblAlpha
            // 
            this.LblAlpha.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.LblAlpha.Location = new System.Drawing.Point(90, 512);
            this.LblAlpha.Name = "LblAlpha";
            this.LblAlpha.Size = new System.Drawing.Size(112, 133);
            this.LblAlpha.TabIndex = 12;
            // 
            // LblTest
            // 
            this.LblTest.AutoSize = true;
            this.LblTest.Location = new System.Drawing.Point(39, 40);
            this.LblTest.Name = "LblTest";
            this.LblTest.Size = new System.Drawing.Size(36, 20);
            this.LblTest.TabIndex = 13;
            this.LblTest.Text = "test";
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(324, 599);
            this.btnTest.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(168, 119);
            this.btnTest.TabIndex = 14;
            this.btnTest.Text = "show test results";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // PnlTest
            // 
            this.PnlTest.Controls.Add(this.LblTest);
            this.PnlTest.Location = new System.Drawing.Point(14, 732);
            this.PnlTest.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.PnlTest.Name = "PnlTest";
            this.PnlTest.Size = new System.Drawing.Size(1040, 77);
            this.PnlTest.TabIndex = 15;
            // 
            // PnlVisibleFalse
            // 
            this.PnlVisibleFalse.Location = new System.Drawing.Point(115, 120);
            this.PnlVisibleFalse.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.PnlVisibleFalse.Name = "PnlVisibleFalse";
            this.PnlVisibleFalse.Size = new System.Drawing.Size(225, 133);
            this.PnlVisibleFalse.TabIndex = 16;
            this.PnlVisibleFalse.Visible = false;
            // 
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1102, 825);
            this.Controls.Add(this.PnlVisibleFalse);
            this.Controls.Add(this.PnlTest);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.LblAlpha);
            this.Controls.Add(this.BtnAddAlpha);
            this.Controls.Add(this.LblChooseCard);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.LblTestAlign);
            this.Controls.Add(this.BtnZIndex);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.PnlInfo);
            this.Controls.Add(this.label3);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "TestForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TestForm";
            this.Load += new System.EventHandler(this.TestForm_Load);
            this.PnlInfo.ResumeLayout(false);
            this.PnlInfo.PerformLayout();
            this.PnlTest.ResumeLayout(false);
            this.PnlTest.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel PnlInfo;
        private System.Windows.Forms.Label LblInfo;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button BtnZIndex;
        private System.Windows.Forms.Label LblTestAlign;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label LblChooseCard;
        private System.Windows.Forms.Button BtnAddAlpha;
        private System.Windows.Forms.Label LblAlpha;
        private System.Windows.Forms.Label LblTest;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.Panel PnlTest;
        private System.Windows.Forms.Panel PnlVisibleFalse;
    }
}