using Google.Apis.Compute.v1;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Google.Apis.Dns.v1;
using GCP_Attributes_EV.Models;
using System.Configuration;

namespace GCP_Attributes_EV.GCP_Services
{
    class GCP_Asset_List3_4CIs
    {
        private readonly static string region_project = ConfigurationManager.AppSettings["Project_Region"];
        private readonly static string Brand = ConfigurationManager.AppSettings["Brand"];
        private readonly static string PROD = ConfigurationManager.AppSettings["PROD"];
        private readonly static string NO_PROD = ConfigurationManager.AppSettings["NO_PROD"];
        private readonly static string Link_label = ConfigurationManager.AppSettings["Link_label"];
        private readonly static string Blocking_Link = ConfigurationManager.AppSettings["Blocking_Link"];
        private readonly static string Blocking_Link_value = ConfigurationManager.AppSettings["Blocking_Link_value"];
        private readonly static string NSG = ConfigurationManager.AppSettings["NSG"];
        private readonly static string WEBAPP = ConfigurationManager.AppSettings["WEBAPP"];
        private readonly static string DATAWAREHOUSE = ConfigurationManager.AppSettings["DATAWAREHOUSE"];
        private readonly static string STORAGE = ConfigurationManager.AppSettings["STORAGE"];

        public static void CMDB_EV_part3(ref List<EV.Attr_List> ev_Attributes, ref List<EV.Char_List> ev_Characteristic
            , ref List<EV.Link_List> ev_Links, ref List<EV.Regions> locationList)
        {

            var credential = GCP_GoogleCredentials.GetCredential();
            ComputeService computeService = new ComputeService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential
            });

            DnsService dnsService = new DnsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential
            });

            //   Get List of Projects
            var projectLists = GCP_Service_List.GetProjectList(credential);

            int projCount = 0;

            List<string> RootFolder = new List<string>();
            RootFolder.Add(NO_PROD);
            RootFolder.Add(PROD);

            for (int j = 0; j < RootFolder.Count; j++)
            {
                string location = string.Empty;
                projCount = projCount + 1;

                // NSG-firewall 
                var project_Firewall = GCP_Service_List.GetInventoryList(credential, RootFolder[j], "Project_Firewall");
                if (project_Firewall.assets != null)
                {
                    var firewallCount = project_Firewall.assets.Where(x => x.assetType == "compute.googleapis.com/Firewall").ToList();
                    try
                    {
                        if (firewallCount.Count > 0)
                        {
                            foreach (var firewall in firewallCount)
                            {
                                string projectNumber = firewall.ancestors[0]?.Replace("projects/", "").ToString();
                                string projectId = projectLists.projects.Where(x => x.projectNumber == projectNumber).ToList()?.FirstOrDefault()?.projectId;
                                if (projectId != null)
                                {
                                    location = firewall.resource.location;

                                    EV.Attr_List evAttrList = new EV.Attr_List();
                                    EV.Char_List evCharList = new EV.Char_List();
                                    evAttrList.Name = firewall.name.ToString().Split('/').Where(y => !string.IsNullOrWhiteSpace(y)).LastOrDefault();
                                    evAttrList.Serial_number_or_CI_code = firewall.name;
                                    evAttrList.Category = NSG;
                                    evAttrList.Model = evAttrList.Category;
                                    evAttrList.Brand = Brand;
                                    evAttrList.Easyvista_name = evAttrList.Name.Substring(0, Math.Min(64, evAttrList.Name.Length));
                                    ev_Attributes.Add(evAttrList);

                                    evCharList.Project = projectId;
                                    evCharList.ProjectId = projectNumber;
                                    evCharList.Serial_Number = evAttrList.Serial_number_or_CI_code;
                                    ev_Characteristic.Add(evCharList);

                                    EV.Link_List evLinkList = new EV.Link_List();
                                    evLinkList.Impactful_CI = projectNumber;
                                    evLinkList.Impacted_CI = evAttrList.Serial_number_or_CI_code;
                                    evLinkList.Link_label = Link_label;
                                    evLinkList.Blocking_Link = Blocking_Link;
                                    evLinkList.Blocking_Link_value = Blocking_Link_value;
                                    if ((evLinkList.Impacted_CI != null) && (evLinkList.Impactful_CI != null))
                                    {
                                        ev_Links.Add(evLinkList);
                                    }
                                }
                            }
                        }
                    }
                    catch (Google.GoogleApiException ex1)
                    {
                        Console.WriteLine("ERROR: firewall  API has not been used in project ");
                    }
                }

                // app engine 
                var project_Webapp = GCP_Service_List.GetInventoryList(credential, RootFolder[j], "Project_WebApp");
                if (project_Webapp.assets != null)
                {
                    var appEngineCount = project_Webapp.assets.Where(x => x.assetType == "appengine.googleapis.com/Application").ToList();
                    var appEngineServiceCount = project_Webapp.assets.Where(x => x.assetType == "appengine.googleapis.com/Service").ToList();
                    var appEngineVersionCount = project_Webapp.assets.Where(x => x.assetType == "appengine.googleapis.com/Version").ToList();

                    try
                    {
                        if (appEngineVersionCount.Count > 0)
                        {
                            foreach (var servicesVerList in appEngineVersionCount)
                            {
                                string projectNumber = servicesVerList.ancestors[0]?.Replace("projects/", "").ToString();
                                string projectId = projectLists.projects.Where(x => x.projectNumber == projectNumber).ToList()?.FirstOrDefault()?.projectId;
                                if (projectId != null)
                                {
                                    location = appEngineCount[0].resource.location;
                                    EV.Attr_List evAttrList = new EV.Attr_List();
                                    EV.Char_List evCharList = new EV.Char_List();
                                    EV.Link_List evLinkList = new EV.Link_List();

                                    evAttrList.Name = servicesVerList.name.ToString().Split('/').Where(y => !string.IsNullOrWhiteSpace(y)).LastOrDefault();
                                    evAttrList.Serial_number_or_CI_code = servicesVerList.name;
                                    evAttrList.Category = WEBAPP;
                                    evAttrList.Model = evAttrList.Category;
                                    evAttrList.Brand = Brand;
                                    evAttrList.Easyvista_name = evAttrList.Name.Substring(0, Math.Min(64, evAttrList.Name.Length));
                                    ev_Attributes.Add(evAttrList);

                                    evCharList.Project = projectId;
                                    evCharList.ProjectId = projectNumber;
                                    evCharList.Serial_Number = evAttrList.Serial_number_or_CI_code;
                                    evCharList.HostingLocation = location;
                                    evCharList.App_Service_Plan = servicesVerList.resource.data.instanceClass;
                                    ev_Characteristic.Add(evCharList);


                                    evLinkList.Impactful_CI = projectNumber;
                                    evLinkList.Impacted_CI = evAttrList.Serial_number_or_CI_code;
                                    evLinkList.Link_label = Link_label;
                                    evLinkList.Blocking_Link = Blocking_Link;
                                    evLinkList.Blocking_Link_value = Blocking_Link_value;
                                    if ((evLinkList.Impacted_CI != null) && (evLinkList.Impactful_CI != null))
                                    {
                                        ev_Links.Add(evLinkList);
                                    }

                                    EV.Link_List evLinkList_region = new EV.Link_List();

                                    evLinkList_region.Impactful_CI = evAttrList.Serial_number_or_CI_code;
                                    evLinkList_region.Impacted_CI = locationList.Where(x => x.Name.Contains(location)).Select(z => z.Id).FirstOrDefault();
                                    evLinkList_region.Link_label = Link_label;
                                    evLinkList_region.Blocking_Link = Blocking_Link;
                                    evLinkList_region.Blocking_Link_value = Blocking_Link_value;
                                    if ((evLinkList_region.Impacted_CI != null) && (evLinkList_region.Impactful_CI != null))
                                    {
                                        ev_Links.Add(evLinkList_region);

                                    }
                                }
                            }
                        }
                    }
                    catch (Google.GoogleApiException ex1)
                    {
                        Console.WriteLine("ERROR: AppEngine has not been used in project ");
                    }
                }

                // DataWarehouse(BigQuery) 
                var project_DW = GCP_Service_List.GetInventoryList(credential, RootFolder[j], "Project_Datawarehouse");
                if (project_DW.assets != null)
                {
                    var bigQueryCount = project_DW.assets.Where(x => x.assetType == "bigquery.googleapis.com/Dataset").ToList();
                    try
                    {
                        if (bigQueryCount.Count > 0)
                        {
                            foreach (var datasets in bigQueryCount)
                            {
                                string projectNumber = datasets.resource.parent.ToString().Split('/').Where(y => !string.IsNullOrWhiteSpace(y)).LastOrDefault();
                                string projectId = projectLists.projects.Where(x => x.projectNumber == projectNumber).ToList()?.FirstOrDefault()?.projectId;
                                if (projectId != null)
                                {
                                    location = datasets.resource.location;
                                    EV.Attr_List evAttrList = new EV.Attr_List();
                                    EV.Char_List evCharList = new EV.Char_List();
                                    evAttrList.Serial_number_or_CI_code = datasets.name;
                                    evAttrList.Name = datasets?.resource.data.datasetReference?.datasetId;
                                    evAttrList.Category = DATAWAREHOUSE;
                                    evAttrList.Model = evAttrList.Category;
                                    evAttrList.Brand = Brand;
                                    evAttrList.Easyvista_name = evAttrList.Name.Substring(0, Math.Min(64, evAttrList.Name.Length));
                                    ev_Attributes.Add(evAttrList);

                                    evCharList.Project = projectId;
                                    evCharList.ProjectId = projectNumber;
                                    evCharList.Serial_Number = evAttrList.Serial_number_or_CI_code;
                                    evCharList.Database_name = datasets?.resource.data.datasetReference?.datasetId;
                                    evCharList.HostingLocation = location;
                                    ev_Characteristic.Add(evCharList);

                                    EV.Link_List evLinkList = new EV.Link_List();
                                    evLinkList.Impactful_CI = projectNumber;
                                    evLinkList.Impacted_CI = evAttrList.Serial_number_or_CI_code;
                                    evLinkList.Link_label = Link_label;
                                    evLinkList.Blocking_Link = Blocking_Link;
                                    evLinkList.Blocking_Link_value = Blocking_Link_value;
                                    if ((evLinkList.Impacted_CI != null) && (evLinkList.Impactful_CI != null))
                                    {
                                        ev_Links.Add(evLinkList);
                                    }

                                    EV.Link_List evLinkList_region = new EV.Link_List();

                                    evLinkList_region.Impactful_CI = evAttrList.Serial_number_or_CI_code;
                                    evLinkList_region.Impacted_CI = locationList.Where(x => x.Name == location).Select(z => z.Id).FirstOrDefault();
                                    evLinkList_region.Link_label = Link_label;
                                    evLinkList_region.Blocking_Link = Blocking_Link;
                                    evLinkList_region.Blocking_Link_value = Blocking_Link_value;
                                    if ((evLinkList_region.Impacted_CI != null) && (evLinkList_region.Impactful_CI != null))
                                    {
                                        ev_Links.Add(evLinkList_region);
                                    }
                                }
                            }
                        }
                    }
                    catch (Google.GoogleApiException ex1)
                    {
                        Console.WriteLine("ERROR: DataWarehouse  API has not been used in project ");
                    }

                }

                // Storage 
                var project_Storage = GCP_Service_List.GetInventoryList(credential, RootFolder[j], "Project_Storage");
                if (project_Storage.assets != null)
                {
                    var storage = project_Storage.assets.Where(x => x.assetType == "storage.googleapis.com/Bucket").ToList();
                    try
                    {
                        if (storage.Count > 0)
                        {
                            foreach (var storageType in storage)
                            {
                                string projectNumber = storageType.resource.parent.ToString().Split('/').Where(y => !string.IsNullOrWhiteSpace(y)).LastOrDefault();
                                string projectId = projectLists.projects.Where(x => x.projectNumber == projectNumber).ToList()?.FirstOrDefault()?.projectId;
                                if (projectId != null)
                                {
                                    location = storageType.resource.location;
                                    EV.Attr_List evAttrList = new EV.Attr_List();
                                    EV.Char_List evCharList = new EV.Char_List();
                                    evAttrList.Name = storageType.name?.ToString().Split('/').Where(y => !string.IsNullOrWhiteSpace(y)).LastOrDefault();
                                    evAttrList.Serial_number_or_CI_code = storageType.name;
                                    evAttrList.Category = STORAGE;
                                    evAttrList.Brand = Brand;
                                    evAttrList.Easyvista_name = evAttrList.Name.Substring(0, Math.Min(64, evAttrList.Name.Length));
                                    evAttrList.Model = evAttrList.Category;
                                    ev_Attributes.Add(evAttrList);

                                    evCharList.Project = projectId;
                                    evCharList.ProjectId = projectNumber;
                                    evCharList.Serial_Number = evAttrList.Serial_number_or_CI_code;
                                    evCharList.HostingLocation = location;
                                    evCharList.Storage_Type = storageType.resource.data.StorageClass;
                                    ev_Characteristic.Add(evCharList);

                                    EV.Link_List evLinkList = new EV.Link_List();
                                    evLinkList.Impactful_CI = projectNumber;
                                    evLinkList.Impacted_CI = evAttrList.Serial_number_or_CI_code;
                                    evLinkList.Link_label = Link_label;
                                    evLinkList.Blocking_Link = Blocking_Link;
                                    evLinkList.Blocking_Link_value = Blocking_Link_value;
                                    if ((evLinkList.Impacted_CI != null) && (evLinkList.Impactful_CI != null))
                                    {
                                        ev_Links.Add(evLinkList);
                                    }

                                    EV.Link_List evLinkList_region = new EV.Link_List();

                                    evLinkList_region.Impactful_CI = evAttrList.Serial_number_or_CI_code;
                                    evLinkList_region.Impacted_CI = locationList.Where(x => x.Name == location).Select(z => z.Id).FirstOrDefault();
                                    evLinkList_region.Link_label = Link_label;
                                    evLinkList_region.Blocking_Link = Blocking_Link;
                                    evLinkList_region.Blocking_Link_value = Blocking_Link_value;
                                    if ((evLinkList_region.Impacted_CI != null) && (evLinkList_region.Impactful_CI != null))
                                    {
                                        ev_Links.Add(evLinkList_region);
                                    }
                                }
                            }
                        }
                    }
                    catch (Google.GoogleApiException ex1)
                    {
                        Console.WriteLine("ERROR: storageService  has not been used in project");
                    }
                }
            }
        }
    }
}
