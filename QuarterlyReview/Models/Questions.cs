using System;
using System.Collections.Generic;

namespace QuarterlyReview.Models
{
    public partial class Questions
    {
        public int RtId { get; set; }
        public int RId { get; set; }
        public int QtId { get; set; }
        public int QtOrder { get; set; }
        public string QtType { get; set; }
        public string QtQuestion { get; set; }
        public int QId { get; set; }
        public string Answer { get; set; }

        public virtual QuestionTemplate Qt { get; set; }
        public virtual QuestionTypeList QtTypeNavigation { get; set; }
        public virtual Reviews R { get; set; }
    }
}
