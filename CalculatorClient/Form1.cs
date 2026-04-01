using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CalculatorClient.CalculatorService;

namespace CalculatorClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private bool GetNumbers(out double num1, out double num2)
        {
            num1 = 0; num2 = 0;
            if (!double.TryParse(txtNum1.Text, out num1))
            {
                MessageBox.Show("Số 1 không hợp lệ!", "Lỗi");
                return false;
            }
            if (!double.TryParse(txtNum2.Text, out num2))
            {
                MessageBox.Show("Số 2 không hợp lệ!", "Lỗi");
                return false;
            }
            return true;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!GetNumbers(out double num1, out double num2)) return;
            var client = new CalculatorWsClient();
            txtResult.Text = client.add((int)num1, (int)num2).ToString();
        }

        private void btnSubtract_Click(object sender, EventArgs e)
        {
            if (!GetNumbers(out double num1, out double num2)) return;
            var client = new CalculatorWsClient();
            txtResult.Text = client.subtract((int)num1, (int)num2).ToString();
        }

        private void btnMultiply_Click(object sender, EventArgs e)
        {
            if (!GetNumbers(out double num1, out double num2)) return;
            var client = new CalculatorWsClient();
            txtResult.Text = client.multiply(num1, num2).ToString();
        }

        private void btnDivide_Click(object sender, EventArgs e)
        {
            if (!GetNumbers(out double num1, out double num2)) return;
            if (num2 == 0)
            {
                MessageBox.Show("Không thể chia cho 0!", "Lỗi");
                return;
            }
            var client = new CalculatorWsClient();
            txtResult.Text = client.divide(num1, num2).ToString();
        }
    }
}
