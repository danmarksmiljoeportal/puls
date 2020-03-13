using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dmp.Examples.PulsIntegration
{
    public static class BathingwaterStationExamples
    {
        public static async Task SubmitSchedule(PulsClient client)
        {
            var bathingwaterStationId = new Guid("673b92fa-ae6a-4a2c-81b5-e9fb4a65a000");

            Console.WriteLine("Submitting bathingwater schedule");
            await client.BathingwaterScheduleSubmitAsync(new SubmitBathingwaterScheduleRequest
            {
                StationId = bathingwaterStationId,
                Year = 2020,
                Extended = false,
                Samplings = new List<DateTimeOffset>
                {
                    new DateTime(2019, 5, 22),
                    new DateTime(2019, 6, 2),
                    new DateTime(2019, 6, 16),
                    new DateTime(2019, 6, 30),
                    new DateTime(2019, 7, 7),
                    new DateTime(2019, 7, 14),
                    new DateTime(2019, 7, 21),
                    new DateTime(2019, 7, 28),
                    new DateTime(2019, 8, 4),
                    new DateTime(2019, 8, 18)
                }
            });
        }
    }
}