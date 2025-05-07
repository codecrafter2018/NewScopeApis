using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.PowerPlatform.Dataverse.Client;
using System;
using System.Collections.Generic;

public class UserFilterService
{
    private readonly ServiceClient _client;

    public UserFilterService(ServiceClient client)
    {
        _client = client;
    }

    public List<FilteredUserDto> GetFilteredUsers()
    {
        var query = new QueryExpression("systemuser")
        {
            ColumnSet = new ColumnSet("systemuserid", "fullname", "zox_lob", "zox_role", "zox_segment"),
            Criteria = new FilterExpression
            {
                FilterOperator = LogicalOperator.And,
                Conditions =
                {
                    new ConditionExpression("zox_segment", ConditionOperator.Equal, 100000002), // Key Accounts
                    new ConditionExpression("zox_lob", ConditionOperator.Equal, 100000000), // Cement
                    new ConditionExpression("zox_role", ConditionOperator.In, new object[]
                    {
                        515140004, // HCR
                        515140005, // HPR
                        515140011  // ZTH
                    })
                }
            }
        };

        var result = _client.RetrieveMultiple(query);
        var users = new List<FilteredUserDto>();

        foreach (var entity in result.Entities)
        {
            users.Add(new FilteredUserDto
            {
                UserId = entity.Id,
                FullName = entity.GetAttributeValue<string>("fullname"),
                LOB = entity.GetAttributeValue<OptionSetValue>("zox_lob")?.Value ?? -1,
                Role = entity.GetAttributeValue<OptionSetValue>("zox_role")?.Value ?? -1,
                Segment = entity.GetAttributeValue<OptionSetValue>("zox_segment")?.Value ?? -1
            });
        }

        return users;
    }
}

public class FilteredUserDto
{
    public Guid UserId { get; set; }
    public string FullName { get; set; }
    public int LOB { get; set; }
    public int Role { get; set; }
    public int Segment { get; set; }
}
