using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace timeular_api.Activities
{
    public class AllActivitiesResponse
    {
        public Activity[] activities { get; set; }
        public Inactiveactivity[] inactiveActivities { get; set; }
        public Archivedactivity[] archivedActivities { get; set; }
    }

    public class Activity
    {
        public string id { get; set; }
        public string name { get; set; }
        public string color { get; set; }
        public string integration { get; set; }
        public string spaceId { get; set; }
        public object deviceSide { get; set; }
    }

    public class Inactiveactivity
    {
        public string id { get; set; }
        public string name { get; set; }
        public string color { get; set; }
        public string integration { get; set; }
        public string spaceId { get; set; }
    }

    public class Archivedactivity
    {
        public string id { get; set; }
        public string name { get; set; }
        public string color { get; set; }
        public string integration { get; set; }
        public string spaceId { get; set; }
    }

}
