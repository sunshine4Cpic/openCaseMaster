using System.Collections.Generic;
using System.Web;
using System.Web.Optimization;

namespace openCaseMaster
{
    public class BundleConfig
    {
        // 有关绑定的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // 使用要用于开发和学习的 Modernizr 的开发版本。然后，当你做好
            // 生产准备时，请使用 http://modernizr.com 上的生成工具来仅选择所需的测试。
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                       "~/Scripts/public.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));


            bundles.Add(new ScriptBundle("~/bundles/easyui").Include(
                      "~/Scripts/jquery.easyui-{version}.js"));

            bundles.Add(new StyleBundle("~/Content/easyui").Include(
                      "~/Content/themes/bootstrap/easyui.css", "~/Content/themes/icon.css"));





            bundles.Add(new StyleBundle("~/Content/uploader").Include(
                      "~/Scripts/plupload/jquery.ui.plupload/css/jquery.ui.plupload.css", "~/Scripts/plupload/jquery-ui.css"));



            ScriptBundle uploader = new ScriptBundle("~/bundles/uploader");
            uploader.Orderer = new AsIsBundleOrderer();
            uploader.Include(
                "~/Scripts/plupload/jquery-ui.js",
                    "~/Scripts/plupload/plupload.full.min.js",
                    "~/Scripts/plupload/jquery.ui.plupload/jquery.ui.plupload.js",
                    "~/Scripts/plupload/i18n/zh_CN.js"
                );
            bundles.Add(uploader);



            bundles.Add(new ScriptBundle("~/bundles/bootstrap-select").Include(
                       "~/Scripts/bootstrap-select.js"));

            bundles.Add(new StyleBundle("~/Content/bootstrap-select").Include(
                       "~/Content/bootstrap-select.css"));


            bundles.Add(new ScriptBundle("~/bundles/markdownView").Include(
                       "~/Scripts/editormd/lib/marked.min.js",
                       "~/Scripts/editormd/lib/prettify.min.js",
                       "~/Scripts/editormd/lib/raphael.min.js",
                       "~/Scripts/editormd/lib/underscore.min.js",
                       "~/Scripts/editormd/lib/sequence-diagram.min.js",
                       "~/Scripts/editormd/lib/flowchart.min.js",
                       "~/Scripts/editormd/lib/jquery.flowchart.min.js",
                       "~/Scripts/editormd.js"));

            bundles.Add(new StyleBundle("~/Content/markdownView").Include(
                       "~/Content/editormd.preview.css",
                       "~/Content/editormd.css"));



            BundleTable.EnableOptimizations = false;//不启用压缩
        }
    }
    /// <summary>
    /// 文件顺序加载
    /// </summary>
    internal class AsIsBundleOrderer : IBundleOrderer
    {
        public virtual IEnumerable<BundleFile> OrderFiles(BundleContext context, IEnumerable<BundleFile> files)
        {
            return files;
        }
    }
}
