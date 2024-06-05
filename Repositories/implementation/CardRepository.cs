using HomeBankingNet8.Models;
using HomeBankingNet8.Repositories.Interfaces;

namespace HomeBankingNet8.Repositories.implementation
{
    public class CardRepository: RepositoryBase<Card>,ICardRepository
    {
        public CardRepository(HomeBankingContext repositoryContext) : base(repositoryContext) { }

        public Card FindByCardNum(string CardNum)
        {
            return FindByCondition(card => string.Equals(card.Number, CardNum))
                .FirstOrDefault();
        }

        public IEnumerable<Card> FindByClientId(long ClientId)
        {
            return FindByCondition(card => card.ClientId == ClientId)
                .ToList();
        }

        public Card FindById(long id)
        {
            return FindByCondition(card => card.Id == id)
                .FirstOrDefault();
        }

        public IEnumerable<Card> GetAllCards()
        {
            return FindAll()
                .ToList();
        }
        public void Save(Card card)
        {
            Create(card);
            SaveChanges();
        }
    }
}
