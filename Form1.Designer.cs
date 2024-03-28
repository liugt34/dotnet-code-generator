namespace CodeGenerator
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnStart = new Button();
            txtConnectionString = new TextBox();
            label1 = new Label();
            label2 = new Label();
            txtTableName = new TextBox();
            label3 = new Label();
            txtNamespace1 = new TextBox();
            label4 = new Label();
            txtNamespace2 = new TextBox();
            label5 = new Label();
            txtEntityName = new TextBox();
            label6 = new Label();
            txtEntityNameCN = new TextBox();
            rbRemark = new RichTextBox();
            SuspendLayout();
            // 
            // btnStart
            // 
            btnStart.Font = new Font("Microsoft YaHei UI", 10F);
            btnStart.Location = new Point(465, 155);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(134, 32);
            btnStart.TabIndex = 0;
            btnStart.Text = "生成代码";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // txtConnectionString
            // 
            txtConnectionString.Location = new Point(111, 15);
            txtConnectionString.Name = "txtConnectionString";
            txtConnectionString.Size = new Size(488, 23);
            txtConnectionString.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(13, 18);
            label1.Name = "label1";
            label1.Size = new Size(92, 17);
            label1.TabIndex = 2;
            label1.Text = "数据库连接串：";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(61, 132);
            label2.Name = "label2";
            label2.Size = new Size(44, 17);
            label2.TabIndex = 3;
            label2.Text = "表名：";
            label2.TextAlign = ContentAlignment.TopCenter;
            // 
            // txtTableName
            // 
            txtTableName.Location = new Point(111, 126);
            txtTableName.Name = "txtTableName";
            txtTableName.Size = new Size(210, 23);
            txtTableName.TabIndex = 4;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(30, 58);
            label3.Name = "label3";
            label3.Size = new Size(75, 17);
            label3.TabIndex = 5;
            label3.Text = "命名空间1：";
            // 
            // txtNamespace1
            // 
            txtNamespace1.Location = new Point(111, 52);
            txtNamespace1.Name = "txtNamespace1";
            txtNamespace1.Size = new Size(210, 23);
            txtNamespace1.TabIndex = 6;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(30, 94);
            label4.Name = "label4";
            label4.Size = new Size(75, 17);
            label4.TabIndex = 5;
            label4.Text = "命名空间2：";
            // 
            // txtNamespace2
            // 
            txtNamespace2.Location = new Point(111, 88);
            txtNamespace2.Name = "txtNamespace2";
            txtNamespace2.Size = new Size(210, 23);
            txtNamespace2.TabIndex = 6;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(339, 129);
            label5.Name = "label5";
            label5.Size = new Size(56, 17);
            label5.TabIndex = 3;
            label5.Text = "实体名：";
            label5.TextAlign = ContentAlignment.TopCenter;
            // 
            // txtEntityName
            // 
            txtEntityName.Location = new Point(401, 126);
            txtEntityName.Name = "txtEntityName";
            txtEntityName.Size = new Size(198, 23);
            txtEntityName.TabIndex = 4;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(25, 167);
            label6.Name = "label6";
            label6.Size = new Size(80, 17);
            label6.TabIndex = 3;
            label6.Text = "实体中文名：";
            label6.TextAlign = ContentAlignment.TopCenter;
            // 
            // txtEntityNameCN
            // 
            txtEntityNameCN.Location = new Point(111, 164);
            txtEntityNameCN.Name = "txtEntityNameCN";
            txtEntityNameCN.Size = new Size(210, 23);
            txtEntityNameCN.TabIndex = 4;
            // 
            // rbRemark
            // 
            rbRemark.Location = new Point(5, 201);
            rbRemark.Name = "rbRemark";
            rbRemark.Size = new Size(623, 388);
            rbRemark.TabIndex = 7;
            rbRemark.Text = "";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(632, 601);
            Controls.Add(rbRemark);
            Controls.Add(txtNamespace2);
            Controls.Add(txtNamespace1);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(txtEntityName);
            Controls.Add(label5);
            Controls.Add(txtEntityNameCN);
            Controls.Add(label6);
            Controls.Add(txtTableName);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(txtConnectionString);
            Controls.Add(btnStart);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnStart;
        private TextBox txtConnectionString;
        private Label label1;
        private Label label2;
        private TextBox txtTableName;
        private Label label3;
        private TextBox txtNamespace1;
        private Label label4;
        private TextBox txtNamespace2;
        private Label label5;
        private TextBox txtEntityName;
        private Label label6;
        private TextBox txtEntityNameCN;
        private RichTextBox rbRemark;
    }
}
