using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using static TPM_QAS.Enum.EnumType;

namespace TPM_QAS.Models
{
    public class CommonModel<TListModel, TModel>
                where TListModel : class
                where TModel : class
    {
        public CommonModel(Controller controller, string title, bool Create = true, bool Export = true,
                           bool Delete = true, bool Save = true, string userID = "")
        {
            Title = title;
            Controller = controller;
            UserID = userID;
            string tmpControllerName = "";
            AddDetailsButtonURL = tmpControllerName + "/Add";
            DeleteButtonURL = tmpControllerName + "/Delete";
            ExportButtonURL = tmpControllerName + "/Export";
            AuditTrailButtonURL = tmpControllerName + "/Audit";
        }

        public CommonModel()
        {
        }

        public Controller Controller { get; set; }
        public string UserID { get; set; }
        public bool Create { get; set; }
        public bool Export { get; set; }
        public bool Delete { get; set; }
        public bool Save { get; set; }
        public string Title { get; set; }
        public string AddDetailsButtonURL { get; set; }
        public string DeleteButtonURL { get; set; }
        public string ExportButtonURL { get; set; }
        public string AuditTrailButtonURL { get; set; }
        public StatusType CurrentStatus { get; set; }
        public TModel ModelObj { get; set; }
        public List<TListModel> ModelList { get; set; }
        public List<TListModel> ModelListP { get; set; }
        public List<TListModel> ModelListY { get; set; }
        public string AT_TABLE { get; set; }
        public string AT_VALUE { get; set; }
        public string AT_TITLE { get; set; }
        public string AT_TITLE2 { get; set; }
        public string AT_VIEW { get; set; }
        public string AT_CONTROLLER { get; set; }
    }
}
