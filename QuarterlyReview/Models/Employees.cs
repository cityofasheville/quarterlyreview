using System;
using System.Collections.Generic;

namespace QuarterlyReview.Models
{
    public partial class Employees
    {
        public Employees()
        {
            ReviewsEmp = new HashSet<Reviews>();
            ReviewsSup = new HashSet<Reviews>();
        }

        public int EmpId { get; set; }
        public string Active { get; set; }
        public string Employee { get; set; }
        public string EmpEmail { get; set; }
        public string Position { get; set; }
        public string DeptId { get; set; }
        public string Department { get; set; }
        public string DivId { get; set; }
        public string Division { get; set; }
        public int? SupId { get; set; }
        public string Supervisor { get; set; }
        public string SupEmail { get; set; }

        public virtual ICollection<Reviews> ReviewsEmp { get; set; }
        public virtual ICollection<Reviews> ReviewsSup { get; set; }
        public virtual DeptToDivMapping Div { get; set; }
    }
}
