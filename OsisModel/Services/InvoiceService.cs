using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OsisModel.Models;
using System.Web.Mvc;
using System.Configuration;
using PagedList;

namespace OsisModel.Services
{
    public class InvoiceService:CommonServices,IInvoiceService
    {
        private OsisContext db = new OsisContext();

        private ModelStateDictionary _modelstate;

        public InvoiceService(ModelStateDictionary modelstate)
        {
            _modelstate = modelstate;
        }

        public OsisContext getDBContext()
        {
            return db;
        }
        public PagedList.IPagedList<InvoiceSingle> getInvoiceList(int? page, string user = null)
        {
            Tuple<int, int> currentschoolandacademicyear = getUserCurrentSchool(db, user);
            int pageSize = Convert.ToInt32(ConfigurationManager.AppSettings["pageSize"]);
            var pageNumber = page ?? 1;
            var invoices = db.InvoiceSingle.AsNoTracking().OrderBy(d => d.InvoiceDate).Where(sa => sa.SchoolRefID == currentschoolandacademicyear.Item1 && sa.AcademicYearRefID == currentschoolandacademicyear.Item2);

            return invoices.ToPagedList(pageNumber, pageSize);
        }
    }

    public interface IInvoiceService
    {
        OsisContext getDBContext();
        PagedList.IPagedList<InvoiceSingle> getInvoiceList(int? page, string user = null);


    }
}