namespace OrigineelDataLezer;

public class LengteBeperking
{
    public int? MinimaleLengte { get; set; }
    public int? MaximaleLengte { get; set; }
    public void VerwijderMinimumLengte()
    {
        MinimaleLengte = null;
    }
    public void VerwijderMaximumLengte()
    {
        MaximaleLengte = null;
    }
    public override string ToString()
    {
        return $"[LengteBeperking {{ MinimaleLengte = {MinimaleLengte}, MaximaleLengte = {MaximaleLengte} }}]";
    }
}

public class AttractieContext
{
    public List<Attractie> Attracties { get; set; } = new List<Attractie>();
    public void NieuweAttractie(Attractie attractie)
    {
        Attracties.Add(attractie);
    }
}

public abstract class Attractie
{
    public string Naam { get; set; }
    public string BouwDatum { get; set; }
    public abstract LengteBeperking LengteBeperking { get; }
    public Attractie(string Naam, string BouwDatum) { this.Naam = Naam; this.BouwDatum = BouwDatum; }
}

public class Draaimolen : Attractie
{
    public Draaimolen(string Naam, string BouwDatum, int DraaiSnelheid) : base(Naam, BouwDatum)
    {
        this.DraaiSnelheid = DraaiSnelheid;
    }
    public int DraaiSnelheid { get; set; }
    public override LengteBeperking LengteBeperking
    {
        get
        {
            return new LengteBeperking { MinimaleLengte = 100, MaximaleLengte = 200 };
        }
    }
}

public class Rollercoaster : Attractie
{
    public Rollercoaster(string Naam, string BouwDatum, int Lengte) : base(Naam, BouwDatum)
    {
        this.Lengte = Lengte;
    }
    public int Lengte { get; set; }
    public override LengteBeperking LengteBeperking
    {
        get
        {
            return new LengteBeperking { MinimaleLengte = 150 };
        }
    }
}

public interface AttractieFactory
{
    string Naam { get; }
    Attractie Maak(string Naam, string Datum, string[] extraArgumenten);
}

public class DraaimolenFactory : AttractieFactory
{
    public string Naam
    {
        get
        {
            return "Draaimolen";
        }
    }
    public Attractie Maak(string Naam, string Datum, string[] extraArgumenten)
    {
        return new Draaimolen(Naam, Datum, int.Parse(extraArgumenten[1]));
    }
}

public class RollercoasterFactory : AttractieFactory
{
    public string Naam
    {
        get
        {
            return "Rollercoaster";
        }
    }
    public Attractie Maak(string Naam, string Datum, string[] extraArgumenten)
    {
        return new Rollercoaster(Naam, Datum, int.Parse(extraArgumenten[0]));
    }
}

public static class AttractieDataLezer
{
    private static List<AttractieFactory> attractieFactories = new List<AttractieFactory>() { new DraaimolenFactory(), new RollercoasterFactory() };
    public static AttractieContext Lees()
    {
        var context = new AttractieContext();
        foreach (var regel in File.ReadAllLines("attractie_data.csv"))
        {
            if (regel.StartsWith("#") || regel == "")
                continue;
            var velden = regel.Split(",");
            var type = velden[2];
            if (velden[1] == "Kapot") continue;
            foreach (var attractieFactory in attractieFactories)
                if (attractieFactory.Naam == type)
                    context.NieuweAttractie(attractieFactory.Maak(velden[0], velden[3], velden[4..]));
        }
        return context;
    }
}
