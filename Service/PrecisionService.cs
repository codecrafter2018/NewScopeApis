

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.PowerPlatform.Dataverse.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ultratechapis.Service
{
    public class PrecisionService
    {
        private readonly ServiceClient _client;

        public PrecisionService(ServiceClient client)
        {
            _client = client;
        }

        public PrecisionDto CalculatePrecision(Guid userId, DateTime startDate, DateTime endDate)
        {
            var query = new QueryExpression("zox_opportunityproduct")
            {
                ColumnSet = new ColumnSet("zox_recommendationtype", "zox_clientstatus", "zox_contractorstatus", "modifiedby", "createdon"),
                Criteria = new FilterExpression
                {
                    Conditions =
                    {    new ConditionExpression("createdon", ConditionOperator.GreaterEqual, startDate),
                        new ConditionExpression("createdon", ConditionOperator.LessEqual, endDate),
                        new ConditionExpression("modifiedby", ConditionOperator.Equal, userId)
                    }
                }
            };

            var results = _client.RetrieveMultiple(query).Entities;

            // Step 1: Filter for Cross Sell, Upsell, Substitution , suggested Product , Similar product
            var recommendedProducts = results.Where(r =>
            {
                var recType = r.GetAttributeValue<OptionSetValue>("zox_recommendationtype")?.Value;
                return recType == 100000000 || recType == 100000001 || recType == 100000002 || recType == 100000003 || recType == 100000004;
            }).ToList();

            int totalRecommended = recommendedProducts.Count;

            // Step 2: Count both Client and Contractor Approval
            int approvedCount = recommendedProducts.Count(r =>
            {
                var clientApproval = r.GetAttributeValue<OptionSetValue>("zox_clientstatus")?.Value;
                var contractorApproval = r.GetAttributeValue<OptionSetValue>("zox_contractorstatus")?.Value;
                return clientApproval == 515140001 && contractorApproval == 100000000; //  515140001 = Approved by client status &&  100000000 = Approved by contractor status
            });

            // Step 3: Count Not Approved (False Positive)
            int notApprovedCount = totalRecommended - approvedCount;

            float precision = (approvedCount + notApprovedCount) > 0
                ? ((float)approvedCount / (approvedCount + notApprovedCount)) * 100
                : 0f;

            return new PrecisionDto
            {
                TotalRecommended = totalRecommended,
                Approved = approvedCount,
                NotApproved = notApprovedCount,
                Precision = (float)Math.Round(precision, 2)
            };
        }
    }

    public class PrecisionDto
    {
        public int TotalRecommended { get; set; }
        public int Approved { get; set; }
        public int NotApproved { get; set; }
        public float Precision { get; set; }
    }
}
