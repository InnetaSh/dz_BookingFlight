

using System;

class Program
{
    static void Main()
    {

        var seatsAB11 = new List<Seat>
        {
            new Seat("Эконом", 100),
            new Seat("Эконом", 100),
            new Seat("Бизнес", 200),
            new Seat("Бизнес", 200),
            new Seat("Первый класс", 300)
        };

        var flightAB11 = new Flight("AB11", DateTime.Now.AddHours(2), seatsAB11, 20);

        var seatsAB12 = new List<Seat>
        {
            new Seat("Эконом", 150),
            new Seat("Эконом", 150),
            new Seat("Бизнес", 200),
            new Seat("Бизнес", 200),
            new Seat("Первый класс", 500),
            new Seat("Первый класс", 500)
        };

        var flightAB12 = new Flight("AB12", DateTime.Now.AddHours(2), seatsAB12, 0);

        var flights = new List<Flight>();
        flights.Add(flightAB11);
        flights.Add(flightAB12);




        Menu(flights);
    }

    static void Menu(List<Flight> flights)
    {
        string name = "";
        Console.WriteLine("Введите имя пассажира");
        name = Console.ReadLine();

        Console.WriteLine("Имеете премиум подписку?(1 - true, 2 - false)");
        int input;
        while (!Int32.TryParse(Console.ReadLine(), out input) || input < 0 || input > 2)
        {
            Console.WriteLine("Неверный ввод. Введите 1 или 2:");
        }
        bool premium = false;
        if (input == 1)
            premium = true;


        User user = input == 1 ? new PremiumUser(name) : new User(name); if (premium == true)
            Console.WriteLine($"Пользователь {user.Name} зарегистрирован. Выберете рейс:");
        foreach (var flight in flights)
        {
            Console.WriteLine($"- {flight.FlightNumber}");
        }




        Console.WriteLine("Введите выбраный Вами рейс");
        var flightNumber = Console.ReadLine();

        var selectedFlight = flights.FirstOrDefault(f => f.FlightNumber.Equals(flightNumber, StringComparison.OrdinalIgnoreCase));

        if (selectedFlight == null)
        {
            Console.WriteLine("Рейс не найден.");

            Console.WriteLine("Забронировать еще пассажира на рейс? (Y/N)");

            string inputStr = Console.ReadLine().ToUpper();
            while (inputStr.ToUpper() != "Y" && inputStr.ToUpper() != "N")
            {
                Console.WriteLine("Не верный ввод.Забронировать еще пассажира на рейс? (Y/N)");
                inputStr = Console.ReadLine().ToUpper();
            }

            if (inputStr == "Y")
            {
                Console.WriteLine("----------------------------------------------------");
                Menu(flights);
            }
            else
                return;
        }
        selectedFlight.ShowAvailableSeats();





        Console.WriteLine("Введите класс места для бронирования (1- Эконом, 2 - Бизнес, 3 - Первый класс):");
        while (!Int32.TryParse(Console.ReadLine(), out input) || input < 0 || input > 3)
        {
            Console.WriteLine("Неверный ввод. Введите 1,2 или 3:");
        }
        string seatClass = "";
        switch (input)
        {
            case 1:
                seatClass = "Эконом";
                break;
            case 2:
                seatClass = "Бизнес";
                break;
            case 3:
                seatClass = "Первый класс";
                break;
        }


        var availableSeat = selectedFlight.Seats.FirstOrDefault(s => s.Class.Equals(seatClass, StringComparison.OrdinalIgnoreCase) && !s.IsBooked);

        if (availableSeat == null)
        {
            Console.WriteLine("Места данного класса нет в наличии.");


            Console.WriteLine("Забронировать еще пассажира на рейс? (Y/N)");

            string inputStr = Console.ReadLine().ToUpper();
            while (inputStr.ToUpper() != "Y" && inputStr.ToUpper() != "N")
            {
                Console.WriteLine("Не верный ввод.Забронировать еще пассажира на рейс? (Y/N)");
                inputStr = Console.ReadLine().ToUpper();
            }

            if (inputStr == "Y")
            {
                Console.WriteLine("----------------------------------------------------");
                Menu(flights);
            }
            else
                return;
        }

        user.BookSeat(selectedFlight, availableSeat);
        Console.WriteLine("----------------------------------------------------");
        selectedFlight.ShowAvailableSeats();


        Console.WriteLine("Забронировать еще пассажира на рейс? (Y/N)");

        string flag = Console.ReadLine().ToUpper();
        while (flag.ToUpper() != "Y" && flag.ToUpper() != "N")
        {
            Console.WriteLine("Не верный ввод.Забронировать еще пассажира на рейс? (Y/N)");
            flag = Console.ReadLine().ToUpper();
        }

        if (flag == "Y")
        {
            Console.WriteLine("----------------------------------------------------");
            Menu(flights);
        }
    }
}




//User: базовый класс для представления пользователей, наследуемый класс PremiumUser для премиум-пользователей.
public class User
{
    public string Name { get; private set; }

    public User(string name)
    {
        Name = name;
    }

    public virtual decimal CalculatePrice(decimal basePrice, Flight flight)
    {
        return flight.discountManager.ApplyDiscounts(basePrice, false);
    }

    public virtual void BookSeat(Flight flight, Seat seat)
    {
        if (seat.IsBooked)
        {
            Console.WriteLine("Место уже забронировано.");
            return;
        }

        decimal finalPrice = CalculatePrice(seat.Price, flight);
        flight.Book(seat);
        Console.WriteLine($"{Name} забронировал {seat.Class} место за {finalPrice}");
    }
}

public class PremiumUser : User
{
    public PremiumUser(string name) : base(name) { }

    public override decimal CalculatePrice(decimal basePrice, Flight flight)
    {
        return flight.discountManager.ApplyDiscounts(basePrice, true);
    }
}


//Flight: класс для представления рейса.
public class Flight
{
    public string FlightNumber { get; private set; }
    public DateTime DepartureTime { get; private set; }
    public List<Seat> Seats { get; private set; }

    public DiscountManager discountManager { get; private set; }

    public Flight(string flightNumber, DateTime departureTime, List<Seat> seats, int specialDiscount)
    {
        FlightNumber = flightNumber;
        DepartureTime = departureTime;
        Seats = seats;
        discountManager = new DiscountManager(specialDiscount);
    }

    public void Book(Seat seat)
    {
        seat.IsBooked = true;
    }

    public void ShowAvailableSeats()
    {
        var availableSeats = Seats.Where(s => !s.IsBooked);
        Console.WriteLine($"Доступные места на рейсе {FlightNumber}:");
        foreach (var seat in availableSeats)
        {
            Console.WriteLine($"- {seat.Class}: {seat.Price}");
        }
    }
}


//Seat: класс для представления мест в самолете.
public class Seat
{
    public string Class { get; private set; }
    public decimal Price { get; private set; }
    public bool IsBooked { get; set; }

    public Seat(string seatClass, decimal price)
    {
        Class = seatClass;
        Price = price;
        IsBooked = false;
    }
}


//Booking: класс для обработки бронирования.
public class Booking
{
    public User User { get; private set; }
    public Flight Flight { get; private set; }
    public Seat Seat { get; private set; }

    public Booking(User user, Flight flight, Seat seat)
    {
        User = user;
        Flight = flight;
        Seat = seat;
    }
}


//DiscountManager: класс для управления скидками и акциями.
public class DiscountManager
{
    public int specialDiscount { get; private set; }
    public DiscountManager(int discount)
    {
        specialDiscount = discount;
    }
    public decimal ApplyDiscounts(decimal basePrice, bool isPremium)
    {
        decimal finalPrice = basePrice;

        if (isPremium)
            finalPrice = 0.9m * basePrice;

        if (specialDiscount > 0)
            finalPrice -= finalPrice * specialDiscount / 100;

        return finalPrice;
    }
}











//. Основная задача — спроектировать и реализовать несколько классов, которые будут взаимодействовать друг с другом, и включить в систему элементы ООП: инкапсуляцию, полиморфизм и наследование.

//Пользователи:

//Каждый пользователь может зарегистрироваться в системе и бронировать билеты.
//Пользователи могут быть обычными или премиум (с различными скидками на бронирование).
//Премиум пользователи получают 10% скидку на все рейсы.
//Рейсы:

//Каждый рейс должен иметь уникальный номер, дату, время вылета, количество мест и их стоимость.
//Пользователь может забронировать место на рейс. После бронирования количество доступных мест уменьшается.
//Система должна учитывать, что на некоторых рейсах доступны несколько классов обслуживания: эконом, бизнес, первый класс. Цена на билеты зависит от класса.
//Места в самолете:

//Система должна поддерживать различные классы обслуживания с разными ценами.
//Каждое место должно быть связано с конкретным рейсом и классом (эконом, бизнес или первый класс).
//Пользователь может выбрать конкретное место на рейсе, если оно еще не забронировано.
//Скидки и акции:

//Система должна поддерживать возможность добавления специальных акций на рейсы (например, скидка 20% на все билеты рейса на определенную дату).
//Возможность объединения скидок: если премиум пользователь бронирует билет по акции, обе скидки должны применяться.
//Классы:

//User: базовый класс для представления пользователей, наследуемый класс PremiumUser для премиум-пользователей.
//Flight: класс для представления рейса.
//Seat: класс для представления мест в самолете.
//Booking: класс для обработки бронирования.
//DiscountManager: класс для управления скидками и акциями.


//Наследование и полиморфизм — реализуйте классы пользователей с поддержкой базовых функций бронирования, а также специальных функций для премиум пользователей.
//Инкапсуляция — скрывайте внутреннюю реализацию и работайте через методы классов.
//Интерактивная система — пользователь может вводить информацию через консоль (выбрать рейс, забронировать билет, просмотреть доступные места и т.д.).
//Подсчет стоимости билета — система должна корректно рассчитывать стоимость билета с учетом всех скидок и класса обслуживания.

