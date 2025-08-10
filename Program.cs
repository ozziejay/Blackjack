using System;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;

class Game
{
    static List<Card> PlayerCards = new List<Card>(); 
    static List<Card> DealerCards = new List<Card>(); 
    static List<Card> deck = new List<Card>(); 
    static Random random = new Random(); 
    static void Main()
    {
        Console.WriteLine("Dealer stands on soft 17 \nwelcome to blackjack type Play to start or Exit to quit");
        string Start = Console.ReadLine().ToLower();
        if (Start == "play")
        {
            CardsReset();
        }
        else if (Start == "exit")
        {
            System.Environment.Exit(0);
        }
        else
        {
            Console.WriteLine("no valid input given");
            Main();
        }

    }
    static void InitialiseDeck()
    {
        var cardTypes = new List<string> { "Spades", "Diamonds", "Hearts", "Clubs" };
        var cardValues = Enum.GetValues(typeof(Cards));

        for (int d = 0; d < 4; d++) 
        {
            for (int c = 0; c < cardValues.Length; c++) 
            {
                foreach (var cardType in cardTypes)
                {
                    deck.Add(new Card
                    {
                        Name = Enum.GetName(typeof(Cards), cardValues.GetValue(c)),
                        Type = cardType,
                        Value = (int)cardValues.GetValue(c)
                    });
                }
            }
        }
    }
    static void Play()
    {
        Console.Clear();
        var handValues = HandValues(); 
        int playerHandValue = handValues.PlayerHandValue; 
        int dealerHandValue = handValues.DealerHandValue; 

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Player cards: {string.Join(", ", PlayerCards.Select(card => card.Name))}, total: {playerHandValue}"); 
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Dealer cards: {DealerCards[0].Name}, ?");
        Console.ResetColor();
        if (playerHandValue > 21)
        {
            Console.WriteLine("Bust! you lose \n\n");
            Main();
        } 
        else if (playerHandValue == 21 && dealerHandValue < 21 && PlayerCards.Count == 2 ) 
        {
            Console.WriteLine("blackjack! you win \n\n");
            Main();
        }
        else if (playerHandValue == 21 && dealerHandValue < 21) 
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Dealer cards: {string.Join(", ", DealerCards.Select(card => card.Name))}, total: {dealerHandValue}");
            Console.ResetColor();
            Console.WriteLine("21! you win \n\n");
            Main();
        }
        Console.WriteLine("type Hit to hit, type Stand to stand");
        string Action = Console.ReadLine().ToLower();
        if (Action == "hit") 
        {
            Card newCard = NextCard();
            if (newCard == null) return;
            PlayerCards.Add(newCard);
            Play();
        }
        else if(Action == "stand") 
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Player cards: {string.Join(", ", PlayerCards.Select(card => card.Name))}, total: {playerHandValue}");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Dealer cards: {string.Join(", ", DealerCards.Select(card => card.Name))}, total: {dealerHandValue}");
            Console.ResetColor();
            Stand();
        }
        else
        {
            Console.WriteLine("no valid input given");
            Play();
        }
    }
    static Card NextCard()
    {
        if (deck.Count == 0) 
        {
            Console.WriteLine("Deck is empty!");
            return null;
        }

        int index = random.Next(deck.Count); 
        Card drawnCard = deck[index]; 
        deck.RemoveAt(index); 
        return drawnCard;
    }
    static void Stand()
    {
        var handValues = HandValues(); 
        int playerHandValue = handValues.PlayerHandValue; 
        int dealerHandValue = handValues.DealerHandValue; 

        bool isSoft17 = dealerHandValue == 17 && DealerCards.Any(card => card.Name == "Ace" && card.Value == 11);

        while (dealerHandValue < 17 || (dealerHandValue == 17 && !isSoft17)) 
        {
            Card newCard = NextCard();
            if (newCard == null) return; 

            DealerCards.Add(newCard); 
            handValues = HandValues(); 
            dealerHandValue = handValues.DealerHandValue;

            Console.WriteLine("dealer Hits");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Dealer cards: {string.Join(", ", DealerCards.Select(card => card.Name))}, total: {dealerHandValue}");
            Console.ResetColor();

            isSoft17 = dealerHandValue == 17 && DealerCards.Any(card => card.Name == "Ace" && card.Value == 11); 
        }

        if (playerHandValue > 21)
        {
            Console.WriteLine("Bust, player lost \n\n");
        }
        else if (dealerHandValue > 21)
        {
            Console.WriteLine("Dealer bust, player won \n\n");
        }
        else if (playerHandValue > dealerHandValue)
        {
            Console.WriteLine("Player won \n\n");
        }
        else if (playerHandValue == dealerHandValue)
        {
            Console.WriteLine("Push \n\n");
        }
        else
        {
            Console.WriteLine("Player lost! \n\n");
        }

        Main(); 
    }
    static void CardsReset()
    {
        deck.Clear(); 
        InitialiseDeck(); 
        PlayerCards.Clear(); 
        DealerCards.Clear(); 
        for (int i = 0; i < 2; i++)
        {
            Card playerCard = NextCard();
            if (playerCard == null) return;
            Card dealerCard = NextCard();
            if (dealerCard == null) return;
            PlayerCards.Add(playerCard);
            DealerCards.Add(dealerCard);
        }
        Play();
    }
    static (int PlayerHandValue, int DealerHandValue) HandValues()
    {
        int CalculateHandValue(List<Card> hand) 
        {
            int totalValue = 0;
            int aceCount = 0;

            foreach (var card in hand)
            {
                if (card.Value == (int)Cards.Ace)
                {
                    totalValue += 11;
                    aceCount++;
                }
                else
                {
                    totalValue += card.Value;
                }
            }
            while (totalValue > 21 && aceCount > 0) 
            {
                totalValue -= 10;
                aceCount--;
            }
            return totalValue;
        }
        int PlayerHandValue = CalculateHandValue(PlayerCards);
        int DealerHandValue = CalculateHandValue(DealerCards);

        return (PlayerHandValue, DealerHandValue);
    }
}
enum Cards
{
    Ace = 1, Two, Three, Four, Five, Six,
    Seven, Eight, Nine, Ten, Jack = 10, Queen = 10, King = 10
}
class Card
{
    public string Name { get; set; }
    public string Type { get; set; }
    public int Value { get; set; }
}