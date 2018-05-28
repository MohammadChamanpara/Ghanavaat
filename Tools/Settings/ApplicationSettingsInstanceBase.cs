using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.ComponentModel;

namespace APMTools
{
    public class ApplicationSettingsInstanceBase
    {
        #region Constructor
        public ApplicationSettingsInstanceBase()
        {
            string configurationFileName = HelperMethods.ThisIsAWebApplication()
                ? SettingService.ConfigurationFileNames.CustomAssemblyConfigFile(Assembly.GetCallingAssembly())
                : SettingService.ConfigurationFileNames.CurrentApplicationConfigFile;

            this.SettingService = new SettingService
            (
                configurationFileName,
                SettingService.ConfigurationFileNames.DefaultConfigurationSectionName,
                this
            );
            SettingService.LoadSettings();
        }
        #endregion

        #region Properties
        protected SettingService SettingService { get; set; }
        #endregion

        #region Methods
        public void Save()
        {
            this.SettingService.SaveSettings();
        }
        public void Load()
        {
            this.SettingService.LoadSettings();
        }
        #endregion

        #region Indexer
        public object this[string propertyName]
        {
            get
            {
                return this.SettingService[propertyName];
            }
            set
            {
                this.SettingService[propertyName] = value;
            }
        }
        #endregion
    }
}
