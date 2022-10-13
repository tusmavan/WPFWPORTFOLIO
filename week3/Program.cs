//using OrigineelDataLezer;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using FunctioneelDataLezer;

class MainKlasse {
    private static int Engheid2(Attractie attractie)
    {
        switch (attractie)
        {
            case Draaimolen:
                var draaimolen = (Draaimolen)attractie;
                return 5+draaimolen.DraaiSnelheid;
                break;

            case Rollercoaster:
                var rollercoaster = (Rollercoaster)attractie;
                var lengte = rollercoaster.Lengte / 100;
                if (lengte > 100) return 100;
                return lengte;
                break;
                
        }
        return 0;
    }
    private static int Engheid(Attractie attractie) => attractie switch
    {
        Draaimolen => ((Draaimolen)attractie).DraaiSnelheid+5,
        Rollercoaster when ((Rollercoaster)attractie).Lengte/100 > 100 => 100,
        Rollercoaster => ((Rollercoaster)attractie).Lengte/100
    };
    public static void Main(string[] args) {
        foreach (var attractie in AttractieDataLezer.Lees().Attracties) {
            Console.WriteLine(attractie.Naam + " uit " + attractie.BouwDatum + " [" + attractie.LengteBeperking + "]");
            Console.WriteLine("Engheidsfactor: " + Engheid(attractie));
        }
    }
}
