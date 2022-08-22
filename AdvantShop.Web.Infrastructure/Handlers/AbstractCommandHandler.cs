﻿using AdvantShop.Core.Services.Localization;

namespace AdvantShop.Web.Infrastructure.Handlers
{
    public abstract class AbstractCommandHandler<TResult> : ICommandHandler<TResult>
    {
        protected virtual void Load()
        {
        }

        protected virtual void Validate()
        {
        }

        protected virtual TResult Handle()
        {
            throw new System.NotImplementedException();
        }

        protected string T(string t)
        {
            return LocalizationService.GetResource(t);
        }

        protected string T(string t, params object[] parameters)
        {
            return LocalizationService.GetResourceFormat(t, parameters);
        }

        public TResult Execute()
        {
            Load();
            Validate();
            return Handle();
        }
    }


    public abstract class AbstractCommandHandler : ICommandHandler
    {
        protected virtual void Load()
        {
        }

        protected virtual void Validate()
        {
        }

        protected virtual void Handle()
        {
            throw new System.NotImplementedException();
        }

        protected string T(string t)
        {
            return LocalizationService.GetResource(t);
        }

        protected string T(string t, params object[] parameters)
        {
            return LocalizationService.GetResourceFormat(t, parameters);
        }

        public void Execute()
        {
            Load();
            Validate();
            Handle();
        }
    }
}