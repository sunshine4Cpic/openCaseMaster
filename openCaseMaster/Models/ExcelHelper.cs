using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace openCaseMaster.Models
{
    public class ExcelHelper
    {
        /// <summary>
        /// 创建EXCEL
        /// </summary>
        /// <returns></returns>
        public static HSSFWorkbook InitializeWorkbook()
        {
            HSSFWorkbook hssfworkbook = new HSSFWorkbook();

            //Create a entry of DocumentSummaryInformation
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "神州数码";
            //dsi.Manager = "管理者";
            //dsi.Category = "类别";
            hssfworkbook.DocumentSummaryInformation = dsi;

            //Create a entry of SummaryInformation
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();

            si.Author = "孙建平";
            //si.Comments = "备注";
            //si.Subject = "主题";
            hssfworkbook.SummaryInformation = si;

            return hssfworkbook;
        }

        /// <summary>
        /// 创建案例sheet
        /// </summary>
        /// <param name="xls"></param>
        /// <param name="ParamList"></param>
        /// <param name="sheetName"></param>
        /// <param name="caseName"></param>
        public static void creatCaseSheet(HSSFWorkbook xls, Dictionary<string,string> ParamList, string sheetName, string caseName)
        {
            ISheet sheet = xls.CreateSheet(sheetName);

            IRow row = sheet.CreateRow(0);




            ICell cell1 = row.CreateCell(0);
            cell1.SetCellValue("案例名");



            foreach (var str in ParamList)
            {
                sheet.SetColumnWidth(row.LastCellNum - 1, 15 * 256); //宽度
                ICell cell = row.CreateCell(row.LastCellNum);
                cell.SetCellValue(str.Key);
            }
            sheet.SetColumnWidth(row.LastCellNum - 1, 15 * 256);

            IRow DemoRow = sheet.CreateRow(1);
            DemoRow.CreateCell(0).SetCellValue(caseName);

        }


        /// <summary>
        /// 编辑配置页
        /// </summary>
        /// <param name="Config">sheet</param>
        /// <param name="caseName">案例名</param>
        /// <param name="sheetName">案例的ID sheet名</param>
        public static void creatConfigRow(ISheet Config, string caseName, string sheetName, int pCount)
        {
            IRow row = Config.CreateRow(Config.LastRowNum + 1);
            ICell cell0 = row.CreateCell(0);
            cell0.SetCellValue(caseName);



            //创建一个超链接对象
            HSSFHyperlink link = new HSSFHyperlink(HyperlinkType.Document);
            // strTableName 这个参数为 sheet名字 A1 为单元格 其他是固定格式
            link.Address = "'" + sheetName + "'!A1";
            //设置 cellTableName 单元格 的连接对象
            cell0.Hyperlink = link;

            ICellStyle hlink_style = Config.Workbook.CreateCellStyle();

            IFont hlink_font = Config.Workbook.CreateFont();

            hlink_font.Underline = FontUnderlineType.Single;

            hlink_font.Color = HSSFColor.Blue.Index;

            hlink_style.SetFont(hlink_font);
            cell0.CellStyle = hlink_style;


            ICell cell1 = row.CreateCell(1);
            cell1.SetCellValue(sheetName);

            ICell cell2 = row.CreateCell(2);
            cell2.SetCellValue(pCount);

        }


        public static void creatScene(Stream stm, int id, string p)
        {
            
        }
    }
}