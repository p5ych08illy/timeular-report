using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using timeular_report;

namespace timeular_api
{
    public static class TimeularAPIFactory
    {
        public static ITimeularAPI Create() => new TimeularAPI();
    }
}
