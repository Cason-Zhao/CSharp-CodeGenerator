/***************************************************************************
 * 文件名：PreCondition
 * 功能：
 * 说明：
 * -------------------------------------------------------------------------
 * 创建时间：2022/12/21 11:06:07
 * 创建人：赵旋
 * 邮箱： 1192508693@qq.com
 * ========================================================================= 
 * 
 * 修改人：    
 * 修改时间：    
 * 修改说明：    
 ***************************************************************************/
using CodeGenerator.ComUtil;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.DBUtil.Models
{
    public class PreCondition
    {
        public string FilterText { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    public class PreConditions
    {
        public string ConnectionString { get; set; }

        [JsonIgnore]
        public string FileName { get { return PreConditionUtil.GetFileName(this.ConnectionString); } }

        public List<PreCondition> PreConditionList { get; set; }
    }

    /// <summary>
    /// 预设条件工具类
    ///     文件结构
    ///         结构文件（连接字符串与预设条件文件之间的映射关系）
    ///         预设条件文件（预设条件文件名，文件内以List<string> Json字符串的形式存储）
    ///         
    /// </summary>
    public class PreConditionUtil
    {
        public static string _PreConditionFolderPath = System.IO.Path.Combine($"{AppDomain.CurrentDomain.BaseDirectory}", @"AppData\PreCondition");
        public static string _PreConditionMapPath = System.IO.Path.Combine($"{_PreConditionFolderPath}", "PreConditionMap.json");

        private static Dictionary<string, string> GetMap()
        {
            if(!Directory.Exists(_PreConditionFolderPath))
            {
                Directory.CreateDirectory(_PreConditionFolderPath);
            }
            if (!File.Exists(_PreConditionMapPath))
            {
                using (File.Create(_PreConditionMapPath, 10000, FileOptions.Asynchronous))
                {

                }
                File.WriteAllText(_PreConditionMapPath, "{}");
            }

            var content = File.ReadAllText(_PreConditionMapPath);
            if(string.IsNullOrEmpty(content))
            {
                content = "{}";
            }

            return JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
        }

        /// <summary>
        /// Add
        /// </summary>
        /// <param name="map"></param>
        public static string AddMap(string connectionString)
        {
            if (!Directory.Exists(_PreConditionFolderPath))
            {
                Directory.CreateDirectory(_PreConditionFolderPath);
            }
            if (!File.Exists(_PreConditionMapPath))
            {
                File.Create(_PreConditionMapPath);
            }
            string connectionFileName = GetFileName(connectionString);

            var map = GetMap();
            if(!map.ContainsKey(connectionString))
            {
                map.Add(connectionString, connectionFileName);
                File.WriteAllText(_PreConditionMapPath, JsonConvert.SerializeObject(map, Formatting.Indented));
            }
            return connectionFileName;
        }

        public static string GetFileName(string connectionString)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                return $"{sqlConnection.DataSource}{sqlConnection.Database}".ToLower().EncodeMD5().Replace("-", "");
            }
        }

        public static PreConditions LoadData(string connectionString)
        {
            var result = new PreConditions();
            result.ConnectionString = connectionString;

            // 获取映射关系信息
            AddMap(connectionString);

            var maps = GetMap();
            if (maps == null || maps.Count == 0)
            {
                return result;
            }

            // 读取文件并反序列化
            var filePath = Path.Combine(_PreConditionFolderPath, $"{maps[connectionString]}.json");
            if (!File.Exists(filePath))
            {
                using(var fileStream = File.Create(filePath))
                {
                }
                File.WriteAllText(filePath, "[]");
            }

            result.PreConditionList = JsonConvert.DeserializeObject<List<PreCondition>>(File.ReadAllText(filePath));

            return result;
        }

        public static List<PreConditions> LoadDatas()
        {
            return GetMap().Select(p => LoadData(p.Key)).ToList();
        }

        public static void SaveData(PreConditions preConditions)
        {
            if (!Directory.Exists(_PreConditionFolderPath))
            {
                Directory.CreateDirectory(_PreConditionFolderPath);
            }
            var filePath = Path.Combine(_PreConditionFolderPath, $"{preConditions.FileName}.json");

            if (!File.Exists(filePath))
            {
                using (File.Create(filePath, 10000, FileOptions.Asynchronous))
                {

                }
            }

            File.WriteAllText(filePath, JsonConvert.SerializeObject(preConditions.PreConditionList, Formatting.Indented));
        }
    }
}
