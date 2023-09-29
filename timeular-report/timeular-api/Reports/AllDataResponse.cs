using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace timeular_api.Reports
{
    public class AllDataResponse
    {
        public Timeentry[] timeEntries { get; set; }
    }

    public class Timeentry
    {
        public string id { get; set; }
        public string creator { get; set; }
        public Activity activity { get; set; }
        public Duration duration { get; set; }
        public Note note { get; set; }
    }

    public class Activity
    {
        public string id { get; set; }
        public string name { get; set; }
        public string color { get; set; }
        public string integration { get; set; }
        public string spaceId { get; set; }
    }

    public class Duration
    {
        public DateTime startedAt { get; set; }
        public DateTime stoppedAt { get; set; }
    }

    public class Note
    {
        public string text { get; set; }
        public object[] tags { get; set; }
        public Mention[] mentions { get; set; }
    }

    public class Mention
    {
        public int id { get; set; }
        public string key { get; set; }
        public string label { get; set; }
        public string scope { get; set; }
        public string spaceId { get; set; }
    }

}
