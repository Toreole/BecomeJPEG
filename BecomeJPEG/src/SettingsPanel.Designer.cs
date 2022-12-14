namespace BecomeJPEG
{
    partial class SettingsPanel
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
            this.StartStopButton = new System.Windows.Forms.Button();
            this.TemplateNameInput = new System.Windows.Forms.TextBox();
            this.QualityInput = new System.Windows.Forms.TextBox();
            this.NameLabel = new System.Windows.Forms.Label();
            this.QualityLabel = new System.Windows.Forms.Label();
            this.DroprateInput = new System.Windows.Forms.TextBox();
            this.DroprateLabel = new System.Windows.Forms.Label();
            this.LagtimeLabel = new System.Windows.Forms.Label();
            this.LagtimeInput = new System.Windows.Forms.TextBox();
            this.LagrandomLabel = new System.Windows.Forms.Label();
            this.LagrandomInput = new System.Windows.Forms.TextBox();
            this.TemplateList = new System.Windows.Forms.ListBox();
            this.TemplatePanel = new System.Windows.Forms.Panel();
            this.TemplateAddButton = new System.Windows.Forms.Button();
            this.TemplateDelete = new System.Windows.Forms.Button();
            this.TemplateLabel = new System.Windows.Forms.Label();
            this.LogText = new System.Windows.Forms.TextBox();
            this.DeviceSelection = new System.Windows.Forms.ComboBox();
            this.DeviceSelectionLabel = new System.Windows.Forms.Label();
            this.ResolutionSelection = new System.Windows.Forms.ComboBox();
            this.ResolutionLabel = new System.Windows.Forms.Label();
            this.TemplatePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // StartStopButton
            // 
            this.StartStopButton.Enabled = false;
            this.StartStopButton.Location = new System.Drawing.Point(354, 221);
            this.StartStopButton.Name = "StartStopButton";
            this.StartStopButton.Size = new System.Drawing.Size(103, 25);
            this.StartStopButton.TabIndex = 0;
            this.StartStopButton.Text = "Start";
            this.StartStopButton.UseVisualStyleBackColor = true;
            this.StartStopButton.Click += new System.EventHandler(this.StartStopButton_Click);
            // 
            // TemplateNameInput
            // 
            this.TemplateNameInput.Location = new System.Drawing.Point(12, 12);
            this.TemplateNameInput.MaxLength = 20;
            this.TemplateNameInput.Name = "TemplateNameInput";
            this.TemplateNameInput.Size = new System.Drawing.Size(85, 20);
            this.TemplateNameInput.TabIndex = 2;
            this.TemplateNameInput.Text = "Default";
            this.TemplateNameInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TemplateName_KeyDown);
            this.TemplateNameInput.Leave += new System.EventHandler(this.TemplateName_StopEdit);
            // 
            // QualityInput
            // 
            this.QualityInput.Location = new System.Drawing.Point(12, 38);
            this.QualityInput.MaxLength = 3;
            this.QualityInput.Name = "QualityInput";
            this.QualityInput.Size = new System.Drawing.Size(85, 20);
            this.QualityInput.TabIndex = 3;
            this.QualityInput.Text = "100";
            this.QualityInput.TextChanged += new System.EventHandler(this.QualityInput_TextChanged);
            this.QualityInput.Leave += new System.EventHandler(this.QualityInput_StopEdit);
            // 
            // NameLabel
            // 
            this.NameLabel.AutoSize = true;
            this.NameLabel.Location = new System.Drawing.Point(103, 15);
            this.NameLabel.Name = "NameLabel";
            this.NameLabel.Size = new System.Drawing.Size(82, 13);
            this.NameLabel.TabIndex = 5;
            this.NameLabel.Text = "Template Name";
            // 
            // QualityLabel
            // 
            this.QualityLabel.AutoSize = true;
            this.QualityLabel.Location = new System.Drawing.Point(103, 45);
            this.QualityLabel.Name = "QualityLabel";
            this.QualityLabel.Size = new System.Drawing.Size(39, 13);
            this.QualityLabel.TabIndex = 6;
            this.QualityLabel.Text = "Quality";
            // 
            // DroprateInput
            // 
            this.DroprateInput.Location = new System.Drawing.Point(12, 64);
            this.DroprateInput.MaxLength = 3;
            this.DroprateInput.Name = "DroprateInput";
            this.DroprateInput.Size = new System.Drawing.Size(85, 20);
            this.DroprateInput.TabIndex = 4;
            this.DroprateInput.Text = "0";
            this.DroprateInput.TextChanged += new System.EventHandler(this.DroprateInput_TextChanged);
            this.DroprateInput.Leave += new System.EventHandler(this.DroprateInput_StopEdit);
            // 
            // DroprateLabel
            // 
            this.DroprateLabel.AutoSize = true;
            this.DroprateLabel.Location = new System.Drawing.Point(103, 71);
            this.DroprateLabel.Name = "DroprateLabel";
            this.DroprateLabel.Size = new System.Drawing.Size(65, 13);
            this.DroprateLabel.TabIndex = 7;
            this.DroprateLabel.Text = "Droprate (%)";
            // 
            // LagtimeLabel
            // 
            this.LagtimeLabel.AutoSize = true;
            this.LagtimeLabel.Location = new System.Drawing.Point(103, 97);
            this.LagtimeLabel.Name = "LagtimeLabel";
            this.LagtimeLabel.Size = new System.Drawing.Size(66, 13);
            this.LagtimeLabel.TabIndex = 9;
            this.LagtimeLabel.Text = "Lagtime (ms)";
            // 
            // LagtimeInput
            // 
            this.LagtimeInput.Location = new System.Drawing.Point(12, 90);
            this.LagtimeInput.MaxLength = 4;
            this.LagtimeInput.Name = "LagtimeInput";
            this.LagtimeInput.Size = new System.Drawing.Size(85, 20);
            this.LagtimeInput.TabIndex = 8;
            this.LagtimeInput.Text = "0";
            this.LagtimeInput.TextChanged += new System.EventHandler(this.LagtimeInput_TextChanged);
            this.LagtimeInput.Leave += new System.EventHandler(this.LagtimeInput_StopEdit);
            // 
            // LagrandomLabel
            // 
            this.LagrandomLabel.AutoSize = true;
            this.LagrandomLabel.Location = new System.Drawing.Point(103, 123);
            this.LagrandomLabel.Name = "LagrandomLabel";
            this.LagrandomLabel.Size = new System.Drawing.Size(82, 13);
            this.LagrandomLabel.TabIndex = 11;
            this.LagrandomLabel.Text = "Lagrandom (ms)";
            // 
            // LagrandomInput
            // 
            this.LagrandomInput.Location = new System.Drawing.Point(12, 116);
            this.LagrandomInput.MaxLength = 4;
            this.LagrandomInput.Name = "LagrandomInput";
            this.LagrandomInput.Size = new System.Drawing.Size(85, 20);
            this.LagrandomInput.TabIndex = 10;
            this.LagrandomInput.Text = "0";
            this.LagrandomInput.TextChanged += new System.EventHandler(this.LagrandomInput_TextChanged);
            this.LagrandomInput.Leave += new System.EventHandler(this.LagrandomInput_StopEdit);
            // 
            // TemplateList
            // 
            this.TemplateList.FormattingEnabled = true;
            this.TemplateList.Location = new System.Drawing.Point(0, 24);
            this.TemplateList.Name = "TemplateList";
            this.TemplateList.Size = new System.Drawing.Size(197, 134);
            this.TemplateList.TabIndex = 12;
            this.TemplateList.SelectedIndexChanged += new System.EventHandler(this.TemplateList_SelectedIndexChanged);
            this.TemplateList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.TemplateList_DoubleClick);
            // 
            // TemplatePanel
            // 
            this.TemplatePanel.Controls.Add(this.TemplateAddButton);
            this.TemplatePanel.Controls.Add(this.TemplateDelete);
            this.TemplatePanel.Controls.Add(this.TemplateLabel);
            this.TemplatePanel.Controls.Add(this.TemplateList);
            this.TemplatePanel.Location = new System.Drawing.Point(264, 3);
            this.TemplatePanel.Name = "TemplatePanel";
            this.TemplatePanel.Size = new System.Drawing.Size(203, 202);
            this.TemplatePanel.TabIndex = 13;
            // 
            // TemplateAddButton
            // 
            this.TemplateAddButton.BackColor = System.Drawing.Color.ForestGreen;
            this.TemplateAddButton.Location = new System.Drawing.Point(99, 158);
            this.TemplateAddButton.Name = "TemplateAddButton";
            this.TemplateAddButton.Size = new System.Drawing.Size(98, 41);
            this.TemplateAddButton.TabIndex = 17;
            this.TemplateAddButton.Text = "Save Active Template";
            this.TemplateAddButton.UseVisualStyleBackColor = false;
            this.TemplateAddButton.Click += new System.EventHandler(this.AddTemplateButton_Click);
            // 
            // TemplateDelete
            // 
            this.TemplateDelete.BackColor = System.Drawing.Color.Red;
            this.TemplateDelete.Location = new System.Drawing.Point(0, 158);
            this.TemplateDelete.Name = "TemplateDelete";
            this.TemplateDelete.Size = new System.Drawing.Size(102, 41);
            this.TemplateDelete.TabIndex = 16;
            this.TemplateDelete.Text = "Delete Selected";
            this.TemplateDelete.UseVisualStyleBackColor = false;
            this.TemplateDelete.Click += new System.EventHandler(this.TemplateDelete_Click);
            // 
            // TemplateLabel
            // 
            this.TemplateLabel.AutoSize = true;
            this.TemplateLabel.Location = new System.Drawing.Point(4, 5);
            this.TemplateLabel.Name = "TemplateLabel";
            this.TemplateLabel.Size = new System.Drawing.Size(102, 13);
            this.TemplateLabel.TabIndex = 13;
            this.TemplateLabel.Text = "Available Templates";
            // 
            // LogText
            // 
            this.LogText.BackColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.LogText.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LogText.ForeColor = System.Drawing.SystemColors.Window;
            this.LogText.Location = new System.Drawing.Point(12, 252);
            this.LogText.MaxLength = 800;
            this.LogText.MinimumSize = new System.Drawing.Size(420, 70);
            this.LogText.Multiline = true;
            this.LogText.Name = "LogText";
            this.LogText.ReadOnly = true;
            this.LogText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.LogText.Size = new System.Drawing.Size(445, 82);
            this.LogText.TabIndex = 14;
            // 
            // DeviceSelection
            // 
            this.DeviceSelection.FormattingEnabled = true;
            this.DeviceSelection.Location = new System.Drawing.Point(12, 224);
            this.DeviceSelection.Name = "DeviceSelection";
            this.DeviceSelection.Size = new System.Drawing.Size(209, 21);
            this.DeviceSelection.TabIndex = 16;
            this.DeviceSelection.Text = "- Select Device -";
            this.DeviceSelection.SelectedIndexChanged += new System.EventHandler(this.DeviceSelection_IndexChanged);
            // 
            // DeviceSelectionLabel
            // 
            this.DeviceSelectionLabel.AutoSize = true;
            this.DeviceSelectionLabel.Location = new System.Drawing.Point(8, 203);
            this.DeviceSelectionLabel.Name = "DeviceSelectionLabel";
            this.DeviceSelectionLabel.Size = new System.Drawing.Size(78, 13);
            this.DeviceSelectionLabel.TabIndex = 17;
            this.DeviceSelectionLabel.Text = "Target Device:";
            // 
            // ResolutionSelection
            // 
            this.ResolutionSelection.Enabled = false;
            this.ResolutionSelection.FormattingEnabled = true;
            this.ResolutionSelection.Location = new System.Drawing.Point(227, 224);
            this.ResolutionSelection.Name = "ResolutionSelection";
            this.ResolutionSelection.Size = new System.Drawing.Size(121, 21);
            this.ResolutionSelection.TabIndex = 18;
            this.ResolutionSelection.SelectedIndexChanged += new System.EventHandler(this.ResolutionSelectionChanged);
            // 
            // ResolutionLabel
            // 
            this.ResolutionLabel.AutoSize = true;
            this.ResolutionLabel.Location = new System.Drawing.Point(231, 205);
            this.ResolutionLabel.Name = "ResolutionLabel";
            this.ResolutionLabel.Size = new System.Drawing.Size(60, 13);
            this.ResolutionLabel.TabIndex = 19;
            this.ResolutionLabel.Text = "Resolution:";
            // 
            // SettingsPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(469, 346);
            this.Controls.Add(this.ResolutionLabel);
            this.Controls.Add(this.ResolutionSelection);
            this.Controls.Add(this.DeviceSelectionLabel);
            this.Controls.Add(this.DeviceSelection);
            this.Controls.Add(this.LogText);
            this.Controls.Add(this.TemplatePanel);
            this.Controls.Add(this.LagrandomLabel);
            this.Controls.Add(this.LagrandomInput);
            this.Controls.Add(this.LagtimeLabel);
            this.Controls.Add(this.LagtimeInput);
            this.Controls.Add(this.DroprateLabel);
            this.Controls.Add(this.QualityLabel);
            this.Controls.Add(this.NameLabel);
            this.Controls.Add(this.DroprateInput);
            this.Controls.Add(this.QualityInput);
            this.Controls.Add(this.TemplateNameInput);
            this.Controls.Add(this.StartStopButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "SettingsPanel";
            this.Text = "BecomeJPEG Settings Panel";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SettingsPanel_Closing);
            this.Load += new System.EventHandler(this.SettingsPanel_Load);
            this.TemplatePanel.ResumeLayout(false);
            this.TemplatePanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button StartStopButton;
        private System.Windows.Forms.TextBox TemplateNameInput;
        private System.Windows.Forms.TextBox QualityInput;
        private System.Windows.Forms.Label NameLabel;
        private System.Windows.Forms.Label QualityLabel;
        private System.Windows.Forms.TextBox DroprateInput;
        private System.Windows.Forms.Label DroprateLabel;
        private System.Windows.Forms.Label LagtimeLabel;
        private System.Windows.Forms.TextBox LagtimeInput;
        private System.Windows.Forms.Label LagrandomLabel;
        private System.Windows.Forms.TextBox LagrandomInput;
        private System.Windows.Forms.ListBox TemplateList;
        private System.Windows.Forms.Panel TemplatePanel;
        private System.Windows.Forms.Button TemplateAddButton;
        private System.Windows.Forms.Button TemplateDelete;
        private System.Windows.Forms.Label TemplateLabel;
        private System.Windows.Forms.TextBox LogText;
        private System.Windows.Forms.ComboBox DeviceSelection;
        private System.Windows.Forms.Label DeviceSelectionLabel;
        private System.Windows.Forms.ComboBox ResolutionSelection;
        private System.Windows.Forms.Label ResolutionLabel;
    }
}