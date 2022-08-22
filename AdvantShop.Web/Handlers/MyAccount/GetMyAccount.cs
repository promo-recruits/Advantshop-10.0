using System;
using System.Collections.Generic;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.MyAccount;
using AdvantShop.Customers;
using AdvantShop.ViewModel.MyAccount;

namespace AdvantShop.Handlers.MyAccount
{
    public class GetMyAccount
    {
        private readonly Customer _customer;
        private readonly object _isRegisteredNow;

        public GetMyAccount(Customer customer, object isRegisteredNow)
        {
            _customer = customer;
            _isRegisteredNow = isRegisteredNow;
        }

        public MyAccountViewModel Execute()
        {
            var model = new MyAccountViewModel()
            {
                DisplayBonuses = BonusSystem.IsActive,
                DisplayChangeEmail = _customer.EMail.Contains("@temp"),
                Tabs = new List<MyAccountTab>(),
                IsRegisteredNow = _isRegisteredNow != null
            };

            if (model.DisplayBonuses)
            {
                var bonusCard = BonusSystemService.GetCard(_customer.Id);
                if (bonusCard != null)
                    model.BonusesAmount = bonusCard.BonusesTotalAmount;
            }

            foreach (var type in AttachedModules.GetModules<IMyAccountTabs>())
            {
                var instance = (IMyAccountTabs)Activator.CreateInstance(type, null);
                if (instance != null)
                    model.Tabs.AddRange(instance.GetMyAccountTabs());
            }

            return model;
        }
    }
}