namespace GCP_Attributes_EV.Models
{
    public class EV
    {
        public class Attr_List
        {
            public string FQDN { get; set; }
            public string Operation_System { get; set; }
            public string Primary_IP { get; set; }
            public string Mac_Address { get; set; }
            public string Hostname { get; set; }
            public string CSP { get; set; }
            public string Region { get; set; }
            public string Resource_Group { get; set; }
            public string User_type { get; set; }
            public string User_location { get; set; }
            public string Range_of_users { get; set; }
            public string Date_of_creation { get; set; }
            public string Last_Discovery_made { get; set; }
            public string Last_updated_by { get; set; }
            public string Last_updated_on { get; set; }
            public string Source_of_the_data { get; set; }
            public string Criticality_level { get; set; }
            public string Availability_SLA { get; set; }
            public string SLA { get; set; }
            public string Availability { get; set; }
            public string Maintenance_range { get; set; }
            public string Confidentiality { get; set; }
            public string Integrity { get; set; }
            public string RTO { get; set; }
            public string RPO { get; set; }
            public string Easyvista_name { get; set; }
            public string Name { get; set; }
            public string Current_status { get; set; }
            public string Future_Status { get; set; }
            public string CI_state { get; set; }
            public string Current_version { get; set; }
            public string Serial_number_or_CI_code { get; set; }
            public string Description { get; set; }
            public string Model { get; set; }
            public string Brand { get; set; }
            public string Category { get; set; }
            public string Product { get; set; }
            public string Environment { get; set; }
            public string Is_DML { get; set; }
            public string Default_incident_catalog_entry { get; set; }
            public string Default_service_catalog_entry { get; set; }
            public string Default_change_catalog_entry { get; set; }
            public string IT_owner { get; set; }
            public string Business_owner { get; set; }
            public string Approver_Group { get; set; }
            public string Organization { get; set; }
            public string CI_on_call { get; set; }
            public string Shutdown_strategy { get; set; }
            public string Backup_strategy { get; set; }
            public string Is_backuped { get; set; }
            public string Availability_area { get; set; }
            public string Tags { get; set; }
            public string Patch { get; set; }
            public string Patching_strategy { get; set; }
            public string Buildby { get; set; }
            public string Patching_range { get; set; }
        }
        public class Char_List
        {
            public string Serial_Number { get; set; }
            public string DiskSpace { get; set; }
            public string Primary_IP { get; set; }
            public string Disktype { get; set; }
            public string Network_card_name { get; set; }
            public string NIC { get; set; }
            public string OS_version { get; set; }
            public string Node_Size { get; set; }
            public int? Number_of_disks { get; set; }
            public string private_IP { get; set; }
            public string CPU_Count { get; set; }
            public int? RAM { get; set; }
            public string Size { get; set; }
            public string ProjectId { get; set; }
            public string Project { get; set; }
            public string Parents_folders { get; set; }
            public string Domain { get; set; }
            public string OS_Name { get; set; }
            public string HostingLocation { get; set; }
            public string Storage_Type { get; set; }
            public string Record_Type { get; set; }
            public string Dynamic_DNS { get; set; }
            public string TTL { get; set; }
            public string Folder_name { get; set; }
            public string Folder_ID { get; set; }
            public string Begins_on { get; set; }
            public string Expires_on { get; set; }
            public string URL { get; set; }

            public string dockerRegistry { get; set; }

            public string Certificate_Type { get; set; }
            public string HttpsOnly { get; set; }
            public string Load_Balancer_Name { get; set; }
            public string LB_Pool_Name { get; set; }
            public string Type { get; set; }
            public string Virtual_Server_Name { get; set; }
            public string Virtual_Server_IP { get; set; }
            public int? Virtual_Server_Port { get; set; }
            public string Virtual_Network { get; set; }
            public string Database_name { get; set; }
            public string Database_Version { get; set; }
            public string SSL_Serial_number { get; set; }
            public string dataset_name { get; set; }
            public string Tier { get; set; }
            public string Price_Tier { get; set; }
            public string Time_Zone { get; set; }
            public string Description { get; set; }
            public int? Node_pools { get; set; }
            public string Nodes { get; set; }
            public string Kubernetes_version { get; set; }
            public string App_Service_Plan { get; set; }



        }
        public class Link_List
        {
            public string Impactful_CI { get; set; }
            public string Impacted_CI { get; set; }
            public string Link_label { get; set; }
            public string Blocking_Link { get; set; }
            public string Blocking_Link_value { get; set; }
        }
        public class Regions
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }
    }
}
