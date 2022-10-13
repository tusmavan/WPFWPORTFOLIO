namespace FunctioneelDataLezer;

using System;
using System.Collections.Immutable;

static class Record
{
    public static int getal = 0;
    public static int Getalis() => getal++;
}

static class Functies
{
    

    public static Func<(T1, T2), T3> Unsplat<T1, T2, T3>(Func<T1, T2, T3> f) => (ValueTuple<T1, T2> t1) => f(t1.Item1, t1.Item2);
    public static Func<T1, T2, T3> Splat<T1, T2, T3>(Func<(T1, T2), T3> f) => (T1 t1, T2 t2) => f((t1, t2));
    public static Func<T1, T3> Combineer<T1, T2, T3>(Func<T2, T3> f2, Func<T1, T2> f1) => (T1 t1) => f2(f1(t1));

    public static Func<TIn, TOutOut> CombineerOutputs<TIn, TOut, TOutOut>(Func<TIn, TOut> f1, Func<TIn, TOut> f2, Func<TOut, TOut, TOutOut> combiner) =>
        (TIn i) => combiner(f1(i), f2(i));
    
    public static Func<T1, (T2, T1)> TellerPrefix<T1, T2>(Func<T2> f) => (T1 t1) => (f(), t1);
    public static Func<int> Getgetal() => Record.Getalis;
}

static class Lezer<T>
{
    public static T[] Lees(string fileName, Func<int, string, T> Functie) =>
        ElkeRegel(
            Filter(
                Functies.Combineer(
                    (bool a) => !a,
                    Functies.CombineerOutputs<string, bool, bool>(
                        IsLegeRegel,
                        IsCommentaar,
                        (bool a, bool b) => a || b
                    )
                ),
                File.ReadAllLines(fileName)
            ),
            Functies.Combineer<string, (int, string), T>(
                Functies.Unsplat<int, string, T>(Functie),
                Functies.TellerPrefix<string, int>(MetNummer())
            )
        );

    public static Func<int> MetNummer() {
        return Functies.Getgetal();
    }

    public static string[] Filter(Func<string, bool> f, string[] args) => args switch {
        [] => new string[] {},
        [string s, ..] when f(s) => new string[] { s }.Concat(Filter(f, args[1..])).ToArray(),
        _ => Filter(f, args[1..])
    };

    public static T[] ElkeRegel(string[] regels, Func<string, T> functie) => regels switch {
        [] => new T[] {},
        _ => new T[] {functie(regels[0])}.Concat(ElkeRegel(regels[1..], functie)).ToArray()
    };
    
    
    private static bool IsLegeRegel(string s) => s.Trim() == "";
    private static bool IsCommentaar(string s) => s.StartsWith("#");
}

public readonly record struct LengteBeperking(int? MinimaleLengte, int? MaximaleLengte) {
    public LengteBeperking ZonderMinimum() => this with { MinimaleLengte = null };
    public LengteBeperking ZonderMaximum() => this with { MaximaleLengte = null };
}



readonly record struct AttractieContext(ImmutableList<Attractie> Attracties)
{
    public AttractieContext NieuweAttractie(Attractie a)
    {
        AttractieContext at = this with { Attracties = Attracties.Add(a) };
        return at;
    }
}

static class AttractieDataLezer
{
    private static Attractie?[] LeesUitBestand() => Lezer<Attractie?>.Lees("attractie_data.csv", 
        (int i, string regel) => regel.Split(",") switch {
            [string Naam, "Kapot", _, _, _, _] when Naam.StartsWith("") => 
                null,
            [string Naam, _, "Rollercoaster", string BouwDatum, string Lengte, _] => 
                new Rollercoaster(Naam, BouwDatum, int.Parse(Lengte)),
            [string Naam, _, "Draaimolen", string BouwDatum, _, string DraaiSnelheid] => 
                new Draaimolen(Naam, BouwDatum, int.Parse(DraaiSnelheid)),
            _ => throw new Exception("Leesfout!")
        } );
    private static AttractieContext Verzamel(Attractie?[] attracties) => attracties switch {
        [] => new AttractieContext { Attracties = ImmutableList<Attractie>.Empty }, 
        [null, ..] => Verzamel(attracties[1..]),
        [Attractie a, ..] => Verzamel(attracties[1..]).NieuweAttractie(a)
    };
    public static AttractieContext Lees() => Verzamel(LeesUitBestand());
}

public abstract class Attractie
{
    public string Naam;
    public string BouwDatum;

    public abstract LengteBeperking LengteBeperking { get; }
    
}

public class Draaimolen : Attractie
{
    public int DraaiSnelheid;

    public Draaimolen(string naam, string bouwDatum, int draai)
    {
        Naam = naam;
        BouwDatum = bouwDatum;
        DraaiSnelheid = draai;
    }
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
    public int Lengte;

    public Rollercoaster(string naam, string bouwDatum, int lengte)
    {
        Naam = naam;
        BouwDatum = bouwDatum;
        Lengte = lengte;
    }
    public override LengteBeperking LengteBeperking
    {
        get
        {
            return new LengteBeperking { MinimaleLengte = 150 };
        }
    }
}