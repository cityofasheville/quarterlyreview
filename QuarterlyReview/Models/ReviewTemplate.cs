using System;
using System.Collections.Generic;

namespace QuarterlyReview.Models
{
    public partial class ReviewTemplate
    {
        public ReviewTemplate()
        {
            QuestionTemplate = new HashSet<QuestionTemplate>();
            Reviews = new HashSet<Reviews>();
        }

        public int RtId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }

        public virtual ICollection<QuestionTemplate> QuestionTemplate { get; set; }
        public virtual ICollection<Reviews> Reviews { get; set; }
    }
}
