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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator
{
    public class PocoGenerator
    {
        public static string Generate(List<ColumnInfo> columnInfos, string pocoType = PocoType.A00)
        {
            var resultStrBuilder = new StringBuilder();

            PocoType pType = new PocoType();
            if (pocoType == PocoType.A00)
            {
                resultStrBuilder.AppendLine("/*");
                foreach (var item in pType.GetAllKeys().Where(p => p != PocoType.A00))
                {
                    columnInfos.ForEach(column => resultStrBuilder.AppendLine(string.Format(pType.GetFormat(item), column.Description, string.Format("{0}{1}", DataTypeConst.GetCSharpType(column.Type), column.Nullable ? string.Empty : ""), column.Name)));
                    resultStrBuilder.AppendLine();
                    resultStrBuilder.AppendLine();
                }
                resultStrBuilder.AppendLine("*/");
            }
            else
            {
                columnInfos.ForEach(column => resultStrBuilder.AppendLine(string.Format(pType.GetFormat(pocoType), column.Description, string.Format("{0}{1}", DataTypeConst.GetCSharpType(column.Type), column.Nullable ? string.Empty : ""), column.Name)));
            }

            return resultStrBuilder.ToString();
        }
    }

    public class PocoType
    {
        public static string _PocoTypeFolderPath = System.IO.Path.Combine($"{AppDomain.CurrentDomain.BaseDirectory}", @"AppData\PocoGenerator");
        public static string _PocoTypePath = System.IO.Path.Combine(_PocoTypeFolderPath, "PocoType.json");

        /// <summary>
        /// 模板类（全）
        /// </summary>
        public const string A00 = "00";

        private Dictionary<string, string> _Names;

        public Dictionary<string, string> GetPocoTypes()
        {
            if (!Directory.Exists(_PocoTypeFolderPath))
            {
                Directory.CreateDirectory(_PocoTypeFolderPath);
            }
            if (!File.Exists(_PocoTypePath))
            {
                using (File.Create(_PocoTypePath, 10000, FileOptions.Asynchronous))
                {

                }
                File.WriteAllText(_PocoTypePath, "{}");
            }

            var content = File.ReadAllText(_PocoTypePath);
            if (string.IsNullOrEmpty(content))
            {
                content = "{}";
            }

            return JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
        }

        public string GetFormat(string key)
        {
            if (!Directory.Exists(_PocoTypeFolderPath))
            {
                Directory.CreateDirectory(_PocoTypeFolderPath);
            }

            var filePath = Path.Combine(_PocoTypeFolderPath, $"{key}.txt");
            if (!File.Exists(filePath))
            {
                using (File.Create(filePath, 10000, FileOptions.Asynchronous))
                {

                }
            }

            var content = File.ReadAllText(filePath);

            return content;
        }

        public PocoType()
        {
            _Names = new Dictionary<string, string>();
            _Names.Add(A00, "模板类（全）");
            var typeList = GetPocoTypes().ToList();
            if (typeList != null && typeList.Count > 0)
            {
                typeList.ForEach(p => _Names.Add(p.Key, p.Value));
            }
        }

        public string[] GetAllKeys()
        {
            return _Names.Keys.ToArray();
        }

        public string GetName(string key)
        {
            return !_Names.ContainsKey(key) ? "" : _Names[key];
        }

        public List<KeyValuePair<string, string>> GetAllList()
        {
            return _Names.ToList();
        }
    }
}
