using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static eComAPI.Models.Annotations;

namespace eComAPI.Models
{
    public class Member
    {
        [Required]
        [Key]
        [AutoIncrement]
        public int MemberId { get; set; }
        public DateTime LockoutEndDate { get; set; }
        public int LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public string MemberName { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastLoginDate { get; set; }
    }
}