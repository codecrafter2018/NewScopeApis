using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.PowerPlatform.Dataverse.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ultratechapis.Service
{
    public class EventSequenceValidationService
    {
        private readonly ServiceClient _client;

        public EventSequenceValidationService(ServiceClient client)
        {
            _client = client;
        }

        // Calculate the success rate of the event sequence
        public EventSequenceValidationDto ValidateEventSequence(Guid userId, DateTime startDate, DateTime endDate)
        {
            // Step 1: Fetch Events for the given User ID and within Start Date and End Date
            var query = new QueryExpression("zox_tasknotificationandrecommendation")
            {
                ColumnSet = new ColumnSet("zox_event", "statecode", "createdon", "modifiedby"),
                Criteria = new FilterExpression
                {
                    Conditions =
                    {
                        new ConditionExpression("createdon", ConditionOperator.GreaterEqual, startDate),
                        new ConditionExpression("createdon", ConditionOperator.LessEqual, endDate),
                        new ConditionExpression("modifiedby", ConditionOperator.Equal, userId) // Filter by User ID
                    }
                }
            };

            var events = _client.RetrieveMultiple(query).Entities;

            // Step 2: Initialize Variables for Counting
            int totalEvents = events.Count;
            int successfulSequences = 0;

            // Step 3: Loop through each event and check if the next event was triggered
            foreach (var currentEvent in events)
            {
                // Get the current event and the next event (logic for next event based on 'Interval')
                var currentEventName = currentEvent.GetAttributeValue<string>("zox_event");
                var nextEvent = GetNextEvent(currentEventName); //  this function returns the next event name

                // Check if the next event was triggered
                var nextEventTriggered = events.Any(e => e.GetAttributeValue<string>("zox_event") == nextEvent);

                // If the next event was triggered, increment the successful sequence count
                if (nextEventTriggered)
                {
                    successfulSequences++;
                }
            }

            // Step 4: Calculate the Event Sequence Success Percentage
            float successPercentage = totalEvents > 0
                ? ((float)successfulSequences / totalEvents) * 100
                : 0f;

            // Return the result
            return new EventSequenceValidationDto
            {
                TotalEvents = totalEvents,
                SuccessfulSequences = successfulSequences,
                SuccessPercentage = (float)Math.Round(successPercentage, 2)
            };
        }

        // Function to get the next event (this should be replaced with logic to fetch the correct next event)
        private string GetNextEvent(string currentEventName)
        {
            //  logic to determine next event 
            switch (currentEventName)
            {
                case "Foundation Started":
                    return "Site Commencement Started";
                case "Site Commencement Started":
                    return "Plastering Started";
                case "Plastering Started":
                    return "Storm Water Drainage Started";
                default:
                    return string.Empty;
            }
        }
    }

    // DTO to return the event sequence validation data
    public class EventSequenceValidationDto
    {
        public int TotalEvents { get; set; }
        public int SuccessfulSequences { get; set; }
        public float SuccessPercentage { get; set; }
    }
}
