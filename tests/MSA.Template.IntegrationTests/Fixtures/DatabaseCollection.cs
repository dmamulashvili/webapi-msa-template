using Microsoft.EntityFrameworkCore;

namespace MSA.Template.IntegrationTests.Fixtures;

[CollectionDefinition(nameof(DbContext))]
public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
{
}