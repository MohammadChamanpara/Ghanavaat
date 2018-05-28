using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Reflection;
using APMTools;
using System.Net;

namespace DataAccessLayer
{
    public abstract class DDB
    {
        #region Variables
        private static DataContext dc;
        #endregion

        #region Constructor
        static DDB()
        {
            GlobalVariables.current_company_name = "شرکت";
            string companyFileAddress = PathMethods.GetCompanyPath();
            if (System.IO.File.Exists(companyFileAddress))
            {
                string[] fileContent = System.IO.File.ReadAllLines(companyFileAddress);
                if (fileContent.Length > 0)
                    GlobalVariables.current_company_name = fileContent[0];
            }
        }
        #endregion

        #region RefrencePaire structure
        public struct RefrencePair
        {
            public int ValuePosition;
            public PropertyInfo Property;
        };
        #endregion

        #region Connection
        public static void Connect()
        {
            dc = new Ghanaat_DBDataContext(ApplicationSettings.Instance.SimpleConnectionString);
            dc.CommandTimeout = 400;
        }
        public static string GetClientIp()
        {
            IPHostEntry clientEntry = Dns.GetHostEntry(Dns.GetHostName());
            string clientIPAddress = "";
            foreach (IPAddress ipaddress in clientEntry.AddressList)
                clientIPAddress += ipaddress.ToString();
            clientIPAddress = clientIPAddress.Trim();
            return clientIPAddress;
        }
        public static bool IP_BelongsToThisSystem(string selectedIp)
        {
            if (selectedIp != null && selectedIp.Trim() != string.Empty && GetClientIp().Trim().StartsWith(selectedIp.Trim()))
                return true;
            return false;
        }

        #endregion

        #region Find Stored Procedure
        static public MethodInfo FindStoredProcedure(string StoredProcedureName)
        {
            return (dc == null) ? null : dc.GetType().GetMethod(StoredProcedureName);
        }
        #endregion

        #region Retrieve Parameters From Record
        static public object[] RetrieveParametersFromRecord(object Record, MethodInfo storedProcedure, List<RefrencePair> refrencePairs)
        {
            int parametersCount = storedProcedure.GetParameters().Count();          // Get count of parameters of calling stored procedure
            int valuesIndex = 0;                                                    // first value is the property and second is the position in values 
            int findedParameters = 0, finded;
            string notFindedParameters = "";
            ParameterInfo[] methodParameters = storedProcedure.GetParameters();
            object[] parametersValues = new object[parametersCount];                // Array for Collecting Parameter Values from Record
            foreach (ParameterInfo parameter in methodParameters)
            {
                finded = 1;
                if (parameter.Name.ToLower() == "fiscal_year_id")
                    parametersValues[valuesIndex++] =
                        GlobalVariables.current_fiscal_year_id;
                else if (parameter.Name.ToLower() == "branch_id")
                    parametersValues[valuesIndex++] =
                        GlobalVariables.current_branch_id;
                else if (parameter.Name.ToLower() == "user_id")
                    parametersValues[valuesIndex++] =
                        GlobalVariables.current_user_id;
                else
                {
                    finded = 0;
                    foreach (PropertyInfo property in Record.GetType().GetProperties())
                    {
                        if (parameter.Name.ToLower() == property.Name.ToLower())         // related Prperty finded to get data from.
                        {
                            if (refrencePairs != null && parameter.ParameterType.IsByRef)// this parameter is a refrence(output) property and we must save it.
                            {
                                refrencePairs.Add                                        // saving the property and the position in values. 
                                    (new RefrencePair()
                                    {
                                        ValuePosition = valuesIndex,                     // position of value in parametersValues
                                        Property = property                              // property of record to get output value from S.P.
                                    });
                            }
                            object parameterValue = property.GetValue(Record, null);

                            if (parameterValue != null && property.Name.EndsWith("_password"))
                                parametersValues[valuesIndex++] =
                                    Encryption.EncryptPassword_String(parameterValue.ToString());
                            else if ((parameterValue is string) && parameterValue != null && parameterValue.ToString().Trim() == "")
                                parametersValues[valuesIndex++] = null;
                            else if (parameterValue != null && property.Name.EndsWith("_date"))
                                parametersValues[valuesIndex++] =
                                    APMDateTime.dateWithNoSlash(parameterValue.ToString());

                            else if (property.Name.EndsWith("_id") && (parameterValue is long) && (long)parameterValue == 0)
                                parametersValues[valuesIndex++] = null;
                            else
                                parametersValues[valuesIndex++] = parameterValue;
                            finded = 1;
                            break;
                        }
                    }
                }

                findedParameters += finded;
                if (finded == 0)
                    notFindedParameters = notFindedParameters + "," + parameter.Name;
            }
            if (findedParameters < parametersCount)                                         // if finded properties is less than parameters.
            {
                Messages.ErrorMessage("." + "با نام فیلد های متناظر در پایگاه داده متفاوت است " +
                    storedProcedure.Name + " نام پارامتر های " + "\n"
                    + " : نام پارامتر های یافت نشده " + "\n" + notFindedParameters);        // less parameters is because of the diffrence between 
                // parameters names and Database field Names.
                return null;                                                                // Operation Failed.
            }
            return parametersValues;
        }
        #endregion

        #region CustomStoredProcedure
        /// <summary>
        ///     برای فراخوانی پروسیجر های حذف، درج و ویرایش  می توانید از این تابع استفاده کنید .
        ///     این تابع رکورد و نام پروسیجر را دریافت می کند و
        ///     پروسیجر را روی رکورد داده شده اجرا می کند  
        /// </summary>
        /// <param name="Record">
        /// رکوردی که برای درج ،حذف یا ویرایش به عنوان ورودی به پروسیجر داده می شود    
        /// 
        /// </param>
        /// <param name="storedProcedureName">
        ///     نام پروسیجری که می خواهید فراخوانی شود
        /// </param>
        /// <returns>
        /// موفقیت آمیز بودن عملیات توسط یک مقدار منطقی بر گردانده می شود 
        /// </returns>
        static public Boolean CustomStoredProcedure(object Record, string storedProcedureName)
        {
            try
            {
                MethodInfo storedProcedure = FindStoredProcedure(storedProcedureName);   // finding the Stored Procedure from DataContext.
                if (storedProcedure == null)                                             // the stored procedure name is wrong.
                    return false;                                                        // Operation Failed.
                List<RefrencePair> refrencePairs = new List<RefrencePair>();             // this list contains some pairs for refrence parameters.
                object[] parametersValues = RetrieveParametersFromRecord                 // Getting the value of parameters from properties of the record.
                    (Record, storedProcedure, refrencePairs);                            // and collect them in an array.
                if (parametersValues == null)
                    return false;

                object store =
                storedProcedure.Invoke(dc, parametersValues);                            // Call the Stored Procedure.
                foreach (RefrencePair r in refrencePairs)                                // for every refrence pair in RefrencePairs :
                    r.Property.SetValue(Record, parametersValues[r.ValuePosition], null);// Put value of output Parameters in Record.
                return true;                                                             // Operation Successed.
            }
            catch (Exception e)
            {
                Messages.ExceptionMessage(e);
            }
            return false;
        }
        #endregion

        #region Select
        static public List<T> Select<T>(string StoredProcedureName, T inputRecord)
        {
            try
            {
                MethodInfo StoredProcedure = FindStoredProcedure(StoredProcedureName);      // Find Stored Procedure.
                if (StoredProcedure == null)                                                // Stored Procedure does Not Exist.       
                    return null;                                                            // Operation Failed.
                object[] parametersValues = RetrieveParametersFromRecord
                    (inputRecord, StoredProcedure, null);
                // Calling The Stored Procedure and Casting the result.
                var Result = ((ISingleResult<T>)StoredProcedure.Invoke
                  (dc, parametersValues));
                if (Result.ReturnValue.Equals(0))
                    return Result.ToList();
                else
                    throw new ArgumentException();
            }
            catch (Exception e)
            {
                Messages.ExceptionMessage(e);
            }

            return null;

        }
        #endregion

        #region Transaction
        public static class Transaction
        {
            #region BeginTransaction
            public static Boolean BeginTransaction()
            {
                try
                {
                    if (dc.Connection.State != System.Data.ConnectionState.Open)
                        dc.Connection.Open();
                    if (dc.Transaction == null)
                        dc.Transaction = dc.Connection.BeginTransaction(System.Data.IsolationLevel.Serializable);
                    return true;
                }
                catch (System.Exception ex)
                {
                    Messages.ExceptionMessage(ex);
                    return false;
                }
            }
            #endregion

            #region RollBackTransaction
            public static Boolean RollBackTransaction()
            {
                try
                {
                    if (dc.Transaction != null)
                    {
                        dc.Transaction.Rollback();
                        dc.Transaction = null;
                    }
                    return true;
                }
                catch (System.Exception ex)
                {
                    Messages.ExceptionMessage(ex);
                    return false;
                }
            }
            #endregion

            #region CommitTransaction
            public static Boolean CommitTransaction()
            {
                try
                {
                    if (dc.Transaction != null)
                    {
                        dc.Transaction.Commit();
                        dc.Transaction = null;
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Messages.ExceptionMessage(ex);
                    return false;
                }
            }
            #endregion

        }
        #endregion

        #region Context
        public static SahaamEntities NewContext()
        {
            return new SahaamEntities(ApplicationSettings.Instance.EFConnectionString);
        } 
        #endregion
    }
}