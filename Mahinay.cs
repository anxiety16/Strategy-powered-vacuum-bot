using System;

public class Program
{
    public class Map
    {
        public enum Celltype
        {
            Empty,
            Obstacle,
            Dirt,
            Cleaned
        };
        private Celltype[,] _grid;
        public int Width { get; private set; }
        public int Height { get; private set; }

        public Map(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            _grid = new Celltype[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    _grid[x, y] = Celltype.Empty;
                }
            }
        }
        public bool IsinBounds(int x, int y)
        {
            return x >= 0 && x < this.Width && y >= 0 && y < this.Height;
        }
        public bool IsDirt(int x, int y)
        {
            if (!IsinBounds(x, y)) return false;
            return _grid[x, y] == Celltype.Dirt;
        }
        public bool IsObstacle(int x, int y)
        {
            if (!IsinBounds(x, y)) return false;
            return _grid[x, y] == Celltype.Obstacle;
        }
        public bool IsCleaned(int x, int y)
        {
            if (!IsinBounds(x, y)) return false;
            return _grid[x, y] == Celltype.Cleaned;
        }
        public void SetCell(int x, int y, Celltype type = Celltype.Empty)
        {
            if (IsinBounds(x, y))
            {
                _grid[x, y] = type;
            }
        }
        public void Clean(int x, int y)
        {
            if (IsinBounds(x, y))
            {
                _grid[x, y] = Celltype.Cleaned;
            }
        }
        public void Display(int robotX, int robotY)
        {
            Console.Clear();
            Console.WriteLine("Vacuum Cleaner Robot Simulator");
            Console.WriteLine("-------------------------------");
            Console.WriteLine("Legends: .=Empty, #=Obstacles, R=Robot,  D=Dirt, C=Cleaned");

            for (int y = 0; y < this.Height; y++)
            {
                for (int x = 0; x < this.Width; x++)
                {
                    if (x == robotX && y == robotY)
                    {
                        Console.Write("R ");
                    }
                    else
                    {
                        switch (_grid[x, y])
                        {
                            case Celltype.Empty:
                                Console.Write(". ");
                                break;
                            case Celltype.Obstacle:
                                Console.Write("# ");
                                break;
                            case Celltype.Dirt:
                                Console.Write("D ");
                                break;
                            case Celltype.Cleaned:
                                Console.Write("C ");
                                break;
                        }
                    }
                }
                Console.WriteLine();
            }
            System.Threading.Thread.Sleep(200); 
        }

        public class Robot
        {
            private readonly Map _map;
            private CleaningStrategy _cleaningStrategy;
            public int X { get; set; }
            public int Y { get; set; }

            public Robot(Map map, CleaningStrategy initialStrategy) 
            {
                this._map = map;
                this._cleaningStrategy = initialStrategy;
                this.X = 0;
                this.Y = 0;
            }
            public bool Move(int newX, int newY)
            {
                if (_map.IsinBounds(newX, newY) && !_map.IsObstacle(newX, newY))
                {
                    this.X = newX;
                    this.Y = newY;

                    _map.Display(this.X, this.Y);
                    return true;
                }
                return false;
            }
            public void CleanCurrentSpot()
            {
                if (_map.IsDirt(this.X, this.Y))
                {
                    _map.Clean(this.X, this.Y);
                }
            }
            public void StartCleaning()
            {
                Console.WriteLine("Starting cleaning process...");
                _cleaningStrategy.Clean(_map, this);
                Console.WriteLine("Cleaning completed!");
            }
            public void SetCleaningStrategy(CleaningStrategy strategy)
            {
                this._cleaningStrategy = strategy;
            }
        }

        public interface CleaningStrategy
        {
            void Clean(Map map, Robot robot);
        }

        public class SnakeCleaningStrategy : CleaningStrategy
        {
            public void Clean(Map map, Robot robot)
            {
                int direction = 1;
                for (int y = 0; y < map.Height; y++)
                {
                    int startX = direction == 1 ? 0 : map.Width - 1; 
                    int endX = direction == 1 ? map.Width : -1; 
                    for (int x = startX; x != endX; x += direction)
                    {
                        robot.Move(x, y);
                        robot.CleanCurrentSpot();
                    }
                   direction *= -1; 
                }
            } 
        }

        public class ReverseSpiralStrategy : CleaningStrategy
        {
            public void Clean(Map map, Robot robot)
            {
                int left = 0, right = map.Width - 1;
                int top = 0, bottom = map.Height - 1;

                while (left <= right && top <= bottom)
                {
           
                    for (int x = right; x >= left; x--)
                    {
                        robot.Move(x, bottom);
                        robot.CleanCurrentSpot();
                    }
                    bottom--;

                    for (int y = bottom; y >= top; y--)
                    {
                        robot.Move(left, y);
                        robot.CleanCurrentSpot();
                    }
                    left++;

            
                    if (top <= bottom)
                    {
                        for (int x = left; x <= right; x++)
                        {
                            robot.Move(x, top);
                            robot.CleanCurrentSpot();
                        }
                        top++;
                    }

              
                    if (left <= right)
                    {
                        for (int y = top; y <= bottom; y++)
                        {
                            robot.Move(right, y);
                            robot.CleanCurrentSpot();
                        }
                        right--;
                    }
                }
            }
        }

        public static void Main(string[] args)
        {
            Map map = new Map(10, 7);

            
            map.SetCell(3, 1, Celltype.Obstacle);
            map.SetCell(3, 2, Celltype.Obstacle);
            map.SetCell(3, 3, Celltype.Obstacle);
            map.SetCell(2, 0, Celltype.Dirt);
            map.SetCell(4, 2, Celltype.Dirt);
            map.SetCell(5, 5, Celltype.Dirt);
            map.SetCell(0, 6, Celltype.Dirt);

            
            var snakeStrategy = new SnakeCleaningStrategy();
            Robot robot = new Robot(map, snakeStrategy);

            robot.StartCleaning();

            
            Console.Write("\nDo you want to run Reverse Spiral Strategy? (1 = Yes, 0 = No): ");
            string input = Console.ReadLine();

            if (input == "1")
            {
                var reverseSpiral = new ReverseSpiralStrategy();
                robot.SetCleaningStrategy(reverseSpiral);
                robot.StartCleaning();
            }
            else
            {
                Console.WriteLine("Exiting program. Goodbye!");
            }
        }
    }
}
