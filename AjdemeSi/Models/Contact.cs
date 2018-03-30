using System.ComponentModel.DataAnnotations;

namespace AjdemeSi.Models
{
    public class Contact
    {
        public int ContactId { get; set; }

        // user ID from AspNetUser table
        public string OwnerID { get; set; }

        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public ContactStatus Status { get; set; }
    }

    public enum ContactStatus
    {
        Submitted,
        Approved,
        Rejected
    }

    public class Constants
    {
        public static readonly string CreateOperationName = "Create";
        public static readonly string ReadOperationName = "Read";
        public static readonly string UpdateOperationName = "Update";
        public static readonly string DeleteOperationName = "Delete";
        public static readonly string ApproveOperationName = "Approve";
        public static readonly string RejectOperationName = "Reject";

        public static readonly string ContactAdministratorsRole = "ContactAdministrators";
        public static readonly string ContactManagersRole = "ContactManagers";
    }
}