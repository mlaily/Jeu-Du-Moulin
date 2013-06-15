namespace JeuDuMoulin
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
			this.humanhumanbtn = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.humanaibtn = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.aiaibtn = new System.Windows.Forms.ToolStripButton();
			this.labelVS = new System.Windows.Forms.Label();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.toolStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStrip1
			// 
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.humanhumanbtn,
            this.toolStripSeparator1,
            this.humanaibtn,
            this.toolStripSeparator2,
            this.aiaibtn});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(1218, 25);
			this.toolStrip1.TabIndex = 1;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// toolStripLabel1
			// 
			this.toolStripLabel1.Name = "toolStripLabel1";
			this.toolStripLabel1.Size = new System.Drawing.Size(68, 22);
			this.toolStripLabel1.Text = "New Game:";
			// 
			// humanhumanbtn
			// 
			this.humanhumanbtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.humanhumanbtn.Image = ((System.Drawing.Image)(resources.GetObject("humanhumanbtn.Image")));
			this.humanhumanbtn.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.humanhumanbtn.Name = "humanhumanbtn";
			this.humanhumanbtn.Size = new System.Drawing.Size(108, 22);
			this.humanhumanbtn.Text = "Human vs Human";
			this.humanhumanbtn.Click += new System.EventHandler(this.humanhumanbtn_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// humanaibtn
			// 
			this.humanaibtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.humanaibtn.Image = ((System.Drawing.Image)(resources.GetObject("humanaibtn.Image")));
			this.humanaibtn.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.humanaibtn.Name = "humanaibtn";
			this.humanaibtn.Size = new System.Drawing.Size(79, 22);
			this.humanaibtn.Text = "Human vs AI";
			this.humanaibtn.Click += new System.EventHandler(this.humanaibtn_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
			// 
			// aiaibtn
			// 
			this.aiaibtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.aiaibtn.Image = ((System.Drawing.Image)(resources.GetObject("aiaibtn.Image")));
			this.aiaibtn.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.aiaibtn.Name = "aiaibtn";
			this.aiaibtn.Size = new System.Drawing.Size(50, 22);
			this.aiaibtn.Text = "AI vs AI";
			this.aiaibtn.Click += new System.EventHandler(this.aiaibtn_Click);
			// 
			// labelVS
			// 
			this.labelVS.AutoSize = true;
			this.labelVS.Font = new System.Drawing.Font("Microsoft Sans Serif", 72F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelVS.Location = new System.Drawing.Point(519, 209);
			this.labelVS.Name = "labelVS";
			this.labelVS.Size = new System.Drawing.Size(173, 108);
			this.labelVS.TabIndex = 3;
			this.labelVS.Text = "VS";
			// 
			// textBox1
			// 
			this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox1.Location = new System.Drawing.Point(12, 539);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.ReadOnly = true;
			this.textBox1.Size = new System.Drawing.Size(1194, 154);
			this.textBox1.TabIndex = 4;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1218, 705);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.labelVS);
			this.Controls.Add(this.toolStrip1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.MaximizeBox = false;
			this.Name = "Form1";
			this.Text = "Form1";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton humanhumanbtn;
		private System.Windows.Forms.Label labelVS;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.ToolStripLabel toolStripLabel1;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripButton humanaibtn;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripButton aiaibtn;
	}
}

