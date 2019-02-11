namespace Mini
{
    partial class FormTestMsdfGen
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
            this.cmdTestMsdfGen = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cmdTestMsdfGen
            // 
            this.cmdTestMsdfGen.Location = new System.Drawing.Point(28, 25);
            this.cmdTestMsdfGen.Name = "cmdTestMsdfGen";
            this.cmdTestMsdfGen.Size = new System.Drawing.Size(115, 44);
            this.cmdTestMsdfGen.TabIndex = 3;
            this.cmdTestMsdfGen.Text = "TestMsdfGen";
            this.cmdTestMsdfGen.UseVisualStyleBackColor = true;
            this.cmdTestMsdfGen.Click += new System.EventHandler(this.cmdTestMsdfGen_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(28, 161);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(115, 44);
            this.button2.TabIndex = 5;
            this.button2.Text = "TestFakeMsdfGen";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // FormTestMsdfGen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.cmdTestMsdfGen);
            this.Name = "FormTestMsdfGen";
            this.Text = "FormTestMsdfGen";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button cmdTestMsdfGen;
        private System.Windows.Forms.Button button2;
    }
}