using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Models.Enums;

namespace Models.Models;

public class User : IdentityUser<int>
{
    public string Name { get; set; }
        
    public virtual DateTimeOffset LastLoginTime { get; set; }
        
    public RoleEnum Role { get; set; }
        
    [Column(TypeName = "text")]
    public string Description { get; set; }

    public List<Unit> Blogs { get; set; }

    public string Photo { get; set; }
}