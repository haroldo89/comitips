using CoreBot.Details;
using CoreBot.Message;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Text;

namespace CoreBot.Chain
{
    public class Renegociacion : BotMessage
    {
        public override void ProcessRequest(WaterfallStepContext stepContext, StringBuilder stbResult)
        {
            var details = (VentaDetails)stepContext.Options;
            if (details.MessageValue.Contains("venta", StringComparison.InvariantCultureIgnoreCase))
            {
                VentaMessage.GetMessageVenta(stbResult);
            }
            else if (this.Successor != null)
            {
                this.Successor.ProcessRequest(stepContext, stbResult);
            }
        }
    }
}
