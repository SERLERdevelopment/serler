﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dapper;
using Serler.Models;
using System.Data.SqlClient;

namespace Serler.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        //
        // GET: /Admin/

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ModifyUser(int id)
        {
            using (var conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Serler"].ConnectionString))
            {
                var query = "select * from Users where UserId = @UserId";
                conn.Open();
                var model = conn.Query<UserViewModel>(query, new { UserId = id }).FirstOrDefault();
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ModifyUser(UserViewModel model)
        {
            return View(model);
        }

        public ActionResult AllUsers(string searchText, int? page)
        {
            if (page == null || page < 0)
            {
                page = 1;
            }

            if (string.IsNullOrEmpty(searchText))
            {
                searchText = "";
            }
            var model = new PagedViewModel<UserViewModel>();

            using (var conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Serler"].ConnectionString))
            {
                var countQuery = "select COUNT(UserId) from Users where Email like @searchText;";
                var query = string.Format("select * from Users where IsActive = 1 order by Email;");
                conn.Open();
                model.TotalCount = conn.Query<int>(countQuery, new { searchText = "%" + searchText + "%" }).FirstOrDefault();
                model.SearchModel = new SearchModel
                {
                    SearchText = "%" + searchText + "%",
                    Take = 10
                };
                model.SearchModel.Skip = (page.Value - 1) * model.SearchModel.Take;

                var member = conn.Query<UserViewModel>(query).ToList();
                if(member != null)
                {
                    model.Result = member;
                }
                var maxPage = Math.Ceiling((double)model.TotalCount / model.SearchModel.Take);

                var pagination1 = new PaginationModel();
                pagination1.DisplayLabel = "<<";
                pagination1.Url = "~/AllUsers?page=1";
                pagination1.IsEnable = page <= 1 ? false : true;

                var pagination2 = new PaginationModel();
                pagination2.DisplayLabel = "<";
                pagination2.Url = "~/AllUsers?page=" + (page - 1);
                pagination2.IsEnable = page <= 1 ? false : true;

                var pagination3 = new PaginationModel();
                pagination3.DisplayLabel = ">";
                pagination3.Url = "~/AllUsers?page=" + (page + 1);
                pagination3.IsEnable = page >= maxPage ? false : true;

                var pagination4 = new PaginationModel();
                pagination4.DisplayLabel = ">>";
                pagination4.Url = "~/AllUsers?page=" + (page - 1);
                pagination4.IsEnable = page >= maxPage ? false : true;

                model.PaginationModel = new List<PaginationModel>();
                model.PaginationModel.Add(pagination1);
                model.PaginationModel.Add(pagination2);

                for (int i = 0; i < maxPage; i++)
                {
                    var paginationModel = new PaginationModel();
                    paginationModel.DisplayLabel = i + 1 + "";
                    paginationModel.Url = "~/AllUsers?page=" + (i + 1);
                    paginationModel.IsEnable = true;
                    if (i + 1 == page)
                    {
                        paginationModel.IsActive = true;
                    }
                    model.PaginationModel.Add(paginationModel);
                }

                model.PaginationModel.Add(pagination3);
                model.PaginationModel.Add(pagination4);

                return View(model);
            }
        }

        public ActionResult PendingUser()
        {
            using (var conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Serler"].ConnectionString))
            {
                var query = string.Format("select * from Users where IsPendingUser = 1 and IsActive = 1 order by Email;");
                conn.Open();
                var member = conn.Query<UserViewModel>(query).ToList();
                return View(member);
            }
        }

        public ActionResult SystemAdmin()
        {
            using (var conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Serler"].ConnectionString))
            {
                var query = string.Format("select * from Users where IsSystemAdmin = 1 and IsActive = 1 order by Email;");
                conn.Open();
                var member = conn.Query<UserViewModel>(query).ToList();
                return View(member);
            }
        }

        public ActionResult Moderator()
        {
            using (var conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Serler"].ConnectionString))
            {
                var query = string.Format("select * from Users where IsModerator = 1 and IsActive = 1 order by Email;");
                conn.Open();
                var member = conn.Query<UserViewModel>(query).ToList();
                return View(member);
            }
        }

        public ActionResult Analyst()
        {
            using (var conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Serler"].ConnectionString))
            {
                var query = string.Format("select * from Users where IsAnalyst = 1 and IsActive = 1 order by Email;");
                conn.Open();
                var member = conn.Query<UserViewModel>(query).ToList();
                return View(member);
            }
        }
    }
}
