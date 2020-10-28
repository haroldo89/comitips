using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoreBot.Details;
using CoreBot.Model;
using Luis;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    /// <summary>
    /// "luisgen Comision.json -cs Comision -o ."
    /// </summary>
    public class MainComisionDialog : ComponentDialog
    {
        private readonly ComisionRecognizer _luisRecognizer;
        protected readonly ILogger Logger;
        private string InitialMessageDialog { get; set; }

        public MainComisionDialog(ComisionRecognizer luisRecognizer,
            ComisionDialog comisionDialog,
            CompensacionDialog compensacionDialog,
            PenalizacionesDialog penalizacionesDialog,
            ReclamacionesDialog reclamacionesDialog,
            VentasDialog ventasDialog,
            ILogger<MainDialog> logger)
            : base(nameof(MainComisionDialog))
        {
            StringBuilder stbMessage = new StringBuilder();
            stbMessage.AppendLine("Hola! ¿en que te puedo colaborar?");
            _luisRecognizer = luisRecognizer;
            Logger = logger;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(comisionDialog);
            AddDialog(compensacionDialog);
            AddDialog(penalizacionesDialog);
            AddDialog(reclamacionesDialog);
            AddDialog(ventasDialog);
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                IntroStepAsync,
                ActStepAsync,
                FinalStepAsync,
            }));
            this.InitialMessageDialog = string.Empty;
            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (!_luisRecognizer.IsConfigured)
            {
                await stepContext.Context.SendActivityAsync(
                    MessageFactory.Text("NOTE: LUIS is not configured. To enable all capabilities, add 'LuisAppId', 'LuisAPIKey' and 'LuisAPIHostName' to the appsettings.json file.", inputHint: InputHints.IgnoringInput), cancellationToken);

                return await stepContext.NextAsync(null, cancellationToken);
            }
            // Use the text provided in FinalStepAsync or the default if it is the first time.
            var messageText = stepContext.Options?.ToString() ?? this.InitialMessageDialog;
            var promptMessage = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (!_luisRecognizer.IsConfigured)
            {
                // LUIS is not configured, we just run the BookingDialog path with an empty BookingDetailsInstance.
                return await stepContext.BeginDialogAsync(nameof(ComisionDialog), new ComisionDetails(), cancellationToken);
            }

            var details = new MessageDetails();
            // Call LUIS and gather any potential booking details. (Note the TurnContext has the response to the prompt.)
            var luisResult = await _luisRecognizer.RecognizeAsync<Comision>(stepContext.Context, cancellationToken);
            switch (luisResult.TopIntent().intent)
            {
                case Comision.Intent.Comision:
                    details = new ComisionDetails()
                    {
                        RequestMessage = (string)stepContext.Result,
                        //MessageValue = luisResult.Venta,
                        GenerateDate = luisResult.Date
                    };
                    return await stepContext.BeginDialogAsync(nameof(ComisionDialog), details, cancellationToken);
                case Comision.Intent.Compensacion:
                    details = new CompensacionDetails()
                    {
                        RequestMessage = (string)stepContext.Result,
                        //MessageValue = luisResult.Venta,
                        GenerateDate = luisResult.Date
                    };
                    return await stepContext.BeginDialogAsync(nameof(CompensacionDialog), details, cancellationToken);
                case Comision.Intent.Penalizaciones:
                    details = new PenalizacionDetails()
                    {
                        RequestMessage = (string)stepContext.Result,
                        //MessageValue = luisResult.Venta,
                        GenerateDate = luisResult.Date
                    };
                    return await stepContext.BeginDialogAsync(nameof(PenalizacionesDialog), details, cancellationToken);
                case Comision.Intent.Reclamaciones:
                    details = new ReclamacionDetails()
                    {
                        RequestMessage = (string)stepContext.Result,
                        MessageValue = luisResult.Reclamacion,
                        GenerateDate = luisResult.Date
                    };
                    return await stepContext.BeginDialogAsync(nameof(ReclamacionesDialog), details, cancellationToken);
                case Comision.Intent.Ventas:
                    details = new VentaDetails()
                    {
                        RequestMessage = (string)stepContext.Result,
                        MessageValue = luisResult.Venta,
                        GenerateDate = luisResult.Date
                    };
                    return await stepContext.BeginDialogAsync(nameof(VentasDialog), details, cancellationToken);
                default:
                    // Catch all for unhandled intents
                    var didntUnderstandMessageText = string.Format("Hola! ¿en que te puedo colaborar?");
                    var didntUnderstandMessage = MessageFactory.Text(didntUnderstandMessageText, didntUnderstandMessageText, InputHints.IgnoringInput);
                    await stepContext.Context.SendActivityAsync(didntUnderstandMessage, cancellationToken);
                    break;
            }
            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // If the child dialog ("BookingDialog") was cancelled, the user failed to confirm or if the intent wasn't BookFlight
            // the Result here will be null.
            if (stepContext.Result is ComisionDetails result)
            {
                // Now we have all the booking details call the booking service.
                // If the call to the booking service was successful tell the user.

                var timeProperty = new TimexProperty(result.GenerateDate);
                var dateMsg = timeProperty.ToNaturalLanguage(DateTime.Now);
                var messageText = string.Format("La informacion seleccionada es {0} en la fecha {1}", result.Segmento, dateMsg);
                var message = MessageFactory.Text(messageText, messageText, InputHints.IgnoringInput);
                await stepContext.Context.SendActivityAsync(message, cancellationToken);
            }

            // Restart the main dialog with a different message the second time around
            var promptMessage = "realiza una pregunta";
            return await stepContext.ReplaceDialogAsync(InitialDialogId, promptMessage, cancellationToken);
        }

    }
}
