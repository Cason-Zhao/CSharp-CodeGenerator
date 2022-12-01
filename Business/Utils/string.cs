/***************************************************************************
 * 文件名：@string
 * 功能：
 * 说明：
 * -------------------------------------------------------------------------
 * 创建时间：2022/12/1 20:56:27
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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Utils
{
    public static class @string
    {
        public static bool IsNumeric(this object value)
        {
            double defaultValue;
            return double.TryParse(value.ToString(), NumberStyles.Any, NumberFormatInfo.InvariantInfo, out defaultValue);
        }
    }
}
