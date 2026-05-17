using SafeConcierge.Domain.Models;

namespace SafeConcierge.Domain.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
}

