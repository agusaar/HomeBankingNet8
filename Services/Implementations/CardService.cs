﻿using HomeBankingNet8.DTOs;
using HomeBankingNet8.Models;
using HomeBankingNet8.Repositories.Interfaces;
using HomeBankingNet8.Services.Interfaces;
using HomeBankingNet8.Utils;

namespace HomeBankingNet8.Services.Implementations
{
    public class CardService: ICardService
    {
        private readonly ICardRepository _cardRepository;
        private readonly IClientRepository _clientRepository;

        public CardService(ICardRepository cardRepository, IClientRepository clientRepository)
        {
            _cardRepository = cardRepository;
            _clientRepository = clientRepository;
        }

        public Response<CardDTO> CreateNewCard(NewCardDTO newCardDTO,string email)
        {
            try
            {
                if(email == string.Empty)
                    return new Response<CardDTO>(null, 401);

                Client ownerClient = _clientRepository.FindByEmail(email);
                if (ownerClient == null)
                    return new Response<CardDTO>(null, 404);
                else
                {
                CardType newCardType = (CardType)Enum.Parse(typeof(CardType), newCardDTO.Type);
                var ownerCards = _cardRepository.FindByClientId(ownerClient.Id);
                int cantCards = ownerCards.Count(card => card.Type == newCardType);

                if (cantCards < 3)
                {
                    CardColor newCardColor = (CardColor)Enum.Parse(typeof(CardColor), newCardDTO.Color);
                    Random random = new Random();
                    int numeroRand;
                    string cardNum = "";
                    Boolean existeCardNum = true;
                    while (existeCardNum)
                    {
                        for (var i = 0; i < 4; i++)
                        {
                            for (var j = 0; j < 4; j++)
                            {
                                numeroRand = random.Next(0, 10);
                                cardNum += numeroRand;
                            }
                            if (i < 3)
                                cardNum += "-";
                        }
                        if (_cardRepository.FindByCardNum(cardNum) != null)
                            cardNum = "";
                        else
                            existeCardNum = false;
                    }

                    Card card = new Card
                    {
                        ClientId = ownerClient.Id,
                        CardHolder = ownerClient.FirstName + " " + ownerClient.LastName,
                        Type = newCardType,
                        Color = newCardColor,
                        Number = cardNum,
                        Cvv = random.Next(100, 1000),
                        FromDate = DateTime.Now,
                        ThruDate = DateTime.Now.AddYears(5)
                    };
                    _cardRepository.Save(card);
                    return new Response<CardDTO>(new CardDTO(card),200);

                }
                else
                {
                    return new Response<CardDTO>(null,403);
                }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
