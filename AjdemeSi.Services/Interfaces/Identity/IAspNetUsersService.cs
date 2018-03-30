using AjdemeSi.Domain;
using AjdemeSi.Domain.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AjdemeSi.Services.Interfaces.Identity
{
    public interface IAspNetUsersService
    {
        List<AspNetUser> GetAll();
        IdentityUserPagedListViewModel GetAll(DateTime dateFrom, DateTime dateTo, string sortOrder = null, string searchString = null, int pageIndex = 1, int countOnPage = 10, List<string> userRoles = null, bool confirmedOnly = true);
        AspNetUser GetUser(string id);
        bool UpdateUser(AspNetUser user);
        bool ConfirmEmail(string id, string email);
        bool BlockUser(string id, string email);
        bool ContactExists(string id, string userName, string email);
        bool RemoveUser(string id);
    }
}
