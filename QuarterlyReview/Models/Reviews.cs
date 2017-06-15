using System;
using System.Collections.Generic;

namespace QuarterlyReview.Models
{
    public partial class Reviews
    {
        public Reviews()
        {
            Questions = new HashSet<Questions>();
            Responses = new HashSet<Responses>();
        }

        public int RtId { get; set; }
        public string RtName { get; set; }
        public string RtDesc { get; set; }
        public int RId { get; set; }
        public string Status { get; set; }
        public DateTime StatusDate { get; set; }
        public int SupId { get; set; }
        public int EmpId { get; set; }
        public string DivId { get; set; }
        public string Position { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }

        public virtual ICollection<Questions> Questions { get; set; }
        public virtual ICollection<Responses> Responses { get; set; }
        public virtual DeptToDivMapping Div { get; set; }
        public virtual Employees Emp { get; set; }
        public virtual ReviewTemplate Rt { get; set; }
        public virtual ReviewStatusList StatusNavigation { get; set; }
        public virtual Employees Sup { get; set; }
    }
}
