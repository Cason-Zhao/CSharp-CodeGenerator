/***************************************************************************
 * 文件名：DataTypeConst
 * 功能：
 * 说明：
 * -------------------------------------------------------------------------
 * 创建时间：2022/12/14 0:15:32
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.DBUtil.Consts
{
    // .*?\("(\S+)", "(\S+)"\);\r\n\s+
    // $1:$2,
    /// <summary>
    /// 
    /// </summary>
    public class DataTypeConst
    {
        /// <summary>
        /// long
        /// </summary>
        public const string bigint = "bigint";
        /// <summary>
        /// System.Data.Linq.Binary
        /// </summary>
        public const string binary = "binary";
        /// <summary>
        /// bool
        /// </summary>
        public const string bit = "bit";
        /// <summary>
        /// string
        /// </summary>
        public const string @char = "char";
        /// <summary>
        /// System.DateTime
        /// </summary>
        public const string date = "date";
        /// <summary>
        /// System.DateTime
        /// </summary>
        public const string datetime = "datetime";
        /// <summary>
        /// System.DateTime
        /// </summary>
        public const string datetime2 = "datetime2";
        /// <summary>
        /// System.DateTimeOffset
        /// </summary>
        public const string datetimeoffset = "datetimeoffset";
        /// <summary>
        /// decimal
        /// </summary>
        public const string @decimal = "decimal";
        /// <summary>
        /// double
        /// </summary>
        public const string @float = "float";
        /// <summary>
        /// System.Data.Linq.Binary
        /// </summary>
        public const string image = "image";
        /// <summary>
        /// int
        /// </summary>
        public const string @int = "int";
        /// <summary>
        /// decimal
        /// </summary>
        public const string money = "money";
        /// <summary>
        /// string
        /// </summary>
        public const string nchar = "nchar";
        /// <summary>
        /// string
        /// </summary>
        public const string ntext = "ntext";
        /// <summary>
        /// decimal
        /// </summary>
        public const string numeric = "numeric";
        /// <summary>
        /// string
        /// </summary>
        public const string nvarchar = "nvarchar";
        /// <summary>
        /// float
        /// </summary>
        public const string real = "real";
        /// <summary>
        /// System.DateTime
        /// </summary>
        public const string smalldatetime = "smalldatetime";
        /// <summary>
        /// short
        /// </summary>
        public const string smallint = "smallint";
        /// <summary>
        /// decimal
        /// </summary>
        public const string smallmoney = "smallmoney";
        /// <summary>
        /// object
        /// </summary>
        public const string sql_variant = "sql_variant";
        /// <summary>
        /// string
        /// </summary>
        public const string text = "text";
        /// <summary>
        /// System.TimeSpan
        /// </summary>
        public const string time = "time";
        /// <summary>
        /// System.Data.Linq.Binary
        /// </summary>
        public const string timestamp = "timestamp";
        /// <summary>
        /// byte
        /// </summary>
        public const string tinyint = "tinyint";
        /// <summary>
        /// System.Guid
        /// </summary>
        public const string uniqueidentifier = "uniqueidentifier";
        /// <summary>
        /// System.Data.Linq.Binary
        /// </summary>
        public const string varbinary = "varbinary";
        /// <summary>
        /// string
        /// </summary>
        public const string varchar = "varchar";
        /// <summary>
        /// System.Xml.Linq.XElement
        /// </summary>
        public const string xml = "xml";

        private static readonly Dictionary<string, string> _SQLType2CSharpTypeMap = new Dictionary<string, string>
        {
           { bigint, "long" },
           { binary, "System.Data.Linq.Binary" },
           { bit, "bool" },
           { @char, "string" },
           { date, "System.DateTime" },
           { datetime, "System.DateTime" },
           { datetime2, "System.DateTime" },
           { datetimeoffset, "System.DateTimeOffset" },
           { @decimal, "decimal" },
           { @float, "double" },
           { image, "System.Data.Linq.Binary" },
           { @int, "int" },
           { money, "decimal" },
           { nchar, "string" },
           { ntext, "string" },
           { numeric, "decimal" },
           { nvarchar, "string" },
           { real, "float" },
           { smalldatetime, "System.DateTime" },
           { smallint, "short" },
           { smallmoney, "decimal" },
           { sql_variant, "object" },
           { text, "string" },
           { time, "System.TimeSpan" },
           { timestamp, "System.Data.Linq.Binary" },
           { tinyint, "byte" },
           { uniqueidentifier, "System.Guid" },
           { varbinary, "System.Data.Linq.Binary" },
           { varchar, "string" },
           { xml, "System.Xml.Linq.XElement" }
        };

        /// <summary>
        /// 常量列表
        /// </summary>
        /// <returns></returns>
        public static string[] GetAllKeys()
        {
            return _SQLType2CSharpTypeMap.Keys.ToArray();
        }

        /// <summary>
        /// 获取名称
        /// </summary>
        /// <returns></returns>
        public static string GetCSharpType(string key)
        {
            return _SQLType2CSharpTypeMap.ContainsKey(key) ? _SQLType2CSharpTypeMap[key] : string.Empty;
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        public static List<KeyValuePair<string, string>> GetAllList()
        {
            return _SQLType2CSharpTypeMap.ToList();
        }
    }
}
