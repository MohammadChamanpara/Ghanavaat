using System;
using System.Windows;
using DataAccessLayer;
using BusinessLogicLayer;
using UserInterfaceLayer;
using APMTools;
namespace APM_SubSystems
{
    public partial class frm_PersonelSearch : WindowBase<stp_glb_personel_selResult>
    {
        public void FillComboBoxes()
        {
           // BLLCoding.FillComboBoxForShow_Coding(cboPersonelLatinTitleCodingId, Category.LatinTitle, "", 0);
            //BLLCoding.FillComboBoxForShow_Coding(cboPersonelTitleCodingId, Category.Title, "", 0);
        }
        public frm_PersonelSearch()
        {
            InitializeComponent();
            Initial_WindowBase(null, windowToolbar,null,null, false, null);
            txtPersonelCode.Focus();
            this.MinHeight = this.Height;
            this.MinWidth = this.Width;
            this.MaxHeight = this.Height;
            this.MaxWidth = this.Width;
        }
        public override void SearchClick()
        {
            searchRecord.glb_personel_code = txtPersonelCode.Text.Trim();
            searchRecord.glb_personel_title_glb_coding_id = Convert.ToInt64(cboPersonelTitleCodingId.SelectedValue);
            searchRecord.glb_personel_name = txtPersonelName.Text.Trim();
            searchRecord.glb_personel_family = txtPersonelFamily.Text.Trim();
            searchRecord.glb_personel_national_code = txtPersonelNationalCode.Text.Trim();
            searchRecord.glb_personel_identity_no = txtPersonelIdentityNo.Text.Trim();
            searchRecord.glb_personel_father_name = txtPersonelFatherName.Text.Trim();
            searchRecord.glb_personel_latin_title_glb_coding_id = Convert.ToInt64(cboPersonelLatinTitleCodingId.SelectedValue);
            searchRecord.glb_personel_latin_name = txtPersonelLatinName.Text.Trim();
            searchRecord.glb_personel_latin_family = txtPersonelLatinFamily.Text.Trim();
            searchRecord.glb_personel_birth_date = clnrPersonelBirthDate.Text;
            searchRecord.glb_personel_description = txtPersonelDescription.Text.Trim();
            this.DialogResult = true;
        }
        public override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FillComboBoxes();
        }
    }
} 
