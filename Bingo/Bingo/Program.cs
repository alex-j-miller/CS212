using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Runtime.CompilerServices;

namespace Bingo
{
    class Program
    {
        private static RelationshipGraph rg;

        // Read RelationshipGraph whose filename is passed in as a parameter.
        // Build a RelationshipGraph in RelationshipGraph rg
        private static void ReadRelationshipGraph(string filename)
        {
            rg = new RelationshipGraph();                           // create a new RelationshipGraph object

            string name = "";                                       // name of person currently being read
            int numPeople = 0;
            string[] values;
            Console.Write("Reading file " + filename + "\n");
            try
            {
                string input = System.IO.File.ReadAllText(filename);// read file
                input = input.Replace("\r", ";");                   // get rid of nasty carriage returns 
                input = input.Replace("\n", ";");                   // get rid of nasty new lines
                string[] inputItems = Regex.Split(input, @";\s*");  // parse out the relationships (separated by ;)
                foreach (string item in inputItems) 
		{
                    if (item.Length > 2)                            // don't bother with empty relationships
                    {
                        values = Regex.Split(item, @"\s*:\s*");     // parse out relationship:name
                        if (values[0] == "name")                    // name:[personname] indicates start of new person
                        {
                            name = values[1];                       // remember name for future relationships
                            rg.AddNode(name);                       // create the node
                            numPeople++;
                        }
                        else
                        {               
                            rg.AddEdge(name, values[1], values[0]); // add relationship (name1, name2, relationship)

                            // handle symmetric relationships -- add the other way
                            if (values[0] == "hasSpouse" || values[0] == "hasFriend")
                                rg.AddEdge(values[1], name, values[0]);

                            // for parent relationships add child as well
                            else if (values[0] == "hasParent")
                                rg.AddEdge(values[1], name, "hasChild");
                            else if (values[0] == "hasChild")
                                rg.AddEdge(values[1], name, "hasParent");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.Write("Unable to read file {0}: {1}\n", filename, e.ToString());
            }
            Console.WriteLine(numPeople + " people read");
        }

        // Show the relationships a person is involved in
        private static void ShowPerson(string name)
        {
            GraphNode n = rg.GetNode(name);
            if (n != null)
                Console.Write(n.ToString());
            else
                Console.WriteLine("{0} not found", name);
        }

        // Show a person's friends
        private static void ShowFriends(string name)
        {
            GraphNode n = rg.GetNode(name);
            if (n != null)
            {
                Console.Write("{0}'s friends: ",name);
                List<GraphEdge> friendEdges = n.GetEdges("hasFriend");
                foreach (GraphEdge e in friendEdges) {
                    Console.Write("{0} ",e.To());
                }
                Console.WriteLine();
            }
            else
                Console.WriteLine("{0} not found", name);     
        }
        private static void ShowSiblings(string name)
        {
            GraphNode n = rg.GetNode(name);
            if (n != null) // find person
            {
                Console.Write("{0}'s siblings: ", name);
                List<GraphEdge> parentEdges = n.GetEdges("hasParent"); // get parents
                List<GraphNode> parentNodes = new List<GraphNode>();
                foreach (GraphEdge p in parentEdges) // get parent nodes
                {
                    parentNodes.Add(p.ToNode());
                }
                foreach (GraphNode p in parentNodes)
                {
                    List<GraphEdge> childEdges = p.GetEdges("hasChild"); // get children
                    foreach (GraphEdge e in childEdges)
                    {
                        if (e.To() != name) // make sure sibling isn't original person
                            Console.Write("{0} ", e.To());
                    }
                }
            } else
                Console.WriteLine("{0} not found", name);
        }

        private static void ShowDesc(string name)
        {
            GraphNode n = rg.GetNode(name);
            if (n != null)
            {
                Console.Write("{0}'s children: ", name);
                List<GraphEdge> childEdges = n.GetEdges("hasChild");
                List<GraphNode> childNodes = new List<GraphNode>();
                foreach (GraphEdge c in childEdges)
                {
                    Console.Write("{0} ", c.To());
                }
                Console.WriteLine();
                foreach (GraphEdge c in childEdges)
                {
                    ShowDesc(c.To());
                }
                
            } else
                Console.WriteLine("{0} not found", name);
        }

        private static void ShowBingo(string name1, string name2)
        {
            GraphNode n1 = rg.GetNode(name1);
            GraphNode n2 = rg.GetNode(name2);

            List<string> pathList = new List<string>();

            // pathList.Add(n1.Name);

            ShowBingoRecur(n1, n2, pathList);
        }

        private static void ShowBingoRecur(GraphNode n1, GraphNode n2, List<string> localPathList)
        {

            if (n1.Equals(n2))
            {
                Console.WriteLine(string.Join(" ", localPathList));
                // if match found then no need to traverse more till depth
                return;
            }

            n1.Label = "Visited"; // mark the current node

            foreach (GraphEdge e in n1.GetEdges())
            {
                GraphNode n = e.ToNode();
                if (n.Label == "Unvisited")
                {
                    localPathList.Add(e.ToString());
                    ShowBingoRecur(n, n2, localPathList);
                    localPathList.Remove(e.ToString());               }
            }
            n1.Label = "Unvisited";
        }

        // accept, parse, and execute user commands
        private static void CommandLoop()
        {
            string command = "";
            string[] commandWords;
            Console.Write("Welcome to Alex's Dutch Bingo Parlor!\n");

            while (command != "exit")
            {
                Console.Write("\nEnter a command: ");
                command = Console.ReadLine();
                commandWords = Regex.Split(command, @"\s+");        // split input into array of words
                command = commandWords[0];

                if (command == "exit")
                    ;                                               // do nothing

                // read a relationship graph from a file
                else if (command == "read" && commandWords.Length > 1)
                    ReadRelationshipGraph(commandWords[1]);

                // show information for one person
                else if (command == "show" && commandWords.Length > 1)
                    ShowPerson(commandWords[1]);

                else if (command == "friends" && commandWords.Length > 1)
                    ShowFriends(commandWords[1]);

                // show all orphans
                else if (command == "orphans")
                    rg.Orphans();

                // show all siblings
                else if (command == "siblings" && commandWords.Length > 1)
                    ShowSiblings(commandWords[1]);

                // show all descendants
                else if (command == "descendants" && commandWords.Length > 1)
                    ShowDesc(commandWords[1]);

                // play bingo
                else if (command == "bingo" && commandWords.Length > 2)
                    ShowBingo(commandWords[1], commandWords[2]);

                // dump command prints out the graph
                else if (command == "dump")
                    rg.Dump();

                // illegal command
                else
                    Console.Write("\nLegal commands: read [filename], dump, orphans, show [personname],\n" +
                        " friends [personname], siblings [personname], descandents [personname], bingo [personname] [personname], exit\n");
            }

        }

        static void Main(string[] args)
        {
            CommandLoop();
        }
    }
}
