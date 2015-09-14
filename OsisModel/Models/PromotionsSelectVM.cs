using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;
using FluentValidation.Attributes;


namespace OsisModel.Models
{
    [Validator(typeof(CheckClassOrder))]
    public class PromotionsSelectVM
    {
        public PromotionsSelectVM()
        {
            this.StudentLists = new List<StudentListVM>();
        }

        public int ClassFrom{ get; set; }
        public int ClassTo{ get; set; }

        public IList<StudentListVM> StudentLists { get; set; }
    }

    public class CheckClassOrder : AbstractValidator<PromotionsSelectVM>
    {
        public CheckClassOrder()
        {
            RuleFor(Promo => Promo.ClassFrom).NotEmpty().WithMessage("Please select a Class From").Must((PromotionsSelectVM, ClassFrom) => CheckLessThan(PromotionsSelectVM, ClassFrom)).WithMessage("To Class to should be > Class From"); 
            RuleFor(Promo => Promo.ClassTo).NotNull().WithMessage("Please select a Class To");
        
        }
        private bool CheckLessThan(PromotionsSelectVM instance, int ClassFrom)
        {
            bool isInOrder = false;
            string cofValue = Convert.ToString(instance.ClassFrom);
            string cotValue = Convert.ToString(instance.ClassTo);
            int classorderFrom = Convert.ToInt32(cofValue.Substring(0,1));
            int classorderTo = Convert.ToInt32(cotValue.Substring(0, 1));
            if(classorderTo > classorderFrom)
            {
                isInOrder = true;
            }
            return isInOrder;
        }
               
    }
}