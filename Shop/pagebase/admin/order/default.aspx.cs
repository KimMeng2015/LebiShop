﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Shop.Bussiness;
using Shop.Model;
using Shop.Tools;

namespace Shop.Admin.order
{
    public partial class Default : AdminPageBase
    {
        protected List<Lebi_Order> models;
        protected string PageString;
        protected int t;
        protected string mark;
        protected string type;
        protected string key;
        protected string dateFrom;
        protected string dateTo;
        protected int Supplier_id;
        protected int user_id;
        protected int product_type_id = 0;//产品分类ID
        protected SearchOrder su;
        protected void Page_Load(object sender, EventArgs e)
        {
            type = RequestTool.RequestString("type");
            key = RequestTool.RequestString("key");
            dateFrom = RequestTool.RequestString("dateFrom");
            dateTo = RequestTool.RequestString("dateTo");
            t = RequestTool.RequestInt("t", 211);
            mark = RequestTool.RequestString("mark");
            DateTime lbsql_dateFrom = RequestTool.RequestDate("dateFrom");
            DateTime lbsql_dateTo = RequestTool.RequestDate("dateTo");
            su = new SearchOrder(CurrentAdmin, CurrentLanguage.Code);
            if (t == 212)
            {
                if (!EX_Admin.Power("order_return_list", "退货订单列表"))
                {
                    PageReturnMsg = PageNoPowerMsg();
                }
            }
            else
            {
                if (!EX_Admin.Power("order_list", "订单列表"))
                {
                    PageReturnMsg = PageNoPowerMsg();
                }
            }
            
            string where = "(IsDel!=1 or IsDel is null) and Type_id_OrderType=" + t + su.SQL;
            if (key != "")
                where += " and (Code like lbsql{'%" + key + "%'} or User_UserName like lbsql{'%" + key + "%'} or T_Name like lbsql{'%" + key + "%'} or Transport_Name like lbsql{'%" + key + "%'})";
            if (dateFrom != "" && dateTo != "")
                where += " and (datediff(d,Time_Add,'" + FormatDate(lbsql_dateFrom) + "')<=0 and datediff(d,Time_Add,'" + FormatDate(lbsql_dateTo) + "')>=0)";

            switch (type.ToLower())
            {
                case "1":
                    where += " and IsVerified = 0 and IsInvalid = 0";
                    break;
                case "2":
                    where += " and IsVerified = 1 and IsInvalid = 0";
                    break;
                case "3":
                    where += " and IsVerified = 1 and IsPaid = 0 and IsInvalid = 0";
                    break;
                case "4":
                    where += " and IsPaid = 1 and IsShipped = 0 and IsInvalid = 0";
                    break;
                case "5":
                    where += " and IsVerified = 1 and IsShipped = 0 and IsInvalid = 0";
                    break;
                case "6":
                    where += " and IsVerified = 1 and IsShipped_All= 1 and IsInvalid = 0";
                    break;
                case "7":
                    where += " and IsVerified = 1 and IsShipped = 1 and IsShipped_All= 0 and IsInvalid = 0";
                    break;
                case "8":
                    where += " and IsVerified = 1 and IsReceived = 0 and IsInvalid = 0";
                    break;
                case "9":
                    where += " and IsVerified = 1 and IsReceived = 1 and IsInvalid = 0";
                    break;
                case "10":
                    where += " and IsVerified = 1 and IsCompleted = 1 and IsInvalid = 0";
                    break;
                case "11":
                    where += " and IsInvalid = 1";
                    break;
                case "12":
                    where += " and IsRefund = 2";
                    break;
                case "13":
                    where += " and IsRefund = 1";
                    break;
            }
            if (mark != "")
                where += " and Mark = " + int.Parse(mark);
            //if (Supplier_id > 0)
            //    where += " and Supplier_id = " + Supplier_id;
            //if (user_id > 0)
            //    where += " and User_id = " + user_id;
            if (domain3admin && CurrentAdmin.Site_ids != "")
            {
                where += " and (Site_id in (" + CurrentAdmin.Site_ids + ") or Site_id=0)";
            }


            PageSize = RequestTool.getpageSize(25);
            //if (product_id > 0)
            //{
            //    Lebi_Product pro = B_Lebi_Product.GetModel(product_id);
            //    if (pro != null)
            //    {
            //        if (pro.Product_id == 0)
            //        {
            //            string pids = "";
            //            List<Lebi_Product> ps = B_Lebi_Product.GetList("Product_id=" + product_id + "", "");
            //            if (ps.Count > 0)
            //            {
            //                foreach (Lebi_Product p in ps)
            //                {
            //                    pids += p.id + ",";
            //                }
            //                pids = pids.TrimEnd(',');
            //                where += " and id in (select Order_id from Lebi_Order_Product where Product_id in (" + pids + "))";
            //            }
            //            else
            //            {
            //                where += " and id in (select Order_id from Lebi_Order_Product where Product_id=" + product_id + ")";
            //            }
            //        }
            //        else
            //        {
            //            where += " and id in (select Order_id from Lebi_Order_Product where Product_id=" + product_id + ")";
            //        }
            //    }
            //}
            models = B_Lebi_Order.GetList(where, "id desc", PageSize, page);
            int recordCount = B_Lebi_Order.Counts(where);
            PageString = Pager.GetPaginationString("?page={0}&t=" + t + "&type=" + type + "&mark=" + mark + "&dateFrom=" + dateFrom + "&dateTo=" + dateTo + "&key=" + key +"&"+ su.URL, page, PageSize, recordCount);

        }

        public string getstatusicon(int stat)
        {
            string str = "";
            if (stat == 1)
                str = site.AdminImagePath + "icon_yes.gif";
            else
                str = site.AdminImagePath + "icon_no.gif";
            return "<img src=\"" + str + "\" />";
        }


        public List<Lebi_Order_Product> GetOrderProduct(int order_id)
        {
            List<Lebi_Order_Product> ps = B_Lebi_Order_Product.GetList("Order_id=" + order_id + "", "");
            return ps;
        }
    }
}