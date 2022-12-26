using CodeGenerator;
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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSharp_CodeGenerator2
{
    public partial class FMPocoGeneratorY : Form
    {
        #region PropertiesInfo

        private List<TableInfo> _TableInfos;
        private bool _IsConnecting
        {
            get { return bool.Parse(btnConnect.Tag == null || string.IsNullOrWhiteSpace(btnConnect.Tag.ToString()) ? "false" : btnConnect.Tag.ToString()); }
        }

        private PreConditions _PreConditions;

        private DateTime _ConnectStartTime;
        private DateTime _ConnectEndTime;
        private int _ConnectMilliseconds;

        delegate void AsynUpdateProgressBar(int step);
        private BackgroundWorker _Worker = new BackgroundWorker();
        
        #endregion

        public FMPocoGeneratorY()
        {
            InitializeComponent();

            txtDataSource.Text = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
            this.Text += $"({Assembly.GetEntryAssembly().GetName().Version.ToString()})";
        
            _Worker.WorkerReportsProgress = true;
            _Worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            _Worker.RunWorkerCompleted+=new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);                       
        }

        private void FMPocoGeneratorY_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(this.txtDataSource.Text))
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

        #region gridTables

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

        #endregion

        #region gridColumns

        private void gridColumns_CellClick(object sender, DataGridViewCellEventArgs e)
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

        #endregion

        #region gridColumnSelected

        private void gridColumnSelected_CellClick(object sender, DataGridViewCellEventArgs e)
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

        #endregion

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
            _ConnectStartTime = DateTime.Now;           

            _Worker.RunWorkerAsync();
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

            if (!timer1.Enabled)
            {
                timer1.Start();
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

            Action action = BindData;
            this.Invoke(action);
        }

        private void BindData()
        {
            bsData.DataSource = _TableInfos;
            bsData.ResetBindings(true);
            if (_TableInfos.Count > 0)
            {
                bsColumnInfos.DataSource = _TableInfos.First().ColumnInfos;

                bsColumnInfosSelected.DataSource = _TableInfos.SelectMany(p => p.ColumnInfos.Where(y => y.IsSelected).ToList()).ToList();
                bsColumnInfosSelected.ResetBindings(true);
            }

            var timeSpan = DateTime.Now - _ConnectStartTime; // 连接后时间
            var connectMilliseconds = timeSpan.TotalMilliseconds; // 连接所需毫秒数

            var nowConMilliseconds = (connectMilliseconds + (_ConnectMilliseconds == 0 ? _ConnectMilliseconds : _ConnectMilliseconds)) / 2;// 新的平均连接时间
            PreConditionUtil.SetConnectionMilliseconds(txtDataSource.Text, nowConMilliseconds);

            _ConnectMilliseconds = (int)nowConMilliseconds;

            InitProgressBar();
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
            if (selectedList.Count == 0)
            {
                MessageBox.Show("请先选择列信息！");
                return;
            }
            GeneratedCodes.Clear();

            #region Code

            PocoType pType = new PocoType();
            pType.GetAllKeys()
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

            listVTargetMenu.Items.AddRange(pType.GetAllList().Select(p => new ListViewItem(p.Value)
            {
                Name = $"A{p.Key}",
                Tag = p.Key,
            }).ToArray());

            listVTargetMenu.EndUpdate();

            if (listVTargetMenu.Items.Count > 0)
            {
                this.listVTargetMenu.Items[0].Selected = true;
                this.listVTargetMenu.Select();
            }

            #endregion

            #region Panel

            richTxtTarget.Text = GeneratedCodes.First().Value;

            #endregion
        }

        private void listVTargetMenu_Click(object sender, EventArgs e)
        {
            if (GeneratedCodes.Count > 0)
            {
                var selectedItem = listVTargetMenu.Items.OfType<ListViewItem>().Where(p => p.Focused).FirstOrDefault();
                if (selectedItem != null)
                {
                    var pocoType = selectedItem.Tag.ToString();
                    richTxtTarget.Text = !GeneratedCodes.ContainsKey(pocoType) ? "" : GeneratedCodes[pocoType];
                }
            }
        }

        // 预设条件
        private void btnPreCondition_Click(object sender, EventArgs e)
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

            FMPreCondition form = new FMPreCondition(this.txtDataSource.Text, preConditions =>
            {
                btnPreCondition.Text = $"预设条件({preConditions.PreConditionList.Count})";

                _PreConditions = preConditions;
            });
            form.ShowDialog();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            _ConnectStartTime = DateTime.Now;

            if (string.IsNullOrEmpty(txtDataSource.Text))
            {
                MessageBox.Show("请先填入连接字符串!");
                return;
            }

            if (!_IsConnecting)
            {
                try
                {
                    _ConnectMilliseconds = (int)PreConditionUtil.GetConnectionMilliseconds(txtDataSource.Text); // 之前所有连接的平均时间
                    InitProgressBar();

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
                _Worker.RunWorkerAsync();
                timer1.Start();
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

        // 排序
        private void gridTables_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            SortTable(gridTables, e);
        }

        // 排序
        private void gridColumns_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            SortTable(gridColumns, e);
        }

        // 排序
        private void gridColumnSelected_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            SortTable(gridColumnSelected, e);
        }

        // 排序
        private void SortTable(DataGridView dataGridView, DataGridViewRowPostPaintEventArgs e)
        {
            //自动编号，与数据无关
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
               e.RowBounds.Location.Y,
               dataGridView.RowHeadersWidth - 4,
               e.RowBounds.Height);
            TextRenderer.DrawText(e.Graphics,
                  (e.RowIndex + 1).ToString(),
                   dataGridView.RowHeadersDefaultCellStyle.Font,
                   rectangle,
                   dataGridView.RowHeadersDefaultCellStyle.ForeColor,
                   TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        #region 进度条

        private void InitProgressBar()
        {
            progressBar.Maximum = _ConnectMilliseconds == 0 ? 100 : _ConnectMilliseconds; ;
            progressBar.Step = progressBar.Maximum / 100;
            progressBar.Value = 0;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (progressBar.Value < progressBar.Maximum)
            {
                progressBar.PerformStep();
                lblProgressBar.Text = (progressBar.Value * 100 / progressBar.Maximum).ToString() + "%";
            }
            else
            {
                lblProgressBar.Text = "99%";
            }
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            timer1.Stop();
            this.progressBar.Value = this.progressBar.Maximum;
            lblProgressBar.Text = "连接完成";
        }

        void worker_DoWork(object sernder, DoWorkEventArgs e)
        {
            SearchData();
        }

        #endregion

        private void btnEdit_Click(object sender, EventArgs e)
        {
            FMConditionSave form = new FMConditionSave();
            form.Show();
        }
    }
}
