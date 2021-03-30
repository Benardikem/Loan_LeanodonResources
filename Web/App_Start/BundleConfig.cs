using System.Web;
using System.Web.Optimization;

namespace Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/js/jquery").Include(
                        "~/lib/jquery/js/jquery.js"));
            //bundles.Add(new ScriptBundle("~/bundles/jquery-validate").Include(
            //            "~/lib/jquery.validate*"));
            bundles.Add(new ScriptBundle("~/js/jquery-ui").Include(
                       "~/lib/jquery-ui/js/jquery-ui.js"));

            //Bootstrap
            bundles.Add(new ScriptBundle("~/js/bootstrap").Include(
                      "~/lib/bootstrap/js/bootstrap.js",
                      "~/lib/bootstrap-tagsinput/js/bootstrap-tagsinput.js"));
            //Session parameters
            bundles.Add(new ScriptBundle("~/js/session").Include(
                      "~/js/session-param.js"));
            //ResizeSensor
            bundles.Add(new ScriptBundle("~/js/ResizeSensor").Include(
                      "~/js/ResizeSensor.js"));

            //Layout
            bundles.Add(new ScriptBundle("~/js/Application").Include(
                      "~/js/Application.js"));
            //Popper
            bundles.Add(new ScriptBundle("~/js/popper").Include(
                      "~/lib/popper.js/js/popper.js"));

            //jquery.cookie
            bundles.Add(new ScriptBundle("~/js/jquery.cookie").Include(
                      "~/lib/jquery.cookie/js/jquery.cookie.js"));
            //bootbox
            bundles.Add(new ScriptBundle("~/js/bootbox").Include(
                      "~/lib/bootbox/js/bootbox.min.js"));

            //datatables
            bundles.Add(new ScriptBundle("~/js/datatables").Include(
                                       "~/lib/datatables/js/jquery.dataTables.js",
                                       "~/lib/datatables-responsive/js/dataTables.responsive.js"));
            //bundles.Add(new ScriptBundle("~/js/datatables").Include(
            //                           "~/lib/datatables/js/jquery.dataTables.js"));
            ////datepicker
            //bundles.Add(new ScriptBundle("~/bundles/datepicker").Include(
            //                             "~/Scripts/datepicker/bootstrap-datepicker.js",
            //                             "~/Scripts/datepicker/locales/bootstrap-datepicker*"));
            //Select2
            bundles.Add(new ScriptBundle("~/js/select2").Include(
                                         "~/lib/select2/js/select2.min.js"));

            //moment 
            bundles.Add(new ScriptBundle("~/js/moment").Include(
                                         "~/lib/moment/js/moment.js"));

            //jquery.steps 
            bundles.Add(new ScriptBundle("~/js/jquery.steps").Include(
                                         "~/lib/jquery.steps/js/jquery.steps.js"));

            //parsleyjs 
            bundles.Add(new ScriptBundle("~/js/parsleyjs").Include(
                                         "~/lib/parsleyjs/js/parsley.js"));

            //**************************CSS
            bundles.Add(new StyleBundle("~/css/font-awesome").Include(
                     "~/lib/font-awesome/css/font-awesome.css"));

            bundles.Add(new StyleBundle("~/css/ionicons").Include(
                    "~/lib/Ionicons/css/ionicons.css"));

            bundles.Add(new StyleBundle("~/css/Application").Include(
                    "~/css/Application.css"));

            bundles.Add(new StyleBundle("~/css/datatables").Include(
                    "~/lib/datatables/css/jquery.dataTables.css"));

            bundles.Add(new StyleBundle("~/css/select2").Include(
                  "~/lib/select2/css/select2.min.css"));
            bundles.Add(new StyleBundle("~/css/spinkit").Include(
                  "~/lib/SpinKit/css/spinkit.css"));

            bundles.Add(new StyleBundle("~/css/jquery.steps").Include(
                             "~/lib/jquery.steps/css/jquery.steps.css"));


            //bundles.Add(new ScriptBundle("~/Content/dataTables-checkboxes").Include(
            //         "~/Content/dataTables.checkboxes.css"));
            //bundles.Add(new StyleBundle("~/Content/css").Include(
            //          "~/Content/font-awesome.min.css",
            //          "~/Content/bootstrap.css",
            //          "~/Content/datepicker3.css",
            //          "~/Content/dataTables.bootstrap.min.css",
            //          "~/Content/jquery.dataTables.min.css",
            //          "~/Content/metisMenu.min.css",
            //          "~/Content/site.css"));
            //bundles.Add(new ScriptBundle("~/Content/jqueryui").Include(
            //          "~/Content/jquery-ui.css"));
            //bundles.Add(new ScriptBundle("~/Content/roles").Include(
            //          "~/Content/roles.css"));
        }
    }
}
