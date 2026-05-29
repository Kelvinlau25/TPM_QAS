using System;
using TPM_QAS.DAL;

namespace TPM_QAS.Helpers
{
    public class Copyright
    {
        public static string GetCopyrightText()
        {
            string copyrightText = Database.GetAppSettingStatic("CopyrightText");
            int currentYear = DateTime.Now.Year;

            return copyrightText.Replace("{YEAR}", currentYear.ToString());
        }
    }
}
