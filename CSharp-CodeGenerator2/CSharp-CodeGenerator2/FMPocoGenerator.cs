/***************************************************************************
 * 文件名：FMPocoGenerator
 * 功能：
 * 说明：
 * -------------------------------------------------------------------------
 * 创建时间：2022/12/15 22:29:29
 * 创建人：赵旋
 * 邮箱： 1192508693@qq.com
 * ========================================================================= 
 * 
 * 修改人：    
 * 修改时间：    
 * 修改说明：    
 ***************************************************************************/
using CodeGenerator;
using CodeGenerator.DBUtil.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSharp_CodeGenerator2
{
    public partial class FMPocoGenerator : Form
    {
        private List<TableInfo> _TableInfos;

        public FMPocoGenerator()
        {
            InitializeComponent();

            LoadIconFont();

            // 防界面闪烁三件套 之一
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }

        #region 图标加载

        private PrivateFontCollection _Pfc;
        private Dictionary<string, FontFamily> _FondDic = new Dictionary<string, FontFamily>();

        private void LoadIconFont()
        {
            var _Pfc = new PrivateFontCollection();

            _Pfc.AddFontFile("./Resources/fontawesome-webfont.ttf");

            foreach (var fontFamily in _Pfc.Families)
            {
                if (!_FondDic.ContainsKey(fontFamily.Name))
                {
                    _FondDic.Add(fontFamily.Name, fontFamily);
                }
            }
            //label1.Text = "\uf00d sss";
            //label1.ForeColor = Color.White;

            labClose.Font = new Font(_FondDic["FontAwesome"], 9);
            labClose.Text = "\uf00d";
            labClose.ForeColor = Color.White;
            labClose.MouseHover += LabelBtn_MouseHover;
            labClose.MouseLeave += LabelBtn_MouseLeave;

            labMax.Font = new Font(_FondDic["FontAwesome"], 9);
            labMax.Text = "\uf24d";
            labMax.ForeColor = Color.White;
            labMax.MouseHover += LabelBtn_MouseHover;
            labMax.MouseLeave += LabelBtn_MouseLeave;

            labMin.Font = new Font(_FondDic["FontAwesome"], 9);
            labMin.Text = "\uf068";
            labMin.ForeColor = Color.White;
            labMin.MouseHover += LabelBtn_MouseHover;
            labMin.MouseLeave += LabelBtn_MouseLeave;


            //ShowMsg(label1);
        }

        private void LabelBtn_MouseLeave(object sender, EventArgs e)
        {
            ((Label)sender).ForeColor = Color.White;
        }

        private void LabelBtn_MouseHover(object sender, EventArgs e)
        {
            ((Label)sender).ForeColor = Color.FromArgb(0x1eaff5);
        }

        #endregion

        #region 实现界面拖动

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_MOVE = 0xF010;
        public const int HTCAPTION = 0x0002;
        /// <summary>
        /// 为了使主界面能够移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FMPocoGenerator_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
        }

        #endregion

        #region 界面闪烁

        // 防界面闪烁三件套 之三
        protected override CreateParams CreateParams  //防止界面闪烁
        {
            get
            {
                CreateParams paras = base.CreateParams;
                paras.ExStyle |= 0x02000000;
                return paras;
            }
        }

        // 防界面闪烁三件套 之二
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            if (m.Msg == 0x0014) // 禁掉清除背景消息

                return;

            base.WndProc(ref m);
        }


        #endregion

        private void FMPocoGenerator_Paint(object sender, PaintEventArgs e)
        {
            PaintBackColor(e.Graphics);//实例化Graphics 对象g
        }

        private void PaintBackColor(Graphics g)
        {
            if(this.ClientRectangle.Width <= 0 || this.ClientRectangle.Height <= 0)
            {
                return;
            }
            //Color FColor = Color.FromArgb(0x38, 0x38, 0x38); //颜色1
            //Color TColor = Color.FromArgb(0x0f, 0x0f, 0x0f);  //颜色2
            //Brush b = new LinearGradientBrush(this.ClientRectangle, FColor, TColor, LinearGradientMode.ForwardDiagonal);  //实例化刷子，第一个参数指示上色区域，第二个和第三个参数分别渐变颜色的开始和结束，第四个参数表示颜色的方向。
            //g.FillRectangle(b, this.ClientRectangle);  //进行上色
        }

        private void FMPocoGenerator_Load(object sender, EventArgs e)
        {

        }

        private void labClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void labMax_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Maximized;
                PaintBackColor(this.CreateGraphics());
            }
            else if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
            }
        }

        private void labMin_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void FMPocoGenerator_SizeChanged(object sender, EventArgs e)
        {
            PaintBackColor(this.CreateGraphics());
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            using (var helper = new DBHelper(txtDataSource.Text))
            {
                try
                {
                    _TableInfos = (from p in helper.GetTables(txtFilter.Text).Select()
                                   group p by p["TableName"] into g
                                   let firstInfo = g.First()
                                   select new TableInfo
                                   {
                                       Name = g.Key.ToString(),
                                       Description = firstInfo["TableDescription"].ToString(),
                                       Type = firstInfo["TableType"].ToString(),
                                       ColumnInfos = g.Select(row => new ColumnInfo(row)).ToList(),
                                   }).ToList();
                }
                catch (Exception x)
                {
                    MessageBox.Show(x.Message, "错误");
                    return;
                }
            }
            bsData.DataSource = _TableInfos;
            bsData.ResetBindings(true);
            if (_TableInfos.Count > 0)
            {
                bsColumnInfos.DataSource = _TableInfos.First().ColumnInfos;
            }
        }

        private void btnSplitCBottom_Click(object sender, EventArgs e)
        {
            if(splitCBottom.SplitterDistance == 25)
            {
                gridTables.Visible = true;
                splitCBottom.SplitterDistance = 462 + 11;
                btnSplitCBottom.Text = "<";
                btnGenerate.Focus();// 使当前按钮失去焦点，否则当前按钮中的文字无法显示
            }
            else
            {
                gridTables.Visible = false;
                splitCBottom.SplitterDistance = 25;
                btnSplitCBottom.Text = ">";
                btnGenerate.Focus();// 使当前按钮失去焦点，否则当前按钮中的文字无法显示
            }
        }

        private void btnToggleColumns_Click(object sender, EventArgs e)
        {
            if (splitContainerDetail.SplitterDistance == 25)
            {
                gridColumns.Visible = true;
                splitContainerDetail.SplitterDistance = 500 + 18;
                btnToggleColumns.Text = "↑";
                btnGenerate.Focus();// 使当前按钮失去焦点，否则当前按钮中的文字无法显示
            }
            else
            {
                gridColumns.Visible = false;
                splitContainerDetail.SplitterDistance = 25;
                btnToggleColumns.Text = "↓";
                btnGenerate.Focus();// 使当前按钮失去焦点，否则当前按钮中的文字无法显示
            }
        }

        private void tabCtrlTarget_DrawItem(object sender, DrawItemEventArgs e)
        {
            SolidBrush back = new SolidBrush(Color.FromArgb(45, 45, 48));
            SolidBrush white = new SolidBrush(Color.FromArgb(122, 193, 255));
            Rectangle rec = tabCtrlTarget.GetTabRect(0);
            e.Graphics.FillRectangle(back, rec);
            Rectangle rec1 = tabCtrlTarget.GetTabRect(1);
            e.Graphics.FillRectangle(back, rec1);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            for (int i = 0; i < tabCtrlTarget.TabPages.Count; i++)
            {
                Rectangle rec2 = tabCtrlTarget.GetTabRect(i);
                e.Graphics.DrawString(tabCtrlTarget.TabPages[i].Text, new Font("微软雅黑", 9), white, rec2, sf);
            }
        }
    }
}
