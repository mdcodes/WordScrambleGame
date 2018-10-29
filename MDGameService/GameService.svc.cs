/*
 * Name: Michal Drahorat
 * Date: January 6th, 2017
 * Project Description: The interface methods for Assignment 3
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace MDGameService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    [ServiceBehavior]
    public class GameService : IGameService
    {
        // the maximum number of players allowed playing simultaneously
        private const int MAX_PLAYERS = 5;
        // the user hosting the game. If it’s null nobody is hosting the game.
        private static String userHostingTheGame = null;
        // the Word object that contains the scrambled and unscrambled words
        private static Word gameWord;
        // the list of players playing the game
        private static List<String> activePlayers = new List<string>();

        [OperationBehavior]
        public bool isGameBeingHosted()
        {
            // TO BE COMPLETED BY YOU: Add exception and program logic
            if (!String.IsNullOrEmpty(userHostingTheGame))
            {
                return true;
            }
            return false;
        }

        [OperationBehavior]
        public string hostGame(String playerName, string hostAddress, String wordToScramble)
        {
            // TO BE COMPLETED BY YOU: Add exception and program logic

            if (!String.IsNullOrEmpty(userHostingTheGame))
            {
                GameBeingHostedFaultClass gameBeingHostedEx = new GameBeingHostedFaultClass();
                gameBeingHostedEx.hostName = userHostingTheGame;
                gameBeingHostedEx.hostAddress = hostAddress;
                gameBeingHostedEx.reason = "Game is already being hosted.";

                throw new FaultException<GameBeingHostedFaultClass>(gameBeingHostedEx, new FaultReason(gameBeingHostedEx.reason));
            }
            else
            {
                BasicHttpBinding bind = new BasicHttpBinding();
                EndpointAddress address = new EndpointAddress(hostAddress);
                gameWord = new Word();
                activePlayers.Add(playerName);
                gameWord.unscrambledWord = wordToScramble;
                gameWord.scrambledWord = scrambleWord(wordToScramble);
                userHostingTheGame = playerName;
                return userHostingTheGame;
            }
        }

        [OperationBehavior]
        public Word join(string playerName)
        {
            // TO BE COMPLETED BY YOU: Add exception and program logic
            if (activePlayers.Count != 5)
            {
                activePlayers.Add(playerName);
            }
            else if (activePlayers.Count >= 5)
            {
                MaximumNumberOfPlayersReachedFaultClass maxPlayerEx = new MaximumNumberOfPlayersReachedFaultClass();
                maxPlayerEx.numOfPlayers = activePlayers.Count();
                maxPlayerEx.reason = "Max number of players has been reached.";
                throw new FaultException<MaximumNumberOfPlayersReachedFaultClass>(maxPlayerEx, new FaultReason(maxPlayerEx.reason));
            }
            else if(playerName == userHostingTheGame)
            {
                HostCannotJoinTheGameFaultClass hostCannotJoinEx = new HostCannotJoinTheGameFaultClass();
                hostCannotJoinEx.reason = "You are the host, you cannot join as a player.";

                throw new FaultException<HostCannotJoinTheGameFaultClass>(hostCannotJoinEx, new FaultReason(hostCannotJoinEx.reason));
            }
            else if (String.IsNullOrEmpty(userHostingTheGame))
            {
                GameIsNotBeingHostedFaultClass notHostedEx = new GameIsNotBeingHostedFaultClass();
                notHostedEx.reason = "Game is not currently being hosted";

                throw new FaultException<GameIsNotBeingHostedFaultClass>(notHostedEx, new FaultReason(notHostedEx.reason));
            }
            return gameWord;
        }

        [OperationBehavior]
        public bool guessWord(string playerName, string guessedWord, string unscrambledWord)
        {
            // TO BE COMPLETED BY YOU: Add exception and program logic
            if (!activePlayers.Exists(x => x == playerName))
            {
                PlayerNotPlayingTheGameFaultClass playerNotPlayingEx = new PlayerNotPlayingTheGameFaultClass();
                playerNotPlayingEx.playerName = playerName;
                playerNotPlayingEx.reason = "Player is not playing the game";
            }
            else if (guessedWord.Equals(unscrambledWord))
            {
                return true;
            }
            return false;
        }

        [OperationBehavior]
        // Utility function to scramble a word
        public string scrambleWord(string word)
        {
            char[] chars = word.ToArray();
            Random r = new Random(2011);
            for (int i = 0; i < chars.Length; i++)
            {
                int randomIndex = r.Next(0, chars.Length);
                char temp = chars[randomIndex];
                chars[randomIndex] = chars[i];
                chars[i] = temp;
            }
            return new string(chars);
        }
    }
}
