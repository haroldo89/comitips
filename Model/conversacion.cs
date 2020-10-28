using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Model
{
    public class conversacion
    {
        [Key]
        public long id { get; set; }
        public string identificacion { get; set; }
        public string intension { get; set; }
        public bool fue_solucionado { get; set; }
        public string solicitud { get; set; }
        public string respuesta { get; set; }
        public System.DateTime fecha { get; set; }
    }
}
