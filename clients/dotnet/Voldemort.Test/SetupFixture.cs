using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Voldemort.Test
{
    [SetUpFixture]
    public class SetupFixture
    {
        [SetUp]
        public void Logging()
        {
            Voldemort.Logger.ConfigureForUnitTesting();
        }
    }
}
