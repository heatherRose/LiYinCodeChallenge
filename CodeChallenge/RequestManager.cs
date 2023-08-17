using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// RequestManager.cs
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace CodeChallenge
{
    public class FloorRequest
    {
        public int Floor { get; set; }
        public DateTime Timestamp { get; set; }
    }
    public class RequestManager
    {
        private List<FloorRequest> InternalRequests { get; } = new List<FloorRequest>();
        private List<FloorRequest> ExternalRequestsUp { get; } = new List<FloorRequest>();
        private List<FloorRequest> ExternalRequestsDown { get; } = new List<FloorRequest>();


        public void AddInternalRequest(int floor, int currentFloor, Direction direction)
        {

            if (!InternalRequests.Any(r => r.Floor == floor))
                InternalRequests.Add(new FloorRequest { Floor = floor, Timestamp = DateTime.Now });

        }
        public void AddExternalRequest(int floor, Direction requestDirection)
        {
            if (requestDirection == Direction.Up && !ExternalRequestsUp.Any(r => r.Floor == floor))
            {
                ExternalRequestsUp.Add(new FloorRequest { Floor = floor, Timestamp = DateTime.Now });
            }
            else if (requestDirection == Direction.Down && !ExternalRequestsDown.Any(r => r.Floor == floor))
            {
                ExternalRequestsDown.Add(new FloorRequest { Floor = floor, Timestamp = DateTime.Now });
            }
        }


        public (int? NextRequest, Direction UpdatedDirection) GetNextRequest(Direction direction, int currentFloor)
        {

            DateTime currentTime = DateTime.Now;
            if (direction == Direction.Up)
            {
                var StopDownRequests = InternalRequests.Concat(ExternalRequestsUp)
                                                        .Where(f => f.Floor < currentFloor).OrderByDescending(f => f);

                var relevantUpRequests = InternalRequests.Concat(ExternalRequestsUp)
                                                        .Where(f => f.Floor >= currentFloor && currentTime.Subtract(f.Timestamp).TotalSeconds >= Elevator.TravelTime)
                                                        .OrderBy(f => f.Floor);
                var largestDownRequest = ExternalRequestsDown.Where(f => f.Floor > currentFloor);
                if (relevantUpRequests.Any())
                {
                    if (relevantUpRequests.Count() == 1 && currentFloor == relevantUpRequests.Max(r => r.Floor) && !largestDownRequest.Any())
                    {
                        direction = Direction.Down;
                        return (relevantUpRequests.First().Floor, direction);
                    }
                    return (relevantUpRequests.First().Floor, direction);
                }

                if (largestDownRequest.Any())
                    if (largestDownRequest.Max(r => r.Floor) > currentFloor)
                    {
                        direction = Direction.Down;
                        return (largestDownRequest.Max(r => r.Floor), direction);
                    }

                if (StopDownRequests.Count() > 0)
                {
                    direction = Direction.Down;
                    return (StopDownRequests.Max(r => r.Floor), direction);
                }
                return (0, direction);


            }
            else
            {
                var StopUpRequests = InternalRequests.Concat(ExternalRequestsDown)
                                                     .Where(f => f.Floor > currentFloor).OrderBy(f => f);

                var relevantDownRequests = InternalRequests.Concat(ExternalRequestsDown)
                                                           .Where(f => f.Floor <= currentFloor && currentTime.Subtract(f.Timestamp).TotalSeconds >= Elevator.TravelTime)
                                                           .OrderByDescending(f => f.Floor);
                var smallestUpRequest = ExternalRequestsUp.Where(f => f.Floor < currentFloor);

                if (relevantDownRequests.Any())
                {
                    if (relevantDownRequests.Count() == 1 && currentFloor == relevantDownRequests.Min(r => r.Floor) && !smallestUpRequest.Any())
                    {
                        direction = Direction.Up;
                        return (relevantDownRequests.First().Floor, direction);
                    }
                    return (relevantDownRequests.First().Floor, direction);
                }

                if (smallestUpRequest.Any())
                    if (smallestUpRequest.Min(r => r.Floor) < currentFloor)
                    {
                        direction = Direction.Up;
                        return (smallestUpRequest.Min(r => r.Floor), direction);
                    }

                if (StopUpRequests.Count() > 0)
                {
                    direction = Direction.Up;
                    return (StopUpRequests.Min(r => r.Floor), direction);
                }


                return (0, direction);

            }

        }

        public void RemoveRequest(int floor)
        {
            InternalRequests.RemoveAll(r => r.Floor == floor);
            ExternalRequestsUp.RemoveAll(r => r.Floor == floor);
            ExternalRequestsDown.RemoveAll(r => r.Floor == floor);
        }
    }

}




