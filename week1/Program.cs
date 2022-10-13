namespace Opdracht
{
    using ExtensionMethods;
    using static System.Net.Mime.MediaTypeNames;

    public class Pad : Tekenbaar
    {
        public Coordinaat van { get; set; }
        public Coordinaat naar { get; set; }
        private float? lengteBerekend;
        public float Lengte()
        {
            if (!lengteBerekend.HasValue)
                lengteBerekend = (float)Math.Sqrt((van.x - naar.x) * (van.x - naar.x) + (van.y - naar.y) * (van.y - naar.y));
            return lengteBerekend.Value;
        }
        public void TekenConsole(ConsoleTekener t)
        {
            for (int i = 0; i < (int)Lengte(); i++)
            {
                float factor = i / Lengte();
                t.SchrijfOp(new Coordinaat((int)Math.Round(van.x + (naar.x - van.x) * factor), (int)Math.Round(van.y + (naar.y - van.y) * factor)), "#");
            }
            t.SchrijfOp(new Coordinaat((int)Math.Round(van.x + (naar.x - van.x) * .5), (int)Math.Round(van.y + (naar.y - van.y) * .5)), MyExtensions.metSuffixen((1000 * Lengte())));
        }
    }

    public struct Coordinaat
    {
        public readonly int x;
        public readonly int y;

        public Coordinaat(int x, int y)
        {
            this.x = System.Math.Abs(x);
            this.y = System.Math.Abs(y);
        }
    }

    class Kaart
    {
        public readonly int Breedte;
        public readonly int Hoogte;

        public List<KaartItem> itemLijst = new List<KaartItem>();

        public List<Pad> padLijst = new List<Pad>();

        public Kaart(int Breedte, int Hoogte)
        {
            this.Breedte = Breedte;
            this.Hoogte = Hoogte;
        }
        public void Teken(Tekener t)
        {
            foreach(var pad in padLijst)
            {
                pad.TekenConsole((ConsoleTekener)t);
            }
            foreach(var item in itemLijst)
            {
                item.TekenConsole((ConsoleTekener)t);
            }

        }
        public void VoegItemToe(KaartItem item)
        { 
            itemLijst.Add(item);
        }
        public void VoegPadToe(Pad Pad)
        { 
            padLijst.Add(Pad);
        }

    }
    interface Tekener
    {

        public void Teken(Tekenbaar t)
        {
          
        }
    }
    class KaartItem : Tekenbaar
    {
        private Coordinaat _locatie {get; set;}

        public KaartItem(Kaart kaart, Coordinaat _locatie)
        {
            if (_locatie.x < 0 || _locatie.y < 0)
                throw new Exception("Kan niet een locatie hebben in het negatieve!");
            this._locatie = _locatie;
        }

        public void TekenConsole(ConsoleTekener t)
        {
            t.SchrijfOp(_locatie, "A");
        }


    }


    public interface Tekenbaar
    {
        public void TekenConsole(ConsoleTekener t)
        { }
    }

    public class ConsoleTekener : Tekener
    {
        public void TekenConsole(ConsoleTekener t)
        { }

        public void SchrijfOp(Coordinaat Positie, string Text)
        {
            if (Positie.x < 0 || Positie.y < 0)
                throw new Exception("Kan niet tekenen in het negatieve!");
            Console.SetCursorPosition(Positie.x, Positie.y);
            Console.WriteLine(Text);
        }
    }

   class Starter
    {
        // public static void Main(string[] args)
        // {
        //     ConsoleTekener tekener = new ConsoleTekener();
        //     Kaart k = new Kaart(30, 30);
        //     Pad p1 = new Pad();
        //     p1.van = new Coordinaat(2, 5);
        //     p1.van = new Coordinaat(12, 30);
        //     k.VoegPadToe(p1);
        //     Pad p2 = new Pad();
        //     p2.van = new Coordinaat(26, 4);
        //     p2.naar = new Coordinaat(10, 5);
        //     k.VoegPadToe(p2);
        //     k.VoegItemToe(new KaartItem(k, new Coordinaat(15, 15)));
        //     k.VoegItemToe(new KaartItem(k, new Coordinaat(20, 15)));
        //     k.VoegItemToe(new KaartItem(k, new Coordinaat(5, 18)));
        //     k.Teken(tekener);
        //     tekener.SchrijfOp(new Coordinaat(0, k.Hoogte + 1), "Deze kaart is schaal 1:1000");
        //     System.Console.Read();

        //     GebruikerService gebruikerService = new GebruikerService();
        //     gebruikerService.Registreer("mijnmail", "mijnww");
        //     var fase1 = gebruikerService.Login("mijnmail", "mijnww");
        //     gebruikerService.Verifieer("mijnmail", "token");
        //     var fase3 = gebruikerService.Login("mijnmail", "mijnww");


        // }
    }
}

namespace ExtensionMethods
{
    public static class MyExtensions
    {
        public static string metSuffixen(float f)
        {
            string suffix;
            
            if(f>=1000000 && f<1000000000)
            {
                var n = Math.Round((double)f/1000000, 2).ToString();
                suffix = n+"M";
            }
            else if(f>=1000 && f<1000000)
            {
                var n = Math.Round((double)f/1000, 2).ToString();
                suffix = n+"K";
            }
            else if(f>=1000000000)
            {
                var n = Math.Round((double)f/1000000000, 2).ToString();
                suffix = n+"B";
            }
            else{
                suffix = f.ToString();
            }
            

            return suffix;
        }
    }
}