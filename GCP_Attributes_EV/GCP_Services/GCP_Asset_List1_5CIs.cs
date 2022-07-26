using GCP_Attributes_EV.Models;
using Google.Apis.Compute.v1;
using Google.Apis.OSConfig.v1;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Configuration;

namespace GCP_Attributes_EV.GCP_Services
{
    class GCP_Asset_List1_5CIs
    {
        private static readonly string region_project = ConfigurationManager.AppSettings["Project_Region"];
        private static readonly string Brand = ConfigurationManager.AppSettings["Brand"];
        private static readonly string PROD = ConfigurationManager.AppSettings["PROD"];
        private static readonly string NO_PROD = ConfigurationManager.AppSettings["NO_PROD"];
        private static readonly string Link_label = ConfigurationManager.AppSettings["Link_label"];
        private static readonly string Blocking_Link = ConfigurationManager.AppSettings["Blocking_Link"];
        private static readonly string Blocking_Link_value = ConfigurationManager.AppSettings["Blocking_Link_value"];
        private static readonly string REGION = ConfigurationManager.AppSettings["REGION"];
        private static readonly string FOLDER = ConfigurationManager.AppSettings["FOLDER"];
        private static readonly string SERVERLESS = ConfigurationManager.AppSettings["SERVERLESS"];
        private static readonly string CONTAINERREGISTERY = ConfigurationManager.AppSettings["CONTAINERREGISTERY"];
        private static readonly string VIRTUALMACHINE = ConfigurationManager.AppSettings["VIRTUALMACHINE"];
        private static readonly string KUBERNETESCLUSTER = ConfigurationManager.AppSettings["KUBERNETESCLUSTER"];

        public static void CMDB_EV_part1(List<EV.Attr_List> ev_Attributes, List<EV.Char_List> ev_Characteristic, List<EV.Link_List> ev_Links, List<EV.Regions> locationList)
        {
            var credential = GCP_GoogleCredentials.GetCredential();
            string location = string.Empty;

            ComputeService computeService = new ComputeService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "Google-ComputeService",
            });
            OSConfigService oSConfigService = new OSConfigService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "Google-OSConfigService",
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

            //Project List 
            var projectLists = GCP_Service_List.GetProjectList(credential);

            List<string> RootFolder = new List<string>();
            RootFolder.Add(PROD); // Prod
            RootFolder.Add(NO_PROD); // No-Prod

            for (int j = 0; j < RootFolder.Count; j++)
            {
                var folderLists_Rootlevel = GCP_Service_List.GetFoldersList(credential, "folders/" + RootFolder[j]);
                foreach (var folders in folderLists_Rootlevel.projects)
                {
                    EV.Attr_List evAttrList = new EV.Attr_List();
                    evAttrList.Name = folders.displayName;
                    evAttrList.Serial_number_or_CI_code = folders.name;
                    evAttrList.Category = FOLDER;
                    evAttrList.Model = evAttrList.Category;
                    evAttrList.Brand = Brand;
                    evAttrList.Easyvista_name = evAttrList.Name.Substring(0, Math.Min(64, evAttrList.Name.Length));
                    ev_Attributes.Add(evAttrList);

                    EV.Char_List evCharList = new EV.Char_List();
                    evCharList.Serial_Number = folders.name;
                    evCharList.Folder_name = folders.displayName;
                    evCharList.Folder_ID = folders.name;
                    ev_Characteristic.Add(evCharList);

                    var folderLists_secondLevel = GCP_Service_List.GetFoldersList(credential, folders.name);
                    if (folderLists_secondLevel.projects != null)
                    {
                        foreach (var folders_seclevel in folderLists_secondLevel.projects)
                        {
                            EV.Attr_List evAttrList_1 = new EV.Attr_List();
                            evAttrList_1.Name = folders_seclevel.displayName;
                            evAttrList_1.Serial_number_or_CI_code = folders_seclevel.name;
                            evAttrList_1.Category = FOLDER;
                            evAttrList_1.Model = evAttrList_1.Category;
                            evAttrList_1.Brand = Brand;
                            evAttrList_1.Easyvista_name = evAttrList_1.Name.Substring(0, Math.Min(64, evAttrList_1.Name.Length));
                            ev_Attributes.Add(evAttrList_1);

                            EV.Char_List evCharList_1 = new EV.Char_List();

                            evCharList_1.Serial_Number = folders_seclevel.name;
                            evCharList_1.Folder_name = folders_seclevel.displayName;
                            evCharList_1.Folder_ID = folders_seclevel.name;
                            ev_Characteristic.Add(evCharList_1);

                            var folderLists_thirdlevel = GCP_Service_List.GetFoldersList(credential, folders_seclevel.name);
                            if (folderLists_thirdlevel.projects != null)
                            {
                                foreach (var folders_thirdLevel in folderLists_thirdlevel.projects)
                                {
                                    EV.Attr_List evAttrList_2 = new EV.Attr_List();
                                    evAttrList_2.Name = folders_thirdLevel.displayName;
                                    evAttrList_2.Serial_number_or_CI_code = folders_thirdLevel.name;
                                    evAttrList_2.Category = FOLDER;
                                    evAttrList_2.Model = evAttrList_2.Category;
                                    evAttrList_2.Brand = Brand;
                                    evAttrList_2.Easyvista_name = evAttrList_2.Name.Substring(0, Math.Min(64, evAttrList_2.Name.Length));
                                    ev_Attributes.Add(evAttrList_2);

                                    EV.Char_List evCharList_2 = new EV.Char_List();

                                    evCharList_2.Serial_Number = folders_thirdLevel.name;
                                    evCharList_2.Folder_name = folders_thirdLevel.displayName;
                                    evCharList_2.Folder_ID = folders_thirdLevel.name;
                                    ev_Characteristic.Add(evCharList_2);
                                }
                            }
                        }
                    }

                }

                var inventoryListLocation = GCP_Service_List.GetInventoryList(credential, RootFolder[j], "first_ListCIs");
                if (inventoryListLocation.assets != null)
                {
                    // Functions  
                    var functionsCount = inventoryListLocation.assets.Where(x => x.assetType == "cloudfunctions.googleapis.com/CloudFunction").ToList();
                    // Container Registry  
                    var containerCount = inventoryListLocation.assets.Where(x => x.assetType == "artifactregistry.googleapis.com/Repository").ToList();
                    // Virtual Machine  
                    var VirtualmachineCount = inventoryListLocation.assets.Where(x => x.assetType == "compute.googleapis.com/Instance").ToList();
                    // Kubernetes
                    var kubernetesCount = inventoryListLocation.assets.Where(x => x.assetType == "container.googleapis.com/Cluster").ToList();

                    if (functionsCount.Count > 0)
                    {
                        foreach (var asset in functionsCount)
                        {
                            string projectNumber = asset.resource.parent.ToString().Split('/').Where(y => !string.IsNullOrWhiteSpace(y)).LastOrDefault();
                            string projectId = projectLists.projects.Where(x => x.projectNumber == projectNumber).ToList().FirstOrDefault().projectId;
                            location = asset.resource.location;
                            EV.Attr_List evAttrList = new EV.Attr_List();
                            EV.Char_List evCharList = new EV.Char_List();
                            EV.Link_List evLinkList = new EV.Link_List();
                            evAttrList.Serial_number_or_CI_code = asset.name;
                            evAttrList.Name = asset.name?.ToString().Split('/').Where(y => !string.IsNullOrWhiteSpace(y)).LastOrDefault();
                            evAttrList.Category = SERVERLESS;
                            evAttrList.Model = evAttrList.Category;
                            evAttrList.Brand = Brand; ;
                            evAttrList.Easyvista_name = evAttrList.Name.Substring(0, Math.Min(64, evAttrList.Name.Length));
                            ev_Attributes.Add(evAttrList);

                            evCharList.Project = projectId;
                            evCharList.ProjectId = projectNumber;
                            evCharList.Serial_Number = evAttrList.Serial_number_or_CI_code;
                            evCharList.dockerRegistry = asset.resource?.data?.dockerRegistry;
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

                    if (containerCount.Count > 0)
                    {
                        foreach (var asset in containerCount)
                        {
                            string projectNumber = asset.resource.parent.ToString().Split('/').Where(y => !string.IsNullOrWhiteSpace(y)).LastOrDefault();
                            string projectId = projectLists.projects.Where(x => x.projectNumber == projectNumber).ToList().FirstOrDefault().projectId;
                            location = asset.resource.location;
                            EV.Attr_List evAttrList = new EV.Attr_List();
                            EV.Char_List evCharList = new EV.Char_List();
                            evAttrList.Name = asset.name?.ToString().Split('/').Where(y => !string.IsNullOrWhiteSpace(y)).LastOrDefault();
                            evAttrList.Serial_number_or_CI_code = asset.name;
                            evAttrList.Category = CONTAINERREGISTERY;
                            evAttrList.Model = evAttrList.Category;
                            evAttrList.Brand = Brand; ;
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

                    if (VirtualmachineCount.Count > 0)
                    {
                        foreach (var asset in VirtualmachineCount)
                        {
                            string projectNumber = asset.resource.parent.ToString().Split('/').Where(y => !string.IsNullOrWhiteSpace(y)).LastOrDefault();
                            string projectId = projectLists.projects.Where(x => x.projectNumber == projectNumber).ToList().FirstOrDefault().projectId;
                            EV.Attr_List evAttrList = new EV.Attr_List();
                            EV.Char_List evCharList = new EV.Char_List();
                            EV.Link_List evLinkList = new EV.Link_List();
                            location = asset.resource.location;

                            evAttrList.Name = asset?.name.ToString().Split('/').Where(y => !string.IsNullOrWhiteSpace(y)).LastOrDefault();
                            evAttrList.FQDN = evAttrList.Name + "." + location + ".c." + projectId + ".internal";
                            evAttrList.Brand = Brand; ;
                            evCharList.Serial_Number = asset?.name;
                            evAttrList.Easyvista_name = evAttrList.Name.Substring(0, Math.Min(64, evAttrList.Name.Length));
                            try
                            {
                                var vm_osconfig = oSConfigService.Projects.Locations.Instances.Inventories.List("projects/" + projectId + "/locations/" + location + "/instances/-");
                                var osconfigResponse = vm_osconfig.Execute();
                                if (osconfigResponse.Inventories != null)
                                {
                                    if (osconfigResponse.Inventories.Count > 0)
                                    {
                                        evCharList.OS_Name = osconfigResponse.Inventories[0]?.OsInfo?.ShortName;
                                        evCharList.OS_version = osconfigResponse.Inventories[0]?.OsInfo?.Version;
                                        evAttrList.Hostname = osconfigResponse.Inventories[0]?.OsInfo?.Hostname;
                                        evCharList.Serial_Number = evCharList.Serial_Number;
                                    }
                                }
                            }
                            catch (Exception ee)
                            {

                            }
                            evAttrList.Category = VIRTUALMACHINE;
                            evAttrList.Model = evAttrList.Category;
                            evCharList.HostingLocation = location;
                            evCharList.Project = projectId;
                            evCharList.ProjectId = projectNumber;

                            //machineType
                            var machineType = computeService.MachineTypes.List(projectNumber, location);
                            var machineTypeResponse = machineType.Execute();
                            evCharList.Size = asset.resource.data.machineType?.ToString().Split('/').Where(y => !string.IsNullOrWhiteSpace(y)).LastOrDefault();
                            if (machineTypeResponse != null)
                            {
                                if (machineTypeResponse.Items.Count > 0)
                                {
                                    evCharList.RAM = (machineTypeResponse.Items.Where(x => x.Name == evCharList.Size)?.FirstOrDefault()?.MemoryMb) / 1024;
                                    evCharList.CPU_Count = machineTypeResponse.Items.Where(x => x.Name == evCharList.Size)?.FirstOrDefault()?.GuestCpus.ToString();
                                }
                            }
                            if (evCharList.Size != null)
                            {
                                if (evCharList.Size.ToString().Contains("custom"))
                                {
                                    var memorySize = evCharList.Size.Split('-').Where(y => !string.IsNullOrWhiteSpace(y)).LastOrDefault();
                                    System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"^-?[0-9][0-9,\.]+$");

                                    if (regex.IsMatch(memorySize))
                                    {
                                        evCharList.RAM = Convert.ToInt32(memorySize) / 1024;
                                        var cpuCount = evCharList.Size.Replace("-" + memorySize, "").Split('-').Where(y => !string.IsNullOrWhiteSpace(y)).LastOrDefault();
                                        evCharList.CPU_Count = cpuCount;
                                    }
                                }
                            }

                            evAttrList.Serial_number_or_CI_code = evCharList.Serial_Number;

                            if (asset.resource.data.networkInterfaces.Count > 1)
                            {
                                StringBuilder builder = new StringBuilder();
                                string delimiter = "";

                                foreach (var item in asset.resource.data.networkInterfaces.ToList())
                                {
                                    builder.Append(delimiter);
                                    builder.Append(item.name);
                                    delimiter = ",";
                                }
                                evCharList.NIC = builder.ToString();

                                delimiter = "";
                                builder.Clear();
                                foreach (var item in asset?.resource.data.networkInterfaces.ToList())
                                {
                                    builder.Append(delimiter);
                                    builder.Append(item.networkIP);
                                    delimiter = ",";
                                }
                                evCharList.private_IP = builder.ToString();
                                evAttrList.Primary_IP = builder.ToString();
                            }
                            else
                            {
                                evCharList.NIC = asset.resource.data?.networkInterfaces?.FirstOrDefault().name;
                                evCharList.private_IP = asset.resource.data?.networkInterfaces?.FirstOrDefault().networkIP;
                                evAttrList.Primary_IP = asset.resource.data?.networkInterfaces?.FirstOrDefault().networkIP;
                            }

                            // Network card name NodeSize  Domain  
                            evCharList.Number_of_disks = asset.resource.data?.disks?.Count;
                            if (asset.resource.data?.disks?.Count > 1)
                            {
                                StringBuilder builder = new StringBuilder();
                                string delimiter = "";

                                foreach (var item in asset.resource.data?.disks?.ToList())
                                {
                                    builder.Append(delimiter);
                                    builder.Append(item.diskSizeGb);
                                    delimiter = ",";
                                }
                                evCharList.DiskSpace = builder.ToString();

                                delimiter = "";
                                builder.Clear();
                                foreach (var item in asset.resource.data.disks?.ToList())
                                {
                                    builder.Append(delimiter);
                                    builder.Append(item.type);
                                    delimiter = ",";
                                }
                                evCharList.Disktype = builder.ToString();
                            }
                            else
                            {
                                evCharList.DiskSpace = asset.resource.data.disks?.FirstOrDefault()?.diskSizeGb?.ToString();
                                evCharList.Disktype = asset.resource.data.disks?.FirstOrDefault()?.type;
                            }

                            evLinkList.Impactful_CI = projectNumber;
                            evLinkList.Impacted_CI = evCharList.Serial_Number;
                            evLinkList.Link_label = Link_label;
                            evLinkList.Blocking_Link = Blocking_Link;
                            evLinkList.Blocking_Link_value = Blocking_Link_value;
                            ev_Attributes.Add(evAttrList);
                            ev_Characteristic.Add(evCharList);

                            if ((evLinkList.Impacted_CI != null) && (evLinkList.Impactful_CI != null))
                            {
                                ev_Links.Add(evLinkList);
                            }

                            EV.Link_List evLinkList_region = new EV.Link_List();
                            evLinkList_region.Impactful_CI = evCharList.Serial_Number;
                            evLinkList_region.Impacted_CI = locationList.Where(x => x.Name == location).Select(z => z.Id).FirstOrDefault();
                            evLinkList_region.Link_label = Link_label;
                            evLinkList_region.Blocking_Link = Blocking_Link;
                            evLinkList_region.Blocking_Link_value = Blocking_Link_value;
                            if ((evLinkList_region.Impacted_CI != null) && (evLinkList_region.Impactful_CI != null))
                            {
                                ev_Links.Add(evLinkList_region);
                            }

                            if (asset.resource.data.name.ToString().Contains("gke"))
                            {
                                for (int i = 0; i < asset.resource.data.tags?.items.Count; i++)
                                {
                                    EV.Link_List evLinkList_nodes = new EV.Link_List();
                                    evLinkList_nodes.Impactful_CI = asset.resource.data.tags?.items[i]?.ToString();
                                    evLinkList_nodes.Impacted_CI = evCharList.Serial_Number;
                                    evLinkList_nodes.Link_label = Link_label;
                                    evLinkList_nodes.Blocking_Link = Blocking_Link;
                                    evLinkList_nodes.Blocking_Link_value = Blocking_Link_value;
                                    if ((evLinkList_nodes.Impacted_CI != null) && (evLinkList_nodes.Impactful_CI != null))
                                    {
                                        ev_Links.Add(evLinkList_nodes);
                                    }
                                }
                            }
                        }

                    }

                    if (kubernetesCount.Count > 0)
                    {
                        foreach (var asset in kubernetesCount)
                        {
                            string projectNumber = asset.resource.parent.ToString().Split('/').Where(y => !string.IsNullOrWhiteSpace(y)).LastOrDefault();
                            string projectId = projectLists.projects.Where(x => x.projectNumber == projectNumber).ToList().FirstOrDefault().projectId;
                            EV.Attr_List evAttrList = new EV.Attr_List();
                            EV.Char_List evCharList = new EV.Char_List();
                            EV.Link_List evLinkList = new EV.Link_List();

                            location = asset.resource.location;
                            evAttrList.Name = asset?.name.ToString().Split('/').Where(y => !string.IsNullOrWhiteSpace(y)).LastOrDefault();
                            evAttrList.Serial_number_or_CI_code = asset.name;
                            evAttrList.Category = KUBERNETESCLUSTER;
                            evAttrList.Model = evAttrList.Category;
                            evAttrList.Brand = Brand; ;
                            evAttrList.Easyvista_name = evAttrList.Name.Substring(0, Math.Min(64, evAttrList.Name.Length));
                            evCharList.Project = projectId;
                            evCharList.ProjectId = projectNumber;
                            evCharList.Serial_Number = evAttrList.Serial_number_or_CI_code;
                            evCharList.Type = asset.resource.data.nodeConfig?.machineType;
                            evCharList.HostingLocation = location;
                            evCharList.Node_pools = asset.resource.data.nodePools?.Count;
                            StringBuilder builder = new StringBuilder();
                            string delimiter = "";

                            if (asset.resource.data.nodePools != null)
                            {
                                foreach (var nodes in asset.resource.data.nodePools)
                                {
                                    builder.Append(delimiter);
                                    builder.Append(asset.resource.data.nodeConfig?.diskSizeGb);
                                    delimiter = ",";
                                }
                                evCharList.Node_Size = builder.ToString();
                                delimiter = "";
                                builder.Clear();
                                foreach (var nodes in asset.resource.data.nodePools)
                                {
                                    builder.Append(delimiter);
                                    builder.Append(nodes.name);
                                    delimiter = ",";
                                }
                                evCharList.Nodes = builder.ToString();
                                evCharList.Kubernetes_version = asset.resource?.data?.currentNodeVersion;
                            }

                            evLinkList.Impactful_CI = projectNumber;
                            evLinkList.Impacted_CI = evAttrList.Serial_number_or_CI_code;
                            evLinkList.Link_label = Link_label;
                            evLinkList.Blocking_Link = Blocking_Link;
                            evLinkList.Blocking_Link_value = Blocking_Link_value;


                            ev_Attributes.Add(evAttrList);
                            ev_Characteristic.Add(evCharList);
                            if ((evLinkList.Impacted_CI != null) && (evLinkList.Impactful_CI != null))
                            {
                                ev_Links.Add(evLinkList);
                            }

                            if (location != null)
                            {
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
            }
        }
    }
}