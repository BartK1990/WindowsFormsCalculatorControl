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
        private double _resultValue = 0;
        private char _lastOperation;
        private int _lastTypePressed = 0; // 0 - nothing or C, 1 - number, 2 - sign
        readonly char _decimalSeparator = Convert.ToChar(Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator);

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
                    _resultValue = 0;
                    _lastTypePressed = 0;
                }
                else
                {
                    if (_lastTypePressed != 1) 
                    { 
                        if (double.TryParse(Display.Text, out double result))
                        {
                            double currentValue = result;
                            switch (_lastOperation)
                            {
                                case '+':
                                    _resultValue += currentValue;
                                    break;
                                case '-':
                                    _resultValue -= currentValue;
                                    break;
                                case '×':
                                    _resultValue *= currentValue;
                                    break;
                                case '÷':
                                    _resultValue /= currentValue;
                                    break;
                                default:
                                    _resultValue = currentValue;
                                    break;
                            }
                        }
                    }
                }
                _lastOperation = operation;
                Display.Text = _resultValue.ToString();
                _lastTypePressed = 1;
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
                number = _decimalSeparator.ToString();
            if (Display.Text.Contains(_decimalSeparator.ToString()) && number.Contains(_decimalSeparator.ToString())) return;
            if (_lastTypePressed == 2)
            {
                Display.Text = Display.Text == @"0" ? number : Display.Text + number;
            }
            else
            {
                if (double.TryParse(number, out double result))
                {
                    Display.Text = number;
                }
            }
            _lastTypePressed = 2;
        }
    }
}
