using System;
using System.Threading;
using System.Collections.Generic;

class Snake
{
	//0 = UP ^
	//1 = RIGHT >
	//2 = DOWN v
	//3 = LEFT <
	private int direction, bannedDirection;
	private int length;

	private bool playing;

	private Location apple;
	private LinkedList<Location> snake;

	public static void Main()
	{
		Snake snake = new Snake();

		snake.Init();
		snake.Loop();

		snake.Exit();

	}

	public void Init()
	{
		Console.CursorVisible = false;

		WriteSeparator();
		Console.WriteLine("                         Bienvenue sur Snake console !");
		Console.WriteLine("                    Jeu par Alexander Winter & Alexi Tessier");
		WriteSeparator();
		Console.WriteLine();

		Thread.Sleep(2000);

		direction = 0;
		bannedDirection = getBannedDirection(direction);
		length = 3;
		playing = true;

		snake = new LinkedList<Location>();
		snake.AddLast(new Location(39, 20));
		snake.AddLast(new Location(39, 21));
		snake.AddLast(new Location(39, 22));

		apple = GetRandomApple();

		new Thread(new KeyListener(this).DoWork).Start();

	}

	public void Loop()
	{
		while(playing)
		{
			Update();
			if(!playing)
				break;
			Render();
			Thread.Sleep(100);
		}
	}

	public void Update()
	{
		int i, j;

		switch(direction)
		{
		case 0:
			i = 0;
			j = -1;
			break;
		case 1:
			i = 1;
			j = 0;
			break;
		case 2:
			i = 0;
			j = 1;
			break;
		case 3:
			i = -1;
			j = 0;
			break;
		default:
			i = 0;
			j = 0;
			break;
		}

		Location newHead = new Location(snake.First.Value.GetX() + i, snake.First.Value.GetY() + j);

		LinkedListNode<Location> collision = snake.Find(newHead);

		if(collision != null && !collision.Value.Equals(snake.Last.Value))
		{
			playing = false;
			return;
		}

		snake.AddFirst(newHead);


		if(snake.First.Value.Equals(apple))
		{
			apple = GetRandomApple();
			length += 1;
		}

		if(snake.First.Value.GetX() >= 80 || snake.First.Value.GetX() < 0)
		{
			playing = false;
			return;
		}

		if(snake.First.Value.GetY() >= 25 || snake.First.Value.GetY() < 0)
		{
			playing = false;
			return;
		}

		while(snake.Count > length)
		{
			snake.RemoveLast();
		}

		bannedDirection = getBannedDirection(direction);
	}

	public void KeyEvents()
	{

		ConsoleKeyInfo keyInfo = Console.ReadKey(false);

		int newDirection;

		switch(keyInfo.Key)
		{
		case ConsoleKey.UpArrow:
			newDirection = 0;
			break;

		case ConsoleKey.RightArrow:
			newDirection = 1;
			break;

		case ConsoleKey.DownArrow:
			newDirection = 2;
			break;

		case ConsoleKey.LeftArrow:
			newDirection = 3;
			break;

		default:
			return;

		}

		if(newDirection != bannedDirection)
			direction = newDirection;
	}

	public void Render()
	{
		Console.Clear();
		for(int y = 0; y < 25; y++)
		{
			string line = "";
			for(int x = 0; x < 80; x++)
			{
				Location current = new Location(x, y);

				if(snake.Contains(current))
				{
					if(snake.First.Value.Equals(current))
					{
						line += getHead();
						continue;
					}

					line += 'o';
					continue;
				}

				if(apple.Equals(current))
				{
					line += 'X';
					continue;
				}

				int score = length - 3;

				if(x == 76 && y == 0)
				{
					line += score / 1000;
					continue;
				}
				if(x == 77 && y == 0)
				{
					line += score % 1000 / 100;
					continue;
				}
				if(x == 78 && y == 0)
				{
					line += score % 1000 % 100 / 10;
					continue;
				}
				if(x == 79 && y == 0)
				{
					line += score % 1000 % 100 % 10;
					continue;
				}

				line += ' ';
			}
			Console.Write(line);
		}
		Console.SetCursorPosition(0, 0);
	}

	public char getHead()
	{
		switch(direction)
		{
		case 0:
			return '^';
		case 1:
			return '>';
		case 2:
			return 'v';
		case 3:
			return '<';
		default:
			return ' ';
		}
	}

	public void WriteSeparator()
	{
		string separator = "";

		for(int index = 0; index < 80; index++)
		{
			separator += "-";
		}


		Console.WriteLine(separator);
	}

	public Location GetRandomApple()
	{
		Random random = new Random();

		Location location;
		do
		{
			location = new Location(random.Next(80), random.Next(25));
		}
		while(snake.Contains(location));

		return location;
	}

	public int getBannedDirection(int direction)
	{
		int banned = direction - 2;

		if(banned < 0)
			banned += 4;

		return banned;
	}

	public void Exit()
	{
		Console.Clear();
		Console.WriteLine("Game Over");
		Thread.Sleep(2000);
	}

	public class KeyListener
	{
		private Snake snake;

		public KeyListener(Snake snake)
		{
			this.snake = snake;
		}

		public void DoWork()
		{
			while(snake.playing)
			{
				snake.KeyEvents();
			}
		}
	}

	public class Location
	{
		private int x, y;

		public Location(int x, int y)
		{
			this.x = x;
			this.y = y;
		}


		public int GetX()
		{
			return x;
		}

		public int GetY()
		{
			return y;
		}

		public void SetX(int x)
		{
			this.x = x;
		}

		public void SetY(int y)
		{
			this.y = y;
		}


		public override bool Equals(object location)
		{
			if(!(location is Location))
				return false;


			if(((Location)location).GetX() != this.x)
				return false;

			if(((Location)location).GetY() != this.y)
				return false;

			return true;
		}

		public override string ToString()
		{
			return "Location(" + this.x + ", " + this.y + ")";
		}
	}
}