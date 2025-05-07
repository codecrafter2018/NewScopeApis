

//using Microsoft.Xrm.Sdk.Query;
//using Microsoft.PowerPlatform.Dataverse.Client;
//using System;
//using System.Collections.Generic;
//using Microsoft.Xrm.Sdk;
//using Microsoft.PowerPlatform.Dataverse.Client.Extensions;
//using Microsoft.Extensions.Caching.Memory;

//namespace Ultratechapis.Service
//{
//    public class SobService
//    {
//        private readonly ServiceClient _client;
//        private readonly IMemoryCache _cache;

//        public SobService(ServiceClient client, IMemoryCache cache)
//        {
//            _client = client;
//            _cache = cache;
//        }

//        public SobResult CalculateSOBForLoggedInUser()
//        {
//            var userId = _client.GetMyUserId();
//            if (userId == Guid.Empty)
//            {
//                throw new Exception("User ID not found");
//            }

//            string cacheKey = $"SobResult_{userId}";

//            if (_cache.TryGetValue(cacheKey, out SobResult cachedResult))
//            {
//                return cachedResult;
//            }



//            DateTime startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

//            var query = new QueryExpression("zox_opportunityproduct")
//            {
//                ColumnSet = new ColumnSet("ownerid", "zox_recommendationtype", "zox_productstatus", "modifiedon"),
//                Criteria = new FilterExpression
//                {
//                    Conditions =
//                    {
//                        new ConditionExpression("ownerid", ConditionOperator.Equal, userId),
//                        new ConditionExpression("modifiedon", ConditionOperator.OnOrAfter, startOfMonth),
//                        new ConditionExpression("zox_productstatus", ConditionOperator.Equal, 100000000)
//                    }
//                }
//            };

//            var results = _client.RetrieveMultiple(query);
//            Console.WriteLine($"Total records found for owner: {results.Entities.Count}");

//            int oldsob = 0;
//            int currentsob = 0;

//            foreach (var record in results.Entities)
//            {
//                var reco = record.GetAttributeValue<OptionSetValue>("zox_recommendationtype")?.Value;

//                if (reco == 100000000 || reco == 100000001 || reco == 100000002)
//                    currentsob++;

//                if (reco == 100000000 || reco == 100000001)
//                    oldsob++;
//            }

//            double sobPercentage = currentsob > 0 ? ((double)oldsob / currentsob) * 100 : 0;

//            var result = new SobResult
//            {
//                OwnerId = userId,
//                OldSOB = oldsob,
//                CurrentSOB = currentsob,
//                SOBPercentage = Math.Round(sobPercentage, 2)
//            };

//            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));
//            return result;
//        }

//        public SobResult CalculateSOBByUserId(Guid userId)
//        {
//            if (userId == Guid.Empty)
//            {
//                throw new ArgumentException("User ID is required.");
//            }

//            string cacheKey = $"SobResult_{userId}";

//            if (_cache.TryGetValue(cacheKey, out SobResult cachedResult))
//            {
//                return cachedResult;
//            }

//            DateTime startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

//            var query = new QueryExpression("zox_opportunityproduct")
//            {
//                ColumnSet = new ColumnSet("ownerid", "zox_recommendationtype", "zox_productstatus", "modifiedon"),
//                Criteria = new FilterExpression
//                {
//                    Conditions =
//                    {
//                        new ConditionExpression("ownerid", ConditionOperator.Equal, userId),
//                        new ConditionExpression("modifiedon", ConditionOperator.OnOrAfter, startOfMonth),
//                        new ConditionExpression("zox_productstatus", ConditionOperator.Equal, 100000000)
//                    }
//                }
//            };

//            var results = _client.RetrieveMultiple(query);
//            Console.WriteLine($" [UserId] Total records found: {results.Entities.Count}");

//            int oldsob = 0;
//            int currentsob = 0;

//            foreach (var record in results.Entities)
//            {
//                var reco = record.GetAttributeValue<OptionSetValue>("zox_recommendationtype")?.Value;

//                if (reco == 100000000 || reco == 100000001 || reco == 100000002)
//                    currentsob++;

//                if (reco == 100000000 || reco == 100000001)
//                    oldsob++;
//            }

//            double sobPercentage = currentsob > 0 ? ((double)oldsob / currentsob) * 100 : 0;

//            var result = new SobResult
//            {
//                OwnerId = userId,
//                OldSOB = oldsob,
//                CurrentSOB = currentsob,
//                SOBPercentage = Math.Round(sobPercentage, 2)
//            };

//            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));
//            return result;
//        }
//    }

//    public class SobResult
//    {
//        public Guid OwnerId { get; set; }
//        public int OldSOB { get; set; }
//        public int CurrentSOB { get; set; }
//        public double SOBPercentage { get; set; }
//    }
//}




//using Microsoft.Xrm.Sdk.Query;
//using Microsoft.PowerPlatform.Dataverse.Client;
//using System;
//using System.Collections.Generic;
//using Microsoft.Xrm.Sdk;
//using Microsoft.PowerPlatform.Dataverse.Client.Extensions;
//using Microsoft.Extensions.Caching.Memory;

//namespace Ultratechapis.Service
//{
//    public class SobService
//    {
//        private readonly ServiceClient _client;
//        private readonly IMemoryCache _cache;

//        public SobService(ServiceClient client, IMemoryCache cache)
//        {
//            _client = client;
//            _cache = cache;
//        }

//        // Calculate SOB Growth for a specific UserId (not the logged-in user)
//        public SobResult CalculateSOBGrowth(Guid userId, DateTime startDate, DateTime endDate)
//        {
//            if (userId == Guid.Empty)
//            {
//                throw new ArgumentException("User ID is required.");
//            }

//            string cacheKey = $"SobResult_{userId}_{startDate}_{endDate}";

//            if (_cache.TryGetValue(cacheKey, out SobResult cachedResult))
//            {
//                return cachedResult;
//            }

//            // Fetch Opportunity Products for the given date range (StartDate and EndDate)
//            var query = new QueryExpression("zox_opportunityproduct")
//            {
//                ColumnSet = new ColumnSet("ownerid", "zox_recommendationtype", "zox_productstatus", "modifiedon"),
//                Criteria = new FilterExpression
//                {
//                    Conditions =
//                {
//                    new ConditionExpression("ownerid", ConditionOperator.Equal, userId),
//                    new ConditionExpression("modifiedon", ConditionOperator.OnOrAfter, startDate),
//                    new ConditionExpression("modifiedon", ConditionOperator.OnOrBefore, endDate),
//                    new ConditionExpression("zox_productstatus", ConditionOperator.Equal, 100000000) // Closed as Won
//                }
//                }
//            };

//            var results = _client.RetrieveMultiple(query);
//            Console.WriteLine($"Total records found for owner: {results.Entities.Count}");

//            int oldsob = 0;
//            int currentsob = 0;

//            // Calculate the SOB for the date range
//            foreach (var record in results.Entities)
//            {
//                var reco = record.GetAttributeValue<OptionSetValue>("zox_recommendationtype")?.Value;

//                // For Old SOB, excluding Substitution
//                if (reco == 100000000 || reco == 100000001 || reco == 100000003 || reco == 100000004) // Cross Sell  || Upsell ||  suggested product || similar product

//                    oldsob++;

//                // For Current SOB, include all types (including Substitution)
//                if (reco == 100000000 || reco == 100000001 || reco == 100000002 || reco == 100000003 || reco == 100000004) // All types (including Substitution)

//                    currentsob++;
//            }

//            // Step 3: Get Previous Month SOB from the cache or return 0 if not available
//            int previousMonthSOB = GetPreviousMonthSOB(userId);

//            // Step 4: Add the previous month's SOB to the current month
//            int totalOldSOB = oldsob + previousMonthSOB;

//            // Step 5: Calculate SOB Growth Percentage
//            double sobPercentage = totalOldSOB > 0
//                ? ((double)currentsob - totalOldSOB) / totalOldSOB * 100
//                : 0;

//            var result = new SobResult
//            {
//                OwnerId = userId,
//                OldSOB = totalOldSOB,
//                CurrentSOB = currentsob,
//                SOBPercentage = Math.Round(sobPercentage, 2)
//            };

//            // Step 6: Store the result in cache
//            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));
//            return result;
//        }

//        // Function to get the previous month's SOB from cache, if available
//        private int GetPreviousMonthSOB(Guid userId)
//        {
//            string cacheKey = $"PreviousSOB_{userId}";

//            // Try to get Previous SOB from the cache
//            if (_cache.TryGetValue(cacheKey, out int previousSOB))
//            {
//                return previousSOB;
//            }

//            // If not found in cache, return 0 or calculate it
//            Console.WriteLine("Previous month SOB data not found. Using 0.");
//            return 0; // Returning 0 for the first calculation or when previous SOB is not available
//        }

//        // Store Previous SOB in cache for future use (e.g., for next month)
//        public void SetPreviousMonthSOB(Guid userId, int sob)
//        {
//            string cacheKey = $"PreviousSOB_{userId}";
//            _cache.Set(cacheKey, sob, TimeSpan.FromDays(30)); // Cache for 30 days or until next calculation
//        }
//    }

//}

//public class SobResult
//{
//    public Guid OwnerId { get; set; }
//    public int OldSOB { get; set; }
//    public int CurrentSOB { get; set; }
//    public double SOBPercentage { get; set; }
//}


//using Microsoft.Xrm.Sdk.Query;
//using Microsoft.PowerPlatform.Dataverse.Client;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Microsoft.Xrm.Sdk;
//using Microsoft.Extensions.Caching.Memory;

//namespace Ultratechapis.Service
//{
//    public class SobService
//    {
//        private readonly ServiceClient _client;
//        private readonly IMemoryCache _cache;

//        public SobService(ServiceClient client, IMemoryCache cache)
//        {
//            _client = client;
//            _cache = cache;
//        }

//        public SobGrowthResult CalculateSOBGrowth(Guid userId, DateTime startDate, DateTime endDate)
//        {
//            if (userId == Guid.Empty)
//            {
//                throw new ArgumentException("User ID is required.");
//            }

//            // Fetch Opportunity Products for the given user and date range
//            var query = new QueryExpression("zox_opportunityproduct")
//            {
//                ColumnSet = new ColumnSet("ownerid", "zox_recommendationtype", "zox_productstatus", "modifiedon"),
//                Criteria = new FilterExpression
//                {
//                    Conditions =
//                    {
//                        new ConditionExpression("ownerid", ConditionOperator.Equal, userId),
//                        new ConditionExpression("createdon", ConditionOperator.GreaterEqual, startDate),
//                        new ConditionExpression("createdon", ConditionOperator.LessEqual, endDate)
//                    }
//                }
//            };

//            var results = _client.RetrieveMultiple(query);

//            // Step 1: Initialize counts for Old SOB and Current SOB
//            int oldSOB = 0;
//            int currentSOB = 0;
//            int substitutionCount = 0;

//            // Loop through the opportunity products to calculate Old SOB and Current SOB
//            foreach (var record in results.Entities)
//            {
//                var reco = record.GetAttributeValue<OptionSetValue>("zox_recommendationtype")?.Value;

//                // For Old SOB, exclude Substitution (only include Cross Sell and Upsell)
//                if (reco == 100000000 || reco == 100000001 || reco == 100000003 || reco == 100000004) // Cross Sell , Upsell , suggested Product , similar Product
//                    oldSOB++;

//                // For Current SOB, include Cross Sell, Upsell, and Substitution
//                if (reco == 100000000 || reco == 100000001 || reco == 100000002 || reco == 100000003 || reco == 100000004) // Cross Sell, Upsell, Substitution , suggested Product , similar Product
//                    currentSOB++;

//                // Track Substitution count for calculating in Current SOB
//                if (reco == 100000002) // Substitution
//                    substitutionCount++;
//            }

//            // Step 2: Get Previous Month SOB from the cache or return 0 if not available
//            int previousMonthOldSOB = GetPreviousMonthSOB(userId, "Old");
//            int previousMonthCurrentSOB = GetPreviousMonthSOB(userId, "Current");

//            // Add Substitution count only to Current SOB
//            int totalOldSOB = oldSOB + previousMonthOldSOB; // Old SOB remains unaffected by Substitution
//            int totalCurrentSOB = currentSOB + previousMonthCurrentSOB + substitutionCount; // Add Substitution to Current SOB oldsob + substituion count + previous month substition count 

//            // Step 3: Calculate SOB Growth (Current SOB - Old SOB) / Old SOB * 100
//            double sobGrowth = totalOldSOB > 0 ? ((double)(totalCurrentSOB - totalOldSOB) / totalOldSOB) * 100 : 0;

//            // Step 4: Return the result
//            var result = new SobGrowthResult
//            {
//                OldSOB = totalOldSOB,
//                CurrentSOB = totalCurrentSOB,
//                SOBGrowth = Math.Round(sobGrowth, 2),
//                PreviousMonthOldSOB = previousMonthOldSOB,
//                PreviousMonthCurrentSOB = previousMonthCurrentSOB
//            };

//            // Step 5: Store the current month's SOB in cache for future use
//            SetPreviousMonthSOB(userId, "Old", oldSOB);
//            SetPreviousMonthSOB(userId, "Current", currentSOB);

//            return result;
//        }

//        // Function to get the previous month's SOB from cache, if available
//        private int GetPreviousMonthSOB(Guid userId, string sobType)
//        {
//            string cacheKey = $"{sobType}SOB_{userId}";

//            // Try to get Previous SOB from the cache
//            if (_cache.TryGetValue(cacheKey, out int previousSOB))
//            {
//                return previousSOB;
//            }

//            // If not found in cache, return 0 (or calculate if necessary)
//            return 0; // Returning 0 if no previous data is found
//        }

//        // Store Previous SOB (Old or Current) in cache for future use (e.g., for next month)
//        public void SetPreviousMonthSOB(Guid userId, string sobType, int sob)
//        {
//            string cacheKey = $"{sobType}SOB_{userId}";
//            _cache.Set(cacheKey, sob, TimeSpan.FromDays(30)); // Cache for 30 days or until next calculation
//        }
//    }

//    // DTO to hold the SOB Growth result
//    public class SobGrowthResult
//    {
//        public int OldSOB { get; set; }
//        public int CurrentSOB { get; set; }
//        public double SOBGrowth { get; set; }
//        public int PreviousMonthOldSOB { get; set; }  // Added Previous Month Old SOB
//        public int PreviousMonthCurrentSOB { get; set; }  // Added Previous Month Current SOB
//    }
//}



using Microsoft.Xrm.Sdk.Query;
using Microsoft.PowerPlatform.Dataverse.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Extensions.Caching.Memory;

namespace Ultratechapis.Service
{
    public class SobService
    {
        private readonly ServiceClient _client;
        private readonly IMemoryCache _cache;

        public SobService(ServiceClient client, IMemoryCache cache)
        {
            _client = client;
            _cache = cache;
        }

        public SobGrowthResult CalculateSOBGrowth(Guid userId, DateTime startDate, DateTime endDate)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID is required.");
            }

            // Fetch Opportunity Products for the given user and date range
            var query = new QueryExpression("zox_opportunityproduct")
            {
                ColumnSet = new ColumnSet("ownerid", "zox_recommendationtype", "zox_productstatus", "modifiedon"),
                Criteria = new FilterExpression
                {
                    Conditions =
                    {
                        new ConditionExpression("ownerid", ConditionOperator.Equal, userId),
                        new ConditionExpression("createdon", ConditionOperator.GreaterEqual, startDate),
                        new ConditionExpression("createdon", ConditionOperator.LessEqual, endDate)
                    }
                }
            };

            var results = _client.RetrieveMultiple(query);

            // Step 1: Initialize counts for Old SOB and Substitution count
            int oldSOB = 0;
            int substitutionCountCurrentMonth = 0;

            // Loop through the opportunity products to calculate Old SOB and Substitution count for current month
            foreach (var record in results.Entities)
            {
                var reco = record.GetAttributeValue<OptionSetValue>("zox_recommendationtype")?.Value;

                // For Old SOB, exclude Substitution 
                if (reco == 100000000 || reco == 100000001 || reco == 100000003 || reco == 100000004) // Cross Sell , Upsell , suggested Product , similar Product
                    oldSOB++;

                // Track Substitution count for the current month
                if (reco == 100000002) // Substitution
                    substitutionCountCurrentMonth++;
            } 

            // Step 2: Get Previous Month SOB from the cache or return 0 if not available
            int previousMonthCurrentSOB = GetPreviousMonthSOB(userId, "Current");
            int previousMonthOldSOB = GetPreviousMonthSOB(userId, "Old");

            // Step 3: Calculate Total Current SOB (Old SOB of current month + Substitution count of current month + Previous Month Current SOB)
            int totalCurrentSOB = oldSOB + substitutionCountCurrentMonth + previousMonthCurrentSOB;
            int totalOldSOB = oldSOB + previousMonthOldSOB; // Old SOB remains unaffected by Substitution


            // Step 4: Calculate SOB Growth (Current SOB - Old SOB) / Old SOB * 100
            double sobGrowth = totalOldSOB > 0 ? ((double)(totalCurrentSOB - totalOldSOB) / totalOldSOB) * 100 : 0;
            Console.WriteLine($"Total Current SOB: {totalCurrentSOB}, Total Old SOB: {totalOldSOB}, SOB Growth: {sobGrowth}");


            // Step 5: Return the result
            var result = new SobGrowthResult
            {
                OldSOB = totalOldSOB,
                CurrentSOB = totalCurrentSOB,
                SOBGrowth = Math.Round(sobGrowth, 2),
                PreviousMonthCurrentSOB = previousMonthCurrentSOB,
                PreviousMonthOldSOB = previousMonthOldSOB,
            };

            // Step 6: Store the current month's SOB in cache for future use
            SetPreviousMonthSOB(userId, "Current", totalCurrentSOB);
            SetPreviousMonthSOB(userId, "Old", oldSOB);

            return result;
        }

        // Function to get the previous month's Current SOB from cache, if available
        private int GetPreviousMonthSOB(Guid userId, string sobType)
        {
            string cacheKey = $"{sobType}SOB_{userId}";

            // Try to get Previous SOB from the cache
            if (_cache.TryGetValue(cacheKey, out int previousSOB))
            {
                return previousSOB;
            }

            // If not found in cache, return 0 (or calculate if necessary)
            return 0; // Returning 0 if no previous data is found
        }

        // Store Previous SOB (Current) in cache for future use (e.g., for next month)
        public void SetPreviousMonthSOB(Guid userId, string sobType, int sob)
        {
            string cacheKey = $"{sobType}SOB_{userId}";
            _cache.Set(cacheKey, sob, TimeSpan.FromDays(30)); // Cache for 30 days or until next calculation
        }
    }

    // DTO to hold the SOB Growth result
    public class SobGrowthResult
    {
        public int OldSOB { get; set; }
        public int CurrentSOB { get; set; }
        public double SOBGrowth { get; set; }
        public int PreviousMonthCurrentSOB { get; set; }  // Added Previous Month Current SOB
        public int PreviousMonthOldSOB { get; set; }  // Added Previous Month Old SOB
    }
}
