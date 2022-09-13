using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

/* 1. Make sure you have the maze file "Lab9TerrainFile2.txt"
    in [Astar\Astar\bin\Debug\netcoreapp3.1].
    This file can be changed with another if you rename the file name on line 49*/



namespace Astar
{
    
    class Node
    {
        public int startDistance;
        public int EndDistance;
        public int positionX;
        public int positionY;

        public int cost;
        public Node parentNode;                                                     //The node previously visited
        public bool obstruction;                                                    //Detects an obstruction
        public string nodeState;                                                    //node can be open or closed

        public int setDistancetoEnd(Node Target_Node)
        {

            int distanceX = Math.Abs(Target_Node.positionX - positionX);
            int distanceY = Math.Abs(Target_Node.positionY - positionY);
            if (distanceX > distanceY)
            {
                return ((14 * distanceY) + (10 * (distanceX - distanceY)));
            }
            else
            {
                return ((14 * distanceX) + (10 * (distanceY - distanceX)));
            }
        }
        public void Set_Cost()
        {
            cost = startDistance + EndDistance;
        }
    }
    class Program
    {
        public TimeSpan Elapsed { get; }
        static void Main(string[] args)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            Console.WriteLine("Please Enter the name of the file, (without .txt)");
            string FileName = Console.ReadLine();
            string MazeString = "";
            using (StreamReader sr = new StreamReader(FileName + ".txt")) // rename this or make sure the filename matches this StreamReader name.
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    MazeString = line;
                }

                int columns = Convert.ToInt32(Convert.ToString(MazeString[0]));
                int Rows = Convert.ToInt32(Convert.ToString(MazeString[2]));

                Node[,] Map = new Node[Rows, columns];
                for (int i = 0; i < Rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        Map[i, j] = new Node();
                        Map[i, j].positionX = j;
                        Map[i, j].positionY = i;
                    }

                }
                int startX = 0;
                int startY = 0;
                int targetX = 0;
                int targetY = 0;
                int index = 0;
                if (columns > 9)
                {

                    if (Rows > 9)
                    {
                        index = 6;
                    }
                    else
                    {
                        index = 5;
                    }
                }
                else if (Rows > 9)
                {
                    index = 5;
                }
                else
                {
                    index = 4;
                }
                bool finish = false;
                for (int i = 0; i < Rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        switch (MazeString[index])
                        {
                            case '1':
                                Map[i, j].obstruction = true;
                                break;
                            case '2':
                                startX = j;
                                startY = i;
                                break;
                            case '3':
                                targetX = j;
                                targetY = i;
                                break;
                        }

                        index += 2;
                    }
                }
                List<Node> open = new List<Node>();
                Map[startY, startX].cost = 1;
                open.Add(Map[startY, startX]);
                Map[startY, startX].nodeState = "open";
                Node agent = null;

                while (open.Count > 0 && finish == false)
                {
                    int leastCost = 0;

                    foreach (Node aNode in open)
                    {
                        if (leastCost == 0)
                        {
                            leastCost = aNode.cost;
                            agent = aNode;
                        }
                        else if (aNode.cost == leastCost)
                        {
                            if (aNode.EndDistance < agent.EndDistance)
                            {
                                agent = aNode;
                            }
                        }
                        else
                        {
                            if (aNode.cost < leastCost)
                            {
                                leastCost = aNode.cost;
                                agent = aNode;
                            }
                        }
                    }
                    if (agent.positionX == targetX && agent.positionY == targetY)
                    {
                        finish = true;
                    }
                    else
                    {
                        agent.nodeState = "closed";
                        for (int i = agent.positionY - 1; i < agent.positionY + 2; i++)
                        {
                            for (int j = agent.positionX - 1; j < agent.positionX + 2; j++)
                            {
                                if (i > -1 && i < Rows && j > -1 && j < columns)
                                {
                                    if (Map[i, j].obstruction == false && Map[i, j].nodeState != "open" && Map[i, j].nodeState != "closed")
                                    {
                                        Map[i, j].nodeState = "open";
                                        Map[i, j].startDistance = Map[i, j].setDistancetoEnd(Map[startY, startX]);
                                        Map[i, j].EndDistance = Map[i, j].setDistancetoEnd(Map[targetY, targetX]);
                                        Map[i, j].parentNode = agent;
                                        Map[i, j].Set_Cost();

                                        open.Add(Map[i, j]);
                                    }
                                }
                            }
                        }
                        open.Remove(agent);
                    }
                    for (int i = 0; i < Rows; i++)
                    {
                        for (int j = 0; j < columns; j++)
                        {
                            if (i == startY && j == startX)
                            {
                                Console.Write(" | S");
                            }
                            else if (i == targetY && j == targetX)
                            {
                                Console.Write(" | F");
                            }
                            else if (Map[i, j].obstruction == true)
                            {
                                Console.Write(" | X");
                            }
                            else if (Map[i, j] == agent)
                            {
                                Console.Write(" | *");
                            }
                            else
                            {
                                Console.Write(" |  ");
                            }
                        }
                        Console.Write(" |");
                        Console.WriteLine();
                        Console.Write("  ");
                        for (int k = 0; k < columns; k++)
                        {
                            Console.Write("----");
                        }
                        Console.WriteLine();
                    }

                }
                finish = false;
                while (finish == false)
                {
                    agent = agent.parentNode;

                    if (agent.positionY == startY && agent.positionX == startX)
                    {
                        finish = true;
                        break;
                    }
                    agent.nodeState = "found";
                }
                for (int i = 0; i < Rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        if (i == startY && j == startX)
                        {
                            Console.Write(" | S");
                        }
                        else if (Map[i, j].obstruction == true)
                        {
                            Console.Write(" | X");
                        }
                        else if (i == targetY && j == targetX)
                        {
                            Console.Write(" | F");
                        }
                        else if (Map[i, j].nodeState == "found")
                        {
                            Console.Write(" | *");
                        }
                        else
                        {
                            Console.Write(" |  ");
                        }
                    }
                    Console.Write(" |");
                    Console.WriteLine();
                    Console.Write("  ");
                    for (int k = 0; k < columns; k++)
                    {
                        Console.Write("----");



                    }
                    Console.WriteLine();
                }
                stopWatch.Stop();
                TimeSpan tElapsed = stopWatch.Elapsed;
                Console.WriteLine("Time Elapsed: " + tElapsed);
                Console.ReadLine();
            }
        }

    }
}