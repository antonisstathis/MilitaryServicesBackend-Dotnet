using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MilitaryServices.App.Entity
{
    [Table("ser_of_unit", Schema = "ms")]
    public class ServiceOfUnit
    {
        [Key]
        [Column("ser_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Column("ser_name")]
        public string ServiceName { get; set; }

        [Column("armed")]
        public string Armed { get; set; }

        [Column("company")]
        public string Company { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("shift")]
        public string Shift { get; set; }

        [ForeignKey("UnitId")]
        [Column("unit_id")]
        public long? UnitId { get; set; }

        public virtual Unit Unit { get; set; }

        public ServiceOfUnit() { }

        public ServiceOfUnit(string serviceName, string armed, string company, string description, string shift, Unit unit)
        {
            ServiceName = serviceName;
            Armed = armed;
            Company = company;
            Description = description;
            Shift = shift;
            Unit = unit;
            UnitId = unit?.Id;
        }

        [NotMapped]
        public bool IsArmed => string.Equals(Armed, Enums.Situation.ARMED.ToString(), StringComparison.OrdinalIgnoreCase);
    }
}
