namespace Opdracht
{
    public interface IGebruikerService
    {

        public IGebruikerContext context { get; set; }
        public IEmailService emailService { get; set; }

        public Gebruiker Registreer(string email, string ww)
        {

            context.NieuweGebruiker(email, ww);
            context.GetGebruiker(context.AantalGebruikers() - 1).geverifieerd = emailService.Email("Welkom", email);
            return context.GetGebruiker(context.AantalGebruikers() - 1);
        }
        public bool Login(string email, string ww)
        {
            if (context.GebruikerBestaatEnGeverifieerd(email, ww)) { return true; }
            return false;
        }
        public bool Verifieer(string email, string token)
        {
            var gebruiker = context.GetGebruikerByMail(email);
            if (gebruiker == null) return false;
            if (gebruiker.verificatieToken == null) return true;
            if (token == gebruiker.verificatieToken.token && gebruiker.verificatieToken.verloopDatum > DateTime.Now)
            {
                gebruiker.verificatieToken = null;
                return true;
            }
            return false;
        }
    }

    public class GebruikerService : IGebruikerService
    {
        public IGebruikerContext context { get; set; }
        public IEmailService emailService { get; set; }
        public GebruikerService()
        {
            context = new GebruikerContext();
            emailService = new EmailService();
        }

        public Gebruiker Registreer(string email, string ww) 
        {
            
            context.NieuweGebruiker(email, ww);
            context.GetGebruiker(context.AantalGebruikers() - 1).geverifieerd = emailService.Email("Welkom", email);
            return context.GetGebruiker(context.AantalGebruikers()-1);
        }
        public bool Login(string email, string ww) 
        { 
            if (context.GebruikerBestaatEnGeverifieerd(email, ww)){ return true; }
            return false;
        }
        public bool Verifieer(string email, string token)
        {
            var gebruiker = context.GetGebruikerByMail(email);
            if (gebruiker == null) return false;
            if (gebruiker.verificatieToken == null) return true;
            if (token == gebruiker.verificatieToken.token && gebruiker.verificatieToken.verloopDatum > DateTime.Now)
            {
                gebruiker.verificatieToken = null;
                return true;
            }
            return false;
        }
    }

    public interface IEmailService
    {
        public bool Email(string tekst, string adres) { return true; }
    }

    public interface IGebruikerContext
    {
        public static List<Gebruiker> gebruikersLijst = new List<Gebruiker>();
        public int AantalGebruikers() { return gebruikersLijst.Count(); }
        public Gebruiker GetGebruiker(int i) { return gebruikersLijst[i]; }
        public bool GebruikerBestaatEnGeverifieerd(string email, string ww)
        {
            foreach (Gebruiker gebruiker in gebruikersLijst) { if (email == gebruiker.Email && ww == gebruiker.Wachtwoord && gebruiker.Geverifieerd()) { return true; } }
            return false;
        }
        public void NieuweGebruiker(string email, string ww) { gebruikersLijst.Add(new Gebruiker(email, ww)); }

        public Gebruiker GetGebruikerByMail(string mail)
        {
            foreach (Gebruiker geb in gebruikersLijst)
            {
                if (mail == geb.Email) { return geb; }
            }
            return null;
        }

    }

    public class EmailService : IEmailService
    {
        public bool Email(string tekst, string adres) { return true; }
    }

    public class GebruikerContext : IGebruikerContext
    {
        public static List<Gebruiker> gebruikersLijst = new List<Gebruiker>();
        public int AantalGebruikers() { return gebruikersLijst.Count(); }
        public Gebruiker GetGebruiker(int i) { return gebruikersLijst[i]; }
        public bool GebruikerBestaatEnGeverifieerd(string email, string ww)
        {
            foreach (Gebruiker gebruiker in gebruikersLijst) { if (email == gebruiker.Email && ww == gebruiker.Wachtwoord && gebruiker.Geverifieerd()) { return true; } }
            return false;
        }
        public void NieuweGebruiker(string email, string ww) { gebruikersLijst.Add(new Gebruiker(email, ww)); }

        public Gebruiker GetGebruikerByMail(string mail)
        {
            foreach (Gebruiker geb in gebruikersLijst)
            {
                if (mail == geb.Email) { return geb; }
            }
            return null;
        }

    }

    public class Gebruiker
    {
        public string Email { get; set; }
        public string Wachtwoord { get; set; }
        public VerificatieToken? verificatieToken { get; set; }
        public bool geverifieerd = false;

        public Gebruiker(string email, string ww) { Email = email; Wachtwoord = ww; verificatieToken = new VerificatieToken("token", DateTime.Now.AddDays(3)); }

        public bool Geverifieerd() 
        {
            if (verificatieToken == null) return true; 
            return false;
        }
    }

    public class VerificatieToken
    {
        public string token { get; set; }
        public DateTime verloopDatum { get; set; }

        public VerificatieToken(string token, DateTime verloopDatum)
        {
            this.token = token;
            this.verloopDatum = verloopDatum;
        }
    }

    public class Gast : Gebruiker
    {
        public Gast(string email, string ww) : base(email, ww)
        {
        }

        public int Rating { get; set; }
        public int Boete { get; set; }
        public DateTime Geboortedatum { get; set; }

        public void Bezoek()
        {

        }

        public void VIPBezoek() { }

        public void GeefStraf(string daden) { }
    }

    public class Planner : Gebruiker
    {
        public Planner(string email, string ww) : base(email, ww)
        {
        }
    }

    public class Medewerker : Gebruiker
    {
        public Medewerker(string email, string ww) : base(email, ww)
        {
        }
    }

    public class Testen
    {
        /*static async Task Main(string[] args)
        {  
            string email;  
            string ww;
            var service = new GebruikerService();
            Console.WriteLine("Welkom");
            string key = null;
            while (key != "q")
            {
                Console.WriteLine("r) Registreren");
                Console.WriteLine("i) Inloggen");
                Console.WriteLine("v) Verifieer mail");
                Console.WriteLine("q) Quit");
                key = Console.ReadLine();
                if (key == "r")
                {
                    Console.WriteLine("Hier kan je Registreren");
                    Console.WriteLine("Email:");
                    email = Console.ReadLine();
                    Console.WriteLine("Wachtwoord:");
                    ww = Console.ReadLine();
                    service.Registreer(email, ww);
                    Console.WriteLine("Geregistreerd");
                }
                else if (key == "i")
                {
                    Console.WriteLine("Hier kan je Inloggen");
                    Console.WriteLine("Email:");
                    email = Console.ReadLine();
                    Console.WriteLine("Wachtwoord:");
                    ww = Console.ReadLine();
                    if (service.Login(email, ww)) Console.WriteLine("Succesvol ingelogd");
                    else Console.WriteLine("inloggen mislukt. Heb je je mail geverifieerd?");
                }
                else if (key == "v")
                {
                    Console.WriteLine("Hier kan je Verifieren");
                    Console.WriteLine("Email:");
                    email = Console.ReadLine();
                    Console.WriteLine("Token:(is 'token')");
                    ww = Console.ReadLine();
                    if (service.Verifieer(email, ww)) Console.WriteLine("Succesvol Geverifieerd");
                    else Console.WriteLine("verifieren mislukt");
                }
                    
                else Console.WriteLine("Ongeldige invoer!");
            }

        }*/
    }
}
