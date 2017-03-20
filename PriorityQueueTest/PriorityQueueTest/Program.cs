using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriorityQueueTest
{
    class Program
    {
        static void Main(string[] args)
        {
            PriorityQueue<string> pQueue = new PriorityQueue<string>();

            while (true)
            { 
                Console.WriteLine("Delete Min (min), Deleta Max (max), or Add Priority (p)");

                string answer = Console.ReadLine();

                switch (answer)
                {
                    case "min":
                        string minValue;
                        pQueue.DequeueMin(out minValue);
                        Console.WriteLine(minValue);
                        Console.WriteLine();
                        pQueue.Print();
                        Console.WriteLine();
                        break;
                    case "max":
                        string maxValue;
                        pQueue.DequeueMax(out maxValue);
                        Console.WriteLine(maxValue);
                        Console.WriteLine();
                        pQueue.Print();
                        Console.WriteLine();
                        break;
                    case "p":

                        while (true)
                        {
                            Console.WriteLine("Enter priority: ");
                            int priority;
                            try
                            {
                                priority = int.Parse(Console.ReadLine());
                            }
                            catch (Exception e)
                            {
                                break;
                            }

                            pQueue.Enqueue(string.Empty, priority);
                            Console.WriteLine();
                            pQueue.Print();
                            Console.WriteLine();
                        }

                        break;
                    default:
                        break;
                }
            }
        }
    }
}
