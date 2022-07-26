using Google.Apis.Auth.OAuth2;
using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace GCP_Attributes_EV.GCP_Services
{
    class GCP_GoogleCredentials
    {
        public static string ServiceAccount_JSON_Path = ConfigurationManager.AppSettings["ServiceAccount_JSON_Path"];
        public static GoogleCredential GetCredential()
        {
            GoogleCredential credential;
            string bearer = string.Empty;
            String JSONPath = ServiceAccount_JSON_Path + ".json";
            using (var stream = new FileStream(JSONPath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream);
            }
            if (credential.IsCreateScopedRequired)
            {
                credential = credential.CreateScoped("https://www.googleapis.com/auth/cloud-platform");
            }
            // to get Bearer token
            try
            {
                Task<string> task = ((ITokenAccess)credential).GetAccessTokenForRequestAsync();
                task.Wait();
                bearer = task.Result.ToString();
            }
            catch (AggregateException ex)
            {
                throw ex.InnerException;
            }
            return credential;
        }
    }
}
