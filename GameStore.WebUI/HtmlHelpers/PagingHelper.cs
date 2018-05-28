using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GameStore.WebUI.Models;
using System.Text;

namespace GameStore.WebUI.HtmlHelpers {
    public static class PagingHelper {
        public static MvcHtmlString PageLinks(this HtmlHelper html, PagingInfo pagingInfo, Func<int, string> pageUrl) {
            StringBuilder result = new StringBuilder();

            for (int i = 1; i <= pagingInfo.TotalPages; i++) {
                TagBuilder tagBuilder = new TagBuilder("a");
                tagBuilder.MergeAttribute("href", pageUrl(i));
                tagBuilder.InnerHtml = i.ToString();
                if (i == pagingInfo.CurrentPage) {
                    tagBuilder.AddCssClass("btn-primary selected");
                }
                tagBuilder.AddCssClass("btn btn-default");
                result.Append(tagBuilder);
            }

            return MvcHtmlString.Create(result.ToString());
        }
    }
}