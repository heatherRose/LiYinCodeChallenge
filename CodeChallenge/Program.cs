using CodeChallenge;


// Program.cs
using System;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

/**Before you run the application, you should know:
 * 
 * The console will keep outputting the current status of elevator, 
 * but it won't affect you command in the console(press number or number+"U"/"D")
 * 
 * The elevator only move between floor 1 and floor 20
 * 
 * Enjoy the elevator!!!
 */
namespace CodeChallenge
{
    class Program
    {
        static async Task Main(string[] args)
        {

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("elevator_log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            var elevator = new Elevator();
            var sensor = new Sensor(elevator);
            var requestManager = new RequestManager();
            
            // Elevator Start
            Console.WriteLine("Elevator is ready! (Press 'Q' to quit, Enter a number for a floor request, Add 'U' for an external up request, or 'D' for an external down request)");

            // Console Start in the background
            var inputTask = Task.Run(() => HandleUserInput(requestManager,sensor));

            while (true)
            {
                var (currentFloor, direction, state) = sensor.GetData();

                Log.Information($"{DateTime.Now}: Elevator is at floor {currentFloor}, direction {direction}, state {state}"); 

                //Tell Elevator what he should do next
                var (nextRequest,updatedirection) = requestManager.GetNextRequest(direction, currentFloor);
                if (nextRequest.HasValue)
                {

                    if (nextRequest.Value == 0 ||nextRequest.Value==currentFloor)
                    {
                        elevator.Stop();
                        elevator.Direction = updatedirection;
                        requestManager.RemoveRequest(currentFloor);
                        Console.WriteLine($"Stopped at Floor {currentFloor}");
                        Log.Information($"{DateTime.Now}: Elevator stops at floor {currentFloor}"); 
                        Thread.Sleep(1000);
                        
                    }
                    else if (nextRequest.Value > currentFloor)
                    {
                        
                        elevator.MoveUp();
                        Console.WriteLine($"Moving Up: Current Floor {elevator.CurrentFloor}");
                        Thread.Sleep(Elevator.TravelTime * 1000);
                    }
                    else if (nextRequest.Value < currentFloor)
                    {
                        elevator.MoveDown();
                        Console.WriteLine($"Moving Down: Current Floor {elevator.CurrentFloor}");
                        Thread.Sleep(Elevator.TravelTime * 1000);
                    }
                }

                if (inputTask.IsCompleted) break;

                
            }
            Log.CloseAndFlush();
        }

        static async Task HandleUserInput(RequestManager requestManager, Sensor sensor)
        {
            while (true)
            {
                var input = Console.ReadLine();
                if (input.ToUpper() == "Q") break; // End the simulation

                var (currentFloor, direction, state) = sensor.GetData();

                if (int.TryParse(input, out int floor))
                {
                    if (floor > 20)
                    {
                        Console.WriteLine("The elevator can only reach up to floor 20. Please enter a valid floor number.");
                        continue;
                    }
                    if (floor < 1)
                    {
                        Console.WriteLine("The elevator cannot go below floor 1. Please enter a valid floor number.");
                        continue;
                    }
                    requestManager.AddInternalRequest(floor, currentFloor, direction);
                    Console.WriteLine($"Internal request added for floor {floor}");
                }
                else if (input.EndsWith("U") || input.EndsWith("D"))
                {
                    int externalFloor;
                    if (int.TryParse(input.Substring(0, input.Length - 1), out externalFloor))
                    {
                        if (externalFloor > 20)
                        {
                            Console.WriteLine("The elevator can only reach up to floor 20. Please enter a valid floor number.");
                            continue;
                        }
                        if (externalFloor < 1)
                        {
                            Console.WriteLine("The elevator cannot go below floor 1. Please enter a valid floor number.");
                            continue;
                        }
                        Direction externalDirection = input.EndsWith("U") ? Direction.Up : Direction.Down;
                        requestManager.AddExternalRequest(externalFloor, externalDirection);
                        Console.WriteLine($"External request added for floor {externalFloor} direction {externalDirection}");
                    }
                    else
                    {
                        Console.WriteLine("Invalid request format. Please enter a number or a number followed by 'U' or 'D'.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid request format. Please enter a number or a number followed by 'U' or 'D'.");
                }
            }
        }


    }
}



