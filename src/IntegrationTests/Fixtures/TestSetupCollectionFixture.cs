﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTests.Fixtures;

[CollectionDefinition("TestSetupCollection")]
public class TestSetupCollectionFixture : ICollectionFixture<TestSetupFixture>
{
}
