using System;
using System.Collections.Generic;

namespace QuarterlyReview.Models
{
    public partial class QuestionTypeList
    {
        public QuestionTypeList()
        {
            Questions = new HashSet<Questions>();
            QuestionTemplate = new HashSet<QuestionTemplate>();
        }

        public string QuestionType { get; set; }
        public int QtypeOrder { get; set; }

        public virtual ICollection<Questions> Questions { get; set; }
        public virtual ICollection<QuestionTemplate> QuestionTemplate { get; set; }
    }
}
