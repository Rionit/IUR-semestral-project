using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace IUR_Semestral_Work.Data
{
    public class PushpinData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string PinType { get; set; }
        public bool Picked { get; set; }
        public Guid UserID { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateRemoved { get; set; }
    }

}
