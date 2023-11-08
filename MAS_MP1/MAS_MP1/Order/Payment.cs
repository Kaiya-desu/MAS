using MAS_MP1.Database;
using MAS_MP1.Enums;

namespace MAS_MP1.Order;

public class Payment
{
    // tworzy sie podczas tworzenia orderu (obiekt w db, nic wiecej)

    private Status TypeOfPayment;
    private DateTime? Date;
    private Status PaymentStatus;

    private int IDOrder;
    
    // zrobic kompozycje jak w UserScore
    public Payment(Status typeOfPayment, int id_order)
    {
        TypeOfPayment = typeOfPayment;
        Date = typeOfPayment != Status.Cash ? DateTime.Now : null;
        // w zaleznosci od typu płatnosci status w tabeli jest inny
        PaymentStatus = typeOfPayment == Status.Cash ? Status.WaitingForPayment : Status.Paid;
        IDOrder = id_order;
        AddPayment();
    }
    
    // dodanie do payment następuje przez Order, tak samo edycja Payment (po order ID)
    private void AddPayment()
    {
        Connection.Insert($"INSERT INTO Payment (Type_Of_Payment, Date, Status, Order_ID_Order) VALUES ('{TypeOfPayment}', '{Date}', '{PaymentStatus}', {IDOrder})");
    }


}