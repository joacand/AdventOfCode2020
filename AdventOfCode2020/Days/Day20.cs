using AdventOfCode2020.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020.Days
{
    public class Day20 : IDay
    {
        public string Solve()
        {
            var inputRaw = EmbeddedResource.ReadInput("InputDay20.txt");

            var lines = inputRaw.Split(new[] { "\r\n" }, StringSplitOptions.None).ToList();

            var part1result = "";// SolvePart1(lines.ToList());
            var part2result = SolvePart2(lines.ToList());

            return $"Part 1: {part1result}, Part 2: {part2result}";
        }

        private static string SolvePart1(List<string> input)
        {
            var tiles = ParseInput(input);

            var matches = FindMatch(tiles);

            var corners = matches.Where(x => x.Item2 == 2).Select(x => x.Item1.Id);

            return corners.Product().ToString();
        }

        private static string SolvePart2(List<string> input)
        {
            var tiles = ParseInput(input);

            var matches = FindMatch(tiles);

            foreach (var m in matches)
            {
             m.Item1.CleanUpNeighbors();
            }

            foreach (var tile in tiles)
            {
                Console.WriteLine($"{tile.Id} neighbors: {tile.Neighbors.Count} - neighbordirections: {string.Join(",", tile.Neighbors.Select(t => $"{t.Key}:{t.Value.Item1.Id}:{t.Value.Item2}").ToList())}");
            }

            var corners = matches.Where(x => x.Item2 == 2).Select(x => x.Item1.Id);

            var topleftcorner = matches.Where(x => x.Item1.Neighbors.Count == 2 && x.Item1.Neighbors.Any(y => y.Key == Dir.Bottom && x.Item1.Neighbors.Any(y => y.Key == Dir.Right))).Select(x => x.Item1).ToList();

            var toprightcorner = matches.Where(x => x.Item1.Neighbors.Count == 2 && x.Item1.Neighbors.Any(y => y.Key == Dir.Bottom && x.Item1.Neighbors.Any(y => y.Key == Dir.Left))).Select(x => x.Item1).ToList();

            var botleftcorner = matches.Where(x => x.Item1.Neighbors.Count == 2 && x.Item1.Neighbors.Any(y => y.Key == Dir.Top && x.Item1.Neighbors.Any(y => y.Key == Dir.Right))).Select(x => x.Item1).ToList();

            var botrightcorner = matches.Where(x => x.Item1.Neighbors.Count == 2 && x.Item1.Neighbors.Any(y => y.Key == Dir.Top && x.Item1.Neighbors.Any(y => y.Key == Dir.Left))).Select(x => x.Item1).ToList();

            return corners.Product().ToString();
        }

        private static List<(Tile, int)> FindMatch(List<Tile> tiles)
        {
            List<(Tile, int)> test = new();

            for (var i = 0; i < tiles.Count; i++)
            {
                var firstTile = tiles[i];

                var maxmatch = 0;

                for (var firstTilePerms = 0; firstTilePerms < firstTile.Permutations.Count; firstTilePerms++)
                {
                    if (firstTile.HasLockedPerm && firstTile.LockedPerm != firstTilePerms) continue;
                    var matchesToOtherTiles = 0;

                    var firstNeighbors = new List<(Tile secondTile, int dir, Dictionary<int, Dir> dirMap, int permIndex)>();
                    var secondNeighbors = new List<(Tile secondTile, int dir, Dictionary<int, Dir> dirMap, int permIndex)>();

                    // other tiles..
                    for (var j = 0; j < tiles.Count; j++)
                    {
                        if (j == i) { continue; }
                        var secondTile = tiles[j];


                        for (var secondTilePerms = 0; secondTilePerms < secondTile.Permutations.Count; secondTilePerms++)
                        {
                            if (secondTile.HasLockedPerm && secondTile.LockedPerm != secondTilePerms) continue;

                            var firstTilePerm1 = firstTile.Permutations[firstTilePerms];
                            var secondTilePerm2 = secondTile.Permutations[secondTilePerms];

                            var firstTilePerm = firstTilePerm1.Item1;
                            var secondTilePerm = secondTilePerm2.Item1;

                            (bool result, int firstTileI, int secondTileI) = Match(firstTilePerm, secondTilePerm);
                            if (result)
                            {
                                matchesToOtherTiles++;
                                firstNeighbors.Add((secondTile, firstTileI, firstTilePerm1.Item2, secondTilePerms));
                                secondNeighbors.Add((firstTile, secondTileI, secondTilePerm2.Item2, firstTilePerms));
                                break;
                            }
                        }

                        if (matchesToOtherTiles == 4)
                        {
                            break;
                        }
                    }

                    if (matchesToOtherTiles > maxmatch)
                    {
                        maxmatch = matchesToOtherTiles;

                        for (var x = 0; x < firstNeighbors.Count; x++)
                        {
                            var n = firstNeighbors[x];
                            var nn = secondNeighbors[x];
                            firstTile.AddNeighbor(n.secondTile, n.dir, n.dirMap, n.permIndex);
                            n.secondTile.AddNeighbor(nn.secondTile, nn.dir, nn.dirMap, nn.permIndex);
                        }
                    }


                    if (matchesToOtherTiles == 4)
                    {
                        foreach (var n in firstNeighbors)
                        {
                            n.secondTile.LockPerm(n.permIndex);
                        }
                        break;
                    }
                }

                test.Add((firstTile, maxmatch));
            }

            return test;
        }

        private static (bool, int, int) Match(char[,] tile1, char[,] tile2)
        {
            var tile1borders = Tile.GetBorders(tile1);
            var tile2borders = Tile.GetBorders(tile2);

            for (var i = 0; i < tile1borders.Length; i++)
            {
                var compareborderindex = (i + 2) % tile1borders.Length;
                if (tile1borders[i].Equals(tile2borders[compareborderindex])) return (true, i, compareborderindex);
            }
            return (false, 0, 0);
        }

        private static List<Tile> ParseInput(List<string> input)
        {
            List<Tile> tiles = new();

            List<string> processingTile = new();
            foreach (var line in input)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    tiles.Add(new Tile(processingTile));
                    processingTile.Clear();
                    continue;
                }
                processingTile.Add(line);
            }
            tiles.Add(new Tile(processingTile));

            return tiles;
        }

        private enum Dir
        {
            Top, Right, Bottom, Left
        }

        private class Tile
        {
            public int Id { get; set; }
            public char[,] Data { get; set; }
            public char[,] DataWithoutBorders { get; set; }
            public List<(char[,], Dictionary<int, Dir>)> Permutations { get; set; }
            public int LockedPerm { get; set; } = -1;
            public bool HasLockedPerm => LockedPerm != -1;
            Dictionary<int, Dir> DefaultDirs = new Dictionary<int, Dir> { { 0, Dir.Top }, { 1, Dir.Right }, { 2, Dir.Bottom }, { 3, Dir.Left } };

            public Dictionary<Dir, (Tile, int)> Neighbors { get; set; } = new();

            public Tile(List<string> rawData)
            {
                Data = ParseData(rawData);
                Permutations = GetPermutations();
            }

            public void AddNeighbor(Tile secondTile, int dir, Dictionary<int, Dir> dirMap, int permIndex)
            {
                Neighbors[DefaultDirs[dir]] = (secondTile, permIndex);
            }

            public void LockPerm(int firstTilePerms)
            {
                LockedPerm = firstTilePerms;
            }

            public void CleanUpNeighbors()
            {
                List<Dir> dirsToRemove = new();
                foreach (var n in Neighbors)
                {
                    var val = n.Value;
                    if (val.Item1.LockedPerm != -1 && val.Item1.LockedPerm != n.Value.Item2)
                    {
                        dirsToRemove.Add(n.Key);
                    }
                }
                foreach (var rm in dirsToRemove)
                {
                    Neighbors.Remove(rm);
                }
            }

            private List<(char[,], Dictionary<int, Dir>)> GetPermutations()
            {
                List<(char[,], Dictionary<int, Dir>)> permutations = new();
                Dictionary<int, Dir> dirs = new Dictionary<int, Dir> { { 0, Dir.Top }, { 1, Dir.Right }, { 2, Dir.Bottom }, { 3, Dir.Left } };

                var rotated = new char[10, 10];
                for (int i = 0; i < Data.GetLength(0); i++)
                {

                    for (int j = 0; j < Data.GetLength(1); j++)
                    {
                        rotated[i, j] = Data[i, j];
                    }
                }
                for (int rotation = 0; rotation < 4; rotation++)
                {
                    rotated = Rotate90(rotated);
                    var tmp = dirs[0];
                    var tmp2 = dirs[1];
                    var tmp3 = dirs[2];
                    dirs[0] = dirs[3];
                    dirs[1] = tmp;
                    dirs[2] = tmp2;
                    dirs[3] = tmp3;

                    for (int j = 0; j < 4; j++)
                    {
                        var flipped = Flip(rotated, j, dirs, out var dirRo);

                        permutations.Add((flipped, new Dictionary<int, Dir>(dirRo)));
                    }
                }

                return permutations;
            }

            public void RemoveBorders(int permToUse)
            {
                var newData = new char[8, 8];

                var dataToUse = Permutations[permToUse].Item1;

                for (int r = 1; r < dataToUse.GetLength(0) - 1; r++)
                {
                    for (int c = 1; c < dataToUse.GetLength(1) - 1; c++)
                    {
                        newData[r - 1, c - 1] = dataToUse[r, c];
                    }
                }

                DataWithoutBorders = newData;
            }

            // Top, right, bottom, left
            public static string[] GetBorders(char[,] data)
            {
                var borders = new List<string>();
                List<char> r = new();

                r.Clear();
                for (int i = 0; i < data.GetLength(0); i++)
                {
                    r.Add(data[i, 0]);
                }
                borders.Add(new string(r.ToArray()));

                r.Clear();
                for (int i = 0; i < data.GetLength(1); i++)
                {
                    r.Add(data[data.GetLength(0) - 1, i]);
                }
                borders.Add(new string(r.ToArray()));

                r.Clear();
                for (int i = 0; i < data.GetLength(0); i++)
                {
                    r.Add(data[i, data.GetLength(1) - 1]);
                }
                borders.Add(new string(r.ToArray()));

                r.Clear();
                for (int i = 0; i < data.GetLength(1); i++)
                {
                    r.Add(data[0, i]);
                }
                borders.Add(new string(r.ToArray()));

                return borders.ToArray();
            }

            public static char[,] Rotate90(char[,] borderCodes)
            {
                // Transpose and reverse each row
                var transposed = borderCodes.Transpose();
                var rotated = transposed.ReverseRows();
                return rotated;
            }

            private static char[,] Flip(char[,] flipArray, int dir, Dictionary<int, Dir> dirs, out Dictionary<int, Dir> dirs1)
            {
                dirs1 = new Dictionary<int, Dir>(dirs);
                switch (dir)
                {
                    case 0:
                        {
                            var a = flipArray.FlipVertical();
                            var tmp = dirs1[1];
                            dirs1[1] = dirs1[3];
                            dirs1[3] = tmp;

                            return a;
                        }
                    case 1:
                        {
                            var a = flipArray.FlipHorizontal();
                            var tmp = dirs1[0];
                            dirs1[0] = dirs1[2];
                            dirs1[2] = tmp;

                            return a;
                        }
                    case 2:
                        {
                            var a = flipArray.FlipHorizontal().FlipVertical();
                            var tmp = dirs1[0];
                            dirs1[0] = dirs1[2];
                            dirs1[2] = tmp;

                            tmp = dirs1[1];
                            dirs1[1] = dirs1[3];
                            dirs1[3] = tmp;

                            return a;
                        }
                    case 3:
                        {
                            return flipArray;
                        }
                    default:
                        throw new Exception();
                }
            }

            private char[,] ParseData(List<string> rawData)
            {
                Id = int.Parse(new string(rawData.First().Split(" ")[1].SkipLast(1).ToArray()));
                var data = new char[10, 10];

                var row = 0;
                foreach (var line in rawData.Skip(1))
                {
                    var column = 0;
                    foreach (var c in line)
                    {
                        data[row, column++] = c;
                    }
                    row++;
                }

                return data;
            }
        }
    }

    public static class Day20Extensions
    {
        public static T[,] FlipVertical<T>(this T[,] array)
        {
            var rows = array.GetLength(0);
            var columns = array.GetLength(1);

            T[,] flippedArray = new T[rows, columns];

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < columns; c++)
                {
                    flippedArray[r, c] = array[rows - 1 - r, c];
                }
            }

            return flippedArray;
        }

        public static T[,] FlipHorizontal<T>(this T[,] array)
        {
            var rows = array.GetLength(0);
            var columns = array.GetLength(1);

            T[,] flippedArray = new T[rows, columns];

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < columns; c++)
                {
                    flippedArray[r, c] = array[r, columns - 1 - c];
                }
            }

            return flippedArray;
        }

        public static T[,] Transpose<T>(this T[,] array)
        {
            var rows = array.GetLength(0);
            var columns = array.GetLength(1);

            T[,] result = new T[columns, rows];

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < columns; c++)
                {
                    result[c, r] = array[r, c];
                }
            }

            return result;
        }

        public static T[,] ReverseRows<T>(this T[,] array)
        {
            var rows = array.GetLength(0);
            var columns = array.GetLength(1);

            T[,] result = new T[columns, rows];

            for (int r = 0; r < array.GetLength(0); r++)
            {
                for (int c = 0; c < array.GetLength(1) / 2; c++)
                {
                    var tempArray = array[r, c];
                    result[r, c] = array[r, array.GetLength(0) - c - 1];
                    result[r, array.GetLength(0) - c - 1] = tempArray;
                }
            }

            return result;
        }
    }
}
