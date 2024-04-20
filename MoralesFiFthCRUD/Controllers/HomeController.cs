﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MoralesFiFthCRUD.Controllers
{

    public class HomeController : BaseController
    {
        // GET: Home

        public ActionResult Index()
        {
            return View(_userRepo.GetAll());
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Login");
            return View();
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(User u)
        {
            var user = _userRepo._table.Where(m => m.username == u.username).FirstOrDefault();
            if (user != null)
            {
                FormsAuthentication.SetAuthCookie(u.username, false);
                return RedirectToAction("Dashboard");
            }
            ModelState.AddModelError("", "User not Exist or Incorrect Password");

            return View(u);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(User u, string SelectedRole)
        {
            _userRepo.Create(u);

            var userAdded = _userRepo._table.FirstOrDefault(m => m.username == u.username);

            if (userAdded == null)
            {
                // Handle case where user creation failed
                ModelState.AddModelError("", "Failed to create user.");
                return View(u); // Redisplay the form with an error message
            }

            if (string.IsNullOrEmpty(SelectedRole))
            {
                // Handle case where role is not selected
                ModelState.AddModelError("", "Role not selected.");
                return View(u); // Redisplay the form with an error message
            }

            var role = _db.Roles.FirstOrDefault(r => r.roleName == SelectedRole);

            if (role == null)
            {
                // Handle case where role is not found (invalid selection)
                ModelState.AddModelError("", "Invalid role selected.");
                return View(u); // Redisplay the form with an error message
            }

            var userRole = new UserRole
            {
                userId = userAdded.id,
                roleId = role.id // Assign the retrieved roleId
            };

            _userRole.Create(userRole);

            TempData["Msg"] = $"User {u.username} added!";
            return RedirectToAction("LandingPage");
        }
        [Authorize(Roles = "Tutor")]
        public ActionResult Details(int id)
        {
            return View(_userRepo.Get(id));
        }
        [Authorize(Roles = "Tutor")]
        public ActionResult Edit(int id)
        {

            return View(_userRepo.Get(id));
        }
        [HttpPost]
        public ActionResult Edit(User u)
        {
            _userRepo.Update(u.id, u);
            TempData["Msg"] = $"User {u.username} updated!";

            return RedirectToAction("index");

        }
        [Authorize(Roles = "Manager")]
        public ActionResult Delete(int id)
        {
            _userRepo.Delete(id);
            TempData["Msg"] = $"User deleted!";
            return RedirectToAction("index");
        }
        public ActionResult LandingPage()
        {
            return View();
        }

        public ActionResult Dashboard()
        {
            return View();
        }
        public ActionResult profileclient()
        {
            return View();
        }
        public ActionResult profiletutor()
        {
            return View();
        }
        public ActionResult About()
        {
            return View();
        }
        public ActionResult ContactUs()
        {
            return View();
        }
    }
}