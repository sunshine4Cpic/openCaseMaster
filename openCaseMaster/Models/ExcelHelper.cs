using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;

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


        public static void creatScene(Stream MS, int DemandID, string name)
        {
            QCTESTEntities QC_DB = new QCTESTEntities();

            M_runScene mrs = new M_runScene();
            mrs.DemandID = DemandID;
            mrs.name = name;
            mrs.creatDate = DateTime.Now;
            QC_DB.M_runScene.Add(mrs);

            

            HSSFWorkbook hssfworkbook = new HSSFWorkbook(MS);


            HSSFFormulaEvaluator eva = new HSSFFormulaEvaluator(hssfworkbook);
            //eva.EvaluateInCell(cell);//取结果不取公式
            eva.EvaluateAll();//取结果不取公式

            ISheet sheet = hssfworkbook.GetSheetAt(0);

            System.Collections.IEnumerator rows = sheet.GetRowEnumerator();
            rows.MoveNext();



            while (rows.MoveNext())
            {
                IRow row = (IRow)rows.Current;
                string sheetName = row.GetCell(1).ToString();
                if (sheetName == null || sheetName.Trim() == "") break;

                ISheet caseSheet = hssfworkbook.GetSheet(sheetName);

                insertRunCase(QC_DB,caseSheet, mrs.ID);

            }

            QC_DB.SaveChanges();
        }

        /// <summary>
        /// 创建执行案例
        /// </summary>
        /// <param name="sheet">sheet页</param>
        /// <param name="sceneID">场景ID</param>
        private static void insertRunCase(QCTESTEntities QC_DB, ISheet sheet, int sceneID)
        {

            int ID = Convert.ToInt32(sheet.SheetName);

            M_testCase mtc = QC_DB.M_testCase.Where(t => t.ID == ID).FirstOrDefault();
            if (mtc == null) return;

            System.Collections.IEnumerator rows = sheet.GetRowEnumerator();
            //读取下一行 
            rows.MoveNext();//如果没有要处理下
            IRow headRow = (IRow)rows.Current;
          


            Dictionary<string, string> data = new Dictionary<string, string>();
            List<string> keys = new List<string>();

            //首先获得参数列表
            for (int i = 1; i < headRow.LastCellNum; i++)
            {
                ICell cell = headRow.GetCell(i);
                if (cell == null || cell.ToString() == "") break;//有空数据直接退出
                string key = cell.ToString();
                data.Add(key, "");
                keys.Add(key);
               
            }

            //逐行转化案例
            while (rows.MoveNext())
            {
                 
                IRow row = (IRow)rows.Current;
                if (row.GetCell(0) == null || row.GetCell(0).StringCellValue.Trim() == "") break;//案例名字没有退出

                string caseName = row.GetCell(0).StringCellValue;
                if (caseName.Length > 50)
                    caseName = caseName.Substring(0, 50);
                XElement cloneXML = XElement.Parse(mtc.testXML);
                cloneXML.SetAttributeValue("name", caseName);//name
              
                for (int i = 0; i < keys.Count; i++)
                {
                    ICell cell = row.GetCell(i+1);
                    string value = "";
                    if (cell != null)
                    {
                        //cell = eva.EvaluateInCell(cell);
                        cell.SetCellType(CellType.String);
                        value = cell.StringCellValue;
                    }
                    data[keys[i]] = value;
                }

                
                //获得最终案例
                cloneXML.getRunScript(data);

                //放入数据库

                M_runTestCase mrtc = new M_runTestCase();
                mrtc.sceneID = sceneID;
                mrtc.testXML = cloneXML.ToString();
                mrtc.name = caseName;
                QC_DB.M_runTestCase.Add(mrtc);
              
            }


        }
    }
}