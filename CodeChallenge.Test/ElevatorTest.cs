using Serilog;

namespace CodeChallenge.Test
{
    [TestClass]
    public class ElevatorTest
    {

        [TestMethod]
        public void TestSensorData()
        {
            var elevator = new Elevator();
            var sensor = new Sensor(elevator);

            var data = sensor.GetData();

            Assert.AreEqual(1, data.currentFloor);
            Assert.AreEqual(Direction.Up, data.direction);
            Assert.AreEqual(State.Stopped, data.state);
        }

        [TestMethod]
        public void TestElevator_GoToFloor10_PressInternal2_StopAt2()
        {
            var elevator = new Elevator { CurrentFloor = 10 };
            var requestManager = new RequestManager();
            var sensor = new Sensor(elevator);
            
            requestManager.AddInternalRequest(2, 10, Direction.Down);

            while (true)
            {
                var (currentFloor, direction, state) = sensor.GetData();

                var (nextRequest, updatedirection) = requestManager.GetNextRequest(direction, currentFloor);
                if (nextRequest.HasValue)
                {


                    if (nextRequest.Value == 0 || nextRequest.Value == currentFloor)
                    {

                        elevator.Stop();
                        elevator.Direction = updatedirection;
                        requestManager.RemoveRequest(currentFloor);
                        Thread.Sleep(1000);
                        break;

                    }
                    else if (nextRequest.Value > currentFloor)
                    {
                        elevator.MoveUp();
                        Thread.Sleep(Elevator.TravelTime * 1000);
                    }
                    else if (nextRequest.Value < currentFloor)
                    {
                        elevator.MoveDown();
                        Thread.Sleep(Elevator.TravelTime * 1000);
                    }

                }
            }

            Assert.AreEqual(2, elevator.CurrentFloor);
        }


        [TestMethod]
        public void TestElevator_GoToFloor8_PressInternal2_StopAt2_GoToFloor5()
        {
            var elevator = new Elevator { CurrentFloor = 8 };
            var requestManager = new RequestManager();
            var sensor = new Sensor(elevator);

            requestManager.AddInternalRequest(2, 10, Direction.Down);

            var addRequestTask = Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(15));
                var (currentFloor, direction, _) = sensor.GetData();
                requestManager.AddInternalRequest(5, currentFloor, direction);
            });
            bool stopBefore = false;

            while (true)
            {
                var (currentFloor, direction, state) = sensor.GetData();

                var (nextRequest, updatedirection) = requestManager.GetNextRequest(direction, currentFloor);
                if (nextRequest.HasValue)
                {


                    if (nextRequest.Value == 0 || nextRequest.Value == currentFloor)
                    {

                        elevator.Stop();
                        elevator.Direction = updatedirection;
                        requestManager.RemoveRequest(currentFloor);
                        Thread.Sleep(1000);
                        if (!stopBefore)
                        {
                            stopBefore=true;
                            continue;
                        }
                        break;

                    }
                    else if (nextRequest.Value > currentFloor)
                    {
                        elevator.MoveUp();
                        Thread.Sleep(Elevator.TravelTime * 1000);
                    }
                    else if (nextRequest.Value < currentFloor)
                    {
                        elevator.MoveDown();
                        Thread.Sleep(Elevator.TravelTime * 1000);
                    }

                }
            }

            Assert.AreEqual(5, elevator.CurrentFloor);
        }

    }
}