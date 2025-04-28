using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoursWork
{
    public class TestForeign
    {
        [MySqlColumn("ForeignKey")]
        public int ForeignKey { get; set; }
    }
}
