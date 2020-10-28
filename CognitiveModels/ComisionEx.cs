using System.Linq;

namespace Luis
{
    /// <summary>
    /// "luisgen Comision.json -cs Comision -o ."
    /// </summary>
    public partial class Comision
    {
        public (string De, string Agente) DeEntities
        {
            get
            {
                var fromValue = Entities?._instance?.de?.FirstOrDefault()?.Text;
                var fromAirportValue = Entities?.de?.FirstOrDefault()?.Agente?.FirstOrDefault()?.FirstOrDefault();
                return (fromValue, fromAirportValue);
            }
        }

        public (string Para, string Agente) ParaEntities
        {
            get
            {
                var toValue = Entities?._instance?.para?.FirstOrDefault()?.Text;
                var toAirportValue = Entities?.para?.FirstOrDefault()?.Agente?.FirstOrDefault()?.FirstOrDefault();
                return (toValue, toAirportValue);
            }
        }

        public string InfoVenta
             => Entities?._instance?.VentaMessage?.FirstOrDefault()?.Text;

        public string Date
            => Entities.datetime?.FirstOrDefault()?.Expressions.FirstOrDefault()?.Split('T')[0];

        /// <summary>
        /// Obtener mensaje de intencion cuando se detecta que es para venta
        /// </summary>
        public string Venta
            => Entities.VentaMessage?.FirstOrDefault()?.FirstOrDefault();
        
        /// <summary>
        /// Obtener mensaje de intension cuando se detecta que es para una reclamacion
        /// </summary>
        public string Reclamacion
            => Entities.ReclamacionMessage?.FirstOrDefault()?.FirstOrDefault();
    }
}
