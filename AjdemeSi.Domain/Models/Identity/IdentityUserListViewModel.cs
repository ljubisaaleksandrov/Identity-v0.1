using AutoMapper;
using PagedList;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;

namespace AjdemeSi.Domain.Models.Identity
{
    public class IdentityUserListViewModel
    {
        public IdentityUserPagedListViewModel PagedListModel { get; set; }
    }

    public class IdentityUserPagedListViewModel : PagedList<AspNetUser>
    {
        public IEnumerable<IdentityUserViewModel> ListEntries { get; set; }
        public DateTime DateFrom;
        public DateTime DateTo;
        public string ItemController { get; set; }
        public List<SelectListItem> CountsList { get; set; }
        public List<SelectListItem> UserRoles { get; set; }

        public IdentityUserPagedListViewModel(IQueryable<AspNetUser> superset, 
                                              int pageNumber, 
                                              int pageSize, 
                                              int totalItemsCount, 
                                              DateTime newestUserCreationDate, 
                                              DateTime oldestUserCreationDate,
                                              List<string> userRolesExisting,
                                              List<string> userRolesSelected,
                                              IMapper mapper)
        : base(superset, pageNumber, pageSize)
        {
            ListEntries = mapper.Map<List<IdentityUserViewModel>>(superset.ToList());
            TotalItemCount = totalItemsCount;
            PageCount = totalItemsCount % pageSize == 0 ? totalItemsCount / pageSize : totalItemsCount / pageSize + 1;
            IsLastPage = PageCount == pageNumber;
            HasNextPage = !IsLastPage;
            HasPreviousPage = pageNumber != 1;
            DateFrom = oldestUserCreationDate;
            DateTo = newestUserCreationDate;

            CountsList = new List<SelectListItem>()
                               {
                                   new SelectListItem() { Selected = PageCount == 10, Text = "10", Value = "10" },
                                   new SelectListItem() { Selected = PageCount == 20, Text = "20", Value = "20" },
                                   new SelectListItem() { Selected = PageCount == 50, Text = "50", Value = "50" },
                                   new SelectListItem() { Selected = PageCount == 100, Text = "100", Value = "100" }
                               };

            UserRoles = new List<SelectListItem>();
            if(userRolesExisting != null)
                foreach (string existingRole in userRolesExisting)
                    UserRoles.Add(new SelectListItem() { Selected = userRolesSelected.Contains(existingRole), Text = existingRole, Value = existingRole });
        }

        public IdentityUserPagedListViewModel(IEnumerable<AspNetUser> superset, 
                                              int pageNumber, 
                                              int pageSize, 
                                              int totalItemsCount, 
                                              DateTime newestUserCreationDate, 
                                              DateTime oldestUserCreationDate,
                                              List<string> userRolesExisting,
                                              List<string> userRolesSelected,
                                              IMapper mapper)
        : this(superset.AsQueryable<AspNetUser>(), pageNumber, pageSize, totalItemsCount, newestUserCreationDate, oldestUserCreationDate, userRolesExisting, userRolesSelected, mapper)
        { }
    }
}
