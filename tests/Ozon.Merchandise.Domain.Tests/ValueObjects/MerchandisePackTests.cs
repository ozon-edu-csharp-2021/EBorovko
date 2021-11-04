using Ozon.MerchandiseService.Domain.Aggregates.MerchandiseProvidingRequest.ValueObjects;
using Ozon.MerchandiseService.Domain.Exceptions.MerchandiseProvidingRequest;
using Xunit;

namespace Ozon.Merchandise.Domain.Tests.ValueObjects
{
    public class MerchandisePackTests
    {
        [Theory]
        [InlineData(10)]
        [InlineData(20)]
        [InlineData(30)]
        [InlineData(40)]
        [InlineData(50)]
        public void Parse_ValidId_ReturnNewMerchandisePackWithSuchId(int id)
        {
            var merchandisePack = MerchandisePack.Parse(id);
            Assert.NotNull(merchandisePack);
            Assert.Equal(id, merchandisePack.Id );
        }
        
        [Theory]
        [InlineData(0)]
        [InlineData(60)]
        public void Parse_InValidId_ThrowUnknownMerchPackIdException(int id)
        {
            Assert.Throws<UnknownMerchPackId>(() => MerchandisePack.Parse(id));
        }
    }
}