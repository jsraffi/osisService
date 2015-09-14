using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OsisModel.Models;

namespace OsisModel.Models
{
    public class AjaxClassOrder
    {
        public AjaxClassOrder()
        {
            this.ClassOrder = new List<PromotionsSelectVM>();
        }
        public IList<PromotionsSelectVM> ClassOrder {get;set;}
    }
}