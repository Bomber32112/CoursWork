using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoursWork
{
    [MySqlTable("TestTable")]
    public class TestClass: BaseDataClass
    {
        [MySqlColumn("TestString")]
        public string TestString { get; set; }
        [MySqlColumn("TestDateTime")]
        public DateTime TestDateTime { get; set; }
    }
}
