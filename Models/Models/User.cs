using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Models.Enums;

namespace Models.Models
{
    /// <summary>
    /// Website user model
    /// </summary>
    public class User : IdentityUser<int>
    {
        public string Name { get; set; }
        
        public virtual DateTimeOffset LastLoginTime { get; set; }  = DateTimeOffset.MinValue;
        
        public RoleEnum Role { get; set; }
        
        [Column(TypeName = "text")]
        public string Description { get; set; }

        public List<Blog> Blogs { get; set; } = new List<Blog>();

        public string Photo { get; set; }
    }
}