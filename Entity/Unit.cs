using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MilitaryServices.App.Entity
{
    [Table("unit", Schema = "ms")]
    public class Unit
    {
        [Key]
        [Column("unit_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Column("name_of_unit")]
        public string NameOfUnit { get; set; }

        [Column("companies")]
        public string Companies { get; set; }

        public virtual List<ServiceOfUnit> ServicesOfUnit { get; set; }

        public Unit()
        {
            ServicesOfUnit = new List<ServiceOfUnit>();
        }

        public List<ServiceOfUnit> GetServicesOfUnit()
        {
            return ServicesOfUnit;
        }
    }
}
