using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoursWork
{
    public class MySqlColumnAttribute: Attribute
    {
        public string ColumnName { get; set; }
        public MySqlColumnAttribute(string columnName) 
        {
            ColumnName = columnName;
        }
    }
}
