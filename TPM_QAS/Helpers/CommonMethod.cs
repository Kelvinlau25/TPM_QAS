using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TPM_QAS.DAL;

namespace TPM_QAS.Helpers
{
    public class CommonMethod
    {
        #region Convert datatable into dropdown listing
        /// <summary>
        /// Azham 07/11/2022
        /// CommonGetSelectItem convert generic dropdown data list to Select List Item
        /// This function require session method to load data
        /// Load data must be do at login time and store at session using name Setting_DropDown
        /// General Configuration
        /// Setting_DropDown = Session Name (List of Data Table with the columns name TXT and VAL)
        /// TypeName = Type you want to select the data
        /// </summary>
        public static async Task<List<SelectListItem>> CommonGetSelectItem(string TypeName, string DropDown_Setting = "Setting_DropDown", string USER_ID = "")
        {
            List<SelectListItem> list = new List<SelectListItem>();
            try
            {
                HttpContext context = HttpContext.Current;
                DataTable dt = context.Session[DropDown_Setting] as DataTable;
                SelectListItem slc = null;
                if (dt != null && dt.Rows.Count > 0)
                {

                    foreach (DataRow item in dt.Rows)
                    {
                        if (item["TYPE"].ToString().ToUpper() == TypeName.ToUpper())
                        {
                            slc = null;
                            slc = new SelectListItem();
                            slc.Text = item["TXT"].ToString();
                            slc.Value = item["VAL"].ToString();
                            slc.Selected = Convert.ToBoolean(item["SELECTED"]);
                            list.Add(slc);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, USER_ID);
                err = null;
            }
            return list;
        }
        #endregion

        #region Convert datatable into normal listing
        /// <summary>
        /// Azham 01/2/2022
        /// List<T> ConvertToList<T> Convert Datatable to the model
        /// Warning!! Please do not use this function if data more than 100rows or Column More than 10
        /// </summary>
        public static List<T> ConvertToList<T>(DataTable dt)
        {
            List<string> columnNames = dt.Columns.Cast<DataColumn>().Select(x => x.ColumnName.ToLower()).ToList();
            PropertyInfo[] properties = typeof(T).GetProperties();
            return dt.AsEnumerable().Select(row => {
                var obj = Activator.CreateInstance<T>();
                Parallel.ForEach(properties, property =>
                {
                    if (columnNames.Contains(property.Name.ToLower()))
                    {
                        var value = ChangeType(row[property.Name], property.PropertyType);
                        property.SetValue(obj, value);
                    }
                });
                return obj;
            }).ToList();
        }

        public static object ChangeType(object value, Type conversion)
        {
            var t = conversion;

            if (value == null || value == DBNull.Value)
            {
                return null;
            }

            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                t = Nullable.GetUnderlyingType(t);
            }

            return Convert.ChangeType(value, t);
        }

        #endregion

        #region Collect searching parameter into SQL Query 
        /// <summary>
        /// Azham 01/2/2022
        /// Collect searching parameter and convert into SQL Query 
        /// </summary>
        public static string ConvertToSearchSQL(object obj, string DeletedColumnName)
        {
            string SQL = "";
            string probName = "";
            string PropertyType = "";
            string probval = "";
            Type typ = obj.GetType();

            foreach (var prop2 in typ.GetProperties())
            {

                if (prop2.Name.Contains("SEARCH") == true)
                {
                    probName = prop2.Name;
                    PropertyType = prop2.PropertyType.Name;
                    probval = string.Format("{0}", prop2.GetValue(obj));

                    if (prop2.GetValue(obj) != null && probval.ToString().Trim() != "")
                    {
                        if (PropertyType == "int" || PropertyType == "double" || PropertyType == "float" || PropertyType == "decimal")
                        {
                            SQL += string.Format(" AND {0}={1}", prop2.Name.Replace("SEARCH_", ""), prop2.GetValue(obj));
                        }
                        else if (PropertyType == "bool" || PropertyType == "Boolean")
                        {
                            if (DeletedColumnName == probName.Replace("SEARCH_", ""))
                            {
                                if (Convert.ToBoolean(prop2.GetValue(obj)) == false)
                                {
                                    SQL += " AND " + DeletedColumnName + " <>'5'";
                                }
                            }
                            else
                            {
                                SQL += string.Format(" AND {0}='{1}'", prop2.Name.Replace("SEARCH_", ""), prop2.GetValue(obj));
                            }
                        }
                        else
                        {
                            SQL += string.Format(" AND {0}='{1}'", prop2.Name.Replace("SEARCH_", ""), prop2.GetValue(obj));
                        }
                    }
                }

            }
            return SQL;
        }
        #endregion

        #region object type checking 
        /// <summary>
        /// Azham 07/11/2022
        /// cDate checking the string is date or not correct date format and will be return true or false
        /// </summary>
        public static bool isDate(string value)
        {
            try
            {
                DateTime dt;


                return DateTime.TryParse(value, out dt);
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
        }
        /// <summary>
        /// Azham 07/11/2022
        /// isNumeric checking the string is number or not number and will be return true or false
        /// </summary>
        public static bool isNumeric(string value)
        {
            try
            {
                float output;
                return float.TryParse(value, out output);
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
        }


        //Non-standard
        public static string ConvertSearchValue(string[] Scol, string str)
        {
            var val = "";
            if (str == null)
            {
                val = "'%" + str + "%'";
            }
            else
            {
                val = "'%" + str.Trim() + "%'";
            }

            str = "";
            var additonal = "";
            foreach (var col in Scol)
            {
                string[] c = col.Split(new Char[] { '/' });
                str += (additonal + " UPPER(" + c[1] + ") " + "LIKE" + " UPPER(" + val + ") ");
                additonal = " OR";
            }
            return str;
        }
        #endregion
    }
}