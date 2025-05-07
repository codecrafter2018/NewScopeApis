using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.PowerPlatform.Dataverse.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ultratechapis.Service
{
    public class OpportunityConversionService
    {
        private readonly ServiceClient _client;

        public OpportunityConversionService(ServiceClient client)
        {
            _client = client;
        }

        public OpportunityConversionDto CalculateOpportunityConversion(Guid userId, DateTime startDate, DateTime endDate)
        {
            var query = new QueryExpression("zox_opportunityproduct")
            {
                ColumnSet = new ColumnSet("zox_recommendationtype", "zox_productstatus", "modifiedby", "createdon"),
                Criteria = new FilterExpression
                {
                    Conditions =
                    {
                          new ConditionExpression("createdon", ConditionOperator.GreaterEqual, startDate),
                        new ConditionExpression("createdon", ConditionOperator.LessEqual, endDate),
                        new ConditionExpression("modifiedby", ConditionOperator.Equal, userId)
                    }
                }
            };

            var results = _client.RetrieveMultiple(query).Entities;

            // Filter: Recommendation Type = Cross Sell, Upsell, Substitution
            var filteredRecommendations = results.Where(r =>
            {
                var recType = r.GetAttributeValue<OptionSetValue>("zox_recommendationtype")?.Value;
                return recType == 100000000 || recType == 100000001 || recType == 100000002 || recType == 100000003 || recType == 100000004;
            }).ToList();

            int totalRecommended = filteredRecommendations.Count;

            int closedAsWonCount = filteredRecommendations.Count(r =>
            {
                var status = r.GetAttributeValue<OptionSetValue>("zox_productstatus")?.Value;
                return status == 100000000; //Closed as Won
            });

            float conversionRate = totalRecommended > 0
                ? ((float)closedAsWonCount / totalRecommended) * 100
                : 0f;

            return new OpportunityConversionDto
            {
                TotalRecommended = totalRecommended,
                ClosedAsWon = closedAsWonCount,
                ConversionRate = (float)Math.Round(conversionRate, 2)
            };
        }
    }

    public class OpportunityConversionDto
    {
        public int TotalRecommended { get; set; }
        public int ClosedAsWon { get; set; }
        public float ConversionRate { get; set; } // Percentage
    }
}
