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
    
    public List<String> ErrorMessages { get; set; } = new();
    
    private Random Rnd { get; set; } = new();

    public UnoEngine(GameState state)
    {
        _state = state;
    }

    public UnoEngine()
    {
        _state = new GameState();
    }
    
    public UnoEngine(GameOptions gameOptions, List<Player> players)
    {
        StartNewGame(gameOptions, players);
    }
    
    public void StartNewGame(GameOptions gameOptions, List<Player> players)
    {
        _state = new GameState(GenerateDeckOfCards(), players, gameOptions, 0);
        RandomizeStartingPlayer();
        foreach (var player in _state.Players)
        {
            player.PlayerHand = new List<GameCard>();
        }
        ShuffleDeckOfCards();
        DealCards();
        while (_state.DeckOfGameCardsInPlay[^1]!.CardText == ECardText.ChooseColorPlusFour)
        {
            ShuffleDeckOfCards();
        }
        _state.DeckOfCardsGraveyard.Add(TakeCardFromDeck());
        _state.LastCardPlayed = _state.DeckOfCardsGraveyard[^1];
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
        int playerNumber = Rnd.Next(_state.Players.Count());
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

    private bool Validate(String playerChoice)
    {
        switch (_state.TurnState)
        {
            case ETurnState.PlayCard:
            {
                if (playerChoice == "p")
                {
                    return true;
                }
                if (!int.TryParse(playerChoice, out int result))
                {
                    ErrorMessages.Add("Input must be an number or p -to pick a card up.");
                    return false;
                }
                int cardCount = _state.CurrentPlayerHand().Count;
                if (!(0 < result && result <= cardCount))
                {
                    ErrorMessages.Add($"You can only play cards 1 - {cardCount}");
                    return false;
                }
                if (!ValidateCardPlayed(_state.CurrentPlayerHand()[result - 1]!))
                {
                    ErrorMessages.Add($"Can not play {_state.CurrentPlayerHand()[result - 1]} on top of {_state.LastCardPlayed}.");
                    return false;
                }
                break;
            }
            case ETurnState.PlayCardAfterPickingUp:
            {
                if (playerChoice is not "y" and not "n" and not "")
                {
                    ErrorMessages.Add("Valid choices are (y - to play picked up card) and (n - to not play it).");
                    return false;
                }
                break;
            }
            case ETurnState.ChooseColor:
            {
                if (playerChoice is not "y" and not "g" and not "b" and not "r")
                {
                    ErrorMessages.Add("Valid choices are (y - yellow), (g - green), (b - blue), (r - red).");
                    return false;
                }
                break;
            }
            case ETurnState.PlusTwo:
            {
                break;
            }
            case ETurnState.WildPlusFour:
            {
                if (playerChoice is not "p" and not "c")
                {
                    ErrorMessages.Add("Valid choices are (p - pick up cards) and (c - call bluff).");
                    return false;
                }
                break;
            }
            case ETurnState.RevealLastPlayerCards:
            {
                break;
            }
            case ETurnState.Skip:
            {
                break;
            }
        }

        return true;
    }

    public void MakeMove(String playerChoice)
    {
        ErrorMessages = new();
        if (Validate(playerChoice))
        {
            ExecuteMove(playerChoice);
            NextPlayerMove();
        }
    }

    public void ExecuteMove(String playerChoice)
    {
        switch (_state.TurnState)
        {
            case ETurnState.PlayCard:
            {
                if (playerChoice == "p")
                {
                    PickUpCards();
                }
                else
                {
                    PlayCard(playerChoice);
                    CheckIfWin();
                }
                break;
            }
            case ETurnState.PlayCardAfterPickingUp:
            {
                if (playerChoice == "y")
                {
                    PlayCard(_state.CurrentPlayerHand()[^1]!);
                }
                else
                {
                    _state.TurnState = ETurnState.PlayCard;
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
                WildPlusFourAction();
                _state.TurnState = ETurnState.RevealLastPlayerCards;
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
            if (ValidateCardPlayed(_state.CurrentPlayerHand()[^1]!))
            {
                _state.TurnState = ETurnState.PlayCardAfterPickingUp;
                _state.DoubleTurn = true;
            }
        }
        else
        {
            for (int i = 0; i < _state.CardsToPickUp; i++)
            {
                DrawCardForPlayer(_state.CurrentPlayer());
            }
            _state.CardsToPickUp = 0;
        }
    }

    private void CheckIfWin()
    {
        if (_state.CurrentPlayerHand().Count != 0)
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
                pointsWon += GetCardPointValue(card);
            }
        }

        return pointsWon;
    }

    private int GetCardPointValue(GameCard card)
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
            _state.DoubleTurn = true;
            _state.TurnState = ETurnState.RevealLastPlayerCards;
        }
    }

    private void WildPlusFourAction()
    {
        if (WildPlusFourReveal())
        {
            for (int i = 0; i < 4; i++)
            {
                DrawCardForPlayer(_state.GetLastPlayer());
                
            }
        }
        else
        {
            for (int i = 0; i < 6; i++)
            {
                DrawCardForPlayer(_state.CurrentPlayer());
            }
        }
    }
    
    public bool WildPlusFourReveal()
    {
        Player playerWhoPlayedPlusFour = _state.GetLastPlayer();
        foreach (var card in playerWhoPlayedPlusFour.PlayerHand)
        {
            if (ValidateCardPlayed(card))
            {
                return true;
            }
        }
        return false;
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

    private void PlayCard(String playerChoice)
    {
        PlayCard(_state.CurrentPlayerHand()[int.Parse(playerChoice) - 1]!);
    }

    private void PlayCard(GameCard card)
    {
        _state.CurrentPlayerHand().Remove(card);
        _state.DeckOfCardsGraveyard.Add(card);
        _state.LastCardPlayed = card;

        switch (card.CardText)
        {
            case ECardText.ChooseColor:
                _state.DoubleTurn = true;
                _state.TurnState = ETurnState.ChooseColor;
                break;

            case ECardText.PlusTwo:
                _state.CardsToPickUp += 2;
                _state.TurnState = ETurnState.PlusTwo;
                break;

            case ECardText.ChooseColorPlusFour:
                _state.DoubleTurn = true;
                _state.CardsToPickUp += 4;
                _state.TurnState = ETurnState.ChooseColor;
                break;

            case ECardText.Reverse:
                if (_state.Players.Count == 2)
                {
                    _state.TurnState = ETurnState.Skip;
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
            _state.DoubleTurn = false;
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
    
    public List<EPlayerAction> GetValidActions()
    {
        List<EPlayerAction> playerActions = [];
        switch (_state.TurnState)
        {
            case ETurnState.PlayCard:
            {
                foreach (var card in _state.CurrentPlayerHand())
                {
                    if (ValidateCardPlayed(card))
                    {
                        playerActions.Add(EPlayerAction.PlayCard);
                        break;
                    }
                }
                playerActions.Add(EPlayerAction.PickUpCards);
                break;
            }
            case ETurnState.PlayCardAfterPickingUp:
            {
                playerActions.Add(EPlayerAction.PlayDrawnCard);
                break;
            }
            case ETurnState.ChooseColor:
            {
                playerActions.Add(EPlayerAction.ChooseColor);
                break;
            }
            case ETurnState.PlusTwo:
            {
                playerActions.Add(EPlayerAction.PickUpCards);
                break;
            }
            case ETurnState.WildPlusFour:
            {
                playerActions.Add(EPlayerAction.PickUpCards);
                playerActions.Add(EPlayerAction.CallBluff);
                break;
            }
            case ETurnState.RevealLastPlayerCards:
            {
                playerActions.Add(EPlayerAction.DoNothing);
                break;
            }
            case ETurnState.Skip:
            {
                playerActions.Add(EPlayerAction.DoNothing);
                break;
            }
        }

        return playerActions;
    }
}