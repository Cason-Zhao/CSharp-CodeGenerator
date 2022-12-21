/***************************************************************************
 * 文件名：DBHelper
 * 功能：
 * 说明：
 * -------------------------------------------------------------------------
 * 创建时间：2022/12/13 16:55:19
 * 创建人：赵旋
 * 邮箱： 1192508693@qq.com
 * ========================================================================= 
 * 
 * 修改人：    
 * 修改时间：    
 * 修改说明：    
 ***************************************************************************/
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator
{
    public class DBHelper: IDisposable
    {
        #region Properties & Fields

        private SqlConnection _SqlConn = null;

        #endregion
        public DBHelper(string connectionStr = "")
        {
            this._SqlConn = new SqlConnection(connectionStr);
        }

        public DataTable GetTables(string filterText = "")
        {
            string commandStr = @"select TableName, TableType, TableDescription,ColumnName , ColumnDescription,IsPrimary ,IsIdentity,ColumnType ,ColumnMaxLength ,ColumnPrecision,ColumnScale, Nullable from 
(SELECT sys_objs.name AS TableName ,-- 表名
       sys_objs.type AS TableType ,-- 表类型
       CASE WHEN sys_cols.column_id = 1 THEN ISNULL(extProp.value, '') ELSE '' END AS TableDescription,-- 表说明
       sys_cols.name AS ColumnName ,-- 列名
       exprop.value AS ColumnDescription, -- 列说明
       CASE WHEN ISNULL(sys_ind_cols.column_id, 0) = 0 THEN 0 ELSE 1 END AS IsPrimary ,-- 列是否为主键
       sys_cols.is_identity as IsIdentity,-- 列是否自增
       sys_types.name AS ColumnType ,-- 列数据类型
       CAST(CASE WHEN sys_types.name IN ( N'nchar', N'nvarchar' ) AND sys_cols.max_length <> -1 
	             THEN sys_cols.max_length / 2 
				 ELSE sys_cols.max_length END AS INT) AS ColumnMaxLength ,-- 列最大字符数
       sys_cols.precision AS ColumnPrecision,-- （数值类型的）精度（数字总位数）
       sys_cols.scale AS ColumnScale,-- 小数点后有效位数
       sys_cols.is_nullable AS Nullable,-- 是否可空
	   sys_cols.column_id as ColumnId-- 列标识
FROM sys.columns sys_cols-- 系统~列信息
     INNER JOIN sys.objects sys_objs ON sys_cols.object_id = sys_objs.object_id -- 系统~对象信息
                                 AND sys_objs.type IN ( 'U', 'V' )
                                 AND sys_objs.name <> 'sysdiagrams' AND sys_objs.name <> 'dtproperties'
     LEFT OUTER JOIN sys.extended_properties extProp ON extProp.major_id = sys_cols.object_id -- 系统~扩展属性信息
                                                        AND extProp.minor_id = 0
                                                        AND extProp.class = 1
                                                        AND extProp.name = 'MS_Description'
     LEFT OUTER JOIN sys.types sys_types ON sys_types.user_type_id = sys_cols.user_type_id -- 系统~类型信息
     LEFT OUTER JOIN sys.indexes sys_inds ON sys_inds.object_id = sys_cols.object_id -- 系统~索引信息
                                             And ((sys_objs.type='U' And sys_inds.is_primary_key = 1) Or
												  (sys_objs.type='V' And sys_inds.type_desc='CLUSTERED' And sys_inds.is_unique=1))
     LEFT OUTER JOIN sys.index_columns sys_ind_cols ON sys_ind_cols.object_id = sys_cols.object_id -- 系统~索引列信息
                                                       AND sys_ind_cols.column_id = sys_cols.column_id
                                                       AND sys_inds.index_id = sys_ind_cols.index_id
     LEFT OUTER JOIN sys.extended_properties exprop ON exprop.major_id = sys_cols.object_id -- 系统~扩展属性信息
                                                       AND exprop.minor_id = sys_cols.column_id
                                                       AND exprop.class = 1
                                                       AND exprop.name = 'MS_Description'
-- ORDER BY sys_objs.type, sys_objs.name, sys_cols.column_id-- 按照表类型（表格、视图）、表名、列名排序
) as target";
            if (!string.IsNullOrEmpty(filterText))
            {
                commandStr += string.Format(@"
Where Convert(nvarchar(100), target.TableName) like '%'+ @tableName + '%' or Convert(nvarchar(100), target.TableDescription) like '%'+ @tableDescription + '%'", filterText);
            }
            commandStr += @"
ORDER BY target.TableType, target.TableName, target.ColumnId-- 按照表类型（表格、视图）、表名、列名排序
;";
            this._SqlConn.Open();
            using (var sqlCommand = new SqlCommand(commandStr, this._SqlConn))
            {
                sqlCommand.Parameters.AddWithValue("tableName", filterText);
                sqlCommand.Parameters.AddWithValue("tableDescription", filterText);

                using (SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand))
                {
                    var ds = new DataSet();
                    adapter.Fill(ds);
                    return ds.Tables[0];
                }
            }
                //return this._SqlConn.GetSchema("Tables");
        }

        public void Dispose()
        {
            if(this._SqlConn != null && this._SqlConn.State > System.Data.ConnectionState.Closed)
            {
                this._SqlConn.Close();
            }
        }
    }
}
