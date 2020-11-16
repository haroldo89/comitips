using CoreBot.Details;
using CoreBot.Message;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Text;

namespace CoreBot.Chain
{
    public class Recuperacionq : BotMessage
    {
        public override void ProcessRequest(WaterfallStepContext stepContext, StringBuilder stbResult)
        {
            var details = (VentaDetails)stepContext.Options;
            if (details.MessageValue.Contains("recuperacionq", StringComparison.InvariantCultureIgnoreCase))
            {
                VentaMessage.GetMessageRecuperacionQ(stbResult);
            }
            else if (this.Successor != null)
            {
                this.Successor.ProcessRequest(stepContext, stbResult);
            }
        }
    }
}
