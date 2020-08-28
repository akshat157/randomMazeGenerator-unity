using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze {
    public int Length;
    public int Width;
    private char [, ] maze;
    public Maze(int rows, int cols) {
        Length = 2*rows+1;
        Width = 2*cols+1;
        maze = new char [Length, Width];
        for (int x = 0; x < Length; x++) {
            for (int z = 0; z < Width; z++) {
                if ((x & 1) == 0 || (z & 1) == 0) {
                    maze[x, z] = '#';
                }
                else {
                    maze[x, z] = ' ';
                }
            }
        }
    }

    public char getMazeValueAt(int x, int y) {
        return maze[x, y];
    }

    public void setMazeValueAt(int x, int y, char c) {
        maze[x, y] = c;
    }
}

public class genMaze : MonoBehaviour {
    public GameObject wallBlock;
    public int nCellsX = 20;
    public int nCellsY = 20;

    private int mazeSize;
    private Maze theMaze;

    struct mCell {
        public int Idx;
        public int X;
        public int Y;
    };

    void Start() {
        theMaze = new Maze(nCellsX, nCellsY);
        GameObject baseFloor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        baseFloor.transform.position = new Vector3(theMaze.Length/2, 0, theMaze.Width/2);
        baseFloor.transform.localScale = new Vector3(theMaze.Length, 0.2f, theMaze.Width);
        StartCoroutine(createMaze());
    }

    int getIdx(int x, int y, mCell[] cellList) {
        for (int i = 0; i < nCellsX*nCellsY; i++) {
            if (cellList[i].X == x && cellList[i].Y == y)
                return cellList[i].Idx;
        }
        Debug.Log("getIdx() couldn't find the index!");
        return -1;
    }

    void genRandMaze() {
        mazeSize = nCellsX*nCellsY;
        mCell[] cellList = new mCell [mazeSize];
        bool[] visited = new bool [mazeSize];
        Stack<mCell> mStack = new Stack<mCell>();
        
        for (int i = 0; i < mazeSize; i++) {
            visited[i] = false;
        }

        int nVisited = 0;
        int idxx = 0;

        for (int i = 1; i < theMaze.Length; i+=2) {
            for (int j = 1; j < theMaze.Width; j+=2) {
                cellList[idxx].Idx = idxx;
                cellList[idxx].X = i;
                cellList[idxx].Y = j;
                idxx++;
            }
        }

        int randIdx = Random.Range(1, 100) % mazeSize;
        mStack.Push(cellList[randIdx]);
        visited[cellList[randIdx].Idx] = true;
        nVisited++;

        while (nVisited < mazeSize) {
            ArrayList neighbours = new ArrayList();

            // North neighbour
            if (mStack.Peek().X > 1) {
                if (theMaze.getMazeValueAt(mStack.Peek().X - 2, mStack.Peek().Y + 0) == ' ' && 
                    !visited[getIdx(mStack.Peek().X - 2, mStack.Peek().Y + 0, cellList)]) {
                    neighbours.Add(0);
                }
            }
            // East neighbour
            if (mStack.Peek().Y < theMaze.Width - 2) {
                if (theMaze.getMazeValueAt(mStack.Peek().X + 0, mStack.Peek().Y + 2) == ' ' && 
                    !visited[getIdx(mStack.Peek().X + 0, mStack.Peek().Y + 2, cellList)]) {
                    neighbours.Add(1);
                }
            }
            // South neighbour
            if (mStack.Peek().X < theMaze.Length - 2) {
                if (theMaze.getMazeValueAt(mStack.Peek().X + 2, mStack.Peek().Y + 0) == ' ' && 
                    !visited[getIdx(mStack.Peek().X + 2, mStack.Peek().Y + 0, cellList)]) {
                    neighbours.Add(2);
                }
            }
            // West neighbour
            if (mStack.Peek().Y > 1) {
                if (theMaze.getMazeValueAt(mStack.Peek().X + 0, mStack.Peek().Y - 2) == ' ' && 
                    !visited[getIdx(mStack.Peek().X + 0, mStack.Peek().Y - 2, cellList)]) {
                    neighbours.Add(3);
                }
            }

            if (neighbours.Count > 0 ) {
                
                int next_cell_dir = (int)neighbours[Random.Range(1, 100) % neighbours.Count];

                switch (next_cell_dir) {
                    case 0: // North
                        theMaze.setMazeValueAt(mStack.Peek().X - 1, mStack.Peek().Y + 0, ' ');
                        mStack.Push(cellList[getIdx(mStack.Peek().X - 2, mStack.Peek().Y + 0, cellList)]);
                        break;
                    case 1: // East
                        theMaze.setMazeValueAt(mStack.Peek().X + 0, mStack.Peek().Y + 1, ' ');
                        mStack.Push(cellList[getIdx(mStack.Peek().X + 0, mStack.Peek().Y + 2, cellList)]);
                        break;
                    case 2: // South
                        theMaze.setMazeValueAt(mStack.Peek().X + 1, mStack.Peek().Y + 0, ' ');
                        mStack.Push(cellList[getIdx(mStack.Peek().X + 2, mStack.Peek().Y + 0, cellList)]);
                        break;
                    case 3: // West
                        theMaze.setMazeValueAt(mStack.Peek().X + 0, mStack.Peek().Y - 1, ' ');
                        mStack.Push(cellList[getIdx(mStack.Peek().X + 0, mStack.Peek().Y - 2, cellList)]);
                        break;
                }

                visited[mStack.Peek().Idx] = true;
                nVisited++;
            }
            else {
                mStack.Pop();
            }
        }        
    }
    
    IEnumerator createMaze() {
        
        genRandMaze();

        for (int x = 0; x < theMaze.Length; x++) {
            for (int z = 0; z < theMaze.Width; z++) {
                yield return new WaitForSeconds(0.0001f);
                Vector3 pos = new Vector3((float)x, 0.6f, (float)z);
                Quaternion rot = new Quaternion(0, 0, 0, 1);
                // wallBlock.transform.localScale = new Vector3(0.9f, 1.0f, 0.9f);
                if (theMaze.getMazeValueAt(x, z) == '#') {
                    Instantiate(wallBlock, pos, rot);
                }
            }
        }
    }
}
