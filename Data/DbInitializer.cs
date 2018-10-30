using MicroServicesCDU.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroServicesCDU.Data
{
    public class DbInitializer
    {
        public static void Initialize(MicroServicesCDUContext context)
        {
            context.Database.EnsureCreated();
        }
    }
}
