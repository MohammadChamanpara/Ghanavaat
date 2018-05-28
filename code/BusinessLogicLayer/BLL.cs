using System;
using System.Collections.Generic;
using System.Linq;
using APMTools;
using System.Windows;
using System.Reflection;
using System.Windows.Controls;
using System.ComponentModel;
using DataAccessLayer;

namespace BusinessLogicLayer
{
    public sealed class BLL<RT>
    {
        #region StoredProcedures
        public class StoredProceduresClass
        {
            public string Insert, Update, Delete, Select;
        }
        public StoredProceduresClass StoredProcedures = new StoredProceduresClass();
        #endregion

        #region Variables
        private List<RT> allRecords = new List<RT>();
        private RT recordParameterForGetAllRecords;
        private RT recordParameterForRefresh;
        public static List<stp_glb_entity_type_option_selResult> ListDetailTypeOption = new List<stp_glb_entity_type_option_selResult>();
        #endregion

        #region Constructor
        public BLL()
        {
            FindStoredProceduresNames();
            recordParameterForGetAllRecords = Activator.CreateInstance<RT>();
            recordParameterForRefresh = Activator.CreateInstance<RT>();
        }
        #endregion

        #region Find StoredProcedures Names
        private void FindStoredProceduresNames()
        {
            string typeName = typeof(RT).Name;
            StoredProcedures.Select = typeName.Remove(typeName.Length - 6);
            typeName = typeName.Remove(typeName.Length - 9);
            StoredProcedures.Insert = typeName + "ins";
            StoredProcedures.Delete = typeName + "del";
            StoredProcedures.Update = typeName + "upd";
        }
        #endregion

        #region DoDataBaseOperation
        public bool DoDataBaseOperation()
        {
            return DoDataBaseOperation(Activator.CreateInstance<RT>());
        }
        public bool DoDataBaseOperation(RT inputRecord)
        {
            string storedProcedureName = typeof(RT).Name.Substring(0, typeof(RT).Name.Length - 6);
            allRecords = DDB.Select<RT>(storedProcedureName, inputRecord);
            if (allRecords == null)
                return false;
            return true;
        }
        #endregion

        #region CustomSelect
        /// <summary>
        ///<para> .دلخواه را برای ما انجام می دهد Select است که هر  Generic این تابع یک تابع </para>
        ///<para> برای شما بر گرداند Select که بعد از نام تابع می نویسید نوع لیستی را مشخص می کند که می خواهید  Generic نوع  </para>
        /// </summary>
        /// <typeparam name="CUSTOM_TYPE">
        /// برای شما برگرداند  Select نوع لیستی که می خواهید 
        /// </typeparam>
        /// <param name="storedProcedureName">
        /// .نام پروسیجری که می خواهید فراخوانی شود
        /// </param>
        /// <param name="parameters">
        /// <para> رکوردی که مقادیر پارامترهای پروسیجر را در خود دارد</para>
        /// <para> .در این قسمت باید یک رکورد به تابع بدهید تا مقادیر پارامترهای پروسیجر از آن استخراج شود </para>
        /// </param>
        /// <returns></returns>
        private List<RT> CustomSelect(string storedProcedureName, RT inputRecord)
        {
            allRecords = DDB.Select<RT>(storedProcedureName, inputRecord);
            if (allRecords == null)
                allRecords = new List<RT>();
            if (allRecords.Count > 0)
                foreach (PropertyInfo property in typeof(RT).GetProperties())
                    if (property.Name.EndsWith("_date"))
                        foreach (RT record in allRecords)
                            property.SetValue(record, APMDateTime.dateWithSlash((string)property.GetValue(record, null)), null);
            return allRecords;
        }
        #endregion

        #region Custom StoredProcedure
        public Boolean CustomStoredProcedure(string storedProcedureName, RT inputRecord)
        {
            return DDB.CustomStoredProcedure(inputRecord, storedProcedureName);
        }
        #endregion

        #region Delete Record
        /// <summary>
        ///     .این تابع یک رکورد را دریافت می کند و آن را از جدول مربوط حذف می نماید
        /// </summary>
        /// <param name="recordForDelete" > 
        ///     .رکوردی که می خواهید حذف می شود
        /// </param>
        /// <returns></returns>
        public Boolean DeleteRecord(RT recordForDelete, params Boolean[] askForConfirm)
        {
            try
            {
                if (recordForDelete == null)
                {
                    Messages.InformationMessage("یک ردیف را برای حذف انتخاب کنید");
                    return false;
                }
                if ((askForConfirm.Length == 0) || (askForConfirm.Length == 1 && askForConfirm[0] == true))
                {
                    MessageBoxResult r = Messages.DeleteMessage("ردیف");
                    if (r == MessageBoxResult.No)
                        return false;
                }
                return DDB.CustomStoredProcedure(recordForDelete, StoredProcedures.Delete);
            }
            catch (System.Exception e)
            {
                Messages.ErrorMessage(e.Message);
            }
            return false;
        }
        #endregion

        #region Save Record
        /// <summary>
        ///     <para>.ثبت می کند DataBase این تابع یک رکورد را در </para>
        ///     <para>.تشخیص داده شود، نوع عملیات را به عنوان دومین پارامتر به تابع بدهید update و insert برای اینکه نوع عملیات بین  </para>
        /// </summary>
        /// <param name="recordForSave">
        ///     .ثبت شود DataBase رکوردی که می خواهید در 
        /// </param>
        /// <param name="currentOperation">
        ///     .انتخاب شود OperationType.Update و OperationType.Insert نوع عملیات که باید بین
        /// </param>
        /// <returns></returns>
        public SaveResult SaveRecord(RT recordForSave, OperationType currentOperation, Boolean askForConfirm)
        {
            try
            {
                if (recordForSave == null)
                    return SaveResult.Cancelled;
                if (askForConfirm == true)
                {
                    MessageBoxResult result = Messages.ConfirmMessage();
                    switch (result)
                    {
                        case MessageBoxResult.Cancel:
                            return SaveResult.Cancelled;
                        case MessageBoxResult.No:
                            return SaveResult.DontSave;
                    }
                }
                string storedProcedureName = (currentOperation == OperationType.Insert)
                ? StoredProcedures.Insert : StoredProcedures.Update;
                if (DDB.CustomStoredProcedure(recordForSave, storedProcedureName))
                {
                    if (currentOperation == OperationType.Insert)
                        allRecords.Add(recordForSave);
                    return SaveResult.Saved;
                }
                return SaveResult.Cancelled;
            }
            catch (System.Exception e)
            {
                Messages.ErrorMessage(e.Message);
            }
            return SaveResult.Cancelled;
        }
        #endregion

        #region Refresh Data
        /// <summary>
        /// <para> .درخواست میکند DataBase این تابع اطلاعات در حال نمایش را مجدداً از </para> 
        /// <para> .برای شما همان اطلاعات را به روز می کند Refresh اخرین درخواست شما برای نمایش اطلاعات ثبت شده است و  </para>
        /// </summary>
        /// <returns></returns>
        public List<RT> RefreshData()
        {
            return CustomSelect(StoredProcedures.Select, recordParameterForRefresh);
        }

        #endregion

        #region GetAllRecords_local
        /// <summary>
        /// <para> .برمی گرداند ،DataBase این تابع لیست تمام رکورد های جدول مربوط را بدون ارجاع به </para> 
        /// <para>برای بدست آوردن همۀ رکورد ها درخواست داده شود ، لیست رکورد ها  DataBase هرگاه به </para>
        /// <para> .ذخیره می شود تا بتوان در چنین مواقعی از آن استفاده نمود allRecords در لیستی به نام  </para>
        /// <para>
        ///  _
        /// </para>
        /// <para> .درخواست می کند DataBase خالی باشد این تابع لیست کامل رکورد ها را از  allRecords اگر لیست </para>
        /// </summary>
        /// <returns></returns>
        public List<RT> GetAllRecords_local()
        {
            if (allRecords.Count == 0)
                GetAllRecords_DB();
            recordParameterForRefresh = recordParameterForGetAllRecords;
            return allRecords;
        }
        #endregion

        #region GetAllRecords_DB
        public List<RT> GetAllRecords_DB()
        {
            recordParameterForRefresh = recordParameterForGetAllRecords;
            return CustomSelect(StoredProcedures.Select, recordParameterForGetAllRecords);
        }
        #endregion

        #region GetAllRecords_Password
        public List<RT> GetAllRecord_Password()
        {
            allRecords = GetAllRecords_DB();
            foreach (RT record in allRecords)
                Encryption.DecryptPassword_Record(record);
            return allRecords;
        }
        #endregion

        #region GetSomeRecords_DB
        /// <summary>
        ///<para> .این تابع تعدادی از رکورد های جدول مربوط را بر می گرداند</para>
        ///<para> به عنوان پارامتر لازم نیست تک تک پارامتر ها جداگانه به تابع ارسال شود </para>
        ///<para> ،کافیست یک رکورد به عنوان پارامتر به تابع بدهید</para>
        ///<para> .را از فیلد های رکورد استخراج می کند StoredProcedure  تابع پارامتر های مورد نیاز  </para>
        /// </summary>
        /// <param name="inputRecord"></param>
        /// <returns></returns>
        public List<RT> GetSomeRecords_DB(RT inputRecord)
        {
            GlobalFunctions.CopyRecord(recordParameterForRefresh, inputRecord);
            return CustomSelect(StoredProcedures.Select, inputRecord);
        }
        #endregion

        #region Fill ComboBox

        public void FillComboBoxForShow(ComboBox comboBox)
        {
            FillComboBoxForShow(comboBox, Activator.CreateInstance<RT>(), null, 0);
        }
        public void FillComboBoxForShow(ComboBox comboBox, string captionOfFirstItem, long idOfFirstItem)
        {
            FillComboBoxForShow(comboBox, Activator.CreateInstance<RT>(), captionOfFirstItem, idOfFirstItem);
        }
        public void FillComboBoxForShow(ComboBox comboBox, RT recordParameter)
        {
            FillComboBoxForShow(comboBox, recordParameter, null, 0);
        }
 
        public void FillComboBoxForShow(ComboBox comboBox, RT recordParameter, string captionOfFirstItem, long idOfFirstItem)
        {
            if (allRecords == null || allRecords.Count == 0 || !GlobalFunctions.ObjectsAreEqual(recordParameter, recordParameterForRefresh))
                allRecords = GetSomeRecords_DB(recordParameter);
            if (allRecords == null || allRecords.Count == 0 && captionOfFirstItem==null)
            {
                comboBox.ItemsSource = allRecords;
                return;
            }
            //if (GlobalFunctions.PropertyExist(allRecords[0], FieldNames<RT>.Selected))
                if (captionOfFirstItem == null && GlobalFunctions.PropertyExist(allRecords[0], FieldNames<RT>.Selected))
                allRecords = allRecords.FindAll(record => GlobalFunctions.GetValueFromProperty<RT, Boolean>(record, FieldNames<RT>.Selected));
            var list = new List<RT>();
            comboBox.DisplayMemberPath = FieldNames<RT>.Name;
            comboBox.SelectedValuePath = FieldNames<RT>.ID;
            if (captionOfFirstItem != null)
            {
                var firstItemOfComboBox = Activator.CreateInstance<RT>();
                GlobalFunctions.SetValueToProperty(firstItemOfComboBox, FieldNames<RT>.Name, captionOfFirstItem);
                GlobalFunctions.SetValueToProperty(firstItemOfComboBox, FieldNames<RT>.ID, idOfFirstItem);
                list.Add(firstItemOfComboBox);
            }
            foreach (RT item in allRecords)
                list.Add(item);
            comboBox.ItemsSource = list;
        }

        public void FillComboBox<T>(ComboBox comboBox, BindingList<T> bindingList)
        {
            FillComboBox(comboBox, bindingList, Activator.CreateInstance<RT>());
        }
        public void FillComboBox<T>(ComboBox comboBox, BindingList<T> bindingList, RT recordParameter)
        {
            FillComboBoxForShow(comboBox, recordParameter);
            comboBox.DataContext = bindingList;
            if (comboBox.Name != null && comboBox.Name.Length > 4)
                comboBox.SetBinding(ComboBox.SelectedValueProperty, comboBox.Name.Substring("cmb_".Length));
        }

        #endregion

        #region GetOneRecord
        public RT GetOneRecord()
        {
            return GetOneRecord(Activator.CreateInstance<RT>());
        }
        public RT GetOneRecord(RT recordParameter)
        {
            var list = GetSomeRecords_DB(recordParameter);
            if (list.Count == 0)
            {
                Messages.ErrorMessage(".تنظیمات را وارد کنید");
                return Activator.CreateInstance<RT>();
            }
            return list.First();
        }
        #endregion

        #region GetDetailsOption
        public static stp_glb_entity_type_option_selResult GetDetailsOption(long entity)
        {
            stp_glb_entity_type_option_selResult RecordDetailTypeOption = new stp_glb_entity_type_option_selResult();
            if (ListDetailTypeOption.Count == 0)
                ListDetailTypeOption = new BLL<stp_glb_entity_type_option_selResult>().GetAllRecords_DB();
            RecordDetailTypeOption = ListDetailTypeOption.Find(detail => detail.glb_entity_type_option_glb_entity_type_id == entity);
            if (RecordDetailTypeOption == null)
            {
                Messages.ErrorMessage("تنظیمات موجودیت ها وارد نشده است ");
                return RecordDetailTypeOption;
            }
            return RecordDetailTypeOption;
        }
        #endregion

        #region ClearAllRecord
        public void ClearAllRecord()
        { allRecords.Clear(); }
        #endregion

    }
}