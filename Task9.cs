using System;

namespace Fabriq
{
    class Program
    {
        static void Main(string[] args)
        {
            Fabriq VoenTorg = Fabriq.CreateFabriq();

            Fabriq PorsheFactory = Fabriq.CreateFabriq();

            Console.WriteLine($"Is Singleton?: {VoenTorg == PorsheFactory}");

            var T28 = VoenTorg.CreateObject(new Trace<Tank>(new Tank(5, "T-28")));

            T28.Drive();
            T28.Shooting();
            IProjectile gold = new Projectile();
            T28.Reload(gold);
            T28.Shooting();

            Tank VK36 = new Tank(15, "VK 36.01", new Projectile("Explosive", 5));

            var V36C = VoenTorg.CreateObject(new Trace<Tank>(VK36).Clone() as Trace<Tank>);

            V36C.Drive();
            V36C.Shooting();
            V36C.Reload(gold);

            Truck Volvo = VoenTorg.CreateObject(new Trace<Truck>(new Truck("Volvo FH")));

            Volvo.Drive();

            Turret CM20 = VoenTorg.CreateObject(new Trace<Turret>(new Turret(5, "CM-20-ЗИФ")));

            IProjectile cm = new Projectile("Cumulative", 6);

            CM20.Reload(cm);
            CM20.Shooting();

            V36C.Reload(cm);
            V36C.Shooting();

            Console.ReadKey();
        }
    }

    // Pattern Singleton
    class Fabriq
    {
        static Fabriq instance;

        Fabriq() { }

        public static Fabriq CreateFabriq()
            => instance ?? (instance = new Fabriq()); 

        // Тут можно было бы объектами и через интерфейс, но зато не надо писать as Tank .. etc.
        // Хотелось бы сделать CreateTrace не публичным, чтобы создавать чертежи можно было только на фабрике
        // Но кроме наследования (уверен, что плохая идея) других идей у меня нет. 
        public Tank CreateObject(Trace<Tank> trace) => trace.CreateTrace();

        public Truck CreateObject(Trace<Truck> trace) => trace.CreateTrace();

        public Turret CreateObject(Trace<Turret> trace) => trace.CreateTrace();
        
    }

    // Pattern Prtotype
    interface IPrototype
    {
        object Clone();
    }

    class Trace <T> : IPrototype 
        where T : GameObject
    {

        T production;
        
        public Trace(T obj)
        {
            this.production = obj;
        }

        public object Clone() => new Trace<T>(production);

        public T CreateTrace() => production;

    }

    interface IDrive
    {
        void Drive();
    }

    interface IShoot
    {
        IProjectile Type { get; }

        void Shooting();

        void Reload(IProjectile newType);

    }

    abstract class GameObject
    {
        abstract public uint HitPoint { get; }

        abstract public string Name {get;}
    }

    class Tank : GameObject, IDrive, IShoot
    {
        public override uint HitPoint { get; }

        public override string Name { get; }

        public IProjectile Type { get; private set; }

        public Tank() : this (10, "Base Version of Tank", null) { }

        public Tank (uint hp, string name) : this (hp, name, null) { }

        public Tank(uint hp, string name, IProjectile proj)
        {
            HitPoint = (hp > 0) ? hp : 5;
            Name = name;
            Type = proj;
        }

        public void Drive() => Console.WriteLine($"{Name} move to you");

        public void Shooting()
        {
            if (Type != null)
                Type.Shoot();
            else
                Console.WriteLine($"Charge {Name} first");
        }

        public void Reload(IProjectile newType)
        {
            Type = newType;

            Console.WriteLine($"Load {newType.Name} progectile...");
        }

    }


    class Turret : GameObject, IShoot
    {
        public override uint HitPoint { get; }

        public override string Name { get; }

        public IProjectile Type { get; private set; }

        public Turret() : this(1, "Base Version of Turret", null) { }

        public Turret(uint hp, string name) : this(hp, name, null) { }

        public Turret(uint hp, string name, IProjectile proj)
        {
            HitPoint = (hp > 0) ? hp : 1;
            Name = name;
            Type = proj;
        }

        public void Shooting()
        {
            if (Type != null)
                Type.Shoot();
            else
                Console.WriteLine("Charge first");
        }


        public void Reload(IProjectile newType)
        {
            Type = newType;

            Console.WriteLine($"Load {newType.Name} progectile...");
        }

    }

    class Truck : GameObject, IDrive
    {
        public override uint HitPoint => 3;

        public override string Name { get; }

        public Truck() : this("Base Version of Truck") { }

        public Truck (string name)
        {
            Name = name;
        }

        public void Drive() => Console.WriteLine($"The {Name} is moving to its destination");
    }

    // Strategy Pattern
    interface IProjectile
    {
        string Name {get;}

        uint Damage {get;}

        void Shoot();
    }

    class Projectile : IProjectile
    {
        public string Name { get; }

        public uint Damage { get; }

        public Projectile() : this("Golden Standart", 1) { }

        public Projectile(string name, uint damage)
        {
            this.Name = name;
            this.Damage = damage;
        }

        public void Shoot()
        {
            Console.WriteLine($"Projectile {Name} inflicts {Damage} damage");
        }
    }

}
