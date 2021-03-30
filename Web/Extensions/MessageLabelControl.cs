using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace Web.Extensions
{
    public static class MessageLabelControl
    {
        public enum MessageType { info, danger, success, warning };
        public static MvcHtmlString MessageLabel(this HtmlHelper htmlHelper)
        {
            string htmlContent = "";
            string _type = MessageType.info.ToString();
            if (!String.IsNullOrEmpty(htmlHelper.ViewBag.MessageText))
            {
                if (htmlHelper.ViewBag.MessageType != null)
                    _type = htmlHelper.ViewBag.MessageType.ToString();
                HtmlGenericControl control = new HtmlGenericControl("div");
                control.InnerHtml = "<button class='close' data-dismiss='alert'>&times;</button>" + htmlHelper.ViewBag.MessageText;
                control.Attributes.Add("class", "alert alert-" + _type.ToString());
                StringWriter sw = new StringWriter();
                HtmlTextWriter hw = new HtmlTextWriter(sw);
                control.RenderControl(hw);
                htmlContent = sw.ToString();
            }
            return MvcHtmlString.Create(htmlContent);
        }

        public static void AddRegisterClientsSript(this HtmlHelper helper, string key, string script)
        {
            Dictionary<string, string> scripts;
            if (helper.ViewBag.RegisterClientsSript != null)
            {
                scripts = (Dictionary<string, string>)helper.ViewBag.RegisterClientsSript;
            }
            else
            {
                scripts = new Dictionary<string, string>();
                helper.ViewBag.RegisterClientsSript = scripts;
            }

            if (!scripts.ContainsKey(key))
            {
                scripts.Add(key, script);
            }
        }

        public static MvcHtmlString RegisterClientsSript(this HtmlHelper helper)
        {
            var outScripts = new StringBuilder();

            if (helper.ViewBag.RegisterClientsSript != null)
            {
                var scripts = (Dictionary<string, string>)helper.ViewBag.RegisterClientsSript;
                foreach (string script in scripts.Values)
                {
                    outScripts.AppendLine(script);
                }
            }

            return MvcHtmlString.Create(outScripts.ToString());
        }
    }

}