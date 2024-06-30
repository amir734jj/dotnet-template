using System.ComponentModel.DataAnnotations;
using Models.Interfaces;

namespace Models.Models;

public class Unit : IEntity
{
    [Key]
    public int Id { get; set; }
    
    public string Address { get; set; }

    public int Number { get; set; }
        
    public User Owner { get; set; }
}