using System;
using System.Collections.Generic;
using System.Drawing;

namespace MemoryGame
{
    public struct Board
    {
        private byte m_Cols;
        private byte m_Rows;
        private Card[,] m_Matrix;
        public const byte k_MinimumBoarderSize = 4;
        public const byte k_MaximumBoarderSize = 6;

        internal class BoardBuilder
        {
            internal LinkedList<CardListNode> m_CardValuesList = new LinkedList<CardListNode>();

            internal CardListNode this [int i_Index]
            {
                get
                {
                    LinkedList<CardListNode>.Enumerator iterator = m_CardValuesList.GetEnumerator();
                    for (int i = 0; i <= i_Index; i++)
                    {
                        iterator.MoveNext();
                    }

                    return iterator.Current;
                }             
            }

            internal static void BuildBoard(Board o_Board)
            {
                BoardBuilder builder = new BoardBuilder();
                buildCardValuesList(o_Board, builder);
                Random rand = new Random();
                for (int i = 0; i < o_Board.Rows; ++i)
                {
                    for (int j = 0; j < o_Board.Cols; ++j)
                    {
                        int randIndex = rand.Next(builder.m_CardValuesList.Count);
                        o_Board.Matrix[i, j] = new Card(builder[randIndex].m_Card.Value);
                        ++builder[randIndex].m_AppearanceTimes;
                        if (builder[randIndex].m_AppearanceTimes == 2)
                        {
                            builder.m_CardValuesList.Remove(builder[randIndex]);
                        }
                    }
                }
            }

            private static void buildCardValuesList(Board i_Board, BoardBuilder o_Builder)
            {
                for (int i = 0; i < i_Board.Rows * i_Board.Cols / 2; ++i)
                {
                    CardListNode cell = new CardListNode((char)('A' + i));
                    o_Builder.m_CardValuesList.AddLast(cell);
                }
            }

            internal class CardListNode
            {
                internal Card m_Card = null;
                internal byte m_AppearanceTimes = 0;

                public CardListNode(char i_CardValue)
                {
                    m_Card = new Card(i_CardValue);
                }
            }
        }

        public byte Rows
        {
            get
            {
                return m_Rows;
            }
        }

        public byte Cols
        {
            get
            {
                return m_Cols;
            }
        }

        public Card[,] Matrix
        {
            get
            {
                return m_Matrix;
            }
        }

        public enum eBoardSizeExceptions
        {
            OutOfBoarderLines = 1,
            OddBoardSize,
            NotInteger,
            NoException
        }

        public Board(byte i_Rows, byte i_Cols)
        {
            m_Rows = i_Rows;
            m_Cols = i_Cols;
            m_Matrix = new Card[m_Rows, m_Cols];
            BoardBuilder.BuildBoard(this);
        }
        
    public static eBoardSizeExceptions CheckRowsAndCols(string i_Rows, string i_Cols)
        {
            eBoardSizeExceptions retException;
            if (!byte.TryParse(i_Rows, out byte numericRows) || !byte.TryParse(i_Cols, out byte numericCols))
            {
                retException = eBoardSizeExceptions.NotInteger;
            }
            else if (numericRows < k_MinimumBoarderSize || numericRows > k_MaximumBoarderSize || numericCols < k_MinimumBoarderSize || numericCols > k_MaximumBoarderSize)
            {
                retException = eBoardSizeExceptions.OutOfBoarderLines;
            }
            else if ((numericRows * numericCols) % 2 != 0)
            {
                retException = eBoardSizeExceptions.OddBoardSize;
            }
            else
            {
                retException = eBoardSizeExceptions.NoException;
            }

            return retException;
        }

        public bool IsFull()
        {
            bool thereUnvisableCard = false;
            foreach (Card card in m_Matrix)
            {
                if (!card.IsVisable)
                {
                    thereUnvisableCard = true;
                    break;
                }
            }

            return !thereUnvisableCard;
        }

        public static Point TranslatePlayerCellChoiceToCoordinate(string i_PlayerCohice)
        {
            int col = char.ToUpper(i_PlayerCohice[0]) - 'A';
            int row = int.Parse(i_PlayerCohice[1].ToString()) - 1;

            return new Point(row, col);
        }
    }
}