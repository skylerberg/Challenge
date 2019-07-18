using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;

namespace Challenge
{
    public class UserServiceClient
    {
        private static readonly Uri UserServiceBaseUrI = new Uri("https://appsheettest1.azurewebsites.net");
        private readonly HttpClient client = new HttpClient();

        public class UserServiceListResponse
        {
            public IEnumerable<int> Result { get; set; }
            public string Token { get; set; }
        }

        public IEnumerable<User> All()
        {
            var users = new List<User>();
            foreach (int id in GetAllIds())
            {
                User user = Get(id);
                if (user != null)
                {
                    users.Add(user);
                }
            }
            return users;
        }
        
        public User Get(int id)
        {
            return SendRequest<User>($"/sample/detail/{id}");
        }

        private IEnumerable<int> GetAllIds()
        {
            var ids = new List<int>();
            var listResponse = SendRequest<UserServiceListResponse>("/sample/list");

            while (!String.IsNullOrEmpty(listResponse.Token))
            {
                ids.AddRange(listResponse.Result);

                // Get the next few users
                listResponse = SendRequest<UserServiceListResponse>($"/sample/list?token={listResponse.Token}");
            }

            ids.AddRange(listResponse.Result);
            return ids;
        }

        private T SendRequest<T>(string path)
        {
            HttpResponseMessage response = client.GetAsync(new Uri(UserServiceBaseUrI, path)).Result;
            if (response.IsSuccessStatusCode)
            {
                // TODO handle case where response ends abruptly or is not JSON
                string content = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<T>(content);
            }
            else
            {
                // TODO add appropriate logging here or raise exception
                return default(T);
            }
        }
    }
}
