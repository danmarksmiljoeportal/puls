using IdentityModel.Client;
using IdentityModel.OidcClient;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dmp.Examples.PulsIntegration
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var oidcClient = InitializeLoginClient();

            Console.WriteLine("Logging in");
            var loginResult = await Login(oidcClient);

            // Request a new access token using the refresh token if it is expired.
            var refreshResult = await oidcClient.RefreshTokenAsync(loginResult.RefreshToken);

            var httpClient = new HttpClient();
            httpClient.SetBearerToken(loginResult.AccessToken);

            string pulsEndpoint = "https://puls-api.demo.miljoeportal.dk";

            Console.WriteLine("Initializing PULS client");
            var pulsClient = new PulsClient(pulsEndpoint, httpClient);

            await BathingwaterStationExamples.SubmitSchedule(pulsClient);

            await WwtpExamples.GetSamples(pulsClient);
            await WwtpExamples.GetVolumes(pulsClient);
            await WwtpExamples.GetTransportComputation(pulsClient);
            await WwtpExamples.Rename(pulsClient);
        }

        private static OidcClient InitializeLoginClient()
        {
            int port = 7890;

            string authority = "https://log-in.test.miljoeportal.dk/runtime/oauth2";
            string clientId = "** insert client id **";
            string clientSecret = "** insert client secret **";

            string redirectUri = string.Format($"http://127.0.0.1:{port}");

            var options = new OidcClientOptions
            {
                Authority = authority,

                Policy = new Policy
                {
                    Discovery = new DiscoveryPolicy
                    {
                        ValidateIssuerName = false,
                        ValidateEndpoints = false
                    }
                },

                ClientId = clientId,
                ClientSecret = clientSecret,
                RedirectUri = redirectUri,

                Scope = "openid",

                Flow = OidcClientOptions.AuthenticationFlow.AuthorizationCode,
                ResponseMode = OidcClientOptions.AuthorizeResponseMode.Redirect,

                Browser = new SystemBrowser(port)
            };

            var client = new OidcClient(options);

            return client;
        }

        private static async Task<LoginResult> Login(OidcClient client)
        {
            var request = new LoginRequest();
            var result = await client.LoginAsync(request);

            return result;
        }
    }
}