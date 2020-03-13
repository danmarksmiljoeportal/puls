using System;
using System.Threading.Tasks;

namespace Dmp.Examples.PulsIntegration
{
    public static class WwtpExamples
    {
        public static async Task GetSamples(PulsClient pulsClient)
        {
            var wwtpId = Guid.Parse("6ba64d23-d2c7-4e1d-9068-8b38b388dfe0");

            Console.WriteLine("Retrieving wwtp details");
            var wwtp = await pulsClient.WwtpGetAsync(wwtpId);

            foreach (var outlet in wwtp.Outlets)
            {
                var from = new DateTime(2018, 1, 1);
                var to = new DateTime(2019, 1, 1);

                Console.WriteLine($"Retrieving wwtp outlet examinations from {outlet.Name} for 2018");
                var examinations = await pulsClient.WwtpOutletGetExaminationsAsync(wwtp.ObservationFacilityId, outlet.Id, new DateTimeOffset(from), new DateTimeOffset(to));
            }
        }

        public static async Task GetVolumes(PulsClient pulsClient)
        {
            var wwtpId = Guid.Parse("6ba64d23-d2c7-4e1d-9068-8b38b388dfe0");

            Console.WriteLine($"Retrieving wwtp volume submissions");
            var submissions = await pulsClient.WwtpGetVolumeSubmissionsAsync(wwtpId, 10);
        }

        public static async Task GetTransportComputation(PulsClient pulsClient)
        {
            var wwtpId = Guid.Parse("6ba64d23-d2c7-4e1d-9068-8b38b388dfe0");

            Console.WriteLine($"Retrieving wwtp transport computation for 2018");
            var computations = await pulsClient.WwtpGetTransportComputationAsync(wwtpId, 2018);
        }

        public static async Task Rename(PulsClient pulsClient)
        {
            var wwtpId = Guid.Parse("6ba64d23-d2c7-4e1d-9068-8b38b388dfe0");

            Console.WriteLine("Renaming wwtp");
            await pulsClient.WwtpRenameAsync(wwtpId, new RenameWwtpRequest
            {
                Name = "Nyt renseanlæg",
                Reason = "Begrundelse for ændring af navn"
            });
        }
    }
}
