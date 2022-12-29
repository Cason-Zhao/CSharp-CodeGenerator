using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.ComUtil
{
    public static class FileHelper
    {
        #region CheckFileFormat

        /// <summary>
        /// 校验文件格式（真实文件类型）
        ///             文件扩展名说明
        ///                 4946/104116 txt
        ///                 7173        gif 
        ///                 255216      jpg
        ///                 13780       png
        ///                 6677        bmp
        ///                 239187      txt,aspx,asp,sql
        ///                 208207      xls.doc.ppt
        ///                 6063        xml
        ///                 6033        htm,html
        ///                 4742        js
        ///                 8075        xlsx,zip,pptx,mmap,zip
        ///                 8297        rar   
        ///                 01          accdb,mdb
        ///                 7790        exe,dll           
        ///                 5666        psd 
        ///                 255254      rdp 
        ///                 10056       bt种子 
        ///                 64101       bat 
        ///                 4059        sgf
        /// </summary>
        /// <param name="fileHeaders">文件头列表</param>
        /// <returns></returns>
        public static string CheckFileFormat(this Stream fs, params string[] fileHeaders)
        {
            if (fileHeaders.Length == 0)
            {
                throw new ArgumentNullException("fileHeaders");
            }

            string result = "";
            fs.Position = 0;
            fs.Seek(0, SeekOrigin.Begin);
            System.IO.BinaryReader br = new System.IO.BinaryReader(fs);
            try
            {
                byte buffer = br.ReadByte();
                result = buffer.ToString();

                buffer = br.ReadByte();
                result += buffer.ToString();
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("文件格式异常！未读取到完整文件头数据，请检查文件！"));
            }

            if (!fileHeaders.Contains(result))
            {
                throw new Exception(string.Format("文件格式异常！未知的文件头类型{0}，请检查文件或使用另存为修正文件格式！", result));
            }
            return result;
        }

        /// <summary>
        /// 校验Excel文件格式
        ///     仅支持xls、xlsx
        /// </summary>
        /// <param name="fs"></param>
        /// <returns></returns>
        public static string CheckExcelFileFormat(this Stream fs)
        {
            return fs.CheckFileFormat(FileHeaderConst.H208207, FileHeaderConst.H8075);
        }

        #endregion

        public class FileHeaderConst
        {
            /// <summary>
            /// xls、doc、ppt
            /// </summary>
            public const string H208207 = "208207";
            /// <summary>
            /// xlsx、zip、pptx、mmap、zip
            /// </summary>
            public const string H8075 = "8075";

            /// <summary>
            /// txt
            /// </summary>
            public const string H4946 = "4946";
            /// <summary>
            /// txt
            /// </summary>
            public const string H104116 = "104116";

            /// <summary>
            /// gif
            /// </summary>
            public const string H7173 = "7173";

            /// <summary>
            /// jpg
            /// </summary>
            public const string H255216 = "255216";

            /// <summary>
            /// png
            /// </summary>
            public const string H13780 = "13780";

            /// <summary>
            /// bmp
            /// </summary>
            public const string H6677 = "6677";

            /// <summary>
            /// txt,aspx,asp,sql
            /// </summary>
            public const string H239187 = "239187";

            /// <summary>
            /// xml
            /// </summary>
            public const string H6063 = "6063";

            /// <summary>
            /// htm,html
            /// </summary>
            public const string H6033 = "6033";

            /// <summary>
            /// js
            /// </summary>
            public const string H4742 = "4742";

            /// <summary>
            /// rar   
            /// </summary>
            public const string H8297 = "8297";

            /// <summary>
            /// accdb,mdb
            /// </summary>
            public const string H01 = "01";

            /// <summary>
            /// exe,dll 
            /// </summary>
            public const string H7790 = "7790";

            /// <summary>
            /// psd
            /// </summary>
            public const string H5666 = "5666";

            /// <summary>
            /// rdp
            /// </summary>
            public const string H255254 = "255254";

            /// <summary>
            /// bt种子
            /// </summary>
            public const string H10056 = "10056";

            /// <summary>
            ///  bat
            /// </summary>
            public const string H64101 = "64101";

            /// <summary>
            /// sgf
            /// </summary>
            public const string H4059 = "4059";
        }
    }
}
