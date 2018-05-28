using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace APMTools
{
    #region GlobalVariables
    public class GlobalVariables
    {
        public static long current_branch_id;
        public static string current_branch_name;
        public static string current_company_name;
        public static long current_fiscal_year_id;
        public static string current_fiscal_year_name;
        public static long current_user_id;
        public static string current_user_name;
        public static SubSystems currentSubSystem = SubSystems.Inventory;
        public static Boolean collectionViewIsMoving = false;
        public static Boolean collectionViewArticleIsMoving = false;
        public static int FixEntitysCount = typeof(EntityType).GetFields().Count() - 1;
    }
    #endregion

    #region FieldNames
    public static class FieldNames<T>
    {
        #region Variables
        public static string
                      FixPart,

                      ID,
                      Code,
                      CodeFrom,
                      CodeTo,
                      Name,
                      RealName,
                      Date,
                      FromDate,
                      ToDate,

                      LevelNo,
                      LevelNos,
                      ParentID,
                      ParentName,
                      ChildCode,
                      PreCode,
                      Count,
                      CountMeasure,

                      Debt,
                      ArticleFromDebt,
                      ArticleToDebt,
                      Credit,
                      ArticleFromCredit,
                      ArticleToCredit,

                      SumDebt,
                      DocumentFromDebt,
                      DocumentToDebt,
                      SumCredit,
                      DocumentFromCredit,
                      DocumentToCredit,

                      SumCount,

                      Selected,
                      IsGroup,
                      UserId,

                      RegisterDate,
                      RegisterTime,
                      RegistererUserName,
                      RegistererUserId,

                      ConfirmDate,
                      ConfirmTime,
                      ConfirmerUserName,
                      ConfirmerUserId,
                      ConfirmerPersonelId,
                      ConfirmFromDate,

                      GroupGoodsId,
                      GroupId,
                      StoreId,
                      PersonelId,
                      CostCenterId,
                      DetailId,
                      MeasureId,
                      MeasureName,

                      ReceiveId,
                      GoodsRequestId,
                      BuyRequestId,
                      SendId,

                      DestinationEntityId,
                      DestinationDetailId,
                      DestinationEntityTypeId,
                      SendDeadLineDate,
                      SendDeadLineFromDate,
                      SendDeadLineToDate,

                      RequesterPersonelId,
                      RemainingCount,
                      BuyRemainingCount,
                      RemainingCountMeasure,
                      Price,
                      SumPrice,
                      AllPrice,
                      SendType,
                      ReceiveType,

                      Description,
                      DocumentStatusId,
                      DocumentStatusName,
                      DocumentNo,
                      DocumentNoFrom,
                      DocumentNoTo,
                      DocumentType,

                      HaveBaseDocument,
                      HaveAccDocument,
                      IsBaseOfAnother,

                      DocumentStatusIsConfirm,
                      DocumentStatusIsTemp,
                      DocumentStatusIsNote,
                      DocumentStatusIsUseLess,
                      ChartAccountId,
                      PhysicalStartDate,
                      PhysicalEndDate,

                      SumAmountFrom,
                      SumAmountTo,
                      SumPriceFrom,
                      SumPriceTo,
                      ConfirmToDate,
                      RegisterToDate,
                      RegisterFromDate,
                      AccRemaining,
                      HasFollowingAccDocument,
                      AccountantId,
                      WardenName,
                      IsReceive,
                      IsSend,

                      FiscalYear,

                      SimpleSearch
                      ;


        #endregion

        #region Constructor
        static FieldNames()
        {
            string TypeName = typeof(T).Name.ToString();
            TypeName = TypeName.Remove(TypeName.Length - 9).ToLower();
            TypeName = TypeName.Substring(4);
            FieldNames<T>.FixPart = TypeName;
            FieldNames<T>.ID = TypeName + "id";
            FieldNames<T>.Code = TypeName + "code";
            FieldNames<T>.CodeFrom = TypeName + "from_code";
            FieldNames<T>.CodeTo = TypeName + "to_code";
            FieldNames<T>.Name = TypeName + "name";
            FieldNames<T>.RealName = TypeName + "real_name";
            FieldNames<T>.Date = TypeName + "date";
            FieldNames<T>.FromDate = TypeName + "from_date";
            FieldNames<T>.ToDate = TypeName + "to_date";
            FieldNames<T>.LevelNo = TypeName + "level_no";
            FieldNames<T>.LevelNos = TypeName + "level_nos";
            FieldNames<T>.Selected = TypeName + "selected";
            FieldNames<T>.IsGroup = TypeName + "is_group";
            FieldNames<T>.ChildCode = TypeName + "child_code";
            FieldNames<T>.DetailId = TypeName + "acc_detail_id";
            FieldNames<T>.ParentID = TypeName + "parent_id";
            FieldNames<T>.PreCode = TypeName + "pre_code";
            FieldNames<T>.ParentName = TypeName + "parent_name";
            FieldNames<T>.Count = TypeName + "count";
            FieldNames<T>.SumCount = TypeName + "sum_count";
            FieldNames<T>.CountMeasure = TypeName + "count_measure";

            FieldNames<T>.Debt = TypeName + "debt";
            FieldNames<T>.ArticleFromDebt = TypeName + "article_from_debt";
            FieldNames<T>.ArticleToDebt = TypeName + "article_to_debt";

            FieldNames<T>.Credit = TypeName + "credit";
            FieldNames<T>.ArticleFromCredit = TypeName + "article_from_credit";
            FieldNames<T>.ArticleToCredit = TypeName + "article_to_credit";

            FieldNames<T>.SumCredit = TypeName + "sum_credit";
            FieldNames<T>.DocumentFromCredit = TypeName + "document_from_credit";
            FieldNames<T>.DocumentToCredit = TypeName + "document_to_credit";

            FieldNames<T>.SumDebt = TypeName + "sum_debt";
            FieldNames<T>.DocumentFromDebt = TypeName + "document_from_debt";
            FieldNames<T>.DocumentToDebt = TypeName + "document_to_debt";

            FieldNames<T>.GroupGoodsId = TypeName + "inv_group_goods_id";
            FieldNames<T>.StoreId = TypeName + "inv_store_id";
            FieldNames<T>.CostCenterId = TypeName + "glb_cost_center_id";
            FieldNames<T>.PersonelId = TypeName + "glb_personel_id";
            FieldNames<T>.GroupId = TypeName + TypeName + "group_id";

            FieldNames<T>.DestinationEntityId = TypeName + "destination_glb_entity_id";
            FieldNames<T>.DestinationEntityTypeId = TypeName + "destination_glb_entity_type_id";
            FieldNames<T>.DestinationDetailId = TypeName + "destination_acc_detail_id";

            FieldNames<T>.ReceiveId = TypeName + "inv_goods_receive_id";
            FieldNames<T>.GoodsRequestId = TypeName + "inv_goods_request_id";
            FieldNames<T>.BuyRequestId = TypeName + "inv_buy_request_id";
            FieldNames<T>.SendId = TypeName + "inv_goods_send_id";
            FieldNames<T>.Price = TypeName + "price";
            FieldNames<T>.SumPrice = TypeName + "sum_price";
            FieldNames<T>.AllPrice = TypeName + "all_price";
            FieldNames<T>.MeasureId = TypeName + "glb_measure_id";
            FieldNames<T>.MeasureName = TypeName + "glb_measure_name";

            FieldNames<T>.ConfirmerUserId = TypeName + "confirmer_glb_user_id";
            FieldNames<T>.ConfirmerUserName = TypeName + "confirmer_glb_user_name";
            FieldNames<T>.ConfirmerPersonelId = TypeName + "confirmer_glb_personel_id";
            FieldNames<T>.ConfirmDate = TypeName + "confirm_date";
            FieldNames<T>.ConfirmTime = TypeName + "confirm_time";
            FieldNames<T>.ConfirmFromDate = TypeName + "confirm_from_date";

            FieldNames<T>.RequesterPersonelId = TypeName + "requester_glb_personel_id";
            FieldNames<T>.SendDeadLineDate = TypeName + "send_deadline_date";
            FieldNames<T>.SendDeadLineFromDate = TypeName + "send_deadline_from_date";
            FieldNames<T>.SendDeadLineToDate = TypeName + "send_deadline_to_date";

            FieldNames<T>.RegisterDate = TypeName + "register_date";
            FieldNames<T>.RegisterTime = TypeName + "register_time";
            FieldNames<T>.RegistererUserId = TypeName + "registerer_glb_user_id";
            FieldNames<T>.RegistererUserName = TypeName + "registerer_glb_user_name";

            FieldNames<T>.RemainingCount = TypeName + "remaining_count";
            FieldNames<T>.BuyRemainingCount = TypeName + "buy_remaining_count";
            FieldNames<T>.RemainingCountMeasure = TypeName + "remaining_count_measure";

            FieldNames<T>.SendType = TypeName + "send_type_glb_coding_id";
            FieldNames<T>.ReceiveType = TypeName + "receive_type_glb_coding_id";

            FieldNames<T>.Description = TypeName + "description";

            FieldNames<T>.DocumentNo = TypeName + "no";
            FieldNames<T>.DocumentNoFrom = TypeName + "from_no";
            FieldNames<T>.DocumentNoTo = TypeName + "to_no";
            FieldNames<T>.DocumentStatusId = TypeName + "status_glb_coding_id";
            FieldNames<T>.DocumentStatusName = TypeName + "status_glb_coding_name";
            FieldNames<T>.DocumentType = TypeName + "acc_document_type_id";

            FieldNames<T>.HaveBaseDocument = TypeName + "have_base_document";
            FieldNames<T>.HaveAccDocument = TypeName + "have_acc_document";
            FieldNames<T>.IsBaseOfAnother = TypeName + "is_base_of_another";

            FieldNames<T>.DocumentStatusIsConfirm = TypeName + "confirm_glb_coding_id";
            FieldNames<T>.DocumentStatusIsTemp = TypeName + "temporary_glb_coding_id";
            FieldNames<T>.DocumentStatusIsUseLess = TypeName + "useLess_glb_coding_id";
            FieldNames<T>.ChartAccountId = TypeName + "acc_chart_account_id";

            FieldNames<T>.PhysicalStartDate = TypeName + "start_date";
            FieldNames<T>.PhysicalEndDate = TypeName + "end_date";

            FieldNames<T>.SumAmountFrom = TypeName + "sum_amount_from";
            FieldNames<T>.SumAmountTo = TypeName + "sum_amount_to";
            FieldNames<T>.SumPriceTo = TypeName + "sum_price_to";
            FieldNames<T>.SumPriceFrom = TypeName + "sum_price_from";
            FieldNames<T>.ConfirmToDate = TypeName + "confirm_to_date";
            FieldNames<T>.RegisterToDate = TypeName + "register_to_date";
            FieldNames<T>.RegisterFromDate = TypeName + "register_from_date";
            FieldNames<T>.AccRemaining = TypeName + "remaining";
            FieldNames<T>.AccountantId = TypeName + "accountant_id";
            FieldNames<T>.WardenName = TypeName + "warden_name";
            FieldNames<T>.HasFollowingAccDocument = TypeName + "has_following_acc_document";

            FieldNames<T>.IsReceive = TypeName + "is_receive";
            FieldNames<T>.IsSend = TypeName + "is_send";

            FieldNames<T>.FiscalYear = TypeName + "glb_fiscal_year_id";
            FieldNames<T>.UserId = TypeName + "glb_user_id";

            FieldNames<T>.SimpleSearch = TypeName + "simple_search";
        }
        #endregion

    }
    #endregion

    #region GlobalFunctions
    public class GlobalFunctions
    {
        #region Records

        #region Reflection
        public static void SetValueToProperty(object record, string fieldName, object value)
        {
            if (fieldName == null)
                return;
            PropertyInfo property = record.GetType().GetProperty(fieldName);
            if (property == null)
                return;
            property.SetValue(record, value, null);
        }
        public static void SetValueToProperty<Record_T, Value_Type>(Record_T record, string fieldName, Value_Type fieldValue)
        {
            if (record == null)
                return;
            PropertyInfo property = record.GetType().GetProperty(fieldName);
            if (property == null)
                return;
            property.SetValue(record, fieldValue, null);
        }
        public static Return_Type GetValueFromProperty<Record_T, Return_Type>(Record_T record, string fieldName)
        {
            if (record == null)
                return CreateEmptyObject<Return_Type>();
            PropertyInfo property = record.GetType().GetProperty(fieldName);
            if (property == null || property.GetValue(record, null) == null)
            {
                return CreateEmptyObject<Return_Type>();
            }
            return (Return_Type)property.GetValue(record, null);
        }
        private static Return_Type CreateEmptyObject<Return_Type>()
        {
            if (typeof(Return_Type) == typeof(string))
                return (Return_Type)("" as object);
            return Activator.CreateInstance<Return_Type>();
        }
        public static object GetValueFromProperty(object record, string fieldName)
        {
            PropertyInfo property = record.GetType().GetProperty(fieldName);
            if (property == null)
                return null;
            return property.GetValue(record, null);
        }
        public static bool PropertyExist(object record, string fieldName)
        {
            if (record == null || fieldName == null)
                return false;
            return (record.GetType().GetProperty(fieldName) == null) ? false : true;
        }
        #endregion

        #region List To BindingList
        public static void ListToBindingList<T>(List<T> list, BindingList<T> bindingList, params CollectionView[] collectionViews)
        {
            bindingList.Clear();
            if (list == null)
                return;
            foreach (T item in list)
                bindingList.Add(item);

            if (
                (collectionViews != null) &&
                (collectionViews.Length == 1) &&
                (collectionViews[0] is CollectionView)
                )
                (collectionViews[0] as CollectionView).MoveCurrentToFirst();
        }
        #endregion

        #region Arrays Are Equal
        public static Boolean ArraysAreEqual(object[] array1, object[] array2)
        {
            if (array1.Length != array2.Length)
                return false;
            for (int i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i])
                    return false;
                if (array1[i] != null && array1[i].ToString() != array2[i].ToString())
                    return false;
            }
            return true;
        }
        #endregion

        #region Objects Are Equal
        public static Boolean ObjectsAreEqual(object a, object b)
        {
            if (a.GetType() != b.GetType())
                return false;
            foreach (PropertyInfo property in a.GetType().GetProperties())
            {
                object valueA = property.GetValue(a, null);
                object valueB = property.GetValue(b, null);
                if (valueA != null && valueB != null)
                    if (valueA.ToString() != valueB.ToString())
                        return false;
                if ((valueA != null && valueB == null) || (valueA == null && valueB != null))
                    return false;
            }
            return true;
        }
        #endregion

        #region Copy Records

        #region Copy Record
        /// <summary>
        ///     این تابع دو رکورد را دریافت می کند و رکورد دوم را در رکورد اول کپی می نماید 
        /// </summary>
        /// <param name="target">
        ///     رکورد مقصد
        /// </param>
        /// <param name="source">
        ///     رکورد مبدا
        /// </param>
        /// <returns></returns>
        public static void CopyRecord(object target, object source)
        {
            var targetProperties = target.GetType().GetProperties();
            var sourceProperties = source.GetType().GetProperties();
            foreach (PropertyInfo propertyTarget in targetProperties)
                foreach (PropertyInfo propertySource in sourceProperties)
                    if (propertySource.Name == propertyTarget.Name)
                        propertyTarget.SetValue(target, propertySource.GetValue(source, null), null);
        }
        public static void Copy_NonEmptyFields(object target, object source)
        {
            var targetProperties = target.GetType().GetProperties();
            var sourceProperties = source.GetType().GetProperties();
            foreach (PropertyInfo propertyTarget in targetProperties)
                foreach (PropertyInfo propertySource in sourceProperties)
                    if
                    (
                        propertySource.Name == propertyTarget.Name &&
                        propertySource.GetValue(source, null) != null &&
                        propertySource.GetValue(source, null).ToString() != "" &&
                        Convert.ToInt64(propertySource.GetValue(source, null)) != 0
                    )
                        propertyTarget.SetValue(target, propertySource.GetValue(source, null), null);
        }


        #endregion

        #region Copy_FK_To_PK
        public static void Copy_FK_To_PK<Destination_T, Source_T>(Destination_T destinationRecord, Source_T soureceRecord)
        {
            foreach (PropertyInfo sourceProperty in soureceRecord.GetType().GetProperties())
                if (sourceProperty.Name.Length >= FieldNames<Source_T>.FixPart.Length)
                    Copy_Value(destinationRecord, sourceProperty.Name.Substring(FieldNames<Source_T>.FixPart.Length),
                        soureceRecord, sourceProperty.Name);
        }
        #endregion

        #region Copy_PK_To_FK
        public static void Copy_PK_To_FK<Destination_T, Source_T>(Destination_T destinationRecord, Source_T sourceRecord)
        {
            Copy_PK_To_FK(destinationRecord, sourceRecord, FieldNames<Destination_T>.FixPart + FieldNames<Source_T>.ID);
        }
        public static void Copy_PK_To_FK<Destination_T, Source_T>(Destination_T destinationRecord, Source_T sourceRecord, string destinationID)
        {
            if (destinationID == null || destinationRecord == null || sourceRecord == null)
                return;
            Copy_Value(destinationRecord, destinationID, sourceRecord, FieldNames<Source_T>.ID);
            if (destinationID.Length < 3)
                return;
            destinationID = destinationID.Substring(0, destinationID.Length - 2);
            foreach (PropertyInfo property in destinationRecord.GetType().GetProperties())
            {
                if (!property.Name.StartsWith(destinationID))
                    continue;
                Copy_Value(destinationRecord, property.Name,
                    sourceRecord, FieldNames<Source_T>.FixPart + property.Name.Substring(destinationID.Length));
            }
        }
        #endregion

        #region Copy_FK_To_FK
        public static void Copy_FK_To_FK<Destination_T, Source_T>(Destination_T Destination, Source_T Source)
        {
            foreach (PropertyInfo property in Source.GetType().GetProperties())
            {
                if (property.Name == FieldNames<Source_T>.ID)
                    continue;
                if (property.Name.Length >= FieldNames<Source_T>.FixPart.Length)
                    if (property.Name.EndsWith("id"))
                        Copy_Value(Destination, FieldNames<Destination_T>.FixPart + property.Name.Substring(FieldNames<Source_T>.FixPart.Length), Source, property.Name);
            }
        }
        #endregion

        #region Copy_Same_Fields
        public static void Copy_Same_Fields<Destination_T, Source_T>(Destination_T Destination, Source_T Source)
        {
            foreach (PropertyInfo property in Source.GetType().GetProperties())
            {
                if (property.Name == FieldNames<Source_T>.ID)
                    continue;
                if (property.Name.Length >= FieldNames<Source_T>.FixPart.Length)
                    Copy_Value(Destination, FieldNames<Destination_T>.FixPart + property.Name.Substring(FieldNames<Source_T>.FixPart.Length), Source, property.Name);
            }
        }
        #endregion

        #region Copy_Value
        public static void Copy_Value<Destination_T, Source_T>(Destination_T Destination, string destinationFieldName, Source_T Source, string sourceFieldName)
        {
            if (PropertyExist(Destination, destinationFieldName) && PropertyExist(Source, sourceFieldName))
                SetValueToProperty(Destination, destinationFieldName, GetValueFromProperty(Source, sourceFieldName));
        }
        #endregion

        #endregion

        #region BindingListToDataTable
        public static DataTable BindingListToDataTable<T>(BindingList<T> data)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];

                Type propIno = null;
                if (prop.PropertyType.Name == "Nullable`1")
                    propIno = Nullable.GetUnderlyingType(prop.PropertyType);
                if (propIno != null)
                    table.Columns.Add(prop.Name, propIno);
                else
                    table.Columns.Add(prop.Name, prop.PropertyType);
            } object[] values = new object[props.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                } table.Rows.Add(values);
            } return table;
        }
        #endregion

        #region ListToDataTable
        public static DataTable ListToDataTable<T>(List<T> data)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];

                Type propIno = null;
                if (prop.PropertyType.Name == "Nullable`1")
                    propIno = Nullable.GetUnderlyingType(prop.PropertyType);
                if (propIno != null)
                    table.Columns.Add(prop.Name, propIno);
                else
                    table.Columns.Add(prop.Name, prop.PropertyType);
            }
            object[] values = new object[props.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;
        }

        #endregion

        #endregion

        #region String Decimal

        #region ThousandFormat
        public static string ThousandFormat(string nonFormated)
        {
            string formatedString = "";
            double number = 0;
            try
            {
                if (nonFormated.Trim() != string.Empty)
                {
                    number = Convert.ToDouble(nonFormated);
                    formatedString = number.ToString("N0");
                }
            }
            catch (System.Exception ex)
            {
                Messages.ErrorMessage(ex.Message);
            }
            return formatedString;
        }
        #endregion

        #region ConvertCurrencyToNullableDecimal
        public static decimal? ConvertCurrencyToNullableDecimal(string thousandAmount)
        {
            try
            {
                if (thousandAmount.Trim() == string.Empty)
                    return null;
                else
                    return Convert.ToDecimal(thousandAmount);
            }
            catch (System.Exception ex)
            {
                Messages.ErrorMessage(ex.Message + " " + ex.Source + "  " + ex.TargetSite);
                return null;
            }
        }
        #endregion

        #region ConvertCurrencyToDecimal
        public static decimal ConvertCurrencyToDecimal(string thousandAmount)
        {
            try
            {
                if (thousandAmount == null || thousandAmount.Trim() == string.Empty)
                    return 0;
                else
                    return Convert.ToDecimal(thousandAmount);
            }
            catch (System.Exception ex)
            {
                Messages.ErrorMessage(ex.Message + " " + ex.Source + "  " + ex.TargetSite);
                return 0;
            }
        }
        #endregion

        #region ConvertStringToNullableDecimal
        public static decimal? ConvertStringToNullableDecimal(string _strring)
        {
            try
            {
                if (_strring.Trim() == string.Empty)
                    return null;
                else
                    return decimal.Parse(_strring);
            }
            catch (System.Exception ex)
            {
                Messages.ErrorMessage(ex.Message + " " + ex.TargetSite + " " + ex.Source);
                return null;
            }
        }
        #endregion

        #region ConvertStringToDecimal
        public static decimal ConvertStringToDecimal(string _strring)
        {
            try
            {
                if (_strring.Trim() == string.Empty)
                    return 0;
                else
                    return decimal.Parse(_strring);
            }
            catch (System.Exception ex)
            {
                Messages.ErrorMessage(ex.Message + " " + ex.TargetSite + " " + ex.Source);
                return 0;
            }
        }
        #endregion

        #region InserCharInSpecifiedLocation
        public static string InserCharInSpecifiedLocation(string str, int index, char ch)
        {
            try
            {
                if (str.Length < 0)
                    return str;
                return str.Substring(0, index) + ch.ToString() + str.Substring(index + 1, str.Length - (index + 1));
            }
            catch (System.Exception ex)
            {
                Messages.ErrorMessage(ex.Message);
                return str;
            }
        }
        #endregion
        #endregion

        #region Code Operations
        public static string CreateNewCode<T>(List<T> list, int digitCount, string fieldName)
        {
            long maxCode = 0;
            string maxCodeStr = "";
            string newCode;
            PropertyInfo codeProperty = typeof(T).GetProperty(fieldName);
            if (fieldName == string.Empty)
            {
                codeProperty = typeof(T).GetProperty(FieldNames<T>.ChildCode);
                if (codeProperty == null)
                    codeProperty = typeof(T).GetProperty(FieldNames<T>.Code);
                if (codeProperty == null)
                    return "";
            }
            if (codeProperty == null)
                return "";
            foreach (T row in list)
            {
                object _code = codeProperty.GetValue(row, null);
                if (_code == null)
                    continue;
                string code = _code.ToString().Trim();
                if (code == "")
                    continue;
                if (Convert.ToInt64(code) > maxCode)
                {
                    maxCodeStr = code;
                    maxCode = Convert.ToInt64(code);
                }
            }
            maxCode++;
            if (digitCount == 0)
                digitCount = (maxCodeStr.Length > 0) ? ((int)maxCodeStr.Trim().Length) : (int)1;
            newCode = maxCode.ToString();
            newCode = PutZeroBeforeCode(newCode, digitCount);
            newCode = newCode.Substring(newCode.Length - digitCount, digitCount);
            return newCode;
        }
        public static string CreateNewCode<T>(List<T> list, int digitCount)
        {
            return CreateNewCode<T>(list, digitCount, string.Empty);
        }
        public static string PutZeroBeforeCode(string code, int digitCount)
        {
            if (code == null)
                code = "";
            while (code.Length < digitCount)
                code = "0" + code;
            return code;
        }
        public static void PutZeroBeforeCode<T>(T selectedRecord, int digitCount)
        {
            if (selectedRecord == null)
                return;
            var code = GlobalFunctions.GetValueFromProperty<T, string>(selectedRecord, FieldNames<T>.ChildCode);
            GlobalFunctions.SetValueToProperty(selectedRecord, FieldNames<T>.ChildCode, PutZeroBeforeCode(code, digitCount));
        }
        public static Boolean CheckCodeLength<T>(List<T> list, int digitCount)
        {
            PropertyInfo codeProperty = typeof(T).GetProperty(FieldNames<T>.ChildCode);
            if (codeProperty == null)
                return false;
            foreach (T row in list)
            {
                object _code = codeProperty.GetValue(row, null);
                if (_code == null)
                    continue;
                string code = _code.ToString().Trim();
                if (code == "")
                    continue;
                if (code.Split('9').Length > digitCount)
                    return false;
            }
            return true;
        }
        #endregion

        #region Controls
        public static Visibility BooleanToVisibility(Boolean value)
        {
            return ((Boolean)value) ? Visibility.Visible : Visibility.Collapsed;
        }
        public static void SetVisibilityForControl(UIElement control, Boolean Visibility)
        {
            if (control == null)
                return;
            control.Visibility = BooleanToVisibility(Visibility);
        }
        public static void SetEnabledForControl(UIElement control, Boolean isEnable)
        {
            if (control == null)
                return;
            control.IsEnabled = isEnable;
        }
        #endregion

        #region BindControlsToEachOther
        public static void BindControlToAnotherControl(FrameworkElement BindedControl, DependencyProperty bindedProperty, FrameworkElement SourceControl, DependencyProperty SourceProperty)
        {
            BindedControl.SetBinding(bindedProperty,
                   new Binding() { Source = SourceControl, Path = new PropertyPath(SourceProperty.Name), Mode = BindingMode.TwoWay, Converter = new BooleanToVisibilityConverter() });

        }
        public static void BindVisibilityToIsChecked(FrameworkElement BindedControl, FrameworkElement SourceControl)
        {
            BindedControl.SetBinding(FrameworkElement.VisibilityProperty,
                new Binding() { Source = SourceControl, Path = new PropertyPath("IsChecked"), Converter = new BooleanToVisibilityConverter() });
        }
        public static void BindVisibilityToNotIsChecked(FrameworkElement BindedControl, FrameworkElement SourceControl)
        {
            BindedControl.SetBinding(FrameworkElement.VisibilityProperty,
                new Binding() { Source = SourceControl, Path = new PropertyPath("IsChecked"), Converter = new BoolToOppositeVisibilityConverter() });
        }
        public static void BindEnableToIsCheched(FrameworkElement BindedControl, FrameworkElement SourceControl)
        {
            BindedControl.SetBinding(FrameworkElement.IsEnabledProperty,
               new Binding() { Source = SourceControl, Path = new PropertyPath("IsChecked") });
        }
        public class BoolToOppositeVisibilityConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter,
                System.Globalization.CultureInfo culture)
            {
                if (targetType != typeof(Visibility))
                    throw new InvalidOperationException("The target must be a boolean");
                return ((bool)value) ? Visibility.Collapsed : Visibility.Visible;
            }

            public object ConvertBack(object value, Type targetType, object parameter,
                System.Globalization.CultureInfo culture)
            {
                return false;
            }
        }
        #endregion
    }
    #endregion

    #region Messages
    public class Messages
    {
        static string message;
        public static MessageBoxResult ExceptionMessage(Exception exception)
        {
            message = "";
            if (exception is SqlException)
            {
                string persianPart = FindPersianPart(exception);

                if (persianPart != null)
                    persianPart = persianPart.Trim();

                if (exception.Message.ToLower().Contains("allow null") || exception.Message.Contains("FOREIGN KEY") || exception.Message.Contains("INSERT"))
                    if (persianPart != "")
                        message = string.Format("لطفاً اطلاعات {0} را وارد کنید", persianPart);
                    else
                        message = string.Format("لطفاً اطلاعات را کامل کنید.{0} متن خطا : {1}", "\r\n", exception.Message);

                else if (exception.Message.ToLower().Contains("duplicate key"))
                    if (persianPart != "")
                        message = string.Format("اطلاعات {0} تکراری وارد شده است", persianPart);
                    else
                        message = string.Format("اطلاعات تکراری وارد شده است.{0} متن خطا :{1}", "\r\n", exception.Message);


                else if (exception.Message.ToLower().Contains("reference"))
                    if (persianPart != "")
                        message = string.Format("به دلیل وجود ارتباط  با {0} انجام این عملیات امکان پذیر نیست.", persianPart);
                    else
                        message = string.Format("به دلیل وجود ارتباط  با سایر بخش ها انجام این عملیات امکان پذیر نیست.{0} متن خطا : {1}", "\r\n", exception.Message);
                else
                    message += "\n" + (exception as SqlException).Message;
            }
            else if (exception.InnerException != null)
            {
                message = "";
                return ExceptionMessage(exception.InnerException);
            }
            else
                message = exception.Message;


            return MessageBox.Show(message, "", MessageBoxButton.OK,
                MessageBoxImage.Error, MessageBoxResult.OK,
                MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
        }

        private static string FindPersianPart(Exception exception)
        {

            string persianPart = "", message = exception.Message;
            string procedure = (exception is SqlException) ? (exception as SqlException).Procedure : "";
            if (message.Contains("description"))
                persianPart = "شرح";
            else if (message.Contains("code"))
                persianPart = "کد";
            else if (message.Contains("date"))
                persianPart = "تاریخ";
            else if (message.Contains("water1") && !(procedure.Contains("water1")))
                persianPart = "آب اصلی";
            else if (message.Contains("water2") && !(procedure.Contains("water2")))
                persianPart = "جوی";
            else if (message.Contains("water") && !(procedure.Contains("water")))
                persianPart = "آب";
            else if (message.Contains("name"))
                persianPart = "نام";
            else if (message.Contains("family") && !(procedure.Contains("family")))
                persianPart = "نام خانوادگی";
            else if (message.Contains("store") && !(procedure.Contains("store")))
                persianPart = "انبار";
            else if (message.Contains("acc_document") && !(procedure.Contains("acc_document")))
                persianPart = "سند حسابداری";
            else if (message.Contains("destination") && !(procedure.Contains("destination")))
                persianPart = "طرف سند";
            else if (message.Contains("acc_chart_account") && !(procedure.Contains("acc_chart_account")))
                persianPart = "کدینگ حسابداری";
            else if (message.Contains("acc_detail") && !(procedure.Contains("acc_detail")))
                persianPart = "تفصیل";
            else if (message.Contains("acc_document_type") && !(procedure.Contains("acc_document_type")))
                persianPart = "نوع سند";
            else if (message.Contains("glb_bank") && !(procedure.Contains("glb_bank")))
                persianPart = "بانک";
            else if (message.Contains("glb_bank_account") && !(procedure.Contains("glb_bank_account")))
                persianPart = "حساب بانکی";
            else if (message.Contains("glb_bank_account_type") && !(procedure.Contains("glb_bank_account_type")))
                persianPart = "نوع حساب ";
            else if (message.Contains("glb_bank_branch") && !(procedure.Contains("glb_bank_branch")))
                persianPart = "شعب بانکی";
            else if (message.Contains("glb_branch") && !(procedure.Contains("glb_branch")))
                persianPart = "شعب";
            else if (message.Contains("glb_cash") && !(procedure.Contains("glb_cash")))
                persianPart = "صندوق";
            else if (message.Contains("financial_glb_coding_id") && !(procedure.Contains("financial_glb_coding_id")))
                persianPart = "الگوی عملیات مالی";
            else if (message.Contains("glb_company") && !(procedure.Contains("glb_company")))
                persianPart = "شرکت ها و موسسات";
            else if (message.Contains("glb_company_group") && !(procedure.Contains("glb_company_group")))
                persianPart = "گروه شرکت ها و موسسات";
            else if (message.Contains("glb_cost_center") && !(procedure.Contains("glb_cost_center")))
                persianPart = "مرکز هزینه";
            else if (message.Contains("glb_entity_type") && !(procedure.Contains("glb_entity_type")))
                persianPart = "نوع موجودیت";
            else if (message.Contains("glb_entity_type_option") && !(procedure.Contains("glb_entity_type_option")))
                persianPart = "";
            else if (message.Contains("glb_measure") && !(procedure.Contains("glb_measure")))
                persianPart = "واحد اندازه گیری";
            else if (message.Contains("glb_person") && !(procedure.Contains("glb_person")))
                persianPart = "شخص";
            else if (message.Contains("glb_person_group") && !(procedure.Contains("glb_person_group")))
                persianPart = "اشخاص";
            else if (message.Contains("glb_personel") && !(procedure.Contains("")) && !(procedure.Contains("glb_personel")))
                persianPart = "پرسنل";
            else if (message.Contains("glb_personel_group") && !(procedure.Contains("glb_personel_group")))
                persianPart = "گروه پرسنل";
            else if (message.Contains("glb_project") && !(procedure.Contains("glb_project")))
                persianPart = "پروژه";
            else if (message.Contains("glb_user_group") && !(procedure.Contains("glb_user_group")))
                persianPart = "گروه کاربری";
            else if (message.Contains("inv_attribute") && !(procedure.Contains("inv_attribute")))
                persianPart = "خصوصیت";
            else if (message.Contains("inv_buy_request") && !(procedure.Contains("inv_buy_request")))
                persianPart = "درخواست خرید";
            else if (message.Contains("inv_buy_request_article") && !(procedure.Contains("inv_buy_request_article")))
                persianPart = "جزئیات درخواست خرید";
            else if (message.Contains("inv_group_goods") && !(procedure.Contains("inv_group_goods")))
                persianPart = "گروه کالا";
            else if (message.Contains("inv_goods_id") && !(procedure.Contains("inv_goods_id")))
                persianPart = " کالا";
            else if (message.Contains("inv_goods_attribute") && !(procedure.Contains("inv_goods_attribute")))
                persianPart = "خصوصیت کالا";
            else if (message.Contains("inv_goods_parts") && !(procedure.Contains("inv_goods_parts")))
                persianPart = "اجزاء کالا";
            else if (message.Contains("inv_goods_receive") && !(procedure.Contains("inv_goods_receive")))
                persianPart = "رسید کالا";
            else if (message.Contains("inv_goods_receive_article") && !(procedure.Contains("inv_goods_receive_article")))
                persianPart = "جزئیات رسید کالا";
            else if (message.Contains("receive_type_coding_id") && !(procedure.Contains("receive_type_coding_id")))
                persianPart = "نوع رسید";
            else if (message.Contains("inv_goods_request") && !(procedure.Contains("inv_goods_request")))
                persianPart = "درخواست کالا";
            else if (message.Contains("inv_goods_request_article") && !(procedure.Contains("inv_goods_request_article")))
                persianPart = "جزئیات درخواست کالا";
            else if (message.Contains("inv_goods_send") && !(procedure.Contains("inv_goods_send")))
                persianPart = "حواله";
            else if (message.Contains("inv_goods_send_article") && !(procedure.Contains("inv_goods_send_article")))
                persianPart = "جزئیات حواله";
            else if (message.Contains("send_type_coding_id") && !(procedure.Contains("send_type_coding_id")))
                persianPart = "نوع حواله";
            else if (message.Contains("inv_goods_similar") && !(procedure.Contains("inv_goods_similar")))
                persianPart = "کالاهای مشابه";
            else if (message.Contains("inv_opening") && !(procedure.Contains("inv_opening")))
                persianPart = "سند افتتاحیه";
            else if (message.Contains("inv_opening_article") && !(procedure.Contains("inv_opening_article")))
                persianPart = "جزئیات سند افتتاحیه";
            else if (message.Contains("inv_group_attribute") && !(procedure.Contains("inv_group_attribute")))
                persianPart = "خصوصیت گروه کالا";
            else if (message.Contains("glb_fiscal_year") && !(procedure.Contains("glb_fiscal_year")))
                persianPart = "دوره مالی";
            else if (message.Contains("type_glb_coding_id") && !(procedure.Contains("type_glb_coding_id")))
                persianPart = " نوع ";
            else if (message.Contains("product") && !(procedure.Contains("product")))
                persianPart = " محصول ";
            else if (message.Contains("period") && !(procedure.Contains("period")))
                persianPart = " دوره ";
            else if (message.Contains("location") && !(procedure.Contains("location")))
                persianPart = "موقعیت";
            else if (message.Contains("earth") && !(procedure.Contains("earth")))
                persianPart = "زمین";
            else if (message.Contains("cost_type") && !(procedure.Contains("cost_type")))
                persianPart = "تعرفه ها";
            else if (message.Contains("gnt_creditor_account") && !(procedure.Contains("gnt_creditor_account")))
                persianPart = "ریز حساب سهامدار";
            else if (message.Contains("deal") && !(procedure.Contains("deal")))
                persianPart = "سند نقل و انتقال سهام";
            else if (message.Contains("gnt_ownership") && !(procedure.Contains("gnt_ownership")))
                persianPart = "مالکیت سهامدار";
            else if (message.Contains("gnt_creditor") && !(procedure.Contains("gnt_creditor")))
                persianPart = "سهامدار";
            else if (message.Contains("glb_user_access_item") && !(procedure.Contains("glb_user_access_item")))
                persianPart = "آیتم مورد دسترسی";
            else if (message.Contains("glb_user_access") && !(procedure.Contains("glb_user_access")))
                persianPart = "دسترسی کاربران";
            else if (message.Contains("glb_user") && !(procedure.Contains("glb_user")))
                persianPart = "کاربر";
            return persianPart;
        }
        public static void ErrorMessage(string text)
        {
            MessageBox.Show(text, "خطا", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
        }
        public static void EnterDataMessage(string field)
        {
            message = "لطفاً {0} را وارد کنید";
            message = String.Format(message, field);
            MessageBox.Show(message, "خطا", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
        }
        public static MessageBoxResult InformationMessage(string message)
        {
            return MessageBox.Show(message, "پیغام", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
        }
        public static MessageBoxResult ConfirmMessage()
        {
            message = "آیا مایلید تغییرات ثبت شود؟";
            return MessageBox.Show(message, "", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.RightAlign);
        }
        public static void SuccessMessage()
        {
            message = "عملیات با موفقیت انجام شد";
            MessageBox.Show(message, "موفقیت", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
        }
        public static void SuccessMessage(string text)
        {
            message = text + " با موفقیت انجام شد";
            MessageBox.Show(message, "موفقیت", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
        }
        public static void UnSuccessMessage()
        {
            message = "انجام عملیات با شکست روبرو شد، لطفاً مجدداً تلاش کنید";
            MessageBox.Show(message, "خطا", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
        }
        public static MessageBoxResult DeleteMessage(string entity)
        {
            message = "آیا مایل به حذف {0} جاری هستید؟";
            message = string.Format(message, entity);
            return MessageBox.Show(message, "", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.RightAlign);
        }
        public static void WarningMessage(string message)
        {
            MessageBox.Show(message, "توجه", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
        }
        public static MessageBoxResult CancelMessage()
        {
            message = "آیا از انصراف عملیات جاری اطمینان دارید؟";
            return MessageBox.Show(message, "", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.RightAlign);
        }
        public static void NotExistRowForSelect(string field)
        {
            message = "هیچ {0} برای انتخاب وجود ندارد";
            message = string.Format(message, field);
            MessageBox.Show(message, "توجه", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RightAlign);

        }
        public static void NoSelectedRow()
        {
            message = "هیچ گزینه ای انتخاب نشده است";
            MessageBox.Show(message, "توجه", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RightAlign);

        }
        public static void NoResultForPrint()
        {
            MessageBox.Show("جستجو  نتیجه ای در بر نداشت", "توجه", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
        }
        public static MessageBoxResult QuestionMessage_YesNoCancel(string question)
        {
            return MessageBox.Show(question, "", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.RightAlign);
        }
        public static MessageBoxResult QuestionMessage_YesNo(string question)
        {
            return MessageBox.Show(question, "", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.RightAlign);
        }
        public static void NotExistsRecordForEditOrDel(OperationType operationType)
        {
            string operation, message;
            switch (operationType)
            {
                case OperationType.Update: operation = "ویرایش";
                    break;
                case OperationType.Delete: operation = "حذف";
                    break;
                default: operation = "";
                    break;
            }

            message = ".کاربر گرامی : ردیفی برای {0} انتخاب نکرده اید";
            message = string.Format(message, operation);
            MessageBox.Show(message, "توجه", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
        }
        public static void UnableToEditOrDelDocumentBecause(OperationType operationType, AccDocumentStatus document_status)
        {
            string operation, status, message;
            switch (operationType)
            {
                case OperationType.Update: operation = "ویرایش";
                    break;
                case OperationType.Delete: operation = "حذف";
                    break;
                default: operation = "";
                    break;
            }
            switch (document_status)
            {
                case AccDocumentStatus.Confirm: status = "قطعی";
                    break;
                case AccDocumentStatus.UseLess: status = "ابطالی";
                    break;
                default: status = "نامعین";
                    break;
            }

            message = ".کاربر گرامی : به دلیل {0} بودن سند قادر به {1} آن نیستید";
            message = string.Format(message, status, operation);
            MessageBox.Show(message, "توجه", MessageBoxButton.OK, MessageBoxImage.Stop, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
        }
    }

    #endregion

    #region Enums
    public enum SubSystems
    {
        Global,
        Accounting,
        Inventory,
        AfterSaleServices,
        Exchange,
        IndustryEngineering,
        PayRoll,
        InnerAuditing,
        ProductionManagement,
        Sell,
        Automation,
        Tools,
    }
    public class MessagesForUser
    {
        public static readonly string
            ClosingMessage = "آیا مایلید تغییرات شما ثبت شود؟",
            x = "...",
            y = "...";
    };
    public enum OperationType
    {
        Nothing,
        Insert,
        Update,
        Delete
    }
    public enum SubmitResult
    {
        Success,        // اطلاعات با موفقیت در دیتابیس درج شد
        UnSuccess,      // در هنگام ثبت اطلاعات به دلیل پیش بینی شده، نمی توان تغییرات را اعمال کرد
        Suspend,        // کاربر مقادیر نامعتبری را وارد کرده که در هنگام ثبت کشف شده، و لازم است کاربر اطلاعات خود را تصحیح کند
        SystemError     // خطای سیستمی رخ داده باشد
    }
    public enum SaveResult
    {
        Saved,          // The Record Saved Successfully.
        Cancelled,      // Opertaion Cancelled because of an Error or user's decision to cancel saving.
        DontSave        // Recover previous data Before Changes.
    }
    public enum CodingCategory
    {
        Glb_LatinTitle = 1,
        Glb_Title = 2,
        Glb_CostCenterType = 9,
        Glb_SecurityQuestion = 15,
        Glb_CompanyTypes = 18,
        Glb_MeasureType = 34,

        Inv_AttributeType = 35,
        Inv_StoreType = 20,
        Inv_StoreNature = 21,
        Inv_MaterialType = 22,
        Inv_BachStatus = 23,
        Inv_PersonPost = 24,
        Inv_DocumentType = 25,
        Inv_DestinationEntity = 26,
        Inv_GoodsPriceing = 27,
        Inv_ReceiveType = 36,
        Inv_SendType = 37,

        Acc_TempAction = 28,
        Acc_AccountType = 4,
        Acc_AccountNature = 5,
        Acc_FixedLevels = 17,
        Acc_RepeatedComment = 30,

        Tre_StatusChequePayable = 32,
        Tre_DocumentType = 33,
    }
    public enum AccDocumentStatus
    {
        Note = 87,
        Temporary = 88,
        Confirm = 89,
        UseLess = 90,
        Deleted = 91
    }
    public enum DocumentTypes
    {

        /* Gnt Documents */
        GntDeal,

        /* Inv Documents */
        Send,
        Receive,
        GoodsRequest,
        BuyRequest,
        Opening,
        Closing,
        PhysicalCounting,

        /* Inv Reports */
        StockReport,
        CardexReport,
        SendOrReceiveReport,
        SendReport,
        ReceiveReport,
        GoodsRequestReport,
        BuyRequestReport,
        OpeningReport,

        /* Acc Document */
        AccountingDocument,

        /* Acc Reports */
        AccountBalanceReport,
        NoteBooksReport,
        KolNoteBookRule,
        Balance4Columns
    }
    public enum TreeType
    {
        SingleSelect_All,
        SingleSelect_LastChild,
        SingleSelect_Entity,
        SingleSelect_LastEntity,
        MultiSelect_All,
        MultiSelect_LastNode,
    }
    public enum EntityType
    {
        glb_personel = 1,
        glb_person = 2,
        glb_company = 3,
        glb_cash = 4,
        glb_bank = 5,
        glb_bank_branch = 6,
        glb_bank_account = 7,
        glb_cost_center = 8,
        glb_project = 9,
        inv_store = 10,
        inv_goods = 11,
        gnt_creditor = 12
    }
    public enum GoodsFinancial
    {
        Inv_LIFO = 179,
        Inv_FIFO = 180,
        Inv_Average = 182,
        Inv_Weighted_Average = 185,
        Inv_Specific_Price = 269

    }
    public enum SendType
    {
        Transitive = 259,                 //انتقالی
        Sales = 260,                     // فروش
        LendFromUs = 261,                 //امانی ما نزد دیگران
        ReturnedLendFromOthers = 262,     //مرجوع امانی دیگران نزد ما
        ReturnedFromBying = 263,         //مرجوع از خرید
        DeliverToProduction = 264,       //تحویل به تولید
        Consume = 265,                    // مصرف
        Adjustment = 271                //تعدیلی
    }
    public enum ReceiveType
    {
        Transitive = 251,                 //انتقالی
        LendFromOthersToUs = 254,         //امانی دیگران نزد ما
        ReturnedLendFromUs = 253,         //مرجوع امانی ما نزد دیگران
        ReturnedFromُSales = 252,          //مرجوع از فروش
        ReceivedFromProduction = 256,     //دریافت از تولید
        Bying = 255,                      //خرید
        ReturnedFromProduction = 257,   //مرجوع از مصرف
        Adjustment = 270                 //تعدیلی

    }
    #endregion Enums

    #region Path Methods
    public class PathMethods
    {
        public static string GetExePath()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }
        public static string GetServerPath()
        {
            return GetExePath() + "server.txt";
        }
        public static string GetCompanyPath()
        {
            return GetExePath() + "company.txt";
        }
    }
    #endregion

    #region APMDateTime
    public class APMDateTime
    {
        #region Private Tools
        private static string GetTodayWithWordMonth()
        {
            string strMonth;
            SetYearMonthDay();
            switch (_mounth)
            {
                case 1: strMonth = "فروردین";
                    break;
                case 2: strMonth = "اردیبهشت";
                    break;
                case 3: strMonth = "خرداد";
                    break;
                case 4: strMonth = "تیر";
                    break;
                case 5: strMonth = "مرداد";
                    break;
                case 6: strMonth = "شهریور";
                    break;
                case 7: strMonth = "مهر";
                    break;
                case 8: strMonth = "آبان";
                    break;
                case 9: strMonth = "آذر";
                    break;
                case 10: strMonth = "دی";
                    break;
                case 11: strMonth = "بهمن";
                    break;
                case 12: strMonth = "اسفند";
                    break;
                default: strMonth = "";
                    break;
            }
            return _day.ToString() + " " + strMonth + " " + _year.ToString();
        }
        private static void SetYearMonthDay()
        {
            PersianCalendar persianCalendar = new PersianCalendar();
            _year = persianCalendar.GetYear(DateTime.Now.Date);
            _mounth = persianCalendar.GetMonth(DateTime.Now.Date);
            _day = persianCalendar.GetDayOfMonth(DateTime.Now.Date);
        }
        private static string GetToday()
        {
            string today, day, mounth, year;
            SetYearMonthDay();
            day = _day.ToString();
            mounth = _mounth.ToString();
            year = _year.ToString();
            if (day.Length == 1)
                day = "0" + day;
            if (mounth.Length == 1)
                mounth = "0" + mounth;
            today = year + "/" + mounth + "/" + day;
            return today;
        }
        private static int _year, _mounth, _day;
        #endregion

        #region Public Tools
        public static string Today
        {
            get
            {
                return GetToday();
            }
        }
        public static string TodayWithWordMonth
        {
            get
            {
                return GetTodayWithWordMonth();
            }
        }
        public static string SystemTime
        {
            get
            {
                string strHours = DateTime.Now.TimeOfDay.Hours.ToString();
                string strMinutes = DateTime.Now.TimeOfDay.Minutes.ToString();
                if (int.Parse(strHours) < 10)
                    strHours = "0" + strHours;
                if (int.Parse(strMinutes) < 10)
                    strMinutes = "0" + strMinutes;
                return strHours + ":" + strMinutes;
            }
        }
        public static string dateWithNoSlash(string DateWithSlash)
        {
            string datewithnoslash;
            datewithnoslash = DateWithSlash;
            string[] date = datewithnoslash.Split(new char[] { '/' });
            for (int i = 0; i < date.Length; i++)
                if (date[i].Length == 1)
                    date[i] = "0" + date[i];
            datewithnoslash = "";
            foreach (string j in date)
            {
                datewithnoslash = datewithnoslash + j;

            }

            return datewithnoslash;

        }
        public static string dateWithSlash(string DateWithoutSlash)
        {
            string datewithslash;
            datewithslash = DateWithoutSlash;
            if (datewithslash != null && datewithslash.Length == 8)
            {
                datewithslash = datewithslash.Insert(4, "/");
                datewithslash = datewithslash.Insert(7, "/");
            }
            else
                datewithslash = "    /  /  ";
            return datewithslash;
        }

        #endregion
    }
    #endregion

    #region Encryption
    public class Encryption
    {
        #region Variables
        static string EncryptionString = "&%#@?,:*";
        #endregion
        public static void EncryptPassword_Record<T>(T record)
        {
            foreach (PropertyInfo Property in record.GetType().GetProperties())
                if (Property.Name.EndsWith("_password"))
                    GlobalFunctions.SetValueToProperty(record, Property.Name, Encrypt((string)GlobalFunctions.GetValueFromProperty(record, Property.Name), EncryptionString));
        }
        public static void DecryptPassword_Record<T>(T record)
        {
            foreach (PropertyInfo Property in record.GetType().GetProperties())
                if (Property.Name.EndsWith("_password"))
                    GlobalFunctions.SetValueToProperty(record, Property.Name, Decrypt((string)GlobalFunctions.GetValueFromProperty(record, Property.Name), EncryptionString));
        }
        private static string Encrypt(string strText, string strEncrypt)
        {
            byte[] byKey = new byte[20];
            byte[] dv = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
            try
            {
                byKey = Encoding.UTF8.GetBytes(strEncrypt.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputArray = Encoding.UTF8.GetBytes(strText);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(byKey, dv), CryptoStreamMode.Write);
                cs.Write(inputArray, 0, inputArray.Length);
                cs.FlushFinalBlock();
                return Convert.ToBase64String(ms.ToArray()) + "ENC";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static string Decrypt(string strText, string strEncrypt)
        {
            if (!strText.EndsWith("ENC"))
            {
                Messages.WarningMessage("کلمه عبور شما در پایگاه داده دستکاری شده");
                return strText;
            }
            strText = strText.Substring(0, strText.Length - 3);
            byte[] bKey = new byte[20];
            byte[] IV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
            try
            {
                bKey = System.Text.Encoding.UTF8.GetBytes(strEncrypt.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByteArray = Convert.FromBase64String(strText);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(bKey, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                Encoding encoding = Encoding.UTF8;
                return encoding.GetString(ms.ToArray());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string EncryptPassword_String(string password)
        {
            return password = Encrypt(password, EncryptionString);
        }
        public static string DecryptPassword_String(string password)
        {
            return password = Decrypt(password, EncryptionString);
        }
    }
    #endregion Encryption
}