namespace SnakeGame.Gui
{
    partial class ShowWindow
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
            this.SuspendLayout();
            // 
            // ShowWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(205, 200);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "ShowWindow";
            this.Text = "ShowWindow";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ShowWindow_FormClosed);
            this.Load += new System.EventHandler(this.ShowWindow_Load);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ShowWindow_KeyPress);
            this.ResumeLayout(false);

        }

        #endregion
    }
}