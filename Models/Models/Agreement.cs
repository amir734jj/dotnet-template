using System.ComponentModel.DataAnnotations;
using Models.Interfaces;

namespace Models.Models;

public class Agreement : IEntity
{
    [Key]
    public int Id { get; set; }
    
    
}