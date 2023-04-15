using MSA.Template.Core.OrderAggregate;
using MSA.Template.Infrastructure;
using MSA.Template.Infrastructure.Data;
using MSA.Template.IntegrationTests.Fixtures;
using MSA.Template.UnitTests.Builders;
using SharedKernel.Interfaces;

namespace MSA.Template.IntegrationTests.Repositories;

[Collection(nameof(DatabaseFixture))]
public class OrderRepositoryTests
{
    private OrderBuilder OrderBuilder { get; } = new();
    private readonly MasterDbContext _masterDbContext;
    private readonly IRepository<Order, Guid> _repository;

    public OrderRepositoryTests(DatabaseFixture fixture)
    {
        _masterDbContext = fixture.MasterDbContext;
        _repository = new BaseRepository<Order, Guid>(_masterDbContext);
    }

    [Fact]
    public async Task Add_NewOrder()
    {
        var newOrder = OrderBuilder.CreateDraftOrder();

        await _repository.AddAsync(newOrder);
        await _repository.UnitOfWork.SaveChangesAsync();

        var orderFromDb = await _masterDbContext.FindAsync<Order>(newOrder.Id);

        Assert.NotNull(orderFromDb);
        Assert.Equal(newOrder.OrderStatus, orderFromDb!.OrderStatus);
        Assert.Equal(newOrder.OrderDate, orderFromDb.OrderDate);
    }

    [Fact]
    public async Task Find_ExistingOrder_By_Id()
    {
        var existingOrder = OrderBuilder.CreateDraftOrder();
        _masterDbContext.Add(existingOrder);
        await _masterDbContext.SaveChangesAsync();

        var orderFromRepo = await _repository.FindByIdAsync(existingOrder.Id);

        Assert.NotNull(orderFromRepo);
        Assert.Equal(existingOrder.OrderStatus, orderFromRepo!.OrderStatus);
        Assert.Equal(existingOrder.OrderDate, orderFromRepo.OrderDate);
    }
}
