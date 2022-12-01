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
    /// <summary>
    /// 常量类参数模型
    /// </summary>
    public class ConstClassParamModel
    {
        public string ClassName { get; set; } = "ClassNameConst";

        public string ClassSummary { get; set; } = "";

        public string FieldType { get; set; } = CommonTypeConsts.String;

        public string FieldPrefix { get; set; } = "";

        public string RegexPattern { get; set; } = CommonRegexPattern.A;

        public string InputString { get; set; } = "";

        public ConstClassFieldModel FieldModel { get; set; }

        public void Validate()
        {
            if(string.IsNullOrWhiteSpace(RegexPattern))
            {
                throw new Exception("正则表达式不可为空！");
            }
            if (string.IsNullOrWhiteSpace(InputString))
            {
                throw new Exception("输入字符串不可为空！");
            }
        }
    }

    public class ConstClassFieldModel: List<ConstClassFieldModel>
    {
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
        public string FieldSummary { get; set; }
    }
}
