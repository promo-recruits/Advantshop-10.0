//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

using AdvantShop.Core.SQL;
using AdvantShop.Helpers;
using AdvantShop.Orders;

namespace AdvantShop.Mails
{
    public class MailAnswerTemplateService
    {
        public MailAnswerTemplate Get(int id, bool onlyActive)
        {
            return SQLDataAccess.Query<MailAnswerTemplate>(
                "SELECT * FROM [Settings].[MailTemplate] WHERE TemplateId = @id" + (onlyActive ? " AND Active=1" : string.Empty),
                new { id = id }).FirstOrDefault();
        }

        public List<MailAnswerTemplate> Gets(bool onlyActive)
        {
            return
                SQLDataAccess.ExecuteReadList<MailAnswerTemplate>(
                    "SELECT * FROM [Settings].[MailTemplate] " + (onlyActive ? " Where Active=1 " : string.Empty) + "Order By SortOrder",
                    CommandType.Text,
                    GetFromReader);
        }

        public void Update(MailAnswerTemplate mailTemplate)
        {
            SQLDataAccess.ExecuteNonQuery(
                "UPDATE [Settings].[MailTemplate] SET Name = @Name, Body = @Body, SortOrder = @SortOrder, Active = @Active, Subject = @Subject WHERE TemplateId = @TemplateId",
                CommandType.Text,
                new SqlParameter("@TemplateId", mailTemplate.TemplateId),
                new SqlParameter("@Name", mailTemplate.Name),
                new SqlParameter("@Body", mailTemplate.Body),
                new SqlParameter("@Subject", mailTemplate.Subject),
                new SqlParameter("@SortOrder", mailTemplate.SortOrder),
                new SqlParameter("@Active", mailTemplate.Active));
        }

        public int Add(MailAnswerTemplate mailTemplate)
        {
            mailTemplate.TemplateId =
                SQLDataAccess.ExecuteScalar<int>(
                    "INSERT INTO [Settings].[MailTemplate] (Name, Body, SortOrder, Active, Subject) VALUES (@Name, @Body, @SortOrder, @Active, @Subject); SELECT SCOPE_IDENTITY()",
                    CommandType.Text,
                    new SqlParameter("@TemplateId", mailTemplate.TemplateId),
                    new SqlParameter("@Name", mailTemplate.Name),
                    new SqlParameter("@Body", mailTemplate.Body),
                    new SqlParameter("@Subject", mailTemplate.Subject),
                    new SqlParameter("@SortOrder", mailTemplate.SortOrder),
                    new SqlParameter("@Active", mailTemplate.Active));

            return mailTemplate.TemplateId;
        }

        public void Delete(int templateId)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Settings].[MailTemplate] WHERE TemplateId = @TemplateId",
                CommandType.Text, new SqlParameter("@TemplateId", templateId));
        }

        public MailAnswerTemplate GetLetterWithTemplate(int templateId, string firstName, string lastName,
                                                        string patronymicName, string shopName, string trackNumber, string text, string managerName,
                                                        string managerSign, Guid? customerId, bool notFormatted = false)
        {
            var template = Get(templateId, true);
            if (template == null)
                return null;

            if (notFormatted)
                return template;

            template.Subject = FormatLetter(template.Subject, firstName, lastName, patronymicName, shopName, trackNumber,
                                            text, managerName, managerSign, customerId);

            template.Body = FormatLetter(template.Body, firstName, lastName, patronymicName, shopName, trackNumber, text,
                                         managerName, managerSign, customerId);

            return template;
        }

        public string FormatLetter(string body, string firstName, string lastName, string patronymicName,
                                   string shopName, string trackNumber, string text, string managerName, string managerSign, Guid? customerId)
        {
            var format = body;
            format = format.Replace("#FIRSTNAME#", firstName);
            format = format.Replace("#LASTNAME#", lastName);
            format = format.Replace("#PATRONYMIC#", patronymicName);
            format = format.Replace("#STORE_NAME#", shopName);
            format = format.Replace("#TRACKNUMBER#", trackNumber);
            format = format.Replace("#TEXT#", text);
            format = format.Replace("#MANAGER_NAME#", managerName);
            format = format.Replace("#MANAGER_SIGN#", (managerSign ?? string.Empty).Replace("\n", "<br />"));

            if (customerId != null && format.Contains("#LAST_ORDER_NUMBER#"))
            {
                var order = OrderService.GetCustomerOrderHistory(customerId.Value).FirstOrDefault();
                if (order != null)
                {
                    format = format.Replace("#LAST_ORDER_NUMBER#", order.OrderNumber);
                }
            }

            return format;
        }

        private MailAnswerTemplate GetFromReader(SqlDataReader reader)
        {
            {
                return new MailAnswerTemplate
                {
                    TemplateId = SQLDataHelper.GetInt(reader, "TemplateId"),
                    Name = SQLDataHelper.GetString(reader, "Name"),
                    Body = SQLDataHelper.GetString(reader, "Body"),
                    Subject = SQLDataHelper.GetString(reader, "Subject"),
                    SortOrder = SQLDataHelper.GetInt(reader, "SortOrder"),
                    Active = SQLDataHelper.GetBoolean(reader, "Active")
                };
            }
        }
    }
}
