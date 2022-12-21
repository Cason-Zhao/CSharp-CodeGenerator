/***************************************************************************
 * 文件名：PocoGenerator
 * 功能：
 * 说明：
 * -------------------------------------------------------------------------
 * 创建时间：2022/12/13 16:54:05
 * 创建人：赵旋
 * 邮箱： 1192508693@qq.com
 * ========================================================================= 
 * 
 * 修改人：    
 * 修改时间：    
 * 修改说明：    
 ***************************************************************************/
using CodeGenerator.DBUtil.Consts;
using CodeGenerator.DBUtil.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator
{
    public class PocoGenerator
    {
        public static string Generate(List<ColumnInfo> columnInfos, string pocoType = PocoType.A01)
        {
            var resultStrBuilder = new StringBuilder();
            columnInfos.ForEach(column => resultStrBuilder.AppendLine(string.Format(PocoType.GetFormat(pocoType), column.Description, string.Format("{0}{1}", DataTypeConst.GetCSharpType(column.Type), column.Nullable ? string.Empty : ""), column.Name)));

            return resultStrBuilder.ToString();
        }
    }

    public class PocoType
    {
        public const string A01 = "01";
        public const string A02 = "02";
        public const string A03 = "03";
        public const string A04 = "04";
        public const string A05 = "05";

        private static readonly Dictionary<string, string> _Names = new Dictionary<string, string>()
        {
            { A01, "模板类"},
            { A02, "模板类（无说明）"},
            { A03, "以逗号隔开的属性名称"},
            { A04, "Linq结果属性赋值"},
            { A05, "对象属性复制"},
        };

        private static readonly Dictionary<string, string> _Formats = new Dictionary<string, string>();

        static PocoType()
        {
            _Formats.Add(A01, @"
    /// <summary>
    /// {0}
    /// </summary>
    public {1} {2} {{ get; set; }}");

            _Formats.Add(A02, @"
    public {1} {2} {{ get; set; }}");

            _Formats.Add(A03, @"    {2},");
            _Formats.Add(A04, @"    {2} = p.{2},");
            _Formats.Add(A05, @"    this.{2} = data.{2};");
        }

        public static string GetFormat(string key)
        {
            return !_Formats.ContainsKey(key) ? "" : _Formats[key];
        }

        public static string[] GetAllKeys()
        {
            return _Names.Keys.ToArray();
        }

        public static string GetName(string key)
        {
            return !_Names.ContainsKey(key) ? "" : _Names[key];
        }

        public static List<KeyValuePair<string, string>> GetAllList()
        {
            return _Names.ToList();
        }
    }
}
