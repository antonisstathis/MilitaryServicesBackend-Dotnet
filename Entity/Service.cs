using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MilitaryServices.App.Entity
{
    [Table("services", Schema = "ms")]
    public class Service
    {
        [Key]
        [Column("ser_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Column("ser_name")]
        public string ServiceName { get; set; }

        [Column("armed")]
        public string Armed { get; set; }

        [Column("ser_date")]
        public DateTime? Date { get; set; }

        [ForeignKey("UnitId")]
        [Column("unit_id")]
        public long? UnitId { get; set; }

        public virtual Unit Unit { get; set; }

        [Column("company")]
        public string Company { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("shift")]
        public string Shift { get; set; }

        [ForeignKey("SoldierId")]
        [Column("sold_id")]
        public long? SoldierId { get; set; }

        public virtual Soldier Soldier { get; set; }

        public Service() { }

        public Service(string serviceName, string armed, DateTime date, Unit unit)
        {
            ServiceName = serviceName;
            Armed = armed;
            Date = date;
            Unit = unit;
            UnitId = unit?.Id;
        }

        public Service(string serviceName, string armed, DateTime date, Unit unit, string company, string description, string shift)
        {
            ServiceName = serviceName;
            Armed = armed;
            Date = date;
            Unit = unit;
            UnitId = unit?.Id;
            Company = company;
            Description = description;
            Shift = shift;
        }

        [NotMapped]
        public bool IsArmed => string.Equals(Armed, Enums.Situation.ARMED.ToString(), StringComparison.OrdinalIgnoreCase);
    }
}
