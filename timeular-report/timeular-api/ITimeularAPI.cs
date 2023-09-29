using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using timeular_api.Activities;
using timeular_api.Reports;

namespace timeular_api
{
    public interface ITimeularAPI : IDisposable
    {
        Task<bool> SignInAsync(SignIn.SignInRequest request);
        void Logout();
        
        Task<AllDataResponse> GetAllData(DateTime from, DateTime to);
        Task<AllActivitiesResponse> GetAllActivities();
    }
}
