using Ozon.MerchandiseService.Domain.Aggregates.MerchandiseProvidingRequest.ValueObjects;
using Ozon.MerchandiseService.Domain.Exceptions.MerchandiseProvidingRequest;
using Ozon.MerchandiseService.Domain.Models;

namespace Ozon.MerchandiseService.Domain.ValueObjects
{
    public class MerchandisePackType: Enumeration
    {
        public static readonly MerchandisePackType WelcomePack = new(
            10, 
            nameof(WelcomePack), 
            new SkuList(new []
            {
                new Sku(1, "TShirtStarter "), 
                new Sku(2, "NotepadStarter"), 
                new Sku(3, "PenStarter"), 
                new Sku(4, "SocksStarter")
            }));
        public static readonly MerchandisePackType ConferenceListenerPack  = new(
            20, 
            nameof(ConferenceListenerPack), 
            new SkuList(new []
            {
                new Sku(10, "TShirtСonferenceListener"), 
                new Sku(11, "NotepadСonferenceListener"), 
                new Sku(12, "PenСonferenceListener")
            }));
        public static readonly MerchandisePackType ConferenceSpeakerPack  = new(
            30,
            nameof(ConferenceSpeakerPack), 
            new SkuList(new []
            {
                new Sku(7, "SweatshirtСonferenceSpeaker"), 
                new Sku(8, "NotepadСonferenceSpeaker"), 
                new Sku(9, "PenСonferenceSpeaker")
            }));
        public static readonly MerchandisePackType ProbationPeriodEndingPack  = new(
            40, 
            nameof(ProbationPeriodEndingPack), 
            new SkuList(new []
            {
                new Sku(6, "SweatshirtAfterProbation "), 
                new Sku(5, "TShirtAfterProbation ")
            }));
        public static readonly MerchandisePackType VeteranPack = new(
            50, 
            nameof(VeteranPack),
            new SkuList(new []
            {
                new Sku(13, "TShirtVeteran"), 
                new Sku(14, "SweatshirtVeteran"), 
                new Sku(15, "NotepadVeteran"), 
                new Sku(16, "PenVeteran"), 
                new Sku(17, "CardHolderVeteran")
            }));

        
        public static MerchandisePackType Parse(int merchPackId)
        {
            return merchPackId switch
            {
                10 => WelcomePack,
                20 => ConferenceListenerPack ,
                30 => ConferenceSpeakerPack,
                40 => ProbationPeriodEndingPack ,
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