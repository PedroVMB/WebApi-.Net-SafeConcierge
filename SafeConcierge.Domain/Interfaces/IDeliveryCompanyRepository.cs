using SafeConcierge.Domain.Models;

namespace SafeConcierge.Domain.Interfaces;

public interface IDeliveryCompanyRepository : IRepositoryBase<DeliveryCompany>
{
    DeliveryCompany? GetByName(string name);
}

