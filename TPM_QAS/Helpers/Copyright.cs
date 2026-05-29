using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace TPM_QAS.Helpers
{
    public class Copyright
    {
        public static string GetCopyrightText()
        {
            string copyrightText = ConfigurationManager.AppSettings["CopyrightText"];
            int currentYear = DateTime.Now.Year;

            return copyrightText.Replace("{YEAR}", currentYear.ToString());
        }
    }
}