using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using static GCP_Attributes_EV.Models.GCP_Folders_Lists;
using static GCP_Attributes_EV.Models.GCP_Project_List;

namespace GCP_Attributes_EV.GCP_Services
{
    class GCP_Service_List
    {
        public static string accToken = string.Empty;
        public static string Asset_API = ConfigurationManager.AppSettings["Asset_API"];
        public static string Resource_API = ConfigurationManager.AppSettings["Resource_API"];
        public static string OSConfig_API = ConfigurationManager.AppSettings["OSConfig_API"];
        
        public static InventoryList GetInventoryList(GoogleCredential credentials, string projectID, string CIBased)
        {
            HttpResponseMessage response = null;
            string accessToken = GetAccessToken(credentials);
            accToken = accessToken;
            string Baseurl = Asset_API;
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(Baseurl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
            client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:19.0) Gecko/20100101 Firefox/19.0");

            // VM ,Function ,Container Registry and Kubernetes (4 CI's)
            if (CIBased == "first_ListCIs")
            {
                response = client.GetAsync("v1/folders/" + projectID + "/assets?" +
                    "assetTypes=cloudfunctions.googleapis.com/CloudFunction" +
                    "&assetTypes=artifactregistry.googleapis.com/Repository" +
                    "&assetTypes=compute.googleapis.com/Instance" +
                    "&assetTypes=container.googleapis.com/Cluster" +
                    "&pageSize=1000&contentType=RESOURCE").Result;
            }

            // Database(Sql) ,Key Vault ,Certificat ,LB ,DNS and Endpoint (6 CI's)
            else if (CIBased == "second_ListCIs")
            {
                response = client.GetAsync("v1/folders/" + projectID + "/assets?" +
               "&assetTypes=sqladmin.googleapis.com/Instance" +
               "&assetTypes=secretmanager.googleapis.com/Secret" +
               "&assetTypes=compute.googleapis.com/SslCertificate" +
               "&assetTypes=compute.googleapis.com/BackendService" +
               "&assetTypes=dns.googleapis.com/ManagedZone&assetTypes=compute.googleapis.com/NetworkEndpointGroup" +
               "&pageSize=1000&contentType=RESOURCE").Result;
            }
            // Firewall
            else if (CIBased == "Project_Firewall")
            {
                response = client.GetAsync("v1/folders/" + projectID + "/assets?assetTypes=compute.googleapis.com/Firewall" +
                    "&pageSize=1000&contentType=RESOURCE").Result;
            }
            // WebApp (Serverless)
            else if (CIBased == "Project_WebApp")
            {
                response = client.GetAsync("v1/folders/" + projectID + "/assets?" +
                    "assetTypes=appengine.googleapis.com/Application" +
                    "&assetTypes=appengine.googleapis.com/Service" +
                    "&assetTypes=appengine.googleapis.com/Version" +
                    "&pageSize=1000&contentType=RESOURCE").Result;
            }
            // Datawarehouse
            else if (CIBased == "Project_Datawarehouse")
            {
                response = client.GetAsync("v1/folders/" + projectID + "/assets?assetTypes=bigquery.googleapis.com/Dataset" +
                    "&pageSize=1000&contentType=RESOURCE").Result;
            }
            // Storage
            else if (CIBased == "Project_Storage")
            {
                response = client.GetAsync("v1/folders/" + projectID + "/assets?assetTypes=storage.googleapis.com/Bucket" +
                    "&pageSize=1000&contentType=RESOURCE").Result;
            }
            if (response.IsSuccessStatusCode)
            {
                var inventoryList = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<InventoryList>(inventoryList);
            }
            else
            {
                Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }

            return null;
        }

        public static ProjectList GetProjectList(GoogleCredential credentials)
        {
            string accessToken = GetAccessToken(credentials);
            accToken = accessToken;
            string Baseurl = Resource_API;
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(Baseurl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
            client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:19.0) Gecko/20100101 Firefox/19.0");
            HttpResponseMessage response = client.GetAsync("v1/projects").Result;

            if (response.IsSuccessStatusCode)
            {
                var proj_list = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<ProjectList>(proj_list);
            }
            else
            {
                Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }

            return null;
        }

        public static FoldersList GetFoldersList(GoogleCredential credentials, string folderID)
        {
            string accessToken = GetAccessToken(credentials);
            accToken = accessToken;
            string Baseurl = Resource_API;
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(Baseurl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
            client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:19.0) Gecko/20100101 Firefox/19.0");
            HttpResponseMessage response = client.GetAsync("v2/folders?parent=" + folderID).Result;

            if (response.IsSuccessStatusCode)
            {
                var folder_list = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<FoldersList>(folder_list);
            }
            else
            {
                Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }

            return null;
        } 

        public static string GetAccessToken(GoogleCredential credentials)
        {
            Task<string> task = ((ITokenAccess)credentials).GetAccessTokenForRequestAsync();
            var accessToken = task.Result.ToString()?.TrimEnd('.');
            return accessToken;
        }

    }
}
