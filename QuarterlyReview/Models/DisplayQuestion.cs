using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuarterlyReview.Models
{
    public class DisplayQuestion
    {
        public int qID { get; set; }
        public string qType { get; set; }
        public string question { get; set; }
        public string answer { get; set; }

        public DisplayQuestion (int id, string type, string qtext, string atext)
        {
            qID = id;
            qType = type;
            question = qtext;
            answer = atext;
        }
    }
}
