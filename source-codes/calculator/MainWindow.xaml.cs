using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CalculatorApp
{
    public partial class MainWindow : Window
    {
        private double _lastNumber = 0;
        private string _operator = "";
        private bool _isNewEntry = true;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Focus();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9)
            {
                HandleNumber((e.Key - Key.D0).ToString());
                e.Handled = true;
            }
            else if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            {
                HandleNumber((e.Key - Key.NumPad0).ToString());
                e.Handled = true;
            }
            else if (e.Key == Key.Add)
            {
                HandleOperator("+");
                e.Handled = true;
            }
            else if (e.Key == Key.Subtract)
            {
                HandleOperator("−");
                e.Handled = true;
            }
            else if (e.Key == Key.Multiply)
            {
                HandleOperator("×");
                e.Handled = true;
            }
            else if (e.Key == Key.Divide)
            {
                HandleOperator("÷");
                e.Handled = true;
            }
            else if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                Result_Click(this, new RoutedEventArgs());
                e.Handled = true;
            }
            else if (e.Key == Key.Delete)
            {
                Clear_Click(this, new RoutedEventArgs());
                e.Handled = true;
            }
            else if (e.Key == Key.OemComma || e.Key == Key.Decimal)
            {
                Decimal_Click(this, new RoutedEventArgs());
                e.Handled = true;
            }
        }

        private void HandleNumber(string num)
        {
            if (num == "0" && (Display.Text == "0" || Display.Text == ""))
                return;
            if (Display.Text == "0" || _isNewEntry)
                Display.Text = "";
            _isNewEntry = false;
            Display.Text += num;
        }

        private void HandleOperator(string op)
        {
            string displayOp = op switch
            {
                "/" => "÷",
                "*" => "×",
                "-" => "−",
                _ => op
            };

            if (Display.Text.Contains("÷") || Display.Text.Contains("×") || 
                Display.Text.Contains("−") || Display.Text.Contains("+"))
            {
                return;
            }

            if (double.TryParse(Display.Text, out double num))
            {
                _lastNumber = num;
            }
            _operator = op switch
            {
                "÷" => "/",
                "×" => "*",
                "−" => "-",
                _ => op
            };
            Display.Text += displayOp;
            _isNewEntry = true;
        }

        private void Number_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            string num = button.Content.ToString()!;
            if (num == "0" && (Display.Text == "0" || Display.Text == ""))
                return;
            if (Display.Text == "0" || _isNewEntry)
                Display.Text = "";

            _isNewEntry = false;
            Display.Text += num;
        }

        private void Operator_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            
            if (Display.Text.Contains("÷") || Display.Text.Contains("×") || 
                Display.Text.Contains("−") || Display.Text.Contains("+"))
            {
                return;
            }

            if (double.TryParse(Display.Text, out double num))
            {
                _lastNumber = num;
            }
            
            string op = button.Content.ToString()!;
            string displayOp = op;
            _operator = op switch
            {
                "÷" => "/",
                "×" => "*",
                "−" => "-",
                _ => op
            };
            
            Display.Text += displayOp;
            _isNewEntry = true;
        }

        private void Decimal_Click(object sender, RoutedEventArgs e)
        {
            if (!Display.Text.Contains(","))
            {
                Display.Text += ",";
                _isNewEntry = false;
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            Display.Text = "0";
            _lastNumber = 0;
            _operator = "";
            _isNewEntry = true;
        }

        private void Result_Click(object sender, RoutedEventArgs e)
        {
            string expression = Display.Text;
            string displayOp = _operator switch
            {
                "/" => "÷",
                "*" => "×",
                "-" => "−",
                _ => _operator
            };

            if (string.IsNullOrEmpty(_operator) || !expression.Contains(displayOp))
            {
                return;
            }

            string[] parts = expression.Split(new string[] { displayOp }, StringSplitOptions.None);
            if (parts.Length != 2 || !double.TryParse(parts[0], out double num1) || !double.TryParse(parts[1], out double num2))
            {
                Clear_Click(sender, e);
                return;
            }

            double result = 0;
            switch (_operator)
            {
                case "+": result = num1 + num2; break;
                case "-": result = num1 - num2; break;
                case "*": result = num1 * num2; break;
                case "/": 
                    if (num2 != 0) result = num1 / num2;
                    else 
                    {
                        MessageBox.Show("Nulou nelze dělit!");
                        Clear_Click(sender, e);
                        return;
                    }
                    break;
            }

            Display.Text = result.ToString("G10");
            _isNewEntry = true;
        }
    }
}