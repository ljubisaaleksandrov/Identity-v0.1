using AjdemeSi.Models;
using AjdemeSi.Domain;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Constants = AjdemeSi.Models.Constants;
using Microsoft.AspNet.Identity.EntityFramework;
using AjdemeSi.Services.Interfaces.Identity;
using System;
using AutoMapper;
using AjdemeSi.Domain.Models.Identity;
using System.Collections.Generic;

namespace AjdemeSi.Areas.Administration.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private DataContext _dataContext;

        private readonly IMapper _mapper;
        private readonly IAspNetUsersService _aspNetUsersService;

        //public ContactsController() { }

        public HomeController(IAspNetUsersService aspNetUsersService, IMapper mapper)
        {
            _mapper = mapper;
            _aspNetUsersService = aspNetUsersService;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }


        [Authorize]
        public ViewResult Index()
        {
            IdentityUserPagedListViewModel pagedListModel = _aspNetUsersService.GetAll(DateTime.MinValue, DateTime.MaxValue);
            pagedListModel.ItemController = "~/areas/administration/Home";

            return View(pagedListModel);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult ApplyFilters(string sortOrder, string searchString, string dateFrom, string dateTo, int pageNumber = 1, int pageSize = 10, List<string> selectedUserRoles = null, bool confirmedOnly = false)
        {
            IdentityUserPagedListViewModel pagedListModel = _aspNetUsersService.GetAll(!String.IsNullOrEmpty(dateFrom) ? DateTime.Parse(dateFrom.Substring(4, 11)) : DateTime.MinValue, !String.IsNullOrEmpty(dateTo) ? DateTime.Parse(dateTo.Substring(4, 11)) : DateTime.MaxValue, sortOrder, searchString, pageNumber, pageSize, selectedUserRoles != null ? selectedUserRoles : new List<string>(), confirmedOnly);
            return PartialView("~/Areas/Administration/Views/Home/IndexData.cshtml", pagedListModel);
        }

        public ActionResult Details(string id)
        {
            if (String.IsNullOrEmpty(id))
                return View();

            var contact = _aspNetUsersService.GetUser(id);
            var model = _mapper.Map<IdentityUserViewModel>(contact);
            return View(model);
        }

        public ActionResult Edit(string id)
        {
            if (String.IsNullOrEmpty(id))
                return View();

            var contact = _aspNetUsersService.GetUser(id);
            var model = _mapper.Map<IdentityUserViewModel>(contact);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, IdentityUserViewModel editModel)
        {
            if (!ModelState.IsValid)
                return View(editModel);

            var contact = _aspNetUsersService.GetUser(id);
            if (contact != null)
            {
                var isAuthorized = true;
                if (!isAuthorized)
                    return View();

                contact = _mapper.Map<AspNetUser>(editModel);
                bool result = _aspNetUsersService.UpdateUser(contact);
            }

            return RedirectToAction("Index");
        }

        public ActionResult Delete(string id)
        {
            if (String.IsNullOrEmpty(id))
                return View();

            var result = _aspNetUsersService.RemoveUser(id);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmEmail(string id, string email)
        {
            if (String.IsNullOrEmpty(id))
                return View();

            _aspNetUsersService.ConfirmEmail(id, email);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BlockUser(string id, string email)
        {
            if (String.IsNullOrEmpty(id))
                return View();

            _aspNetUsersService.BlockUser(id, email);

            return RedirectToAction("Index");
        }

        private bool ContactExists(string id, string userName, string email)
        {
            return _aspNetUsersService.ContactExists(id, userName, email);
        }

    }
}