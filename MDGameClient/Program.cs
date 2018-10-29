/*
 * Name: Michal Drahorat
 * Date: January 6th, 2017
 * Project Description: The client for Assignment 3
 */

using MDGameClient.GameServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MDGameClient
{
    class Program
    {
        static void Main(string[] args)
        {
            GameServiceClient proxy = new GameServiceClient();

            bool canPlayGame = true;

            Console.WriteLine("Player's name?");
            String playerName = Console.ReadLine();

            if (!proxy.isGameBeingHosted())
            {
                try
                {
                    Console.WriteLine("Welcome " + playerName +
                       "! Do you want to host the game?");
                    if (Console.ReadLine().CompareTo("yes") == 0)
                    {
                        Console.WriteLine("Type the word to scramble.");
                        string inputWord = Console.ReadLine();
                        string scrambledWord = proxy.scrambleWord(inputWord);
                        proxy.hostGame(playerName, "https://localhost:8080", inputWord);
                        canPlayGame = false;
                        Console.WriteLine("You're hosting the game with word '" + inputWord + "' scrambled as '" + scrambledWord + "'");
                        Console.ReadKey();
                    }
                }
                catch (FaultException<GameBeingHostedFaultClass> e)
                {
                    Console.WriteLine("Exception thrown: {0}",e.Reason);
                    Console.WriteLine("HostName= {0}, HostAddress= {1}, Reason= {2}", e.Detail.hostName, e.Detail.hostAddress, e.Detail.reason);
                }
            }
            if (canPlayGame)
            {
                Console.WriteLine("Do you want to play the game?");
                if (Console.ReadLine().CompareTo("yes") == 0)
                {
                    try
                    {
                        Word gameWords = proxy.join(playerName);
                        Console.WriteLine("Can you unscramble this word? => " + gameWords.scrambledWord);
                        String guessedWord;
                        bool gameOver = false;
                        while (!gameOver)
                        {
                            guessedWord = Console.ReadLine();
                            gameOver = proxy.guessWord(playerName, guessedWord, gameWords.unscrambledWord);
                            if (!gameOver)
                            {
                                Console.WriteLine("Nope, try again...");
                            }
                        }
                        Console.WriteLine("You WON!!!");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadLine();
                        proxy.Close();
                    }
                    catch (FaultException<MaximumNumberOfPlayersReachedFaultClass> e)
                    {
                        Console.WriteLine("Exception thrown: {0}", e.Reason);
                        Console.WriteLine("NumOfPlayers= {0}, Reason= {1}", e.Detail.numOfPlayers, e.Detail.reason);
                    }
                    catch(FaultException<HostCannotJoinTheGameFaultClass> e)
                    {
                        Console.WriteLine("Exception thrown: {0}", e.Reason);
                        Console.WriteLine("Reason= {1}", e.Detail.reason);
                    }
                }
            }
        }
    }
}
