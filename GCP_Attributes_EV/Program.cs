using GCP_Attributes_EV.GCP_Services;
using System.Collections.Generic;
using GCP_Attributes_EV.Models;
using System;

namespace GCP_Services
{
    public class GCP_Services
    {
        private static List<EV.Attr_List> ev_Attr = new List<EV.Attr_List>();
        private static List<EV.Char_List> ev_Char = new List<EV.Char_List>();
        private static List<EV.Link_List> ev_Link = new List<EV.Link_List>();
        private static List<EV.Regions> regions = new List<EV.Regions>();

        public static void Main(string[] args)
        { 
            Console.WriteLine("Service Stared.." + DateTime.Now);

            EASY_VISTA_CMDB_GCP_Services();
            Console.WriteLine("Service Ended " + DateTime.Now);
            Console.WriteLine("“File extract is completed.please check the files inside the output folder“");
        }

        public static void EASY_VISTA_CMDB_GCP_Services()
        {
            GCP_Asset_List1_5CIs.CMDB_EV_part1(ev_Attr, ev_Char,  ev_Link, regions);
            GCP_Asset_List2_6CIs.CMDB_EV_part2(ref ev_Attr, ref ev_Char, ref ev_Link, ref regions);
            GCP_Asset_List3_4CIs.CMDB_EV_part3(ref ev_Attr, ref ev_Char, ref ev_Link, ref regions);

            // Excel output 
            GCP_ExportToExcel.GCP_CMDB_EXCEL_Output(ev_Attr, ev_Char, ev_Link);
        }


    }
}

