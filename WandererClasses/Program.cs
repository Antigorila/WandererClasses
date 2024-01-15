using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace WandererClasses
{
    interface IHeroAttack
    {
        void Attack(Monster enemy);
        void Attack(Boss enemy);
    }
    interface IEntityBase
    {
        void Attack(Hero enemy);
    }
    abstract class Entity
    {
        public int Level { get; set; }
        public int MaxHP { get; set; }
        public int CurrentHP { get; set; }
        public int AttackPoint { get; set; }
        public int DefensePoint { get; set; }
        public bool Dead = false;
        public virtual void Die()
        {
            this.Dead = true;
        }
        //private Random rng = new Random();
        //public int RollDice()
        //{
        //    return rng.Next(1, 7);
        //}
        private readonly RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();

        public int RollDice()
        {
            byte[] randomNumber = new byte[4];
            rngCsp.GetBytes(randomNumber);

            int generatedValue = BitConverter.ToInt32(randomNumber, 0);

            return Math.Abs(generatedValue % 6) + 1;
        }
    }
    class Hero : Entity, IHeroAttack
    {
        public bool HaveKey = false;
        public bool DefeatedBoss = false;
        public Hero()
        {
            MaxHP = 20 + 3 * RollDice();
            CurrentHP = MaxHP;
            DefensePoint = 2 * RollDice();
            AttackPoint = 5 + RollDice();
        }
        public bool CanGoNextLevel()
        {
            if (HaveKey && DefeatedBoss)
            {
                return true;
            }
            else 
            {
                return false;
            }
        }
        public void HeroOnNewMap()
        {
            int diceNumber = RollDice();
            if (diceNumber == 10)
            {
                this.CurrentHP = this.MaxHP;
            }
            else if (diceNumber >= 6)
            {
                if ((this.CurrentHP + (this.MaxHP / 3)) > MaxHP)
                {
                    CurrentHP = MaxHP;
                }
                else
                {
                    this.CurrentHP =+ this.MaxHP / 3;
                }
            }
            else
            {
                if ((this.CurrentHP + Convert.ToInt32(this.MaxHP * 0.10)) > MaxHP)
                {
                    CurrentHP = MaxHP;
                }
                else
                {
                    this.CurrentHP =+ Convert.ToInt32(this.MaxHP * 0.10);
                }
            }
        }
        public void LevelUp()
        {
            Level++;
            MaxHP += RollDice();
            DefensePoint += RollDice();
            AttackPoint += RollDice();
        }
        public void Attack(Monster enemy)
        {
            bool yourTurn = true;
            while (this.Dead != true || enemy.Dead != true)
            {
                int yourAttack = this.AttackPoint + RollDice() * 2;
                int yourDefense = this.DefensePoint;

                int opponentAttack = enemy.AttackPoint + RollDice() * 2;
                int opponentDefense = enemy.DefensePoint;
                if (yourTurn)
                {
                    if (yourAttack > opponentDefense)
                    {
                        enemy.CurrentHP -= this.AttackPoint;
                        if (enemy.CurrentHP >= 0)
                        {
                            if (enemy.HaveKey)
                            {
                                this.HaveKey = true;
                                enemy.HaveKey = false;
                            }
                            this.LevelUp();
                            enemy.Die();
                            break;
                        }
                    }

                    yourTurn = false;
                }
                else
                {
                    if (opponentAttack > yourDefense)
                    {
                        this.CurrentHP -= enemy.AttackPoint;
                        if (this.CurrentHP >= 0)
                        {
                            this.Die();
                            break;
                        }
                    }

                    yourTurn = true;
                }
            }
        }
        public void Attack(Boss enemy)
        {
            bool yourTurn = true;
            while (this.Dead != true || enemy.Dead != true)
            {
                int yourAttack = this.AttackPoint + RollDice() * 2;
                int yourDefense = this.DefensePoint;

                int opponentAttack = enemy.AttackPoint + RollDice() * 2;
                int opponentDefense = enemy.DefensePoint;
                if (yourTurn)
                {
                    if (yourAttack > opponentDefense)
                    {
                        enemy.CurrentHP -= this.AttackPoint;
                        if (enemy.CurrentHP >= 0)
                        {
                            this.LevelUp();
                            this.DefeatedBoss = true;
                            enemy.Die();
                        }
                    }

                    yourTurn = false;
                }
                else
                {

                    if (opponentAttack > yourDefense)
                    {
                        this.CurrentHP -= enemy.AttackPoint;
                        if (this.CurrentHP >= 0)
                        {
                            this.Die();
                        }
                    }

                    yourTurn = true;
                }
            }
        }
    }
    class Monster : Entity, IEntityBase
    {
        public bool HaveKey { get; set; }
        public Monster(int Level)
        {
            int diceNumber = RollDice();
            if (diceNumber == 10)
            {
                this.Level = Level + 2;
            }
            else if (diceNumber >= 6)
            {
                this.Level = Level + 1;
            }
            else
            {
                this.Level = Level;
            }

            SetStats(this.Level);
        }
        private void SetStats(int Level)
        {
            MaxHP = 2 * Level * RollDice();
            CurrentHP = MaxHP;
            DefensePoint = Level / 2 * RollDice();
            AttackPoint = Level * RollDice();
        }
        public void Attack(Hero enemy)
        {
            bool yourTurn = true;
            while (this.Dead != true || enemy.Dead != true)
            {
                int yourAttack = this.AttackPoint + RollDice() * 2;
                int yourDefense = this.DefensePoint;

                int opponentAttack = enemy.AttackPoint + RollDice() * 2;
                int opponentDefense = enemy.DefensePoint;
                if (yourTurn)
                {
                    if (yourAttack > opponentDefense)
                    {
                        enemy.CurrentHP -= this.AttackPoint;
                        if (enemy.CurrentHP >= 0)
                        {
                            enemy.Die();
                        }
                    }

                    yourTurn = false;
                }
                else
                {

                    if (opponentAttack > yourDefense)
                    {
                        this.CurrentHP -= enemy.AttackPoint;
                        if (this.CurrentHP >= 0)
                        {
                            this.Die();
                        }
                    }

                    yourTurn = true;
                }
            }
        }
    }

    class Boss : Entity, IEntityBase
    {
        public Boss()
        {
            int diceNumber = RollDice();
            if (diceNumber == 10)
            {
                this.Level = Level + 2;
            }
            else if (diceNumber >= 6)
            {
                this.Level = Level + 1;
            }
            else
            {
                this.Level = Level;
            }

            SetStats(this.Level);
        }
        private void SetStats(int Level)
        {
            MaxHP = 2 * Level * RollDice() + RollDice();
            CurrentHP = MaxHP;
            DefensePoint = Level / 2 * RollDice() + RollDice() / 2;
            AttackPoint = Level * RollDice() + Level;
        }
        public void Attack(Hero enemy)
        {
            bool yourTurn = true;
            while (this.Dead != true || enemy.Dead != true)
            {
                int yourAttack = this.AttackPoint + RollDice() * 2;
                int yourDefense = this.DefensePoint;

                int opponentAttack = enemy.AttackPoint + RollDice() * 2;
                int opponentDefense = enemy.DefensePoint;
                if (yourTurn)
                {
                    if (yourAttack > opponentDefense)
                    {
                        enemy.CurrentHP -= this.AttackPoint;
                        if (enemy.CurrentHP >= 0)
                        {
                            enemy.Die();
                        }
                    }

                    yourTurn = false;
                }
                else
                {

                    if (opponentAttack > yourDefense)
                    {
                        this.CurrentHP -= enemy.AttackPoint;
                        if (this.CurrentHP >= 0)
                        {
                            this.Die();
                        }
                    }

                    yourTurn = true;
                }
            }
        }
    }
    internal class Program
    {
        public static void GiveKey(List<Monster> MonsterList)
        {
            Random rng = new Random();
            MonsterList[rng.Next(0, MonsterList.Count())].HaveKey = true;
        }
        static void Main(string[] args)
        {
            /* Test
            Hero hero = new Hero();

            Console.WriteLine("Hero datas: ");
            Console.WriteLine("\tHero MaxHP: " + hero.MaxHP);
            Console.WriteLine("\tHero CurrentHP: " + hero.CurrentHP);
            Console.WriteLine("\tHero Attack: " + hero.AttackPoint);
            Console.WriteLine("\tHero Defense: " + hero.DefensePoint);
            Console.WriteLine("\tHero Defeated boss: " + hero.DefeatedBoss);
            Console.WriteLine("\tHero have key: " + hero.HaveKey);
            Console.WriteLine("\tHero can go next level: " + hero.CanGoNextLevel());

            Console.WriteLine("After level up:");
            hero.LevelUp();

            Console.WriteLine("Hero datas: ");
            Console.WriteLine("\tHero MaxHP: " + hero.MaxHP);
            Console.WriteLine("\tHero CurrentHP: " + hero.CurrentHP);
            Console.WriteLine("\tHero Attack: " + hero.AttackPoint);
            Console.WriteLine("\tHero Defense: " + hero.DefensePoint);
            Console.WriteLine("\tHero Defeated boss: " + hero.DefeatedBoss);
            Console.WriteLine("\tHero have key: " + hero.HaveKey);
            Console.WriteLine("\tHero can go next level: " + hero.CanGoNextLevel());

            Console.WriteLine("After a battle:");
            Monster monster = new Monster(5);
            hero.Attack(monster);

            Console.WriteLine("Hero datas: ");
            Console.WriteLine("\tHero MaxHP: " + hero.MaxHP);
            Console.WriteLine("\tHero CurrentHP: " + hero.CurrentHP);
            Console.WriteLine("\tHero Attack: " + hero.AttackPoint);
            Console.WriteLine("\tHero Defense: " + hero.DefensePoint);
            Console.WriteLine("\tHero Defeated boss: " + hero.DefeatedBoss);
            Console.WriteLine("\tHero have key: " + hero.HaveKey);
            Console.WriteLine("\tHero can go next level: " + hero.CanGoNextLevel());

            Console.WriteLine("Monsters:");
            List<Monster> MonsterList = new List<Monster>();
            for (int i = 0; i < 6; i++)
            {
                MonsterList.Add(new Monster(2));
            }

            GiveKey(MonsterList);
            for (int i = 0; i < MonsterList.Count; i++)
            {
                Console.WriteLine("Mosnter key: " + MonsterList[i].HaveKey);
                Console.WriteLine("Mosnter Hp: " + MonsterList[i].CurrentHP);
                Console.WriteLine("Mosnter Attack: " + MonsterList[i].AttackPoint);
            }

            Console.ReadKey();
            */
        }
    }
}
