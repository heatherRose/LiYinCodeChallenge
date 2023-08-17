using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeChallenge
{
    public class Elevator
    {
        public int CurrentFloor { get; set; } = 1;
        public Direction Direction { get;set; } = Direction.Up;
        public State State { get; set; } = State.Stopped;

        public const int TravelTime = 3;

        public void MoveUp()
        {
            CurrentFloor++;
            Direction = Direction.Up;
            State = State.Moving;
        }

        public void MoveDown()
        {
            CurrentFloor--;
            Direction = Direction.Down;
            State = State.Moving;
        }

        public void Stop()
        {
            
            State = State.Stopped;
        }
    }

    public enum Direction
    {
        Up,
        Down,
    }

    public enum State
    {
        Moving,
        Stopped
    }
}


