using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Runtime.Serialization;
using System.Data;

namespace TPM_QAS.Models
{
    public class COAGARAWMATABNORMModel
    {
        public int GA_ABNORMAL_H_ID { get; set; }
        public string SUPPLIER_NAME { get; set; }
        public string MATERIAL_NAME { get; set; }
        public string SUPP_ITEM_NAME { get; set; }
        public string ABNORM_STATUS { get; set; }
        public string TPM_CODE { get; set; }

        public string RECORD_TYP { get; set; }
        public string REC_TYPE_DESC { get; set; }
        public string SEARCH_VALUE { get; set; }
        public string CREATED_BY { get; set; }
        public string CREATED_DATE { get; set; }
        public string CREATED_LOC { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_LOC { get; set; }

        public string DATE_FROM { get; set; }
        public string DATE_TO { get; set; }
    }

    public class GraphRpt2
    {
        public int ID_PROD_LOSS { get; set; }
        public string ID_PROD_LINE { get; set; }
        public string YEAR_MONTH { get; set; }
        public string PRODLINE { get; set; }
        public string REASON_CATEGORY { get; set; }
        public decimal TOTAL_LOSS { get; set; }
        public IEnumerable<SelectListItem> Prod_Line { get; set; }
        public List<SelectListItem> DropdownProdLine { get; set; }
        public bool IsEnabledProdLoss { get; set; }
        // Prod Loss Summary
        public List<string> labels { get; set; }
        public List<int> dataAPL { get; set; }
        public List<int> dataAAP { get; set; }
        public List<int> dataATP { get; set; }
        // Marginal Profit Gain/Loss
        public List<int> dataGL { get; set; }
        public bool IsEnabledGainLoss { get; set; }

        // Generation NG & R 
        public bool IsEnabledGenNGR { get; set; }
        public List<int> dataAccNGR { get; set; }
        public List<int> dataNG { get; set; }
        public List<int> dataR { get; set; }
        // Actual Vs Target
        public bool IsEnabledActTgt { get; set; }
        public List<decimal> dataActTgt { get; set; }
        //public List<int> dataAAP { get; set; }
        //public List<int> dataATP { get; set; }

    }

    public class chartinfo
    { 
        public List<DataPoint> DataPoint { get; set; }
        public string OcrResult { get; set; }
        public string Yaxis { get; set; }

    }

    internal class DataContractAttribute : Attribute
    {
    }

    internal class DataMemberAttribute : Attribute
    {
        public string Name { get; set; }
    }

    [DataContract]
    public class DataPoint
    {
        public DataPoint(string label, double y)
        {
            this.label = label;
            this.y = y;
        }

        public DataPoint()
        {
        }

        //public string REASON_CATEGORY { get; set; }
        //public double TOTAL_LOSS { get; set; }
        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "label")]
        public string label = "";

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "y")]
        public Nullable<double> y = null;
    }
}