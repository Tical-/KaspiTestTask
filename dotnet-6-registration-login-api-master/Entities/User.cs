using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Entities;

using System.Text.Json.Serialization;

public class User
{
    [Required]
    public int Id { get; set; }
    [JsonIgnore]
    public string PasswordHash { get; set; }
    public bool IsSeller { get; set; }
    [Required]
    public string Username { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    [MaxLength(12)]
    [Required]
    [Column(TypeName = "char")]
    public string Phone { get; set; }
}

