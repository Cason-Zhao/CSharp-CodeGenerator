using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

using CodeGenerator.ComUtil;
using System.Text.RegularExpressions;

namespace CodeGenerator.ExcelUtil
{
    public static class ExcelUtil
    {
        public static DataTable ExcelToDataTable(string fileName, string sheetName = "", bool isFirstRowTitle = true, bool isDeleteFile = false)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new System.Exception("选择文件有误！请重新选择文件");
            }
            IWorkbook workbook = fileName.GetWorkbook();
            if (workbook == null)
            {
                throw new System.Exception("选择文件有误！请重新选择文件");
            }
            ISheet iSheet = workbook.GetISheet(sheetName, null);
            if (iSheet == null)
            {
                throw new System.Exception("文件中Sheet不存在！请检查文件。");
            }
            DataTable result = SheetToDataTable(iSheet, isFirstRowTitle);
            if (isDeleteFile)
            {
                DeleteFile(fileName);
            }
            return result;
        }

        public static IWorkbook GetWorkbook(this string fileName)
        {
            IWorkbook workbook;
            using (System.IO.FileStream fileStream = new System.IO.FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                workbook = fileStream.GetWorkbook(OfficeType.OfficeNull);
            }
            return workbook;
        }

        public static IWorkbook GetWorkbook(this System.IO.Stream stream, OfficeType officeType = OfficeType.Office2007)
        {
            IWorkbook result;
            if (stream is System.IO.FileStream)
            {
                System.IO.FileStream fileStream = (System.IO.FileStream)stream;
                officeType = fileStream.GetRealOfficeType();
                if (!System.IO.File.Exists(fileStream.Name) || fileStream.Length == 0L)
                {
                    result = null;
                    return result;
                }
            }
            stream.Position = 0L;
            switch (officeType)
            {
                case OfficeType.Office2003:
                    result = new HSSFWorkbook(stream);
                    break;
                case OfficeType.Office2007:
                    result = new XSSFWorkbook(stream);
                    break;
                default:
                    result = null;
                    break;
            }
            return result;
        }

        public static OfficeType GetRealOfficeType(this System.IO.FileStream fs)
        {
            string text = FileHelper.CheckExcelFileFormat(fs);
            OfficeType result;
            if (text != null)
            {
                if (text == "208207")
                {
                    result = OfficeType.Office2003;
                    return result;
                }
                if (text == "8075")
                {
                    result = OfficeType.Office2007;
                    return result;
                }
            }
            result = OfficeType.OfficeNull;
            return result;
        }

        public static ISheet GetISheet(this IWorkbook workbook, string sheetName, int? sheetIndex = null)
        {
            ISheet sheet;
            if (!string.IsNullOrEmpty(sheetName))
            {
                sheet = workbook.GetSheet(sheetName);
                if (sheet == null)
                {
                    sheet = workbook.GetSheetAt(0);
                }
            }
            else if (sheetIndex.HasValue)
            {
                sheet = workbook.GetSheetAt(sheetIndex.Value);
            }
            else
            {
                sheet = workbook.GetSheetAt(0);
            }
            return sheet;
        }

        private static DataTable SheetToDataTable(ISheet sheet, bool isFirstRowTitle = true)
        {
            DataTable dataTable = new DataTable();
            IRow row = sheet.GetRow(0);
            for (int i = (int)row.FirstCellNum; i < (int)row.LastCellNum; i++)
            {
                if (!isFirstRowTitle)
                {
                    dataTable.Columns.Add(new DataColumn());
                }
                else
                {
                    dataTable.Columns.Add(new DataColumn(row.GetCell(i).GetCellValue("[\n\t\r\v\f\b]", true)));
                }
            }
            int j = (!isFirstRowTitle) ? sheet.FirstRowNum : (sheet.FirstRowNum + 1);
            while (j <= sheet.LastRowNum)
            {
                IRow row2 = sheet.GetRow(j++);
                if (row2 != null)
                {
                    dataTable.Rows.Add(row2.GetRowValue(row.LastCellNum - row.FirstCellNum, "[\n\t\r\v\f\b]", true, row.FirstCellNum));
                }
            }
            return dataTable;
        }

        public static string[] GetRowValue(this IRow row, int cellsCount = -1, string emptyCharPattern = "[\n\t\r\v\f\b]", bool isTrim = true, int titleRowFirstCellNum = 0)
        {
            string[] result;
            if (row == null)
            {
                result = null;
            }
            else
            {
                cellsCount = ((cellsCount == -1) ? ((int)(row.LastCellNum - row.FirstCellNum + 1)) : cellsCount);
                string[] array = new string[cellsCount];
                int num = titleRowFirstCellNum;
                for (int i = 0; i < cellsCount; i++)
                {
                    array[i] = row.GetCell(num).GetCellValue(emptyCharPattern, isTrim);
                    num++;
                }
                result = array;
            }
            return result;
        }

        public static string GetCellValue(this ICell cell, string emptyCharPattern = "[\n\t\r\v\f\b]", bool isTrim = true)
        {
            string result;
            if (cell == null)
            {
                result = null;
            }
            else
            {
                string text;
                if (cell.CellType == CellType.Formula)
                {
                    cell.SetCellType(CellType.Formula);
                    text = cell.StringCellValue;
                }
                else
                {
                    text = cell.ToString();
                }
                if (isTrim)
                {
                    text = text.Trim();
                }
                text = Regex.Replace(text, emptyCharPattern, "");
                result = text;
            }
            return result;
        }

        public static void DeleteFile(string filepath)
        {
            if (System.IO.File.Exists(filepath))
            {
                System.IO.File.Delete(filepath);
            }
        }

        public enum OfficeType
        {
            [Description("OfficeNull")]
            OfficeNull,
            [Description("Office2003")]
            Office2003,
            [Description("Office2007")]
            Office2007
        }
    }
}
