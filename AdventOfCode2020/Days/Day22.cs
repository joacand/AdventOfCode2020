using AdventOfCode2020.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020.Days
{
    public class Day22 : IDay
    {
        public string Solve()
        {
            var inputRaw = EmbeddedResource.ReadInput("InputDay22.txt");

            var lines = inputRaw.Split(new[] { "\r\n" }, StringSplitOptions.None).ToList();

            var part1result = SolvePart1(lines.ToList());
            var part2result = SolvePart2(lines.ToList());

            return $"Part 1: {part1result}, Part 2: {part2result}";
        }

        private static string SolvePart1(List<string> input)
        {
            (var player1, var player2) = ParseInput(input);

            return Game.PlayCombatGame(player1, player2).ToString();
        }

        private static string SolvePart2(List<string> input)
        {
            (var player1, var player2) = ParseInput(input);
            (var _, var score) = Game.PlayRecursiveGame(player1, player2, Game.NewPreviousDeckHashes);

            return score.ToString();
        }

        private class Game
        {
            // Previous deck-holder initialized with player 1 and 2
            public static Dictionary<int, List<int>> NewPreviousDeckHashes => new() { { 1, new() }, { 2, new() } };

            public static long PlayCombatGame(Queue<int> player1, Queue<int> player2)
            {
                while (player1.Count > 0 && player2.Count > 0)
                {
                    var p1card = player1.Dequeue();
                    var p2card = player2.Dequeue();

                    if (p1card > p2card)
                    {
                        player1.Enqueue(p1card);
                        player1.Enqueue(p2card);
                    }
                    else
                    {
                        player2.Enqueue(p2card);
                        player2.Enqueue(p1card);
                    }
                }

                var winner = player1.Count > 0 ? player1 : player2;

                return CalculateScore(winner);
            }

            public static (int, long) PlayRecursiveGame(Queue<int> player1, Queue<int> player2, IDictionary<int, List<int>> previousRoundDecks)
            {
                while (player1.Count > 0 && player2.Count > 0)
                {
                    // Recursion rule
                    if (DeckHasBeenPlayedPreviously(1, player1.GetHash(), previousRoundDecks) ||
                        DeckHasBeenPlayedPreviously(2, player2.GetHash(), previousRoundDecks))
                    {
                        return (1, CalculateScore(player1));
                    }

                    var p1card = player1.Dequeue();
                    var p2card = player2.Dequeue();

                    // Recursion sub-game
                    if (HasCardsRemainingForSubGame(p1card, player1) &&
                        HasCardsRemainingForSubGame(p2card, player2))
                    {
                        var subGameDeckP1 = new Queue<int>(player1.Take(p1card));
                        var subGameDeckP2 = new Queue<int>(player2.Take(p2card));

                        (var winningPlayer, var _) = PlayRecursiveGame(subGameDeckP1, subGameDeckP2, NewPreviousDeckHashes);

                        if (winningPlayer == 1)
                        {
                            player1.Enqueue(p1card);
                            player1.Enqueue(p2card);
                        }
                        else
                        {
                            player2.Enqueue(p2card);
                            player2.Enqueue(p1card);
                        }
                    } // Else normal high-card wins rule
                    else
                    {
                        if (p1card > p2card)
                        {
                            player1.Enqueue(p1card);
                            player1.Enqueue(p2card);
                        }
                        else
                        {
                            player2.Enqueue(p2card);
                            player2.Enqueue(p1card);
                        }
                    }
                }

                return (
                    player1.Count > 0 ? 1 : 2,
                    CalculateScore(player1.Count > 0 ? player1 : player2));
            }

            private static bool DeckHasBeenPlayedPreviously(int player, int deckHash, IDictionary<int, List<int>> previousRoundDecks)
            {
                if (previousRoundDecks[player].Any(hash => hash == deckHash))
                {
                    return true;
                }
                previousRoundDecks[player].Add(deckHash);
                return false;
            }

            private static bool HasCardsRemainingForSubGame(int cardValue, Queue<int> deck)
            {
                return deck.Count >= cardValue;
            }

            private static long CalculateScore(Queue<int> winnerDeck)
            {
                var winnerList = new List<int>(winnerDeck);
                long score = 0;
                for (var i = 0; i < winnerList.Count; i++)
                {
                    score += winnerList[i] * (winnerList.Count - i);
                }
                return score;
            }
        }

        private static (Queue<int>, Queue<int>) ParseInput(List<string> input)
        {
            List<int> player1 = new();
            List<int> player2 = new();

            bool isPlayer1 = true;
            foreach (var line in input)
            {
                if (line.Contains("Player")) continue;

                if (string.IsNullOrWhiteSpace(line))
                {
                    isPlayer1 = false;
                    continue;
                }

                if (isPlayer1)
                {
                    player1.Add(int.Parse(line));
                }
                else
                {
                    player2.Add(int.Parse(line));
                }
            }

            return (new Queue<int>(player1), new Queue<int>(player2));
        }
    }

    public static class Day22Extensions
    {
        public static int GetHash<T>(this IEnumerable<T> src)
        {
            var hash = new HashCode();
            foreach (var element in src)
            {
                hash.Add(element);
            }
            return hash.ToHashCode();
        }
    }
}
