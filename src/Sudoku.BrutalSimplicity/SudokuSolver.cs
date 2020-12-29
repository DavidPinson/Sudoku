// taken from https://github.com/BrutalSimplicity/SudokuSolver
// for base comparaison and results validation

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Sudoku.BrutalSimplicity
{
  internal class SudokuSolver
  {
    // Convenience class for tracking candidates
    class Candidate : IEnumerable
    {
      bool[] m_values;
      int m_count;
      int m_numCandidates;

      public int Count { get { return m_count; } }

      public Candidate(int numCandidates, bool initialValue)
      {
        m_values = new bool[numCandidates];
        m_count = 0;
        m_numCandidates = numCandidates;

        for(int i = 1; i <= numCandidates; i++)
          this[i] = initialValue;
      }

      public bool this[int key]
      {
        // Allows candidates to be referenced by actual value (i.e. 1-9, rather than 0 - 8)
        get { return m_values[key - 1]; }

        // Automatically tracks the number of candidates
        set
        {
          m_count += (m_values[key - 1] == value) ? 0 : (value == true) ? 1 : -1;
          m_values[key - 1] = value;
        }
      }

      public void SetAll(bool value)
      {
        for(int i = 1; i <= m_numCandidates; i++)
          this[i] = value;
      }

      public override string ToString()
      {
        StringBuilder values = new StringBuilder();
        foreach(int candidate in this)
          values.Append(candidate);
        return values.ToString();
      }

      public IEnumerator GetEnumerator()
      {
        return new CandidateEnumerator(this);
      }

      // Enumerator simplifies iterating over candidates
      private class CandidateEnumerator : IEnumerator
      {
        private int m_position;
        private Candidate m_c;

        public CandidateEnumerator(Candidate c)
        {
          m_c = c;
          m_position = 0;
        }

        // only iterates over valid candidates
        public bool MoveNext()
        {
          ++m_position;
          if(m_position <= m_c.m_numCandidates)
          {
            if(m_c[m_position] == true)
              return true;
            else
              return MoveNext();
          }
          else
          {
            return false;
          }
        }

        public void Reset()
        {
          m_position = 0;
        }

        public object Current
        {
          get { return m_position; }
        }
      }
    }

    // True values for row, grid, and region constraint matrices
    // mean that they contain that candidate, inversely,
    // True values in the candidate constraint matrix means that it
    // is a possible value for that cell.
    Candidate[,] m_cellConstraintMatrix;
    Candidate[] m_rowConstraintMatrix;
    Candidate[] m_colConstraintMatrix;
    Candidate[,] m_regionConstraintMatrix;

    // Actual puzzle grid (uses 0s for unsolved squares)
    public int[,] m_grid;

    // Another convenience structure. Easy and expressive way
    // of passing around row, column information.
    struct Cell
    {
      public int row, col;
      public Cell(int r, int c) { row = r; col = c; }
    }

    // helps avoid iterating over solved squares
    HashSet<Cell> solved;
    HashSet<Cell> unsolved;

    // Tracks the cells changed due to propagation (i.e. the rippled cells)
    Stack<HashSet<Cell>> changed;

    // keeps cell counts by keeping them in buckets
    // this allows the cell with the least candidates
    // (minimum count) to be selected in O(1)
    HashSet<Cell>[] bucketList;

    // Tracks the number of steps a solution takes
    public int steps;

    private void InitializeMatrices()
    {
      for(int row = 0; row < 9; row++)
      {
        for(int col = 0; col < 9; col++)
        {
          // if the square is solved update the candidate list
          // for the row, column, and region
          if(m_grid[row, col] > 0)
          {
            int candidate = m_grid[row, col];
            m_rowConstraintMatrix[row][candidate] = true;
            m_colConstraintMatrix[col][candidate] = true;
            m_regionConstraintMatrix[row / 3, col / 3][candidate] = true;
          }
        }
      }
    }

    private void PopulateCandidates()
    {
      //Add possible candidates by checking
      //the rows, columns and grid
      for(int row = 0; row < 9; row++)
      {
        for(int col = 0; col < 9; col++)
        {
          //if solved, then there are no possible candidates
          if(m_grid[row, col] > 0)
          {
            m_cellConstraintMatrix[row, col].SetAll(false);
            solved.Add(new Cell(row, col));
          }
          else
          {
            // populate each cell with possible candidates
            // by checking the row, col, and grid associated 
            // with that cell
            foreach(int candidate in m_rowConstraintMatrix[row])
              m_cellConstraintMatrix[row, col][candidate] = false;
            foreach(int candidate in m_colConstraintMatrix[col])
              m_cellConstraintMatrix[row, col][candidate] = false;
            foreach(int candidate in m_regionConstraintMatrix[row / 3, col / 3])
              m_cellConstraintMatrix[row, col][candidate] = false;

            Cell c = new Cell(row, col);
            bucketList[m_cellConstraintMatrix[row, col].Count].Add(c);
            unsolved.Add(c);
          }
        }
      }
    }

    private Cell NextCell()
    {
      if(unsolved.Count == 0)
        return new Cell(-1, -1);    // easy way to signal a solved puzzle

      for(int i = 0; i < bucketList.Length; i++)
        if(bucketList[i].Count > 0)
          return bucketList[i].First();

      // should never execute
      return new Cell(99, 99);

    }

    // Backtracking method. Undoes the specified selection
    private void UnselectCandidate(Cell aCell, int candidate)
    {
      // 1) Remove selected candidate from grid
      m_grid[aCell.row, aCell.col] = 0;

      // 2) Add that candidate back to the cell constraint matrix.
      //    Since it wasn't selected, it can still be selected in the 
      //    future
      m_cellConstraintMatrix[aCell.row, aCell.col][candidate] = true;

      // 3) Put cell back in the bucket list
      bucketList[m_cellConstraintMatrix[aCell.row, aCell.col].Count].Add(aCell);

      // 4) Remove the candidate from the row, col, and region constraint matrices
      m_rowConstraintMatrix[aCell.row][candidate] = false;
      m_colConstraintMatrix[aCell.col][candidate] = false;
      m_regionConstraintMatrix[aCell.row / 3, aCell.col / 3][candidate] = false;

      // 5) Add the candidate back to any cells that changed from
      //    its selection (propagation).
      foreach(Cell c in changed.Pop())
      {
        // shift affected cells up the bucket list
        bucketList[m_cellConstraintMatrix[c.row, c.col].Count].Remove(c);
        bucketList[m_cellConstraintMatrix[c.row, c.col].Count + 1].Add(c);
        m_cellConstraintMatrix[c.row, c.col][candidate] = true;
      }

      // 6) Add the cell back to the list of unsolved
      solved.Remove(aCell);
      unsolved.Add(aCell);
    }

    private void SelectCandidate(Cell aCell, int candidate)
    {
      HashSet<Cell> changedCells = new HashSet<Cell>();

      // place candidate on grid
      m_grid[aCell.row, aCell.col] = candidate;

      // remove from bucket list
      bucketList[m_cellConstraintMatrix[aCell.row, aCell.col].Count].Remove(aCell);

      // remove candidate from cell constraint matrix
      m_cellConstraintMatrix[aCell.row, aCell.col][candidate] = false;

      // add the candidate to the cell, row, col, region constraint matrices
      m_colConstraintMatrix[aCell.col][candidate] = true;
      m_rowConstraintMatrix[aCell.row][candidate] = true;
      m_regionConstraintMatrix[aCell.row / 3, aCell.col / 3][candidate] = true;

      /**** RIPPLE ACROSS COL, ROW, REGION ****/

      // (propagation)
      // remove candidates across unsolved cells in the same
      // row and col.
      for(int i = 0; i < 9; i++)
      {
        // only change unsolved cells containing the candidate
        if(m_grid[aCell.row, i] == 0)
        {
          if(m_cellConstraintMatrix[aCell.row, i][candidate] == true)
          {
            // shift affected cells down the bucket list
            bucketList[m_cellConstraintMatrix[aCell.row, i].Count].Remove(new Cell(aCell.row, i));
            bucketList[m_cellConstraintMatrix[aCell.row, i].Count - 1].Add(new Cell(aCell.row, i));

            // remove the candidate
            m_cellConstraintMatrix[aCell.row, i][candidate] = false;

            //update changed cells (for backtracking)
            changedCells.Add(new Cell(aCell.row, i));
          }
        }
        // only change unsolved cells containing the candidate
        if(m_grid[i, aCell.col] == 0)
        {
          if(m_cellConstraintMatrix[i, aCell.col][candidate] == true)
          {
            // shift affected cells down the bucket list
            bucketList[m_cellConstraintMatrix[i, aCell.col].Count].Remove(new Cell(i, aCell.col));
            bucketList[m_cellConstraintMatrix[i, aCell.col].Count - 1].Add(new Cell(i, aCell.col));

            // remove the candidate
            m_cellConstraintMatrix[i, aCell.col][candidate] = false;

            //update changed cells (for backtracking)
            changedCells.Add(new Cell(i, aCell.col));
          }
        }
      }

      // (propagation)
      // remove candidates across unsolved cells in the same
      // region.
      int grid_row_start = aCell.row / 3 * 3;
      int grid_col_start = aCell.col / 3 * 3;
      for(int row = grid_row_start; row < grid_row_start + 3; row++)
        for(int col = grid_col_start; col < grid_col_start + 3; col++)
          // only change unsolved cells containing the candidate
          if(m_grid[row, col] == 0)
          {
            if(m_cellConstraintMatrix[row, col][candidate] == true)
            {
              // shift affected cells down the bucket list
              bucketList[m_cellConstraintMatrix[row, col].Count].Remove(new Cell(row, col));
              bucketList[m_cellConstraintMatrix[row, col].Count - 1].Add(new Cell(row, col));

              // remove the candidate
              m_cellConstraintMatrix[row, col][candidate] = false;

              //update changed cells (for backtracking)
              changedCells.Add(new Cell(row, col));
            }
          }

      // add cell to solved list
      unsolved.Remove(aCell);
      solved.Add(aCell);
      changed.Push(changedCells);
    }

    private bool SolveRecurse(Cell nextCell)
    {
      // Our base case: No more unsolved cells to select, 
      // thus puzzle solved
      if(nextCell.row == -1)
        return true;

      // Loop through all candidates in the cell
      foreach(int candidate in m_cellConstraintMatrix[nextCell.row, nextCell.col])
      {
        SelectCandidate(nextCell, candidate);

        // Move to the next cell.
        // if it returns false backtrack
        if(SolveRecurse(NextCell()) == false)
        {
          ++steps;
          UnselectCandidate(nextCell, candidate);
          continue;
        }
        else // if we recieve true here this means the puzzle was solved earlier
          return true;
      }

      // return false if path is unsolvable
      return false;

    }

    public bool Solve()
    {
      steps = 1;
      return SolveRecurse(NextCell());
    }

    public SudokuSolver(int[,] initialGrid)
    {
      m_grid = new int[9, 9];
      m_cellConstraintMatrix = new Candidate[9, 9];
      m_rowConstraintMatrix = new Candidate[9];
      m_colConstraintMatrix = new Candidate[9];
      m_regionConstraintMatrix = new Candidate[9, 9];
      solved = new HashSet<Cell>();
      unsolved = new HashSet<Cell>();
      changed = new Stack<HashSet<Cell>>();
      bucketList = new HashSet<Cell>[10];
      steps = 0;

      // initialize constraints

      for(int row = 0; row < 9; row++)
      {
        for(int col = 0; col < 9; col++)
        {
          // copy grid, and turn on all Candidates for every cell
          m_grid[row, col] = initialGrid[row, col];
          m_cellConstraintMatrix[row, col] = new Candidate(9, true);
        }
      }

      for(int i = 0; i < 9; i++)
      {
        m_rowConstraintMatrix[i] = new Candidate(9, false);
        m_colConstraintMatrix[i] = new Candidate(9, false);
        bucketList[i] = new HashSet<Cell>();
      }
      bucketList[9] = new HashSet<Cell>();

      for(int row = 0; row < 3; row++)
        for(int col = 0; col < 3; col++)
          m_regionConstraintMatrix[row, col] = new Candidate(9, false);

      InitializeMatrices();
      PopulateCandidates();
    }
  }
}