using CoreBot.Details;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBot.Chain
{
    public abstract class BotMessage
    {
        public BotMessage Successor;

        public void SetSuccessor(BotMessage successor)
        {
            this.Successor = successor;
        }

        public abstract void ProcessRequest(WaterfallStepContext stepContext, StringBuilder stbResult);
    }
}
