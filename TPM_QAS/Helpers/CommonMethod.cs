using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using TPM_QAS.DAL;

namespace TPM_QAS.Helpers
{
    public class CommonMethod
    {
        #region Convert datatable into dropdown listing
        public static async Task<List<SelectListItem>> CommonGetSelectItem(ISession session, string TypeName, string DropDown_Setting = "Setting_DropDown", string USER_ID = "")
        {
            List<SelectListItem> list = new List<SelectListItem>();
            try
            {
                DataTable dt = session.GetObject<DataTable>(DropDown_Setting);
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
            }
        }

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
            }
        }

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
