using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace SnakeGame
{
    public partial class Form1 : Form
    {
        // Lista w której trzymamy kolejne elementy ciała węża, Snake[0] to głowa, Snake[Snake.Count-1] to "dupa"
        private List<Square> Snake = new List<Square>();
        private Square food = new Square();

        public Form1()
        {
            InitializeComponent();

            new Settings();
            gameTimer.Tick += new EventHandler(UpdateScreen);

            StartGame();
        }

        private int GetMaxX() { return pbCanvas.Size.Width / Settings.Width; }
        private int GetMaxY() { return pbCanvas.Size.Height / Settings.Height; }

        private void StartGame()
        {
            new Settings();

            // ustaw odpowiednie parametry do poziomu trudności
            if (radioButtonEasy.Checked)
            {
                Settings.Points = 75;
                Settings.Speed = 7;
            }
            else if(radioButtonNormal.Checked)
            {
                Settings.Points = 100;
                Settings.Speed = 10;
            }
            else if(radioButtonHard.Checked)
            {
                Settings.Points = 200;
                Settings.Speed = 15;
            }

            // ustawienie właściwości timera gry
            gameTimer.Interval = 1000 / Settings.Speed;
            gameTimer.Stop();
            gameTimer.Start();

            // przygotowanie węża, najpierw czyścimy
            Snake.Clear();
            // potem ustawiamy głowe na środku planszy
            Square head = new Square();
            head.X = GetMaxX() / 2;
            head.Y = GetMaxY() / 2;
            Snake.Add(head);

            // pokaż punkty i ustaw jedzonko
            lblScore.Text = "Punkty: " + Settings.Score.ToString();
            lblGameover.Visible = false;
            GenerateFood();
        }

        // wygeneruj nowe jedzonko, nowe miejsce (X,Y)
        private void GenerateFood()
        {
            // losowanie wartości losowej
            Random random = new Random();   
            food = new Square();
            food.X = random.Next(0, GetMaxX());
            food.Y = random.Next(0, GetMaxY());
        }

        // Zdarzenie dodane do timera które uaktualnie co ustalony interwał czasu plansze
        private void UpdateScreen(object sender, EventArgs e)
        {
            if(Settings.GameOver)
            {
                if(Inputs.KeyPressed(Keys.F))
                {
                    StartGame();
                }
            }
            else
            {
                // Obsługa klawiszy, jest troche kiepska... ale wystarczy
                // 1. Im wolniejszy będzie interwał czasu w timerze tym trudniej wysterować węża
                // 2. Nie możemy używać strzeałek bo po prostacku przechwytujemy zdarzenia z głównej formy Form
                // w Form1_KeyDown i Form1_KeyUp, a tam strzałki, enter itp. są zarezerwowane

                // dodatkowe warunki są po to żeby nie można było zawracać wężem w miejscu, czyli maks obrót o 90 stopni
                if (Inputs.KeyPressed(Keys.D) && Settings.Dir != Direction.Left)
                {
                    Settings.Dir = Direction.Right;
                }
                else if (Inputs.KeyPressed(Keys.A) && Settings.Dir != Direction.Right)
                {
                    Settings.Dir = Direction.Left;
                }
                else if (Inputs.KeyPressed(Keys.W) && Settings.Dir != Direction.Down)
                {
                    Settings.Dir = Direction.Up;
                }
                else if (Inputs.KeyPressed(Keys.S) && Settings.Dir != Direction.Up)
                {
                    Settings.Dir = Direction.Down;
                }

                MovePlayer();
            }

            pbCanvas.Invalidate();  // spowoduje odrysowania - wywołanie zdarzenia pbCanvas_Paint
        }

        private void MovePlayer()
        {
            // przechodzi przez węża zaczynając od "dupy strony", tak jest łatwiej
            for (int i = Snake.Count - 1; i >= 0; --i)
            {
                if(i == 0)  // przesuń głowe
                {
                    // sprawdź kierunek i dodaj współrzędne
                    switch(Settings.Dir)
                    {
                        case Direction.Right:
                            Snake[i].X++;
                            break;
                        case Direction.Left:
                            Snake[i].X--;
                            break;
                        case Direction.Up:
                            Snake[i].Y--;
                            break;
                        case Direction.Down:
                            Snake[i].Y++;
                            break;
                    }

                    // kolizja na planszy
                    if( Snake[i].X < 0 || Snake[i].Y < 0 
                        || Snake[i].X >= GetMaxX() || Snake[i].Y >= GetMaxY())
                    {
                        Die();
                    }

                    // kolizja głowy z samym sobą
                    for (int j = 1; j < Snake.Count - 1; ++j)
                    {
                        if (Snake[i].X == Snake[j].X && 
                            Snake[i].Y == Snake[j].Y)
                            Die();
                    }

                    // zjedzenie
                    if(Snake[0].X == food.X &&  Snake[0].Y == food.Y)
                    {
                        Eat();
                    }

                }
                else // reszta ciała, przesuń na pozycje poprzednika
                {
                    Snake[i].X = Snake[i - 1].X;
                    Snake[i].Y = Snake[i - 1].Y;
                }
            }

        }

        private void Die()
        {
            Settings.GameOver = true;
        }

        // zjedzenie jedzonka
        private void Eat()
        {
            // ustaw jego pozycje na koniec węża
            Square food = new Square();
            food.X = Snake[Snake.Count - 1].X;
            food.Y = Snake[Snake.Count - 1].Y;
            // wydłuuuuż
            Snake.Add(food);

            //dodaj punkty i wygeneruj nowe
            Settings.Score += Settings.Points;
            lblScore.Text = Settings.Score.ToString();

            GenerateFood();
        }

        // Zdarzenie rysowania plansz, właściwie kontrolki PictureBox
        private void pbCanvas_Paint(object sender, PaintEventArgs e)
        {
            //przypisanie odpowiedniego "pędzla"
            Graphics canvas = e.Graphics;

            if (!Settings.GameOver)
            {
                Brush snakeColor;   // obiekt "pędzla"

                // rysuje weza
                for (int i = 0; i < Snake.Count; ++i)
                {
                    if (i == 0)
                        snakeColor = Brushes.Yellow;
                    else
                        snakeColor = Brushes.Green;

                    // rysuj kwadraty
                    canvas.FillRectangle(snakeColor, Snake[i].X * Settings.Width,
                        Snake[i].Y * Settings.Height, Settings.Width, Settings.Height);
                    // albo koła
                    //canvas.FillEllipse(snakeColor, Snake[i].X * Settings.Width,
                    //    Snake[i].Y * Settings.Height, Settings.Width, Settings.Height);    
                }

                canvas.FillRectangle(Brushes.Red, food.X * Settings.Width,
                        food.Y * Settings.Height, Settings.Width, Settings.Height);
                //canvas.FillEllipse(Brushes.Red, food.X * Settings.Width,
                //        food.Y * Settings.Height, Settings.Width, Settings.Height);

            }
            else
            {
                string gameOver = "GAME OVER\nWynik: " + Settings.Score + "\nNaciśnij \"F\"...";
                lblGameover.Text = gameOver;
                lblGameover.Visible = true;
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            Inputs.ChangeState(e.KeyCode, true);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            Inputs.ChangeState(e.KeyCode, false);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            StartGame();
        }
    }
}
