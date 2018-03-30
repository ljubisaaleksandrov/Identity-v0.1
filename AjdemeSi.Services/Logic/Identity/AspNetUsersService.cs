using AjdemeSi.Domain;
using AjdemeSi.Domain.Models.Identity;
using AjdemeSi.Services.Interfaces.Identity;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace AjdemeSi.Services.Logic.Identity
{
    public class AspNetUsersService : IAspNetUsersService
    {
        private readonly IMapper _mapper;

        public AspNetUsersService()
        {

        }

        public AspNetUsersService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public List<AspNetUser> GetAll()
        {
            using(DataContext dc = new DataContext())
            {
                return dc.AspNetUsers.ToList();
            }
        }

        public IdentityUserPagedListViewModel GetAll(DateTime dateFrom, DateTime dateTo, string sortOrder = null, string searchString = null, int pageIndex = 1, int countOnPage = 10, List<string> selectedUserRoles = null, bool confirmedOnly = true)
        {
            using (DataContext db = new DataContext())
            {
                IQueryable<AspNetUser> users = db.AspNetUsers.Include(u => u.AspNetRoles);
                List<string> existingUserRoles = db.AspNetRoles.Select(x => x.Name).ToList();

                if (selectedUserRoles == null)
                    selectedUserRoles = existingUserRoles;
                else if(selectedUserRoles.Count != existingUserRoles.Count)
                    users = users.Where(u => u.AspNetRoles.Any(ur => selectedUserRoles.Contains(ur.Name)));

                if (!String.IsNullOrEmpty(searchString))
                    users = users.Where(x => x.UserName.Contains(searchString) || x.Email.Contains(searchString));

                if(dateFrom != null && dateTo != null && dateFrom <= dateTo)
                    users = users.Where(u => DbFunctions.TruncateTime(u.DateCreated) >= DbFunctions.TruncateTime(dateFrom) &&
                                             DbFunctions.TruncateTime(u.DateCreated) <= DbFunctions.TruncateTime(dateTo));

                switch (sortOrder)
                {
                    case "registrationDate_asc":
                        users = users.OrderBy(x => x.DateCreated);
                        break;
                    case "username_asc":
                        users = users.OrderBy(x => x.UserName);
                        break;
                    case "username_desc":
                        users = users.OrderByDescending(x => x.UserName);
                        break;
                    case "email_asc":
                        users = users.OrderBy(x => x.Email);
                        break;
                    case "email_desc":
                        users = users.OrderByDescending(x => x.Email);
                        break;
                    case "isConfirmed_asc":
                        users = users.OrderBy(x => x.EmailConfirmed);
                        break;
                    case "isConfirmed_desc":
                        users = users.OrderByDescending(x => x.EmailConfirmed);
                        break;
                    default:
                        users = users.OrderByDescending(x => x.DateCreated);
                        break;
                }

                if (confirmedOnly)
                    users = users.Where(x => x.EmailConfirmed);

                int totalCount = users.Count();
                DateTime oldestUserCreationDate = totalCount > 0 ? users.OrderBy(u => u.DateCreated).FirstOrDefault().DateCreated.Value : DateTime.Now;
                DateTime newestUserCreationDate = totalCount > 0 ? users.OrderByDescending(u => u.DateCreated).FirstOrDefault().DateCreated.Value : DateTime.Now;

                users = users.Skip(countOnPage * (pageIndex - 1)).Take(countOnPage);
                return new IdentityUserPagedListViewModel(users, pageIndex, countOnPage, totalCount, newestUserCreationDate, oldestUserCreationDate, existingUserRoles, selectedUserRoles, _mapper);
            }
        }

        public AspNetUser GetUser(string id)
        {
            using (DataContext dc = new DataContext())
            {
                return dc.AspNetUsers.FirstOrDefault(u => u.Id == id);
            }
        }

        public bool UpdateUser(AspNetUser user)
        {
            using (DataContext dc = new DataContext())
            {
                var currentUser = dc.AspNetUsers.FirstOrDefault(u => u.Id == user.Id);
                if(currentUser != null)
                {
                    currentUser.UserName = user.UserName;
                    currentUser.PhoneNumber = user.PhoneNumber;
                    dc.SaveChanges();
                    return true;
                }

                return false;
            }
        }

        public bool RemoveUser(string id)
        {
            using (DataContext dc = new DataContext())
            {
                var currentUser = dc.AspNetUsers.FirstOrDefault(u => u.Id == id);
                if (currentUser != null)
                {
                    dc.AspNetUsers.Remove(currentUser);
                    dc.SaveChanges();
                    return true;
                }

                return false;
            }
        }

        public bool ContactExists(string id, string userName, string email)
        {
            using (DataContext dc = new DataContext())
            {
                return (!String.IsNullOrEmpty(id) && dc.AspNetUsers.Any(u => u.Id == id)) ||
                       (!String.IsNullOrEmpty(userName) && dc.AspNetUsers.Any(u => u.UserName == userName)) ||
                       (!String.IsNullOrEmpty(email) && dc.AspNetUsers.Any(u => u.Email == email)); 
            }
        }

        public bool ConfirmEmail(string id, string email)
        {
            using (DataContext dc = new DataContext())
            {
                var currentUser = dc.AspNetUsers.FirstOrDefault(u => u.Id == id);
                if (currentUser != null)
                    currentUser = dc.AspNetUsers.FirstOrDefault(u => u.Email == email);
                if (currentUser != null)
                {
                    currentUser.EmailConfirmed = true;
                    dc.SaveChanges();
                    return true;
                }

                return false;
            }
        }


        public bool BlockUser(string id, string email)
        {
            using (DataContext dc = new DataContext())
            {
                var currentUser = dc.AspNetUsers.FirstOrDefault(u => u.Id == id);
                if (currentUser != null)
                    currentUser = dc.AspNetUsers.FirstOrDefault(u => u.Email == email);
                if (currentUser != null)
                {
                    currentUser.EmailConfirmed = true;
                    dc.SaveChanges();
                    return false;
                }

                return false;
            }
        }

    }
}