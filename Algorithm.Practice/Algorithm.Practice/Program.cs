// Online C# Editor for free
// Write, Edit and Run your C# code using C# Online Compiler

using System;

public class HelloWorld
{
    public static void Main(string[] args)
    {

        Console.Write("Input the number of swordmen : ");
        int swordsmen = Convert.ToInt32(Console.ReadLine()!);

        Console.Write("Input the number of archers : ");
        int archers = Convert.ToInt32(Console.ReadLine()!);

        Console.Write("Input the number of mages : ");
        int mages = Convert.ToInt32(Console.ReadLine()!);

        Console.Write("Input the number of healers : ");
        int healers = Convert.ToInt32(Console.ReadLine()!);

        Console.WriteLine("\n#========================================#\n");

        var dragonList = CreateDragonTeams(swordsmen, archers, mages, healers);

        for (int i = 0; i < dragonList.Count; i++)
        {
            var dragon = dragonList[i];
            Console.WriteLine($"Dragon {i + 1} : Swordmen: {dragon.Swordmen}, Archers: {dragon.Archers}, Mages: {dragon.Mages}, Healers: {dragon.Healers}");
        }


    }


    static List<Dragon> CreateDragonTeams(int swordsmen, int archers, int mages, int healers)
    {
        const int maxWarriorsPerDragon = 5;
        const int maxWarriorsPerGroup = 2;

        if (swordsmen < 0 || archers < 0 || mages < 0 || healers < 0)
            throw new ArgumentException("Number of warriors cannot be negative.");

        int totalWarriors = swordsmen + archers + mages + healers;

        if (totalWarriors == 0)
            return new List<Dragon>();

        int initialDragonCount = (int)Math.Ceiling(totalWarriors / (double)maxWarriorsPerDragon);

        var dragonList = new List<Dragon>();
        for (int i = 0; i < initialDragonCount; i++)
        {
            dragonList.Add(new Dragon());
        }

        foreach (var dragon in dragonList)
        {
            if (swordsmen > 0)
            {
                dragon.Swordmen++;
                swordsmen--;
            }

            if (archers > 0)
            {
                dragon.Archers++;
                archers--;
            }

            if (mages > 0)
            {
                dragon.Mages++;
                mages--;
            }

            if (healers > 0)
            {
                dragon.Healers++;
                healers--;

            }
        }

        foreach (var dragon in dragonList)
        {
            while (dragon.TotalWarrior < maxWarriorsPerDragon)
            {
                if ( swordsmen > 0 && dragon.Swordmen < maxWarriorsPerGroup)
                {
                    dragon.Swordmen++;
                    swordsmen--;
                }
                else if (archers > 0 && dragon.Archers < maxWarriorsPerGroup)
                {
                    dragon.Archers++;
                    archers--;
                }
                else if (mages > 0 && dragon.Mages < maxWarriorsPerGroup)
                {
                    dragon.Mages++;
                    mages--;
                }
                else if (healers > 0 && dragon.Healers < maxWarriorsPerGroup)
                {
                    dragon.Healers++;
                    healers--;
                }
                else
                {
                    break;
                }       
            }
        }

        while (GetRemainingWarriors(swordsmen, archers, mages, healers) > 0)
        {
            int remainingWarriors =
                GetRemainingWarriors(swordsmen, archers, mages, healers);

            var dragon = new Dragon();

            // If fewer than 5 warriors remain,
            // put all remaining warriors into the last dragon.
            //if (remainingWarriors < maxWarriorsPerDragon)
            //{
            //    dragon.Swordmen = swordsmen;
            //    dragon.Archers = archers;
            //    dragon.Mages = mages;
            //    dragon.Healers = healers;

            //    dragonList.Add(dragon);
            //    break;
            //}

            // Ideally take one warrior from each class.
            if (swordsmen > 0)
            {
                dragon.Swordmen++;
                swordsmen--;
            }

            if (archers > 0)
            {
                dragon.Archers++;
                archers--;
            }

            if (mages > 0)
            {
                dragon.Mages++;
                mages--;
            }

            if (healers > 0)
            {
                dragon.Healers++;
                healers--;
            }

            // Fill remaining slots by priority:
            // Swordsmen -> Archers -> Mages -> Healers
            while (dragon.TotalWarrior < maxWarriorsPerDragon)
            {
                if (swordsmen > 0 &&
                    dragon.Swordmen < maxWarriorsPerGroup)
                {
                    dragon.Swordmen++;
                    swordsmen--;
                }
                else if (archers > 0 &&
                         dragon.Archers < maxWarriorsPerGroup)
                {
                    dragon.Archers++;
                    archers--;
                }
                else if (mages > 0 &&
                         dragon.Mages < maxWarriorsPerGroup)
                {
                    dragon.Mages++;
                    mages--;
                }
                else if (healers > 0 &&
                         dragon.Healers < maxWarriorsPerGroup)
                {
                    dragon.Healers++;
                    healers--;
                }
                else
                {
                    break;
                }
            }

            dragonList.Add(dragon);
        }

        return dragonList;
    }

    static int GetRemainingWarriors(int swordsmen, int archers, int mages, int healers)
    {
        return swordsmen + archers + mages + healers;
    }
 
    class Dragon
    {
        public int Swordmen { get; set; } = 0;
        public int Archers { get; set; } = 0;
        public int Mages { get; set; } = 0;
        public int Healers { get; set; } = 0;

        public int TotalWarrior => Swordmen + Archers + Mages + Healers;
    }
}


