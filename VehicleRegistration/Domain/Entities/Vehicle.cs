using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleRegistration.Domain.Entities;

public class Vehicle
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    [StringLength(150)]
    public string? Name { get; set; }
    [Required]
    [StringLength(100)]
    public string? Brand { get; set; }
    [Required]
    public int Year { get; set; }
}