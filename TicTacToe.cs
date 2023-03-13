/*********************************
 * Fove C# Programming Test ------
 *********************************

 This test simulates a Tic Tac Toe (noughts and crosses) game server.

 The goal is to implement the following functions (each one documented further down):
   CreateGame()
   AddPlayer()
   MakeMove()

 Already-implemented tests call these functions and check that they work.
 Initially, all the tests fail. Your goal is to make them all pass.
 The tests check both valid and invalid inputs, ensuring that error handling is correct.

 Take as much time as you need (within reason), but in general this is expected to take about an hour.

 The rules of tic tac toe are:
   - Two players take turns. Each turn someone takes a spot on a 3x3 grid.
   - The first player to take 3 collinear adjacent spots (vertically, horizontally, or diagonally) wins.
   - The game can end in a draw if all 9 spots are taken and no one wins.

 Notes:
   - Use any C# language features that you like.
   - Use only the standard library. Don't add any other third party dependencies.
   - You will not be judged on the code style. Use any style that works for you.
   - Do not worry about threading, assume single threaded.
   - Design your implementation cleanly, as if someone else were to maintain this code after you implement it.
   - Comments are encouraged where useful, but generally things should be simple enough to not warrant many.

 How to build:
   This is distributed as a single file, so please create a project using you IDE of choice.
*/

using System;
using System.Collections.Generic;

namespace FoveProgrammingTest
{
    // Aliases - these are to give type safety and clarity to what IDs correspond to what
    // These double as result codes for simplicity of the API
    public enum GameId { };// Valid game ids are >= 0
    public enum PlayerId { }; // Valid player ids are >= 0

    class TicTacToeServer
    {
        // Result codes
        // These are listed in order: if multiple are applicable, the higher one takes precedence
        // For example, if the game & the player both don't exist, GAME_DOESNT_EXIST is expected (instead of PLAYER_DOESNT_EXIST)
        public const int GAME_DOESNT_EXIST = -2;
        public const int GAME_NOT_STARTED = -3;
        public const int GAME_ENDED = -4;
        public const int GAME_ONGOING = -5;
        public const int PLAYER_DOESNT_EXIST = -6;
        public const int WRONG_TURN = -7;
        public const int INVALID_LOCATION = -8;

        class Game
        {
            public int gameState;
            public PlayerId[] playerIDs;
            public List<Tuple<int, int>> moveList;

            public Game(int state, PlayerId[] ids, List<Tuple<int, int>> moves)
            {
                gameState = state;
                playerIDs = ids;
                moveList = moves;
            }
        }

        Dictionary<GameId, Game> gameList = new Dictionary<GameId, Game>();
        int game_IDS = 0;
        PlayerId[] newPlayerID = new PlayerId[2];

        // Creates a new game. Multiple games may be running simultaneously.
        //
        // Returns:
        //   - A valid unique ID for the new game
        // Errors:
        //   None
        public GameId CreateGame()
        {
            //Create Game
            game_IDS++;
            gameList.Add((GameId)game_IDS, new Game(GAME_NOT_STARTED, newPlayerID, new List<Tuple<int, int>>()));
            //Create Player
            return (GameId)game_IDS;
        }

        // Adds a player to a game that has been created, but not started.
        // This function starts the game automatically once the 2nd player has joined.
        // Once the game starts, the first player's turn begins (the one identified first call to AddPlayer)
        //
        // Returns:
        //   A valid ID for the new player, unique to this game, which may be any integer greater than zero
        //
        //   GAME_DOESNT_EXIST if the game id does not identify a valid game
        //   GAME_ENDED if the game has ended
        //   GAME_ONGOING if the game has already begun

        public PlayerId AddPlayer(GameId gameId)
        {
            // Check if the game exists
            if (gameId < 0 || gameId > (GameId)game_IDS)
            {
                return (PlayerId)GAME_DOESNT_EXIST;
            }
            //Add Player in Array
            if (gameList[gameId].gameState == GAME_ENDED)
            {
                return (PlayerId)GAME_ENDED;
            }

            Random randomPlayerId = new Random();
            PlayerId newPlayerID = (PlayerId)randomPlayerId.Next(1, 2);
            if (gameList[gameId].playerIDs[0] == 0)
            {
                gameList[gameId].playerIDs[0] = newPlayerID;
            }
            else
            {
                gameList[gameId].playerIDs[1] = newPlayerID;
            }
                //Return game ongoing
                if (gameList[gameId].playerIDs.Length == 2)
            {
                return (PlayerId)GAME_ONGOING;
            }

            return (PlayerId)newPlayerID;

            // IMPLEMENT ME!
            //return (PlayerId)GAME_DOESNT_EXIST;
        }   

        // Allows a player to make a move
        //
        // After each valid move, the turn switches to the other player.
        // If the move completes the game, the game status shall be considered ended.
        // No early detection of draws is done. Game must fully play out (9 moves) to reach a draw.
        //
        // Returns:
        //   GAME_ONGOING if no one has won yet
        //   The id of the current player if he won with this move (game is then ended)
        //   The id of the other player if the game ended in a draw (game is then ended)
        //
        //   GAME_DOESNT_EXIST if the game id does not identify a valid game
        //   GAME_NOT_STARTED if the game has not started
        //   GAME_ENDED if the game has already ended before this was called
        //   PLAYER_DOESNT_EXIST if the player id is not valid for this game
        //   WRONG_TURN if this is not player A turn
        //   INVALID_LOCATION if boardX or boardY is outside the range of [0, 2], or if that spot has been used already
        //
        public PlayerId MakeMove(GameId gameId, PlayerId playerId, int boardX, int boardY)
        {
            if (gameId < 0 || gameId > (GameId)game_IDS)
            {
                return (PlayerId)GAME_DOESNT_EXIST;
            }
            if (gameList[gameId].gameState == GAME_NOT_STARTED)
            {
                return (PlayerId)GAME_NOT_STARTED;
            }
            if (gameList[gameId].gameState == GAME_ENDED)
            {
                return (PlayerId)GAME_ENDED;
            }

            //if (playerId < 0 || playerId > 1)
            //{
            //    return (PlayerId)PLAYER_DOESNT_EXIST;
            //}

            if (playerId != gameList[gameId].playerIDs[gameList[gameId].moveList.Count % 2])
            {
                return (PlayerId)WRONG_TURN;
            }

            if (boardX < 0 || boardX > 2 || boardY < 0 || boardY > 2 || gameList[gameId].moveList.Contains(Tuple.Create(boardX, boardY)))
            {
                return (PlayerId)INVALID_LOCATION;
            }

            gameList[gameId].moveList.Add(Tuple.Create(boardX, boardY));

            //check winner
            if (gameList[gameId].moveList.Count >= 5)
            {
                //check rows
                for (int i = 0; i < 3; i++)
                {
                    if (gameList[gameId].moveList.Contains(Tuple.Create(i, 0)) &&
                        gameList[gameId].moveList.Contains(Tuple.Create(i, 1)) &&
                        gameList[gameId].moveList.Contains(Tuple.Create(i, 2)))
                    {
                        return gameList[gameId].playerIDs[gameList[gameId].moveList.Count % 2];
                    }
                }
                //check columns
                for (int i = 0; i < 3; i++)
                {
                    if (gameList[gameId].moveList.Contains(Tuple.Create(0, i)) &&
                        gameList[gameId].moveList.Contains(Tuple.Create(1, i)) &&
                        gameList[gameId].moveList.Contains(Tuple.Create(2, i)))
                    {
                        return gameList[gameId].playerIDs[gameList[gameId].moveList.Count % 2];
                    }
                }
                //check diagonals
                if (gameList[gameId].moveList.Contains(Tuple.Create(0, 0)) &&
                    gameList[gameId].moveList.Contains(Tuple.Create(1, 1)) &&
                    gameList[gameId].moveList.Contains(Tuple.Create(2, 2)))
                {
                    return gameList[gameId].playerIDs[gameList[gameId].moveList.Count % 2];
                }
                if (gameList[gameId].moveList.Contains(Tuple.Create(0, 2)) &&
                    gameList[gameId].moveList.Contains(Tuple.Create(1, 1)) &&
                    gameList[gameId].moveList.Contains(Tuple.Create(2, 0)))
                {
                    return gameList[gameId].playerIDs[gameList[gameId].moveList.Count % 2];
                }
            }

            // check if game has ended in a draw
            if (gameList[gameId].moveList.Count == 9)
            {
                return gameList[gameId].playerIDs[1];
            }

            return (PlayerId)GAME_ONGOING;
        }
    }   

    //////////////////////////////////////////////////////
    // Nothing below this point needs to be changed ------
    // Below is main() and tests -------------------------
    //////////////////////////////////////////////////////

    class TestSuite : TicTacToeServer
    {
        bool IsValidId(GameId id) { return id >= 0; }
        bool IsValidId(PlayerId id) { return id >= 0; }

        class GameInfo
        {
            public PlayerId[] players = new PlayerId[2] { (PlayerId)(-1), (PlayerId)(-1) };

            public GameInfo()
            {

            }
        };

        Dictionary<GameId, GameInfo> testGames = new Dictionary<GameId, GameInfo>();

        // Simple assert helper
        void Check(bool expr, string text)
        {
            if (!expr)
                throw new Exception(text);
        }

        GameId CreateTestGame() // Wrapper for createGame that does some basic checks
        {
            GameId gameId = CreateGame();
            Check(gameId >= 0, "Negative game id");
            Check(!testGames.ContainsKey(gameId), "Duplicate game id");
            testGames[gameId] = new GameInfo();
            return gameId;
        }
         
        GameId InvalidGameId() // Returns an unused game id
        {
            for (GameId i = (GameId)200; ; ++i)
                if (!testGames.ContainsKey(i))
                    return i;
        }

        PlayerId AddTestPlayer(GameId gameId)
        {
            PlayerId playerId = AddPlayer(gameId);
            if (playerId < 0)
                return playerId;

            GameInfo? game;
            if (!testGames.TryGetValue(gameId, out game))
                throw new Exception("Invalid gameID accepted to addPlayer " + gameId.ToString());

            if (!IsValidId(game.players[0]))
                game.players[0] = playerId;
            else if (game.players[0] == playerId)
                throw new Exception("Duplicate player id in game");
            else if (!IsValidId(game.players[1]))
                game.players[1] = playerId;
            else
                throw new Exception("Received a player id from a full game");

            return playerId;
        }

        void TestCreateGame()
        {
            for (int i = 0; i < 10; ++i)
                CreateTestGame();
        }

        void TestInvalidGameIds()
        {
            // Test some negative ids
            for (GameId i = (GameId)(-1); i > (GameId)(-10); --i)
                Check(AddTestPlayer(i) == (PlayerId)GAME_DOESNT_EXIST, "Negative game id should not be valid");

            // Test an invalid positive id
            Check(AddTestPlayer(InvalidGameId()) == (PlayerId)GAME_DOESNT_EXIST, "Invalid game id was accepted");

            // Test making a move on an invalid game
            Check(MakeMove(InvalidGameId(), 0, 0, 0) == (PlayerId)GAME_DOESNT_EXIST, "Wrong player move accepted");
        }

        void TestAddPlayer()
        {
            GameId gameId = CreateTestGame();

            // Make sure we can't make a move before adding players
            Check(MakeMove(gameId, 0, 0, 0) == (PlayerId)GAME_NOT_STARTED, "Made a move with no players added");

            // Add the first player
            PlayerId player1Id;
            Check(IsValidId(player1Id = AddTestPlayer(gameId)), "Negative game id should not be valid");

            // Make sure we can't make a move after adding the first player
            Check(MakeMove(gameId, player1Id, 0, 0) == (PlayerId)GAME_NOT_STARTED, "Made a move with only one player added");

            // Add the second player
            PlayerId player2Id;
            Check(IsValidId(player2Id = AddTestPlayer(gameId)), "Negative game id should not be valid");

            // Make sure we can't move with the 2nd player, but that the game has started
            Check(MakeMove(gameId, player2Id, 0, 0) == (PlayerId)WRONG_TURN, "Made a move with only one player added");

            // Make sure that we can't add more players after that
            for (int i = 0; i < 10; ++i)
                Check(AddTestPlayer(gameId) == (PlayerId)GAME_ONGOING, "Adding players after the second should return GAME_ONGOING");
        }

        // A series of full where play
        private static readonly Tuple<int, int>[][] wins =
             {
                 // xo_
                 // xo_
                 // x__
                 new Tuple<int, int>[]{
                     Tuple.Create(0, 0),
                     Tuple.Create(1, 0),
                     Tuple.Create(0, 1),
                     Tuple.Create(1, 1),
                     Tuple.Create(0, 2),
                 },

                 // xox
                 // oxo
                 // x__
                 new Tuple<int, int>[]{
                     Tuple.Create(2, 0),
                     Tuple.Create(2, 1),
                     Tuple.Create(0, 0),
                     Tuple.Create(1, 0),
                     Tuple.Create(0, 2),
                     Tuple.Create(0, 1),
                     Tuple.Create(1, 1),
                 },

                 // oxx
                 // ooo
                 // xx_
                 new Tuple<int, int>[]{
                     Tuple.Create(0, 2),
                     Tuple.Create(1, 1),
                     Tuple.Create(2, 0),
                     Tuple.Create(0, 1),
                     Tuple.Create(1, 2),
                     Tuple.Create(0, 0),
                     Tuple.Create(1, 0),
                     Tuple.Create(2, 1),
                 },

                 // ox_
                 // _o_
                 // xx_
                 new Tuple<int, int>[]{
                     Tuple.Create(1, 0),
                     Tuple.Create(1, 1),
                     Tuple.Create(0, 2),
                     Tuple.Create(0, 0),
                     Tuple.Create(1, 2),
                     Tuple.Create(2, 2),
                 },
         };

        private static readonly Tuple<int, int>[][] draws =
            {
                 // xxo
                 // oox
                 // xxo
                 new Tuple<int, int>[]{
                     Tuple.Create(1, 2),
                     Tuple.Create(1, 1),
                     Tuple.Create(0, 2),
                     Tuple.Create(2, 2),
                     Tuple.Create(0, 0),
                     Tuple.Create(0, 1),
                     Tuple.Create(2, 1),
                     Tuple.Create(2, 0),
                     Tuple.Create(1, 0),
                 },

                 // oxo
                 // xxo
                 // xox
                 new Tuple<int, int>[]{
                     Tuple.Create(1, 1),
                     Tuple.Create(0, 0),
                     Tuple.Create(0, 2),
                     Tuple.Create(2, 0),
                     Tuple.Create(1, 0),
                     Tuple.Create(1, 2),
                     Tuple.Create(0, 1),
                     Tuple.Create(2, 1),
                     Tuple.Create(2, 2),
                 }
         };

        void TestFullGame(Tuple<int, int>[] game, bool isDraw)
        {
            GameId gameId = CreateTestGame();

            PlayerId player1Id = AddTestPlayer(gameId);
            PlayerId b = AddTestPlayer(gameId);

            for (int i = 0; i < game.Length; ++i)
            {
                PlayerId currentPlayer = i % 2 == 0 ? player1Id : b;
                PlayerId otherPlayer = i % 2 == 0 ? b : player1Id;
                Tuple<int, int> nextMove = game[i];

                // Test that we can add players at no point during the game
                Check(AddTestPlayer(gameId) == (PlayerId)GAME_ONGOING, "Adding players during the game should return GAME_ONGOING");

                // Test that the wrong player can't move
                Check(MakeMove(gameId, otherPlayer, nextMove.Item1, nextMove.Item2) == (PlayerId)WRONG_TURN, "Wrong player move accepted");

                // Test that the right player can't move to any previously used spot
                for (int i2 = 0; i2 < i; ++i2)
                    Check(MakeMove(gameId, currentPlayer, game[i2].Item1, game[i2].Item2) == (PlayerId)INVALID_LOCATION, "Wrong player move accepted");

                // Test that the correct player can't move to a spot outside the board
                Check(MakeMove(gameId, currentPlayer, -1 - i, 0) == (PlayerId)INVALID_LOCATION, "Invalid board location accepted");
                Check(MakeMove(gameId, currentPlayer, 0, i + 3) == (PlayerId)INVALID_LOCATION, "Invalid board location accepted");

                // Make move
                if (i + 1 < game.Length)
                {
                    // Not-final move
                    Check(MakeMove(gameId, currentPlayer, nextMove.Item1, nextMove.Item2) == (PlayerId)GAME_ONGOING, "Valid move rejected");
                }
                else
                {
                    PlayerId expectedResult = otherPlayer; // Returning the other player means draw
                    string text = "Game should have been a draw";
                    if (!isDraw)
                    {
                        expectedResult = currentPlayer;
                        text = i % 2 == 0 ? "Player 1 should have won" : "Player 2 should have won";
                    }

                    // Final move of the game
                    Check(MakeMove(gameId, currentPlayer, nextMove.Item1, nextMove.Item2) == expectedResult, text);

                    // Test that after the game is complete, addTestPlayer and makeMove return GAME_ENDED
                    Check(AddTestPlayer(gameId) == (PlayerId)GAME_ENDED, "Adding a player after the game ended should return GAME_ENDED");
                    Check(MakeMove(gameId, i % 2 == 0 ? b : player1Id, game[i].Item1, game[i].Item2) == (PlayerId)GAME_ENDED, "Making a move after the game ended shoudl return GAME_ENDED");
                }
            }
        }

        static bool RunTest(string name, Action func)
        {
            try
            {
                func();
                Console.WriteLine("[PASSED] {0}", name);
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine("[FAILED] {0}: {1}", name, e.Message);
                return true;
            }
        }

        public static int Main(string[] args)
        {
            TestSuite tests = new TestSuite();

            bool failed = RunTest("testCreateGame", () => tests.TestCreateGame());
            failed = RunTest("testInvalidGameIds", () => tests.TestInvalidGameIds()) || failed;
            failed = RunTest("testAddPlayer", () => tests.TestAddPlayer()) || failed;

            foreach (var game in wins)
                failed = RunTest(game.Length % 2 == 0 ? "testPlayer1Win" : "testPlayer2Win", () => { tests.TestFullGame(game, false); }) || failed;

            foreach (var game in draws)
                failed = RunTest("testDraw", () => { tests.TestFullGame(game, true); }) || failed;

            return failed ? 1 : 0; // Return nonzero if at least one test failed
        }
    }
}
