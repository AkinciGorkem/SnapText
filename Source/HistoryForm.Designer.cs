namespace SnapText
{
    partial class HistoryForm
    {
        private System.ComponentModel.IContainer components = null;

        private void InitializeComponent()
        {
            searchTextBox = new Guna.UI2.WinForms.Guna2TextBox();
            searchButton = new Guna.UI2.WinForms.Guna2Button();
            clearSearchButton = new Guna.UI2.WinForms.Guna2Button();
            historyDataGridView = new DataGridView();
            exportButton = new Guna.UI2.WinForms.Guna2Button();
            deleteButton = new Guna.UI2.WinForms.Guna2Button();
            clearAllButton = new Guna.UI2.WinForms.Guna2Button();
            statusLabel = new Label();
            tagsComboBox = new Guna.UI2.WinForms.Guna2ComboBox();
            tagLabel = new Label();
            addTagButton = new Guna.UI2.WinForms.Guna2Button();
            removeTagButton = new Guna.UI2.WinForms.Guna2Button();
            newTagTextBox = new Guna.UI2.WinForms.Guna2TextBox();
            exportFormatComboBox = new Guna.UI2.WinForms.Guna2ComboBox();
            ((System.ComponentModel.ISupportInitialize)historyDataGridView).BeginInit();
            SuspendLayout();
            
            searchTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            searchTextBox.BorderRadius = 8;
            searchTextBox.Cursor = Cursors.IBeam;
            searchTextBox.DefaultText = "";
            searchTextBox.FillColor = Color.FromArgb(37, 37, 38);
            searchTextBox.ForeColor = Color.White;
            searchTextBox.Location = new Point(12, 12);
            searchTextBox.Name = "searchTextBox";
            searchTextBox.PlaceholderForeColor = Color.FromArgb(125, 137, 149);
            searchTextBox.PlaceholderText = "Search history...";
            searchTextBox.SelectedText = "";
            searchTextBox.Size = new Size(400, 36);
            searchTextBox.TabIndex = 0;
            
            searchButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            searchButton.BorderRadius = 8;
            searchButton.FillColor = Color.FromArgb(46, 204, 113);
            searchButton.Font = new Font("Segoe UI", 9F);
            searchButton.ForeColor = Color.White;
            searchButton.Location = new Point(425, 12);
            searchButton.Name = "searchButton";
            searchButton.Size = new Size(80, 36);
            searchButton.TabIndex = 1;
            searchButton.Text = "Search";
            searchButton.UseTransparentBackground = true;
            
            clearSearchButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            clearSearchButton.BorderRadius = 8;
            clearSearchButton.FillColor = Color.FromArgb(52, 73, 94);
            clearSearchButton.Font = new Font("Segoe UI", 9F);
            clearSearchButton.ForeColor = Color.White;
            clearSearchButton.Location = new Point(515, 12);
            clearSearchButton.Name = "clearSearchButton";
            clearSearchButton.Size = new Size(80, 36);
            clearSearchButton.TabIndex = 2;
            clearSearchButton.Text = "Clear";
            clearSearchButton.UseTransparentBackground = true;
            
            tagLabel.AutoSize = true;
            tagLabel.ForeColor = Color.White;
            tagLabel.Location = new Point(12, 60);
            tagLabel.Name = "tagLabel";
            tagLabel.Size = new Size(35, 15);
            tagLabel.TabIndex = 3;
            tagLabel.Text = "Tags:";
            
            tagsComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            tagsComboBox.BackColor = Color.Transparent;
            tagsComboBox.BorderRadius = 8;
            tagsComboBox.DrawMode = DrawMode.OwnerDrawFixed;
            tagsComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            tagsComboBox.FillColor = Color.FromArgb(37, 37, 38);
            tagsComboBox.ForeColor = Color.White;
            tagsComboBox.ItemHeight = 30;
            tagsComboBox.Location = new Point(53, 54);
            tagsComboBox.Name = "tagsComboBox";
            tagsComboBox.Size = new Size(150, 36);
            tagsComboBox.TabIndex = 4;
            
            newTagTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            newTagTextBox.BorderRadius = 8;
            newTagTextBox.Cursor = Cursors.IBeam;
            newTagTextBox.DefaultText = "";
            newTagTextBox.FillColor = Color.FromArgb(37, 37, 38);
            newTagTextBox.ForeColor = Color.White;
            newTagTextBox.Location = new Point(215, 54);
            newTagTextBox.Name = "newTagTextBox";
            newTagTextBox.PlaceholderForeColor = Color.FromArgb(125, 137, 149);
            newTagTextBox.PlaceholderText = "New tag...";
            newTagTextBox.SelectedText = "";
            newTagTextBox.Size = new Size(120, 36);
            newTagTextBox.TabIndex = 5;
            
            addTagButton.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            addTagButton.BorderRadius = 8;
            addTagButton.FillColor = Color.FromArgb(52, 152, 219);
            addTagButton.Font = new Font("Segoe UI", 9F);
            addTagButton.ForeColor = Color.White;
            addTagButton.Location = new Point(345, 54);
            addTagButton.Name = "addTagButton";
            addTagButton.Size = new Size(80, 36);
            addTagButton.TabIndex = 6;
            addTagButton.Text = "Add Tag";
            addTagButton.UseTransparentBackground = true;
            
            removeTagButton.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            removeTagButton.BorderRadius = 8;
            removeTagButton.FillColor = Color.FromArgb(231, 76, 60);
            removeTagButton.Font = new Font("Segoe UI", 9F);
            removeTagButton.ForeColor = Color.White;
            removeTagButton.Location = new Point(435, 54);
            removeTagButton.Name = "removeTagButton";
            removeTagButton.Size = new Size(90, 36);
            removeTagButton.TabIndex = 7;
            removeTagButton.Text = "Remove Tag";
            removeTagButton.UseTransparentBackground = true;
            
            historyDataGridView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            historyDataGridView.AllowUserToAddRows = false;
            historyDataGridView.AllowUserToDeleteRows = false;
            historyDataGridView.BackgroundColor = Color.FromArgb(37, 37, 38);
            historyDataGridView.BorderStyle = BorderStyle.None;
            historyDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            historyDataGridView.Location = new Point(12, 105);
            historyDataGridView.MultiSelect = false;
            historyDataGridView.Name = "historyDataGridView";
            historyDataGridView.ReadOnly = true;
            historyDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            historyDataGridView.Size = new Size(760, 350);
            historyDataGridView.TabIndex = 7;
            
            exportFormatComboBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            exportFormatComboBox.BackColor = Color.Transparent;
            exportFormatComboBox.BorderRadius = 8;
            exportFormatComboBox.DrawMode = DrawMode.OwnerDrawFixed;
            exportFormatComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            exportFormatComboBox.FillColor = Color.FromArgb(37, 37, 38);
            exportFormatComboBox.ForeColor = Color.White;
            exportFormatComboBox.ItemHeight = 30;
            exportFormatComboBox.Location = new Point(12, 470);
            exportFormatComboBox.Name = "exportFormatComboBox";
            exportFormatComboBox.Size = new Size(100, 36);
            exportFormatComboBox.TabIndex = 8;
            
            exportButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            exportButton.BorderRadius = 8;
            exportButton.FillColor = Color.FromArgb(155, 89, 182);
            exportButton.Font = new Font("Segoe UI", 9F);
            exportButton.ForeColor = Color.White;
            exportButton.Location = new Point(125, 470);
            exportButton.Name = "exportButton";
            exportButton.Size = new Size(80, 36);
            exportButton.TabIndex = 9;
            exportButton.Text = "Export";
            exportButton.UseTransparentBackground = true;
            
            deleteButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            deleteButton.BorderRadius = 8;
            deleteButton.FillColor = Color.FromArgb(231, 76, 60);
            deleteButton.Font = new Font("Segoe UI", 9F);
            deleteButton.ForeColor = Color.White;
            deleteButton.Location = new Point(560, 470);
            deleteButton.Name = "deleteButton";
            deleteButton.Size = new Size(100, 36);
            deleteButton.TabIndex = 10;
            deleteButton.Text = "Delete";
            deleteButton.UseTransparentBackground = true;
            
            clearAllButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            clearAllButton.BorderRadius = 8;
            clearAllButton.FillColor = Color.FromArgb(192, 57, 43);
            clearAllButton.Font = new Font("Segoe UI", 9F);
            clearAllButton.ForeColor = Color.White;
            clearAllButton.Location = new Point(672, 470);
            clearAllButton.Name = "clearAllButton";
            clearAllButton.Size = new Size(100, 36);
            clearAllButton.TabIndex = 11;
            clearAllButton.Text = "Clear All";
            clearAllButton.UseTransparentBackground = true;
            
            statusLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            statusLabel.ForeColor = Color.White;
            statusLabel.Location = new Point(220, 478);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(320, 20);
            statusLabel.TabIndex = 12;
            statusLabel.Text = "Ready";
            statusLabel.TextAlign = ContentAlignment.MiddleCenter;
            
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(45, 45, 48);
            ClientSize = new Size(784, 521);
            Controls.Add(statusLabel);
            Controls.Add(clearAllButton);
            Controls.Add(deleteButton);
            Controls.Add(exportButton);
            Controls.Add(historyDataGridView);
            Controls.Add(addTagButton);
            Controls.Add(removeTagButton);
            Controls.Add(newTagTextBox);
            Controls.Add(exportFormatComboBox);
            Controls.Add(tagsComboBox);
            Controls.Add(tagLabel);
            Controls.Add(clearSearchButton);
            Controls.Add(searchButton);
            Controls.Add(searchTextBox);
            MinimumSize = new Size(800, 560);
            Name = "HistoryForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "SnapText - History";
            ((System.ComponentModel.ISupportInitialize)historyDataGridView).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private Guna.UI2.WinForms.Guna2TextBox searchTextBox;
        private Guna.UI2.WinForms.Guna2Button searchButton;
        private Guna.UI2.WinForms.Guna2Button clearSearchButton;
        private DataGridView historyDataGridView;
        private Guna.UI2.WinForms.Guna2Button exportButton;
        private Guna.UI2.WinForms.Guna2Button deleteButton;
        private Guna.UI2.WinForms.Guna2Button clearAllButton;
        private Label statusLabel;
        private Guna.UI2.WinForms.Guna2ComboBox tagsComboBox;
        private Label tagLabel;
        private Guna.UI2.WinForms.Guna2Button addTagButton;
        private Guna.UI2.WinForms.Guna2Button removeTagButton;
        private Guna.UI2.WinForms.Guna2TextBox newTagTextBox;
        private Guna.UI2.WinForms.Guna2ComboBox exportFormatComboBox;
    }
}