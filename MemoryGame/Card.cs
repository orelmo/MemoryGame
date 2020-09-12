using System;
using System.Drawing;

namespace MemoryGame
{
    public class Card
    {
        private char m_Value;
        private bool m_IsVisable = false;

        public Card(char i_CardValue)
        {
            m_Value = i_CardValue;
        }
        
        public bool IsVisable 
        {
            get
            {
                return m_IsVisable;
            }

            set
            {
                m_IsVisable = value;
            }
        }

        public char Value
        {
            get
            {
                return m_Value;
            }
        }

        public class CardCoordinate
        {
            private Point? m_FirstCoordinate = null;
            private Point? m_SecondCoordinate = null;
            private byte m_Appearances = 0;

            public byte Appearance
            {
                get
                {
                    return m_Appearances;
                }
            }

            public void AddApearance()
            {
                ++m_Appearances;
            }

            public Point? FirstCoordinate
            {
                get
                {
                    return m_FirstCoordinate;
                }

                set
                {
                    m_FirstCoordinate = value;
                }
            }

            public Point? SecondCoordinate
            {
                get
                {
                    return m_SecondCoordinate;
                }

                set
                {
                    m_SecondCoordinate = value;
                }
            }
            
            public CardCoordinate(Point? i_FirstCoordinate = null, Point? i_SecondCoordinate = null)
            {
                m_FirstCoordinate = i_FirstCoordinate;
                m_SecondCoordinate = i_SecondCoordinate;
            }
        }
    }
}
