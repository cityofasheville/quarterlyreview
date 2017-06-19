using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuarterlyReview.Models
{
    public class DisplayReviewSummary
    {
        public int reviewTemplateID { get; set; }
        public int reviewID { get; set; }
        public string reviewTemplateName { get; set; }
        public string reviewTemplateDescription { get; set; }
        public string status { get; set; }
        public DateTime statusDate { get; set; }
        public int supervisorID { get; set; }
        public int employeeID { get; set; }
        public string employeeName { get; set; }
        public string divisionID { get; set; }
        public string position { get; set; }
        public DateTime periodStart { get; set; }
        public DateTime periodEnd { get; set; }

        public DisplayReviewSummary(int rt_id, int r_id, string rt_name, string rt_desc, string stat,
            DateTime stat_date, int sid, int e_id, string e_name, string div_id, string pos,
            DateTime start, DateTime end)
        {
            reviewTemplateID = rt_id;
            reviewID = r_id;
            reviewTemplateName = rt_name;
            reviewTemplateDescription = rt_desc;
            status = stat;
            statusDate = stat_date;
            supervisorID = sid;
            employeeID = e_id;
            employeeName = e_name;
            divisionID = div_id;
            position = pos;
            periodStart = start;
            periodEnd = end;
        }
    }
}
