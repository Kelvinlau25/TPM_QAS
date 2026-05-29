using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TPM_QAS.Enum;

namespace TPM_QAS.Interface
{
    public interface IExport
    {
        FileContentResult ExportToExcel(DataTable dt, string FileName);
        //byte[] ExportToPDF(string ViewName, ControllerContext ControllerContext,
        //                   object modelMaster, iText.Kernel.Geom.PageSize pagesize,
        //                   EnumType.PDF_TYPE PDF_TYPE);
        string RenderViewToString(ControllerContext context, string viewPath, object model,
            bool partial);
    }
}
