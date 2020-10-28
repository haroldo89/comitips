using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Model
{
    public class registro
    {
        [Key]
        public long id { get; set; }
        public string nombres { get; set; }
        public string identificacion { get; set; }
        public string numero_telefonico { get; set; }
        public Nullable<System.DateTime> fecha_registro { get; set; }
    }
}
