using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.PowerPlatform.Dataverse.Client;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Ultratechapis.Service
{
    public class UserHierarchyService
    {
        private readonly ServiceClient _client;

private static readonly Dictionary<int, string> RoleLabels = new Dictionary<int, string>
{
    { 100000000, "RBBL" },
    { 100000001, "CRM" },
    { 100000002, "ZCMH" },
    { 100000003, "ZTMH" },
    { 100000004, "TSH" },
    { 100000005, "RHM" },
    { 100000006, "ZHM" },
    { 100000007, "ZBBH" },
    { 100000008, "AM" },
    { 515140001, "AH" },
    { 515140002, "LASF" },
    { 515140003, "TM" },
    { 515140004, "HCR" },
    { 515140005, "HPR" },
    { 515140006, "Head" },
    { 515140007, "SM" },
    { 515140008, "NSH" },
    { 515140009, "SE" },
    { 515140010, "RTH" },
    { 515140011, "ZTH" },
    { 515140012, "Technical Head" },
    { 515140013, "Head Key" }
};
        public UserHierarchyService(ServiceClient client)
        {
            _client = client;
        }

        public List<TeamMemberDto> GetTeamByManager(string managerName = null, Guid? managerId = null)

        {
            var query = new QueryExpression("systemuser")
            {
                ColumnSet = new ColumnSet("fullname", "systemuserid", "zox_role", "parentsystemuserid"),
                Criteria = new FilterExpression
                {
                    Conditions =
                {
                    new ConditionExpression("isdisabled", ConditionOperator.Equal, false) // Only active users
                }
                }
            };

            var users = _client.RetrieveMultiple(query).Entities;

            // Filter users who have a manager
            var filteredTeam = users
                .Where(u =>
                    u.Contains("parentsystemuserid") &&
                    (
                        (managerId.HasValue &&
                         u.GetAttributeValue<EntityReference>("parentsystemuserid")?.Id == managerId.Value)
                        ||
                        (!string.IsNullOrWhiteSpace(managerName) &&
                         string.Equals(
                            u.GetAttributeValue<EntityReference>("parentsystemuserid")?.Name?.Trim(),
                            managerName.Trim(),
                            StringComparison.OrdinalIgnoreCase)
                        )
                    )
                )
                //.Select(u => new TeamMemberDto
                //{
                //    UserId = u.Id,
                //    FullName = u.GetAttributeValue<string>("fullname") ?? "N/A",
                //    Role = u.GetAttributeValue<OptionSetValue>("zox_role")?.Value ?? -1
                //})
                //.ToList();
                .Select(u =>
                {
                    var roleValue = u.GetAttributeValue<OptionSetValue>("zox_role")?.Value ?? -1;
                    var roleName = RoleLabels.ContainsKey(roleValue) ? RoleLabels[roleValue] : $"Unknown ({roleValue})";

                    return new TeamMemberDto
                    {
                        UserId = u.Id,
                        FullName = u.GetAttributeValue<string>("fullname") ?? "N/A",
                        Role = roleName
                    };
                })
                .ToList();


            return filteredTeam;
        }

        public List<ManagerDto> GetAllManagers()
        {
            var query = new QueryExpression("systemuser")
            {
                ColumnSet = new ColumnSet("systemuserid", "fullname", "parentsystemuserid"),
                Criteria = new FilterExpression
                {
                    Conditions =
            {
                new ConditionExpression("isdisabled", ConditionOperator.Equal, false)
            }
                }
            };

            var users = _client.RetrieveMultiple(query).Entities;

            var managerRefs = users
                .Where(u => u.Contains("parentsystemuserid"))
                .Select(u => u.GetAttributeValue<EntityReference>("parentsystemuserid"))
                .Where(m => m != null)
                .GroupBy(m => m.Id)
                .Select(g => g.First())
                .ToList();

            var managers = managerRefs.Select(m => new ManagerDto
            {
                ManagerId = m.Id,
                ManagerName = m.Name ?? "Unknown"
            }).ToList();

            return managers;
        }

        public List<UserWithManagerDto> GetAllUsersWithManagers()
        {
            var query = new QueryExpression("systemuser")
            {
                ColumnSet = new ColumnSet("fullname", "systemuserid", "parentsystemuserid"),
                Criteria = new FilterExpression
                {
                    Conditions =
            {
                new ConditionExpression("isdisabled", ConditionOperator.Equal, false)
            }
                }
            };

            var result = _client.RetrieveMultiple(query).Entities;

            var mappedUsers = result
                .Where(user => user.Contains("parentsystemuserid")) 
                .Select(user =>
                {
                    var managerRef = user.GetAttributeValue<EntityReference>("parentsystemuserid");

                    return new UserWithManagerDto
                    {
                        UserId = user.Id,
                        UserName = user.GetAttributeValue<string>("fullname") ?? "Unknown",
                        ManagerId = managerRef?.Id,
                        ManagerName = managerRef?.Name ?? "No Manager"
                    };
                }).ToList();

            return mappedUsers;
        }

    }

    public class UserWithManagerDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public Guid? ManagerId { get; set; }
        public string ManagerName { get; set; }
    }


    public class ManagerDto
    {
        public Guid ManagerId { get; set; }
        public string ManagerName { get; set; }
    }

    public class ManagerHierarchyDto
    {
        public Guid ManagerId { get; set; }

        public string ManagerName { get; set; }
        public List<TeamMemberDto> Team { get; set; }
    }

    public class TeamMemberDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; }
       
        public string Role { get; set; }
    }
}

