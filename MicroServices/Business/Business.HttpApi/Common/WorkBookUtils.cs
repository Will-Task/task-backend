using System;
using System.Collections.Generic;
using Business.Enums;
using ClosedXML.Excel;

namespace Business.Common;

public class WorkBookUtils
{
    /// <summary>
    /// 設定特定欄位鎖起來不能編輯
    /// </summary>
    public static void SetCellLock(IXLWorksheet worksheet, string range)
    {
        if (!worksheet.IsProtected)
        {
            /// 全部鎖起來
            worksheet.Protect();
        }
        
        /// 解開要可以編輯的
        worksheet.Range(range).Style.Protection.SetLocked(false);
    }
    
    /// <summary>
    /// 設定excel下拉選單選項
    /// </summary>
    public static void SetCellDropDown(IXLWorksheet worksheet, List<string> options, char column)
    {
        var validOptions = $"\"{String.Join(",", options)}\"";
        worksheet.Range($"{column}{WorkBook.ExcelBeginLine}:{column}{WorkBook.ExcelEndLine}").SetDataValidation().List(validOptions, true);
    }
}