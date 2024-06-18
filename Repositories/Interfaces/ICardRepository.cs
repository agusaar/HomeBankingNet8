using HomeBankingNet8.Models;

namespace HomeBankingNet8.Repositories.Interfaces
{
    public interface ICardRepository
    {
        IEnumerable<Card> GetAllCards();
        Card FindById(long id);
        void Save(Card card);
        IEnumerable<Card> FindByClientId(long ClientId);
        Card FindByCardNum(string CardNum);
    }
}
