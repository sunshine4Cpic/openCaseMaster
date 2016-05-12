using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace System.Web.Mvc
{
    public static class HtmlExtensions
    {
        public static HtmlString Pagination(this HtmlHelper helper, int page, int row=0)
        {

            int prev = page > 1 ? page - 1 : 1;
            int next = page + 1;


            StringBuilder sb = new StringBuilder();
            sb.Append("<li class=\"prev previous_page\"><a rel=\"prev\" href=\"?page=" + prev + "\">← 上一页</a></li>");

            if(page==1)
                sb.Append("<li class=\"active\"><a rel=\"start\" href=\"?page=1\">1</a></li>");
            else
                sb.Append("<li><a rel=\"start\" href=\"?page=1\">1</a></li>");

            if (page == 2)
                sb.Append("<li class=\"active\"><a href=\"?page=2\">2</a></li>");
            else
                sb.Append("<li><a href=\"?page=2\">2</a></li>");

         

            int start = page - 2;

            if (start > 3)
                sb.Append("<li class=\"disabled\"><a href=\"#\">…</a></li>");

            for (int i = 0; i < 5; i++)
            {
                int pg = start + i;
                if (pg > 2)
                {
                    if (pg==page)
                        sb.Append("<li class=\"active\"><a href=\"?page=" + pg + "\">" + pg + "</a></li>");
                    else
                        sb.Append("<li><a href=\"?page=" + pg + "\">" + pg + "</a></li>");
                }
            }

            sb.Append("<li class=\"disabled\"><a href=\"#\">…</a></li>");

            sb.Append("<li><a href=\"?page=199\">199</a></li>");
            sb.Append("<li><a href=\"?page=200\">200</a></li>");


            sb.Append("<li class=\"next next_page\"><a rel=\"next\" href=\"?page=" + next + "\">下一页 →</a></li>");

            return new HtmlString(sb.ToString());
        }


        
    }

    

    
   
}