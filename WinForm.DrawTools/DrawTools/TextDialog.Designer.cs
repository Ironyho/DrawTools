namespace DrawTools
{
	partial class TextDialog
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TextDialog));
			this.label1 = new System.Windows.Forms.Label();
			this.txtTheText = new System.Windows.Forms.TextBox();
			this.dlgFont = new System.Windows.Forms.FontDialog();
			this.btnFont = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(31, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Text:";
			// 
			// txtTheText
			// 
			this.txtTheText.Location = new System.Drawing.Point(50, 10);
			this.txtTheText.Name = "txtTheText";
			this.txtTheText.Size = new System.Drawing.Size(384, 20);
			this.txtTheText.TabIndex = 1;
			// 
			// btnFont
			// 
			this.btnFont.Image = ((System.Drawing.Image)(resources.GetObject("btnFont.Image")));
			this.btnFont.Location = new System.Drawing.Point(60, 66);
			this.btnFont.Name = "btnFont";
			this.btnFont.Size = new System.Drawing.Size(113, 25);
			this.btnFont.TabIndex = 2;
			this.btnFont.Text = "Text Font";
			this.btnFont.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this.btnFont.UseVisualStyleBackColor = true;
			this.btnFont.Click += new System.EventHandler(this.btnFont_Click);
			// 
			// btnOK
			// 
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(190, 67);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(113, 23);
			this.btnOK.TabIndex = 4;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(321, 67);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(113, 23);
			this.btnCancel.TabIndex = 5;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// TextDialog
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(459, 108);
			this.ControlBox = false;
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.btnFont);
			this.Controls.Add(this.txtTheText);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "TextDialog";
			this.ShowInTaskbar = false;
			this.Text = "Text Entry Dialog";
			this.Load += new System.EventHandler(this.TextDialog_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtTheText;
		private System.Windows.Forms.Button btnFont;
		private System.Windows.Forms.FontDialog dlgFont;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
	}
}