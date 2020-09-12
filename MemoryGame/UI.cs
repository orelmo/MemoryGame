using System;
using System.Text;
using System.Threading;
using System.Drawing;
using Ex02.ConsoleUtils;

namespace MemoryGame
{
    public class UI
    {
        private const int k_RevealWaitingTime = 2000;

        public static void Run()
        {            
            const bool v_IsComputer = true;
            bool continuePlay = true;
            string playerName = getPlayerName();          
            Player.eOpponent opponent = selectComputerOrOtherPlayer();
            
            while (continuePlay)
            {
                bool player1Turn = true;
                Player player1 = new Player(!v_IsComputer, playerName);
                Player player2;
                if (opponent == Player.eOpponent.Computer)
                {
                    player2 = new Player(v_IsComputer, "Computer");
                }
                else
                {
                    playerName = getPlayerName();
                    player2 = new Player(!v_IsComputer, playerName);
                }

                byte rows, cols;
                bool keepPlaying = false;
                getBoardSize(out rows, out cols);               
                Board board = new Board(rows, cols);
                Screen.Clear();
                PrintBoard(board, player1, player2);
                if(player2.IsComputer)
                {
                    player2.InitializedMemmory(board);
                }

                while(!board.IsFull())
                {
                    if(player1Turn)
                    {
                        Console.WriteLine("{0} Turn", player1.Name);
                        keepPlaying = turn(player1, player2, board);
                        if(!keepPlaying)
                        {
                            player1Turn = false;
                        }
                    }
                    else
                    {
                        Console.WriteLine("{0} Turn", player2.Name);
                        if (player2.IsComputer)
                        {
                            Thread.Sleep(k_RevealWaitingTime);
                        }

                        keepPlaying = turn(player2, player1, board);
                        if (!keepPlaying)
                        {
                            player1Turn = true;
                        }
                    }                    
                }

                Screen.Clear();
                PrintBoard(board, player1, player2);
                CongratsWinner(player1, player2);
                AskIfAnotherRound(ref continuePlay);
            }
        }

        private static Player.eOpponent selectComputerOrOtherPlayer()
        {
            int playerChoiceInt;
            bool isValidOpponent = false;
            Console.WriteLine(
@"Do You Want To Play Against The Computer Or Against Other Player?
Press 1 For Computer Or 2 For Another Player.");
            do
            {
                string playersChoiceStr = Console.ReadLine();
                bool opponentChoosementSucceed = Player.CheckOpponentSelection(playersChoiceStr, out playerChoiceInt);
                if (!opponentChoosementSucceed)
                {
                    Console.WriteLine("Invalid Input! Please Try Again.");
                    continue;
                }

                if (!Player.tryPharseIntToeOpponent(playerChoiceInt))
                {
                    Console.WriteLine("Your Choise Was Not Between 1 Or 2. Please Try Again.");
                    continue;
                }

                isValidOpponent = true;
            }
            while (!isValidOpponent);

            return (Player.eOpponent)playerChoiceInt;
        }

        private static string getPlayerName()
        {
            string playerName;
            bool firstAttempt = true;
            do
            {
                if (!firstAttempt)
                {
                    Console.WriteLine("Invalid Input. Please Try Again.");
                }

                Console.WriteLine("Please Enter Your Name: (At Least One Character)");
                playerName = Console.ReadLine();
                firstAttempt = false;
            }
            while (!Player.CheckPlayerName(playerName));

            return playerName;
        }

        public static void PrintBoard(Board i_Board, Player i_Player1, Player i_Player2)
        {
            StringBuilder boardRepresentation = new StringBuilder(i_Board.Rows * i_Board.Cols * 10);
            boardRepresentation.Append(string.Format("Record:{0}{1}: {2}{0}{3}: {4}{0}{0}", Environment.NewLine, i_Player1.Name, i_Player1.Points, i_Player2.Name, i_Player2.Points));
            boardRepresentation.Append("    ");
            for (int i = 0; i < i_Board.Cols; i++)
            {
                boardRepresentation.Append(string.Format("{0}   ", (char)('A' + i)));
            }

            boardRepresentation.Append(Environment.NewLine);
            StringBuilder seperateLine = new StringBuilder();
            seperateLine.Append("  ");
            seperateLine.Append('=', (i_Board.Cols * 4) + 1);
            seperateLine.Append(Environment.NewLine);
            boardRepresentation.Append(seperateLine);
            for (int i = 0; i < i_Board.Rows; i++)
            {
                boardRepresentation.Append(i + 1);
                boardRepresentation.Append(" |");
                for (int j = 0; j < i_Board.Cols; j++)
                {
                    boardRepresentation.Append(" ");
                    if (i_Board.Matrix[i, j].IsVisable)
                    {
                        boardRepresentation.Append(i_Board.Matrix[i, j].Value);
                    }
                    else
                    {
                        boardRepresentation.Append(" ");
                    }

                    boardRepresentation.Append(" |");
                }

                boardRepresentation.Append(Environment.NewLine);
                boardRepresentation.Append(seperateLine);
            }

            Console.WriteLine(boardRepresentation);
        }

        private static void getBoardSize(out byte o_Rows, out byte o_Cols)
        {
            string rows, cols;
            Board.eBoardSizeExceptions exception;
            do
            {
                Console.WriteLine(string.Format("Please Enter The Number Of Rows And Cols (Between {0} to {1} With Enter Between Them)", Board.k_MinimumBoarderSize, Board.k_MaximumBoarderSize));
                rows = Console.ReadLine();
                cols = Console.ReadLine();
                exception = Board.CheckRowsAndCols(rows, cols);
                switch (exception)
                {
                    case Board.eBoardSizeExceptions.NotInteger:
                        {
                            Console.WriteLine("Your Input Was Not An Integer. Please Try Again.");
                            break;
                        }

                    case Board.eBoardSizeExceptions.OddBoardSize:
                        {
                            Console.WriteLine("The Size You Entered Was Odd. Please Enter Even Size.");
                            break;
                        }

                    case Board.eBoardSizeExceptions.OutOfBoarderLines:
                        {
                            Console.WriteLine("The Size You Entered Was Out Of Limits. Please Try Again.");
                            break;
                        }

                    case Board.eBoardSizeExceptions.NoException:
                        {
                            break;
                        }

                    default:
                        {
                            Console.WriteLine("Unnown Issue. Please Try Again.");
                            break;
                        }
                }
            }
            while (exception != Board.eBoardSizeExceptions.NoException);
            o_Rows = byte.Parse(rows);
            o_Cols = byte.Parse(cols);
        }
                
        private static void CongratsWinner(Player player1, Player player2)
        {
            switch(Player.GetWinningStatus(player1, player2))
            {
                case Player.eWinningStatus.Player1Won:
                    {
                        Console.WriteLine(string.Format("Good Job {0}! You Won!", player1.Name));
                        break;
                    }

                case Player.eWinningStatus.Player2Won:
                    {
                        Console.WriteLine(string.Format("Good Job {0}! You Won!", player2.Name));
                        break;
                    }

                case Player.eWinningStatus.Tie:
                    {
                        Console.WriteLine("It's A Tie!");
                        break;
                    }

                default:
                    {
                        break;
                    }
            }
        }

        public static void AskIfAnotherRound(ref bool o_ContinuePlay)
        {
            Player.eAnswer playerAnswer;
            do
            {
                Console.WriteLine("Do You Want To Have Another Game? Press Y/N");
                string answer = Console.ReadLine();
                playerAnswer = Player.CheckPlayerAnswer(answer);
                if(playerAnswer == Player.eAnswer.Undefined)
                {
                    Console.WriteLine("Invalid Input. Please Try Again.");
                }
            }
            while (playerAnswer == Player.eAnswer.Undefined);
            o_ContinuePlay = playerAnswer == Player.eAnswer.Yes ? true : false;
        }

        private static bool turn(Player io_PlayingPlayer, Player io_Player2, Board io_Board)
        {
            bool keepPlaying = false;
            if (io_PlayingPlayer.IsComputer)
            {
                computerTurn(io_PlayingPlayer, io_Player2, io_Board, ref keepPlaying);
            }
            else
            {
                humanTurn(io_PlayingPlayer, io_Player2, io_Board, ref keepPlaying);
            }

            return keepPlaying;
        }  

        private static void computerTurn(Player io_PlayingPlayer, Player io_Player2, Board io_Board, ref bool o_KeepPlaying)
        {
            Card.CardCoordinate cardsToChoose = io_PlayingPlayer.OptionalChoices.ChooseCardFromMemmory();
            if (cardsToChoose == null)
            {
                cardsToChoose = io_PlayingPlayer.OptionalChoices.ChooseRandomCards(io_Board);
            }

            revealCard(cardsToChoose.FirstCoordinate.Value, io_Board, io_PlayingPlayer, io_Player2);
            io_PlayingPlayer.OptionalChoices.Memmorize(io_Board.Matrix[cardsToChoose.FirstCoordinate.Value.X, cardsToChoose.FirstCoordinate.Value.Y], cardsToChoose.FirstCoordinate.Value);
            Thread.Sleep(k_RevealWaitingTime);
            revealCard(cardsToChoose.SecondCoordinate.Value, io_Board, io_PlayingPlayer, io_Player2);
            io_PlayingPlayer.OptionalChoices.Memmorize(io_Board.Matrix[cardsToChoose.SecondCoordinate.Value.X, cardsToChoose.SecondCoordinate.Value.Y], cardsToChoose.SecondCoordinate.Value);
            Thread.Sleep(k_RevealWaitingTime);
            if (io_Board.Matrix[cardsToChoose.FirstCoordinate.Value.X, cardsToChoose.FirstCoordinate.Value.Y].Value == io_Board.Matrix[cardsToChoose.SecondCoordinate.Value.X, cardsToChoose.SecondCoordinate.Value.Y].Value)
            {
                io_PlayingPlayer.AddPoint();                
                o_KeepPlaying = true;
                io_PlayingPlayer.OptionalChoices.Forget(io_Board.Matrix[cardsToChoose.FirstCoordinate.Value.X, cardsToChoose.FirstCoordinate.Value.Y]);
            }
            else
            {
                Screen.Clear();
                io_Board.Matrix[cardsToChoose.FirstCoordinate.Value.X, cardsToChoose.FirstCoordinate.Value.Y].IsVisable = false;
                io_Board.Matrix[cardsToChoose.SecondCoordinate.Value.X, cardsToChoose.SecondCoordinate.Value.Y].IsVisable = false;
                PrintBoard(io_Board, io_PlayingPlayer, io_Player2);
            }
        }

        private static void humanTurn(Player io_PlayingPlayer, Player io_Player2, Board io_Board, ref bool o_KeepPlaying)
        {
            Point playerCohiceCoordinate1 = chooseCell(io_PlayingPlayer, io_Player2, io_Board);
            Point playerCohiceCoordinate2 = chooseCell(io_PlayingPlayer, io_Player2, io_Board);            
            if (io_Board.Matrix[playerCohiceCoordinate1.X, playerCohiceCoordinate1.Y].Value == io_Board.Matrix[playerCohiceCoordinate2.X, playerCohiceCoordinate2.Y].Value)
            {
                io_PlayingPlayer.AddPoint();
                o_KeepPlaying = true;
                if (io_Player2.IsComputer)
                {
                    io_Player2.OptionalChoices.Forget(io_Board.Matrix[playerCohiceCoordinate1.X, playerCohiceCoordinate1.Y]);
                }
            }
            else
            {
                Thread.Sleep(k_RevealWaitingTime);
                Screen.Clear();
                io_Board.Matrix[playerCohiceCoordinate1.X, playerCohiceCoordinate1.Y].IsVisable = false;
                io_Board.Matrix[playerCohiceCoordinate2.X, playerCohiceCoordinate2.Y].IsVisable = false;
                PrintBoard(io_Board, io_PlayingPlayer, io_Player2);
            }
        }

        private static Point chooseCell(Player i_PlayingPlayer, Player io_Player2, Board io_Board)
        {
            Point playerCohiceCoordinate = getPlayerChoosement(io_Board);
            if (io_Player2.IsComputer)
            {
                io_Player2.OptionalChoices.Memmorize(io_Board.Matrix[playerCohiceCoordinate.X, playerCohiceCoordinate.Y], playerCohiceCoordinate);
            }

            revealCard(playerCohiceCoordinate, io_Board, i_PlayingPlayer, io_Player2);
            Thread.Sleep(k_RevealWaitingTime);

            return playerCohiceCoordinate;
        }

        private static void revealCard(Point i_Coordinate, Board io_Board, Player i_Player1, Player i_Player2)
        {
            Screen.Clear();
            io_Board.Matrix[i_Coordinate.X, i_Coordinate.Y].IsVisable = true;
            PrintBoard(io_Board, i_Player1, i_Player2);
        }

        private static Point getPlayerChoosement(Board board)
        {
            Player.eCellChoosementException choosementException;
            string playerCohice;
            do
            {
                Console.WriteLine("Please Select Cell To Reveal (For Example: B3) Or 'Q' To Exit.");
                playerCohice = Console.ReadLine();
                choosementException = Player.CheckPlayerCellChoise(playerCohice, board);
                switch (choosementException)
                {
                    case Player.eCellChoosementException.InvalidCell:
                        {
                            Console.WriteLine("Invalid Pattern. Please Try Again.");
                            break;
                        }

                    case Player.eCellChoosementException.OutOfBoardersCell:
                        {
                            Console.WriteLine("Your Choosement Is Out Of Boarders. Please Try Again.");
                            break;
                        }

                    case Player.eCellChoosementException.RevealedCell:
                        {
                            Console.WriteLine("The Cell You Choose Is Already Revealed. Please Try Again.");
                            break;
                        }

                    case Player.eCellChoosementException.Quit:
                        {
                            Console.WriteLine("Thank You For Playing!");
                            Environment.Exit(0);
                            break;
                        }

                    default:
                        {
                            break;
                        }
                }
            }
            while (choosementException != Player.eCellChoosementException.Valid);

            return Board.TranslatePlayerCellChoiceToCoordinate(playerCohice);
        }
    }
}