using System;
using System.Windows.Forms;

namespace CalculatorClient
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblNum1 = new System.Windows.Forms.Label();
            this.lblNum2 = new System.Windows.Forms.Label();
            this.lblResult = new System.Windows.Forms.Label();
            this.txtNum1 = new System.Windows.Forms.TextBox();
            this.txtNum2 = new System.Windows.Forms.TextBox();
            this.txtResult = new System.Windows.Forms.TextBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnSubtract = new System.Windows.Forms.Button();
            this.btnMultiply = new System.Windows.Forms.Button();
            this.btnDivide = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // lblNum1
            this.lblNum1.Text = "Số 1:";
            this.lblNum1.Location = new System.Drawing.Point(30, 30);
            this.lblNum1.Size = new System.Drawing.Size(60, 23);

            // txtNum1
            this.txtNum1.Location = new System.Drawing.Point(100, 27);
            this.txtNum1.Size = new System.Drawing.Size(200, 23);
            this.txtNum1.Name = "txtNum1";

            // lblNum2
            this.lblNum2.Text = "Số 2:";
            this.lblNum2.Location = new System.Drawing.Point(30, 70);
            this.lblNum2.Size = new System.Drawing.Size(60, 23);

            // txtNum2
            this.txtNum2.Location = new System.Drawing.Point(100, 67);
            this.txtNum2.Size = new System.Drawing.Size(200, 23);
            this.txtNum2.Name = "txtNum2";

            // lblResult
            this.lblResult.Text = "Kết quả:";
            this.lblResult.Location = new System.Drawing.Point(30, 110);
            this.lblResult.Size = new System.Drawing.Size(60, 23);

            // txtResult
            this.txtResult.Location = new System.Drawing.Point(100, 107);
            this.txtResult.Size = new System.Drawing.Size(200, 23);
            this.txtResult.Name = "txtResult";
            this.txtResult.ReadOnly = true;
            this.txtResult.BackColor = System.Drawing.Color.LightYellow;

            // btnAdd
            this.btnAdd.Text = "+";
            this.btnAdd.Location = new System.Drawing.Point(30, 155);
            this.btnAdd.Size = new System.Drawing.Size(60, 40);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);

            // btnSubtract
            this.btnSubtract.Text = "-";
            this.btnSubtract.Location = new System.Drawing.Point(110, 155);
            this.btnSubtract.Size = new System.Drawing.Size(60, 40);
            this.btnSubtract.Name = "btnSubtract";
            this.btnSubtract.Click += new System.EventHandler(this.btnSubtract_Click);

            // btnMultiply
            this.btnMultiply.Text = "×";
            this.btnMultiply.Location = new System.Drawing.Point(190, 155);
            this.btnMultiply.Size = new System.Drawing.Size(60, 40);
            this.btnMultiply.Name = "btnMultiply";
            this.btnMultiply.Click += new System.EventHandler(this.btnMultiply_Click);

            // btnDivide
            this.btnDivide.Text = "÷";
            this.btnDivide.Location = new System.Drawing.Point(270, 155);
            this.btnDivide.Size = new System.Drawing.Size(60, 40);
            this.btnDivide.Name = "btnDivide";
            this.btnDivide.Click += new System.EventHandler(this.btnDivide_Click);

            // Form1
            this.ClientSize = new System.Drawing.Size(370, 230);
            this.Text = "Calculator SOAP Client";
            this.Name = "Form1";
            this.Controls.Add(this.lblNum1);
            this.Controls.Add(this.txtNum1);
            this.Controls.Add(this.lblNum2);
            this.Controls.Add(this.txtNum2);
            this.Controls.Add(this.lblResult);
            this.Controls.Add(this.txtResult);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnSubtract);
            this.Controls.Add(this.btnMultiply);
            this.Controls.Add(this.btnDivide);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblNum1;
        private System.Windows.Forms.Label lblNum2;
        private System.Windows.Forms.Label lblResult;
        private System.Windows.Forms.TextBox txtNum1;
        private System.Windows.Forms.TextBox txtNum2;
        private System.Windows.Forms.TextBox txtResult;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnSubtract;
        private System.Windows.Forms.Button btnMultiply;
        private System.Windows.Forms.Button btnDivide;
    }
}