using System;
using System.Collections.Generic;
using System.Linq;

namespace AvenueCodeTest
{

    public class MagicForest
    {
        List<Edge> Edges {get; set;}
        int Nodes {get; set;}
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MagicForest"/> class.
        /// </summary>
        /// <param name="nodes">Number of nodes in the magic forest. Nodes are numbered 0 .. nodes-1.</param>
        /// <param name="edges">List of edges.</param>
        public MagicForest(int nodes, List<Edge> edges)
        {
            Edges = edges;
            Nodes = nodes;
        }

        public int CountTrees()
        {
            int treeCount = 0;
            List<Edge> tree = new List<Edge>();

            var fGroup = Edges.GroupBy( e => e.From);   // 1, 3, 4, 6
            var tGroup = Edges.GroupBy( e => e.To);     // 2, 4, 5, 7, 8, 9

            for(int i = 0; i< Nodes; i ++)
            {
                int fCount = fGroup.Count( f => f.Key == i);
                int tCount = tGroup.Count( f => f.Key == i);
                
                if (fCount == 0 && tCount == 0 ) { treeCount ++; tree.Add(new Edge(i,i) );  continue; }
                if (fCount > 0 && tCount > 0) continue;                         //Do not add to tree since it found in both direction.

                if (fCount > 0  )
                {
                    foreach(var e in fGroup.Where(g => g.Key == i ))
                    {
                       List<int> tos = e.Select(f => f.To).ToList();
                       
                       if (!Edges.Any(f => tos.Contains( f.From ) )) { treeCount ++; tree.AddRange( e.Select( f => f) );  }                      
                    }
                }
                if (tCount > 0  )
                {
                    foreach(var e in tGroup.Where(g => g.Key == i ))
                    {
                       List<int> froms = e.Select(f => f.From).ToList();
                       
                       if (!Edges.Any(f => froms.Contains( f.To ) )) { treeCount ++; tree.AddRange( e.Select( f => f) );  }                          
                    }
                }
            }

            treeCount = tree.Distinct().Select( d => d.From).Distinct().Count();

            return treeCount;
        }

        public static void Try()
        {
            List<Edge> edges = new List<Edge>();
            edges.Add(new Edge(1, 2));
            edges.Add(new Edge(3, 4));
            edges.Add(new Edge(3, 5));
            edges.Add(new Edge(4, 5));
            edges.Add(new Edge(6, 7));
            edges.Add(new Edge(6, 8));
            edges.Add(new Edge(6, 9));

            MagicForest forest = new MagicForest(10, edges);
            Console.WriteLine(forest.CountTrees());
        }
    }

    public class Edge
    {
        public int From { private set; get; }

        public int To { private set; get; }

        public Edge(int from, int to)
        {
            this.From = from > to ? to : from;
            this.To = to < from ? from : to;
        }
    }
}