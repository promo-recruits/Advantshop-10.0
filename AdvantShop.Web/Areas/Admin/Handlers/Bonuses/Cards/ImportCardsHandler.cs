using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.Bonuses.Model;
using AdvantShop.Core.Services.Bonuses.Model.Enums;
using AdvantShop.Core.Services.Bonuses.Service;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Statistic;
using AdvantShop.Web.Infrastructure.Handlers;
using CsvHelper;

namespace AdvantShop.Web.Admin.Handlers.Bonuses.Cards
{
    public class ImportCardsHandler : AbstractCommandHandler<bool>
    {
        private readonly byte[] _model;
        private readonly bool _accrueBonuses;

        public ImportCardsHandler(byte[] model, bool accrueBonuses = false)
        {
            _model = model;
            _accrueBonuses = accrueBonuses;
        }

        protected override bool Handle()
        {
            if (CommonStatistic.IsRun)
            {
                return true;
            }
            CommonStatistic.StartNew(() =>
                {
                    //add bonus
                    try
                    {
                        var items = ProcessFile(_model, reader =>
                                                                    {
                                                                        if (!(reader.CurrentRecord.Length == 10 || reader.CurrentRecord.Length == 11 )) return null;
                                                                        var t = new ImportCardDto
                                                                        {
                                                                            Mobile = reader[0].TryParseLong(),
                                                                            Email = reader[1],
                                                                            CardNumber = reader[2].TryParseLong(),
                                                                            LastName = reader[3],
                                                                            FirstName = reader[4],
                                                                            SecondName = reader[5],
                                                                            DateOfBirth = reader[6].TryParseDateTime(true),
                                                                            BonusAmount = reader[7].TryParseDecimal(),
                                                                            Grade = reader[9],
                                                                        };
                                                                        if (reader.CurrentRecord.Length == 11)
                                                                        {
                                                                            t.PurchaseSum = reader[10].TryParseDecimal();
                                                                        }
                                                                        return t;
                                                                    }, encoding: Encoding.GetEncoding("windows-1251"));

                        CommonStatistic.TotalRow = items.Count;
                        foreach (var item in items)
                        {
                            AddCard(item);

                            CommonStatistic.RowPosition++;
                            CommonStatistic.TotalUpdateRow++;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.Log.Error(ex);
                        CommonStatistic.WriteLog(ex.Message);
                    }
                    finally
                    {
                    }
                }, 
                "cards/importcards",
                "импорт карты");

            return true;
        }

        private void AddCard(ImportCardDto item)
        {
            var card = item.CardNumber != 0 ? CardService.Get(item.CardNumber) : null;
            if (card != null) {

                if (item.PurchaseSum.HasValue && item.PurchaseSum.Value != 0)
                {
                    var p = PurchaseService.HasImport(card.CardId, "Импорт");
                    if (p == null)
                    {
                        p = new Purchase
                        {
                            PurchaseAmount = item.PurchaseSum.Value,
                            CardId = card.CardId,
                            CreateOn = DateTime.Now,
                            CreateOnCut = DateTime.Now,
                            CashAmount = 0,
                            MainBonusAmount = 0,
                            AdditionBonusAmount = 0,
                            NewBonusAmount = 0,
                            Comment = "Импорт",
                            Status = EPuchaseState.Complete
                        };
                        PurchaseService.Add(p);
                    }
                }
                if (_accrueBonuses && item.BonusAmount > 0)
                {
                    card.BonusAmount += item.BonusAmount;
                    CardService.Update(card);
                    var transLog = Transaction.Factory(card.CardId, item.BonusAmount, "Импорт", EOperationType.AddMainBonus, card.BonusAmount);
                    TransactionService.Create(transLog);
                }
                return;
            }


            if (string.IsNullOrWhiteSpace(item.Email))
                return;

            var grade = GradeService.Get(item.Grade);
            var gradeid = grade != null ? grade.Id : BonusSystem.DefaultGrade;

            var customer = CustomerService.GetCustomerByEmail(item.Email);

            if (customer == null)
            {
                customer = new Customer(CustomerGroupService.DefaultCustomerGroup)
                {
                    LastName = item.LastName,
                    FirstName = item.FirstName,
                    Patronymic = item.SecondName,
                    BirthDay = item.DateOfBirth.HasValue
                    ? (item.DateOfBirth.Value < (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue ? null : item.DateOfBirth)
                    : null,
                    EMail = item.Email,
                    Phone = item.Mobile.ToString(),
                    StandardPhone = item.Mobile
                };
                customer.Id = CustomerService.InsertNewCustomer(customer);
            }
            else
            {
                var cardHave = CardService.Get(customer.Id);
                if (cardHave != null)
                {
                    if (_accrueBonuses && item.BonusAmount > 0)
                    {
                        cardHave.BonusAmount += item.BonusAmount;
                        CardService.Update(cardHave);
                        var transLog = Transaction.Factory(cardHave.CardId, item.BonusAmount, "Импорт", EOperationType.AddMainBonus, cardHave.BonusAmount);
                        TransactionService.Create(transLog);
                    }
                    return;
                }
            }

            if (customer.Id == Guid.Empty) return;

            if (item.CardNumber == 0)
            {
                item.CardNumber = BonusSystemService.GenerateCardNumber(0);
            }

            card = new Card
            {
                CardNumber = item.CardNumber,
                CardId = customer.Id,
                BonusAmount = item.BonusAmount,
                CreateOn = DateTime.Now,
                GradeId = gradeid
            };

            CardService.Add(card);
            if (item.BonusAmount > 0)
            {
                var transLog = Transaction.Factory(card.CardId, item.BonusAmount, "Импорт", EOperationType.AddMainBonus, card.BonusAmount);
                TransactionService.Create(transLog);
            }

            if (item.PurchaseSum.HasValue && item.PurchaseSum.Value != 0)
            {
                var p = PurchaseService.HasImport(customer.Id, "Импорт");
                if (p == null)
                {
                    p = new Purchase
                    {
                        PurchaseAmount = item.PurchaseSum.Value,
                        CardId = customer.Id,
                        CreateOn = DateTime.Now,
                        CreateOnCut = DateTime.Now,
                        CashAmount = 0,
                        MainBonusAmount = 0,
                        AdditionBonusAmount = 0,
                        NewBonusAmount = 0,
                        Comment = "Импорт",
                        Status = EPuchaseState.Complete
                    };
                    PurchaseService.Add(p);
                }
            }
        }

        private List<TResult> ProcessFile<TResult>(byte[] data, Func<CsvReader, TResult> function, TResult defaultValue = default(TResult), Encoding encoding = null, string delimetr = ";", bool hasHeaderRecord = true)
        {
            var result = new List<TResult>();
            using (var csv = new CsvReader(new StreamReader(new MemoryStream(data), encoding ?? Encoding.UTF8)))
            {
                csv.Configuration.Delimiter = delimetr;
                csv.Configuration.HasHeaderRecord = hasHeaderRecord;
                while (csv.Read())
                {
                    var item = function != null ? function(csv) : defaultValue;
                    if (item != null)
                        result.Add(item);
                }
            }
            return result;
        }

        private class ImportCardDto
        {
            public string Email { get; set; }
            public long CardNumber { get; set; }
            public long Mobile { get; set; }
            public string LastName { get; set; }
            public string FirstName { get; set; }
            public string SecondName { get; set; }
            public DateTime? DateOfBirth { get; set; }
            public decimal BonusAmount { get; set; }
            public string Grade { get; set; }
            public decimal? PurchaseSum { get; set; }
        }
    }
}
