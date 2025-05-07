

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Ultratechapis.Data;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.PowerPlatform.Dataverse.Client.Extensions;
using Ultratechapis.Data;

namespace Ultratechapis.Service
{
    public class OfferService
    {
        private readonly ServiceClient _client;
        private readonly IMemoryCache _cache;

        public OfferService(Connect connect, IMemoryCache cache)
        {
            _client = connect.Client;
            _cache = cache;
        }

        public Dictionary<string, float> GetOfferAcceptanceRate()
        { //cache
            if (_cache.TryGetValue("OfferAcceptanceRateCache", out Dictionary<string, float> cachedRates))
            {
                return cachedRates;
            }

            var query = new QueryExpression("zox_opportunityproduct")
            {
                ColumnSet = new ColumnSet("modifiedby", "zox_productstatus")
            };
            query.Criteria.AddCondition("zox_productstatus", ConditionOperator.NotNull);

            var result = _client.RetrieveMultiple(query);
            var userDict = new Dictionary<string, Dictionary<string, int>>();
            var acceptanceRate = new Dictionary<string, float>();
            //var acceptanceRateResult = new Dictionary<string, object>();

            foreach (var entity in result.Entities)
            {
                var modifiedBy = entity.GetAttributeValue<EntityReference>("modifiedby")?.Name;
                var statusValue = entity.GetAttributeValue<OptionSetValue>("zox_productstatus")?.Value;

                if (modifiedBy == null || statusValue == null) continue;

                if (!userDict.ContainsKey(modifiedBy))
                {
                    userDict[modifiedBy] = new Dictionary<string, int> { { "Closed as Won", 0 }, { "Closed as Lost", 0 } };
                }

                if (statusValue == 100000000) userDict[modifiedBy]["Closed as Won"]++;
                else if (statusValue == 100000001) userDict[modifiedBy]["Closed as Lost"]++;
            }

            foreach (var user in userDict)
            {
                var won = user.Value["Closed as Won"];
                var lost = user.Value["Closed as Lost"];
                var total = won + lost;
                acceptanceRate[user.Key] = total > 0 ? (float)won / total : 0f;
                //float ocr = total > 0 ? (float)won / total : 0f;

                //acceptanceRateResult[user.Key] = new
                //{
                //    OCR = Math.Round(ocr, 2), // Round to 2 decimal places
                //    Won = won,
                //    Lost = lost
                //};
            }

            _cache.Set("OfferAcceptanceRateCache", acceptanceRate, TimeSpan.FromMinutes(10));
            return acceptanceRate;
        }

        //public Dictionary<string, float> GetLoggedInUserAcceptanceRate() 
        //{
        //    var userId = _client.GetMyUserId();
        //    if (userId == Guid.Empty)
        //    {
        //        throw new Exception("User ID not found");
        //    }

        //    string cacheKey = $"UserAcceptanceRate_{userId}";
        //    if (_cache.TryGetValue(cacheKey, out Dictionary<string, float> cachedUserRate))
        //    {
        //        return cachedUserRate;
        //    }

        //    var query = new QueryExpression("zox_opportunityproduct")
        //    {
        //        ColumnSet = new ColumnSet("modifiedby", "zox_productstatus")
        //    };
        //    query.Criteria.AddCondition("zox_productstatus", ConditionOperator.NotNull);
        //    query.Criteria.AddCondition("modifiedby", ConditionOperator.Equal, userId);

        //    var result = _client.RetrieveMultiple(query);
        //    var userStats = new Dictionary<string, int> { { "Closed as Won", 0 }, { "Closed as Lost", 0 } };
        //    string userName = null;

        //    foreach (var entity in result.Entities)
        //    {
        //        var modifiedBy = entity.GetAttributeValue<EntityReference>("modifiedby");
        //        if (modifiedBy == null) continue;

        //        userName ??= modifiedBy.Name;
        //        var status = entity.GetAttributeValue<OptionSetValue>("zox_productstatus")?.Value;

        //        if (status == 100000000) userStats["Closed as Won"]++;
        //        else if (status == 100000001) userStats["Closed as Lost"]++;
        //    }

        //    var total = userStats["Closed as Won"] + userStats["Closed as Lost"];
        //    var rate = total > 0 ? (float)userStats["Closed as Won"] / total : 0f;

        //    var resultDict = new Dictionary<string, float> { { userName ?? "Unknown", rate } };
        //    _cache.Set(cacheKey, resultDict, TimeSpan.FromMinutes(5));
        //    return resultDict;
        //}



        public Dictionary<string, object> GetUserAcceptanceRateById(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID is required.");
            }

            string cacheKey = $"UserAcceptanceRate_{userId}";
            if (_cache.TryGetValue(cacheKey, out Dictionary<string, object> cachedUserRate))
            {
                return cachedUserRate;
            }

            var query = new QueryExpression("zox_opportunityproduct")
            {
                ColumnSet = new ColumnSet("modifiedby", "zox_productstatus")
            };
            query.Criteria.AddCondition("zox_productstatus", ConditionOperator.NotNull);
            query.Criteria.AddCondition("modifiedby", ConditionOperator.Equal, userId);

            var result = _client.RetrieveMultiple(query);
            var userStats = new Dictionary<string, int> { { "Closed as Won", 0 }, { "Closed as Lost", 0 } };
            string userName = null;

            foreach (var entity in result.Entities)
            {
                var modifiedBy = entity.GetAttributeValue<EntityReference>("modifiedby");
                if (modifiedBy == null) continue;

                userName ??= modifiedBy.Name;
                var status = entity.GetAttributeValue<OptionSetValue>("zox_productstatus")?.Value;

                if (status == 100000000) userStats["Closed as Won"]++;
                else if (status == 100000001) userStats["Closed as Lost"]++; 
            }

            var total = userStats["Closed as Won"] + userStats["Closed as Lost"];
            float ocr = total > 0 ? (float)userStats["Closed as Won"] / total : 0f;

            var resultDict = new Dictionary<string, object>
    {
        {
            userName ?? "Unknown", new
            {
                OCR = Math.Round(ocr, 2),
                Won = userStats["Closed as Won"],
                Lost = userStats["Closed as Lost"]
            }
        }
    };

            _cache.Set(cacheKey, resultDict, TimeSpan.FromMinutes(5));
            return resultDict;
        }


    }

}