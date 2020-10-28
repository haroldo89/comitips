using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoreBot.Details;
using CoreBot.Model;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class ReclamacionesDialog : CancelAndHelpDialog
    {
        private const string FirstStepMsgText = "por favor realiza tu consulta";
        private const string RequestIdentification = "Digita tu numero de identificación";

        public ReclamacionesDialog()
            : base(nameof(ReclamacionesDialog))
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
            var comisionDetails = (ReclamacionDetails)stepContext.Options;
            if (comisionDetails.MessageValue == null)
            {
                var promptMessage = MessageFactory.Text(FirstStepMsgText, FirstStepMsgText, InputHints.ExpectingInput);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
            }

            return await stepContext.NextAsync(comisionDetails.MessageValue, cancellationToken);
        }

        private async Task<DialogTurnResult> IdentificationStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var comisionDetails = (ReclamacionDetails)stepContext.Options;
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
            var comisionDetails = (ReclamacionDetails)stepContext.Options;
            comisionDetails.IdentificationNumber = (string)stepContext.Result;
            string messageText = string.Empty;
            RegistroRepository registroRepository = new RegistroRepository();
            var registro = registroRepository.GetByIdentification(comisionDetails.IdentificationNumber);
            messageText = registro != null && !string.IsNullOrWhiteSpace(registro.identificacion) ?
                string.Format("{0} por favor confirmar si la solicitud es acerca de {1}", registro.nombres, comisionDetails.MessageValue) :
                string.Format("por favor confirmar, mi solicitd es {0}", comisionDetails.MessageValue, comisionDetails.GenerateDate);
            var promptMessage = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> ResultStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var comisionDetails = (ReclamacionDetails)stepContext.Options;
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
            var details = (ReclamacionDetails)stepContext.Options;
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
            var details = (ReclamacionDetails)stepContext.Options;
            if (details.MessageValue.Contains("reclamacion", StringComparison.InvariantCultureIgnoreCase))
            {
                stbResult.Append(string.Format("{0}", " para la carga o creacion de un requerimiento en remedy se debe realizar "));
                stbResult.Append(string.Format("{0}", " única y exclusivamente en formato de Excel. No la cargues con .rar ni con otro tipo de comprimidos o con "));
                stbResult.Append(string.Format("{0}", "formato de correo (.msg) puesto que se realiza un descargue y consolidación de reclamos por medio de un "));
                stbResult.AppendLine(string.Format("{0}", "robot que solo valida los archivos de Excel. "));
                stbResult.Append(string.Format("{0}", "Sugerencia: En remedy puedes cargar hasta 3 adjuntos, si tus soportes son muy pesados o te generan "));
                stbResult.Append(string.Format("{0}", "problema, solo carga en remedy la plantilla de reclamaciones y si gustas envías los soportes que tengas al "));
                stbResult.AppendLine(string.Format("{0}", "correo de la persona que esté liquidando las comisiones."));
                stbResult.Append(string.Format("{0}", "Es de utilidad el resultado de tu consulta."));
            }
            else if (details.MessageValue.Contains("otros", StringComparison.InvariantCultureIgnoreCase))
            {
                stbResult.AppendLine(string.Format("{0}", "Si tu reclamacion es por un tema  como visitas, metas o recuperacion"));
                stbResult.AppendLine(string.Format("{0}", "no es necesario diligenciar el campo suscriptor, pero si debes asegurarte de incluir tus"));
                stbResult.AppendLine(string.Format("{0}", "datos de documento y los del staff"));
            }
            else
            {
                stbResult.AppendLine("tu solicitud no puede ser procesada, intenta con estas opciones.");
                stbResult.AppendLine("* ¿Cómo se debe cargar la plantilla de reclamos en Remedy?");
                stbResult.AppendLine("* ¿Si mi reclamación es por un tema como visitas, metas o recuperación, que debo escribir en el campo suscriptor?");
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
