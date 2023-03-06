
namespace TileTest {
    partial class Form1 {
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
            this.components = new System.ComponentModel.Container();
            this.button_new = new System.Windows.Forms.Button();
            this.button_scroll_tile = new System.Windows.Forms.Button();
            this.button_scroll_col = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.cBScrollSpeed = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button_new
            // 
            this.button_new.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_new.Location = new System.Drawing.Point(755, 12);
            this.button_new.Name = "button_new";
            this.button_new.Size = new System.Drawing.Size(118, 23);
            this.button_new.TabIndex = 0;
            this.button_new.Text = "New Image";
            this.button_new.UseVisualStyleBackColor = true;
            this.button_new.Click += new System.EventHandler(this.button_new_Click);
            // 
            // button_scroll_tile
            // 
            this.button_scroll_tile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_scroll_tile.Location = new System.Drawing.Point(755, 41);
            this.button_scroll_tile.Name = "button_scroll_tile";
            this.button_scroll_tile.Size = new System.Drawing.Size(118, 23);
            this.button_scroll_tile.TabIndex = 1;
            this.button_scroll_tile.Text = "Scroll >>>";
            this.button_scroll_tile.UseVisualStyleBackColor = true;
            this.button_scroll_tile.Click += new System.EventHandler(this.button_scroll_tile_Click);
            // 
            // button_scroll_col
            // 
            this.button_scroll_col.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_scroll_col.Location = new System.Drawing.Point(755, 70);
            this.button_scroll_col.Name = "button_scroll_col";
            this.button_scroll_col.Size = new System.Drawing.Size(118, 23);
            this.button_scroll_col.TabIndex = 2;
            this.button_scroll_col.Text = "Scroll >";
            this.button_scroll_col.UseVisualStyleBackColor = true;
            this.button_scroll_col.Click += new System.EventHandler(this.button_scroll_col_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // cBScrollSpeed
            // 
            this.cBScrollSpeed.FormattingEnabled = true;
            this.cBScrollSpeed.Location = new System.Drawing.Point(755, 126);
            this.cBScrollSpeed.Name = "cBScrollSpeed";
            this.cBScrollSpeed.Size = new System.Drawing.Size(118, 21);
            this.cBScrollSpeed.TabIndex = 3;
            this.cBScrollSpeed.SelectedIndexChanged += new System.EventHandler(this.cBScrollSpeed_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(755, 107);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Auto scroll:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(885, 583);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cBScrollSpeed);
            this.Controls.Add(this.button_scroll_col);
            this.Controls.Add(this.button_scroll_tile);
            this.Controls.Add(this.button_new);
            this.Name = "Form1";
            this.Text = "Infinite Procedural Generator";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_new;
        private System.Windows.Forms.Button button_scroll_tile;
        private System.Windows.Forms.Button button_scroll_col;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ComboBox cBScrollSpeed;
        private System.Windows.Forms.Label label1;
    }
}

