using System.ComponentModel.DataAnnotations;

namespace Task_06.Models.DTOs;

public class AddAnimal
{
    [MinLength(3)]
    [MaxLength(200)]
    public string Name { get; set; }
    [MaxLength(200)]
    public string? Description { get; set; }
    
}