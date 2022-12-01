/***************************************************************************
 * 文件名：Consts
 * 功能：
 * 说明：
 * -------------------------------------------------------------------------
 * 创建时间：2022/12/1 16:45:53
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
    /// 通用类型常量
    /// </summary>
    public class CommonTypeConsts
    {
        /// <summary>
        /// 
        /// </summary>
        public const string String = "string";
        public const string Int = "int";
        public const string Char = "char";

        //private static readonly List<string> _TypeList = new List<string>()
        //{
        //    TString,
        //    TInt,
        //    TChar,
        //};

        //static CommonTypeConsts()
        //{
        //    _TypeList.Add(nameof(Byte));
        //    _TypeList.Add(nameof(Int16));
        //    _TypeList.Add(nameof(Int64));
        //    _TypeList.Add(nameof(Single));
        //    _TypeList.Add(nameof(Double));
        //    _TypeList.Add(nameof(Decimal));
        //}

    }

    public class CommonRegexPattern
    {
        /// <summary>
        /// 适用情景，形如：
        ///     S:系统,U:用户
        ///     S:系统;U:用户
        /// </summary>
        public const string A = @"(?<=^|[;；,，])(\S+?)\s*[:：]\s*(\S+?)(?=$|[;；,，])";

        /// <summary>
        /// 适用情景，形如：
        ///     S:S|系统,U:U|用户
        ///     S:S|系统;U:U|用户
        /// </summary>
        public const string B = @"(?<=^|[;；,，])(\S+?)\s*[:：]\s*(\S+?)\|(\S+?)(?=$|[;；,，])";

        private static readonly Dictionary<string, string> Examples = new Dictionary<string, string>()
        {
            { A, "S:系统,U:用户"},
            { A, "S:1|系统,U:2|用户"},
        };
    }

    public class CommonPrefix
    {
        public const string T = "T";
        public const string C = "C";
    }


}
