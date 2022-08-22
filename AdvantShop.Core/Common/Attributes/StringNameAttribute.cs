using System;

namespace AdvantShop.Core.Common.Attributes
{
    public class ShippingKeyAttribute : Attribute, IAttribute<string>
    {
        private readonly string _name;
        public ShippingKeyAttribute(string name)
        {
            _name = name;
        }

        public string Value
        {
            get { return _name; }
        }
    }
    
    public class ShippingAdminModelAttribute : Attribute, IAttribute<string>
    {
        private readonly string _key;
        public ShippingAdminModelAttribute(string key)
        {
            _key = key;
        }

        public string Value
        {
            get { return _key; }
        }
    }
    
    public class PaymentKeyAttribute : Attribute, IAttribute<string>
    {
        private readonly string _name;
        public PaymentKeyAttribute(string name)
        {
            _name = name;
        }

        public string Value
        {
            get { return _name; }
        }
    }
    
    public class PaymentAdminModelAttribute : Attribute, IAttribute<string>
    {
        private readonly string _key;
        public PaymentAdminModelAttribute(string key)
        {
            _key = key;
        }

        public string Value
        {
            get { return _key; }
        }
    }
    
    public class PaymentOrderPayModelAttribute : Attribute, IAttribute<string>
    {
        private readonly string _key;
        public PaymentOrderPayModelAttribute(string key)
        {
            _key = key;
        }

        public string Value
        {
            get { return _key; }
        }
    }

    public class StringNameAttribute : Attribute, IAttribute<string>
    {
        private readonly string _name;
        public StringNameAttribute(string name)
        {
            _name = name;
        }

        public string Value
        {
            get { return _name; }
        }
    }

    public class ActiveAttribute : Attribute, IAttribute<bool>
    {
        private readonly bool _active;
        public ActiveAttribute(bool active)
        {
            _active = active;
        }

        public bool Value
        {
            get { return _active; }
        }
    }

    public class ExportFeedKeyAttribute : Attribute, IAttribute<string>
    {
        private readonly string _name;
        public ExportFeedKeyAttribute(string name)
        {
            _name = name;
        }

        public string Value
        {
            get { return _name; }
        }
    }
}