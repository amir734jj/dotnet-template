using System.ComponentModel.DataAnnotations;

namespace Models.Constants;

public class GlobalConfigs
{
    // We should only have single row of global id in database
    [Key]
    public int Id { get; set; }
}