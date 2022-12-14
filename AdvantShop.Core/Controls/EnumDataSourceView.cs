//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using AdvantShop.Core.Common.Extensions;

namespace AdvantShop.Core.Controls
{
    public class EnumDataSourceView : DataSourceView
    {
        private readonly Type _enumType;
        private readonly List<int> _exceptValues;

        public EnumDataSourceView(EnumDataSource owner, string viewName)
            : base(owner, viewName)
        {
            _enumType = owner.EnumType;
            _exceptValues = owner._ExceptValues;
        }

        protected override IEnumerable ExecuteSelect(DataSourceSelectArguments arguments)
        {
            return GetEnumItems(_enumType, _exceptValues);
        }

        private static IList GetEnumItems(Type type, List<int> exceptValues)
        {
            if (type.BaseType != typeof (Enum))
                throw new ArgumentException("Type must be an enumeration.", "type");

            var vals = Enum.GetValues(type).Cast<object>();

            if (exceptValues != null && exceptValues.Count > 0)
                vals = vals.Where(val => !exceptValues.Contains((int)val));

            return vals.Select(x =>
            {
                var resource = ((Enum) x).Localize();

                return new EnumItem()
                {
                    Name = Enum.GetName(type, x),
                    LocalizedName = string.IsNullOrEmpty(resource) ? x.ToString() : resource,
                    Value = (int) x
                };
            }).ToList();

        }
    }


    public class EnumItem
    {
        public string Name { get; set; }
        public string LocalizedName { get; set; }
        public int Value { get; set; }
    }
}