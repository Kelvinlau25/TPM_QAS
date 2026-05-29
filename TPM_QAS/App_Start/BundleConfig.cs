using System.Web;
using System.Web.Optimization;

namespace TPM_QAS
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                       "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new Bundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.bundle.js",
                      "~/Scripts/popper.min.js"));

            bundles.Add(new StyleBundle("~/Content/css-login").Include(
                      "~/Content/bootstrap.css",
                      "~/Plugins/fontawesome/css/fontawesome.min.css",
                      "~/Plugins/fontawesome/css/brands.min.css",
                      "~/Plugins/fontawesome/css/solid.min.css",
                      "~/Plugins/fontawesome/css/regular.min.css",
                      "~/Plugins/datetimepicker-master/build/jquery.datetimepicker.min.css",
                      //"~/Plugins/loading-spinner-plugin/waitMe.min.css",
                      "~/Content/Site-login.css"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Plugins/fontawesome/css/fontawesome.min.css",
                      "~/Plugins/fontawesome/css/brands.min.css",
                      "~/Plugins/fontawesome/css/solid.min.css",
                      "~/Plugins/fontawesome/css/regular.min.css",
                      "~/Plugins/DataTables/datatables.min.css",                      
                      "~/Plugins/DataTables/DataTables-1.13.1/css/dataTables.bootstrap5.min.css",
                      "~/Plugins/bootstrap-select-main/docs/docs/dist/css/bootstrap-select.min.css",
                      "~/Plugins/bootstrap-multiselect-master/dist/css/bootstrap-multiselect.min.css",
                      "~/Plugins/jquery-datatables-checkboxes-1.2.12/css/dataTables.checkboxes.css",
                      "~/Plugins/datetimepicker-master/build/jquery.datetimepicker.min.css",
                      "~/Plugins/file-input-js/css/fileinput.min.css",
                      "~/Plugins/select2-4.1.0-rc.0/dist/css/select2.min.css",
                      "~/Plugins/select2-4.1.0-rc.0/dist/css/select2-bootstrap-5-theme.min.css",
                      //"~/Plugins/loading-spinner-plugin/waitMe.min.css",
                      "~/Plugins/rangeslider/dist/rangeslider.css",
                      "~/Scripts/vendor/bootstrap-datepicker/css/bootstrap-datepicker.min.css",
                      "~/Content/site.css"
            ));
            bundles.Add(new StyleBundle("~/Content/fontawesome").Include(
                      "~/Content/font-awesome.css",
                      "~/Plugins/fontawesome/css/fontawesome.min.css",
                      "~/Plugins/fontawesome/css/brands.min.css",
                      "~/Plugins/fontawesome/css/solid.min.css",
                      "~/Plugins/fontawesome/css/regular.min.css"
                      ));
            bundles.Add(new Bundle("~/bundles/datatables").Include(
                "~/Plugins/DataTables/datatables.min.js",
                "~/Plugins/DataTables/DataTables-1.13.1/js/dataTables.bootstrap5.min.js",                
                "~/Plugins/DataTables/Select-1.5.0/js/dataTables.select.min.js",
                "~/Plugins/jquery-datatables-checkboxes-1.2.12/js/dataTables.checkboxes.min.js",
                "~/Plugins/bootstrap-select-main/docs/docs/dist/js/bootstrap-select.min.js"            
            ));

            bundles.Add(new Bundle("~/bundles/js").Include(
                "~/Plugins/datetimepicker-master/build/jquery.datetimepicker.full.min.js",
                "~/Plugins/jquery-loading-overlay-master/dist/loading-overlay.min.js",
                "~/Plugins/bootstrap-multiselect-master/dist/js/bootstrap-multiselect.min.js",
                "~/Plugins/file-input-js/js/fileinput.min.js",
                "~/Scripts/vendor/bootstrap-datepicker/js/bootstrap-datepicker.min.js",
                "~/Plugins/select2-4.1.0-rc.0/dist/js/select2.min.js",
                //"~/Plugins/loading-spinner-plugin/waitMe.js",
                "~/Plugins/rangeslider/dist/rangeslider.min.js"
            ));

            bundles.Add(new Bundle("~/bundles/vendorscripts.bundle.js").Include(
               //"~/Scripts/vendor/metisMenu/metisMenu.js",
               "~/Scripts/vendor/bootstrap-progressbar/js/bootstrap-progressbar.min.js",
               "~/Scripts/vendor/jquery-sparkline/js/jquery.sparkline.min.js"
           ));

            bundles.Add(new Bundle("~/bundles/mainscripts.bundle.js").Include(
                "~/Scripts/js/common.js"
            ));

            bundles.Add(new Bundle("~/bundles/AIGraph").Include(
                "~/Scripts/js/canvasjs/jquery.canvasjs.min.js",
                "~/Scripts/d3.min.js",
                "~/Scripts/c3.min.js",
                "~/Plugins/html2canvas/js/html2canvas.min.js"
            ));
        }
    }
}
