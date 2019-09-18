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

            await WwtpExamaples(httpClient);
        }

        private static async Task WwtpExamaples(HttpClient httpClient)
        {
            string pulsEndpoint = "https://puls-api.demo.miljoeportal.dk";

            Console.WriteLine("Initializing PULS client");
            var pulsClient = new PulsClient(pulsEndpoint, httpClient);

            var wwtpId = Guid.Parse("6ba64d23-d2c7-4e1d-9068-8b38b388dfe0");

            Console.WriteLine("Retrieving wwtp details");
            var wwtp = await pulsClient.WwtpGetAsync(wwtpId);

            Console.WriteLine("Renaming wwtp");
            await pulsClient.WwtpRenameAsync(wwtpId, new RenameWwtpRequest
            {
                Name = "Nyt renseanlæg",
                Reason = "Begrundelse for ændring af navn"
            });

            Console.WriteLine($"Retrieving wwtp volume submissions for {wwtp.Name}");
            var submissions = await pulsClient.WwtpGetVolumeSubmissionsAsync(wwtpId, 10);

            Console.WriteLine($"Retrieving wwtp transport computation for {wwtp.Name} 2018");
            var computations = await pulsClient.WwtpGetTransportComputationAsync(wwtpId, 2018);

            foreach (var outlet in wwtp.Outlets)
            {
                var from = new DateTime(2018, 1, 1);
                var to = new DateTime(2019, 1, 1);

                Console.WriteLine($"Retrieving wwtp outlet examinations from {outlet.Name} for 2018");
                var examinations = await pulsClient.WwtpOutletGetExaminationsAsync((Guid)wwtp.ObservationFacilityId, (Guid)outlet.Id, new DateTimeOffset(from), new DateTimeOffset(to));
            }
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