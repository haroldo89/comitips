using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBot.Message
{
    public static class VentaMessage
    {
        public static void GetMessageOffice(StringBuilder stbResult)
        {
            stbResult.AppendLine(string.Format("{0}", "debes validar el campo cont_2 en el detalle de ventas. Alli aparece el numero de licencias asociadas a tu cuenta"));
            stbResult.AppendLine(string.Format("{0}", "Es de utilidad el resultado de tu consulta."));
        }

        public static void GetMessageSalesForce(StringBuilder stbResult)
        {
            stbResult.Append(string.Format("{0}", "Según política: 'Cuando el alta de un servicio o "));
            stbResult.Append(string.Format("{0}", "producto su duración en meses es menor a 12 se aplica la siguiente fórmula: "));
            stbResult.AppendLine(string.Format("{0}", "(Cargo fijo mes total en pesos* duración en meses) / 12 + (cargos de conexión total en pesos) / 12'"));
            stbResult.AppendLine("O");
            stbResult.Append(string.Format("{0}", "Para que una MS quede registrada como alta en el detalle, se debe validar en SF que las Fechas RFS y "));
            stbResult.Append(string.Format("{0}", "de legalización se encuentren correctamente registradas y correspondan a las fechas estipuladas para "));
            stbResult.Append(string.Format("{0}", "cargue de cada periodo. Tener presente el anexo 1, ítem 2.2 FIJA, DIGITAL FIJA, PDTI, COBROS ÚNICOS "));
            stbResult.Append(string.Format("{0}", "Y DIGITAL MÓVIL (aplica para lo que se registra en Salesforce), allí se indican las condiciones para cargue "));
            stbResult.AppendLine(string.Format("{0}", "de altas. "));
            stbResult.Append(string.Format("{0}", "Es de utilidad el resultado de tu consulta."));
        }

        public static void GetMessageRegistro(StringBuilder stbResult)
        {
            stbResult.Append(string.Format("{0}", "Debes validar la variación que existe entre el plan anterior y el plan actual, la diferencia entre ellos es lo "));
            stbResult.Append(string.Format("{0}", "que se reporta en el cambio de plan(ya sea positivo o negativo).Aquí se tiene en cuenta los valores de los "));
            stbResult.AppendLine(string.Format("{0}", "planes correspondientes al periodo actual. "));
            stbResult.AppendLine("O");
            stbResult.Append(string.Format("{0}", "Tu número de documento debe estar registrado en los campos CC_COMERCIAL, CC_JEFE, "));
            stbResult.Append(string.Format("{0}", "CC_COORDINADOR o CC_GERENTE (según corresponda con tu cargo). Si un registro está asociado a "));
            stbResult.Append(string.Format("{0}", "tu nombre, pero los campos de cedula dicen '0', '1' o No tienen tu documento, significa que dicho Registro "));
            stbResult.AppendLine(string.Format("{0}", "no te está contando. "));
            stbResult.AppendLine(string.Format("{0}", "Sugerencia: "));
            stbResult.AppendLine(string.Format("{0}", "* Si tienes un registro (altas, bajas, updown) asociado a tu cédula y No te debe contar, lo debes reclamar "));
            stbResult.AppendLine(string.Format("{0}", "* Si tienes un registro (altas, bajas, updown) que es de tu gestión, pero en el campo de la cédula dice '0', '1' o tiene el documento de otra persona, lo debes reclamar "));
            stbResult.Append(string.Format("{0}", "Es de utilidad el resultado de tu consulta."));
        }

        public static void GetMessagePlanesFlex(StringBuilder stbResult)
        {
            stbResult.Append(string.Format("{0}", "Recueda que según política: 'los planes Flex H3M y C1E  "));
            stbResult.Append(string.Format("{0}", "se contarán en el ejecutado 0,8 veces arpu  "));
            stbResult.Append(string.Format("{0}", "tu nombre, pero los campos de cedula dicen '0', '1' o No tienen tu documento, significa que dicho Registro "));
            stbResult.AppendLine(string.Format("{0}", "independientemente del plazo del negocio' "));
            stbResult.Append(string.Format("{0}", "Es de utilidad el resultado de tu consulta."));
        }

        public static void GetMessageCaterpillar(StringBuilder stbResult)
        {
            stbResult.AppendLine(string.Format("{0}", "Recuerda que según política: 'La venta de equipos Caterpillar contarán en el ejecutado al 0, 5'"));
            stbResult.Append(string.Format("{0}", "Es de utilidad el resultado de tu consulta."));
        }

        public static void GetMessageRecuperacionQ(StringBuilder stbResult)
        {
            stbResult.Append(string.Format("{0}", "Es posible que, si hayas aplicado para recuperación por promedio, pero la suma de los pagos que se "));
            stbResult.Append(string.Format("{0}", "realizaron en cada periodo es superior al valor a reportar en recuperación. "));
            stbResult.Append(string.Format("{0}", "Ejemplo: Tu valor de pago de recuperación es de $1.560.000 y de los pagos de cada periodo se te ha "));
            stbResult.Append(string.Format("{0}", "enviado $1.660.000 por lo que lo que el valor que se te ha liquidado en los tres meses que se incluyen en "));
            stbResult.AppendLine(string.Format("{0}", "la recuperación de Q, es superior al valor que se te generó en recuperación."));
            stbResult.AppendLine("O");
            stbResult.Append(string.Format("{0}", "Se tiene en cuenta valor liquidado de: Comisiones, Incentivos(Renegociación, Portabilidad, "));
            stbResult.Append(string.Format("{0}", "Clientes, etc.), Aceleradores(crecimiento ingresos altas) y todas aquellas bonificaciones "));
            stbResult.Append(string.Format("{0}", "adicionales que se les reporte en dinero(exceptuando Mark up)"));
            stbResult.Append(string.Format("{0}", "Ejemplo: en un periodo tuviste comisión por 1500.000 y un incentivo de renegociación por $500.000; el "));
            stbResult.Append(string.Format("{0}", "valor que debes tener en cuenta para el cálculo de la recuperación de Q(el que se restaría), sería de "));
            stbResult.AppendLine(string.Format("{0}", "$2.000.000"));
            stbResult.Append(string.Format("{0}", "Es de utilidad el resultado de tu consulta."));
        }

        public static void GetMessagePDTI(StringBuilder stbResult)
        {
            stbResult.AppendLine(string.Format("Valide que la MS tenga fecha RFS del mes a liquidar"));
            stbResult.Append(string.Format("{0}", "Es de utilidad el resultado de tu consulta."));
        }

        public static void GetMessageVenta(StringBuilder stbResult)
        {
            stbResult.AppendLine(string.Format("Recuerda validar fecha OT y fecha legalizacion.  Deben ser del mes a liqudar"));
            stbResult.Append(string.Format("{0}", "Es de utilidad el resultado de tu consulta."));
        }

        public static void GetMessageRenegociacion(StringBuilder stbResult)
        {
            stbResult.AppendLine(string.Format("Recuerda validar que la fecha fin contrato haya sido modificada dentro del mes liquidado"));
            stbResult.Append(string.Format("{0}", "Es de utilidad el resultado de tu consulta."));
        }


    }
}
