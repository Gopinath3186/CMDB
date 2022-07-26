using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace GCP_Attributes_EV.Models
{
    class GCP_ExportToExcel
    {
        public static void GCP_CMDB_EXCEL_Output(List<EV.Attr_List> ev_Attributes, List<EV.Char_List> ev_Characteristic, List<EV.Link_List> ev_Links)
        {
            ExportToExcel(ev_Attributes, "GCP_EV_Attribute_sheet");
            ExportToExcel(ev_Characteristic, "GCP_EV_Characteristics_sheet");
            ExportToExcel(ev_Links, "GCP_EV_Links_sheet");
        }
        public static void ExportToExcel(object virtual_Machine_CIs, string extractSheet)
        {
            string OutputFilePath = ConfigurationManager.AppSettings["OutputFilePath"];
            DataTable table = (DataTable)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(virtual_Machine_CIs), (typeof(DataTable)));

            if (extractSheet == "GCP_EV_Attribute_sheet")
            {
                table = AttributesXcelDisplayName(table);
            }
            else if (extractSheet == "GCP_EV_Characteristics_sheet")
            {
                table = GenerateTransposedTable(table);
            }
            else
            {
                table = LinksXcelDisplayName(table);
            }
            using (SpreadsheetDocument document = SpreadsheetDocument.Create(OutputFilePath + extractSheet + ".xlsx", SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                var sheetData = new SheetData();
                worksheetPart.Worksheet = new Worksheet(sheetData);

                Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Sheet1" };

                sheets.Append(sheet);

                Row headerRow = new Row();

                List<String> columns = new List<string>();
                foreach (System.Data.DataColumn column in table.Columns)
                {
                    columns.Add(column.ColumnName);

                    Cell cell = new Cell();
                    cell.DataType = CellValues.String;
                    cell.CellValue = new CellValue(column.ColumnName);
                    headerRow.AppendChild(cell);
                }
                sheetData.AppendChild(headerRow);

                foreach (DataRow dsrow in table.Rows)
                {
                    Row newRow = new Row();
                    foreach (String col in columns)
                    {
                        Cell cell = new Cell();
                        cell.DataType = CellValues.String;
                        cell.CellValue = new CellValue(dsrow[col].ToString());
                        newRow.AppendChild(cell);
                    }
                    sheetData.AppendChild(newRow);
                }
                workbookPart.Workbook.Save();
            }
        }
        private static DataTable GenerateTransposedTable(DataTable inputTable)
        {
            inputTable = CharacteristicXcelDisplayName(inputTable);
            DataTable outputTable = new DataTable();
            outputTable.Columns.Add("Serial Number");
            outputTable.Columns.Add("Characteristic Label");
            outputTable.Columns.Add("Characteristic Value");

            for (int cCount = 0; cCount <= inputTable.Rows.Count - 1; cCount++)
            {
                for (int rCount = 1; rCount <= inputTable.Columns.Count - 1; rCount++)
                {
                    DataRow newRow = outputTable.NewRow();
                    newRow["Serial Number"] = inputTable.Rows[cCount][0].ToString();
                    if (!String.IsNullOrEmpty(inputTable.Rows[cCount][rCount].ToString()))
                    {
                        newRow["Characteristic Label"] = inputTable.Columns[rCount].ToString();
                        newRow["Characteristic Value"] = inputTable.Rows[cCount][rCount].ToString();
                        outputTable.Rows.Add(newRow);
                    }

                }
            }

            return outputTable;
        }

        private static DataTable AttributesXcelDisplayName(DataTable table)
        {
            table.Columns[0].ColumnName = "FQDN";
            table.Columns[1].ColumnName = "Operation System";
            table.Columns[2].ColumnName = "Primary IP";
            table.Columns[3].ColumnName = "Mac Adress";
            table.Columns[4].ColumnName = "Hostname";
            table.Columns[5].ColumnName = "CSP";
            table.Columns[6].ColumnName = "Region";
            table.Columns[7].ColumnName = "Resource Group";
            table.Columns[8].ColumnName = "User type";
            table.Columns[9].ColumnName = "User location";
            table.Columns[10].ColumnName = "Range of users";
            table.Columns[11].ColumnName = "Date of creation";
            table.Columns[12].ColumnName = "Last Discovery made";
            table.Columns[13].ColumnName = "Last updated by";
            table.Columns[14].ColumnName = "Last updated on";
            table.Columns[15].ColumnName = "Source of the data";
            table.Columns[16].ColumnName = "Criticality level";
            table.Columns[17].ColumnName = "Availability SLA";
            table.Columns[18].ColumnName = "SLA";
            table.Columns[19].ColumnName = "Availability";
            table.Columns[20].ColumnName = "Maintenance range";
            table.Columns[21].ColumnName = "Confidentiality";
            table.Columns[22].ColumnName = "Integrity";
            table.Columns[23].ColumnName = "RTO";
            table.Columns[24].ColumnName = "RPO";
            table.Columns[25].ColumnName = "Easyvista name";
            table.Columns[26].ColumnName = "Name";
            table.Columns[27].ColumnName = "Current status";
            table.Columns[28].ColumnName = "Future Status";
            table.Columns[29].ColumnName = "CI state";
            table.Columns[30].ColumnName = "Current version";
            table.Columns[31].ColumnName = "Serial number or CI code";
            table.Columns[32].ColumnName = "Description";
            table.Columns[33].ColumnName = "Model";
            table.Columns[34].ColumnName = "Brand";
            table.Columns[35].ColumnName = "Category";
            table.Columns[36].ColumnName = "Product";
            table.Columns[37].ColumnName = "Environment";
            table.Columns[38].ColumnName = "Is DML";
            table.Columns[39].ColumnName = "Default incident catalog entry";
            table.Columns[40].ColumnName = "Default service catalog entry";
            table.Columns[41].ColumnName = "Default change catalog entry";
            table.Columns[42].ColumnName = "IT owner";
            table.Columns[43].ColumnName = "Business owner";
            table.Columns[44].ColumnName = "Approver Group";
            table.Columns[45].ColumnName = "Organization";
            table.Columns[46].ColumnName = "CI on call";
            table.Columns[47].ColumnName = "Shutdown strategy";
            table.Columns[48].ColumnName = "Backup strategy";
            table.Columns[49].ColumnName = "Is backuped";
            table.Columns[50].ColumnName = "Availability area";
            table.Columns[51].ColumnName = "Tags";
            table.Columns[52].ColumnName = "Patch";
            table.Columns[53].ColumnName = "Patching strategy";
            table.Columns[54].ColumnName = "Buildby";
            table.Columns[55].ColumnName = "Patching range";
            return table;
        }

        private static DataTable CharacteristicXcelDisplayName(DataTable table)
        {
            table.Columns[0].ColumnName = "Serial Number";
            table.Columns[1].ColumnName = "Disk Space";
            table.Columns[2].ColumnName = "Primary IP";
            table.Columns[3].ColumnName = "Disk type";
            table.Columns[4].ColumnName = "Network card name";
            table.Columns[5].ColumnName = "NIC";
            table.Columns[6].ColumnName = "OS version";
            table.Columns[7].ColumnName = "Node Size";
            table.Columns[8].ColumnName = "Number of disks";
            table.Columns[9].ColumnName = "private IP";
            table.Columns[10].ColumnName = "CPU Count";
            table.Columns[11].ColumnName = "RAM";
            table.Columns[12].ColumnName = "Size";
            table.Columns[13].ColumnName = "Project Id";
            table.Columns[14].ColumnName = "Project";
            table.Columns[15].ColumnName = "Parents folders";
            table.Columns[16].ColumnName = "Domain";
            table.Columns[17].ColumnName = "OS Name";
            table.Columns[18].ColumnName = "Hosting Location";
            table.Columns[19].ColumnName = "Storage Type";
            table.Columns[20].ColumnName = "Record Type";
            table.Columns[21].ColumnName = "Dynamic DNS";
            table.Columns[22].ColumnName = "TTL";
            table.Columns[23].ColumnName = "Folder name";
            table.Columns[24].ColumnName = "Folder ID";
            table.Columns[25].ColumnName = "Begins on";
            table.Columns[26].ColumnName = "Expires on";
            table.Columns[27].ColumnName = "URL";
            table.Columns[28].ColumnName = "dockerRegistry";
            table.Columns[29].ColumnName = "Certificate Type";
            table.Columns[30].ColumnName = "HttpsOnly";
            table.Columns[31].ColumnName = "Load Balancer Name";
            table.Columns[32].ColumnName = "LB Pool Name";
            table.Columns[33].ColumnName = "Type";
            table.Columns[34].ColumnName = "Virtual Server Name";
            table.Columns[35].ColumnName = "Virtual Server IP";
            table.Columns[36].ColumnName = "Virtual Server Port";
            table.Columns[37].ColumnName = "Virtual Network";
            table.Columns[38].ColumnName = "Database name";
            table.Columns[39].ColumnName = "Database Version";
            table.Columns[40].ColumnName = "SSL Serial number";
            table.Columns[41].ColumnName = "dataset name";
            table.Columns[42].ColumnName = "Tier";
            table.Columns[43].ColumnName = "Price Tier";
            table.Columns[44].ColumnName = "Time Zone";
            table.Columns[45].ColumnName = "Description";
            table.Columns[46].ColumnName = "Node pools";
            table.Columns[47].ColumnName = "Nodes";
            table.Columns[48].ColumnName = "Kubernetes version";
            table.Columns[49].ColumnName = "App Service Plan";
            return table;
        }
        private static DataTable LinksXcelDisplayName(DataTable table)
        {
            table.Columns[0].ColumnName = "Impactful CI";
            table.Columns[1].ColumnName = "Impacted CI";
            table.Columns[2].ColumnName = "Link label";
            table.Columns[3].ColumnName = "Blocking Link";
            table.Columns[4].ColumnName = "Blocking Link value";
            return table;
        }
    }
}
