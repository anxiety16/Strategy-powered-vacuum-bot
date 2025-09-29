## Strategy-Powered Vacuum Bot (C# Console)

This project simulates a simple vacuum-cleaner robot that navigates a 2D grid and cleans dirt using pluggable strategies (Strategy Pattern). The visualization is ASCII-based and runs in the console.

The core implementation lives in `Mahinay.cs` and defines:
- **Map**: 2D grid with cells of type `Empty`, `Obstacle`, `Dirt`, `Cleaned`.
- **Robot**: Moves on the map, cleans the current spot, and delegates pathing to a strategy.
- **CleaningStrategy**: Interface for cleaning algorithms.
- **SnakeCleaningStrategy**: Left-to-right, then right-to-left “snake” sweep per row.
- **ReverseSpiralStrategy**: Cleans the perimeter inwards in a reverse spiral.

### Demo Behavior
- The program initializes a 10×7 map with a few obstacles and dirt spots.
- It first runs the Snake strategy to sweep the grid.
- After finishing, it prompts whether to also run the Reverse Spiral strategy.

Legend in the console display:
- `.` = Empty
- `#` = Obstacle
- `D` = Dirt
- `C` = Cleaned
- `R` = Robot

The display updates as the robot moves. There is a short delay to make motion visible.

### Requirements
- .NET SDK 6.0+ (recommended 8.0)
  - Verify with: `dotnet --version`

### Getting Started
1. Restore/build (from the project root):
   ```bash
   dotnet build
   ```
2. Run:
   ```bash
   dotnet run
   ```
   - If there are multiple projects/solutions, specify the project:
   ```bash
   dotnet run --project "C#.csproj"
   ```

On Windows PowerShell you can run the same commands. If you prefer compiling a single file directly and have `csc` available, you can try:
```powershell
csc Mahinay.cs
./Mahinay.exe
```
Note: Using the provided `.csproj` is recommended.

### Code Structure (high level)
- `Program` (outer class)
  - `Map`
    - `Celltype` enum: `Empty`, `Obstacle`, `Dirt`, `Cleaned`
    - Grid storage and helpers (`IsInBounds`, `IsDirt`, `IsObstacle`, `IsCleaned`, `SetCell`, `Clean`)
    - `Display(int robotX, int robotY)` for ASCII rendering
    - `Robot` with `Move`, `CleanCurrentSpot`, `StartCleaning`, `SetCleaningStrategy`
    - `CleaningStrategy` interface
    - `SnakeCleaningStrategy` and `ReverseSpiralStrategy` implementations
    - `Main(string[] args)` entry point: builds the map, runs strategies

### Customizing the Map
Edit `Mahinay.cs` in `Main` to adjust map size, obstacles, and dirt:
```csharp
Map map = new Map(10, 7);
map.SetCell(3, 1, Celltype.Obstacle);
map.SetCell(2, 0, Celltype.Dirt);
// Add or remove cells as desired
```

### Adding a New Strategy
Create a new class that implements `CleaningStrategy` and define your traversal in `Clean`:
```csharp
public class MyCustomStrategy : Map.CleaningStrategy
{
    public void Clean(Map map, Map.Robot robot)
    {
        // Implement movement and cleaning logic here
        // Example: iterate over all cells, skip obstacles, move and clean
        for (int y = 0; y < map.Height; y++)
        {
            for (int x = 0; x < map.Width; x++)
            {
                if (!map.IsObstacle(x, y))
                {
                    robot.Move(x, y);
                    robot.CleanCurrentSpot();
                }
            }
        }
    }
}
```
Then select it at runtime:
```csharp
var custom = new MyCustomStrategy();
robot.SetCleaningStrategy(custom);
robot.StartCleaning();
```

### Tips and Troubleshooting
- The screen is cleared on every render; if flicker is bothersome, reduce refreshes or the sleep delay in `Display`.
- Large maps will take longer to visualize; you can increase/decrease the delay in `Display` to speed it up.
- Ensure obstacles do not fully block off areas you intend to clean.

### License
You may use, modify, and distribute this code as you see fit. If you add substantial improvements, consider contributing them back.


