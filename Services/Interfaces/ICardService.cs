using HomeBankingNet8.DTOs;
using HomeBankingNet8.Models;
using HomeBankingNet8.Utils;

namespace HomeBankingNet8.Services.Interfaces
{
    public interface ICardService
    {
        public Response<CardDTO> CreateNewCard(NewCardDTO newCardDTO, string email);

    }
}
