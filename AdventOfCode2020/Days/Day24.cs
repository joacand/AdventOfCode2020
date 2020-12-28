using AdventOfCode2020.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020.Days
{
    public class Day24 : IDay
    {
        public string Solve()
        {
            var inputRaw = EmbeddedResource.ReadInput("InputDay24.txt");

            var lines = inputRaw.Split(new[] { "\r\n" }, StringSplitOptions.None).ToList();

            var part1result = SolvePart1(lines.ToList());
            var part2result = SolvePart2(lines.ToList());

            return $"Part 1: {part1result}, Part 2: {part2result}";
        }

        private static string SolvePart1(List<string> input)
        {
            Game game = new();
            game.FlipTiles(input);

            return game.CountBlackTiles().ToString();
        }

        private static string SolvePart2(List<string> input)
        {
            Game game = new();
            game.FlipTiles(input);
            game.PerformArtExhibit(100);

            return game.CountBlackTiles().ToString();
        }

        private class Game
        {
            private List<Tile> Tiles { get; set; } = new();
            private Dictionary<Tile, bool> TilesToBlackSideUpMap { get; set; } = new();

            private readonly Dictionary<Tile, int> blackTileCounterMemo = new();

            public Game()
            {
                var referenceTile = new Tile();
                Tiles.Add(referenceTile);
            }

            public int CountBlackTiles()
            {
                return Tiles.Count(x => x.IsBlackTile);
            }

            public void PerformArtExhibit(int days)
            {
                Console.WriteLine($"Day 0: {CountBlackTiles()}");
                for (var i = 0; i < days; i++)
                {
                    ExecuteRules();
                    Console.WriteLine($"Day {i + 1}: {CountBlackTiles()} - Count: {Tiles.Count}");
                }
            }

            public void FlipTiles(List<string> tileMovements)
            {
                foreach (var tileMovement in tileMovements)
                {
                    FlipTile(tileMovement);
                }
            }

            private void FlipTile(string movements)
            {
                var lexer = new Lexer();
                var tokens = lexer.GetTokens(movements);
                Tile tile = new();

                foreach (var direction in tokens)
                {
                    switch (direction)
                    {
                        case Direction.E: tile.X += 1; tile.Y -= 1; break;
                        case Direction.NE: tile.X += 1; tile.Z -= 1; break;
                        case Direction.NW: tile.Y += 1; tile.Z -= 1; break;
                        case Direction.W: tile.Y += 1; tile.X -= 1; break;
                        case Direction.SW: tile.Z += 1; tile.X -= 1; break;
                        case Direction.SE: tile.Z += 1; tile.Y -= 1; break;
                    }
                }

                var ind = Tiles.IndexOf(tile);

                if (ind == -1)
                {
                    Tiles.Add(tile);
                    ind = Tiles.IndexOf(tile);
                }
                else
                {
                    tile = Tiles[ind];
                }
                tile.Flip();
                Tiles[ind] = tile;
                if (tile.IsBlackTile)
                {
                    TilesToBlackSideUpMap[tile] = true;
                }
                else
                {
                    TilesToBlackSideUpMap[tile] = false;
                }
            }

            private void ExecuteRules()
            {
                blackTileCounterMemo.Clear();

                for (int i = 0; i < Tiles.Count; i++)
                {
                    AddAdjacentTiles(Tiles[i], Tiles);
                }

                blackTileCounterMemo.Clear();

                List<int> tileIndicesToFlip = new();

                for (var i = 0; i < Tiles.Count; i++)
                {
                    var tile = Tiles[i];
                    var blackAdjacentTiles = CountBlackAdjacentTiles(tile);

                    if (tile.IsBlackTile)
                    {
                        if (blackAdjacentTiles == 0 || blackAdjacentTiles > 2)
                        {
                            tileIndicesToFlip.Add(i);
                        }
                    }
                    else
                    {
                        if (blackAdjacentTiles == 2)
                        {
                            tileIndicesToFlip.Add(i);
                        }
                    }
                }

                foreach (var ttc in tileIndicesToFlip)
                {
                    Tiles[ttc] = Tiles[ttc].Flip();
                    if (Tiles[ttc].IsBlackTile)
                    {
                        TilesToBlackSideUpMap[Tiles[ttc]] = true;
                    }
                    else
                    {
                        TilesToBlackSideUpMap[Tiles[ttc]] = false;
                    }
                }
            }

            private readonly (int, int, int)[] neighborCoords =
                new (int, int, int)[6] { (1, -1, 0), (0, -1, 1), (-1, 0, 1), (-1, 1, 0), (0, 1, -1), (1, 0, -1) };

            private int CountBlackAdjacentTiles(Tile tile)
            {
                if (blackTileCounterMemo.ContainsKey(tile))
                {
                    return blackTileCounterMemo[tile];
                }

                var adjacent = 0;
                foreach (var neighborCoord in neighborCoords)
                {
                    var newTile = new Tile { X = tile.X - neighborCoord.Item1, Y = tile.Y - neighborCoord.Item2, Z = tile.Z - neighborCoord.Item3 };

                    if (TilesToBlackSideUpMap.ContainsKey(newTile) && TilesToBlackSideUpMap[newTile])
                    {
                        adjacent++;
                    }
                }
                blackTileCounterMemo[tile] = adjacent;
                return adjacent;
            }

            private void AddAdjacentTiles(Tile tile, List<Tile> tiles)
            {
                foreach (var neighborCoord in neighborCoords)
                {
                    var newTile = new Tile { X = tile.X - neighborCoord.Item1, Y = tile.Y - neighborCoord.Item2, Z = tile.Z - neighborCoord.Item3 };

                    var c = CountBlackAdjacentTiles(newTile);

                    if (!tiles.Contains(newTile) && c > 0)
                    {
                        blackTileCounterMemo[tile] = c;
                        tiles.Add(newTile);
                    }
                }
            }
        }

        private struct Tile : IEquatable<Tile>
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Z { get; set; }
            public bool IsBlackTile { get; set; }

            public Tile Flip()
            {
                IsBlackTile = !IsBlackTile;
                return this;
            }

            public override string ToString()
            {
                return $"X:{X} Y:{Y} Z{Z} - Black tile:{IsBlackTile}";
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(X, Y, Z);
            }

            public override bool Equals(object obj)
            {
                return obj is Tile tile && Equals(tile);
            }

            public bool Equals(Tile other)
            {
                return X == other.X &&
                       Y == other.Y &&
                       Z == other.Z;
            }
        }

        private enum Direction
        {
            E,
            W,
            NE,
            SE,
            SW,
            NW
        }

        private record Token(Direction Direction);

        private class Lexer
        {
            public List<Direction> GetTokens(string input)
            {
                List<Direction> directions = new();
                var currentIndex = 0;

                while (currentIndex < input.Length)
                {
                    directions.Add(GetToken(input, ref currentIndex));
                    currentIndex++;
                }

                return directions;
            }

            private static Direction GetToken(string input, ref int currentIndex)
            {
                var c = input[currentIndex];
                switch (c)
                {
                    case 'n':
                        {
                            var peekIndex = currentIndex + 1;

                            if (peekIndex < input.Length)
                            {
                                var csnd = input[peekIndex];
                                switch (csnd)
                                {
                                    case 'e':
                                        {
                                            currentIndex = peekIndex;
                                            return Direction.NE;
                                        }
                                    case 'w':
                                        {
                                            currentIndex = peekIndex;
                                            return Direction.NW;
                                        }
                                }
                            }
                            throw new Exception();
                        }
                    case 's':
                        {
                            var peekIndex = currentIndex + 1;

                            if (peekIndex < input.Length)
                            {
                                var csnd = input[peekIndex];
                                switch (csnd)
                                {
                                    case 'e':
                                        {
                                            currentIndex = peekIndex;
                                            return Direction.SE;
                                        }
                                    case 'w':
                                        {
                                            currentIndex = peekIndex;
                                            return Direction.SW;
                                        }
                                }
                            }
                            throw new Exception();
                        }
                    case 'e':
                        {
                            return Direction.E;
                        }
                    case 'w':
                        {
                            return Direction.W;
                        }
                    default:
                        throw new Exception();
                }
            }
        }
    }
}
