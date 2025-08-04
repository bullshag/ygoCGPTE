using System.Windows.Forms;

namespace WinFormsApp1
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer? components = null;
        private DataGridView dataGridViewCards;

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
            this.dataGridViewCards = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCards)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewCards
            // 
            this.dataGridViewCards.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewCards.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewCards.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewCards.Name = "dataGridViewCards";
            this.dataGridViewCards.RowTemplate.Height = 25;
            this.dataGridViewCards.Size = new System.Drawing.Size(800, 450);
            this.dataGridViewCards.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.dataGridViewCards);
            this.Name = "Form1";
            this.Text = "Card Browser";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCards)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion
    }
}
