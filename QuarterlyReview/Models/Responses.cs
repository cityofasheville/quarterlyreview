using System;
using System.Collections.Generic;

namespace QuarterlyReview.Models
{
    public partial class Responses
    {
        public int RId { get; set; }
        public int? QId { get; set; }
        public string Response { get; set; }
        public DateTime? ResponseDate { get; set; }
        public int Respid { get; set; }

        public virtual Reviews R { get; set; }
    }
}
