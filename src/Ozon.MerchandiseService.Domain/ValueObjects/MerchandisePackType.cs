using Ozon.MerchandiseService.Domain.Aggregates.MerchandiseProvidingRequest.ValueObjects;
using Ozon.MerchandiseService.Domain.Exceptions.MerchandiseProvidingRequest;
using Ozon.MerchandiseService.Domain.Models;

namespace Ozon.MerchandiseService.Domain.ValueObjects
{
    public class MerchandisePackType: Enumeration
    {
        public static readonly MerchandisePackType WelcomePack = new(10, nameof(WelcomePack), new SkuList(new [] {new Sku(1, "Брелок"), new Sku(2, "Ручка")}) );
        public static readonly MerchandisePackType StarterPack = new(20, nameof(StarterPack), new SkuList(new [] {new Sku(3, "Кружка"), new Sku(4, "Кепка")}));
        public static readonly MerchandisePackType ConferenceListenerPack = new(30, nameof(ConferenceListenerPack), new SkuList(new [] {new Sku(5, "Футболка"), new Sku(2, "Ручка")}));
        public static readonly MerchandisePackType ConferenceSpeakerPack = new(40, nameof(ConferenceSpeakerPack), new SkuList(new [] {new Sku(5, "Футболка"), new Sku(4, "Кепка")}));
        public static readonly MerchandisePackType VeteranPack = new(50, nameof(VeteranPack), new SkuList(new [] {new Sku(5, "Футболка"), new Sku(6, "Толстовка")}));
        
        public static MerchandisePackType Parse(int merchPackId)
        {
            return merchPackId switch
            {
                10 => WelcomePack,
                20 => StarterPack,
                30 => ConferenceListenerPack,
                40 => ConferenceSpeakerPack,
                50 => VeteranPack,
                _ => throw new UnknownMerchPackId(merchPackId)
            };
        }
        
        public SkuList SkuList { get; }
        private MerchandisePackType(int id, string name, SkuList skuList) : base(id, name)
        {
            SkuList = skuList;
        }
    }
}