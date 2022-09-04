using MSA.Template.Core.OrderAggregate;
using MSA.Template.Core.OrderAggregate.Specifications;
using MSA.Template.Infrastructure;
using MSA.Template.Infrastructure.Data;
using MSA.Template.IntegrationTests.Fixtures;
using MSA.Template.UnitTests.Builders;
using SharedKernel.Interfaces;

namespace MSA.Template.IntegrationTests.Repositories;

[Collection(nameof(DatabaseFixture))]
public class OrderReadOnlyRepositoryTests
{
    private OrderBuilder OrderBuilder { get; } = new();
    private readonly MasterDbContext _masterDbContext;
    private readonly IReadOnlyRepository<Order, Guid> _readOnlyRepository;

    public OrderReadOnlyRepositoryTests(DatabaseFixture fixture)
    {
        _masterDbContext = fixture.MasterDbContext;
        _readOnlyRepository = new BaseReadOnlyRepository<Order, Guid>(fixture.SlaveDbContext);
    }

    [Fact]
    public async Task Get_Recent_Placed_Orders_Using_Specification()
    {
        var firstPlacedOrder = OrderBuilder.CreatePlacedOrder();
        var secondPlacedOrder = OrderBuilder.CreatePlacedOrder();

        await _masterDbContext.AddRangeAsync(firstPlacedOrder, secondPlacedOrder);
        await _masterDbContext.SaveChangesAsync();

        var spec = new RecentOrdersByPlacedStatusSpecification();
        var orders = await _readOnlyRepository.FindByAsync(spec);
        
        Assert.NotEmpty(orders);
        Assert.Equal(secondPlacedOrder.Id, orders.First().Id);
    }
}