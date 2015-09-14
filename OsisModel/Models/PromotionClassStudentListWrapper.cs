using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OsisModel.Models;

namespace OsisModel.Models
{
    public class PromotionClassStudentListWrapper
    {
        public PromotionClassStudentListWrapper()
        {
            this.StudentLists = new List<StudentList>();
        }
        
        public PromotionsSelectVM PromoVM { get; set; }

        public IList<StudentList> StudentLists { get; set; }
    }
}