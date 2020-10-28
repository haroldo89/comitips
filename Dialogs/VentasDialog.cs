using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoreBot.Details;
using CoreBot.Message;
using CoreBot.Model;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class VentasDialog : CancelAndHelpDialog
    {
        private const string FirstStepMsgText = "por favor realiza tu consulta";
        private const string RequestIdentification = "Digita tu numero de identificación";

        public VentasDialog()
            : base(nameof(VentasDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new DateResolverDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                FirstStepAsync,
                IdentificationStepAsync,
                ConfirmStepAsync,
                ResultStepAsync,
                FinalStepAsync
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> FirstStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var comisionDetails = (VentaDetails)stepContext.Options;
            if (comisionDetails.MessageValue == null)
            {
                var promptMessage = MessageFactory.Text(FirstStepMsgText, FirstStepMsgText, InputHints.ExpectingInput);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
            }
            return await stepContext.NextAsync(comisionDetails.MessageValue, cancellationToken);
        }

        private async Task<DialogTurnResult> IdentificationStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var comisionDetails = (VentaDetails)stepContext.Options;
            comisionDetails.MessageValue = (string)stepContext.Result;
            if (string.IsNullOrWhiteSpace(comisionDetails.IdentificationNumber))
            {
                var promptMessage = MessageFactory.Text(RequestIdentification, RequestIdentification, InputHints.ExpectingInput);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
            }
            return await stepContext.NextAsync(comisionDetails.IdentificationNumber, cancellationToken);
        }

        private async Task<DialogTurnResult> ConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var comisionDetails = (VentaDetails)stepContext.Options;
            comisionDetails.IdentificationNumber = (string)stepContext.Result;
            string messageText = string.Empty;
            RegistroRepository registroRepository = new RegistroRepository();
            var registro = registroRepository.GetByIdentification(comisionDetails.IdentificationNumber);
            messageText = registro != null && !string.IsNullOrWhiteSpace(registro.identificacion) ?
                string.Format("{0} por favor confirmar, la solicitud es acerca de {1}", registro.nombres, comisionDetails.MessageValue) :
                string.Format("por favor confirmar, mi solicitd es {0}", comisionDetails.MessageValue, comisionDetails.GenerateDate);
            var promptMessage = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> ResultStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var comisionDetails = (VentaDetails)stepContext.Options;
            StringBuilder stbResult = new StringBuilder();
            string message = string.Empty;
            if ((bool)stepContext.Result)
            {
                message = GetResultMessage(stepContext, stbResult);
                comisionDetails.ResultMessage = message;
                var promptMessage = MessageFactory.Text(message, message, InputHints.ExpectingInput);
                return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
            }
            return await stepContext.EndDialogAsync(comisionDetails.ResultMessage, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var details = (VentaDetails)stepContext.Options;
            ConversacionRepository conversacionRepository = new ConversacionRepository();
            conversacion conversacion = new conversacion()
            {
                identificacion = details.IdentificationNumber,
                intension = details.MessageValue,
                fue_solucionado = (bool)stepContext.Result,
                solicitud = details.RequestMessage,
                respuesta = details.ResultMessage,
                fecha = DateTime.Now
            };
            conversacionRepository.InsertNew(conversacion);
            if ((bool)stepContext.Result)
            {
                return await stepContext.EndDialogAsync(details, cancellationToken);
            }
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }

        /// <summary>
        /// Obtener mensaje de resultado
        /// </summary>
        /// <param name="stepContext"></param>
        /// <param name="stbResult"></param>
        /// <returns></returns>
        private static string GetResultMessage(WaterfallStepContext stepContext, StringBuilder stbResult)
        {
            string message;
            var details = (VentaDetails)stepContext.Options;
            if (details.MessageValue.Contains("office", StringComparison.InvariantCultureIgnoreCase))
            {
                VentaMessage.GetMessageOffice(stbResult);
            }
            else if (details.MessageValue.Contains("salesforce", StringComparison.InvariantCultureIgnoreCase))
            {
                VentaMessage.GetMessageSalesForce(stbResult);
            }
            else if (details.MessageValue.Contains("registro", StringComparison.InvariantCultureIgnoreCase))
            {
                VentaMessage.GetMessageRegistro(stbResult);
            }
            else if (details.MessageValue.Contains("planesflex", StringComparison.InvariantCultureIgnoreCase))
            {
                VentaMessage.GetMessagePlanesFlex(stbResult);
            }
            else if (details.MessageValue.Contains("caterpillar", StringComparison.InvariantCultureIgnoreCase))
            {
                VentaMessage.GetMessageCaterpillar(stbResult);
            }
            else if (details.MessageValue.Contains("recuperacionq", StringComparison.InvariantCultureIgnoreCase))
            {
                VentaMessage.GetMessageRecuperacionQ(stbResult);
            }
            else if (details.MessageValue.Contains("pdti", StringComparison.InvariantCultureIgnoreCase))
            {
                VentaMessage.GetMessagePDTI(stbResult);
            }
            else if (details.MessageValue.Contains("venta", StringComparison.InvariantCultureIgnoreCase))
            {
                VentaMessage.GetMessageVenta(stbResult);
            }
            else if (details.MessageValue.Contains("renegociacion", StringComparison.InvariantCultureIgnoreCase))
            {
                VentaMessage.GetMessageRenegociacion(stbResult);
            }
            else
            {
                stbResult.Append("tu solicitud no puede ser procesada, intentalo nuevamente.");
            }
            message = stbResult.ToString();
            return message;
        }

        private static bool IsAmbiguous(string timex)
        {
            var timexProperty = new TimexProperty(timex);
            return !timexProperty.Types.Contains(Constants.TimexTypes.Definite);
        }

    }
}
