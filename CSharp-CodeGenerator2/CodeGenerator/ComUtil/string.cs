/***************************************************************************
 * 文件名：@string
 * 功能：
 * 说明：
 * -------------------------------------------------------------------------
 * 创建时间：2022/12/21 13:38:32
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
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.ComUtil
{
    public static partial class @string
    {
        /// <summary>  
        /// MD5加密  
        /// </summary>  
        /// <param name="s">需要加密的字符串</param>  
        /// <returns>加密后的字符串</returns>  
        public static string EncodeMD5(this string inputStr)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            return BitConverter.ToString(md5.ComputeHash(Encoding.Default.GetBytes(inputStr)));
        }
    }
}
