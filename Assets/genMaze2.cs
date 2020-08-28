using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze3d {
    public int Length;
    public int Width;
    private char [, ] maze;
    public Maze3d(int rows, int cols, GameObject wallBlock) {
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

    public char getMazeAt(int x, int y) {
        return maze[x, y];
    }

    public void setMazeAt(int x, int y, char c) {
        maze[x, y] = c;
    }
}

public class genMaze2 : MonoBehaviour {
    public GameObject wallBlock;
    public int nCellsX = 20;
    public int nCellsY = 20;

    private int mazeSize;
    private Maze3d theMaze;

    struct mCell {
        public int Idx;
        public int X;
        public int Y;
    };

    void Start() {
        theMaze = new Maze3d(nCellsX, nCellsY, wallBlock);
        GameObject baseFloor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        baseFloor.transform.position = new Vector3(theMaze.Length/2, 0, theMaze.Width/2);
        baseFloor.transform.localScale = new Vector3(theMaze.Length, 0.2f, theMaze.Width);
    Quaternion zeroRot = new Quaternion(0, 0, 0, 1);
        for (int x = 0; x < theMaze.Length; x++) {
            for (int z = 0; z < theMaze.Width; z++) {
                if ((x & 1) == 0 || (z & 1) == 0) {
                    Vector3 pos = new Vector3((float)x, 0.6f, (float)z);
                    // wallBlock.transform.localScale = new Vector3(0.9f, 1.0f, 0.9f);
                    Instantiate(wallBlock, pos, zeroRot);
                }
            }
        }


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

    void destroyAtPosition(Vector3 location) {
        float radius = 0.01f;

        Collider[] hitCollider = Physics.OverlapSphere(location, radius);
        int i = 0;
        while (i < hitCollider.Length) {
            Debug.Log("i = "+ i);
            Debug.Log("GameObj: " + hitCollider[i].gameObject);
            Destroy(hitCollider[i].gameObject);
            i++;
        }
    }

    IEnumerator createMaze() {
        yield return new WaitForSeconds(1.0f);
        
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
        float delayTime = 0.0001f;
        int randIdx = Random.Range(1, 100) % mazeSize;
        // int randIdx = 0;
        mStack.Push(cellList[randIdx]);
        visited[cellList[randIdx].Idx] = true;
        nVisited++;

        while (nVisited < mazeSize) {
            ArrayList neighbours = new ArrayList();
            Debug.Log("Top: (" + mStack.Peek().X + ", " + mStack.Peek().Y + "), " + mStack.Peek().Idx);
            // North neighbour
            if (mStack.Peek().X > 1) {
                Debug.Log("N: (" + (mStack.Peek().X - 2) + ", " + mStack.Peek().Y + "), " + mStack.Peek().Idx);

                if (theMaze.getMazeAt(mStack.Peek().X - 2, mStack.Peek().Y + 0) == ' ' && 
                    !visited[getIdx(mStack.Peek().X - 2, mStack.Peek().Y + 0, cellList)]) {
                    neighbours.Add(0);
                }
            }
            // East neighbour
            if (mStack.Peek().Y < theMaze.Width - 2) {
                Debug.Log("E: (" + mStack.Peek().X + ", " + (mStack.Peek().Y + 2) + "), " + mStack.Peek().Idx);

                if (theMaze.getMazeAt(mStack.Peek().X + 0, mStack.Peek().Y + 2) == ' ' && 
                    !visited[getIdx(mStack.Peek().X + 0, mStack.Peek().Y + 2, cellList)]) {
                    neighbours.Add(1);
                }
            }
            // South neighbour
            if (mStack.Peek().X < theMaze.Length - 2) {
                Debug.Log("S: (" + (mStack.Peek().X + 2) + ", " + mStack.Peek().Y + "), " + mStack.Peek().Idx);

                if (theMaze.getMazeAt(mStack.Peek().X + 2, mStack.Peek().Y + 0) == ' ' && 
                    !visited[getIdx(mStack.Peek().X + 2, mStack.Peek().Y + 0, cellList)]) {
                    neighbours.Add(2);
                }
            }
            // West neighbour
            if (mStack.Peek().Y > 1) {
                Debug.Log("W: (" + mStack.Peek().X + ", " + (mStack.Peek().Y - 2) + "), " + mStack.Peek().Idx);

                if (theMaze.getMazeAt(mStack.Peek().X + 0, mStack.Peek().Y - 2) == ' ' && 
                    !visited[getIdx(mStack.Peek().X + 0, mStack.Peek().Y - 2, cellList)]) {
                    neighbours.Add(3);
                }
            }

            if (neighbours.Count > 0 ) {
                
                int next_cell_dir = (int)neighbours[Random.Range(1, 100) % neighbours.Count];
                Debug.Log("NextCellDir = " + next_cell_dir);
                Vector3 loc;
                switch (next_cell_dir) {
                    case 0: // North
                    Debug.Log("NN: (" + (mStack.Peek().X - 1) + ", " + mStack.Peek().Y + "), " + mStack.Peek().Idx);

                        theMaze.setMazeAt(mStack.Peek().X - 1, mStack.Peek().Y + 0, ' ');
                        mStack.Push(cellList[getIdx(mStack.Peek().X - 2, mStack.Peek().Y + 0, cellList)]);
                        loc = new Vector3(mStack.Peek().X - 1 + 2, 0.6f, mStack.Peek().Y + 0);
                        yield return new WaitForSeconds(delayTime);
                        destroyAtPosition(loc);
                        break;
                    case 1: // East
                    Debug.Log("EE: (" + (mStack.Peek().X) + ", " + (mStack.Peek().Y + 1) + "), " + mStack.Peek().Idx);

                        theMaze.setMazeAt(mStack.Peek().X + 0, mStack.Peek().Y + 1, ' ');
                        mStack.Push(cellList[getIdx(mStack.Peek().X + 0, mStack.Peek().Y + 2, cellList)]);
                        loc = new Vector3(mStack.Peek().X + 0, 0.6f, mStack.Peek().Y + 1 - 2);
                        yield return new WaitForSeconds(delayTime);
                        destroyAtPosition(loc);
                        break;
                    case 2: // South
                    Debug.Log("SS: (" + (mStack.Peek().X + 1) + ", " + (mStack.Peek().Y) + "), " + mStack.Peek().Idx);

                        theMaze.setMazeAt(mStack.Peek().X + 1, mStack.Peek().Y + 0, ' ');
                        mStack.Push(cellList[getIdx(mStack.Peek().X + 2, mStack.Peek().Y + 0, cellList)]);
                        loc = new Vector3(mStack.Peek().X + 1 - 2, 0.6f, mStack.Peek().Y + 0);
                        yield return new WaitForSeconds(delayTime);
                        destroyAtPosition(loc);
                        break;
                    case 3: // West
                    Debug.Log("WW: (" + (mStack.Peek().X) + ", " + (mStack.Peek().Y - 1) + "), " + mStack.Peek().Idx);

                        theMaze.setMazeAt(mStack.Peek().X + 0, mStack.Peek().Y - 1, ' ');
                        mStack.Push(cellList[getIdx(mStack.Peek().X + 0, mStack.Peek().Y - 2, cellList)]);
                        loc = new Vector3(mStack.Peek().X + 0, 0.6f, mStack.Peek().Y - 1 + 2);
                        yield return new WaitForSeconds(delayTime);
                        destroyAtPosition(loc);                        
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
}
