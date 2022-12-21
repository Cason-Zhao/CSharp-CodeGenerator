/***************************************************************************
 * 文件名：FMPreCondition
 * 功能：
 * 说明：
 * -------------------------------------------------------------------------
 * 创建时间：2022/12/21 11:00:05
 * 创建人：赵旋
 * 邮箱： 1192508693@qq.com
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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSharp_CodeGenerator2
{
    public partial class FMPreCondition : Form
    {
        private PreConditions _PreConditions { get; set; }
        private string _ConnectionString { get; set; }

        private Action<PreConditions> _SavedFunc { get; set; }

        public FMPreCondition(string connectionString, Action<PreConditions> savedFunc)
        {
            InitializeComponent();

            this._ConnectionString = connectionString;
            this._SavedFunc = savedFunc;
        }

        private void FMPreCondition_Load(object sender, EventArgs e)
        {
            _PreConditions = PreConditionUtil.LoadData(_ConnectionString);
            bsData.DataSource = _PreConditions.PreConditionList;
            bsData.ResetBindings(true);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                PreConditionUtil.SaveData(_PreConditions);
                if(_SavedFunc != null)
                {
                    _SavedFunc.Invoke(_PreConditions);
                }
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message, "Error");
                return;
            }
            this.Close();
        }

        private void btnCancell_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
