using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using DataTables.DatabaseUtil;

namespace DataTables.DatabaseUtil.Oracle
{
    /// <summary>
    /// Oracle Query class - this should be considered to be beta.
    /// </summary>
    public class Query : DataTables.Query
    {
        /// <summary>
        /// Create a query instance specifically for Oracle
        /// </summary>
        /// <param name="db">Host database</param>
        /// <param name="type">Query type</param>
        public Query(Database db, string type)
            : base(db, type)
        {
        }

        override internal string _bindChar { get { return ":"; } }

        override internal string _fieldQuote { get { return "\""; } }

        /// <summary>
        /// Bind parameters to the SQL statement
        /// </summary>
        /// <param name="sql">SQL command</param>
        override protected void _Prepare(string sql)
        {
            var provider = DbProviderFactories.GetFactory(_db.Adapter());
            var cmd = provider.CreateCommand();
            DbParameter param;


            // Need to reliably get the primary key value
            if (_type == "insert")
            {
                var table = _table[0].Split(new string[] { " as " }, StringSplitOptions.None);
                var pkeyCmd = provider.CreateCommand();

                // Get the primary key column name
                pkeyCmd.CommandText =
                    @"SELECT cols.column_name as col_name
                        FROM all_constraints cons, all_cons_columns cols
                        WHERE cols.table_name = :t
                        AND cons.constraint_type = 'P'
                        AND cons.constraint_name = cols.constraint_name
                        AND cons.owner = cols.owner";
                pkeyCmd.Connection = _db.Conn();
                pkeyCmd.Transaction = _db.DbTransaction;

                param = pkeyCmd.CreateParameter();
                param.ParameterName = ":t";
                param.Value = table[0];
                pkeyCmd.Parameters.Add(param);

                using (var dr = pkeyCmd.ExecuteReader())
                {
                    // If the table doesn't have a primary key field, we can't get
                    // the inserted pkey!
                    if (dr.HasRows && dr.Read())
                    {
                        // Add a returning parameter statement into an output parameter
                        sql += " RETURNING " + dr["col_name"] + " INTO :dt_pkey";

                        var outParam = cmd.CreateParameter();
                        outParam.ParameterName = ":dt_pkey";
                        outParam.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(outParam);
                    }
                }
            }

            cmd.CommandText = sql;
            cmd.Connection = _db.Conn();
            cmd.Transaction = _db.DbTransaction;

            // Oracle.DataAccess.Client (and managed) bind by position by default(!)
            // So we need to force it to bind by name if used
            var bindByName = cmd.GetType().GetProperty("BindByName");
            if (bindByName != null)
            {
                bindByName.SetValue(cmd, true);
            }

            // Bind values
            for (int i = 0, ien = _bindings.Count; i < ien; i++)
            {
                var binding = _bindings[i];

                param = cmd.CreateParameter();
                param.ParameterName = binding.Name;
                param.Value = binding.Value ?? DBNull.Value;

                if (binding.Type != null)
                {
                    param.DbType = binding.Type;
                }

                cmd.Parameters.Add(param);
            }

            _stmt = cmd;
        }

        /// <summary>
        /// Execute the SQL command
        /// </summary>
        /// <returns>Result instance</returns>
        override protected DataTables.Result _Exec()
        {
            var dt = new System.Data.DataTable();

            using (var dr = _stmt.ExecuteReader())
            {
                dt.Load(dr);
            }

            return new Oracle.Result(_db, dt, this);
        }

        /// <summary>
        /// Oracle table statement
        /// </summary>
        /// <returns>SQL for the table</returns>
        override protected string _BuildTable()
        {
            var tablesOut = new List<string>();

            foreach (var t in _table)
            {
                if (t.Contains(" as "))
                {
                    var a = t.Split(new[] { " as " }, StringSplitOptions.None);
                    tablesOut.Add(a[0] + " " + a[1]);
                }
                else
                {
                    tablesOut.Add(t);
                }
            }

            return " " + String.Join(", ", tablesOut.ToArray()) + " ";
        }
    }
}
