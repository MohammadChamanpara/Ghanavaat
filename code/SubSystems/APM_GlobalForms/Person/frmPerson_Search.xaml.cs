using System;
using System.Windows;
using APMTools;
using DataAccessLayer;
using BusinessLogicLayer;
using UserInterfaceLayer;
namespace APM_SubSystems
{
    public partial class frm_Person_Search : WindowBase<stp_glb_person_selResult>
    {
       public frm_Person_Search()
        {
            InitializeComponent();
            this.MinHeight = this.Height;
            this.MinWidth = this.Width;
            Initial_WindowBase(null, windowToolbar, null,null, false, null);
            cboglb_person_group_id.Focus();
        }
       public override void SearchClick()
       {
            searchRecord.glb_person_code = txtglb_person_code.Text.Trim();
            searchRecord.glb_person_title_glb_coding_id = Convert.ToInt64(cboglb_person_title_glb_coding_id.SelectedValue);
            searchRecord.glb_person_name = txtglb_person_name.Text.Trim();
            searchRecord.glb_person_family = txtglb_person_family.Text.Trim();
            searchRecord.glb_person_national_code = txtglb_person_national_code.Text.Trim();
            searchRecord.glb_person_identity_no = txtglb_person_identity_no.Text.Trim();
            searchRecord.glb_person_father_name = txtglb_person_father_name.Text.Trim();
            searchRecord.glb_person_latin_title_glb_coding_id = Convert.ToInt64(cboglb_person_latin_title_glb_coding_id.SelectedValue);
            searchRecord.glb_person_latin_name = txtglb_person_latin_name.Text.Trim();
            searchRecord.glb_person_latin_family = txtglb_person_latin_family.Text.Trim();
            searchRecord.glb_person_economic_code = txtglb_person_economic_code.Text.Trim();
            searchRecord.glb_person_birth_date = txtglb_person_birth_date.Text.Trim();
            searchRecord.glb_person_description = txtglb_person_description.Text.Trim();
            this.DialogResult = true;
        }
    }
}
