using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.PowerPlatform.Dataverse.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ultratechapis.Service
{
    public class ProductAccuracyService
    {
        private readonly ServiceClient _client;

        public ProductAccuracyService(ServiceClient client)
        {
            _client = client;
        }

        public ProductAccuracyDto CalculateProductAccuracy(Guid userId, Guid projectId, DateTime startDate, DateTime endDate)
        {
            // Step 1: Fetch Opportunity Products for a given UserId, ProjectId, and Date Range
            var query = new QueryExpression("zox_opportunityproduct")
            {
                ColumnSet = new ColumnSet("zox_productstatus", "zox_recommendationtype", "zox_project_", "modifiedon"),
                LinkEntities =
                {
                    new LinkEntity
                    {
                        LinkFromEntityName = "zox_opportunityproduct",
                        LinkFromAttributeName = "zox_project_",  // Linking Opportunity Product to Project
                        LinkToEntityName = "zox_project",
                        LinkToAttributeName = "zox_projectid",
                        Columns = new ColumnSet("zox_projectid", "zox_name"),
                        EntityAlias = "project"
                    }
                },
                Criteria = new FilterExpression
                {
                    Conditions =
                    {
                        new ConditionExpression("zox_project_", ConditionOperator.Equal, projectId),
                        new ConditionExpression("modifiedon", ConditionOperator.OnOrAfter, startDate),
                        new ConditionExpression("modifiedon", ConditionOperator.OnOrBefore, endDate),
                        new ConditionExpression("modifiedby", ConditionOperator.Equal, userId),  // Filter by UserId
                        new ConditionExpression("zox_productstatus", ConditionOperator.Equal, 100000000)
                    }
                }
            };

            var results = _client.RetrieveMultiple(query).Entities;

            // Step 2: Filter for Cross Sell, Upsell, Substitution recommendation types
            var recommendedProducts = results.Where(r =>
                r.GetAttributeValue<OptionSetValue>("zox_recommendationtype")?.Value == 100000000 || // Cross Sell
                r.GetAttributeValue<OptionSetValue>("zox_recommendationtype")?.Value == 100000001 || // Upsell
                r.GetAttributeValue<OptionSetValue>("zox_recommendationtype")?.Value == 100000002 ||   // Substitution
                r.GetAttributeValue<OptionSetValue>("zox_recommendationtype")?.Value == 100000003 ||   // suggested product 
                r.GetAttributeValue<OptionSetValue>("zox_recommendationtype")?.Value == 100000004    // similar product
            ).ToList();

            int totalRecommended = recommendedProducts.Count;

            // Step 3: Count products marked as "Closed as Won" (Approved)
            int closedAsWonCount = recommendedProducts.Count(r =>
            {
                var status = r.GetAttributeValue<OptionSetValue>("zox_productstatus")?.Value;
                return status == 100000000; // 100000000 = Closed as Won
            });

            // Step 4: Calculate Product Accuracy (Precision)
            float accuracy = totalRecommended > 0
                ? ((float)closedAsWonCount / totalRecommended) * 100
                : 0f;

            // Get Project Details (Project Name, Project Status, etc.)
            var projectName = results.FirstOrDefault()?.GetAttributeValue<AliasedValue>("project.zox_name")?.Value?.ToString() ?? "Unknown Project";

            // Step 5: Return the DTO with the calculated accuracy and product names
            return new ProductAccuracyDto
            {
                TotalRecommended = totalRecommended,
                ClosedAsWon = closedAsWonCount,
                Accuracy = (float)Math.Round(accuracy, 2),
                ProjectName = projectName,
            };
        }
    }

    public class ProductAccuracyDto
    {
        public int TotalRecommended { get; set; }
        public int ClosedAsWon { get; set; }
        public float Accuracy { get; set; }
        public string ProjectName { get; set; }  // Project name

    }
}

