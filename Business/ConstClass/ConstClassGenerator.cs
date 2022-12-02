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
            Analysis((ConstClassParamModel)model);

            var constFieldContent = model.FieldModels
                .Select(p => string.Format(@"        /// <summary>
        /// {0}
        /// </summary>
        public const {1} {2}{3} = {4};", p.FieldSummary, model.FieldType, model.FieldPrefix, p.FieldName, p.FieldValue));

            var namesContent = model.FieldModels
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

        private static void Analysis(ConstClassParamModel model)
        {
            model.FieldModels = new List<ConstClassFieldModel>();

            //var regex = new Regex(model.RegexPattern);

            var matchesX = Regex.Matches(model.InputString, model.RegexPattern)
                .OfType<Match>()
                .ToList();

            model.GenerateFieldPattern = GetGenerateFieldPattern(matchesX);

            var matches = matchesX
                .Select(p => new ConstClassFieldModel 
                { 
                    FieldName = GetFieldMetadata(p, model.GetFieldPatternProperty(y => y.FieldNamePattern)),
                    FieldValue = GetFieldMetadata(p, model.GetFieldPatternProperty(y => y.FieldValuePattern)),
                    FieldSummary = GetFieldMetadata(p, model.GetFieldPatternProperty(y => y.FieldSummaryPattern)),
                })
                .ToList();

            if(matches.All(p => p.FieldValue.IsNumeric()))
            {
                model.GenerateFieldPattern.FieldType = CommonTypeConsts.Int;
                if (!string.IsNullOrEmpty(model.ClassName))
                {
                    if (model.ClassName.ToLower().Contains("type"))
                    {
                        model.GenerateFieldPattern.FieldPrefix = CommonPrefix.T;
                    }
                    else if (model.ClassName.ToLower().Contains("code"))
                    {
                        model.GenerateFieldPattern.FieldPrefix = CommonPrefix.C;
                    }
                }
            }
            else
            {
                matches.ForEach(data => data.FieldValue = $"\"{data.FieldValue}\"");
            }
            model.FieldModels.AddRange(matches);
        }
        private static FieldPattern GetGenerateFieldPattern(List<Match> matches)
        {
            var result = new FieldPattern();

            var count = matches.Max(p => p.Groups.Count);
            if(count == 3)
            {
                result.FieldNamePattern = "$1";
                result.FieldValuePattern = "$1";
                result.FieldSummaryPattern = "$2";
            }
            if(count == 4)
            {
                result.FieldNamePattern = "$1";
                result.FieldValuePattern = "$2";
                result.FieldSummaryPattern = "$3";
            }

            return result;
        }

        private static string GetFieldMetadata(Match match, string pattern)
        {
            var result = pattern;
            Regex.Matches(pattern, @"(\$)(\d+)")
                .OfType<Match>()
                .Select(p => new
                {
                    Node = p.Groups[1].ToString() + p.Groups[2].ToString(),
                    Index = Convert.ToInt32(p.Groups[2])
                })
                .ToList()
                .ForEach(data => result = result.Replace(data.Node, match.Groups[data.Index].ToString()))
                ;
            return result;
        }

        // 解析正则表达式匹配组的个数，并支持对各个位置进行默认配置与修改配置

    }
}
