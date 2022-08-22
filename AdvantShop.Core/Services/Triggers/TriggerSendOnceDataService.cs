using System;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Core.SQL;

namespace AdvantShop.Core.Services.Triggers
{
    public class TriggerSendOnceDataService
    {
        public static void Add(TriggerSendOnceData data)
        {
            SQLDataAccess.ExecuteNonQuery(
                "if not Exists(Select * From [CRM].[TriggerSendOnceData] Where TriggerId=@TriggerId and EntityId=@EntityId and CustomerId=@CustomerId) " +
                "Insert Into [CRM].[TriggerSendOnceData] (TriggerId, EntityId, CustomerId) Values (@TriggerId, @EntityId, @CustomerId)",
                CommandType.Text,
                new SqlParameter("@TriggerId", data.TriggerId),
                new SqlParameter("@EntityId", data.EntityId),
                new SqlParameter("@CustomerId", data.CustomerId));
        }

        public static bool IsExist(int triggerId, int entityId, Guid customerId)
        {
            var exists =
                SQLDataAccess.ExecuteScalar<int>(
                    "if Exists(Select * From [CRM].[TriggerSendOnceData] Where TriggerId=@TriggerId and CustomerId=@CustomerId and EntityId=@EntityId) " +
                    "Select 1 " +
                    "else " +
                    "Select 0",
                    CommandType.Text,
                    new SqlParameter("@TriggerId", triggerId),
                    new SqlParameter("@EntityId", entityId),
                    new SqlParameter("@CustomerId", customerId));

            return exists == 1;
        }

    }
}
