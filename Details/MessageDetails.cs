using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Details
{
    /// <summary>
    /// 
    /// </summary>
    public class MessageDetails 
    {
        /// <summary>
        /// Numero de identificacion
        /// </summary>
        public string IdentificationNumber { get; set; }
        /// <summary>
        /// mensaje de requerimiento completo
        /// </summary>
        public string RequestMessage { get; set; }
        /// <summary>
        /// mensaje contextualizado por el bot
        /// </summary>
        public string MessageValue { get; set; }

        /// <summary>
        /// mensaje resultado del bot
        /// </summary>
        public string ResultMessage { get; set; }
        /// <summary>
        /// fecha generado
        /// </summary>
        public string GenerateDate { get; set; }
    }
}
