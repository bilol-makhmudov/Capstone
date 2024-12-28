using Capstone.Models;

namespace Capstone.Repositories.Interfaces;

public interface IAnswerRepository : IRepository<Answer>
{
    Task<Guid> CreateAnswer(Answer answer);
}