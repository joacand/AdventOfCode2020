using AdventOfCode2020.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020.Days
{
    public class Day17 : IDay
    {
        public string Solve()
        {
            var inputRaw = EmbeddedResource.ReadInput("InputDay17.txt");

            var lines = inputRaw.Split(new[] { "\r\n" }, StringSplitOptions.None).ToList();

            var part1result = SolvePart1(lines.ToList());
            var part2result = SolvePart2(lines.ToList());

            return $"Part 1: {part1result}, Part 2: {part2result}";
        }

        private static string SolvePart1(List<string> input)
        {
            var layout = new Layout(input, 3);
            layout.ApplyRules(6);

            return layout.CountActiveCubes().ToString();
        }

        private static string SolvePart2(List<string> input)
        {
            var layout = new Layout(input, 4);
            layout.ApplyRules(6);

            return layout.CountActiveCubes().ToString();
        }

        private class Layout
        {
            private int dimension;
            private char[,,] area3D;
            private char[,,,] area4D;
            private (int r, int c, int z, int w) lengths;

            public Layout(List<string> input, int dimension)
            {
                lengths = (input.Count, input[0].Length, 1, 1);
                this.dimension = dimension;

                SetupAreas(input);
            }

            public void ApplyRules(int cycles)
            {
                for (var i = 0; i < cycles; i++)
                {
                    ApplyRule();
                }
            }

            public int CountActiveCubes()
            {
                var activeCubes = 0;
                for (int w = 0; w < lengths.w; w++)
                {
                    for (int z = 0; z < lengths.z; z++)
                    {
                        for (int r = 0; r < lengths.r; r++)
                        {
                            for (int c = 0; c < lengths.c; c++)
                            {
                                if (dimension == 3 && area3D[r, c, z] == '#')
                                {
                                    activeCubes++;
                                }
                                else if (dimension == 4 && area4D[r, c, z, w] == '#')
                                {
                                    activeCubes++;
                                }
                            }
                        }
                    }
                }
                return activeCubes;
            }

            private void ApplyRule()
            {
                if (dimension == 3)
                {
                    ApplyRule3D();
                }
                else
                {
                    ApplyRule4D();
                }
            }

            private void ApplyRule3D()
            {
                (int rLength, int cLength, int zLength) = (lengths.r + 2, lengths.c + 2, lengths.z + 2);

                var extendedArea = new char[rLength, cLength, zLength];
                var modifiableArea = new char[rLength, cLength, zLength];
                for (var z = 0; z < lengths.z; z++)
                {
                    for (var r = 0; r < lengths.r; r++)
                    {
                        for (var c = 0; c < lengths.c; c++)
                        {
                            extendedArea[r + 1, c + 1, z + 1] = area3D[r, c, z];
                        }
                    }
                }

                var neighboorCoordinates = Generate3dNeighborCoordinates();

                for (var z = 0; z < zLength; z++)
                {
                    for (var r = 0; r < rLength; r++)
                    {
                        for (var c = 0; c < cLength; c++)
                        {
                            var current = extendedArea[r, c, z];
                            var numberOfActiveNeighboors = GetActiveNeighbors3d(neighboorCoordinates, (r, c, z), extendedArea);

                            switch (current)
                            {
                                case '#' when numberOfActiveNeighboors == 2 || numberOfActiveNeighboors == 3:
                                    modifiableArea[r, c, z] = '#';
                                    break;
                                default:
                                    {
                                        if (numberOfActiveNeighboors == 3)
                                        {
                                            modifiableArea[r, c, z] = '#';
                                        }
                                        else
                                        {
                                            modifiableArea[r, c, z] = '.';
                                        }
                                        break;
                                    }
                            }
                        }
                    }
                }

                lengths = (rLength, cLength, zLength, 1);
                area3D = modifiableArea;
            }

            private void ApplyRule4D()
            {
                (int rLength, int cLength, int zLength, int wLength) = (lengths.r + 2, lengths.c + 2, lengths.z + 2, lengths.w + 2);

                var extendedArea = new char[rLength, cLength, zLength, wLength];
                var modifiableArea = new char[rLength, cLength, zLength, wLength];

                for (var w = 0; w < lengths.w; w++)
                {
                    for (var z = 0; z < lengths.z; z++)
                    {
                        for (var row = 0; row < lengths.r; row++)
                        {
                            for (var column = 0; column < lengths.c; column++)
                            {
                                extendedArea[row + 1, column + 1, z + 1, w + 1] = area4D[row, column, z, w];
                            }
                        }
                    }
                }

                var neighboorCoordinates = Generate4dNeighborCoordinates();

                for (var w = 0; w < wLength; w++)
                {
                    for (var z = 0; z < zLength; z++)
                    {
                        for (var r = 0; r < rLength; r++)
                        {
                            for (var c = 0; c < cLength; c++)
                            {
                                var current = extendedArea[r, c, z, w];
                                var numberOfActiveNeighboors = GetActiveNeighbors4d(neighboorCoordinates, (r, c, z, w), extendedArea);

                                switch (current)
                                {
                                    case '#' when numberOfActiveNeighboors == 2 || numberOfActiveNeighboors == 3:
                                        modifiableArea[r, c, z, w] = '#';
                                        break;
                                    default:
                                        {
                                            if (numberOfActiveNeighboors == 3)
                                            {
                                                modifiableArea[r, c, z, w] = '#';
                                            }
                                            else
                                            {
                                                modifiableArea[r, c, z, w] = '.';
                                            }
                                            break;
                                        }
                                }
                            }
                        }
                    }
                }

                lengths = (rLength, cLength, zLength, wLength);
                area4D = modifiableArea;
            }

            private static int GetActiveNeighbors3d(List<(int r, int c, int z)> neighborCoordinates, (int r, int c, int z) coordinate, char[,,] area)
            {
                var neighbors = 0;
                foreach (var neighborCoord in neighborCoordinates)
                {
                    (int r, int c, int z) = (
                        coordinate.r + neighborCoord.r,
                        coordinate.c + neighborCoord.c,
                        coordinate.z + neighborCoord.z);

                    // Check if outside of bounds
                    if (r < 0 || r >= area.GetLength(0) ||
                        c < 0 || c >= area.GetLength(1) ||
                        z < 0 || z >= area.GetLength(2))
                    {
                        continue;
                    }

                    if (area[r, c, z] == '#')
                    {
                        neighbors++;
                    }
                }
                return neighbors;
            }

            private static int GetActiveNeighbors4d(List<(int r, int c, int z, int w)> neighborCoordinates, (int r, int c, int z, int w) coordinate, char[,,,] area)
            {
                var neighbors = 0;
                foreach (var neighborCoord in neighborCoordinates)
                {
                    (int r, int c, int z, int w) = (
                        coordinate.r + neighborCoord.r,
                        coordinate.c + neighborCoord.c,
                        coordinate.z + neighborCoord.z,
                        coordinate.w + neighborCoord.w);

                    // Check if outside of bounds
                    if (r < 0 || r >= area.GetLength(0) ||
                        c < 0 || c >= area.GetLength(1) ||
                        z < 0 || z >= area.GetLength(2) ||
                        w < 0 || w >= area.GetLength(3))
                    {
                        continue;
                    }

                    if (area[r, c, z, w] == '#')
                    {
                        neighbors++;
                    }
                }
                return neighbors;
            }

            private static List<(int, int, int)> Generate3dNeighborCoordinates()
            {
                List<(int r, int c, int z)> coords = new();

                for (var i = 0; i < 1 << 3; i++)
                {
                    var neighborCoord = Convert.ToString(i, 2).PadLeft(3, '0').Select(c => int.Parse(c.ToString())).ToArray();

                    for (var j = 0; j < 1 << 3; j++)
                    {
                        var mutliplayCoord = Convert.ToString(j, 2).PadLeft(3, '0').Select(c => int.Parse(c.ToString())).Select(x => { if (x == 0) { return -1; } else { return x; } }).ToArray();
                        coords.Add((mutliplayCoord[0] * neighborCoord[0], mutliplayCoord[1] * neighborCoord[1], mutliplayCoord[2] * neighborCoord[2]));
                    }

                }

                return coords.Distinct().Skip(1).ToList();
            }

            private static List<(int, int, int, int)> Generate4dNeighborCoordinates()
            {
                List<(int r, int c, int z, int w)> coords = new();

                for (var i = 0; i < 1 << 4; i++)
                {
                    var neighborCoord = Convert.ToString(i, 2).PadLeft(4, '0').Select(c => int.Parse(c.ToString())).ToArray();

                    for (var j = 0; j < 1 << 4; j++)
                    {
                        var mutliplayCoord = Convert.ToString(j, 2).PadLeft(4, '0').Select(c => int.Parse(c.ToString())).Select(x => { if (x == 0) { return -1; } else { return x; } }).ToArray();
                        coords.Add((mutliplayCoord[0] * neighborCoord[0], mutliplayCoord[1] * neighborCoord[1], mutliplayCoord[2] * neighborCoord[2], mutliplayCoord[3] * neighborCoord[3]));
                    }

                }

                return coords.Distinct().Skip(1).ToList();
            }

            private void SetupAreas(IEnumerable<string> input)
            {
                var area3d = new char[lengths.r, lengths.c, 1];
                var area4d = new char[lengths.r, lengths.c, 1, 1];

                var row = 0;
                foreach (var line in input)
                {
                    var column = 0;
                    foreach (var c in line)
                    {
                        area3d[row, column, 0] = c;
                        area4d[row, column++, 0, 0] = c;
                    }
                    row++;
                }

                area3D = area3d;
                area4D = area4d;
            }
        }
    }
}
