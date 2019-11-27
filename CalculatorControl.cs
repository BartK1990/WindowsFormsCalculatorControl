using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsCalculatorControl
{
    public partial class CalculatorControl: UserControl
    {
        private double resultValue = 0;
        private double lastValue = 0;
        private char lastOperation;
        private int lastTypePressed = 0; // 0 - nothing or C, 1 - number, 2 - sign
        char decimalSeparator = Convert.ToChar(Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator);

        public CalculatorControl()
        {
            InitializeComponent();
        }

        private void Operator_Pressed(object sender, EventArgs e)
        {
            // An operator was pressed; perform the last operation and store the new operator.
            try
            {
                char operation = (sender as Button).Text[0];

                if (operation == 'C')
                {
                    resultValue = 0;
                    lastTypePressed = 0;
                }
                else
                {
                    if (lastTypePressed != 1) 
                    { 
                        if (double.TryParse(Display.Text, out double result))
                        {
                            double currentValue = result;
                            lastValue = resultValue;

                            switch (lastOperation)
                            {
                                case '+':
                                    resultValue += currentValue;
                                    break;
                                case '-':
                                    resultValue -= currentValue;
                                    break;
                                case '×':
                                    resultValue *= currentValue;
                                    break;
                                case '÷':
                                    resultValue /= currentValue;
                                    break;
                                default:
                                    resultValue = currentValue;
                                    break;
                            }
                        }
                    }
                }
                lastOperation = operation;
                Display.Text = resultValue.ToString();
                lastTypePressed = 1;
            }
            catch
            {
                Display.Text = "Error";
            }
        }

        private void Number_Pressed(object sender, EventArgs e)
        {
            // Add it to the display.
            var number = (sender as Button)?.Text;
            if (!(number is string)) return;
            if (number == ".")
                number = decimalSeparator.ToString();
            if (Display.Text.Contains(decimalSeparator.ToString()) && number.Contains(decimalSeparator.ToString())) return;
            if (lastTypePressed == 2)
            {
                Display.Text = Display.Text == "0" ? number : Display.Text + number;
            }
            else
            {
                if (double.TryParse(number, out double result))
                {
                    Display.Text = number;
                }
            }
            lastTypePressed = 2;
        }
    }
}
