using System.Windows;
using UserInterfaceLayer;
using DataAccessLayer;
using BusinessLogicLayer;
using APM_SubSystems;
using APMTools;
using System;
using APM_Accounting;
using System.Linq;

namespace APM_SubSystems
{
    public partial class frm_glb_fiscal_year : WindowBase<stp_glb_fiscal_year_selResult>
    {
        public frm_glb_fiscal_year()
        {
            InitializeComponent();
            Initial_WindowBase(dbg_Fiscal_Year, UserToolbar, grpInfo, "glb_fiscal_year", true, null);
        }
        public override bool ValidationForDelete()
        {
            var result = base.ValidationForDelete();
            if (result == false)
                return false;
            if (bindingList.Count == 1)
            {
                Messages.ErrorMessage("قادر به حذف تنها دوره مالی موجود نمی باشید");
                return false;
            }
            return true;
        }
        public override bool ValidationForInsert()
        {
            if (!base.ValidationForInsert())
                return false;
            var lastFiscalYear = DDB.NewContext().tbl_glb_fiscal_year.OrderByDescending(x => x.glb_fiscal_year_id).First();
            if (GlobalVariables.current_fiscal_year_id != lastFiscalYear.glb_fiscal_year_id)
            {
                Messages.ErrorMessage(string.Format("سال مالی جاری سال {0} و آخرین سال مالی سال {1} می باشد. لطفا با آخرین سال مالی وارد برنامه شوید.", GlobalVariables.current_fiscal_year_name, lastFiscalYear.glb_fiscal_year_name));
                return false;
            }
            return true;
        }
    }
}

