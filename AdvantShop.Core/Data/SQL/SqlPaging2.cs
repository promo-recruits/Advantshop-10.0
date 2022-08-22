using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.SQL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace AdvantShop.Core.SQL2
{
    public enum SqlSort
    {
        None,
        Asc,
        Desc
    }

    public class SqlParam
    {
        public string ParamName { get; set; }
        public object ParamValue { get; set; }
    }

    public class SqlCritera
    {
        private string _prefix;
        private string _name;
        private string _nameAs;
        private SqlSort _sort;

        public SqlCritera(string name, string nameas, SqlSort sort)
        {
            var items = name.Split('.');

            //if (string.IsNullOrWhiteSpace(nameas) && items.Length > 2)
            //    throw new Exception("Wrong fromat sql field: " + name);

            _name = items.Length == 2 ? items[1] : name;
            _prefix = items.Length == 2 ? items[0] : "";
            _nameAs = nameas;
            _sort = sort;
        }

        public string WhereSql()
        {
            return _name;
        }

        public string SelectSql()
        {
            var res = "";
            if (!string.IsNullOrWhiteSpace(_prefix))
                res = _prefix + "." + _name;
            else
                res += _name;
            if (!string.IsNullOrWhiteSpace(_nameAs))
                res += " as " + _nameAs;
            return res;
        }

        public string OrderSql()
        {
            return (string.IsNullOrWhiteSpace(_nameAs) ? _name : _nameAs) + " " + (_sort == SqlSort.None ? SqlSort.Asc : _sort);
        }

        public static implicit operator SqlCritera(string nameField)
        {
            return new SqlCritera(nameField, "", SqlSort.None);
        }

        public void SetSort(SqlSort sort)
        {
            _sort = sort;
        }

        public string FieldName => string.IsNullOrWhiteSpace(_nameAs) ? _name.ToLower() : _nameAs.ToLower();
    }

    public class SqlWhereCondition
    {
        private string _condition;

        public SqlWhereCondition(string condition)
        {
            _condition = condition;
        }

        public bool IgnoreInCustomData { get; set; }

        public SqlWhereCondition(string condition, bool ignoreInCustomData) : this(condition)
        {
            IgnoreInCustomData = ignoreInCustomData;
        }

        public string Sql()
        {
            return _condition;
        }

        public bool Valid()
        {
            return !string.IsNullOrEmpty(_condition);
        }

        public static implicit operator SqlWhereCondition(string condition)
        {
            return new SqlWhereCondition(condition);
        }
    }

    public static class SqlWhereConditions
    {
        public static SqlWhereCondition IgnoreInCustomData(this string val)
        {
            return new SqlWhereCondition(val, true);
        }
    }

    public class SqlWhere : List<SqlWhereCondition>
    {
        public SqlWhere() : base() { }

        public void Add(SqlWhereCondition condition, params string[] args)
        {
            if (condition == null || !condition.Valid())
                return;

            var sql = condition.Sql();
            if (this.Count != 0 && !sql.ToLower().StartsWith("and "))
                sql = "AND " + sql;

            base.Add(new SqlWhereCondition(string.Format(sql, args), condition.IgnoreInCustomData));
        }

        public string Sql()
        {
            return this.Count > 0 
                ? " WHERE " + this.Select(c => c.Sql()).AggregateString(" ") 
                : "";
        }

        public string SqlCustomData()
        {
            return this.Count(c => !c.IgnoreInCustomData) > 0 
                ? " WHERE " + this.Where(c => !c.IgnoreInCustomData).Select(c => c.Sql()).AggregateString(" ")
                : "";
        }
    }

    public static class SqlFields
    {
        public static SqlCritera AsSqlField(this string val, string nameAs)
        {
            var model = new SqlCritera(val, nameAs, SqlSort.None);
            return model;
        }
    }

    public class SqlPaging
    {
        private readonly string _cacheNameKey;
        private readonly bool _useCache;

        private const string Advparam = "@p";
        private readonly SqlWhere _whereCondition;
        private readonly List<SqlParam> _listParams;
        private readonly List<SqlCritera> _selectFields;
        private readonly List<SqlCritera> _orderFields;
        private string _tablename;
        private readonly List<string> _joinTable;

        private readonly int? _limit;
        public int ItemsPerPage { get; set; }
        public int CurrentPageIndex { get; set; }


        public SqlPaging()
            : this(1, 10, "", false, null)
        {
        }

        public SqlPaging(string cacheNameKey)
            : this(1, 10, cacheNameKey, true, null)
        {
        }

        public SqlPaging(string cacheNameKey, int? limit)
        : this(1, 10, cacheNameKey, true, limit)
        {
        }


        public SqlPaging(int currentPageIndex, int itemsPerPage)
            : this(currentPageIndex, itemsPerPage, "", false, null)
        {
        }

        public SqlPaging(int currentPageIndex, int itemsPerPage, string cacheNameKey, bool useCache, int? limit)
        {
            CurrentPageIndex = currentPageIndex;
            ItemsPerPage = itemsPerPage;
            _whereCondition = new SqlWhere();
            _listParams = new List<SqlParam>();
            _selectFields = new List<SqlCritera>();
            _orderFields = new List<SqlCritera>();
            _tablename = string.Empty;
            _joinTable = new List<string>();
            _cacheNameKey = cacheNameKey;
            _useCache = useCache;
            _limit = limit ?? 0;
        }

        public int PageCount(int rowsCount, int itemsPerPage)
        {
            return (int)(Math.Ceiling((double)rowsCount / itemsPerPage));
        }

        public int PageCount(int rowsCount)
        {
            return (int)(Math.Ceiling((double)rowsCount / ItemsPerPage));
        }

        public int PageCount()
        {
            return (int)(Math.Ceiling((double)TotalRowsCount / ItemsPerPage));
        }

        private int? _totalRowsCount = null;
        public int TotalRowsCount
        {
            get
            {
                if (_selectFields.Count == 0) throw new Exception("set any select fields");

                if (_totalRowsCount.HasValue)
                    return _totalRowsCount.Value;

                var query = "SELECT COUNT( " + /*_selectFields.First().SelectSql()*/ "*" + ") FROM "
                            + _tablename
                            + _joinTable.Aggregate(" ", (a, b) => a + " " + b)
                            + _whereCondition.Sql();

                var cacheName = _useCache ? CacheNames.SQlPagingCountCacheName(_cacheNameKey, query, _listParams.Select(p => p.ParamName + p.ParamValue).AggregateString("")) : null;

                if (_useCache && CacheManager.TryGetValue(cacheName, out _totalRowsCount))
                    return _totalRowsCount.Value;

                _totalRowsCount = SQLDataAccess.ExecuteScalar<int>(query, CommandType.Text, _listParams.Select(x => new SqlParameter(x.ParamName, x.ParamValue)).ToArray());

                if (_limit > 0 && _totalRowsCount > _limit)
                {
                    _totalRowsCount = _limit;
                }
                if (_useCache)
                    CacheManager.Insert(cacheName, _totalRowsCount);

                return _totalRowsCount.Value;
            }
        }

        public bool LimitReached => _limit != 0 && _limit == _totalRowsCount;

        public DataTable PageItems
        {
            get
            {
                var needRow = CurrentPageIndex * ItemsPerPage;
                var keyid = (CurrentPageIndex - 1) * ItemsPerPage;

                var selecStr = _selectFields.Select(x => x.SelectSql())
                             .Union(_orderFields.Where(o => _selectFields.All(s => s.FieldName != o.FieldName)).Select(x => x.SelectSql()))
                             .AggregateString(", ");

                var order =
                    _orderFields.Count > 0
                        ? _orderFields.Select(x => x.OrderSql()).AggregateString(", ")
                        : _selectFields[0].OrderSql();


                var query = string.Format("WITH TEMP " +
                             "AS ( " +
                                 "SELECT {7} {0} " +
                                 "FROM {1} {2} {3}" +
                                 ")" +
                              "SELECT * " +
                              "FROM ( " +
                                    "SELECT TOP ({4}) Row_Number() OVER ( " +
                                            "ORDER BY {5} " +
                                            ") AS RowNum " +
                                        ",* " +
                                    "FROM TEMP " +
                                    ") AS t " +
                            "WHERE RowNum > {6} ",
                             selecStr,
                            _tablename,
                            _joinTable.AggregateString(" "),
                            _whereCondition.Sql(),
                            needRow,
                            order,
                            keyid,
                            _limit != 0 ? "Top " + _limit : ""
                            );



                var tbl = SQLDataAccess.ExecuteTable(query, CommandType.Text,
                    _listParams.Select(x => new SqlParameter(x.ParamName, x.ParamValue)).ToArray());
                return tbl;

            }
        }

        public List<T> PageItemsList<T>()
        {
            return PageItemsList<T>(null);
        }

        public List<T> PageItemsList<T>(Func<SqlDataReader, T> function)
        {
            var needRow = CurrentPageIndex * ItemsPerPage;
            var keyid = (CurrentPageIndex - 1) * ItemsPerPage;

            var selecStr = _selectFields.Select(x => x.SelectSql())
                                        .Union(_orderFields.Where(o => _selectFields.All(s => s.FieldName != o.FieldName)).Select(x => x.SelectSql()))
                                        .AggregateString(", ");

            var order =
                _orderFields.Count > 0
                    ? _orderFields.Select(x => x.OrderSql()).AggregateString(", ") // .Union(_selectFields.Take(1).Select(x => x.OrderSql())).AggregateString(", ")
                    : _selectFields[0].OrderSql();

            var query = string.Format("WITH TEMP " +
                            "AS ( " +
                                "SELECT {7} {0} " +
                                "FROM {1} {2} {3}" +
                                ")" +
                            "SELECT * " +
                            "FROM ( " +
                                "SELECT Row_Number() OVER ( " +
                                        "ORDER BY {5} " +
                                        ") AS RowNum " +
                                    ",* " +
                                "FROM TEMP " +
                                ") AS t " +
                        "WHERE RowNum > {6} AND RowNum <= {4}",
                            selecStr,
                        _tablename,
                        _joinTable.AggregateString(" "),
                        _whereCondition.Sql(),
                        needRow,
                        order,
                        keyid,
                        _limit != 0 ? "Top " + _limit : ""
                        );

            var cacheName = _useCache ? CacheNames.SQlPagingItemsCacheName(_cacheNameKey, query, _listParams.Select(p => p.ParamName + p.ParamValue).AggregateString("")) : null;

            List<T> items;

            if (_useCache && CacheManager.TryGetValue(cacheName, out items))
                return items;

            items =
                function == null
                    ? SQLDataAccess.Query<T>(query,
                        _listParams.Select(x => new KeyValuePair<string, object>(x.ParamName, x.ParamValue)).ToArray())
                        .ToList()
                    : SQLDataAccess.ExecuteReadList<T>(query, CommandType.Text, function,
                        _listParams.Select(x => new SqlParameter(x.ParamName, x.ParamValue)).ToArray());

            if (_useCache)
                CacheManager.Insert(cacheName, items);

            return items;
        }

        public List<T> GetCustomData<T>(string selectFields, string newCondition, Func<IDataReader, T> getFromReader, bool useDistinct, string jointable = "")
        {
            var query = String.Format("SELECT {5}{0} FROM {1} {2} {3} {4}",
                selectFields, _tablename,
                _joinTable.AggregateString(" ") + " " + jointable,
                _whereCondition.SqlCustomData(),
                newCondition,
                useDistinct ? "Distinct " : ""
                );

            var table =
                SQLDataAccess.ExecuteReadList(query, CommandType.Text, getFromReader,
                    _listParams.Select(x => new SqlParameter(x.ParamName, x.ParamValue)).ToArray());
            return table;
        }

        public List<T> ItemsIds<T>(string fieldName)
        {
            var query =
                string.Format("SELECT {0} FROM {1} {2} {3}",
                    fieldName,
                    _tablename,
                    _joinTable.Aggregate(" ", (a, b) => a + " " + b),
                    _whereCondition.Sql());

            return SQLDataAccess.Query<T>(query,
                        _listParams.Select(x => new KeyValuePair<string, object>(x.ParamName, x.ParamValue)).ToArray(), CommandType.Text)
                        .ToList();
        }

        private string[] GetParamString(params object[] args)
        {
            if (args == null) return new string[0];

            var returnArr = new string[args.Length];
            var i = 0;

            foreach (var arg in args)
            {
                var argArr = arg as Array;
                if (argArr != null)
                    foreach (var argItem in argArr)
                    {
                        var temp = Advparam + _listParams.Count;
                        _listParams.Add(new SqlParam { ParamName = temp, ParamValue = argItem });
                        //if (!string.IsNullOrEmpty(returnStr))
                        //returnStr += ",";
                        //returnStr += temp;
                        if (!string.IsNullOrEmpty(returnArr[i]))
                            returnArr[i] += ",";
                        returnArr[i] += temp;
                    }
                else
                {
                    var temp = Advparam + _listParams.Count;
                    _listParams.Add(new SqlParam { ParamName = temp, ParamValue = arg });
                    //if (!string.IsNullOrEmpty(returnStr))
                    //    returnStr += ",";
                    //returnStr += temp;
                    //returnArr.Add(temp);
                    returnArr[i] = temp;
                }
                i++;
            }
            return returnArr.ToArray();
        }

        public void Select(params SqlCritera[] field)
        {
            _selectFields.AddRange(field);
        }

        // new version
        public void Where(SqlWhereCondition condition, params object[] args)
        {
            var temp = GetParamString(args);

            _whereCondition.Add(condition, temp);
        }

        // old version. don't break modules
        public void Where(string condition, params object[] args)
        {
            var temp = GetParamString(args);

            _whereCondition.Add(condition, temp);
        }

        private void OrderBy(SqlSort sort, params SqlCritera[] condition)
        {
            foreach (var item in condition)
            {
                item.SetSort(sort);
                _orderFields.Add(item);
            }
        }

        public void OrderBy(params SqlCritera[] condition)
        {
            OrderBy(SqlSort.Asc, condition);
        }

        public void OrderByDesc(params SqlCritera[] condition)
        {
            OrderBy(SqlSort.Desc, condition);
        }

        private void Join(string joinTable, params object[] args)
        {
            var temp = GetParamString(args);
            _joinTable.Add(string.Format(joinTable, temp));
        }

        public void Inner_Join(string joinTable, params object[] args)
        {
            Join("Inner Join " + joinTable, args);
        }

        public void Left_Join(string joinTable, params object[] args)
        {
            Join("Left Join " + joinTable, args);
        }

        public void From(string tablename)
        {
            _tablename = tablename;
        }

        public List<SqlCritera> SelectFields()
        {
            return _selectFields;
        }
    }
}