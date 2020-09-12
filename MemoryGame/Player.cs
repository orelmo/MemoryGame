using System;
using System.Collections.Generic;
using System.Drawing;

namespace MemoryGame
{
    public class Player
    {
        private string m_Name;
        private bool m_IsComputer;
        private byte m_Points;
        private ComputerAI m_OptionalChoices = null;

        public static bool CheckPlayerName(string i_PlayerName)
        {
            return !(i_PlayerName.Length == 0);
        }

        public static bool CheckOpponentSelection(string i_PlayersChoiceStr, out int o_PlayerChoiceInt)
        {
            return int.TryParse(i_PlayersChoiceStr, out o_PlayerChoiceInt);
        }

        public static bool tryPharseIntToeOpponent(int i_PlayerChoiceInt)
        {
            return i_PlayerChoiceInt == 1 || i_PlayerChoiceInt == 2;
        }

        public static eWinningStatus GetWinningStatus(Player i_Player1, Player i_Player2)
        {
            eWinningStatus retWinner;
            if (i_Player1.m_Points > i_Player2.m_Points)
            {
                retWinner = eWinningStatus.Player1Won;
            }
            else if (i_Player1.m_Points < i_Player2.m_Points)
            {
                retWinner = eWinningStatus.Player2Won;
            }
            else
            {
                retWinner = eWinningStatus.Tie;
            }

            return retWinner;
        }

        public static eAnswer CheckPlayerAnswer(string i_Answer)
        {
            eAnswer retAnswer;
            if (i_Answer.ToUpper().CompareTo('Y'.ToString()) == 0)
            {
                retAnswer = eAnswer.Yes;
            }
            else if (i_Answer.ToUpper().CompareTo('N'.ToString()) == 0)
            {
                retAnswer = eAnswer.No;
            }
            else
            {
                retAnswer = eAnswer.Undefined;
            }

            return retAnswer;
        }

        public static eCellChoosementException CheckPlayerCellChoise(string i_PlayerCohice, Board i_Board)
        {
            eCellChoosementException cellChoosementException;
            if (i_PlayerCohice.Length == 1 && char.ToUpper(i_PlayerCohice[0]) == 'Q')
            {
                cellChoosementException = eCellChoosementException.Quit;
            }
            else if (i_PlayerCohice.Length != 2 || !char.IsLetter(i_PlayerCohice[0]) || !char.IsDigit(i_PlayerCohice[1]))
            {
                cellChoosementException = eCellChoosementException.InvalidCell;
            }
            else if (char.ToUpper(i_PlayerCohice[0]) - 'A' > i_Board.Cols || byte.Parse(i_PlayerCohice[1].ToString()) > i_Board.Rows)
            {
                cellChoosementException = eCellChoosementException.OutOfBoardersCell;
            }
            else if (i_Board.Matrix[byte.Parse(i_PlayerCohice[1].ToString()) - 1, byte.Parse((char.ToUpper(i_PlayerCohice[0]) - 'A').ToString())].IsVisable)
            {
                cellChoosementException = eCellChoosementException.RevealedCell;
            }
            else
            {
                cellChoosementException = eCellChoosementException.Valid;
            }

            return cellChoosementException;
        }

        public ComputerAI OptionalChoices
        {
            get
            {
                return m_OptionalChoices;
            }

            set
            {
                m_OptionalChoices = value;
            }
        }

        public void InitializedMemmory(Board i_Board)
        {
            this.OptionalChoices = new ComputerAI(i_Board);
        }

        public class ComputerAI
        {
            private List<Card.CardCoordinate> m_OptionalChoices;
            private List<Point> m_AvaliableCoordinates;

            public ComputerAI(Board i_Board)
            {
                m_OptionalChoices = new List<Card.CardCoordinate>(i_Board.Rows * i_Board.Cols / 2);
                m_AvaliableCoordinates = new List<Point>(i_Board.Rows * i_Board.Cols);
                initializeAvaliableCoordinate(i_Board);
                initializeOptionalChoices(i_Board);
            }

            private void initializeOptionalChoices(Board i_Board)
            {
                for(int i = 0; i < i_Board.Rows * i_Board.Cols / 2; ++i)
                {
                    m_OptionalChoices.Add(new Card.CardCoordinate());
                }
            }

            private void initializeAvaliableCoordinate(Board i_Board)
            {
                for (int i = 0; i < i_Board.Rows; ++i)
                {
                    for (int j = 0; j < i_Board.Cols; ++j)
                    {
                        m_AvaliableCoordinates.Add(new Point(i, j));
                    }
                }
            }

            public void Memmorize(Card i_Card, Point i_CardCoordinate)
            {
                int cardIndex = i_Card.Value - 'A';
                if(m_OptionalChoices[cardIndex].Appearance == 0)
                {
                    m_OptionalChoices[cardIndex].FirstCoordinate = i_CardCoordinate;
                    m_OptionalChoices[cardIndex].AddApearance();
                }                
                else if(m_OptionalChoices[cardIndex].FirstCoordinate != i_CardCoordinate && m_OptionalChoices[cardIndex].Appearance == 1)
                {
                    m_OptionalChoices[cardIndex].SecondCoordinate = i_CardCoordinate;
                    m_OptionalChoices[cardIndex].AddApearance();
                }
            }

            public void Forget(Card i_Card)
            {
                int cardIndex = i_Card.Value - 'A';
                m_AvaliableCoordinates.Remove((Point)m_OptionalChoices[cardIndex].FirstCoordinate);
                m_AvaliableCoordinates.Remove((Point)m_OptionalChoices[cardIndex].SecondCoordinate);
                m_OptionalChoices[cardIndex] = null;
            }

            public Card.CardCoordinate ChooseCardFromMemmory()
            {
                Card.CardCoordinate retCard = null;
                foreach (Card.CardCoordinate cardInMemmory in m_OptionalChoices)
                {
                    if (cardInMemmory != null && cardInMemmory.Appearance == 2)
                    {
                        retCard = cardInMemmory;
                    }
                }
             
                return retCard;
            }

            public Card.CardCoordinate ChooseRandomCards(Board i_Board)
            {
                Random rand = new Random();
                int firstIndexOfCoordinate = rand.Next(m_AvaliableCoordinates.Count);
                int secondIndexOfCoordinate;
                do
                {
                    secondIndexOfCoordinate = rand.Next(m_AvaliableCoordinates.Count);
                } 
                while(secondIndexOfCoordinate == firstIndexOfCoordinate);

                return new Card.CardCoordinate(m_AvaliableCoordinates[firstIndexOfCoordinate], m_AvaliableCoordinates[secondIndexOfCoordinate]);
            }
        }

        public void AddPoint()
        {
            ++m_Points;
        }

        public bool IsComputer
        {
            get
            {
                return m_IsComputer;
            }
        }

        public byte Points
        {
            get
            {
                return m_Points;
            }
        }

        public string Name
        {
            get 
            {
                return m_Name;
            }
        }

        public enum eAnswer
        {
            Yes = 1,
            No,
            Undefined
        }

        public enum eCellChoosementException
        {
            RevealedCell = 1,
            OutOfBoardersCell,
            InvalidCell,
            Valid,
            Quit
        }

        public enum eOpponent
        {
            Computer = 1,
            Player
        }

        public enum eWinningStatus
        {
            Player1Won = 1,
            Player2Won,
            Tie
        }

        public Player(bool i_IsComputer, string playerName)
        {
            m_IsComputer = i_IsComputer;
            m_Name = playerName;                      
            m_Points = 0;
        }   
    }
}
