using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
// dotnet add package MonoGame.Extended.Animations --version 3.8.0
// https://www.monogameextended.net/docs/features/animations/animations/

// http://rbwhitaker.wikidot.com/monogame-texture-atlases-1

namespace ProjectName
{   
    /// <summary>
    /// Дополнения для коллекций
    /// </summary>
    public static class CollectionExtension
    {

        private static Random rng = new Random();
        /// <summary> Возвращает случайный объект из коллекции <seealso cref="List"/>. </summary>
        /// <remarks> Не нуждается в дополнительных параметрах. List.selectRandom();</remarks>
        public static T selectRandom<T>(this IList<T> list)
        {
            return list[rng.Next(list.Count)];
        }
        /// <summary> Возвращает случайный объект из массива <seealso cref="Array"/>. </summary>
        /// <remarks> Не нуждается в дополнительных параметрах. array.selectRandom();</remarks>
        public static T selectRandom<T>(this T[] array)
        {
            return array[rng.Next(array.Length)];
        }
    }
    /// <summary>
    /// Дополнения для Monogame 
    /// </summary>
    public static class MonogameExtension
    {
        /// <summary> Возвращает отмасштабированный объект <seealso cref="Rectangle"/>. </summary>
        /// <param name="scale">Множитель масштабирования.</param>
        public static Rectangle Scale(this Rectangle r, float scale)
        {
            r.Width = (int)((float)r.Width * scale);
            r.Height = (int)((float)r.Height * scale);
            return r;
        }
    }
    /// <summary>
    /// Обработка факта нажатия на клавишу
    /// </summary>
    static class ButtonsCtrl
    {
        private static Keys key = Keys.None;
        private static Buttons button = Buttons.Start;
        /// <summary> Возвращает true, если нажатая клавиша была отпущена <see cref="buttonWaitForReleased"/>. </summary>
        public static bool isRelease { get; private set; }
        /// <summary> Принимает значение нажатой клавиши на контроль её отпуска. </summary>
        /// <param name="buttonForRelease">Клавиша геймпада. <see cref="Buttons"/></param>
        public static void buttonWaitForReleased(Buttons buttonForRelease)
        {
            button = buttonForRelease;
            isRelease = false;
        }
        /// <summary> Принимает значение нажатой клавиши на контроль её отпуска. </summary>
        /// <param name="buttonForRelease">Клавиша на клавиатуре. <see cref="Keys"/></param>
        public static void buttonWaitForReleased(Keys buttonForRelease)
        {
            key = buttonForRelease;
            isRelease = false;
        }
        /// <summary> Контролирует отпущена ли затребованная клавиша после нажатия <see cref="buttonWaitForReleased"/>. </summary>
        public static void releaseButtonControl()
        {
            if (button != 0)
                if (GamePad.GetState(PlayerIndex.One).IsButtonUp(button))
                {
                    isRelease = true; button = 0;
                }
            if (key != 0)
                if (Keyboard.GetState().IsKeyUp(key))
                {
                    isRelease = true; key = Keys.None;
                }
        }
    }
    /// <summary>
    /// Состояние главного игрового цикла
    /// </summary>
    enum Stat
    {
        Splash,
        Settings,
        Game,
        Final,
        Pause,
        GameOver
    }
    // Есть мысль проработать этот класс с наследованием.
    // Поля: - набор спрайт - атласа открывшегося меню.
    // Продумать механизм перехода между разными уровнями меню и продумать структуру меню (способ её реализации)
    // Так как лепить новые классы с одинаковыми механизмами - не вариант. Очень некрасиво. 
    /// <summary> Стартовая заставка и меню.</summary>
    static class SplashScreen
    {
        enum menu { Main, Start, Settings, Credits, Scores, Exit }
        static List<menu> MainMenu = new List<menu> ()
        {
            menu.Start,
            menu.Settings,
            menu.Credits,
            menu.Scores,
            menu.Exit
        };
        private static menu currentMenu = menu.Main; 
        private static int Width = Game1.Width, Height = Game1.Height;
        public static Texture2D mainMenu { get; set; }
        static Color color = Color.White;
       // public static SpriteFont Font { get; set; }
        private static Rectangle[] destignationRectangles;
        private static Rectangle[,] sourceRectangles;
        private const int countItems = 5;
        private static int itemWidth, itemHeight;

        private static int currentItem = 0;
        private static int alphaStep = 0;

        public static void Init()
        {
            itemHeight = mainMenu.Height / countItems;
            itemWidth = mainMenu.Width / 3; //Колонки в спрайте
            destignationRectangles = new Rectangle[countItems];
            sourceRectangles = new Rectangle[countItems, 3];

            for (int i = 0; i < countItems; i++)
            {
                destignationRectangles[i] = new Rectangle
                    (
                         Width / 2 - itemWidth / 2
                        , i * (Height / countItems) + (Height / countItems) / 2 - itemHeight / 2
                        , itemWidth
                        , itemHeight
                    );

            }
            for (int i = 0; i < countItems; i++)
                for (int j = 0; j < 3; j++)
                    sourceRectangles[i, j] = new Rectangle
                        (
                             itemWidth * j
                            , itemHeight * i
                            , itemWidth
                            , itemHeight
                        );


        }
        public static int selectedItem { get; private set; }

        public static void Update()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Down) && ButtonsCtrl.isRelease)
            {
                alphaStep = 0;
                if (selectedItem < countItems - 1) selectedItem++;
                ButtonsCtrl.buttonWaitForReleased(Keys.Down);
            }
            if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadDown) && ButtonsCtrl.isRelease)
            {
                alphaStep = 0;
                if (selectedItem < countItems - 1) selectedItem++;
                ButtonsCtrl.buttonWaitForReleased(Buttons.DPadDown);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up) && ButtonsCtrl.isRelease)
            {
                alphaStep = 0;
                if (selectedItem > 0) selectedItem--;
                ButtonsCtrl.buttonWaitForReleased(Keys.Up);
            }
            if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadUp) && ButtonsCtrl.isRelease)
            {
                alphaStep = 0;
                if (selectedItem > 0) selectedItem--;
                ButtonsCtrl.buttonWaitForReleased(Buttons.DPadUp);
            }


            alphaStep = (alphaStep < 255) ? alphaStep + 10 : 256;

            Spaceship.PosY += -10;
            if (Spaceship.PosY < 0) Spaceship.PosY = (1024 * 12.5f);

        }
        public static void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < countItems; i++)
            {
                int currentAlpha = (currentItem == 1) ? 255 : alphaStep;
                spriteBatch.Draw(mainMenu
                                    , destignationRectangles[i]
                                    , sourceRectangles[i, 1]
                                    , Color.FromNonPremultiplied(255, 255, 0, 255)
                                );
                if (selectedItem == i)

                    spriteBatch.Draw(mainMenu
                                    , destignationRectangles[i]
                                    , sourceRectangles[i, 1]
                                    , Color.FromNonPremultiplied(0, 255, 255, currentAlpha)
                                );
                spriteBatch.Draw(mainMenu
                                    , destignationRectangles[i]
                                    , sourceRectangles[i, 0]
                                    , Color.FromNonPremultiplied(255, 255, 0, 255)
                                );
            }
        }
    }
    static class Settings
    {
        enum menu { Main, Start, Settings, Credits, Scores, Exit }
        static List<menu> MainMenu = new List<menu> ()
        {
            menu.Start,
            menu.Settings,
            menu.Credits,
            menu.Scores,
            menu.Exit
        };
        private static menu currentMenu = menu.Main; 
        private static int Width = Game1.Width, Height = Game1.Height;
        public static Texture2D mainMenu { get; set; }
        static Color color = Color.White;
       // public static SpriteFont Font { get; set; }
        private static Rectangle[] destignationRectangles;
        private static Rectangle[,] sourceRectangles;
        private const int countItems = 5;
        private static int itemWidth, itemHeight;
        private static int currentItem = 0;
        private static int alphaStep = 0;
        public static void Init()
        {
            itemHeight = mainMenu.Height / countItems;
            itemWidth = mainMenu.Width / 3; //Колонки в спрайте
            destignationRectangles = new Rectangle[countItems];
            sourceRectangles = new Rectangle[countItems, 3];

            for (int i = 0; i < countItems; i++)
            {
                destignationRectangles[i] = new Rectangle
                    (
                         Width / 2 - itemWidth / 2
                        , i * (Height / countItems) + (Height / countItems) / 2 - itemHeight / 2
                        , itemWidth
                        , itemHeight
                    );

            }
            for (int i = 0; i < countItems; i++)
                for (int j = 0; j < 3; j++)
                    sourceRectangles[i, j] = new Rectangle
                        (
                             itemWidth * j
                            , itemHeight * i
                            , itemWidth
                            , itemHeight
                        );


        }
        public static int selectedItem { get; private set; }

        public static void Update()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Down) && ButtonsCtrl.isRelease)
            {
                alphaStep = 0;
                if (selectedItem < countItems - 1) selectedItem++;
                ButtonsCtrl.buttonWaitForReleased(Keys.Down);
            }
            if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadDown) && ButtonsCtrl.isRelease)
            {
                alphaStep = 0;
                if (selectedItem < countItems - 1) selectedItem++;
                ButtonsCtrl.buttonWaitForReleased(Buttons.DPadDown);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up) && ButtonsCtrl.isRelease)
            {
                alphaStep = 0;
                if (selectedItem > 0) selectedItem--;
                ButtonsCtrl.buttonWaitForReleased(Keys.Up);
            }
            if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadUp) && ButtonsCtrl.isRelease)
            {
                alphaStep = 0;
                if (selectedItem > 0) selectedItem--;
                ButtonsCtrl.buttonWaitForReleased(Buttons.DPadUp);
            }


            alphaStep = (alphaStep < 255) ? alphaStep + 10 : 256;

            Spaceship.PosY += -10;
            if (Spaceship.PosY < 0) Spaceship.PosY = (1024 * 12.5f);

        }
        public static void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < countItems; i++)
            {
                int currentAlpha = (currentItem == 1) ? 255 : alphaStep;
                spriteBatch.Draw(mainMenu
                                    , destignationRectangles[i]
                                    , sourceRectangles[i, 1]
                                    , Color.FromNonPremultiplied(255, 255, 0, 255)
                                );
                if (selectedItem == i)

                    spriteBatch.Draw(mainMenu
                                    , destignationRectangles[i]
                                    , sourceRectangles[i, 1]
                                    , Color.FromNonPremultiplied(0, 255, 255, currentAlpha)
                                );
                spriteBatch.Draw(mainMenu
                                    , destignationRectangles[i]
                                    , sourceRectangles[i, 0]
                                    , Color.FromNonPremultiplied(255, 255, 0, 255)
                                );
            }
        }
    }
    static class Pause
    {
        private static int Width = Game1.Width, Height = Game1.Height;
        public static Texture2D text { get; set; }
        static Color color = Color.White;
        static public void Update()
        {
        }
        public static void Draw(SpriteBatch spriteBatch)
        {
            int posx = Width / 2 - text.Width / 2;
            int posy = Height / 2 - text.Height / 2;
            spriteBatch.Draw(text
                            , new Rectangle(posx, posy, text.Width, text.Height)
                            , new Rectangle(0, 0, text.Width, text.Height)
                            , color
                            , 0f
                            , new Vector2(0, 0)
                            , SpriteEffects.None
                            , 0);
        }


    }
    static class GameOver
    {
        private static int Width = Game1.Width, Height = Game1.Height;
        public static Texture2D text { get; set; }
        static Color color = Color.White;
        //public static SpriteFont Font {get; set;}
        static public void Update()
        {
        }
        public static void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(text, new Vector2(Width / 2 - text.Width / 2, Height / 2 - text.Height / 2), color);
        }


    }
    /// <summary>
    /// Управляет отрисовкой цифр из атласа в нужном месте экрана.
    /// Реализация с корректным масштабированием.
    /// </summary>
    class Digits
    {
        /// <summary>
        /// Спрайт-атлас шрифта с цифрами от 0 до 9
        /// </summary>
        /// <value>Двухмерная текстура XNA</value>
        private static Texture2D image { get; set; }
        /// <summary>
        /// Словарь соответствия цифры образу из спрайт-атласа
        /// </summary>
        /// <typeparam name="char">Цифра</typeparam>
        /// <typeparam name="Rectangle">Координаты в виде объекта Rectangle в спрайт-атласе</typeparam>
        /// <returns></returns>
        private static Dictionary<char, Rectangle> table = new Dictionary<char, Rectangle>() {};
        static int hSize, vSize;
        static Rectangle D;
        /// <summary>
        /// Инициализация спрайт-атласа. Выполняется расчёт цифр от 0 до 9
        /// </summary>
        /// <param name="png">Изображение 2D текстуры XNA</param>
        public static void Init(Texture2D png, string digits = "0123456789")
        {
            image = png;
            Rectangle sourcesRectangles;
            
            hSize = image.Width / digits.Length; 
            vSize = image.Height;  
            
            for (int i = 0; i < 10; i++)
            {
                sourcesRectangles = new Rectangle
                (
                    i * hSize
                    , 0
                    , hSize
                    , vSize
                );
                table.Add(digits[i], sourcesRectangles);
            }
        }
        public static void Draw(int number, Vector2 LeftCorner, SpriteBatch spriteBatch)
        {
            string digits = number.ToString();
            int PosX = (int)LeftCorner.X;

            foreach (char digit in digits)
            {
                if (table.TryGetValue(digit, out D))
                {

                    spriteBatch.Draw(image
                                        , new Vector2(PosX, LeftCorner.Y)
                                        , D
                                        , Color.White);
                    PosX += hSize;
                }
            }
        }
    }
    /// <summary>Управление музыкальным сопровождением с эффектом "fade"</summary>
    struct MusicManager
    {
        public static Song soundtrack;
        public static float soundtrack_volume = 0.75f;
        public static float effects_volume = 0.75f;
        public static bool playing = false, playing_oldstate = false;
        public static void Stop()
        {
            if (playing)
            {
                if (MediaPlayer.Volume > 0f) MediaPlayer.Volume += -0.05f;
                else
                {
                    MediaPlayer.Pause(); MusicManager.playing = false;
                }
            }
        }
        public static void Play()
        {
            if (!playing)
            {
                if (MediaPlayer.Volume < MusicManager.soundtrack_volume) MediaPlayer.Volume += 0.05f;
                else
                {
                    MediaPlayer.Resume(); MusicManager.playing = true;
                }
            }
        }

    }
    public class Game1 : Game
    {
        public static int Width = 1366, Height = 768;
        private Stat stat = Stat.Splash;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public List<string> listBackgrounds = new List<string>(){   "Blue_Nebula_1",
                                                                    "Blue_Nebula_3",
                                                                    "Blue_Nebula_8",
                                                                    "background/Green_Nebula_2",
                                                                    "background/Green_Nebula_5",
                                                                    "Purple_Nebula_7",
                                                                    "Purple_Nebula_6",
                                                                    "Purple_Nebula_5",
                                                                    "Purple_Nebula_4",
                                                                    "Purple_Nebula_3",
                                                                    "Purple_Nebula_2",
                                                                    "Purple_Nebula_1"};

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);

            Width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            Height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            Content.RootDirectory = "Content";
            IsMouseVisible = false;
            Window.Title = "Space'em up!";
        }
        protected override void Initialize()
        {
            base.Initialize();
            _graphics.HardwareModeSwitch = false;
            _graphics.PreferredBackBufferHeight = Height;
            _graphics.PreferredBackBufferWidth = Width;
            _graphics.SynchronizeWithVerticalRetrace = true;
            _graphics.ToggleFullScreen();
            _graphics.ApplyChanges();
        }
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            //SplashScreen.Backgroung = Content.Load<Texture2D>("SphereSprite");

            background.Nebula = Content.Load<Texture2D>(listBackgrounds.selectRandom());
            //background.StarField = Content.Load<Texture2D>("StarField_7");
            background.SpriteBatch = _spriteBatch;
            //sprSphere = Content.Load<Texture2D>("Ready");

            g_interface.SpriteBatch = _spriteBatch;
            g_interface.i_border = Content.Load<Texture2D>("i_border");
            g_interface.i_fill = Content.Load<Texture2D>("i_fill");
            g_interface.i_overheat_text = Content.Load<Texture2D>("i_ammo");
            g_interface.i_ammo_text = Content.Load<Texture2D>("i_health");
            g_interface.i_health_text = Content.Load<Texture2D>("i_overhead");
           // g_interface.Font = Content.Load<SpriteFont>("Splash");

            Digits.Init(Content.Load<Texture2D>("texts/digits"));

          //  SplashScreen.Font = Content.Load<SpriteFont>("Splash");
            SplashScreen.mainMenu = Content.Load<Texture2D>("texts/mainmenu");
            Pause.text = Content.Load<Texture2D>("texts/paused");
            GameOver.text = Content.Load<Texture2D>("texts/gameover");

            Asteroids.Init(_spriteBatch, Width, Height);
            Star.Texture2D = Content.Load<Texture2D>("Star");


            Spaceship.Texture2D = Content.Load<Texture2D>("SpaceShip");
            Spaceship.explodeSound = Content.Load<SoundEffect>("exposion");


            Spaceship.Init(_spriteBatch
                            , Height / 10
                            , Height / 2
                            , Height / 10
                            , Width
                            , Height);

            ThrottleFlame.texture = Content.Load<Texture2D>("throttleflame");

            Bullet.Texture2D = Content.Load<Texture2D>("bullet");
            Bullet.FireTextures = Content.Load<Texture2D>("muzzle_fire");
            Bullet.BulletCollisions = Content.Load<Texture2D>("bullet_collisions");
            Bullet.gun = Content.Load<SoundEffect>("shoot");
            Bullet.hit = Content.Load<SoundEffect>("hit");

            EnemySpaceShip.Texture2D = Content.Load<Texture2D>("enemyship"); EnemySpaceShip.SpriteBatch = _spriteBatch;
            EnemySpaceShip.explodeSound = Content.Load<SoundEffect>("exposion");
            EnemySpaceShip.Init();

            MusicManager.soundtrack = Content.Load<Song>("FallingSky");
            MediaPlayer.Play(MusicManager.soundtrack);
            MediaPlayer.Volume = MusicManager.soundtrack_volume;
            MediaPlayer.Pause();
            MediaPlayer.IsRepeating = true;

            g_interface.Init();
            SplashScreen.Init();
        }
        protected override void Update(GameTime gameTime)
        {
            ButtonsCtrl.releaseButtonControl();

            switch (stat)
            {
                case Stat.Splash:
                    background.Update(new Vector2(Spaceship.PosX, Spaceship.PosY));
                    SplashScreen.Update();

                    if (Keyboard.GetState().IsKeyDown(Keys.Enter)
                        && ButtonsCtrl.isRelease)
                    {
                        switch (SplashScreen.selectedItem)
                        {
                            case 0:
                                stat = Stat.Game; //Основное меню
                                background.Nebula = Content.Load<Texture2D>(listBackgrounds.selectRandom());
                                //Fader.Fade(Stat.Splash, Stat.Game);
                                Spaceship.PosY = Game1.Height / 2;
                                Spaceship.PosX = 0;
                                ButtonsCtrl.buttonWaitForReleased(Keys.Enter);
                                break;
                            case 1: // Настройки управления
                                stat = Stat.Settings;
                                break;
                            case 4: Exit(); break;
                        }///Перенести в соответствующий класс
                    }
                    if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Start)
                        && ButtonsCtrl.isRelease)
                    {
                        switch (SplashScreen.selectedItem)
                        {
                            case 0:
                                stat = Stat.Game;
                                background.Nebula = Content.Load<Texture2D>(listBackgrounds.selectRandom());
                                Spaceship.PosY = Game1.Height / 2;
                                Spaceship.PosX = 0;
                                ButtonsCtrl.buttonWaitForReleased(Buttons.Start);
                                break;
                            case 1: // Настройки управления
                                stat = Stat.Settings;
                                break;
                            case 4: Exit(); break;
                        }///Перенести в соответствующий класс
                    }
                    break;
                case Stat.Game:
                    background.Update(new Vector2(Spaceship.PosX, Spaceship.PosY));
                    g_interface.Update();
                    Spaceship.Update();
                    EnemySpaceShipManager.Update();
                    Asteroids.Update();
                    BulletsManager.Update();
                    if (Keyboard.GetState().IsKeyDown(Keys.Enter)
                        && ButtonsCtrl.isRelease)
                    {
                        stat = Stat.Pause;
                        ButtonsCtrl.buttonWaitForReleased(Keys.Enter);
                    }
                    if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Start)
                        && ButtonsCtrl.isRelease)
                    {
                        stat = Stat.Pause;
                        ButtonsCtrl.buttonWaitForReleased(Buttons.Start);
                    }
                    if (Player.scores - Player.prev_scores > 500)
                    {
                        EnemySpaceShipManager.MaxEnemyShips++;
                        Player.prev_scores = Player.scores;
                        Player.level++;
                        Spaceship.damage = 0;
                        Spaceship.ammo = 1000;
                    }

                    if (Player.lives < 0) stat = Stat.GameOver;
                    MusicManager.Play();
                    break;
                case Stat.Pause:
                    if (Keyboard.GetState().IsKeyDown(Keys.Enter)
                                         && ButtonsCtrl.isRelease)
                    {
                        stat = Stat.Game;
                        ButtonsCtrl.buttonWaitForReleased(Keys.Enter);
                    }

                    if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Start)
                        && ButtonsCtrl.isRelease)
                    {
                        stat = Stat.Game;
                        ButtonsCtrl.buttonWaitForReleased(Buttons.Start);
                    }
                    Pause.Update();
                    MusicManager.Stop();
                    break;
                case Stat.GameOver:
                    if (Keyboard.GetState().IsKeyDown(Keys.Enter)
                                         && ButtonsCtrl.isRelease)
                    {
                        stat = Stat.Splash;
                        Player.scores = 0;
                        Player.level = 0;
                        Player.lives = 3;
                        Player.prev_scores = 0;
                        EnemySpaceShipManager.MaxEnemyShips = 1;
                        ButtonsCtrl.buttonWaitForReleased(Keys.Enter);
                    }
                    if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Start))
                    {
                        // Эти участки перенести в соответствующий класс оставить только stat
                        stat = Stat.Splash;
                        Player.scores = 0;
                        Player.level = 0;
                        Player.lives = 3;
                        Player.prev_scores = 0;
                        EnemySpaceShipManager.MaxEnemyShips = 1;
                        ButtonsCtrl.buttonWaitForReleased(Buttons.Start);
                    }
                    GameOver.Update();
                    MusicManager.Stop();
                    break;
                case Stat.Settings:
                    
                    break;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();
            base.Update(gameTime);

        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(SpriteSortMode.Immediate); //BlendState.Additive

            switch (stat)
            {
                case Stat.Splash:
                    background.Draw();
                    SplashScreen.Draw(_spriteBatch);
                    break;
                case Stat.Settings:
                    background.Draw();      

                    break;
                case Stat.Game:
                    double time = gameTime.ElapsedGameTime.TotalMilliseconds / 100;
                    background.Draw();
                    BulletsManager.Draw();
                    Spaceship.Draw(); // Звездолёт
                    EnemySpaceShipManager.Draw();
                    Asteroids.Draw(); // Задник
                    g_interface.Draw();
                    break;
                case Stat.Pause:
                    background.Draw();
                    Spaceship.Draw(); // Звездолёт
                    EnemySpaceShipManager.Draw();
                    Asteroids.Draw(); // Задник
                    BulletsManager.Draw();
                    g_interface.Draw();
                    Pause.Draw(_spriteBatch);
                    break;
                case Stat.GameOver:
                    background.Draw();
                    EnemySpaceShipManager.Draw();
                    BulletsManager.Draw();
                    g_interface.Draw();
                    GameOver.Draw(_spriteBatch);
                    break;
            }
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
