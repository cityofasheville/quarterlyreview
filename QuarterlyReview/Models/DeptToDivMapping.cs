using System;
using System.Collections.Generic;

namespace QuarterlyReview.Models
{
    public partial class DeptToDivMapping
    {
        public DeptToDivMapping()
        {
            Employees = new HashSet<Employees>();
            Reviews = new HashSet<Reviews>();
        }

        public string DeptId { get; set; }
        public string Department { get; set; }
        public string DivId { get; set; }
        public string Division { get; set; }

        public virtual ICollection<Employees> Employees { get; set; }
        public virtual ICollection<Reviews> Reviews { get; set; }
    }
}
