using System;
using System.Text.RegularExpressions;
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

        public object Obfuscate()
        {
            const string pattern = @"(?<=[\w]{1})[\w-\._\+%]*(?=[\w]{1}@)";

            var obfuscatedEmail = Regex.Replace(Email, pattern, m => new string('*', m.Length));
            
            return new {Email = obfuscatedEmail, Name};
        }

        public object ToAnonymousObject()
        {
            return new {Email, Name};
        }
    }
}