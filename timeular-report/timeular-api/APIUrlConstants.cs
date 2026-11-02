using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace timeular_api
{
    public class APIUrlConstants
    {
        public const string Base = "https://api.timeular.com/api/v3/";
        public const string SignIn = "developer/sign-in";
        public const string Logout = "developer/logout";
        public const string GetAllData = "report/data";
        // The public v3/v4 endpoints omit excludedFromWorkHours; this is the web client's endpoint.
        public const string GetAllActivities = "https://api.timeular.com/internal/v8/time-tracking/activities";
    }
}
