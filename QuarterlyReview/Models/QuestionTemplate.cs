using System;
using System.Collections.Generic;

namespace QuarterlyReview.Models
{
    public partial class QuestionTemplate
    {
        public QuestionTemplate()
        {
            Questions = new HashSet<Questions>();
        }

        public int RtId { get; set; }
        public int QtId { get; set; }
        public int QOrder { get; set; }
        public string QType { get; set; }
        public string QuestionText { get; set; }

        public virtual ICollection<Questions> Questions { get; set; }
        public virtual QuestionTypeList QTypeNavigation { get; set; }
        public virtual ReviewTemplate Rt { get; set; }
    }
}
