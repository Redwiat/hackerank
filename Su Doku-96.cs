//#define Test

#if Test
using System.Diagnostics;
using System.Text;
#endif

//Project Euler #96: Su Doku
//https://www.hackerrank.com/contests/projecteuler/challenges/euler096/problem


public static class Extensions
{

    #region Arrays

    public static bool MyContains(this int[] matrix, int value)
    {
        //for (int i = 0; i < 9; i++) Redwiat
        //    if (matrix[i] == value)
        //        return true;

        //return false;

        if (matrix[0] == value)
            return true;

        if (matrix[1] == value)
            return true;

        if (matrix[2] == value)
            return true;

        if (matrix[3] == value)
            return true;

        if (matrix[4] == value)
            return true;

        if (matrix[5] == value)
            return true;

        if (matrix[6] == value)
            return true;

        if (matrix[7] == value)
            return true;

        if (matrix[8] == value)
            return true;

        return false;
    }

    public static int[] GetCol(this int[,] matrix, int colNumber)
    {
        var result = new int[9];
        for (int i = 0; i < 9; i++)
            result[i] = matrix[i, colNumber];

        return result;
    }

    public static int[] GetRow(this int[,] matrix, int rowNumber)
    {
        var result = new int[9];
        for (int i = 0; i < 9; i++)
            result[i] = matrix[rowNumber, i];

        return result;
    }


    public static readonly int[] InitNumberForNeighbor = { 0, 0, 0, 3, 3, 3, 6, 6, 6 };

    public static readonly int[] PossibleValues = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
    public static List<int> GetMissing(this int[] board)
    {
        var missing = new List<int>();

        foreach (var item in PossibleValues)
        {
            if (!board.MyContains(item))
                missing.Add(item);
        }

        return missing;
    }

    public static List<int> GetMissing(this int[,] board)
    {
        return GetMissing(board.Flat());
    }

    public static int[] Flat(this int[,] board)
    {
        //return board.Cast<int>().ToArray(); ?Redwiat

        var result = new int[9];
        var index = 0;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                result[index] = board[i, j];
                index++;
            }
        }

        return result;
    }

    public static int[,] Copy(this int[,] source)
    {
        //return (int[,])source.Clone();
        var result = new int[9, 9];
        for (int i = 0; i < 9; i++)
            for (int j = 0; j < 9; j++)
                result[i, j] = source[i, j];

        return result;
    }

    #endregion

    #region Debug

#if Test

    public static string[] SplitBySize(this string str, int chunkSize)
    {
        return Enumerable.Range(0, str.Length / chunkSize)
          .Select(i => str.Substring(i * chunkSize, chunkSize)).ToArray();
    }

    public static int[,] FilledArray(this string[] splitSolve)
    {
        var filled = new int[9, 9];
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                filled[i, j] = int.Parse(splitSolve[i][j].ToString());
            }
        }
        return filled;
    }

    public static string ToString2(this int[,] source, int pad = 10)
    {
        var result = "";
        for (int i = source.GetLowerBound(0); i <= source.GetUpperBound(0); i++)
        {
            for (int j = source.GetLowerBound(1); j <= source.GetUpperBound(1); j++)
                result += source.GetValue(i, j).ToString().PadLeft(pad);
            if (pad != 0)
                result += "\n";
        }
        return result;
    }

    public static string ToString2(this Dictionary<int, Dictionary<int, List<int>>> source)
    {
        var result = new StringBuilder();
        foreach (var rows in source)
        {
            foreach (var cell in rows.Value)
            {
                result.Append("{");
                var cellContent = new StringBuilder();
                foreach (var possibility in cell.Value)
                {
                    cellContent.Append(possibility + ", ");
                }

                var pad = 40 - cellContent.Length;
                result.Append(cellContent.ToString());
                result.Append("}".PadRight(pad < 40 ? pad : 42));
            }
            result.Append("\n");
        }
        return result.ToString();
    }

    public static void Print(this int[,] board)
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                Console.Write(board[i, j] + " ");
            }

            Console.WriteLine();
        }

        Console.WriteLine();
    }

#endif

    #endregion

}

class Solution
{

    #region Helpers

    public static int[,] GetNeighbors(int[,] matrix, int rowNumber, int colNumber)
    {
        var initNeighborRowNumber = Extensions.InitNumberForNeighbor[rowNumber];
        var initNeighborColNumber = Extensions.InitNumberForNeighbor[colNumber];
        var neighbors = new int[3, 3];

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                neighbors[i, j] = matrix[initNeighborRowNumber + i, initNeighborColNumber + j];
            }
        }

        return neighbors;
    }

    public static List<int> GetNeighborsFlat(int[,] matrix, int rowNumber, int colNumber)
    {
        var initNeighborRowNumber = Extensions.InitNumberForNeighbor[rowNumber];
        var initNeighborColNumber = Extensions.InitNumberForNeighbor[colNumber];
        var neighbors = new List<int>(9);

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                neighbors.Add(matrix[initNeighborRowNumber + i, initNeighborColNumber + j]);
            }
        }

        return neighbors;
    }

    private static bool CanFitInAnotherPlaceInBox(int[,] board, int missingValue, List<(int Row, int Col)> missingZerosPositions,
      int rowNumberOriginal, int colNumberOriginal)
    {
        var initNeighborRowNumber = Extensions.InitNumberForNeighbor[rowNumberOriginal];
        var initNeighborColNumber = Extensions.InitNumberForNeighbor[colNumberOriginal];
        foreach (var item in missingZerosPositions)
        {
            var rowNumber = item.Row + initNeighborRowNumber;
            var colNumber = item.Col + initNeighborColNumber;

            if (colNumber != colNumberOriginal || rowNumber != rowNumberOriginal)
            {
                var row = board.GetRow(rowNumber);
                var col = board.GetCol(colNumber);

                if (!row.MyContains(missingValue) && !col.MyContains(missingValue))
                    return true;
            }
        }

        return false;
    }

    private static bool CanFitInAnotherPlaceIn(int[,] board, int missingValue, List<(int Row, int Col)> missingZerosPositions,
      int rowNumberOriginal, int colNumberOriginal)
    {
        foreach (var item in missingZerosPositions)
        {
            var rowNumber = item.Row;
            var colNumber = item.Col;

            bool neighborsDoNotContain = NeighborsDoNotContain(board, missingValue, rowNumber, colNumber);
            if (neighborsDoNotContain)
            {
                var row = board.GetRow(rowNumber);
                var col = board.GetCol(colNumber);

                if (rowNumber != rowNumberOriginal && !row.MyContains(missingValue))
                    return true;

                if (colNumber != colNumberOriginal && !col.MyContains(missingValue))
                    return true;
            }
        }

        return false;
    }

    private static bool NeighborsDoNotContain(int[,] board, int missingValue, int rowNumber, int colNumber)
    {
        //var initNeighborRowNumber = Extensions.InitNumberForNeighbor[rowNumber];
        //var initNeighborColNumber = Extensions.InitNumberForNeighbor[colNumber];

        //for (int i = 0; i < 3; i++)
        //{
        //    for (int j = 0; j < 3; j++)
        //    {
        //        if (board[initNeighborRowNumber + i, initNeighborColNumber + j] == missingValue)
        //            return false;
        //    }
        //}

        //return true;

        var initNeighborRowNumber = Extensions.InitNumberForNeighbor[rowNumber];
        var initNeighborColNumber = Extensions.InitNumberForNeighbor[colNumber];

        if (board[initNeighborRowNumber + 0, initNeighborColNumber + 0] == missingValue)
            return false;
        if (board[initNeighborRowNumber + 0, initNeighborColNumber + 1] == missingValue)
            return false;
        if (board[initNeighborRowNumber + 0, initNeighborColNumber + 2] == missingValue)
            return false;
        if (board[initNeighborRowNumber + 1, initNeighborColNumber + 0] == missingValue)
            return false;
        if (board[initNeighborRowNumber + 1, initNeighborColNumber + 1] == missingValue)
            return false;
        if (board[initNeighborRowNumber + 1, initNeighborColNumber + 2] == missingValue)
            return false;
        if (board[initNeighborRowNumber + 2, initNeighborColNumber + 0] == missingValue)
            return false;
        if (board[initNeighborRowNumber + 2, initNeighborColNumber + 1] == missingValue)
            return false;
        if (board[initNeighborRowNumber + 2, initNeighborColNumber + 2] == missingValue)
            return false;

        return true;
    }

    private static List<(int Row, int Col)> GetMissingZerosPositionsNeighbors(int[,] neighbors)
    {
        var ret = new List<(int Row, int Col)>(9);
        for (int i = 0; i < neighbors.GetLength(0); i++)
        {
            for (int j = 0; j < neighbors.GetLength(1); j++)
            {
                if (neighbors[i, j] == 0)
                    ret.Add(new(i, j));
            }
        }

        return ret;
    }

    private static List<(int Row, int Col)> GetMissingZerosPositionsCol(int[] board, int colNumber)
    {
        var ret = new List<(int Row, int Col)>(9);
        for (var i = 0; i < 9; i++)
        {
            if (board[i] == 0)
                ret.Add(new(i, colNumber));
        }

        return ret;
    }

    private static List<(int Row, int Col)> GetMissingZerosPositionsRow(int[] board, int rowNumber)
    {
        var ret = new List<(int Row, int Col)>(9);
        for (var i = 0; i < 9; i++)
        {
            if (board[i] == 0)
                ret.Add(new(rowNumber, i));
        }

        return ret;
    }

    #endregion


    private static List<int> _emptyList = new List<int>();
    private static List<int> GetPossibilities(int[,] board, int rowNumber, int colNumber)
    {
        if (board[rowNumber, colNumber] != 0)
            return _emptyList;

        var list = Extensions.PossibleValues.ToList();

        var neighbors = GetNeighborsFlat(board, rowNumber, colNumber);
        for (int i = 0; i < 9; i++)
        {
            list.Remove(board[rowNumber, i]);
            list.Remove(board[i, colNumber]);
            list.Remove(neighbors[i]);
        }

        return list;
    }

    private static int GetValueTo(int[,] board, int rowNumber, int colNumber)
    {
        var values = GetPossibilities(board, rowNumber, colNumber);
        if (values.Count == 1)
            return values.First();

        var row = board.GetRow(rowNumber);
        var missingInRow = row.GetMissing();
        if (missingInRow.Count == 1)
            return missingInRow[0];

        var col = board.GetCol(colNumber);
        var missingInCol = col.GetMissing();
        if (missingInCol.Count == 1)
            return missingInCol[0];

        var neighbors = GetNeighbors(board, rowNumber, colNumber);
        var missingInNei = neighbors.GetMissing();
        if (missingInNei.Count == 1)
            return missingInNei[0];

        var missingZerosPositionsInRow = GetMissingZerosPositionsRow(row, rowNumber);
        var missingZerosPositionsInCol = GetMissingZerosPositionsCol(col, colNumber);
        var missingZerosPositionsInNei = GetMissingZerosPositionsNeighbors(neighbors);
        foreach (var missingValue in values)
        {
            if (!CanFitInAnotherPlaceIn(board, missingValue, missingZerosPositionsInRow, rowNumber, colNumber))
            {
                return missingValue;
            }

            if (!CanFitInAnotherPlaceIn(board, missingValue, missingZerosPositionsInCol, rowNumber, colNumber))
            {
                return missingValue;
            }

            if (!CanFitInAnotherPlaceInBox(board, missingValue, missingZerosPositionsInNei, rowNumber, colNumber))
            {
                return missingValue;
            }
        }

        return -1;
    }

    private static bool Verify(int[,] board, bool exception = false)
    {
        for (var i = 0; i < 9; i++)
        {
            var sumRow = board.GetRow(i).Sum();
            var sumCol = board.GetCol(i).Sum();
            if (sumRow < 45 || sumCol < 45)
                return false;

            if (sumRow == 45 && sumCol == 45)
                continue;

            //if (exception)
            //    throw new Exception("Glitch in the matrix!");

            return false;

        }

        return true;
    }

    public static int[,]? Solve(int[,] board, bool backtracking = false)
    {
        var count = 0;
        while (!Verify(board))
        {
            for (var i = 0; i < 9; i++)
            {
                for (var j = 0; j < 9; j++)
                {
                    if (board[i, j] == 0)
                    {
                        var ijValue = GetValueTo(board, i, j);
                        if (ijValue != -1)
                        {
                            board[i, j] = ijValue;
                        }
                    }
                }
            }

            if (count > 4)
            {
                if (backtracking)
                    return null;

                BacktrackSolution(board);
            }

            count++;
        }

#if Test
        //board.Print();
#endif
        return board;
    }

    #region BackTrack

    private static bool BacktrackSolution(int[,] board, int row = 0, int col = 0)
    {
        if (col == 9)
        {
            row += 1;
            col = 0;
        }

        if (row == 9)
            return true;

        if (board[row, col] != 0)
        {
            return BacktrackSolution(board, row, col + 1);
        }

        for (int i = 1; i <= 9; i++)
        {
            if (!ValueIsValid(board, row, col, i))
                continue;

            board[row, col] = i;

            if (BacktrackSolution(board, row, col + 1) == true)
                return true;

            board[row, col] = 0;
        }

        return false;
    }

    public static readonly int[] InitNumberForNeighbor = { 0, 0, 0, 3, 3, 3, 6, 6, 6 };
    public static readonly int[] EndNumberForNeighbor = { 2, 2, 2, 5, 5, 5, 8, 8, 8 };
    private static bool ValueIsValid(int[,] board, int row, int col, int cur)
    {
        //Test Row/Col
        for (int i = 0; i < 9; i++)
        {
            if (board[row, i] == cur)
                return false;
            if (board[i, col] == cur)
                return false;
        }

        //Test Neighbors
        int rowBorderStart = InitNumberForNeighbor[row];
        int rowBorderEnd = EndNumberForNeighbor[row];

        int colBorderStart = InitNumberForNeighbor[col];
        int colBorderEnd = EndNumberForNeighbor[col];

        for (int i = rowBorderStart; i <= rowBorderEnd; i++)
        {
            for (int j = colBorderStart; j <= colBorderEnd; j++)
            {
                if (board[i, j] == cur)
                    return false;
            }
        }

        return true;
    }

    #endregion

#if Test

    public static void SolveTestCases()
    {

        var sudokus = new List<string>(){
    /*00*/"005900000600053800000200003000090000200000040004085001002041008070000600000300000",
    /*01*/"003020600900305001001806400008102900700000008006708200002609500800203009005010300",
    /*02*/"200080300060070084030500209000105408000000000402706000301007040720040060004010003",
    /*03*/"000000907000420180000705026100904000050000040000507009920108000034059000507000000",
    /*04*/"030050040008010500460000012070502080000603000040109030250000098001020600080060020",
    /*05*/"020810740700003100090002805009040087400208003160030200302700060005600008076051090",
    /*06*/"100920000524010000000000070050008102000000000402700090060000000000030945000071006",
    /*07*/"043080250600000000000001094900004070000608000010200003820500000000000005034090710",
    /*08*/"480006902002008001900370060840010200003704100001060049020085007700900600609200018",
    /*09*/"000900002050123400030000160908000000070000090000000205091000050007439020400007000",
    /*10*/"001900003900700160030005007050000009004302600200000070600100030042007006500006800",
    /*11*/"000125400008400000420800000030000095060902010510000060000003049000007200001298000",
    /*12*/"062340750100005600570000040000094800400000006005830000030000091006400007059083260",
    /**/"300000000005009000200504000020000700160000058704310600000890100000067080000005437",
    /**/"630000000000500008005674000000020000003401020000000345000007004080300902947100080",
    /**/"000020040008035000000070602031046970200000000000501203049000730000000010800004000",
    /**/"361025900080960010400000057008000471000603000259000800740000005020018060005470329",
    /**/"050807020600010090702540006070020301504000908103080070900076205060090003080103040",
    /**/"080005000000003457000070809060400903007010500408007020901020000842300000000100080",
    /**/"003502900000040000106000305900251008070408030800763001308000104000020000005104800",
    /**/"000000000009805100051907420290401065000000000140508093026709580005103600000000000",
    /**/"020030090000907000900208005004806500607000208003102900800605007000309000030020050",
    /**/"005000006070009020000500107804150000000803000000092805907006000030400010200000600",
    /**/"040000050001943600009000300600050002103000506800020007005000200002436700030000040",
    /**/"004000000000030002390700080400009001209801307600200008010008053900040000000000800",
    /**/"360020089000361000000000000803000602400603007607000108000000000000418000970030014",
    /**/"500400060009000800640020000000001008208000501700500000000090084003000600060003002",
    /**/"007256400400000005010030060000508000008060200000107000030070090200000004006312700",
    /**/"000000000079050180800000007007306800450708096003502700700000005016030420000000000",
    /**/"030000080009000500007509200700105008020090030900402001004207100002000800070000090",
    /**/"200170603050000100000006079000040700000801000009050000310400000005000060906037002",
    /**/"000000080800701040040020030374000900000030000005000321010060050050802006080000000",
    /**/"000000085000210009960080100500800016000000000890006007009070052300054000480000000",
    /**/"608070502050608070002000300500090006040302050800050003005000200010704090409060701",
    /**/"050010040107000602000905000208030501040070020901080406000401000304000709020060010",
    /**/"053000790009753400100000002090080010000907000080030070500000003007641200061000940",
    /**/"006080300049070250000405000600317004007000800100826009000702000075040190003090600",
    /**/"005080700700204005320000084060105040008000500070803010450000091600508007003010600",
    /**/"000900800128006400070800060800430007500000009600079008090004010003600284001007000",
    /**/"000080000270000054095000810009806400020403060006905100017000620460000038000090000",
    /**/"000602000400050001085010620038206710000000000019407350026040530900020007000809000",
    /**/"000900002050123400030000160908000000070000090000000205091000050007439020400007000",
    /**/"380000000000400785009020300060090000800302009000040070001070500495006000000000092",
    /**/"000158000002060800030000040027030510000000000046080790050000080004070100000325000",
    /**/"010500200900001000002008030500030007008000500600080004040100700000700006003004050",
    /**/"080000040000469000400000007005904600070608030008502100900000005000781000060000010",
    /*46*/"904200007010000000000706500000800090020904060040002000001607000000000030300005702",
    /**/"000700800006000031040002000024070000010030080000060290000800070860000500002006000",
    /**/"001007090590080001030000080000005800050060020004100000080000030100020079020700400",
    /**/"000003017015009008060000000100007000009000200000500004000000020500600340340200000", // 294863517715429638863751492152947863479386251638512974986134725521678349347295186
    /*50*/"300200000000107000706030500070009080900020004010800050009040301000702000000008006",
    /**/
    /**///Not included
    /*51*/"105000370000000200097300010000053102300801004201470000070008640008000000012000807", //125649378834715296697382415746953182359821764281476953573298641468137529912564837
    /**/"703108459900060800000000000010290367075003008000701000002070015086350020000010000",
    /**/"000060005624000010001000300008004037009100500007500090082470000090310000000029053",
    /**/"806000052090740000072658034000200063903160007000000000000500600410000325507000008",
    /**/"100843000008027031600000000004300100537900080010004305490030070300100000000405010",
    /**/"380000000000400785009020300060090000800302009000040070001070500495006000000000092",
    /**/"027000800001000700000400000300000080050000009070028000090067000580000030000040056",
    /**/"000059020090740000406001908017020500800005702000003041580600000934000280061002000",
    /**/"907501820035020010018006003000000209090652001102049500386400000750210600400000080",
    /*60*/"700004001020060080001500200800090700050307020006050008008009100090010060500800003",
    /**/"000815400105000030000004001019000080060400000208790000001000043800000297006002000",
    /**/"040000000806000007500000100005190060004050000000300091000002436090000000000008020",
    /*63*/"009030600036014089100869035090000800010000090068090170601903002972640300003020900",
    /**/
    /*64*/"857912006291346758346785192124560903760000025905020601412600507670250010500070260", // Needs XY-Wing
    /*65*/"703806090614923700980074063030000070179205630040030010801090306397060001460301907", // Needs swordfish
    /*66*/"901063482048902061060841090406385209080120046000604800120430658630018924804206137", // Needs guessing
    /*67*/"790000003000000060801004002005000000300100000040006209200030006030605421000000000", // Needs guessing/Coloring
  };

        var listTotals = new List<double>();
        var listTotalsPerSudoku = new List<double>();
        var benchmark = 25;
        for (int i = 0; i < benchmark; i++)
        {
            var total = 0.0;

            foreach (var sudoku in sudokus)
            {
                var solve2 = sudoku.SplitBySize(9).FilledArray();

                var sw2 = Stopwatch.StartNew();

                solve2 = Solve(solve2);
                //solve2.Print();

                sw2.Stop();

                var milliseconds = sw2.Elapsed.Milliseconds;
                listTotalsPerSudoku.Add(milliseconds);
                //Console.WriteLine($"Total:   {milliseconds:0}ms");
                total = total + milliseconds;
            }

            Console.WriteLine($"Total:   {total:0.00}ms");
            listTotals.Add(total);
        }

        Console.WriteLine($"TotalAverage:   {listTotals.Average():0.00}ms");
        Console.WriteLine($"TotalAverage:   {listTotalsPerSudoku.Average():0.00}ms");
    }

    static void Main(string[] args)
    {
        SolveTestCases();
        Console.ReadLine();
    }

#else


    static void Main(string[] args)
    {
        var sudoku = new int[9, 9];

        for (int i = 0; i < 9; i++)
        {
            var line = Console.ReadLine();
            for (int j = 0; j < 9; j++)
            {
                if (line != null)
                    sudoku[i, j] = int.Parse(line[j].ToString());
            }
        }

        var solve = Solve(sudoku);

        for (int i = 0; i < 9; i++)
        {
            var line = string.Empty;
            for (int j = 0; j < 9; j++)
            {
                line = line + solve[i, j];
            }
            Console.WriteLine(line);
        }

    }

#endif
}
