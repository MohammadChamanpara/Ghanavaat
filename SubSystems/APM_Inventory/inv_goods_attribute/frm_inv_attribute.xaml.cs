using System.Windows;
using System.Windows.Controls;
using DataAccessLayer;
using UserInterfaceLayer;
using BusinessLogicLayer;
using APMTools;

namespace APM_SubSystems
{
    public partial class frm_inv_attribute : WindowBase<stp_inv_attribute_selResult>
    {
        #region variables
        ArticlePackage<stp_inv_group_goods_treResult, stp_inv_group_attribute_selResult> groups = new ArticlePackage<stp_inv_group_goods_treResult, stp_inv_group_attribute_selResult>();
        #endregion

        #region Constructor
        public frm_inv_attribute()
        {
            InitializeComponent();
            Initial_WindowBase(dbg_inv_attribute, tbr_inv_attribute, grb_Info,"inv_attribute", true, null);
        }
        #endregion

        #region Group BrowseClick
        private void group_browse_Click(object sender, RoutedEventArgs e)
        {
            BrowseClick_MultiSelect(new WindowSelectTree<stp_inv_group_goods_treResult>(TreeType.MultiSelect_All, "گروههای کالا"), ref groups, "گروه کالا",typeof(frm_group_goods));
        }
        #endregion

        #region Override
        public override void OperationsAfterSaved()
        {
            groups.Save(selectedRecord);
        }
        #endregion
    }
}

