using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace AdvantShop.Core.Common
{
    public static class DataModificationFlag
    {
        private static DateTime? _lastModified { get; set; }

        public static DateTime LastModified
        {
            get
            {
                if (!_lastModified.HasValue || (DateTime.Now - _lastModified.Value).TotalSeconds > 60)
                    _lastModified = DateTime.Now;
                return _lastModified.Value;
            }
        }

        public static void ResetLastModified()
        {
            _lastModified = DateTime.Now;
        }

        public static void SetLastModifiedSql(string commandText, CommandType commandType)
        {
            if (string.IsNullOrEmpty(commandText))
                return;
            var words = new List<string>
            {
                "insert",
                "update",
                "delete"
            };
            var spWords = new List<string>
            {
                "add",
                "change",
                "disable",
                "inccount",
                "recalculate",
                "remove",
                "precalc",
                "set",
                "decrement",
                "increment",
                "clear",
                "save"
            };
            var ignoreWords = new List<string>
            {
                "recentlyviewsdata"
            };
            commandText = commandText.ToLower();
            if ((words.Any(x => commandText.Contains(x)) && !ignoreWords.Any(x => commandText.Contains(x))) ||
                (commandType == CommandType.StoredProcedure && spWords.Any(x => commandText.Contains(x))))
            {
                _lastModified = DateTime.Now;
            }
        }
    }
}
