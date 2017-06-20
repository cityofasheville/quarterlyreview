using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuarterlyReview.Models
{
    public class DisplayReview
    {
        public int reviewID { get; set; }
        public string status { get; set; }
        public int supervisorID { get; set; }
        public int employeeID { get; set; }
        public string position { get; set; }
        public DateTime periodStart { get; set; }
        public DateTime periodEnd { get; set; }
        public string reviewerName { get; set; }
        public string employeeName { get; set; }
        string response { get; set; }

        public List<DisplayQuestion> questions { get; set; }


        public DisplayReview(int id, string stat, int sid, int eid, string pos,
            DateTime start, DateTime end, string revName, string empName)
        {
            reviewID = id;
            status = stat;
            supervisorID = sid;
            employeeID = eid;
            position = pos;
            periodStart = start;
            periodEnd = end;
            reviewerName = revName;
            employeeName = empName;
            questions = new List<DisplayQuestion>();
        }
    }
}
