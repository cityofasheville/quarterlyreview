using System;
using System.Collections.Generic;

namespace QuarterlyReview.Models
{
    public partial class ReviewStatusList
    {
        public ReviewStatusList()
        {
            Reviews = new HashSet<Reviews>();
        }

        public string ReviewStatus { get; set; }
        public int RstatusOrder { get; set; }

        public virtual ICollection<Reviews> Reviews { get; set; }
    }
}
