using Demo_API.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo_API.UnitTests.Stubs
{
    public class FakeDateTimeProvider : IDateTimeProvider
    {
        public DateTime Now { get; set; }
    }

}
