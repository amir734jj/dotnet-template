using System;
using Microsoft.AspNetCore.Identity;

namespace Models.Models
{
    /// <summary>
    /// Website user model
    /// </summary>
    public class User : IdentityUser<int>
    {
        public string Name { get; set; }
        
        public virtual DateTimeOffset LastLoginTime { get; set; }  = DateTimeOffset.MinValue;
    }
}