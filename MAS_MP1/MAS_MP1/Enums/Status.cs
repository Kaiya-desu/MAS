using System.ComponentModel;

namespace MAS_MP1.Enums;

public enum Status
{
    // domyslnie kazdy z tych trzech klas będzie miał podstawowy status 
    // metoda do zmiany statusu
    // question
    [Description("Waiting for support")]
    WaitingForSupport,
    [Description("Resolving question")]
    Resolving,
    [Description("Problem solved")]
    Solved,
    
    // order
    [Description("We saved your order in our system!")]
    Realization,
    [Description("Collecting your order")]
    Collecting,
    [Description("Shipped")]
    Shipped,
    [Description("Done")]
    Done,
    
    // shipping type
    [Description("Paczkomaty najlepsze es")]
    Paczkomaty,
    [Description("Courier")]
    Courier,
    [Description("Poczta Polska")]
    PocztaPolska,
    [Description("Collect your package at the nearest store")]
    PickUpAtStore,
    
    //payment
    [Description("Paying by card before delivery")]
    Card,
    [Description("Paying by cash after delivery")]
    Cash,
    [Description("Waiting for Payment")]
    WaitingForPayment,
    [Description("Paid")]
    Paid,
}