﻿using Promethium.Plugin.Promotions.Extensions;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.Plugin.Customers;
using Sitecore.Framework.Rules;
using System;
using System.Linq;

namespace Promethium.Plugin.Promotions.Conditions
{
    /// <summary>
    /// A SiteCore Commerce condition for the qualification
    /// "Current Customer first purchase is [operator] [date]"
    /// </summary>
    [EntityIdentifier("Promethium_" + nameof(FirstPurchaseDateCondition))]
    public class FirstPurchaseDateCondition : ICustomerCondition
    {
        private readonly FindEntitiesInListCommand _findEntitiesInListCommand;

        public FirstPurchaseDateCondition(FindEntitiesInListCommand findEntitiesInListCommand)
        {
            _findEntitiesInListCommand = findEntitiesInListCommand;
        }

        //SiteCore only adds Datetime operators out-of-the-box
        public IBinaryOperator<DateTime, DateTime> Promethium_Operator { get; set; }

        //Out-of-the-box DatetimeOffset get's a nice editor and Datetime not
        public IRuleValue<DateTimeOffset> Promethium_Date { get; set; }

        public bool Evaluate(IRuleExecutionContext context)
        {
            var date = Promethium_Date.Yield(context).DateTime;

            if (!context.GetOrderHistory(_findEntitiesInListCommand, out var orders))
            {
                return false;
            }

            var firstPurchaseDate = orders.Min(x => x.OrderPlacedDate).DateTime;
            return Promethium_Operator.Evaluate(firstPurchaseDate, date);
        }
    }
}
