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

    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IGameService
    {
        // Returns true if the game is already being hosted or false otherwise 
        [OperationContract]
        [FaultContract(typeof(GameIsNotBeingHostedFaultClass))]
        bool isGameBeingHosted();

        // User ‘userName’ tries to host the game with word ‘wordToScramble’
        // The function returns the name of the person hosting the game 
        // Exception: game is already being hosted by someone else
        [OperationContract]
        [FaultContract(typeof(GameBeingHostedFaultClass))]
        string hostGame(String userName, string hostAddress, String wordToScramble);

        // Player ‘playerName’ tries to join the game
        // The function returns a Word object containing the host’s (un)scrambled words
        // Exception: maximum number of players reached
        // Exception: host cannot join the game
        // Exception: nobody is hosting the game
        [OperationContract]
        [FaultContract(typeof(MaximumNumberOfPlayersReachedFaultClass))]
        [FaultContract(typeof(HostCannotJoinTheGameFaultClass))]
        Word join(string playerName);

        // Player ‘playerName’ guesses word ‘guessedWord’ compared with word ‘unscrambledWord’
        // Returns true if ‘guessedWord’ is identical to ‘unscrambledWord’ or false otherwise
        // Exception: user is not playing the game 
        [OperationContract]
        bool guessWord(string playerName, string guessedWord, string unscrambledWord);

        [OperationContract]
        string scrambleWord(string word);
    }

    /// <summary>
    /// The class for storing the word currently being used by the game
    /// </summary>
    [DataContract]
    public class Word
    {
        [DataMember]
        public string unscrambledWord; // word typed by the game’s host
        [DataMember]
        public string scrambledWord;
    }

    /// <summary>
    /// The definition for the GameBeingHosted fault exception
    /// </summary>
    [DataContract]
    public class GameBeingHostedFaultClass
    {
        [DataMember]
        public String hostName;

        [DataMember]
        public String hostAddress;

        [DataMember]
        public String reason;

    }


    /// <summary>
    /// The definition for the MaximumNumberOfPlayersReached fault exception
    /// </summary>
    [DataContract]
    public class MaximumNumberOfPlayersReachedFaultClass
    {
        [DataMember]
        public int numOfPlayers;

        [DataMember]
        public String reason;

    }

    /// <summary>
    /// The definition for the HostCannotJoinTheGame fault exception
    /// </summary>
    [DataContract]
    public class HostCannotJoinTheGameFaultClass
    {
        [DataMember]
        public String reason;
    }

    /// <summary>
    /// The definition for the GameIsNotBeingHosted fault exception
    /// </summary>
    [DataContract]
    public class GameIsNotBeingHostedFaultClass
    {
        [DataMember]
        public String reason;

    }

    /// <summary>
    /// The definition for the PlayerNotPlayingTheGame fault exception
    /// </summary>
    [DataContract]
    public class PlayerNotPlayingTheGameFaultClass
    {
        [DataMember]
        public String playerName;

        [DataMember]
        public String reason;

    }

}
