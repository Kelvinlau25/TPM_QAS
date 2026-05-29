using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static TPM_QAS.Enum.EnumType;

namespace TPM_QAS.Models
{
    /// <summary>
    /// Azham 18/1/2023
    /// CommonModel 
    /// require two model, both of this model can be same or difference
    /// depending on situation
    /// 1. TListModel = Listing model
    /// 2. TModel = Create,Edit,View model
    /// CommonModel require constructor for first time initialize
    /// controller = current controller
    /// title = title of the module
    /// Create = allow user the see button create or not
    /// Export = allow user to see button export or not
    /// Delete = allow user to see button delete or not
    /// Save = allow user to button save or not
    /// userID = current login user ID
    /// </summary>
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

        // Parameterless constructor
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