using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AjdemeSi.Domain.Models.Identity
{
    public class IdentityUserViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        [Display(Name = "Confirmed")]
        public bool EmailConfirmed { get; set; }
        public bool LockoutEnabled { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy hh:mm:ss}")]
        public DateTime DateCreated { get; set; }
        public List<string> UserRoles { get; set; }
    }
}