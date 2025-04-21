using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoursWork
{
    public class MySqlTableAttribute: Attribute
    {
        public string TableName { get; set; }
        public MySqlTableAttribute(string tableName) 
        {
            TableName = tableName;
        }
    }
}
