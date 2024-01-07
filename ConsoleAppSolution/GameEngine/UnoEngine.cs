using System.Runtime.InteropServices.JavaScript;
using Domain;

namespace GameEngine;

public class UnoEngine
{
    public GameState State {
        get => _state.Clone();
        private set => _state = value;
    }

    private GameState _state { get; set; }

    public String? ErrorMessage { get; set; } = null;
    
    private Random Rnd { get; set; } = new();

    public UnoEngine(GameState state)
    {
        _state = state;
    }
    
    public UnoEngine(GameOptions gameOptions, List<Player> players)
    {
        StartNewGame(gameOptions, players);
    }
    
    public void StartNewGame(GameOptions gameOptions, List<Player> players)
    {
        _state = new GameState(GenerateDeckOfCards(), players, gameOptions, 0);
        ShuffleDeckOfCards();
        DealCards();
        _state.DeckOfCardsGraveyard.Add(TakeCardFromDeck());
        _state.LastCardPlayed = _state.DeckOfCardsGraveyard[^1];
        RandomizeStartingPlayer();
        TriggerFirstCardEffect();
    }

    private void TriggerFirstCardEffect()
    {
        switch (_state.LastCardPlayed.CardText)
        {
            case ECardText.Reverse:
                _state.IsReverseEffectActive = !_state.IsReverseEffectActive;
                break;
            case ECardText.PlusTwo:
                _state.CardsToPickUp += 2;
                _state.TurnState = ETurnState.PlusTwo;
                break;
            case ECardText.ChooseColor:
                _state.TurnState = ETurnState.ChooseColor;
                _state.DoubleTurn = true;
                break;
        }
    }

    private void RandomizeStartingPlayer()
    {
        int playerNumber = Rnd.Next(_state.Players.Count() - 1);
        _state.ActivePlayerNr = playerNumber;
    }

    private List<GameCard?> GenerateDeckOfCards()
    {
        List<GameCard> generatedDeck = new();
        
        //One of each card for value 0 in every color
        for (var color = 0; color < 4; color++)
        {
            generatedDeck.Add(new GameCard(ECardText.Zero, (ECardColor) color));
        }
        
        //Two of each card for values 1-9, skip, reverse, plus two in every color.
        for (var i = 0; i < 2; i++)
        {
            for (var j = 1; j < 13; j++)
            {
                for (var k = 0; k < 4; k++)
                {
                    generatedDeck.Add(new GameCard((ECardText) j, (ECardColor) k));
                }
            }
        }

        //Four of each card for ChooseColor and ChooseColorPlusFour.
        for (var i = 0; i < 4; i++)
        {
            generatedDeck.Add(new GameCard(ECardText.ChooseColorPlusFour, ECardColor.Wild));
            generatedDeck.Add(new GameCard(ECardText.ChooseColor, ECardColor.Wild));
        }
        return generatedDeck;
    }

    private void ShuffleDeckOfCards()
    {
        while (true)
        {
            List<GameCard?> shuffledDeck = new();
            while (_state.DeckOfGameCardsInPlay.Count > 0)
            {
                int randomPositionInDeck = Rnd.Next(_state.DeckOfGameCardsInPlay.Count());
                shuffledDeck.Add(_state.DeckOfGameCardsInPlay[randomPositionInDeck]);
                _state.DeckOfGameCardsInPlay.RemoveAt(randomPositionInDeck);
            }

            _state.DeckOfGameCardsInPlay = shuffledDeck;
            if (shuffledDeck[^1].CardText == ECardText.ChooseColorPlusFour)
            {
                continue;
            }

            break;
        }
    }

    private List<Player> GetPlayers()
    {
        List<Player> players = new();
        players.Add(new Player()
        {
            NickName = "Madis",
            PlayerType = EPlayerType.Human,
        });        
        players.Add(new Player()
        {
            NickName = "Adolf",
            PlayerType = EPlayerType.AI,
        });

        return players;
    }

    private void DealCards()
    {
        foreach (var player in _state.Players)
        {
            //deal 7 cards for each player
            for (int i = 0; i < _state.GameOptions.StartingHandSize; i++)
            {
                DrawCardForPlayer(player);
            }
        }
    }
    
    public bool ValidateCardPlayed(GameCard cardPlayed)
    {
        var bottomCard = _state.LastCardPlayed;
        if (cardPlayed.CardColor == ECardColor.Wild)
        {
            return true;
        } 
        else if (cardPlayed.CardColor == bottomCard.CardColor || cardPlayed.CardText == bottomCard.CardText)
        {
            return true;
        }
        return false;
    }

    private void ValidatePlayerMove(String? playerChoice)
    {

        List<int> cardsToBePlayed = playerChoice!.Split(",").Select(s => int.Parse(s.Trim())).ToList();
        if (_state.GameOptions.MultibleCardsPlayedPerTurn == false)
        {
            if (cardsToBePlayed.Count != 1)
            {
                ErrorMessage = "Player can play only one card at a time.";
            }

            

            if (ValidateCardPlayed(_state.currentPlayerHand()[cardsToBePlayed[0] - 1]) == false)
            {
                ErrorMessage = $"Can not play {_state.currentPlayerHand()[cardsToBePlayed[0] - 1]} on top of {_state.LastCardPlayed}.";
            }

        }
        else
        {
            throw new NotImplementedException();
        }
    }

    public void MakePlayerMove(String playerChoice)
    {
        List<int> cardsToBePlayed = playerChoice!.Split(",").Select(s => int.Parse(s.Trim())).ToList();
        ValidatePlayerMove(playerChoice);
        if (ErrorMessage == null)
        {
            PlayCard(_state.currentPlayerHand()[cardsToBePlayed[0] - 1]);
        }
    }

    public void TryToMakePlayerMove(String playerChoice)
    {
        switch (_state.TurnState)
        {
            case ETurnState.PlayCard:
            {
                if (playerChoice == "p")
                {
                    PickUpCards();
                    _state.TurnState = ETurnState.PlayCardAfterPickingUp;
                }
                else
                {
                    MakePlayerMove(playerChoice);
                    CheckIfWin();
                }
                break;
            }
            case ETurnState.PlayCardAfterPickingUp:
            {
                if (playerChoice == "y")
                {
                    MakePlayerMove(_state.currentPlayerHand().Count.ToString());
                }
                else if (playerChoice is "" or "n")
                {
                    NextPlayerMove();
                }
                else
                {
                    ErrorMessage = "Not a valid choice";
                }
                break;
            }
            case ETurnState.ChooseColor:
            {
                ChangeColor(playerChoice);
                break;
            }
            case ETurnState.PlusTwo:
            {
                PickUpCards();
                break;
            }
            case ETurnState.WildPlusFour:
            {
                WildPlusFour(playerChoice);
                break;
            }
            case ETurnState.RevealLastPlayerCards:
            {
                WildPlusFourReveal();
                break;
            }
            case ETurnState.Skip:
            {
                _state.TurnState = ETurnState.PlayCard;
                break;
            }
        }
    }

    private void PickUpCards()
    {
        if (_state.CardsToPickUp == 0)
        {
            DrawCardForPlayer(_state.CurrentPlayer());
        }
        for (int i = 0; i < _state.CardsToPickUp; i++)
        {
            DrawCardForPlayer(_state.CurrentPlayer());
            _state.CardsToPickUp = 0;
        }
    }

    private void CheckIfWin()
    {
        if (_state.currentPlayerHand().Count != 0)
        {
            return;
        }

        _state.Winner = _state.CurrentPlayer();
        if (_state.CardsToPickUp != 0)
        {
            NextPlayerMove();
            PickUpCards();
        }
        
        _state.TurnState = ETurnState.ScoreBoard;
        _state.Winner.Points += CalculateWinnerPoints();
    }

    private int CalculateWinnerPoints()
    {
        int pointsWon = 0;
        foreach (Player player in _state.Players)
        {
            foreach (GameCard card in player.PlayerHand)
            {
                pointsWon += GetCardPoinValue(card);
            }
        }

        return pointsWon;
    }

    private int GetCardPoinValue(GameCard card)
    {
        switch (card.CardText)
        {
            case ECardText.ChooseColor:
            case ECardText.ChooseColorPlusFour:
                return 50;
            case ECardText.Reverse:
            case ECardText.Skip:
            case ECardText.PlusTwo:
                return 20;
            default: return (int) card.CardText;
        }
    }

    private void WildPlusFour(String playerChoice)
    {
        if (playerChoice == "p")
        {
            PickUpCards();
        } else if (playerChoice == "c")
        {
            _state.TurnState = ETurnState.RevealLastPlayerCards;
        }else 
        {
            ErrorMessage = "Not a valid option choose (p - to pick) up or (c - to call bluff)";
        }
    }
    
    private void WildPlusFourReveal()
    {
        // TODO: Check if should draw 6 or other player draws cards
        return;
    }
    
    

    public bool IsTurnOver()
    {
        return _state.TurnState is not (ETurnState.PlayCardAfterPickingUp or ETurnState.ChooseColor);
    }

    private void ChangeColor(String? colorChoice)
    {
        var card = _state.LastCardPlayed;
        if (colorChoice == "g")
        {
            card.CardColor = ECardColor.Green;
            
        }else if (colorChoice == "r")
        {
            card.CardColor = ECardColor.Red;
        }else if (colorChoice == "b")
        {
            card.CardColor = ECardColor.Blue;
        }else if (colorChoice == "y")
        {
            card.CardColor = ECardColor.Yellow;
        }
        else
        {
            ErrorMessage = "Not a valid color.";
        }

        if (ErrorMessage == null)
        {
            NextPlayerMove();
            _state.LastCardPlayed = card;
            if (_state.LastCardPlayed.CardText == ECardText.ChooseColorPlusFour)
            {
                _state.TurnState = ETurnState.WildPlusFour;
            }
            else
            {
                _state.TurnState = ETurnState.PlayCard;
            }
        }
    }
    
    private void DrawCardForPlayer(Player player)
    {
        if (_state.DeckOfGameCardsInPlay.Count == 0)
        {
            ReShuffleDeck();
        }
        player.PlayerHand.Add(TakeCardFromDeck());
        _state.TurnState = ETurnState.PlayCard;
    }

    private void ReShuffleDeck()
    {
        GameCard lastCard = _state.DeckOfCardsGraveyard[^1];
        _state.DeckOfCardsGraveyard.Remove(lastCard);
        
        _state.DeckOfGameCardsInPlay.AddRange(_state.DeckOfCardsGraveyard);
        _state.DeckOfCardsGraveyard = [lastCard];
        
        ShuffleDeckOfCards();
    }

    private void PlayCard(GameCard card)
    {
        _state.currentPlayerHand().Remove(card);
        _state.DeckOfCardsGraveyard.Add(card);
        _state.LastCardPlayed = card;

        switch (card.CardText)
        {
            case ECardText.ChooseColor:
                _state.TurnState = ETurnState.ChooseColor;
                break;

            case ECardText.PlusTwo:
                _state.CardsToPickUp += 2;
                _state.TurnState = ETurnState.PlusTwo;
                break;

            case ECardText.ChooseColorPlusFour:
                _state.CardsToPickUp += 4;
                _state.TurnState = ETurnState.ChooseColor;
                break;

            case ECardText.Reverse:
                if (_state.Players.Count == 2)
                {
                }
                else
                {
                    _state.IsReverseEffectActive = !_state.IsReverseEffectActive;
                }
                break;

            case ECardText.Skip:
                _state.TurnState = ETurnState.Skip;
                break;
            default:
                _state.TurnState = ETurnState.PlayCard;
                break;
        }
    }



    public void NextPlayerMove()
    {
        if (_state.DoubleTurn)
        {
            return;
        }
        if (_state.IsReverseEffectActive)
        {
            _state.ActivePlayerNr = (_state.ActivePlayerNr - 1 + _state.Players.Count) % _state.Players.Count;
        }
        else
        {
            _state.ActivePlayerNr = (_state.ActivePlayerNr + 1) % _state.Players.Count;
        }
    }

    private GameCard? TakeCardFromDeck()
    {
        int cardPosition = _state.DeckOfGameCardsInPlay.Count - 1;
        if (cardPosition < 0)
        {
            return null;
        }
        GameCard? card = _state.DeckOfGameCardsInPlay[cardPosition];
        _state.DeckOfGameCardsInPlay.RemoveAt(cardPosition);
        return card;
    }

    public Player? GetGameWinner()
    {
        foreach (Player player in _state.Players)
        {
            if (player.Points >= _state.GameOptions.ScoreToWin)
            {
                return player;
            }
        }
        return null;
    }
}