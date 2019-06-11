using NCalc;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Data;
using System.Windows.Forms;
using static System.String;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {

        Dictionary<string, int> symbols = new Dictionary<string, int> { { "0", 0 }, { "1", 0 }, { "2", 0 },  { "3", 0 }, { "4", 0 }, { "5", 0 }, {"6", 0}, {"7", 0},
            {"8", 0 }, { "+", 1 }, { "-", 1 }, { "*", 1 }, { "/", 1 }, { "(", 2 }, { ")", 2 }};
        int countBrackets = 0;
        bool isMinus = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void ButtonNumber_Click(object sender, EventArgs e)
        {
            var text = (sender as Button).Text;
            Action add = () => { textBox1.Text += text; isMinus = false; };

           
            if (symbols[text] == 0)
            {
                if (ValidateNum(text)) add();
            }
            else if (symbols[text] == 1)
            {
                if (ValidateSim(text)) add();
            }
            else if (symbols[text] == 2)
            {
                if (ValidateBracket(text)) add();
            }
        }

        private void Button13_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }

        private void Button14_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox1.Text == "") return;
                var regex = new Regex(@"\d*", RegexOptions.IgnoreCase);
                var matches = regex.Matches(textBox1.Text);
                var delta = 0;
                foreach (Match element in matches)
                {
                    if (element.Length != 0)
                    {
                        var decInt = ToDec(element.Value, 9).ToString();
                        textBox1.Text = textBox1.Text.Remove(element.Index - delta, element.Length)
                                .Insert(element.Index - delta, decInt);
                        delta += element.Length - decInt.Length;
                    }
                }
                var val = Convert.ToDouble(new Expression(textBox1.Text).Evaluate());
                var integ = (int)val;
                var fract = (int)((val - integ) * 100000);
                textBox1.Text = ToGexInt(integ);
                textBox1.Text += "." + ToBinFrac(fract, fract.ToString().Length);
            }
            catch
            {
                MessageBox.Show("Произошёл принеприятнийший случай");
            }
        }

        public static string ToBinFrac(double frac, int len)
        {
            var str = "";
            int c;
            var n = 0;
            while (n < len)
            {
                frac *= 12;
                c = Convert.ToInt32(Math.Truncate(frac));
                str = str + c;
                frac -= c;
                n++;
            }
            return str;
        }

        public static string ToGexInt(int dec)
        {
            var str = "";
            while(dec>0)
            {
                str = Concat(Convert.ToString(dec % 9), str);
                dec = dec / 9;
            }
            return (str.Length == 0) ? "0" : str;
        }

        private static long ToDec(string value, int fromBase)
        {
            const string table = "012345678";
            long rank = 1, result = 0;
            for (var i = value.Length - 1; i >= 0; i--)
            {
                var index = table.IndexOf(value[i]);
                result += rank * index;
                rank *= fromBase;
            }
            return result;
        }

        private bool ValidateSim(string text)
        {
            var textBoxText = textBox1.Text;
            return ((textBoxText != "") && (symbols[textBoxText[textBoxText.Length - 1].ToString()] != 1));
        }


        private bool ValidateNum(string text)
        {
            var textBoxText = textBox1.Text;
            var lastIndex = textBoxText.Length - 1;
            return lastIndex <= 0 || symbols[textBoxText[textBoxText.Length - 1].ToString()] != 0 || text != ")";
        }

        private bool ValidateBracket(string text)
        {
            var textBoxText = textBox1.Text;
            var lastIndex = textBoxText.Length - 1;
            countBrackets = (text == "(") ? ++countBrackets : --countBrackets;
            if (countBrackets < 0) { countBrackets++; return false; }
            if (lastIndex >= 0 && symbols[textBoxText[lastIndex].ToString()] == 0 && text == "(") return false;
            if (lastIndex >= 0 && symbols[textBoxText[lastIndex].ToString()] == 1 && (text == ")")) return false;
            return false;
        }
    }   
}

