using System.ComponentModel.DataAnnotations;

namespace Models.Interfaces;

public interface IEntity
{
    [Key]
    public int Id { get; set; }
}