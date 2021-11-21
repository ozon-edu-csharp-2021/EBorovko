using System;
using System.Collections.Generic;
using System.Linq;
using Ozon.MerchandiseService.Domain.Aggregates.Employee;
using Ozon.MerchandiseService.Domain.Aggregates.MerchandiseProvidingRequest.ValueObjects;
using Ozon.MerchandiseService.Domain.Events;
using Ozon.MerchandiseService.Domain.Exceptions.MerchandiseProvidingRequest;
using Ozon.MerchandiseService.Domain.Models;
using Ozon.MerchandiseService.Domain.ValueObjects;


namespace Ozon.MerchandiseService.Domain.Aggregates.MerchandiseProvidingRequest
{
    public class MerchandiseProvidingRequest: Entity
    {
        private readonly Employee.Employee _employee;

        /// <summary>
        /// Идентификатор сотрудника, которому предназначен мерч
        /// </summary>
        public long EmployeeId => _employee.Id;

        /// <summary>
        /// Email сотрудника, которому предназначен мерч
        /// </summary>
        public string EmployeeEmail => _employee.Email;

        /// <summary>
        /// Тип набора мерча
        /// </summary>
        public MerchandisePackType MerchandisePackType { get;}
        
        /// <summary>
        /// Текущий статус 
        /// </summary>
        public Status CurrentStatus { get; private set; }

        /// <summary>
        /// Время создания 
        /// </summary>
        public DateTimeOffset CreatedAt { get; }
        
        /// <summary>
        /// Время завершения 
        /// </summary>
        public DateTimeOffset? CompletedAt { get; private set; }

        /// <summary>
        ///  Запрос выполнен
        /// </summary>
        public bool IsDone => CurrentStatus.Equals(Status.Done);
        
        /// <summary>
        ///  Запрос в работе
        /// </summary>
        public bool InWork => CurrentStatus.Equals(Status.InWork);
        
        /// <summary>
        /// Перечень идентификаторов SKU
        /// </summary>
        /// <returns></returns>
        public IEnumerable<long> SkuIds => MerchandisePackType.SkuList.Select(item => item.Value);
        
        public MerchandiseProvidingRequest()
        {
            CurrentStatus = Status.Draft;
        }

        public MerchandiseProvidingRequest(long employeeId, string employeeEmail, int merchPackId, DateTimeOffset createAt)
            : this()
        {
            _employee = new Employee.Employee(employeeId, employeeEmail);
                
            MerchandisePackType = MerchandisePackType.Parse(merchPackId);
            CreatedAt = createAt;
            CurrentStatus = Status.Created;
        }
        
        /// <summary>
        /// Ожидать поставки
        /// </summary>
        /// <exception cref="StatusException"></exception>
        public void Wait()
        {
            if (!CurrentStatus.Equals(Status.Created))
                throw new StatusException(CurrentStatus.Name, $"{Status.Created.Name}");
            
            CurrentStatus = Status.InWork;
        }

        /// <summary>
        /// Завершить запрос 
        /// </summary>
        /// <param name="completeAt"></param>
        /// <exception cref="StatusException"></exception>
        /// <exception cref="CompleteAtException"></exception>
        public void Complete(DateTimeOffset completeAt)
        {
            if (!CurrentStatus.Equals(Status.Created)  && !CurrentStatus.Equals( Status.InWork) )
                throw new StatusException(CurrentStatus.Name, $"{Status.Created.Name} or {Status.InWork.Name}");

            if (completeAt <= CreatedAt)
                throw new CompleteAtException(CreatedAt.ToString(), completeAt.ToString());
            
            _employee.Give(new MerchandisePack(MerchandisePackType));
            
            CompletedAt = completeAt;
            CurrentStatus = Status.Done;
            
            this.AddDomainEvent(new RequestStatusChangedOnDoneDomainEvent(Id));
        }

        /// <summary>
        /// Проверить, что прошел один год в момента выдачи
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public bool CheckOneYearPassFromProviding(DateTimeOffset dateTime)
        {
            if (!CompletedAt.HasValue)
                return false;

            if (CompletedAt >= dateTime)
                return false;
            
            return dateTime.AddYears(-1) > CompletedAt; 
        }
    }
}