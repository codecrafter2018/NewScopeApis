using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.PowerPlatform.Dataverse.Client;
using System;
using System.Collections.Generic;

namespace Ultratechapis.Service
{
    public class TimelineAccuracyService
    {
        private readonly ServiceClient _client;

        public TimelineAccuracyService(ServiceClient client)
        {
            _client = client;
        }

        public TimelineAccuracyDto CalculateTimelineAccuracy(Guid userId, DateTime startDate, DateTime endDate)
        {
            // Step 1: Query the Event Table for triggered events
            var query = new QueryExpression("zox_tasknotificationandrecommendation")
            {
                ColumnSet = new ColumnSet("zox_event", "createdon"),
                Criteria = new FilterExpression
                {
                    Conditions =
                    {
                        new ConditionExpression("createdon", ConditionOperator.GreaterEqual, startDate),
                        new ConditionExpression("createdon", ConditionOperator.LessEqual, endDate),
                        new ConditionExpression("ownerid", ConditionOperator.Equal, userId)  // User ID filtering
                    }
                }
            };

            var results = _client.RetrieveMultiple(query).Entities;

            // Step 2: Calculate Total Events Triggered within the specified date range
            int totalEventsTriggered = results.Count;

            // Step 3: Return the results
            return new TimelineAccuracyDto
            {
                TotalEventsTriggered = totalEventsTriggered,
            };
        }
    }

    public class TimelineAccuracyDto
    {
        public int TotalEventsTriggered { get; set; }
    }
}
