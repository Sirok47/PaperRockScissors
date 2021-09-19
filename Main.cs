using System;
using System.Linq;
using System.Security.Cryptography;
using ConsoleTables;

namespace PaperRockScissors
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            if (Validate(args)==1) {return;}
            int cmpTurn=genCmpMove(args);
            string key = HMAC(args[cmpTurn]);
            printMenu(args);
            MakeMove(args,key,cmpTurn);
        }
        static bool MakeMove(string[] args, string key, int cmpTurn)
        {
            string choice = Console.ReadLine();
            switch (choice)
            {
                case "?":
                    Table(args);
                    break;
                case "0":
                    return false;
                default:
                    int playerMove;
                    playerMove = toInt(choice) - 1;
                    if (playerMove < 0 || playerMove > args.Length)
                    {
                        Console.WriteLine("Wrong input");
                        return MakeMove(args,key,cmpTurn);
                    }
                    Console.WriteLine("Your move: " + args[playerMove] + "\nComputer move: " + args[cmpTurn] +
                                      "\nResult: " + Compare(playerMove, cmpTurn, args.Length) +
                                      "\nHMAC key: " + key);
                    break;
            }
            return false;
        }
        static int Validate(string[] args)
        {
            if ((args.Distinct().Count() != args.Length) || !(args.Length > 1) || (args.Length % 2 == 0))
            {
                Console.WriteLine("Wrong arguments count!!!");
                return 1;
            }
            return 0;
        }
        static int genCmpMove(string[] args){
             Random rnd = new Random();
             int cmpTurn = rnd.Next(0, args.Length);
            
             return cmpTurn;
        }

        static string HMAC(string arg)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] apiKey = new byte[16];
            rng.GetBytes(apiKey);
            string key = BitConverter.ToString(apiKey).Replace("-","");
            HMACSHA256 hmac = new HMACSHA256(System.Text.ASCIIEncoding.ASCII.GetBytes(key));
            Console.WriteLine(BitConverter.ToString(hmac.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(arg))).Replace("-",""));
            return key;
        }
        static void printMenu(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                Console.WriteLine(i+1+" - "+args[i]);
            }
            Console.WriteLine(0+" - exit\n? - help");
        }
        static int Calculate(int playerMove,int length)
        {
            int a = playerMove + (length - 1) / 2;
            return a > length - 1 ? a - length : a;
        }
        static string Compare(int arg1, int cpu, int length)
        {
            int arg2 = Calculate(arg1,length);
            return arg1==cpu?"tie":(arg1<arg2? arg1<cpu && cpu<=arg2 : arg1<cpu || cpu<=arg2) ? "win" : "lose";
        }

        static void Table(string[] args)
        {   
            string[] newArgs = new String[args.Length + 1];
            newArgs[0] = "You\\Comp";
            Array.Copy(args, 0, newArgs, 1, args.Length);
            var table = new ConsoleTable(newArgs);
            for (int i = 0; i < args.Length; i++)
            {
                string[] row = new string[newArgs.Length];
                row[0] = args[i];
                for (int j = 0; j < args.Length; j++)
                {
                    row[j+1] = Compare(i,j,args.Length);
                }
                table.AddRow(row);
            }
            table.Write();
        }

        static int toInt(string str)
        {
            try
            {
                return Int32.Parse(str);
            }
            catch (FormatException)
            {
                Console.WriteLine("Unable to parse "+str);
                return -1;
            }
        }
    }
}
