using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using System.IO;

namespace APMTools
{
    public class ApplicationSettings
    {
        public static ApplicationSettingsInstance Instance = new ApplicationSettingsInstance();
        public class ApplicationSettingsInstance : ApplicationSettingsInstanceBase
        {
            [Category("عملیات")]
            [DisplayName("رشته اتصال به پایگاه داده")]
            [SettingProperty("metadata=res://*/SahaamModel.csdl|res://*/SahaamModel.ssdl|res://*/SahaamModel.msl;provider=System.Data.SqlClient;provider connection string=\";Data Source=.;Initial Catalog=Ghanaat_DB;User ID=sa;Password=sitecore\";")]
            public string EFConnectionString { get; set; }

            [Category("عملیات")]
            [DisplayName("رشته اتصال به پایگاه داده")]
            [SettingProperty("Data Source=.;Initial Catalog=Ghanaat_DB;User ID=sa;Password=sitecore")]
            public string SimpleConnectionString { get; set; }
        }
    }
}
