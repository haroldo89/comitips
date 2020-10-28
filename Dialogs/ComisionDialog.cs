using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoreBot.Details;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class ComisionDialog : CancelAndHelpDialog
    {
        private string DestinationStepMsgText { get; set; }

        public ComisionDialog()
            : base(nameof(ComisionDialog))
        {
            StringBuilder stbMessage = new StringBuilder();
            stbMessage.AppendLine("comisiones - ¿tu consulta es para?");
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new DateResolverDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                SegmentoStepAsync,
                DateStepAsync,
                ConfirmStepAsync,
                FinalStepAsync,
            }));
            this.DestinationStepMsgText = stbMessage.ToString();
            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> SegmentoStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var comisionDetails = (ComisionDetails)stepContext.Options;

            if (comisionDetails.Segmento == null)
            {
                var promptMessage = MessageFactory.Text(DestinationStepMsgText, DestinationStepMsgText, InputHints.ExpectingInput);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
            }

            return await stepContext.NextAsync(comisionDetails.Segmento, cancellationToken);
        }

        private async Task<DialogTurnResult> DateStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var comisionDetails = (ComisionDetails)stepContext.Options;

            comisionDetails.Segmento = (string)stepContext.Result;

            if (comisionDetails.GenerateDate == null || IsAmbiguous(comisionDetails.GenerateDate))
            {
                return await stepContext.BeginDialogAsync(nameof(DateResolverDialog), comisionDetails.GenerateDate, cancellationToken);
            }

            return await stepContext.NextAsync(comisionDetails.GenerateDate, cancellationToken);
        }

        private async Task<DialogTurnResult> ConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var comisionDetails = (ComisionDetails)stepContext.Options;

            comisionDetails.GenerateDate = (string)stepContext.Result;

            var messageText = string.Format("por favor confirmar, mi solicitd es {0} para la fecha {1}", comisionDetails.Segmento, comisionDetails.GenerateDate);

            var promptMessage = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);

            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if ((bool)stepContext.Result)
            {
                var bookingDetails = (ComisionDetails)stepContext.Options;

                return await stepContext.EndDialogAsync(bookingDetails, cancellationToken);
            }
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }

        private static bool IsAmbiguous(string timex)
        {
            var timexProperty = new TimexProperty(timex);
            return !timexProperty.Types.Contains(Constants.TimexTypes.Definite);
        }
    }
}
