/***************************************************************************
 * 文件名：FMConditionSave
 * 功能：
 * 说明：
 * -------------------------------------------------------------------------
 * 创建时间：2022/12/24
 * 创建人：刘艳
 * 邮箱： 2523156074@qq.com
 * ========================================================================= 
 * 
 * 修改人：    
 * 修改时间：    
 * 修改说明：    
 ***************************************************************************/
using CodeGenerator.DBUtil.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Data.OleDb;
using CodeGenerator.ExcelUtil;

namespace CSharp_CodeGenerator2
{
    public partial class FMConditionSave : Form
    {
        private PreConditions _PreConditions { get; set; }

        public FMConditionSave()
        {
            InitializeComponent();

            _PreConditions = new PreConditions();
        }

        private void FMConditionSave_Load(object sender, EventArgs e)
        {
            var list = PreConditionUtil.GetMap().ToList();

            InitForm();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cbxConnectStr.Text))
            {
                MessageBox.Show("请填写连接字符串");
                return;
            }

            if (!Regex.IsMatch(cbxConnectStr.Text, @"^Data\s+Source\s*=\s*['""]*\s*\w+[^;'""]+(?=\b)"))
            {
                MessageBox.Show("连接字符串格式不正确，请检查后保存");
                return;
            }

            _PreConditions.ConnectionString = cbxConnectStr.Text;
            try
            {
                PreConditionUtil.AddMap(_PreConditions.ConnectionString);
                PreConditionUtil.SaveData(_PreConditions);

                MessageBox.Show("保存成功！");

                InitForm();
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message, "Error");
                return;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cbxConnectStr.Text))
            {
                MessageBox.Show("请填写要删除的连接字符串");
                return;
            }
            var maps = PreConditionUtil.GetMap();
            if (!maps.ContainsKey(cbxConnectStr.Text))
            {
                MessageBox.Show("要删除的字符串不存在，无需删除");
                return;
            }

            _PreConditions.ConnectionString = cbxConnectStr.Text;
            try
            {
                PreConditionUtil.DeleteMap(_PreConditions.ConnectionString);

                MessageBox.Show("删除成功");

                _PreConditions.ConnectionString = "";
                InitForm();
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message, "Error");
                return;
            }
        }

        private void InitForm()
        {
            var cbxConnectStrSource = new List<KeyValuePair<string, string>>();
            cbxConnectStrSource.Add(new KeyValuePair<string, string>("--请选择--", ""));
            cbxConnectStrSource.AddRange(PreConditionUtil.GetMap().ToList());
            cbxConnectStr.DataSource = cbxConnectStrSource;
            cbxConnectStr.DisplayMember = "Key";
            cbxConnectStr.ValueMember = "Key";

            if (string.IsNullOrEmpty(_PreConditions.ConnectionString))
            {
                if (_PreConditions.PreConditionList != null)
                {
                    _PreConditions.PreConditionList.Clear();
                    bsData.ResetBindings(true);
                }
            }
            else
            {
                cbxConnectStr.SelectedValue = _PreConditions.ConnectionString;
                _PreConditions = PreConditionUtil.LoadData(_PreConditions.ConnectionString);
                bsData.DataSource = _PreConditions.PreConditionList;
                bsData.ResetBindings(true);
            }
        }

        // 排序
        private void dataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            //自动编号，与数据无关
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
               e.RowBounds.Location.Y,
               dataGridView1.RowHeadersWidth - 4,
               e.RowBounds.Height);
            TextRenderer.DrawText(e.Graphics,
                  (e.RowIndex + 1).ToString(),
                   dataGridView1.RowHeadersDefaultCellStyle.Font,
                   rectangle,
                   dataGridView1.RowHeadersDefaultCellStyle.ForeColor,
                   TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        private void cbxConnectStr_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cbxConnectStr.SelectedIndex == 0)
            {
                if (_PreConditions.PreConditionList != null)
                {
                    _PreConditions.PreConditionList.Clear();
                    bsData.ResetBindings(true);
                }
            }
            else
            {
                _PreConditions.ConnectionString = cbxConnectStr.SelectedValue.ToString();
                _PreConditions = PreConditionUtil.LoadData(_PreConditions.ConnectionString);
                bsData.DataSource = _PreConditions.PreConditionList;
                bsData.ResetBindings(true);
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Excel 文件|*.xls;*.xlsx|所有文件|*.*";
                ofd.Title = "打开文件";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    var fileName = ofd.FileName;

                    List<PreCondition> preConditionList = new List<PreCondition>();

                    DataTable dt = ExcelUtil.ExcelToDataTable(fileName, isFirstRowTitle: false);
                    foreach (DataRow dr in dt.Rows)
                    {
                        preConditionList.Add(new PreCondition
                        {
                            FilterText = dr.ItemArray[0].ToString(),
                        });
                    }
                    
                    _PreConditions.PreConditionList = preConditionList;
                    bsData.DataSource = _PreConditions.PreConditionList;
                    bsData.ResetBindings(true);
                }
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message, "Error");
            }
        }
    }
}
