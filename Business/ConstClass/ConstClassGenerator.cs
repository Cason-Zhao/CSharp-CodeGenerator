/***************************************************************************
 * 文件名：ConstClassGenerator
 * 功能：
 * 说明：
 * -------------------------------------------------------------------------
 * 创建时间：2022/12/1 16:44:58
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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Business.Utils;

namespace Business.ConstClass
{
    /// <summary>
    /// 
    /// </summary>
    public class ConstClassGenerator
    {
        public static string Generate(ConstClassParamModel model)
        {
            if(model == null)
            {
                throw new ArgumentNullException("model");
            }

            model.Validate();
            var analysisResult = Analysis(model);

            var constFieldContent = analysisResult.FieldModel
                .Select(p => string.Format(@"        /// <summary>
        /// {0}
        /// </summary>
        public const {1} {2}{3} = {4};", p.FieldSummary, model.FieldType, model.FieldPrefix, p.FieldName, p.FieldValue));

            var namesContent = analysisResult.FieldModel
                .Select(p => string.Format(@"           {{ {0}{1}, {2} }}", model.FieldPrefix, p.FieldName, $"\"{p.FieldSummary}\""));

            var result = new StringBuilder();
            result.Append(string.Format(@"    /// <summary>
    /// {0}
    /// </summary>
    public class {1}
    {{
{2}

        private static readonly Dictionary<{3}, string> _Names = new Dictionary<{3}, string>
        {{
{4}
        }};

        /// <summary>
        /// 常量列表
        /// </summary>
        /// <returns></returns>
        public static {3}[] GetAllKeys()
        {{
            return _Names.Keys.ToArray();
        }}

        /// <summary>
        /// 获取名称
        /// </summary>
        /// <returns></returns>
        public static string GetName({3} key)
        {{
            return _Names.ContainsKey(key) ? _Names[key] : string.Empty;
        }}

         /// <summary>
         /// 获取列表
         /// </summary>
         /// <returns></returns>
         public static List<KeyValuePair<{3}, string>> GetAllList()
         {{
             return _Names.ToList();
         }}
    }}", model.ClassSummary, model.ClassName, string.Join(Environment.NewLine, constFieldContent), model.FieldType, string.Join("," + Environment.NewLine, namesContent)));

            return result.ToString();
        }

        public static ConstClassParamModel Analysis(ConstClassParamModel model)
        {
            ConstClassParamModel result = new ConstClassParamModel();
            result.ClassName = model.ClassName;
            result.ClassSummary = model.ClassSummary;
            result.FieldType = model.FieldType;
            result.FieldPrefix = model.FieldPrefix;
            result.FieldModel = new ConstClassFieldModel();

            //var regex = new Regex(model.RegexPattern);
            
            var matches = Regex.Matches(model.InputString, model.RegexPattern)
                .OfType<Match>()
                .Select(p => new ConstClassFieldModel 
                { 
                    FieldName = p.Groups[1].ToString(),
                    FieldValue = p.Groups[1].ToString(),
                    FieldSummary = p.Groups[2].ToString()
                })
                .ToList();
            if(matches.All(p => p.FieldValue.IsNumeric()))
            {
                result.FieldType = CommonTypeConsts.Int;
                if(string.IsNullOrEmpty(result.FieldPrefix))
                {
                    result.FieldPrefix = CommonPrefix.C;
                }
            }
            else
            {
                matches.ForEach(data => data.FieldValue = $"\"{data.FieldValue}\"");
            }
            result.FieldModel.AddRange(matches);
            return result;
        }

        // 解析正则表达式匹配组的个数，并支持对各个位置进行默认配置与修改配置

    }
}
