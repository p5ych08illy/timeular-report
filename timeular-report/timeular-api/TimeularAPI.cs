using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using timeular_api;
using timeular_api.Activities;
using timeular_api.Reports;
using timeular_api.SignIn;

namespace timeular_report
{
    internal class TimeularAPI : ITimeularAPI
    {
        private readonly HttpClient httpClient;

        public TimeularAPI()
        {
            httpClient = new();
            httpClient.BaseAddress = new Uri(APIUrlConstants.Base);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new("application/json"));
        }

        public async Task<AllDataResponse> GetAllData(DateTime from, DateTime to)
        {
            var url = $"{APIUrlConstants.GetAllData}/{from.ToString("yyyy-MM-ddTHH:mm:ss.fff")}/{to.ToString("yyyy-MM-ddTHH:mm:ss.fff")}";
            return await httpClient.GetFromJsonAsync<AllDataResponse>(url); //TODO null check
        }

        public async Task<AllActivitiesResponse> GetAllActivities()
        {
            return await httpClient.GetFromJsonAsync<AllActivitiesResponse>(APIUrlConstants.GetAllActivities); //TODO null check
        }

        public async void Logout()
        {
            await httpClient.PostAsync(APIUrlConstants.Logout, null);
        }

        public async Task<bool> SignInAsync(SignInRequest request)
        {
            var response = await httpClient.PostAsJsonAsync(APIUrlConstants.SignIn, request);

            if (response != null)
            {
                var res = await response.Content.ReadFromJsonAsync<SignInResponse>();

                if (res != null)
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", res.token);
                    return true;
                }
            }
            return false;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            httpClient?.Dispose();
        }
    }
}
