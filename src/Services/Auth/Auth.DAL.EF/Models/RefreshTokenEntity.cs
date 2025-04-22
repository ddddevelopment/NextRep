using System.ComponentModel.DataAnnotations;

namespace Auth.DAL.EF.Models;

public class RefreshTokenEntity {
    [Key]
    public Guid id { get; set; }

    [Required]
    [MaxLength(128)]
    public string token { get; set; }

    [Required]
    [MaxLength(128)]
    public Guid user_id { get; set; }

    public DateTime created_date { get; set; }
    public DateTime expiry_date { get; set; }
    public bool used { get; set; }
    public bool revoked { get; set; }
}
