using System;
using System.Collections.Generic;
using System.Linq;
using Ozon.MerchandiseService.Domain.Aggregates.Employee;
using Ozon.MerchandiseService.Domain.Aggregates.MerchandiseProvidingRequest;
using Ozon.MerchandiseService.Domain.Aggregates.MerchandiseProvidingRequest.ValueObjects;
using Ozon.MerchandiseService.Domain.Exceptions.MerchandiseProvidingRequest;
using Ozon.MerchandiseService.Domain.ValueObjects;
using Xunit;
using Email = Ozon.MerchandiseService.Domain.Aggregates.Employee.ValueObjects.Email;

namespace Ozon.Merchandise.Domain.Tests.Aggregates
{
    public class MerchandiseProvidingRequestTests
    {
        private static MerchandiseProvidingRequest CreateParameterized(MerchandisePackType merchandisePack)
        {
            return new (
                new Employee(1, Email.Create("ivanov@ozon.com")),
                merchandisePack.Id,
                new DateTimeOffset(2021, 11, 3, 10, 10, 20, TimeSpan.Zero));
        }
        
        public static IEnumerable<object[]> DataForGetSkuIds
            => new List<object[]>
            {
                new object[]{ CreateParameterized(MerchandisePackType.WelcomePack), MerchandisePackType.WelcomePack.SkuList.Select(s => s.Value)},
                new object[]{ CreateParameterized(MerchandisePackType.StarterPack), MerchandisePackType.StarterPack.SkuList.Select(s => s.Value)},
                new object[]{ CreateParameterized(MerchandisePackType.ConferenceListenerPack), MerchandisePackType.ConferenceListenerPack.SkuList.Select(s => s.Value)},
                new object[]{ CreateParameterized(MerchandisePackType.ConferenceSpeakerPack), MerchandisePackType.ConferenceSpeakerPack.SkuList.Select(s => s.Value)},
                new object[]{ CreateParameterized(MerchandisePackType.VeteranPack), MerchandisePackType.VeteranPack.SkuList.Select(s => s.Value)}
            };
        
        public static IEnumerable<object[]> DataForCheckOneYearPass
            => new List<object[]>
            {
                new object[]{ new DateTimeOffset(2021, 11, 3, 0, 0, 0, TimeSpan.Zero), default(DateTimeOffset), false},
                // менее одного года
                new object[]{ new DateTimeOffset(2021, 11, 3, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2021, 11, 5, 0, 0, 0, TimeSpan.Zero), false},
                // ровно год
                new object[]{ new DateTimeOffset(2020, 11, 3, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2021, 11, 3, 0, 0, 0, TimeSpan.Zero), false},
                // более
                new object[]{ new DateTimeOffset(2020, 11, 2, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2021, 11, 3, 0, 0, 0, TimeSpan.Zero), true}
            };
        
        
        [Fact]
        public void InitDefaultMerchandiseProvidingRequest_CurrentStatusIsDraft()
        {
            var request = new MerchandiseProvidingRequest();
            
            Assert.Equal(Status.Draft, request.CurrentStatus);
        }
        
        [Fact]
        public void InitParameterizedMerchandiseProvidingRequest_CurrentStatusIsCreated()
        {
            var request = CreateParameterized(MerchandisePackType.StarterPack);
            
            Assert.Equal(Status.Created, request.CurrentStatus);
        }
        
        [Fact]
        public void ProcessRequestWithCreatedStatus_CurrentStatusIsInWork()
        {
            var request = CreateParameterized(MerchandisePackType.StarterPack);
            
            request.Wait();
            
            Assert.Equal(Status.InWork, request.CurrentStatus);
        }
        
        [Fact]
        public void ProcessRequestWithDraftStatus_ThrowStatusException()
        {
            var request = new MerchandiseProvidingRequest();
            
            Assert.Throws<StatusException>(()=> request.Wait());
        }
        
        [Fact]
        public void CompleteRequestWithCreatedStatus_CurrentStatusIsInDoneAndCompleteAtMustBeSet()
        {
            var request = CreateParameterized(MerchandisePackType.StarterPack);
            var completeAt = new DateTimeOffset(2021, 11, 5, 10, 10, 20, TimeSpan.Zero);
            
            request.Complete(completeAt);
            
            Assert.Equal(Status.Done, request.CurrentStatus);
            Assert.Equal(completeAt, request.CompletedAt!.Value);
        }
        
        [Fact]
        public void CompleteRequestWithInWorkStatus_CurrentStatusIsDoneAndCompletedDateTimeMustBeSet()
        {
            var request = CreateParameterized(MerchandisePackType.StarterPack);
            var dateTimeCompleted = new DateTimeOffset(2021, 11, 5, 10, 10, 20, TimeSpan.Zero);
            request.Wait();
            
            request.Complete(dateTimeCompleted);
            
            Assert.Equal(Status.Done, request.CurrentStatus);
            Assert.Equal(dateTimeCompleted, request.CompletedAt!.Value);
        }
        
        [Fact]
        public void CompleteRequest_PassInvalidCompletedAt_ThrowCompleteAtException()
        {
            var request = CreateParameterized(MerchandisePackType.StarterPack);
            var completeAt = new DateTimeOffset(2021, 11, 3, 10, 10, 20, TimeSpan.Zero);
            
            Assert.Throws<CompleteAtException>(()=> request.Complete(completeAt));
        }
        
        [Fact]
        public void CompleteRequestWithStatusDraft_ThrowStatusException()
        {
            var request = new MerchandiseProvidingRequest();
            
            Assert.Throws<StatusException>(()=> request.Wait());
        }
        
        [Theory]
        [MemberData(nameof(DataForGetSkuIds))]
        public void GetSkuIds_ReturnExpectedValue(MerchandiseProvidingRequest merchandiseProvidingRequest, IEnumerable<long> expected)
        {
            var skuIds = merchandiseProvidingRequest.SkuIds;
            
            Assert.True(expected.SequenceEqual(skuIds));
        }
        
        [Theory]
        [MemberData(nameof(DataForCheckOneYearPass))]
        public void CheckOneYearPass_ReturnExpected(DateTimeOffset completedAt, DateTimeOffset toCompare, bool expected)
        {
            var request = new MerchandiseProvidingRequest();
            request.GetType().GetProperty("CompletedAt")?.SetValue(request, completedAt);
            
            var result = request.CheckOneYearPassFromProviding(toCompare);
            
            Assert.Equal(expected, result);
        }
    }
}