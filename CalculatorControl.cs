using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
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
        private readonly char _decimalSeparator = Convert.ToChar(Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator);
        private readonly List<Keys> _operatorsList;
        private readonly List<Button> _signButtonsList;
        private readonly Color _buttonBackColor;
        private readonly Color _defaultActiveButtonColor = Color.DeepSkyBlue;

        public CalculatorControl()
        {
            InitializeComponent();
            Display.GotFocus += Display_GotFocus;
                // List of valid operators
            _operatorsList = new List<Keys>()
            {
                Keys.Add,
                Keys.Subtract,
                Keys.Multiply,
                Keys.Divide,
                Keys.Return
            };
            _signButtonsList = new List<Button>()
            {
                addition_button,
                subtraction_button,
                multiplication_button,
                division_button
            };
            _buttonBackColor = addition_button.BackColor;
        }

        private void Operator_Pressed(object sender, EventArgs e)
        {
            // Always focus textBox
            Display.Select();

            // An operator was pressed; perform the last operation and store the new operator.
            try
            {
                char operation = (sender as Button).Text[0];

                if (operation == 'C')
                {
                    _resultValue = 0;
                    _lastTypePressed = 0;
                    _lastOperation = default(char);
                    Display.Text = "";
                }
                else
                {
                    if (_lastTypePressed == 1) return;
                    if (!double.TryParse(Display.Text, out double result)) return;
                    double currentValue = result;
                    bool equalSign = false;
                    switch (operation)
                    {
                        case '+':
                            addition_button.BackColor = _defaultActiveButtonColor;
                            break;
                        case '-':
                            subtraction_button.BackColor = _defaultActiveButtonColor;
                            break;
                        case '×':
                        case '*':
                            multiplication_button.BackColor = _defaultActiveButtonColor;
                            break;
                        case '÷':
                        case '/':
                            division_button.BackColor = _defaultActiveButtonColor;
                            break;
                        default:
                            equalSign = true;
                            _lastTypePressed = 2;
                            break;
                    }
                    switch (_lastOperation)
                    {
                        case '+':
                            _resultValue += currentValue;
                            break;
                        case '-':
                            _resultValue -= currentValue;
                            break;
                        case '×':
                        case '*':
                            _resultValue *= currentValue;
                            break;
                        case '÷':
                        case '/':
                            _resultValue /= currentValue;
                            break;
                        default:
                            _resultValue = currentValue;
                            break;
                    }
                    Display.Text = _resultValue.ToString(CultureInfo.InvariantCulture);
                    _lastOperation = operation;
                    if (equalSign) return;
                    _lastTypePressed = 1;
                }
            }
            catch
            {
                Display.Text = "";
            }
        }

        private void Number_Pressed(object sender, EventArgs e)
        {
            // Always focus textBox
            Display.Select();

            // Clear button colors
            foreach (Button button in _signButtonsList)
            {
                button.UseVisualStyleBackColor = true;
            }

            // Add it to the display.
            var number = (sender as Button)?.Text;
            if (!(number is string)) return;
            if (number == "." || number == ",")
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

        private bool _numberEntered = false;
        private bool _operatorEntered = false;
        private bool _backEntered = false;

        private void Display_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Stop the character from being entered always
            e.Handled = true;

            Button tempButton = new Button();
            tempButton.Text = e.KeyChar.ToString();

            if (_backEntered)
            {
                tempButton.Text = @"C";
                Operator_Pressed(tempButton, e);
            }
            if (_numberEntered)
            {
                Number_Pressed(tempButton, e);
            }
            if (_operatorEntered)
            {
                Operator_Pressed(tempButton, e);
            }
        }

        private void Display_KeyDown(object sender, KeyEventArgs e)
        {
            // Initialize the flag to false.
            _numberEntered = _operatorEntered = _backEntered = false;

            // Determine whether the keystroke is a number from the top of the keyboard.
            if ((e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9) || 
                (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9) || // Determine whether the keystroke is a number from the keypad.
                e.KeyCode == Keys.Decimal) // Determine whether the keystroke is a decimal place separator
            {
                _numberEntered = true;
            }
            if (e.KeyCode == Keys.Back)
            {
                _backEntered = true;
            }
            if (_operatorsList.Contains(e.KeyCode))
            {
                _operatorEntered = true;
            }
            //If shift key was pressed, it's not a number or operator
            if (Control.ModifierKeys == Keys.Shift)
            {
                _numberEntered = false;
                _operatorEntered = false;
            }
        }

        private void CalculatorControl_Click(object sender, EventArgs e)
        {
            Display.Select();
        }

        private void CalculatorLayoutPanel_Click(object sender, EventArgs e)
        {
            Display.Select();
        }

        [DllImport("user32.dll")]
        static extern bool HideCaret(IntPtr hWnd);
        private void Display_GotFocus(object sender, EventArgs e)
        {
            HideCaret(Display.Handle);
        }
    }
}
