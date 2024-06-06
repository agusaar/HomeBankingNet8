using HomeBankingNet8.DTOs;
using HomeBankingNet8.Models;

namespace HomeBankingNet8.Services.Interfaces
{
    public interface ICardService
    {
        public CardDTO CreateNewCard(NewCardDTO newCardDTO, Client ownerClient);

    }
}
