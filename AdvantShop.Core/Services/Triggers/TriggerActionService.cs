using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Core.Scheduler;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;
using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Triggers
{
    public class TriggerActionService
    {
        public static List<TriggerAction> GetTriggerActions(int triggerRuleId)
        {
            return SQLDataAccess.ExecuteReadList(
                "Select * From CRM.TriggerAction Where TriggerRuleId=@TriggerRuleId Order by SortOrder asc, Id asc",
                CommandType.Text, GetReader,
                new SqlParameter("@TriggerRuleId", triggerRuleId));
        }

        public static TriggerAction GetTriggerAction(int id)
        {
            return SQLDataAccess.ExecuteReadOne("Select * From CRM.TriggerAction Where Id=@id", 
                CommandType.Text, GetReader, 
                new SqlParameter("@id", id));
        }

        public static TriggerAction GetTriggerAction(Guid emailingId)
        {
            return SQLDataAccess.ExecuteReadOne("Select * From CRM.TriggerAction Where EmailingId=@EmailingId",
                CommandType.Text, GetReader,
                new SqlParameter("@EmailingId", emailingId));
        }

        private static TriggerAction GetReader(SqlDataReader reader)
        {
            var requestParams = SQLDataHelper.GetString(reader, "RequestParams");
            var requestHeaderParams = SQLDataHelper.GetString(reader, "RequestHeaderParams");

            return new TriggerAction()
            {
                Id = SQLDataHelper.GetInt(reader, "Id"),
                TriggerRuleId = SQLDataHelper.GetInt(reader, "TriggerRuleId"),
                ActionType = (ETriggerActionType) SQLDataHelper.GetInt(reader, "ActionType"),
                EmailSubject = SQLDataHelper.GetString(reader, "EmailSubject"),
                EmailBody = SQLDataHelper.GetString(reader, "EmailBody"),
                SmsText = SQLDataHelper.GetString(reader, "SmsText"),
                MessageText = SQLDataHelper.GetString(reader, "MessageText"),
                TimeDelay = JsonConvert.DeserializeObject<TimeInterval>(SQLDataHelper.GetString(reader, "TimeDelay")),
                EditField = new EditField()
                {
                    Type = SQLDataHelper.GetNullableInt(reader, "EditFieldType"),
                    ObjId = SQLDataHelper.GetNullableInt(reader, "ObjId"),
                    EditFieldValue = SQLDataHelper.GetString(reader, "EditFieldValue"),
                    DealStatusId = SQLDataHelper.GetNullableInt(reader, "DealStatusId"),
                },
                EmailingId = SQLDataHelper.GetGuid(reader, "EmailingId"),
                SendRequestData = new TriggerActionSendRequestData()
                {
                    RequestMethod = (TriggerActionSendRequestMethod)SQLDataHelper.GetInt(reader, "RequestMethod", 0),
                    RequestUrl = SQLDataHelper.GetString(reader, "RequestUrl"),
                    RequestParams = 
                        !string.IsNullOrEmpty(requestParams)
                            ? JsonConvert.DeserializeObject<List<TriggerActionSendRequestParam>>(requestParams) 
                            : null,
                    RequestHeaderParams =
                        !string.IsNullOrEmpty(requestHeaderParams)
                            ? JsonConvert.DeserializeObject<List<TriggerActionSendRequestParam>>(requestHeaderParams)
                            : null,
                    RequestParamsType = (TriggerActionSendRequestParamsType)SQLDataHelper.GetInt(reader, "RequestParamsType"),
                    RequestParamsJson = SQLDataHelper.GetString(reader, "RequestParamsJson")
                },
                SortOrder = SQLDataHelper.GetInt(reader, "SortOrder")
            };
        }

        public static int Add(TriggerAction action)
        {
            action.Id =
                SQLDataAccess.ExecuteScalar<int>(
                    "Insert Into CRM.TriggerAction (TriggerRuleId, ActionType, TimeDelay, EmailSubject, EmailBody, SmsText, EditFieldType, EditFieldValue, ObjId, EmailingId, DealStatusId, RequestMethod, RequestUrl, RequestParams, RequestHeaderParams, SortOrder, RequestParamsType, MessageText) " +
                    "Values (@TriggerRuleId, @ActionType, @TimeDelay, @EmailSubject, @EmailBody, @SmsText, @EditFieldType, @EditFieldValue, @ObjId, @EmailingId, @DealStatusId, @RequestMethod, @RequestUrl, @RequestParams, @RequestHeaderParams, @SortOrder, @RequestParamsType, @MessageText); Select scope_identity();",
                    CommandType.Text,
                    new SqlParameter("@TriggerRuleId", action.TriggerRuleId),
                    new SqlParameter("@ActionType", action.ActionType),
                    new SqlParameter("@TimeDelay",
                        action.TimeDelay != null ? JsonConvert.SerializeObject(action.TimeDelay) : ""),
                    new SqlParameter("@EmailSubject", action.EmailSubject ?? (object) DBNull.Value),
                    new SqlParameter("@EmailBody", action.EmailBody ?? (object) DBNull.Value),
                    new SqlParameter("@SmsText", action.SmsText ?? (object) DBNull.Value),
                    new SqlParameter("@MessageText", action.MessageText ?? (object)DBNull.Value),
                    new SqlParameter("@EditFieldType", (action.EditField != null ? action.EditField.Type : null) ?? (object) DBNull.Value),
                    new SqlParameter("@ObjId", (action.EditField != null ? action.EditField.ObjId : null) ?? (object) DBNull.Value),
                    new SqlParameter("@EditFieldValue", (action.EditField != null ? action.EditField.EditFieldValue : null) ?? (object) DBNull.Value),
                    new SqlParameter("@DealStatusId", (action.EditField != null ? action.EditField.DealStatusId : null) ?? (object) DBNull.Value),
                    new SqlParameter("@EmailingId", Guid.NewGuid()),

                    new SqlParameter("@RequestMethod", action.SendRequestData != null ? (int) action.SendRequestData.RequestMethod : 0),
                    new SqlParameter("@RequestUrl",
                        (action.SendRequestData != null ? action.SendRequestData.RequestUrl : null) ??
                        (object) DBNull.Value),
                    new SqlParameter("@RequestParams",
                        action.SendRequestData != null && action.SendRequestData.RequestParams != null
                            ? JsonConvert.SerializeObject(action.SendRequestData.RequestParams)
                            : (object) DBNull.Value),
                    new SqlParameter("@RequestHeaderParams",
                        action.SendRequestData != null && action.SendRequestData.RequestHeaderParams != null
                            ? JsonConvert.SerializeObject(action.SendRequestData.RequestHeaderParams)
                            : (object) DBNull.Value),

                    new SqlParameter("@RequestParamsType", action.SendRequestData != null ? (int)action.SendRequestData.RequestParamsType : 0),
                    new SqlParameter("@RequestParamsJson", 
                        action.SendRequestData != null && action.SendRequestData.RequestParamsJson != null 
                            ? action.SendRequestData.RequestParamsJson 
                            : (object)DBNull.Value),

                    new SqlParameter("@SortOrder", action.SortOrder)
                );

            return action.Id;
        }

        public static void Update(TriggerAction action)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update CRM.TriggerAction " +
                "Set TriggerRuleId=@TriggerRuleId, ActionType=@ActionType, TimeDelay=@TimeDelay, EmailSubject=@EmailSubject, EmailBody=@EmailBody, SmsText=@SmsText, EditFieldType=@EditFieldType, EditFieldValue=@EditFieldValue, ObjId=@ObjId, " +
                "DealStatusId=@DealStatusId, RequestMethod=@RequestMethod, RequestUrl=@RequestUrl, RequestParams=@RequestParams, RequestHeaderParams=@RequestHeaderParams, SortOrder=@SortOrder, " +
                "RequestParamsType=@RequestParamsType, RequestParamsJson=@RequestParamsJson, MessageText=@MessageText " +
                "Where Id=@Id",
                CommandType.Text,
                new SqlParameter("@Id", action.Id),
                new SqlParameter("@TriggerRuleId", action.TriggerRuleId),
                new SqlParameter("@ActionType", action.ActionType),
                new SqlParameter("@TimeDelay",
                    action.TimeDelay != null ? JsonConvert.SerializeObject(action.TimeDelay) : ""),
                new SqlParameter("@EmailSubject", action.EmailSubject ?? (object) DBNull.Value),
                new SqlParameter("@EmailBody", action.EmailBody ?? (object) DBNull.Value),
                new SqlParameter("@SmsText", action.SmsText ?? (object) DBNull.Value),
                new SqlParameter("@MessageText", action.MessageText ?? (object)DBNull.Value),
                new SqlParameter("@EditFieldType",
                    (action.EditField != null ? action.EditField.Type : null) ?? (object) DBNull.Value),
                new SqlParameter("@ObjId",
                    (action.EditField != null ? action.EditField.ObjId : null) ?? (object) DBNull.Value),
                new SqlParameter("@EditFieldValue",
                    (action.EditField != null ? action.EditField.EditFieldValue : null) ?? (object) DBNull.Value),
                new SqlParameter("@DealStatusId",
                    (action.EditField != null ? action.EditField.DealStatusId : null) ?? (object) DBNull.Value),

                new SqlParameter("@RequestMethod",
                    action.SendRequestData != null ? (int) action.SendRequestData.RequestMethod : 0),
                new SqlParameter("@RequestUrl",
                    (action.SendRequestData != null ? action.SendRequestData.RequestUrl : null) ??
                    (object) DBNull.Value),
                new SqlParameter("@RequestParams",
                    action.SendRequestData != null && action.SendRequestData.RequestParams != null
                        ? JsonConvert.SerializeObject(action.SendRequestData.RequestParams)
                        : (object) DBNull.Value),
                new SqlParameter("@RequestHeaderParams",
                    action.SendRequestData != null && action.SendRequestData.RequestHeaderParams != null
                        ? JsonConvert.SerializeObject(action.SendRequestData.RequestHeaderParams)
                        : (object) DBNull.Value),

                new SqlParameter("@RequestParamsType",
                    action.SendRequestData != null ? (int) action.SendRequestData.RequestParamsType : 0),
                new SqlParameter("@RequestParamsJson",
                    action.SendRequestData != null && action.SendRequestData.RequestParamsJson != null
                        ? action.SendRequestData.RequestParamsJson
                        : (object) DBNull.Value),

                new SqlParameter("@SortOrder", action.SortOrder)
            );
        }

        public static void Delete(int id)
        {
            SQLDataAccess.ExecuteNonQuery("Delete From CRM.TriggerAction Where Id=@id", CommandType.Text, new SqlParameter("@id", id));
        }
    }
}
