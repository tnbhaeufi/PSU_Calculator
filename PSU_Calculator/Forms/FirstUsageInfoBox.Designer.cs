﻿namespace PSU_Calculator
{
  partial class FirstUsageInfoBox
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
 
      this.textBox1 = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      // 
      // textBox1
      // 
      this.textBox1.Location = new System.Drawing.Point(12, 12);
      this.textBox1.Multiline = true;
      this.textBox1.Name = "textBox1";
      this.textBox1.ReadOnly = true;
      this.textBox1.Size = new System.Drawing.Size(374, 104);
      this.textBox1.TabIndex = 0;
      this.textBox1.Text = "Bitte warten während das Programm für die erste verwendung Konfiguriert wird.\nEs " +
    "werden ua. aktuelle Daten vom Server geladen.\nDas Programm startet automatisch w" +
    "enn der Vorgang beendet ist.";
      // 
      // FirstUsageInfoBox
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(398, 129);
      this.Controls.Add(this.textBox1);
      this.Name = "FirstUsageInfoBox";
      this.Text = "Vorbereitung zur benutzung";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox textBox1;

  }
}