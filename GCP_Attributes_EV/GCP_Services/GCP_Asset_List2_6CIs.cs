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
    class GCP_Asset_List2_6CIs
    {
        private static readonly string region_project = ConfigurationManager.AppSettings["Project_Region"];
        private static readonly string Brand = ConfigurationManager.AppSettings["Brand"];
        private static readonly string PROD = ConfigurationManager.AppSettings["PROD"];
        private static readonly string NO_PROD = ConfigurationManager.AppSettings["NO_PROD"];
        private static readonly string Link_label = ConfigurationManager.AppSettings["Link_label"];
        private static readonly string Blocking_Link = ConfigurationManager.AppSettings["Blocking_Link"];
        private static readonly string Blocking_Link_value = ConfigurationManager.AppSettings["Blocking_Link_value"];
        private static readonly string PROJECT = ConfigurationManager.AppSettings["PROJECT"];
        private static readonly string DATABASE = ConfigurationManager.AppSettings["DATABASE"];
        private static readonly string VAULT = ConfigurationManager.AppSettings["VAULT"];
        private static readonly string CERTIFICAT = ConfigurationManager.AppSettings["CERTIFICAT"];
        private static readonly string LOADBALANCER = ConfigurationManager.AppSettings["LOADBALANCER"];
        private static readonly string DNS = ConfigurationManager.AppSettings["DNS"];
        private static readonly string ENDPOINT = ConfigurationManager.AppSettings["ENDPOINT"];
        private static readonly string REGION = ConfigurationManager.AppSettings["REGION"];
        public static void CMDB_EV_part2(ref List<EV.Attr_List> ev_Attributes, ref List<EV.Char_List> ev_Characteristic
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

            // Regions
            var regionList = computeService.Regions.List(region_project);
            var regionResponse = regionList.Execute();
            foreach (var regList in regionResponse.Items)
            {
                EV.Regions regList_fixed = new EV.Regions();
                regList_fixed.Name = regList.Name;
                regList_fixed.Id = regList.Id.ToString();
                locationList.Add(regList_fixed);

                EV.Attr_List evAttrList = new EV.Attr_List();
                evAttrList.Name = regList.Name;
                evAttrList.Serial_number_or_CI_code = regList.Id?.ToString();
                evAttrList.Category = REGION;
                evAttrList.Model = evAttrList.Category;
                evAttrList.Brand = Brand;
                evAttrList.Easyvista_name = evAttrList.Name.Substring(0, Math.Min(64, evAttrList.Name.Length));
                ev_Attributes.Add(evAttrList);
            }


            //  Get List of Projects
            var projectLists = GCP_Service_List.GetProjectList(credential);

            GetProjectList(projectLists, ref ev_Attributes, ref ev_Characteristic, ref ev_Links);

            List<string> RootFolder = new List<string>();
            RootFolder.Add(NO_PROD);
            RootFolder.Add(PROD);

            for (int j = 0; j < RootFolder.Count; j++)
            {
                string location = string.Empty;
                var projectInventory = GCP_Service_List.GetInventoryList(credential, RootFolder[j], "second_ListCIs");
                if (projectInventory.assets != null)
                {
                    // SqlAdmin(DB) 
                    var sqlAdmin = projectInventory.assets.Where(x => x.assetType == "sqladmin.googleapis.com/Instance").ToList();
                    // Secrets Key vault 
                    var secretVault = projectInventory.assets.Where(x => x.assetType == "secretmanager.googleapis.com/Secret").ToList();
                    // certificat  
                    var certCount = projectInventory.assets.Where(x => x.assetType == "compute.googleapis.com/SslCertificate").ToList();
                    // Load balancer 
                    var Loadbalancer = projectInventory.assets.Where(x => x.assetType == "compute.googleapis.com/BackendService").ToList();
                    // DNS  
                    var dnsnCount = projectInventory.assets.Where(x => x.assetType == "dns.googleapis.com/ManagedZone").ToList();
                    // EndPoint  
                    var endPointCount = projectInventory.assets.Where(x => x.assetType == "compute.googleapis.com/NetworkEndpointGroup").ToList();


                    //  SqlAdmin(DB)
                    try
                    {
                        if (sqlAdmin.Count > 0)
                        {
                            foreach (var sqlDB in sqlAdmin)
                            {
                                string projectNumber = sqlDB.resource.parent.ToString().Split('/').Where(y => !string.IsNullOrWhiteSpace(y)).LastOrDefault();
                                string projectId = projectLists.projects.Where(x => x.projectNumber == projectNumber).ToList()?.FirstOrDefault()?.projectId;
                                if (projectId != null)
                                {
                                    location = sqlDB.resource.location;
                                    EV.Attr_List evAttrList = new EV.Attr_List();
                                    EV.Char_List evCharList = new EV.Char_List();

                                    evAttrList.Name = sqlDB.name.ToString().Split('/').Where(y => !string.IsNullOrWhiteSpace(y)).LastOrDefault();
                                    evAttrList.Serial_number_or_CI_code = sqlDB.name;
                                    evAttrList.Category = DATABASE;
                                    evAttrList.Model = evAttrList.Category;
                                    evAttrList.Brand = Brand;
                                    evAttrList.Easyvista_name = evAttrList.Name.Substring(0, Math.Min(64, evAttrList.Name.Length));
                                    ev_Attributes.Add(evAttrList);

                                    evCharList.Project = projectId;
                                    evCharList.ProjectId = projectNumber;
                                    evCharList.Serial_Number = evAttrList.Serial_number_or_CI_code;
                                    evCharList.Database_name = evAttrList.Name;
                                    evCharList.Tier = sqlDB.resource?.data?.settings?.Tier;
                                    evCharList.Database_Version = sqlDB.resource.data.DatabaseInstalledVersion;
                                    evCharList.Time_Zone = sqlDB.resource.data.gceZone;
                                    evCharList.Price_Tier = sqlDB.resource?.data?.settings?.PricingPlan;
                                    evCharList.HostingLocation = location;
                                    evCharList.SSL_Serial_number = sqlDB.resource.data.serverCaCert?.CertSerialNumber;
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
                        Console.WriteLine("ERROR: SqlAdmin(DB)  API has not been used in project ");
                    }

                    // Secrets Key vault
                    try
                    {
                        if (secretVault?.Count > 0)
                        {
                            foreach (var secretResp in secretVault)
                            {
                                string projectNumber = secretResp.resource.parent.ToString().Split('/').Where(y => !string.IsNullOrWhiteSpace(y)).LastOrDefault();
                                string projectId = projectLists.projects.Where(x => x.projectNumber == projectNumber).ToList()?.FirstOrDefault()?.projectId;
                                if (projectId != null)
                                {
                                    EV.Attr_List evAttrList = new EV.Attr_List();
                                    EV.Char_List evCharList = new EV.Char_List();
                                    evAttrList.Name = secretResp.name?.ToString().Split('/').Where(y => !string.IsNullOrWhiteSpace(y)).LastOrDefault();
                                    evAttrList.Serial_number_or_CI_code = secretResp.name;
                                    evAttrList.Category = VAULT;
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
                        Console.WriteLine("ERROR: Secrets Key vault has not been used in project");
                    }

                    // Certificat Manager
                    try
                    {
                        if (certCount.Count > 0)
                        {
                            foreach (var cert in certCount)
                            {
                                string projectNumber = cert.resource.parent.ToString().Split('/').Where(y => !string.IsNullOrWhiteSpace(y)).LastOrDefault();
                                string projectId = projectLists.projects.Where(x => x.projectNumber == projectNumber).ToList()?.FirstOrDefault()?.projectId;
                                if (projectId != null)
                                {
                                    location = cert.resource.location;
                                    EV.Attr_List evAttrList = new EV.Attr_List();
                                    EV.Char_List evCharList = new EV.Char_List();
                                    EV.Link_List evLinkList = new EV.Link_List();

                                    evAttrList.Name = cert.name?.ToString().Split('/').Where(y => !string.IsNullOrWhiteSpace(y)).LastOrDefault();
                                    evAttrList.Serial_number_or_CI_code = cert.name;
                                    evAttrList.Category = CERTIFICAT;
                                    evAttrList.Model = evAttrList.Category;
                                    evAttrList.Brand = Brand;
                                    evAttrList.Easyvista_name = evAttrList.Name.Substring(0, Math.Min(64, evAttrList.Name.Length));
                                    ev_Attributes.Add(evAttrList);
                                    evCharList.HostingLocation = location;
                                    evCharList.Project = projectId;
                                    evCharList.ProjectId = projectNumber;
                                    evCharList.Serial_Number = evAttrList.Serial_number_or_CI_code;
                                    evCharList.Begins_on = cert.resource.data.CreationTimestamp;
                                    evCharList.Expires_on = cert.resource.data.ExpireTime?.ToString();
                                    evCharList.Certificate_Type = cert.resource.data.Type;

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
                        Console.WriteLine("ERROR: certificateManagerService  API has not been used in project");
                    }

                    // Load balancer
                    try
                    {
                        if (Loadbalancer.Count > 0)
                        {
                            foreach (var backendService in Loadbalancer)
                            {
                                string projectNumber = backendService.resource.parent.ToString().Split('/').Where(y => !string.IsNullOrWhiteSpace(y)).LastOrDefault();
                                string projectId = projectLists.projects.Where(x => x.projectNumber == projectNumber).ToList()?.FirstOrDefault()?.projectId;
                                if (projectId != null)
                                {
                                    location = backendService.resource.location;
                                    EV.Attr_List evAttrList = new EV.Attr_List();
                                    EV.Char_List evCharList = new EV.Char_List();
                                    EV.Link_List evLinkList = new EV.Link_List();

                                    evAttrList.Name = backendService.name?.ToString().Split('/').Where(y => !string.IsNullOrWhiteSpace(y)).LastOrDefault();
                                    evAttrList.Serial_number_or_CI_code = backendService.name;
                                    evAttrList.Category = LOADBALANCER;
                                    evAttrList.Model = evAttrList.Category;
                                    evAttrList.Brand = Brand;
                                    evAttrList.Easyvista_name = evAttrList.Name.Substring(0, Math.Min(64, evAttrList.Name.Length));
                                    ev_Attributes.Add(evAttrList);

                                    evCharList.Project = projectId;
                                    evCharList.ProjectId = projectNumber;
                                    evCharList.Serial_Number = evAttrList.Serial_number_or_CI_code;
                                    evCharList.Load_Balancer_Name = evAttrList.Name;
                                    evCharList.LB_Pool_Name = backendService.resource.data.PortName;
                                    evCharList.Virtual_Server_Port = backendService.resource.data.Port;
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
                                    evLinkList_region.Impacted_CI = locationList.Where(x => x.Name == location).Select(z => z.Id).FirstOrDefault();
                                    evLinkList_region.Link_label = Link_label;
                                    evLinkList_region.Blocking_Link = Blocking_Link;
                                    evLinkList_region.Blocking_Link_value = Blocking_Link_value;
                                    if ((evLinkList_region.Impacted_CI != null) && (evLinkList_region.Impactful_CI != null))
                                    {
                                        ev_Links.Add(evLinkList_region);
                                    }

                                    EV.Link_List evLinkList_LB = new EV.Link_List();
                                    evLinkList_LB.Impactful_CI = evAttrList.Serial_number_or_CI_code;
                                    evLinkList_LB.Impacted_CI = evAttrList.Name;
                                    evLinkList_LB.Link_label = Link_label;
                                    evLinkList_LB.Blocking_Link = Blocking_Link;
                                    evLinkList_LB.Blocking_Link_value = Blocking_Link_value;
                                    ev_Links.Add(evLinkList_LB);
                                }
                            }
                        }

                    }
                    catch (Google.GoogleApiException ex1)
                    {
                        Console.WriteLine("ERROR: LoadBalancer");
                    }

                    // /*DNS */
                    try
                    {
                        if (dnsnCount.Count > 0)
                        {
                            foreach (var managedZone in dnsnCount)
                            {
                                string projectNumber = managedZone.resource.parent.ToString().Split('/').Where(y => !string.IsNullOrWhiteSpace(y)).LastOrDefault();
                                string projectId = projectLists.projects.Where(x => x.projectNumber == projectNumber).ToList()?.FirstOrDefault()?.projectId;
                                if (projectId != null)
                                {
                                    var dnsRecordset = dnsService.ResourceRecordSets.List(projectNumber, managedZone.resource.data.name.ToString());
                                    var dnsRecordsetResult = dnsRecordset.Execute();
                                    EV.Attr_List evAttrList = new EV.Attr_List();

                                    evAttrList.Name = managedZone.resource.data.name.ToString();
                                    evAttrList.Serial_number_or_CI_code = managedZone.name;
                                    evAttrList.Category = DNS;
                                    evAttrList.Model = evAttrList.Category;
                                    evAttrList.Brand = Brand;
                                    evAttrList.Easyvista_name = evAttrList.Name.Substring(0, Math.Min(64, evAttrList.Name.Length));
                                    ev_Attributes.Add(evAttrList);

                                    if (dnsRecordsetResult.Rrsets.Count > 0)
                                    {
                                        foreach (var dnsRecSet in dnsRecordsetResult.Rrsets)
                                        {
                                            EV.Char_List evCharList = new EV.Char_List();

                                            evCharList.Project = projectId;
                                            evCharList.ProjectId = projectNumber;
                                            evCharList.Serial_Number = evAttrList.Serial_number_or_CI_code;
                                            evCharList.Record_Type = dnsRecSet.Type;
                                            evCharList.Dynamic_DNS = dnsRecSet.Name;
                                            evCharList.TTL = dnsRecSet.Ttl.ToString();
                                            ev_Characteristic.Add(evCharList);
                                        }
                                    }

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
                        Console.WriteLine("ERROR: dnsService  API has not been used in project ");
                    }

                    // EndPoint
                    try
                    {
                        if (endPointCount.Count > 0)
                        {
                            foreach (var endPoints in endPointCount)
                            {
                                string projectNumber = endPoints.resource.parent.ToString().Split('/').Where(y => !string.IsNullOrWhiteSpace(y)).LastOrDefault();
                                string projectId = projectLists.projects.Where(x => x.projectNumber == projectNumber).ToList()?.FirstOrDefault()?.projectId;
                                if (projectId != null)
                                {
                                    location = endPoints.resource?.location;
                                    EV.Attr_List evAttrList = new EV.Attr_List();
                                    EV.Char_List evCharList = new EV.Char_List();
                                    evAttrList.Name = endPoints.name?.ToString().Split('/').Where(y => !string.IsNullOrWhiteSpace(y)).LastOrDefault(); ;
                                    evAttrList.Serial_number_or_CI_code = endPoints.name;
                                    evAttrList.Category = ENDPOINT;
                                    evAttrList.Model = evAttrList.Category;
                                    evAttrList.Brand = Brand;
                                    evAttrList.Easyvista_name = evAttrList.Name.Substring(0, Math.Min(64, evAttrList.Name.Length));
                                    ev_Attributes.Add(evAttrList);

                                    evCharList.Project = projectId;
                                    evCharList.ProjectId = projectNumber;
                                    evCharList.Serial_Number = evAttrList.Serial_number_or_CI_code;
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
                        Console.WriteLine("ERROR: EndPoints  API has not been used in project");
                    }
                }
            }
        }
        private static void GetProjectList(GCP_Project_List.ProjectList projectLists, ref List<EV.Attr_List> ev_Attributes, ref List<EV.Char_List> ev_Characteristic, ref List<EV.Link_List> ev_Links)
        {
            try
            {
                foreach (var asset in projectLists.projects)
                {
                    string projectNumber = asset.projectNumber;
                    string projectId = asset.projectId;
                    EV.Attr_List evAttrList = new EV.Attr_List();
                    evAttrList.Name = projectId;
                    evAttrList.Serial_number_or_CI_code = projectNumber;
                    evAttrList.Category = PROJECT;
                    evAttrList.Model = evAttrList.Category;
                    evAttrList.Brand = Brand;
                    evAttrList.Easyvista_name = evAttrList.Name.Substring(0, Math.Min(64, evAttrList.Name.Length));
                    ev_Attributes.Add(evAttrList);

                    EV.Char_List evCharList = new EV.Char_List();

                    evCharList.Serial_Number = projectNumber;
                    evCharList.ProjectId = projectNumber;
                    evCharList.Project = projectId;
                    if (asset.parent?.type == "folder")
                    {
                        evCharList.Parents_folders = "folder/" + asset.parent?.id;
                    }
                    else
                    {
                        evCharList.Parents_folders = asset.parent?.id;
                    }
                    ev_Characteristic.Add(evCharList);

                    EV.Link_List evLinkList = new EV.Link_List();

                    evLinkList.Impactful_CI = evCharList.Parents_folders;
                    evLinkList.Impacted_CI = projectNumber;
                    evLinkList.Link_label = Link_label;
                    evLinkList.Blocking_Link = Blocking_Link;
                    evLinkList.Blocking_Link_value = Blocking_Link_value;
                    if ((evLinkList.Impacted_CI != null) && (evLinkList.Impactful_CI != null))
                    {
                        ev_Links.Add(evLinkList);
                    }
                }
            }
            catch (Google.GoogleApiException ex1)
            {
                Console.WriteLine("ERROR: Projects  API has not been used in project ");
            }
        }
    }
}
