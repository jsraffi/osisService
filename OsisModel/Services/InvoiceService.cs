using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OsisModel.Models;
using System.Web.Mvc;
using System.Configuration;
using PagedList;
using AutoMapper;

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

        public InvoiceViewModel createInvoice(Guid id)
        {
            StudentCurrentYear studentcurrentyear = db.StudentCurrentYears.Where(sid => sid.StudentRefID == id && sid.Active == true).SingleOrDefault();

            List<InvoiceDetails> InvDetails = new List<InvoiceDetails>()
            {
                new InvoiceDetails()
                {
                    Description="",
                    Quantity =0,
                    UnitPrice=0,
                    Amount=0
                },
                new InvoiceDetails()
                {
                    Description="",
                    Quantity =0,
                    UnitPrice=0,
                    Amount=0
                },
                new InvoiceDetails()
                {
                    Description="",
                    Quantity =0,
                    UnitPrice=0,
                    Amount=0
                }
            };

            var invoice = new Invoice()
            {
                CurrentYearRefID = studentcurrentyear.CurrentYearID,
                Discount =0,
                Latefee =0,
                InvoiceDate =DateTime.Now,
                InvoiceDetails = InvDetails
            };
            InvoiceViewModel invoiceVM = Mapper.Map<InvoiceViewModel>(invoice);
            return invoiceVM;
            

        }
        public bool validateInvoice(InvoiceViewModel invoiceVM)
        {
            Boolean iserror = true;
            //InvoiceViewModel invVM = new InvoiceViewModel();
            //invVM.InvoiceDetailsViewModel = invoiceVM.InvoiceDetailsViewModel;
            
            for(var i=0;i < invoiceVM.InvoiceDetailsViewModel.Count;i++)
            {
                string desc = (invoiceVM.InvoiceDetailsViewModel[0].Description != null) ? invoiceVM.InvoiceDetailsViewModel[0].Description.Trim() : "";

                if (desc != "" )
                {
                    if(invoiceVM.InvoiceDetailsViewModel[i].Quantity <0)
                    {
                        _modelstate.AddModelError("InvoiceDetailsViewModel[" + i + "].Quantity", "Quanity should be greate than zero");
                        iserror = false;
                    }
                }
            }
            return iserror;
        }

        public bool saveInvoice(InvoiceViewModel invoiceVM)
        {

            return true;
        }
    }

    public interface IInvoiceService
    {
        OsisContext getDBContext();
        PagedList.IPagedList<InvoiceSingle> getInvoiceList(int? page, string user = null);
        InvoiceViewModel createInvoice(Guid id);
        bool saveInvoice(InvoiceViewModel invoiceVM);
        bool validateInvoice(InvoiceViewModel invoiceVM);

    }
}