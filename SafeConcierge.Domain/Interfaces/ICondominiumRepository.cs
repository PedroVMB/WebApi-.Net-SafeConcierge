using SafeConcierge.Domain.Models;

namespace SafeConcierge.Domain.Interfaces;

public interface ICondominiumRepository : IRepositoryBase<Condominium>
{
    Condominium? GetByCnpj(string cnpj);
}

