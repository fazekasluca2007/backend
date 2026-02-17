using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcoTrip.Models
{
    [Table("users")]
    public class Users
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("fullName")]
        public string FullName { get; set; } = null!;

        [Required]
        [Column("username")]
        public string Username { get; set; } = null!;

        [Required]
        [Column("email")]
        public string Email { get; set; } = null!;

        [Required]
        [Column("password_hash")]
        public string PasswordHash { get; set; } = null!;

        [Column("profile_image")]
        public string? ProfileImage { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        [Column("admin")]
        public int Admin { get; set; }
    }
}
