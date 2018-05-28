using System.Windows.Controls;
using DataAccessLayer;
using BusinessLogicLayer;
using UserInterfaceLayer;
namespace APM_SubSystems
{
    public partial class frm_Person_Group : WindowBase<stp_glb_person_group_selResult>
    {
        #region Variables
        ArticlePackage<stp_glb_person_selResult, stp_glb_person_in_group_selResult> glb_person = new WindowBase<stp_glb_person_group_selResult>.ArticlePackage<stp_glb_person_selResult, stp_glb_person_in_group_selResult>();
        #endregion

        #region Constructor
        public frm_Person_Group()
        {
           
            InitializeComponent();
            Initial_WindowBase(dbgPerson_group, windowToolbar, grpInfo,"glb_person_Group",true,null);
        }
        #endregion

        #region Browse_Click
        private void Browser_select_persons_click(object sender, System.Windows.RoutedEventArgs e)
        {
            BrowseClick_MultiSelect(new WindowSelectGrid<stp_glb_person_selResult>(), ref glb_person, "اشخاص",typeof(frm_Person));
        }
        #endregion

        #region Overrid
        public override void OperationsAfterSaved()
        {
            base.OperationsAfterSaved();
            glb_person.Save(selectedRecord);
        }
        #endregion
    }
}
