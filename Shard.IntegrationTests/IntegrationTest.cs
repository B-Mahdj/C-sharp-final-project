using Microsoft.AspNetCore.Mvc.Testing;
using Shard.API;
using Shard.Shared.Web.IntegrationTests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Shard.IntegrationTests;

public class IntegrationTest : BaseIntegrationTests<Program>
{
    public IntegrationTest(WebApplicationFactory<Program> factory, ITestOutputHelper testOutputHelper) : base(factory, testOutputHelper)
    {
    }
}

