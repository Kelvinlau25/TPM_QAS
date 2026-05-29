using System;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using TPM_QAS.Enum;

namespace TPM_QAS.Interface
{
    public interface IExport
    {
        FileContentResult ExportToExcel(DataTable dt, string FileName);
        string RenderViewToString(ControllerContext context, string viewPath, object model, bool partial);
    }
}
