using System.Collections.Generic;

namespace Sudoku.ParallelDP
{
  public class ConstraintCellPos
  {
    //      0   1   2    3   4   5    6   7   8
    //
    // 0    0   1   2    3   4   5    6   7   8
    // 1    9  10  11   12  13  14   15  16  17
    // 2   18  19  20   21  22  23   24  25  26
    //
    // 3   27  28  29   30  31  32   33  34  35
    // 4   36  37  38   39  40  41   42  43  44
    // 5   45  46  47   48  49  50   51  52  53
    //
    // 6   54  55  56   57  58  59   60  61  62 
    // 7   63  64  65   66  67  68   69  70  71
    // 8   72  73  74   75  76  77   78  79  80
    //
    public static readonly List<List<int>> Board = new()
    {
      new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 18, 27, 36, 45, 54, 63, 72, 10, 11, 19, 20 }, // 0
      new List<int>() { 0, 2, 3, 4, 5, 6, 7, 8, 10, 19, 28, 37, 46, 55, 64, 73, 9, 11, 18, 20 }, // 1
      new List<int>() { 0, 1, 3, 4, 5, 6, 7, 8, 11, 20, 29, 38, 47, 56, 65, 74, 9, 10, 18, 19 }, // 2
      new List<int>() { 0, 1, 2, 4, 5, 6, 7, 8, 12, 21, 30, 39, 48, 57, 66, 75, 13, 14, 22, 23 }, // 3
      new List<int>() { 0, 1, 2, 3, 5, 6, 7, 8, 13, 22, 31, 40, 49, 58, 67, 76, 12, 14, 21, 23 }, // 4
      new List<int>() { 0, 1, 2, 3, 4, 6, 7, 8, 14, 23, 32, 41, 50, 59, 68, 77, 12, 13, 21, 22 }, // 5
      new List<int>() { 0, 1, 2, 3, 4, 5, 7, 8, 15, 24, 33, 42, 51, 60, 69, 78, 16, 17, 25, 26 }, // 6
      new List<int>() { 0, 1, 2, 3, 4, 5, 6, 8, 16, 25, 34, 43, 52, 61, 70, 79, 15, 17, 24, 26 }, // 7
      new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 17, 26, 35, 44, 53, 62, 71, 80, 15, 16, 24, 25 }, // 8

      new List<int>() { 10, 11, 12, 13, 14, 15, 16, 17, 0, 18, 27, 36, 45, 54, 63, 72, 1, 2, 19, 20 }, // 9
      new List<int>() { 9, 11, 12, 13, 14, 15, 16, 17, 1, 19, 28, 37, 46, 55, 64, 73, 0, 2, 18, 20 }, // 10
      new List<int>() { 9, 10, 12, 13, 14, 15, 16, 17, 2, 20, 29, 38, 47, 56, 65, 74, 0, 1, 18, 19 }, // 11
      new List<int>() { 9, 10, 11, 13, 14, 15, 16, 17, 3, 21, 30, 39, 48, 57, 66, 75, 4, 5, 22, 23 }, // 12
      new List<int>() { 9, 10, 11, 12, 14, 15, 16, 17, 4, 22, 31, 40, 49, 58, 67, 76, 3, 5, 21, 23 }, // 13
      new List<int>() { 9, 10, 11, 12, 13, 15, 16, 17, 5, 23, 32, 41, 50, 59, 68, 77, 3, 4, 21, 22 }, // 14
      new List<int>() { 9, 10, 11, 12, 13, 14, 16, 17, 6, 24, 33, 42, 51, 60, 69, 78, 7, 8, 25, 26 }, // 15
      new List<int>() { 9, 10, 11, 12, 13, 14, 15, 17, 7, 25, 34, 43, 52, 61, 70, 79, 6, 8, 24, 26 }, // 16
      new List<int>() { 9, 10, 11, 12, 13, 14, 15, 16, 8, 26, 35, 44, 53, 62, 71, 80, 6, 7, 24, 25 }, // 17

      new List<int>() { 19, 20, 21, 22, 23, 24, 25, 26, 0, 9, 27, 36, 45, 54, 63, 72, 1, 2, 10, 11 }, // 18
      new List<int>() { 18, 20, 21, 22, 23, 24, 25, 26, 1, 10, 28, 37, 46, 55, 64, 73, 0, 2, 9, 11 }, // 19
      new List<int>() { 18, 19, 21, 22, 23, 24, 25, 26, 2, 11, 29, 38, 47, 56, 65, 74, 0, 1, 9, 10 }, // 20
      new List<int>() { 18, 19, 20, 22, 23, 24, 25, 26, 3, 12, 30, 39, 48, 57, 66, 75, 4, 5, 13, 14 }, // 21
      new List<int>() { 18, 19, 20, 21, 23, 24, 25, 26, 4, 13, 31, 40, 49, 58, 67, 76, 3, 5, 12, 14 }, // 22
      new List<int>() { 18, 19, 20, 21, 22, 24, 25, 26, 5, 14, 32, 41, 50, 59, 68, 77, 3, 4, 12, 13 }, // 23
      new List<int>() { 18, 19, 20, 21, 22, 23, 25, 26, 6, 15, 33, 42, 51, 60, 69, 78, 7, 8, 16, 17 }, // 24
      new List<int>() { 18, 19, 20, 21, 22, 23, 24, 26, 7, 16, 34, 43, 52, 61, 70, 79, 6, 8, 15, 17 }, // 25
      new List<int>() { 18, 19, 20, 21, 22, 23, 24, 25, 8, 17, 35, 44, 53, 62, 71, 80, 6, 7, 15, 16 }, // 26

      new List<int>() { 28, 29, 30, 31, 32, 33, 34, 35, 0, 9, 18, 36, 45, 54, 63, 72, 37, 38, 46, 47 }, // 27
      new List<int>() { 27, 29, 30, 31, 32, 33, 34, 35, 1, 10, 19, 37, 46, 55, 64, 73, 36, 38, 45, 47 }, // 28
      new List<int>() { 27, 28, 30, 31, 32, 33, 34, 35, 2, 11, 20, 38, 47, 56, 65, 74, 36, 37, 45, 46 }, // 29
      new List<int>() { 27, 28, 29, 31, 32, 33, 34, 35, 3, 12, 21, 39, 48, 57, 66, 75, 40, 41, 49, 50 }, // 30
      new List<int>() { 27, 28, 29, 30, 32, 33, 34, 35, 4, 13, 22, 40, 49, 58, 67, 76, 39, 41, 48, 50 }, // 31
      new List<int>() { 27, 28, 29, 30, 31, 33, 34, 35, 5, 14, 23, 41, 50, 59, 68, 77, 39, 40, 48, 49 }, // 32
      new List<int>() { 27, 28, 29, 30, 31, 32, 34, 35, 6, 15, 54, 42, 51, 60, 69, 78, 43, 44, 52, 53 }, // 33
      new List<int>() { 27, 28, 29, 30, 31, 32, 33, 35, 7, 16, 55, 43, 52, 61, 70, 79, 42, 44, 51, 53 }, // 34
      new List<int>() { 27, 28, 29, 30, 31, 32, 33, 34, 8, 17, 56, 44, 53, 62, 71, 80, 42, 43, 51, 52 }, // 35

      new List<int>() { 37, 38, 39, 40, 41, 42, 43, 44, 0, 9, 18, 27, 45, 54, 63, 72, 28, 29, 46, 47 }, // 36
      new List<int>() { 36, 38, 39, 40, 41, 42, 43, 44, 1, 10, 19, 28, 46, 55, 64, 73, 27, 29, 45, 47 }, // 37
      new List<int>() { 36, 37, 39, 40, 41, 42, 43, 44, 2, 11, 20, 29, 47, 56, 65, 74, 27, 28, 45, 46 }, // 38
      new List<int>() { 36, 37, 38, 40, 41, 42, 43, 44, 3, 12, 21, 30, 48, 57, 66, 75, 31, 32, 49, 50 }, // 39
      new List<int>() { 36, 37, 38, 39, 41, 42, 43, 44, 4, 13, 22, 31, 49, 58, 67, 76, 30, 32, 48, 50 }, // 40
      new List<int>() { 36, 37, 38, 39, 40, 42, 43, 44, 5, 14, 23, 32, 50, 59, 68, 77, 30, 31, 48, 49 }, // 41
      new List<int>() { 36, 37, 38, 39, 40, 41, 43, 44, 6, 15, 24, 33, 51, 60, 69, 78, 34, 35, 52, 53 }, // 42
      new List<int>() { 36, 37, 38, 39, 40, 41, 42, 44, 7, 16, 25, 34, 52, 61, 70, 79, 33, 35, 51, 53 }, // 43
      new List<int>() { 36, 37, 38, 39, 40, 41, 42, 43, 8, 17, 26, 35, 53, 62, 71, 80, 33, 34, 51, 52 }, // 44

      new List<int>() { 46, 47, 48, 49, 50, 51, 52, 53, 0, 9, 18, 27, 36, 54, 63, 72, 28, 29, 37, 38 }, // 45
      new List<int>() { 45, 47, 48, 49, 50, 51, 52, 53, 1, 10, 19, 28, 37, 55, 64, 73, 27, 29, 36, 38 }, // 46
      new List<int>() { 45, 46, 48, 49, 50, 51, 52, 53, 2, 11, 20, 29, 38, 56, 65, 74, 27, 28, 36, 37 }, // 47
      new List<int>() { 45, 46, 47, 49, 50, 51, 52, 53, 3, 12, 21, 30, 39, 57, 66, 75, 31, 32, 40, 41 }, // 48
      new List<int>() { 45, 46, 47, 48, 50, 51, 52, 53, 4, 13, 22, 31, 40, 58, 67, 76, 30, 32, 39, 41 }, // 49
      new List<int>() { 45, 46, 47, 48, 49, 51, 52, 53, 5, 14, 23, 32, 41, 59, 68, 77, 30, 31, 39, 40 }, // 50
      new List<int>() { 45, 46, 47, 48, 49, 50, 52, 53, 6, 15, 24, 33, 42, 60, 69, 78, 34, 35, 43, 44 }, // 51
      new List<int>() { 45, 46, 47, 48, 49, 50, 51, 53, 7, 16, 25, 34, 43, 61, 70, 79, 33, 35, 42, 44 }, // 52
      new List<int>() { 45, 46, 47, 48, 49, 50, 51, 52, 8, 17, 26, 35, 44, 62, 71, 80, 33, 34, 42, 43 }, // 53

      new List<int>() { 55, 56, 57, 58, 59, 60, 61, 62, 0, 9, 18, 27, 36, 45, 63, 72, 64, 65, 73, 74 }, // 54
      new List<int>() { 54, 56, 57, 58, 59, 60, 61, 62, 1, 10, 19, 28, 37, 46, 64, 73, 63, 65, 72, 74 }, // 55
      new List<int>() { 54, 55, 57, 58, 59, 60, 61, 62, 2, 11, 20, 29, 38, 47, 65, 74, 63, 64, 72, 73 }, // 56
      new List<int>() { 54, 55, 56, 58, 59, 60, 61, 62, 3, 12, 21, 30, 39, 48, 66, 75, 67, 68, 76, 77 }, // 57
      new List<int>() { 54, 55, 56, 57, 59, 60, 61, 62, 4, 13, 22, 31, 40, 49, 67, 76, 66, 68, 75, 77 }, // 58
      new List<int>() { 54, 55, 56, 57, 58, 60, 61, 62, 5, 14, 23, 32, 41, 50, 68, 77, 66, 67, 75, 76 }, // 59
      new List<int>() { 54, 55, 56, 57, 58, 59, 61, 62, 6, 15, 24, 33, 42, 51, 69, 78, 70, 71, 79, 80 }, // 60
      new List<int>() { 54, 55, 56, 57, 58, 59, 60, 62, 7, 16, 25, 34, 43, 52, 70, 79, 69, 71, 78, 80 }, // 61
      new List<int>() { 54, 55, 56, 57, 58, 59, 60, 61, 8, 17, 26, 35, 44, 53, 71, 80, 69, 70, 78, 79 }, // 62

      new List<int>() { 64, 65, 66, 67, 68, 69, 70, 71, 0, 9, 18, 27, 36, 45, 54, 72, 55, 56, 73, 74 }, // 63
      new List<int>() { 63, 65, 66, 67, 68, 69, 70, 71, 1, 10, 19, 28, 37, 46, 55, 73, 54, 56, 72, 74 }, // 64
      new List<int>() { 63, 64, 66, 67, 68, 69, 70, 71, 2, 11, 20, 29, 38, 47, 56, 74, 54, 55, 72, 73 }, // 65
      new List<int>() { 63, 64, 65, 67, 68, 69, 70, 71, 3, 12, 21, 30, 39, 48, 57, 75, 58, 59, 76, 77 }, // 66
      new List<int>() { 63, 64, 65, 66, 68, 69, 70, 71, 4, 13, 22, 31, 40, 49, 58, 76, 57, 59, 75, 77 }, // 67
      new List<int>() { 63, 64, 65, 66, 67, 69, 70, 71, 5, 14, 23, 32, 41, 50, 59, 77, 57, 58, 75, 76 }, // 68
      new List<int>() { 63, 64, 65, 66, 67, 68, 70, 71, 6, 15, 24, 33, 42, 51, 60, 78, 61, 62, 79, 80 }, // 69
      new List<int>() { 63, 64, 65, 66, 67, 68, 69, 71, 7, 16, 25, 34, 43, 52, 61, 79, 60, 62, 78, 80 }, // 70
      new List<int>() { 63, 64, 65, 66, 67, 68, 69, 70, 8, 17, 26, 35, 44, 53, 62, 80, 60, 61, 78, 79 }, // 71

      new List<int>() { 73, 74, 75, 76, 77, 78, 79, 80, 0, 9, 18, 27, 36, 45, 54, 63, 55, 56, 64, 65 }, // 72
      new List<int>() { 72, 74, 75, 76, 77, 78, 79, 80, 1, 10, 19, 28, 37, 46, 55, 64, 54, 56, 63, 65 }, // 73
      new List<int>() { 72, 73, 75, 76, 77, 78, 79, 80, 2, 11, 20, 29, 38, 47, 56, 65, 54, 55, 63, 64 }, // 74
      new List<int>() { 72, 73, 74, 76, 77, 78, 79, 80, 3, 12, 21, 30, 39, 48, 57, 66, 58, 59, 67, 68 }, // 75
      new List<int>() { 72, 73, 74, 75, 77, 78, 79, 80, 4, 13, 22, 31, 40, 49, 58, 67, 57, 59, 66, 68 }, // 76
      new List<int>() { 72, 73, 74, 75, 76, 78, 79, 80, 5, 14, 23, 32, 41, 50, 59, 68, 57, 58, 66, 67 }, // 77
      new List<int>() { 72, 73, 74, 75, 76, 77, 79, 80, 6, 15, 24, 33, 42, 51, 60, 69, 61, 62, 70, 71 }, // 78
      new List<int>() { 72, 73, 74, 75, 76, 77, 78, 80, 7, 16, 25, 34, 43, 52, 61, 70, 60, 62, 69, 71 }, // 79
      new List<int>() { 72, 73, 74, 75, 76, 77, 78, 79, 8, 17, 26, 35, 44, 53, 62, 71, 60, 61, 69, 70 }, // 80
    };
    public static readonly List<int> AllPossibleValues = new() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

    public static readonly List<List<int>> Houses = new()
    {
      new List<int>() { 0, 9, 18, 27, 36, 45, 54, 63, 72 },
      new List<int>() { 1, 10, 19, 28, 37, 46, 55, 64, 73 },
      new List<int>() { 2, 11, 20, 29, 38, 47, 56, 65, 74 },
      new List<int>() { 3, 12, 21, 30, 39, 48, 57, 66, 75 },
      new List<int>() { 4, 13, 22, 31, 40, 49, 58, 67, 76 },
      new List<int>() { 5, 14, 23, 32, 41, 50, 59, 68, 77 },
      new List<int>() { 6, 15, 24, 33, 42, 51, 60, 69, 78 },
      new List<int>() { 7, 16, 25, 34, 43, 52, 61, 70, 79 },
      new List<int>() { 8, 17, 26, 35, 44, 53, 62, 71, 80 },

      new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8 },
      new List<int>() { 9, 10, 11, 12, 13, 14, 15, 16, 17 },
      new List<int>() { 18, 19, 20, 21, 22, 23, 24, 25, 26 },
      new List<int>() { 27, 28, 29, 30, 31, 32, 33, 34, 35 },
      new List<int>() { 36, 37, 38, 39, 40, 41, 42, 43, 44 },
      new List<int>() { 45, 46, 47, 48, 49, 50, 51, 52, 53 },
      new List<int>() { 54, 55, 56, 57, 58, 59, 60, 61, 62 },
      new List<int>() { 63, 64, 65, 66, 67, 68, 69, 70, 71 },
      new List<int>() { 72, 73, 74, 75, 78, 77, 78, 79, 80 },

      new List<int>() { 0, 1, 2, 9, 10, 11, 18, 19, 20 },
      new List<int>() { 3, 4, 5, 12, 13, 14, 21, 22, 23 },
      new List<int>() { 6, 7, 8, 15, 16, 17, 24, 25, 26 },
      new List<int>() { 27, 28, 29, 36, 37, 38, 45, 46, 47 },
      new List<int>() { 30, 31, 32, 39, 40, 41, 48, 49, 50 },
      new List<int>() { 33, 34, 35, 42, 43, 44, 51, 52, 53 },
      new List<int>() { 54, 55, 56, 63, 64, 65, 72, 73, 74 },
      new List<int>() { 57, 58, 59, 66, 67, 68, 75, 76, 77 },
      new List<int>() { 60, 61, 62, 69, 70, 71, 78, 79, 80 }
    };
  }
}