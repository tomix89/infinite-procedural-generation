
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
            this.button_new = new System.Windows.Forms.Button();
            this.button_scroll = new System.Windows.Forms.Button();
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
            // button_scroll
            // 
            this.button_scroll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_scroll.Location = new System.Drawing.Point(755, 41);
            this.button_scroll.Name = "button_scroll";
            this.button_scroll.Size = new System.Drawing.Size(118, 23);
            this.button_scroll.TabIndex = 1;
            this.button_scroll.Text = "Scroll >>>";
            this.button_scroll.UseVisualStyleBackColor = true;
            this.button_scroll.Click += new System.EventHandler(this.button_scroll_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(885, 583);
            this.Controls.Add(this.button_scroll);
            this.Controls.Add(this.button_new);
            this.Name = "Form1";
            this.Text = "Infinite Procedural Generator";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_new;
        private System.Windows.Forms.Button button_scroll;
    }
}

