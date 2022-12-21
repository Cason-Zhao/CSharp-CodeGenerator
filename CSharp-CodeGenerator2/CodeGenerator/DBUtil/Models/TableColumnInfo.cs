/***************************************************************************
 * 文件名：TableColumnInfo
 * 功能：
 * 说明：
 * -------------------------------------------------------------------------
 * 创建时间：2022/12/13 23:55:42
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
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.DBUtil.Models
{
    public class ColumnInfo : ExtendInfo
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Type { get; set; }

        public int Length { get; set; }

        public int Decimals { get; set; }

        public bool Nullable { get; set; }

        public bool IsPrimaryKey { get; set; }

        public bool IsIdentity { get; set; }

        public ColumnInfo()
        {

        }

        public ColumnInfo(DataRow row)
        {
            this.Name = row["ColumnName"].ToString();
            this.Description = row["ColumnDescription"].ToString();
            this.Type = row["ColumnType"].ToString();
            this.Nullable = Convert.ToBoolean(row["Nullable"]);
            this.IsPrimaryKey = (int)row["IsPrimary"] == 1;
            this.IsIdentity = (bool)row["IsIdentity"];

            switch (this.Type)
            {
                case DataTypeConst.varchar:
                case DataTypeConst.@char:
                case DataTypeConst.nvarchar:
                case DataTypeConst.nchar:
                    this.Length = Convert.ToInt32(row["ColumnMaxLength"]);
                    break;

                case DataTypeConst.@decimal:
                case DataTypeConst.numeric:
                    this.Length = Convert.ToInt32(row["ColumnPrecision"]);
                    this.Decimals = Convert.ToInt32(row["ColumnScale"]);
                    break;

                case DataTypeConst.varbinary:
                case DataTypeConst.binary:
                    this.Length = Convert.ToInt32(row["ColumnMaxLength"]);
                    break;
                case DataTypeConst.time:
                    this.Length = Convert.ToInt32(row["ColumnScale"]);
                    break;
                default:
                    this.Length = Convert.ToInt32(row["ColumnMaxLength"]);
                    break;
            }
        }
    }

    public class ExtendInfo
    {
        public bool IsSelected { get; set; }
    }
}
