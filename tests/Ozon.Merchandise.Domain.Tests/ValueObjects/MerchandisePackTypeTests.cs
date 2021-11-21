using Ozon.MerchandiseService.Domain.Exceptions.MerchandiseProvidingRequest;
using Ozon.MerchandiseService.Domain.ValueObjects;
using Xunit;

namespace Ozon.Merchandise.Domain.Tests.ValueObjects
{
    public class MerchandisePackTypeTests
    {
        [Theory]
        [InlineData(10)]
        [InlineData(20)]
        [InlineData(30)]
        [InlineData(40)]
        [InlineData(50)]
        public void Parse_ValidId_ReturnNewMerchandisePackWithSuchId(int id)
        {
            var merchandisePack = MerchandisePackType.Parse(id);
            Assert.NotNull(merchandisePack);
            Assert.Equal(id, merchandisePack.Id );
        }
        
        [Theory]
        [InlineData(0)]
        [InlineData(60)]
        public void Parse_InValidId_ThrowUnknownMerchPackIdException(int id)
        {
            Assert.Throws<UnknownMerchPackId>(() => MerchandisePackType.Parse(id));
        }
    }
}