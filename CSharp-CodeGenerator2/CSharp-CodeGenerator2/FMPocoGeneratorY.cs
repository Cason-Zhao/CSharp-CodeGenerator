﻿using CodeGenerator;
using CodeGenerator.DBUtil.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSharp_CodeGenerator2
{
    public partial class FMPocoGeneratorY : Form
    {
        private List<TableInfo> _TableInfos;
        private bool _IsConnecting
        {
            get { return bool.Parse(btnConnect.Tag == null || string.IsNullOrWhiteSpace(btnConnect.Tag.ToString()) ? "false" : btnConnect.Tag.ToString()); }
        }

        private PreConditions _PreConditions;
        public FMPocoGeneratorY()
        {
            InitializeComponent();

            txtDataSource.Text = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
            this.Text += $"({Assembly.GetEntryAssembly().GetName().Version.ToString()})";
        }

        private void FMPocoGeneratorY_Load(object sender, EventArgs e)
        {
            if(!string.IsNullOrWhiteSpace(this.txtDataSource.Text))
            {
                //_PreConditions = 
                //btnPreCondition.Text = $"预设条件({count})";
            }
        }

        private void btnConnToggle_Click(object sender, EventArgs e)
        {
            if (this.splitter1.Location.X == 200)
            {
                this.splitter1.Location = new Point(11, 66);
            }
            else
            {
                this.splitter1.Location = new Point(200, 66);
            }
        }

        private void gridTables_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }
            var bindItem = gridTables.Rows[e.RowIndex].DataBoundItem as TableInfo;
            if (bindItem != null)
            {
                bsColumnInfos.DataSource = bindItem.ColumnInfos;
                bsColumnInfos.ResetBindings(true);
            }
        }

        private void gridColumns_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }
            var bindItem = gridColumns.Rows[e.RowIndex].DataBoundItem as ColumnInfo;
            if (bindItem != null)
            {
                bindItem.IsSelected = !bindItem.IsSelected;
            }
            bsColumnInfosSelected.DataSource = _TableInfos.SelectMany(p => p.ColumnInfos.Where(y => y.IsSelected).ToList()).ToList();
            bsColumnInfosSelected.ResetBindings(true);

            gridColumns.Refresh();
        }
        private void gridColumns_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            if (e.ColumnIndex == 0)
            {
                var bindItem = gridColumns.Rows[e.RowIndex].DataBoundItem as ColumnInfo;
                if (bindItem != null)
                {
                    bindItem.IsSelected = !bindItem.IsSelected;
                }
                bsColumnInfosSelected.DataSource = _TableInfos.SelectMany(p => p.ColumnInfos.Where(y => y.IsSelected).ToList()).ToList();
                bsColumnInfosSelected.ResetBindings(true);
                gridColumns.Refresh();
            }
        }

        private void gridColumnSelected_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            var bindItem = gridColumnSelected.Rows[e.RowIndex].DataBoundItem as ColumnInfo;
            if (bindItem != null)
            {
                bindItem.IsSelected = !bindItem.IsSelected;
            }
            bsColumnInfosSelected.DataSource = _TableInfos.SelectMany(p => p.ColumnInfos.Where(y => y.IsSelected).ToList()).ToList();
            bsColumnInfosSelected.ResetBindings(true);
            gridColumns.Refresh();
        }

        private void btnClearSelected_Click(object sender, EventArgs e)
        {
            _TableInfos.SelectMany(p => p.ColumnInfos.Where(y => y.IsSelected).ToList())
                .ToList()
                .ForEach(data => data.IsSelected = false);

            bsColumnInfos.ResetBindings(true);
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            GenerateCode();
            if (!splitContainer_Y_Bottom.Panel1Collapsed)
            {
                splitContainer_Y_Bottom.Panel1Collapsed = !splitContainer_Y_Bottom.Panel1Collapsed;
                btnSelectedToggle.Text = "↓";
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            SearchData();
        }

        private void SearchData()
        {
            if (string.IsNullOrEmpty(this.txtDataSource.Text))
            {
                MessageBox.Show("请先填入连接字符串");
                return;
            }
            if (!_IsConnecting)
            {
                MessageBox.Show("请先连接！", "提示");
                return;
            }

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

                    if (_PreConditions != null && _PreConditions.PreConditionList != null && _PreConditions.PreConditionList.Count > 0)
                    {
                        _TableInfos = _TableInfos
                            .Where(p => _PreConditions.PreConditionList.Any(y => p.Name.Contains(y.FilterText) || p.Description.Contains(y.FilterText)))
                            .ToList();
                    }
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

                bsColumnInfosSelected.DataSource = _TableInfos.SelectMany(p => p.ColumnInfos.Where(y => y.IsSelected).ToList()).ToList();
                bsColumnInfosSelected.ResetBindings(true);
            }
        }

        #region Toggle

        private void btnTablesToggle_Click(object sender, EventArgs e)
        {
            splitC_Table_Cols.Panel1Collapsed = !splitC_Table_Cols.Panel1Collapsed;
            btnTablesToggle.Text = !splitC_Table_Cols.Panel1Collapsed ? "←" : "→";
        }

        private void btnTables_ColumnsToggle_Click(object sender, EventArgs e)
        {
            splitContainer_Top_Y_Bottom.Panel1Collapsed = !splitContainer_Top_Y_Bottom.Panel1Collapsed;
            btnTables_ColumnsToggle.Text = !splitContainer_Top_Y_Bottom.Panel1Collapsed ? "←" : "→";
        }

        private void btnSelectedToggle_Click(object sender, EventArgs e)
        {
            splitContainer_Y_Bottom.Panel1Collapsed = !splitContainer_Y_Bottom.Panel1Collapsed;
            btnSelectedToggle.Text = !splitContainer_Y_Bottom.Panel1Collapsed ? "↑" : "↓";
        }
        #endregion

        private Dictionary<string, string> GeneratedCodes = new Dictionary<string, string>();
        private void GenerateCode()
        {
            if (_TableInfos == null || _TableInfos.Count == 0)
            {
                MessageBox.Show("当前未找到任何表信息！");
                return;
            }
            var selectedList = _TableInfos.SelectMany(p => p.ColumnInfos.Where(y => y.IsSelected).ToList()).ToList();
            if(selectedList.Count == 0)
            {
                MessageBox.Show("请先选择列信息！");
                return;
            }
            GeneratedCodes.Clear();

            #region Code

            PocoType.GetAllKeys()
                .ToList()
                .ForEach(pocoType => GeneratedCodes.Add(pocoType, PocoGenerator.Generate(selectedList, pocoType)));

            #endregion

            #region ListView
            listVTargetMenu.BeginUpdate();

            listVTargetMenu.Clear();
            //listVTargetMenu.Columns.Add("代码", 500, HorizontalAlignment.Left); //一步添加

            ImageList imgList = new ImageList();
            imgList.ImageSize = new Size(1, 30);// 设置行高 20 //分别是宽和高  
            listVTargetMenu.SmallImageList = imgList; //这里设置listView的SmallImageList ,用imgList将其撑大  

            listVTargetMenu.Items.AddRange(PocoType.GetAllList().Select(p => new ListViewItem(p.Value)
            {
                Name = $"A{p.Key}",
                Tag = p.Key,
            }).ToArray());

            listVTargetMenu.EndUpdate();

            #endregion

            #region Panel

            richTxtTarget.Text = GeneratedCodes.First().Value;

            #endregion
        }

        private void listVTargetMenu_Click(object sender, EventArgs e)
        {
            if(GeneratedCodes.Count > 0)
            {
                var selectedItem = listVTargetMenu.Items.OfType<ListViewItem>().Where(p => p.Focused).FirstOrDefault();
                if(selectedItem != null)
                {
                    var pocoType = selectedItem.Tag.ToString();
                    richTxtTarget.Text = !GeneratedCodes.ContainsKey(pocoType) ? "" : GeneratedCodes[pocoType];
                }
            }
        }

        // 预设条件
        private void btnPreCondition_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(this.txtDataSource.Text))
            {
                MessageBox.Show("请先填入连接字符串");
                return;
            }
            if(!_IsConnecting)
            {
                MessageBox.Show("请先连接！", "提示");
                return;
            }

            FMPreCondition form = new FMPreCondition(this.txtDataSource.Text, preConditions =>
            {
                btnPreCondition.Text = $"预设条件({preConditions.PreConditionList.Count})";

                _PreConditions = preConditions;
            });
            form.ShowDialog();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(txtDataSource.Text))
            {
                MessageBox.Show("请先填入连接字符串!");
                return;
            }

            if(!_IsConnecting)
            {
                try
                {
                    using (SqlConnection objConnection = new SqlConnection(txtDataSource.Text))
                    {
                        objConnection.Open();
                        objConnection.Close();
                    }
                }
                catch (Exception x)
                {
                    MessageBox.Show(x.Message, "Error");
                    return;
                }

                _PreConditions = PreConditionUtil.LoadData(txtDataSource.Text);

                btnPreCondition.Text = $"预设条件({_PreConditions.PreConditionList.Count})";
            }
            else
            {
                _PreConditions = null;

                btnPreCondition.Text = $"预设条件(0)";
            }

            btnConnect.Tag = !_IsConnecting;
            btnConnect.Text = _IsConnecting ? "断开" : "连接";
            txtDataSource.ReadOnly = bool.Parse(btnConnect.Tag.ToString());

            // 加载数据
            if (_IsConnecting)
            {
                SearchData();
            }
            else// 清理数据
            {
                bsData.DataSource = null;
                bsData.ResetBindings(true);

                bsColumnInfos.DataSource = null;
                bsColumnInfos.ResetBindings(true);

                bsColumnInfosSelected.DataSource = null;
                bsColumnInfosSelected.ResetBindings(true);
            }
        }
    }
}
