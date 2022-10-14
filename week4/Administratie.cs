using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.EntityFrameworkCore;
using Opdracht;
using System.ComponentModel.DataAnnotations.Schema;

namespace Administratie
{
    abstract class Rapport
    {
        public abstract string Naam();
        public abstract Task<string> Genereer();
        public async Task VoerUit() => await File.WriteAllTextAsync(Naam() + ".txt", await Genereer());
        public async Task VoerPeriodiekUit(Func<bool> stop)
        {
            while (!stop())
            {
                await VoerUit();
                await Task.Delay(1000);
            }
        }
    }

    class DemografischRapport : Rapport
    {
        private DatabaseContext context;
        public DemografischRapport(DatabaseContext context) => this.context = context;
        public override string Naam() => "Demografie";
        public override async Task<string> Genereer()
        {
            string ret = "Dit is een demografisch rapport: \n";
            ret += $"Er zijn in totaal {await AantalGebruikers()} gebruikers van dit platform (dat zijn gasten en medewerkers)\n";
            var dateTime = new DateTime(2000, 1, 1);
            ret += $"Er zijn {await AantalSinds(dateTime)} bezoekers sinds {dateTime}\n";
            if (await AlleGastenHebbenReservering())
                ret += "Alle gasten hebben een reservering\n";
            else
                ret += "Niet alle gasten hebben een reservering\n";
            ret += $"Het percentage bejaarden is {await PercentageBejaarden()}\n";

            ret += $"De oudste gast heeft een leeftijd van {await HoogsteLeeftijd()} \n";

            ret += "De verdeling van de gasten per dag is als volgt: \n";
            /*var dagAantallen = await VerdelingPerDag();
            var totaal = dagAantallen.Select(t => t.aantal).Max();
            foreach (var dagAantal in dagAantallen)
                ret += $"{dagAantal.dag}: {new string('#', (int)(dagAantal.aantal / (double)totaal * 20))}\n";*/

            //ret += $"{await FavorietCorrect()} gasten hebben de favoriete attractie inderdaad het vaakst bezocht. \n";

            return ret;
        }
        private async Task<int> AantalGebruikers() => context.Gebruikers.Count();
        private async Task<bool> AlleGastenHebbenReservering() => context.Gasten.All(b => b.heeftReservering());
        private async Task<int> AantalSinds(DateTime sinds) => context.Gasten.Count(g => g.EersteBezoek > sinds);
        private async Task<Gast> GastBijEmail(string email) => context.Gasten.SingleOrDefault(g => g.Email == email) ?? null;
        private async Task<Gast?> GastBijGeboorteDatum(DateTime d) => context.Gasten.SingleOrDefault(g => g.GeboorteDatum == d);
        private async Task<double> PercentageBejaarden() => context.Gasten.Count(g => EF.Functions.DateDiffDay(g.GeboorteDatum, DateTime.Now) / 365.25 > 80) / context.Gasten.Count();
        private async Task<int> HoogsteLeeftijd() =>  Convert.ToInt32(EF.Functions.DateDiffDay(context.Gasten.OrderBy(i => i.GeboorteDatum).First().GeboorteDatum, DateTime.Now) / 365.25)  ;
        private async Task<(string dag, int aantal)[]> VerdelingPerDag()
        {
            var ding = context.Gasten.GroupBy(g => g.EersteBezoek.DayOfWeek.ToString()).ToList();

            var dn = context.Gasten.DistinctBy(g => g.EersteBezoek.DayOfWeek.ToString()).ToList();

            var omg = context.Gasten.Count(g.EersteBezoek).Groupby()
            
        }


        //private async Task<int> FavorietCorrect() => /* ... */;
    }


    public class MainClass
    {
        private static async Task<T> Willekeurig<T>(DbContext c) where T : class => await c.Set<T>().OrderBy(r => EF.Functions.Random()).FirstAsync();
        public static async Task Main(string[] args)
        {
            Random random = new Random(1);
            using (DatabaseContext c = new DatabaseContext())
            {
                c.Attracties.RemoveRange(c.Attracties);
                c.Gebruikers.RemoveRange(c.Gebruikers);
                c.Gasten.RemoveRange(c.Gasten);
                c.Medewerkers.RemoveRange(c.Medewerkers);
                c.Reserveringen.RemoveRange(c.Reserveringen);
                c.Onderhoud.RemoveRange(c.Onderhoud);

                c.SaveChanges();

                foreach (string attractie in new string[] { "Reuzenrad", "Spookhuis", "Achtbaan 1", "Achtbaan 2", "Draaimolen 1", "Draaimolen 2" })
                    c.Attracties.Add(new Attractie(attractie));

                c.SaveChanges();

                for (int i = 0; i < 40; i++)
                    c.Medewerkers.Add(new Medewerker($"medewerker{i}@mail.com"));
                c.SaveChanges();

                for (int i = 0; i < 10000; i++)
                {
                    var geboren = DateTime.Now.AddDays(-random.Next(36500));
                    var nieuweGast = new Gast($"gast{i}@mail.com") { GeboorteDatum = geboren, EersteBezoek = geboren + (DateTime.Now - geboren) * random.NextDouble(), Credits = random.Next(5) };
                    if (random.NextDouble() > .6)
                        nieuweGast.heeftFavoriet = await Willekeurig<Attractie>(c);
                    c.Gasten.Add(nieuweGast);
                }
                c.SaveChanges();

                for (int i = 0; i < 10; i++)
                    (await Willekeurig<Gast>(c)).Begeleidt = await Willekeurig<Gast>(c);
                c.SaveChanges();


                Console.WriteLine("Finished initialization");

                Console.Write(await new DemografischRapport(c).Genereer());
                Console.ReadLine();
            }
        }
    }

    class DatabaseContext : DbContext
    {
        public DbSet<Gast> Gasten { get; set; }
        public DbSet<Medewerker> Medewerkers { get; set; }
        public DbSet<Attractie> Attracties { get; set; }
        public DbSet<Gebruiker> Gebruikers { get; set; }
        public DbSet<Onderhoud> Onderhoud { get; set; }
        public DbSet<Reservering> Reserveringen { get; set; }

        //public Task<bool> Boek(Gast g, Attractie a, DateTimeBereik d) => ;

        protected void OnModelCreating(ModelBuilder builder) { }

        protected override void OnConfiguring(DbContextOptionsBuilder builder) => builder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=WPFW;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
    }

    public class Onderhoud
    {
        public string Probleem { get; set; }
        private DateTimeBereik dateTimeBereik;
        private Attractie Attractie { get; set; }
        private List<Medewerker>? Coordineert;
        private List<Medewerker>? Doet;
    }

    public class Reservering
    {
        private Gast? gast;
        private Attractie? Attractie { get; set; }
        private DateTimeBereik dateTimeBereik;
    }

    class Attractie
    {
        public string Naam;
        public List<Onderhoud>? onderhouds { get; set; }
        public List<Gast>? gasten { get; set; }
        public List<Reservering>? reserveringen { get; set; }

        public Attractie(string naam)
        {
            Naam = naam;
        }

        //public Task<bool> OnderhoudBezig(DatabaseContext c) =>;

        //public Task<bool> Vrij(DatabaseContext c, DateTimeBereik d) =>;
    }

    class Gebruiker
    {
        public string Email;

        public Gebruiker(string email)
        {
            Email = email;
        }
    }
    class Medewerker : Gebruiker
    {
        [InverseProperty("Coordinator")]
        private List<Onderhoud>? Coordineert;
        [InverseProperty("Voert uit")]
        private List<Onderhoud>? Doet;
        public Medewerker(string email) : base(email)
        {
        }
    }

    [Owned]
    class Coordinate
    {
        public int X;
        public int Y;
    }

    class GastInfo
    {
        public string LaatstBezochteUrl;
        public Gast gast;
        public Coordinaat coordinaat;
    }

    class Gast : Gebruiker
    {
        public int Credits;
        public DateTime GeboorteDatum;
        public DateTime EersteBezoek;
        public Gast? Begeleidt;
        public List<Reservering> Reserveringen;
        public bool heeftReservering() => !Reserveringen.Count().Equals(0);
        public Attractie? heeftFavoriet;
        public GastInfo gastInfo;

        public Gast(string email) : base(email)
        {

        }


    }

    [Owned]
    class DateTimeBereik
    {
        public DateTime Begin;
        public DateTime? Eind;

        public bool Eindigt() { return Eind > DateTime.Now; }

        public bool Overlapt(DateTimeBereik that)
        {
            if (this.Begin > that.Begin && this.Begin < that.Eind) return true;
            else if (that.Begin > this.Begin && that.Begin < this.Eind) return true;
            else return false;
        }

    }
}