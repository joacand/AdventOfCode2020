using AdventOfCode2020.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020.Days
{
    public class Day20 : IDay
    {
        private const int SeaMonsterSize = 15;

        public string Solve()
        {
            var inputRaw = EmbeddedResource.ReadInput("InputDay20.txt");

            var lines = inputRaw.Split(new[] { "\r\n" }, StringSplitOptions.None).ToList();

            var part1result = SolvePart1(lines.ToList());
            var part2result = SolvePart2(lines.ToList());

            return $"Part 1: {part1result}, Part 2: {part2result}";
        }

        private static string SolvePart1(List<string> input)
        {
            var matches = FindMatches(ParseInput(input));

            var corners = matches.Where(x => x.NumberOfNeighbors == 2).Select(x => x.Tile.Id).ToList();

            return corners.Product().ToString();
        }

        private static string SolvePart2(List<string> input)
        {
            var tiles = ParseInput(input);

            var cornerTiles = FindMatches(tiles).Where(x => x.NumberOfNeighbors == 2).ToList();

            var bigPictureTiles = StitchTogetherBigPicture(tiles, cornerTiles[0].Tile);

            var bigPictureDataArray = AssemblyBigPicture(bigPictureTiles);

            var seaMonsterSquares = CountSeaMonstersAllPermutations(bigPictureDataArray) * SeaMonsterSize;

            var totalSquares = CountTotalSquares(bigPictureDataArray);

            return (totalSquares - seaMonsterSquares).ToString();
        }

        private static Tile[,] StitchTogetherBigPicture(List<Tile> tiles, Tile cornerTile)
        {
            var gridSize = (int)Math.Sqrt(tiles.Count);
            var bigPicture = new Tile[gridSize, gridSize];
            var topLeftPermutation = FindTopLeftCornerTilePermutation(cornerTile, tiles);

            List<Tile> usedTiles = new();
            var currentMatching = topLeftPermutation;
            var column = 0;
            bigPicture[0, column++] = currentMatching;
            Tile nextMatchingTile;

            // Stitch tiles from left to right for top row
            while (MatchRightHandSide(currentMatching.GetBordersClass(), tiles, currentMatching.Id, usedTiles, out nextMatchingTile))
            {
                bigPicture[0, column++] = nextMatchingTile;
                usedTiles.Add(nextMatchingTile);

                currentMatching = nextMatchingTile;
            }

            // Stitch tiles from top to bottom (remaining tiles, exclude top row)
            for (int tileRow = 0; tileRow < bigPicture.GetLength(0) - 1; tileRow++)
            {
                for (int tileCol = 0; tileCol < bigPicture.GetLength(1); tileCol++)
                {
                    var tile = bigPicture[tileRow, tileCol];

                    MatchBottomToTop(tile.GetBordersClass(), tiles, tile.Id, usedTiles, out nextMatchingTile);
                    bigPicture[tileRow + 1, tileCol] = nextMatchingTile;
                    usedTiles.Add(nextMatchingTile);
                }
            }

            return bigPicture;
        }

        private static Tile FindTopLeftCornerTilePermutation(Tile topLeftCorner, List<Tile> tiles)
        {
            var permutationIndex = 0;
            while (
                !MatchRightHandSide(topLeftCorner.PermutationTiles[permutationIndex].GetBordersClass(), tiles, topLeftCorner.Id, new List<Tile>(), out _) &&
                !MatchBottomToTop(topLeftCorner.PermutationTiles[permutationIndex].GetBordersClass(), tiles, topLeftCorner.Id, new List<Tile>(), out _))
            {
                permutationIndex++;
            }
            return topLeftCorner.PermutationTiles[permutationIndex];
        }

        private static int CountSeaMonstersAllPermutations(char[,] bigPictureData)
        {
            for (int rotation = 0; rotation < 4; rotation++)
            {
                bigPictureData = Tile.Rotate90(bigPictureData);

                for (int flipDirection = 0; flipDirection < 4; flipDirection++)
                {
                    var flipped = Tile.Flip(bigPictureData, flipDirection);

                    var seaMonsters = CountSeaMonsters(flipped);

                    if (seaMonsters > 0) return seaMonsters;
                }
            }
            throw new Exception("No sea monsters found");
        }

        private static int CountSeaMonsters(char[,] bigPictureData)
        {
            (int row, int col)[] seaMonsterMatrix = new[]
            {
                (18,0),
                (0,1),
                (5,1),
                (6,1),
                (11,1),
                (12,1),
                (17,1),
                (18,1),
                (19,1),
                (1,2),
                (4,2),
                (7,2),
                (10,2),
                (13,2),
                (16,2)
            };

            var seaMonsterWidth = 20;
            var seaMonsterHeight = 3;

            var seaMonsters = 0;
            for (var row = 0; row < bigPictureData.GetLength(1) - seaMonsterWidth; row++)
            {
                for (var col = 0; col < bigPictureData.GetLength(0) - seaMonsterHeight; col++)
                {
                    seaMonsters += seaMonsterMatrix.All(p => bigPictureData[row + p.row, col + p.col] == '#') ? 1 : 0;
                }
            }
            return seaMonsters;
        }

        private static int CountTotalSquares(char[,] bigPic)
        {
            var totalSquares = 0;
            for (int row = 0; row < bigPic.GetLength(0); row++)
            {
                for (int col = 0; col < bigPic.GetLength(1); col++)
                {
                    if (bigPic[row, col] == '#') totalSquares++;
                }
            }
            return totalSquares;
        }

        private static char[,] AssemblyBigPicture(Tile[,] bigPicture)
        {
            var rowColLength = bigPicture[0, 0].DataWithoutBorders().GetLength(0);

            var result = new char[12 * rowColLength, 12 * rowColLength];

            var charrow = 0;
            var charcol = 0;

            for (var tileRow = bigPicture.GetLength(0) - 1; tileRow >= 0; tileRow--)
            {
                for (var row = 0; row < rowColLength; row++)
                {
                    for (var tileCol = 0; tileCol < bigPicture.GetLength(1); tileCol++)
                    {
                        var tile = bigPicture[tileRow, tileCol];

                        var dataWithoutBorders = tile.DataWithoutBorders();

                        for (var col = 0; col < rowColLength; col++)
                        {
                            result[charrow, charcol++] = dataWithoutBorders[row, col];
                        }
                    }
                    charrow++;
                    charcol = 0;
                }
            }

            return result;
        }

        private static bool MatchRightHandSide(Borders borders, List<Tile> tiles, int id, List<Tile> usedTiles, out Tile matchingTile)
        {
            matchingTile = null;

            for (var i = 0; i < tiles.Count; i++)
            {
                var tile = tiles[i];
                if (tile.Id == id || usedTiles.Contains(tile)) continue;

                for (var j = 0; j < tile.PermutationTiles.Count; j++)
                {
                    var tilePermutation = tile.PermutationTiles[j];

                    var permutationBorders = tilePermutation.GetBordersClass();

                    if (borders.Right == permutationBorders.Left)
                    {
                        matchingTile = tilePermutation;
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool MatchBottomToTop(Borders borders, List<Tile> tiles, int id, List<Tile> usedTiles, out Tile matchingTile)
        {
            matchingTile = null;

            for (var i = 0; i < tiles.Count; i++)
            {
                var tile = tiles[i];
                if (tile.Id == id || usedTiles.Contains(tile)) continue;

                for (var j = 0; j < tile.PermutationTiles.Count; j++)
                {
                    var tilePermutations = tile.PermutationTiles[j];

                    var permutationBorders = tilePermutations.GetBordersClass();

                    if (borders.Bottom == permutationBorders.Top)
                    {
                        matchingTile = tilePermutations;
                        return true;
                    }
                }
            }

            return false;
        }

        private static List<TileConnections> FindMatches(List<Tile> tiles)
        {
            for (int matchesToSearchFor = 4; matchesToSearchFor > 0; matchesToSearchFor--)
            {
                for (var i = 0; i < tiles.Count; i++)
                {
                    var firstTile = tiles[i];

                    if (firstTile.NeighboringTiles >= matchesToSearchFor) continue;

                    var maxmatches = 0;

                    for (var j = 0; j < firstTile.PermutationTiles.Count; j++)
                    {
                        var firstTilePermutation = firstTile.PermutationTiles[j];

                        var matches = FindMatchesAgainstOtherTiles(firstTilePermutation, tiles, i);

                        if (matches == matchesToSearchFor)
                        {
                            firstTile.PermutationIndices.Add(j);
                            maxmatches = matches;
                            firstTile.NeighboringTiles = maxmatches;
                        }
                    }
                }
            }

            return tiles.Select(x => new TileConnections(x, x.NeighboringTiles, x.PermutationIndices.First())).ToList();
        }

        private static int FindMatchesAgainstOtherTiles(Tile tilePermToCheckAgainstOtherTiles, List<Tile> tiles, int indexOfTile)
        {
            var matches = 0;

            for (var i = 0; i < tiles.Count; i++)
            {
                if (i == indexOfTile) continue; // Do not check against yourself

                var localMatches = 0;

                var tileToCheckAgainst = tiles[i];

                // If an index is decided from before
                if (tileToCheckAgainst.PermutationIndices.Any())
                {
                    var match = false;

                    for (var j = 0; j < tileToCheckAgainst.PermutationIndices.Count; j++)
                    {
                        var perm = tileToCheckAgainst.PermutationTiles[tileToCheckAgainst.PermutationIndices[j]];

                        if (Match(tilePermToCheckAgainstOtherTiles, perm))
                        {
                            match = true;
                        }
                    }

                    if (match)
                    {
                        matches++;
                    }
                }
                else
                {
                    // Otherwise check all perms
                    for (var j = 0; j < tileToCheckAgainst.PermutationTiles.Count; j++)
                    {
                        var permutationTileToCheckAgainst = tileToCheckAgainst.PermutationTiles[j];

                        if (Match(tilePermToCheckAgainstOtherTiles, permutationTileToCheckAgainst))
                        {
                            localMatches++;
                            break;
                        }
                    }
                    if (localMatches > 0) matches++;
                }
            }

            return matches;
        }

        private record TileConnections(Tile Tile, int NumberOfNeighbors, int PermutationIndex);

        private static bool Match(Tile t1, Tile t2)
        {
            var tile1borders = t1.GetBorders();
            var tile2borders = t2.GetBorders();

            for (var i = 0; i < tile1borders.Length; i++)
            {
                var compareborderindex = (i + 2) % tile1borders.Length;
                if (tile1borders[i].Equals(tile2borders[compareborderindex]))
                {
                    return true;
                }
            }
            return false;
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

        private class Borders
        {
            public string Left { get; set; }
            public string Top { get; set; }
            public string Right { get; set; }
            public string Bottom { get; set; }
        }

        private class Tile : IEquatable<Tile>
        {
            public int Id { get; private set; }
            public char[,] Data { get; private set; }
            public int NeighboringTiles { get; set; }
            public List<Tile> PermutationTiles { get; } = new();
            public List<int> PermutationIndices { get; } = new();

            public Tile(List<string> rawData)
            {
                Data = ParseData(rawData);
                PermutationTiles = GetPermutationTiles().Distinct().ToList();
            }

            public Tile(char[,] data, int id)
            {
                Data = data;
                Id = id;
            }

            public char[,] DataWithoutBorders()
            {
                var result = new char[8, 8];

                for (int row = 1; row < Data.GetLength(0) - 1; row++)
                {
                    for (int col = 1; col < Data.GetLength(1) - 1; col++)
                    {
                        result[row - 1, col - 1] = Data[row, col];
                    }
                }

                return result;
            }

            private List<Tile> GetPermutationTiles()
            {
                List<Tile> permutations = new();

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

                    for (int j = 0; j < 4; j++)
                    {
                        var flipped = Flip(rotated, j);

                        permutations.Add(new Tile(flipped, Id));
                    }
                }

                return permutations;
            }

            public Borders GetBordersClass()
            {
                var borders = GetBorders();
                return new Borders { Left = borders[0], Top = borders[1], Right = borders[2], Bottom = borders[3] };
            }

            public string[] GetBorders()
            {
                return GetBorders(Data);
            }

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
                return transposed.ReverseRows();
            }

            public static char[,] Flip(char[,] flipArray, int dir)
            {
                return dir switch
                {
                    0 => flipArray.FlipVertical(),
                    1 => flipArray.FlipHorizontal(),
                    2 => flipArray.FlipHorizontal().FlipVertical(),
                    3 => flipArray,
                    _ => throw new Exception(),
                };
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

            public override bool Equals(object obj)
            {
                return Equals(obj as Tile);
            }

            public bool Equals(Tile other)
            {
                return other != null &&
                       EqualityComparer<char[,]>.Default.Equals(Data, other.Data);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Id);
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
