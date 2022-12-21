/***************************************************************************
 * 文件名：TableInfo
 * 功能：
 * 说明：
 * -------------------------------------------------------------------------
 * 创建时间：2022/12/13 23:45:30
 * 创建人：赵旋
 * 邮箱： 1192508693@qq.com
 * ========================================================================= 
 * 
 * 修改人：    
 * 修改时间：    
 * 修改说明：    
 ***************************************************************************/
using CodeGenerator.DBUtil.Consts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.DBUtil.Models
{
    public class TableInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string TypeName { get { return TableTypeConst.GetName((this.Type ?? "").Trim()); } }

        public List<ColumnInfo> ColumnInfos { get; set; }
    }
}
