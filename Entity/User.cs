using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MilitaryServices.App.Entity
{
    [Table("user", Schema = "ms")]
    public class User
    {
        [Key]
        [Column("username", TypeName = "VARCHAR(255)")]
        public string Username { get; set; }

        [Column("password")]
        public string Password { get; set; }

        [Column("enabled")]
        public bool Enabled { get; set; }

        [ForeignKey("SoldierId")]
        [Column("soldier_id")]
        public virtual Soldier Soldier { get; set; }

        public virtual ICollection<Authority> Authorities { get; set; }

        public User()
        {
            Authorities = new List<Authority>();
        }

        public User(string username, string password, bool enabled)
        {
            Username = username;
            Password = password;
            Enabled = enabled;
            Authorities = new List<Authority>();
        }

        public void SetUserId(string username)
        {
            Username = username;
        }

        public void SetPassword(string password)
        {
            Password = password;
        }

        public void SetEnabled(bool enabled)
        {
            Enabled = enabled;
        }

        public void SetSoldier(Soldier soldier)
        {
            Soldier = soldier;
        }

        public void SetAuthorities(ICollection<Authority> authorities)
        {
            Authorities = authorities;
        }

        public string GetUserId()
        {
            return Username;
        }

        public string GetPassword()
        {
            return Password;
        }

        public bool IsEnabled()
        {
            return Enabled;
        }

        public Soldier GetSoldier()
        {
            return Soldier;
        }

        public ICollection<Authority> GetAuthorities()
        {
            return Authorities;
        }
    }
}
