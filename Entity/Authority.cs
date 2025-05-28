using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MilitaryServices.App.Entity
{
    [Table("authority", Schema = "ms")]
    public class Authority
    {
        [Key]
        [Column("auth_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long AuthId { get; set; }

        [ForeignKey("Username")]
        [Column("username")]
        public string Username { get; set; }

        public virtual User User { get; set; }

        [Column("authority")]
        public string AuthorityName { get; set; }

        public Authority() { }

        public Authority(User user, string authority)
        {
            User = user;
            Username = user.Username; // Assumes User has a Username property
            AuthorityName = authority;
        }
    }
}
