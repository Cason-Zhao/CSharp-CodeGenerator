/***************************************************************************
 * 文件名：TableTypeEnum
 * 功能：
 * 说明：
 * -------------------------------------------------------------------------
 * 创建时间：2022/12/13 23:48:16
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
    /// <summary>
    /// 表格类型常量
    /// </summary>
    public class TableTypeConst
    {
        /// <summary>
        /// 表格
        /// </summary>
        public const string U = "U";
        /// <summary>
        /// 视图
        /// </summary>
        public const string V = "V";

        private static readonly Dictionary<string, string> _Names = new Dictionary<string, string>
        {
           { U, "表格" },
           { V, "视图" }
        };

        /// <summary>
        /// 常量列表
        /// </summary>
        /// <returns></returns>
        public static string[] GetAllKeys()
        {
            return _Names.Keys.ToArray();
        }

        /// <summary>
        /// 获取名称
        /// </summary>
        /// <returns></returns>
        public static string GetName(string key)
        {
            return _Names.ContainsKey(key) ? _Names[key] : string.Empty;
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        public static List<KeyValuePair<string, string>> GetAllList()
        {
            return _Names.ToList();
        }
    }
}
