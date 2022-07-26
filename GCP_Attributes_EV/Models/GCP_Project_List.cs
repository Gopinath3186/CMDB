using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GCP_Attributes_EV.Models
{
    class GCP_Project_List
    {
        public class ProjectList
        {
            [JsonProperty("projects")]
            public List<Projects> projects { get; set; }
        }
        public class Projects
        {
            public string projectNumber { get; set; }
            public string projectId { get; set; }
            public string lifecycleState { get; set; }
            public string name { get; set; }
            public Labels labels { get; set; }
            public string createTime { get; set; }
            public Parent parent { get; set; }
        }
        public class InventoryList
        { 
            public string readTime { get; set; }
            public List<assets> assets { get; set; }
        }
        public class assets
        {
            public string name { get; set; }
            public string assetType { get; set; }
            public Resource resource { get; set; }
            public List<String> ancestors { get; set; }

        }
        public class Resource
        {
            public string version { get; set; }
            public string discoveryDocumentUri { get; set; }
            public string discoveryName { get; set; }
            public string parent { get; set; }
            public Data data { get; set; }
            public string location { get; set; }
        }
        public class Data
        {
            public string name { get; set; }
            public string projectId { get; set; }
            public string billingEnabled { get; set; }
            public string billingAccountName { get; set; }
            public string instanceClass { get; set; }
            public string machineType { get; set; }
            public string DatabaseInstalledVersion { get; set; }
            public string gceZone { get; set; }
            public Settings settings { get; set; }
            public ServerCaCert serverCaCert { get; set; }

            public string CreationTimestamp { get; set; }
            public string ExpireTime { get; set; }

            public string Type { get; set; }
            public NodeConfig nodeConfig { get; set; }
            public List<NodePools> nodePools { get; set; }
            public string defaultHostname { get; set; }
            public string currentNodeVersion { get; set; }
            public DatasetReference datasetReference { get; set; }

            public List<NetworkInterfaces> networkInterfaces { get; set; }
            public List<Disks> disks { get; set; }
            public Tags tags { get; set; }
            public string dockerRegistry { get; set; }
            public string dnsName { get; set; }
            public string PortName { get; set; }
            public int? Port { get; set; }

            public string StorageClass { get; set; }

            public datasetReference datasetRef { get; set; }
        }
        public class ServerCaCert
        {
            public string CertSerialNumber { get; set; }
        }
        public class Settings
        {
            public string Tier { get; set; }
            public string PricingPlan { get; set; }
        }
        public class DatasetReference
        {
            public string projectId { get; set; }
            public string datasetId { get; set; }
        }
        public class NodeConfig
        {
            public string machineType { get; set; }
            public string diskSizeGb { get; set; }

        }
        public class NodePools
        {
            public string name { get; set; }

        }
        public class NetworkInterfaces
        {
            public string name { get; set; }
            public string networkIP { get; set; }

        }
        public class Disks
        {
            public string diskSizeGb { get; set; }
            public string type { get; set; }

        }
        public class Tags
        {
            public List<string> items { get; set; }
        }
        public class datasetReference
        {
            public string projectId { get; set; }
            public string datasetId { get; set; }
        }
        public class Labels
        {
            public string country { get; set; }
            public string application { get; set; }
            public string billing_key { get; set; }
            public string environment { get; set; }
        }
        public class Parent
        {
            public string type { get; set; }
            public string id { get; set; }
        }
        public class Region_List
        {
            [JsonProperty("items")]
            public List<Items> items { get; set; }
        }
        public class Items
        {
            public string kind { get; set; }
            public string Id { get; set; }
            public string description { get; set; }
            public List<string> zones { get; set; }

            public List<quotas> quotas { get; set; }
        }
        public class quotas
        {
            public string metric { get; set; }
            public string limit { get; set; }
            public string usage { get; set; }
        }
        public class OS_Config
        {
            [JsonProperty("inventories")]
            public List<inventories> inventories { get; set; }
        }
        public class inventories
        {
            public osInfo osInfo { get; set; }
        }
        public class osInfo
        {
            public string longName { get; set; }
            public string shortName { get; set; }
            public string hostname { get; set; }
            public string version { get; set; }
        }

    }
    class GCP_Folders_Lists
    {
        public class FoldersList
        {
            [JsonProperty("Folders")]
            public List<Folders> projects { get; set; }
        }

        public class Folders
        {
            public string name { get; set; }
            public string parent { get; set; }
            public string displayName { get; set; }
            public string lifecycleState { get; set; }
            public string createTime { get; set; }
        }
    }
    class OVH_VM_List
    {
        public class OVH_VM
        {
            public List<Value> value { get; set; }
        }
        public class Value
        {
            public string memory_size_MiB { get; set; }
            public string vm { get; set; }
            public string name { get; set; }
            public string power_state { get; set; }
            public string cpu_count { get; set; }
        }

    }
}
