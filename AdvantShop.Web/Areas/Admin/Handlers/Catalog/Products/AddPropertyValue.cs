﻿using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Products
{
    public class AddPropertyValue
    {
        private readonly int _productId;
        private readonly int _propertyId;
        private readonly int? _propertyValueId;
        private readonly string _value;
        private readonly bool _isNew;

        public AddPropertyValue(int productId, int propertyId, int? propertyValueId, string value, bool isNew)
        {
            _productId = productId;
            _propertyId = propertyId;
            _propertyValueId = propertyValueId;
            _value = value != null ? value.Trim() : "";
            _isNew = isNew;
        }

        public int Execute()
        {
            var property = PropertyService.GetPropertyById(_propertyId);
            if (property == null)
                return 0;
            
            var propertyValueId = GetPropertyValueId(property);
            if (propertyValueId == 0)
                return 0;
            
            if (!PropertyService.IsExistPropertyValueInProduct(_productId, propertyValueId))
            {
                PropertyService.AddProductProperyValue(propertyValueId, _productId);
            }

            return propertyValueId;
        }

        private int GetPropertyValueId(Property property)
        {
            if (_propertyValueId != null)
            {
                var value = PropertyService.GetPropertyValueById(_propertyValueId.Value);
                if (value != null)
                    return value.PropertyValueId;
            }

            if (_isNew && !string.IsNullOrWhiteSpace(_value))
            {
                var propertyValue = PropertyService.GetPropertyValueByName(property.PropertyId, _value);
                if (propertyValue == null)
                {
                    propertyValue = new PropertyValue()
                    {
                        PropertyId = _propertyId,
                        Value = _value
                    };
                    return PropertyService.AddPropertyValue(propertyValue);
                }
            }
            return 0;
        }
    }
}