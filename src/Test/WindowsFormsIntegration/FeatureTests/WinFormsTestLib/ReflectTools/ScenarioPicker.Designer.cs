namespace ReflectTools
{
	partial class ScenarioPicker
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
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.tssLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnReset = new System.Windows.Forms.Button();
			this.llUncheck = new System.Windows.Forms.LinkLabel();
			this.btnOverride = new System.Windows.Forms.Button();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.chkPersist = new System.Windows.Forms.CheckBox();
			this.llCheckAll = new System.Windows.Forms.LinkLabel();
			this.tvScenarios = new System.Windows.Forms.TreeView();
			this.statusStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tssLabel});
			this.statusStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Table;
			this.statusStrip1.Location = new System.Drawing.Point(0, 317);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(307, 23);
			this.statusStrip1.TabIndex = 1;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// tssLabel
			// 
			this.tssLabel.Name = "tssLabel";
			this.tssLabel.Text = "Continuing in: 3 seconds";
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.Location = new System.Drawing.Point(139, 278);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 2;
			this.btnOK.Text = "&Ok";
			this.btnOK.Visible = false;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnReset
			// 
			this.btnReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnReset.Location = new System.Drawing.Point(220, 278);
			this.btnReset.Name = "btnReset";
			this.btnReset.Size = new System.Drawing.Size(75, 23);
			this.btnReset.TabIndex = 3;
			this.btnReset.Text = "&Reset";
			this.btnReset.Visible = false;
			this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
			// 
			// llUncheck
			// 
			this.llUncheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.llUncheck.AutoSize = true;
			this.llUncheck.Location = new System.Drawing.Point(14, 252);
			this.llUncheck.Name = "llUncheck";
			this.llUncheck.Size = new System.Drawing.Size(61, 13);
			this.llUncheck.TabIndex = 4;
			this.llUncheck.TabStop = true;
			this.llUncheck.Text = "Uncheck All";
			this.llUncheck.Visible = false;
			this.llUncheck.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llUncheck_LinkClicked);
			// 
			// btnOverride
			// 
			this.btnOverride.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOverride.Location = new System.Drawing.Point(220, 278);
			this.btnOverride.Name = "btnOverride";
			this.btnOverride.Size = new System.Drawing.Size(75, 23);
			this.btnOverride.TabIndex = 5;
			this.btnOverride.Text = "Override";
			this.btnOverride.Click += new System.EventHandler(this.btnOverride_Click);
			// 
			// timer1
			// 
			this.timer1.Interval = 1000;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// chkPersist
			// 
			this.chkPersist.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.chkPersist.AutoSize = true;
			this.chkPersist.Location = new System.Drawing.Point(16, 284);
			this.chkPersist.Name = "chkPersist";
			this.chkPersist.Size = new System.Drawing.Size(99, 17);
			this.chkPersist.TabIndex = 6;
			this.chkPersist.Text = "Save Selections";
			this.chkPersist.Visible = false;
			// 
			// llCheckAll
			// 
			this.llCheckAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.llCheckAll.AutoSize = true;
			this.llCheckAll.Location = new System.Drawing.Point(90, 252);
			this.llCheckAll.Name = "llCheckAll";
			this.llCheckAll.Size = new System.Drawing.Size(48, 13);
			this.llCheckAll.TabIndex = 7;
			this.llCheckAll.TabStop = true;
			this.llCheckAll.Text = "Check All";
			this.llCheckAll.Visible = false;
			this.llCheckAll.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llCheckAll_LinkClicked);
			// 
			// tvScenarios
			// 
			this.tvScenarios.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tvScenarios.CheckBoxes = true;
			this.tvScenarios.Enabled = false;
			this.tvScenarios.Location = new System.Drawing.Point(12, 12);
			this.tvScenarios.Name = "tvScenarios";
			this.tvScenarios.ShowNodeToolTips = true;
			this.tvScenarios.Size = new System.Drawing.Size(283, 227);
			this.tvScenarios.TabIndex = 8;
			this.tvScenarios.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.tvScenarios_AfterCheck);
			this.tvScenarios.BeforeCheck += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvScenarios_BeforeCheck);
			// 
			// ScenarioPicker
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(307, 340);
			this.Controls.Add(this.tvScenarios);
			this.Controls.Add(this.llCheckAll);
			this.Controls.Add(this.chkPersist);
			this.Controls.Add(this.llUncheck);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.btnReset);
			this.Controls.Add(this.btnOverride);
			this.Name = "ScenarioPicker";
			this.Text = "Scenario Picker";
			this.statusStrip1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnReset;
		private System.Windows.Forms.LinkLabel llUncheck;
		private System.Windows.Forms.ToolStripStatusLabel tssLabel;
		private System.Windows.Forms.Button btnOverride;
		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.CheckBox chkPersist;
		private System.Windows.Forms.LinkLabel llCheckAll;
		private System.Windows.Forms.TreeView tvScenarios;
	}
}