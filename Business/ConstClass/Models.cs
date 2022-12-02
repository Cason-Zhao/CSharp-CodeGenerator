/***************************************************************************
 * 文件名：Models
 * 功能：
 * 说明：
 * -------------------------------------------------------------------------
 * 创建时间：2022/12/1 19:53:25
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

namespace Business.ConstClass
{
    #region Old
    ///// <summary>
    ///// 常量类参数模型
    ///// </summary>
    //public class ConstClassParamModel: ConstClassBaseParamModel
    //{
    //    public string ClassName { get; set; } = "ClassNameConst";

    //    public string ClassSummary { get; set; } = "";

    //    public ConstClassFieldModel FieldModel { get; set; }

    //    /// <summary>
    //    /// 解析生成
    //    /// </summary>
    //    public ConstClassParamModel ParamModel { get; set; }

    //    public void Validate()
    //    {
    //        if(string.IsNullOrWhiteSpace(RegexPattern))
    //        {
    //            throw new Exception("正则表达式不可为空！");
    //        }
    //        if (string.IsNullOrWhiteSpace(InputString))
    //        {
    //            throw new Exception("输入字符串不可为空！");
    //        }
    //    }
    //}

    //public class ConstClassBaseParamModel
    //{
    //    public string RegexPattern { get; set; } = CommonRegexPattern.A;

    //    public string InputString { get; set; } = "";

    //    public string FieldType { get; set; } = CommonTypeConsts.String;

    //    public string FieldPrefix { get; set; } = "";
    //} 

    //internal class ConstClassAnalysisParamModel : ConstClassBaseParamModel
    //{
    //    public List<ConstClassFieldModel> FieldModels { get; set; }
    //}

    #endregion

    public class ConstClassFieldModel
    {
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
        public string FieldSummary { get; set; }
    }

    public class ConstClassParamModel
    {
        public string ClassName { get; set; } = "ClassNameConst";

        public string ClassSummary { get; set; } = "";

        public string RegexPattern { get; set; } = CommonRegexPattern.A;

        public string InputString { get; set; } = "";

        public string FieldType { get { return GetFieldPatternProperty(p => p.FieldType); } }

        public string FieldPrefix { get { return GetFieldPatternProperty(p => p.FieldPrefix); } }

        public FieldPattern CustomFieldPattern { get; set; }
        public FieldPattern GenerateFieldPattern { get; set; }

        public List<ConstClassFieldModel> FieldModels { get; set; }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(RegexPattern))
            {
                throw new Exception("正则表达式不可为空！");
            }
            if (string.IsNullOrWhiteSpace(InputString))
            {
                throw new Exception("输入字符串不可为空！");
            }
        }

        public string GetFieldPatternProperty(Func<FieldPattern, string> getProperty)
        {
            var cusProperty = getProperty(CustomFieldPattern);
            return this.CustomFieldPattern == null || string.IsNullOrWhiteSpace(cusProperty) ? getProperty(this.GenerateFieldPattern) : cusProperty;
        }
    }

    public class FieldPattern
    {
        public string FieldNamePattern { get; set; } = "";
        public string FieldValuePattern { get; set; } = "";

        public string FieldSummaryPattern { get; set; } = "";

        public string FieldType { get; set; } = CommonTypeConsts.String;

        public string FieldPrefix { get; set; } = CommonPrefix.C;
    }
}
