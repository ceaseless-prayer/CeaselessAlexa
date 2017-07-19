using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using AlexaLambdaHandler;
using Amazon.Lambda.Core;
using Amazon.Runtime;
using Newtonsoft.Json;

[assembly: InternalsVisibleTo("Ceaseless.Tests")]
// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
namespace Ceaseless
{
    public class Function : LambdaHandler
    {

        private readonly IEngine engine;

        public Function() :
            this(new Engine(
                new EnvironmentVariablesAWSCredentials(),
                new EnvironmentVariableAWSRegion().Region))
        {
        }

        internal Function(IEngine engine)
        {
            this.engine = engine;

            AddAsyncIntentRequestHandler("AddPersonToPray", HandleAddPersonToPrayIntent);
            AddAsyncIntentRequestHandler("GetNextPersonToPray", HandleGetNextPersonToPrayIntent);

            DefaultIntentRequestHandler = (request, intentRequest, context) => Task.FromResult(ResponseBuilder.Tell(
                new PlainTextOutputSpeech
                {
                    Text = "I don't understand what you are asking."
                }));

            SystemExceptionRequestHandler = (request, exceptionRequest, context) =>
            {
                context.Logger.Log($"ERROR: {exceptionRequest.Error.Message}");
                return Task.FromResult(ResponseBuilder.Empty());
            };
        }

        public async Task<SkillResponse> FunctionHandler(SkillRequest input, ILambdaContext context)
        {
            context.Logger.Log($"Input: {JsonConvert.SerializeObject(input)}");
            return await Handle(input, context);
        }

        internal async Task<SkillResponse> HandleAddPersonToPrayIntent(SkillRequest request, IntentRequest intentRequest, ILambdaContext context)
        {
            var userId = request.Session.User.UserId;
            var name = intentRequest.Intent.Slots["Name"].Value;
            context.Logger.Log($"{userId}: Adding '{name}'");
            await engine.AddPersonToPray(userId, name);
            return ResponseBuilder.Tell(new PlainTextOutputSpeech() { Text = $"I have added {name} to your prayer list." });
        }

        internal async Task<SkillResponse> HandleGetNextPersonToPrayIntent(SkillRequest request, IntentRequest intentRequest, ILambdaContext context)
        {
            var userId = request.Session.User.UserId;

            var names = await engine.GetPeopleToPray(userId);
            if (names.Count == 0)
            {
                return ResponseBuilder.Tell(new PlainTextOutputSpeech() { Text = "You have no one to pray in your list." });
            }

            var name = names[new Random().Next(names.Count)];
            return ResponseBuilder.Tell(new PlainTextOutputSpeech() { Text = $"The next person to pray for is {name}." });
        }

    }
}
