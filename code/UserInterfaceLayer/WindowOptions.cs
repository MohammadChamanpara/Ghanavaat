using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using APMComponents;

namespace UserInterfaceLayer
{
    public class WindowOptions<RT>:WindowBase<RT>
    {
        public void Initial_WindowOptions(APMToolBar userToolbar, APMGroupBox grpOptions)
        {
            Initial_WindowBase(null, userToolbar, grpOptions, null, true, null);
            userToolbar.XType = XWindowType.OptionWindow;
        }
    }
}
