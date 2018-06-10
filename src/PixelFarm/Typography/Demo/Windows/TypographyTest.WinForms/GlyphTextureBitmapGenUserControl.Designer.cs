namespace TypographyTest.WinForms
{
    partial class GlyphTextureBitmapGenUserControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lstTextureType = new System.Windows.Forms.ListBox();
            this.cmdMakeFromSelectedString = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.cmdMakeFromScriptLangs = new System.Windows.Forms.Button();
            this.chkSaveEachGlyph = new System.Windows.Forms.CheckBox();
            this.cmbSpaceCompactOption = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // lstTextureType
            // 
            this.lstTextureType.FormattingEnabled = true;
            this.lstTextureType.Location = new System.Drawing.Point(3, 3);
            this.lstTextureType.Name = "lstTextureType";
            this.lstTextureType.Size = new System.Drawing.Size(133, 69);
            this.lstTextureType.TabIndex = 0;
            // 
            // cmdMakeFromSelectedString
            // 
            this.cmdMakeFromSelectedString.Location = new System.Drawing.Point(3, 104);
            this.cmdMakeFromSelectedString.Name = "cmdMakeFromSelectedString";
            this.cmdMakeFromSelectedString.Size = new System.Drawing.Size(178, 32);
            this.cmdMakeFromSelectedString.TabIndex = 1;
            this.cmdMakeFromSelectedString.Text = "Make from Sample Text above";
            this.cmdMakeFromSelectedString.UseVisualStyleBackColor = true;
            this.cmdMakeFromSelectedString.Click += new System.EventHandler(this.cmdMakeFromSelectedString_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(3, 78);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(178, 20);
            this.textBox1.TabIndex = 2;
            // 
            // cmdMakeFromScriptLangs
            // 
            this.cmdMakeFromScriptLangs.Location = new System.Drawing.Point(216, 3);
            this.cmdMakeFromScriptLangs.Name = "cmdMakeFromScriptLangs";
            this.cmdMakeFromScriptLangs.Size = new System.Drawing.Size(83, 45);
            this.cmdMakeFromScriptLangs.TabIndex = 3;
            this.cmdMakeFromScriptLangs.Text = "Make from ScriptLang";
            this.cmdMakeFromScriptLangs.UseVisualStyleBackColor = true;
            this.cmdMakeFromScriptLangs.Click += new System.EventHandler(this.cmdMakeFromScriptLangs_Click);
            // 
            // chkSaveEachGlyph
            // 
            this.chkSaveEachGlyph.AutoSize = true;
            this.chkSaveEachGlyph.Location = new System.Drawing.Point(195, 78);
            this.chkSaveEachGlyph.Name = "chkSaveEachGlyph";
            this.chkSaveEachGlyph.Size = new System.Drawing.Size(106, 17);
            this.chkSaveEachGlyph.TabIndex = 5;
            this.chkSaveEachGlyph.Text = "Save each glyph";
            this.chkSaveEachGlyph.UseVisualStyleBackColor = true;
            // 
            // cmbSpaceCompactOption
            // 
            this.cmbSpaceCompactOption.FormattingEnabled = true;
            this.cmbSpaceCompactOption.Location = new System.Drawing.Point(195, 50);
            this.cmbSpaceCompactOption.Name = "cmbSpaceCompactOption";
            this.cmbSpaceCompactOption.Size = new System.Drawing.Size(121, 21);
            this.cmbSpaceCompactOption.TabIndex = 6;
            // 
            // GlyphTextureBitmapGenUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cmbSpaceCompactOption);
            this.Controls.Add(this.chkSaveEachGlyph);
            this.Controls.Add(this.cmdMakeFromScriptLangs);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.cmdMakeFromSelectedString);
            this.Controls.Add(this.lstTextureType);
            this.Name = "GlyphTextureBitmapGenUserControl";
            this.Size = new System.Drawing.Size(327, 142);
            this.Load += new System.EventHandler(this.GenGlyphBitmapTextureUserControl_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lstTextureType;
        private System.Windows.Forms.Button cmdMakeFromSelectedString;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button cmdMakeFromScriptLangs;
        private System.Windows.Forms.CheckBox chkSaveEachGlyph;
        private System.Windows.Forms.ComboBox cmbSpaceCompactOption;
    }
}
