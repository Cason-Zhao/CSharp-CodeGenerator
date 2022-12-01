using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSharp_CodeGenerator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            richTxtResult.Text = Business.ConstClass.ConstClassGenerator.Generate(new Business.ConstClass.ConstClassParamModel
            {
                InputString = "A: 张三,B:李四",
                RegexPattern = Business.ConstClass.CommonRegexPattern.A,
            });
        }
    }
}
