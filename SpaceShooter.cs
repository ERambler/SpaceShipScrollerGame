using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework.Audio;

namespace ProjectName
{
    /// <summary>
    /// Отрисовка динамического заднего плана
    /// </summary>
    class background
    {
        
         private static int Width=Game1.Width, Heigth=Game1.Height;
         public static Texture2D Nebula {get; set;}
         public static Texture2D StarField {get; set;}
         private static Vector2 Position = new Vector2 (-1000,-1000);
         private static Vector2 StaticPosition = new Vector2 (-1000,-1000);
         private static int size = (int) (1024*1.25);
         private static int original_size = 1024;
         private static Rectangle sourceReactangle = new Rectangle (0,0,original_size,original_size);
         public static SpriteBatch SpriteBatch {get;set;}

         public static void Update(Vector2 pos)
         {
            pos = Vector2.Multiply(pos,(float) - 0.1);
            Position=pos;
            
         }
         public static void Draw()
         {
            for (int x=0; x<3;x++)
                for (int y=0; y<3;y++)
                {    
  
                    SpriteBatch.Draw (Nebula
                                     ,new Rectangle ((int) (StaticPosition.X + Position.X + x*size), (int) (StaticPosition.Y + Position.Y + y*size),size,size)
                                     ,sourceReactangle
                                     ,Color.FromNonPremultiplied(255,255,255,255)
                                     );

                }
         }         
    }
    /// <summary>
    /// Параметры игрока
    /// </summary>
    class Player
    {
        public static int scores = 0;
        public static int prev_scores=0;
        public static int level = 0;
        public static int lives = 3;
    }
    /// <summary>
    /// Графический интерфейс. HUD
    /// </summary>
    class g_interface
    {
        private static int Width=Game1.Width, Height=Game1.Height;     
        public static SpriteBatch SpriteBatch {get; set;} 
        public static Texture2D i_border {get; set;}
        public static Texture2D i_fill {get; set;}
        public static Texture2D i_overheat_text {get; set;}
        public static Texture2D i_ammo_text {get; set;}
        public static Texture2D i_health_text {get; set;}
        private static List<(Texture2D sprite, Rectangle position, Rectangle border_position)> inscriptions = new List<(Texture2D sprite,Rectangle position, Rectangle border_position)> () {};
        private static int countOfElements; 
        public static SpriteFont Font {get; set;}
        private static Rectangle[] fillRectangle;
        public static void Init()
        {
            int i=0;
            List<Texture2D> list = new List<Texture2D> () 

                    {i_overheat_text,i_ammo_text,i_health_text};

            countOfElements = list.Count;
            foreach (var text in list)
                {
                    float ratio = text.Width / text.Height;
                    float border_ratio = i_border.Width / i_border.Height;
                    int vSize = Height/30;
                    int border_width = (int) (Width/(2*countOfElements)-(border_ratio*(vSize)));
                    int current_pos  = i*(Width/countOfElements);
                    int text_width   = (int) (Width/(2*countOfElements)-(ratio*(vSize)));
                    int bias = (int) (Width/countOfElements-border_ratio*(vSize)) / 2;
                    inscriptions.Add ((  text
                                        ,new Rectangle (bias + current_pos + text_width/2, Height-2*vSize, (int) (ratio*vSize),vSize)
                                        ,new Rectangle (bias + current_pos+border_width/2, Height-vSize, (int) (border_ratio*vSize),vSize)
                                     ));
                    i++;
                }
            countOfElements = inscriptions.Count;
            Update();
        }
        public static void Update()
        {
            float pecent_health   = (float) ((100-Spaceship.damage)/100);
            float percent_overheat= (Spaceship.overheat)/100;
            float percent_ammo    = (Spaceship.ammo/10)/100;
            fillRectangle = new Rectangle[countOfElements];
            float[] fillValues = new float[] {percent_ammo,pecent_health,percent_overheat}; 
            
            for (int i=0; i < countOfElements; i++)
            {
                fillRectangle[i]=  new Rectangle (   inscriptions[i].border_position.X+7
                                                    ,inscriptions[i].border_position.Y
                                                    ,(int) ((inscriptions[i].border_position.Width-14)*fillValues[i])
                                                    ,inscriptions[i].border_position.Height);
            }
            
        }
        public static void Draw()
        {
                for (int i = 0; i < countOfElements; i++)
                {
                    SpriteBatch.Draw(i_fill
                                    ,fillRectangle[i]
                                    ,Color.FromNonPremultiplied(250,250,0, 255)
                                    );
                    SpriteBatch.Draw(i_border,inscriptions[i].border_position,Color.White);
                    SpriteBatch.Draw(inscriptions[i].sprite
                                    ,inscriptions[i].position
                                    ,Color.White);

                    
                }
            Digits.Draw (Player.scores,new Vector2 (10,Height-81),SpriteBatch);
            if (Player.lives>=0) Digits.Draw (Player.lives,new Vector2 (Width-50,Height-81),SpriteBatch);
            /*SpriteBatch.DrawString(  Font
                                    ,Player.scores.ToString()
                                    ,new Vector2 (10,Height-2*30)
                                    ,Color.FromNonPremultiplied(250,250,0, 180)); 
            SpriteBatch.DrawString(  Font
                                    ,Player.lives.ToString()
                                    ,new Vector2 (Width-50,Height-2*30)
                                    ,Color.FromNonPremultiplied(250,250,0, 180)); */              
        }
    }

    /// <summary>
    /// Изображение пламени
    /// </summary>
    class ThrottleFlame
    {
        private int alpha=255;
        private static Random rnd = new Random();
        public bool outed =false; 
        public static Texture2D texture {get;set;}
        private Vector2   position;
        private Rectangle destignationRectangle = new Rectangle();
        private int startSize;
        public ThrottleFlame (Vector2 position,int startSize)
        {
            this.position = position;
            this.startSize= startSize;
        }
        public void Update()
        {   
            if (alpha==0) {this.outed = true; return;}
            position.X+=rnd.Next(-2,-1);
            position.Y+=rnd.Next(-2,3);
            alpha-=15;
            startSize-=1;
            destignationRectangle.X=(int) position.X;
            destignationRectangle.Y=(int) position.Y-startSize/2;
            destignationRectangle.Width  = startSize;
            destignationRectangle.Height = startSize;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture,destignationRectangle,Color.FromNonPremultiplied(255,255,255,alpha));
        }

    }
    ///<summary>Космический корабль игрока</summary>
    class Spaceship
    {
        ///<summary> Ширина и высота экрана</summary>

        private static int Width, Heigth;
        private static int timer=1;
        ///<summary> Вектор скорости в "жидком вакууме"</summary>
        public static Vector2 Velocity;
         private static Vector2 up = new Vector2 (0,-1);
         private static Vector2 down = new Vector2 (0,1);
         private static Vector2 right = new Vector2 (1,0);
         private static Vector2 left = new Vector2 (-1,0);
         private static List <Bullet> Bullets = new List<Bullet>(){};
         private static List <Bullet> bulletsForRemove = new List<Bullet>(){};
         private static List <Rectangle> shipAnimations=new List<Rectangle>(){}; private static int animatorPhase=0;
         private static List <ThrottleFlame> throttleFlame = new List<ThrottleFlame>() {};
         private static Vector2[] GunPositions, FlamePositions;
         /// <summary>Номер пушки для ведения огня</summary>
         private static int activeGun = 0;
         ///<summary> Определяет анимацию взрыва </summary>        
         public static bool isAlive { get { return damage < 100; } }
         public static bool isRespawn=true;
         public static bool isManevring = false; private static int animationPhaseOld;
         public static float damage = 0;
         public static float ammo = 1000;
         public static float overheat = 0;
         private static int animationCounter = 0;
        ///<summary> Размер спрайта </summary>
        public static int Size;
        public static Rectangle destignationRectangle;
        //Vector2 Pos;
        ///<summary> Позиция на экране</summary>
        public static float PosX, PosY;
        public static SpriteBatch SpriteBatch {get; set;}
        public static Texture2D Texture2D {get; set;}
        private static Random rnd = new Random();
        private static (int Columns,int Rows) atlas;
        private static (int X,int Y) spriteSize;

        private static Rectangle ShipSourceRectangle; 
        public static SoundEffect explodeSound;
    
        private static void getSourcePosition (int Column,int Row,ref Rectangle Rectangle) 
        {
            Rectangle.X = Column*spriteSize.X;
            Rectangle.Y = Row*spriteSize.Y;
            Rectangle.Width  = spriteSize.X;
            Rectangle.Height = spriteSize.Y;
        }
        public static void Init (SpriteBatch spriteBatch, int X, int Y, int Size, int Width, int Heigth)
        {
            Spaceship.SpriteBatch = spriteBatch;
            PosX = X;
            PosY = Y;
            Spaceship.Width = Width; 
            Spaceship.Heigth= Heigth;
            //this.Pos = new Vector2 (X,Y);
            Spaceship.Size = Size;
            Velocity = new Vector2 (0,0);
            GunPositions = new Vector2[2] { new Vector2((float)0.8,(float)0.15)*Size
                                           ,new Vector2((float)0.8,(float)0.85)*Size};
            atlas = (30,4);
            spriteSize = ((int) Texture2D.Width/atlas.Columns, (int) Texture2D.Height/atlas.Rows);

            getSourcePosition(0,1,ref ShipSourceRectangle);
            Rectangle rect;
            for (int R=1; R < atlas.Rows-1; R++)
                for (int C=0; C < atlas.Columns; C++)
                    {
                        rect = new Rectangle();
                        getSourcePosition(C,R,ref rect);
                        shipAnimations.Add (rect);
                    }
        }
        private static void Fire()
        {
            if (overheat<95)
            {
                activeGun++;
                if (activeGun == GunPositions.Length) activeGun = 0; 
                    BulletsManager.Fire(new Vector2 (PosX,PosY)+GunPositions[activeGun]
                                        ,Velocity
                                        ,SpriteBatch
                                        ,50);
                PosX+=rnd.Next(-4,2);
                PosY+=rnd.Next(-2,3);
                Velocity += left;
                --ammo;
                overheat+=5;
            }
        }
        private static void Fire(bool twoGuns)
        {
            Fire();
            Fire();
        }
        public static void Update()
        {
            if (!isRespawn & isAlive)
            {
            if (Keyboard.GetState().IsKeyDown(Keys.Down)
                || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadDown))  Velocity += down;
            if (Keyboard.GetState().IsKeyDown(Keys.Up)
                || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadUp))    Velocity += up;
            if (Keyboard.GetState().IsKeyDown(Keys.Left)
                || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadLeft))  Velocity += left;
            if (Keyboard.GetState().IsKeyDown(Keys.Right)
                || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadRight)) Velocity = Vector2.Add (Velocity, right);
            
            if (!isManevring)
            {//ОГОНЬ
            if  (Keyboard.GetState().IsKeyDown(Keys.Z) 
                || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.X)
                & timer%3 == 0) 
                    Fire(); 

            if  (Keyboard.GetState().IsKeyDown(Keys.X)
                || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Y) & timer%2 == 0)
                    Fire();

            if  (Keyboard.GetState().IsKeyDown(Keys.C)
                || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.A))
                    Fire();                
            if  (Keyboard.GetState().IsKeyDown(Keys.V)
                || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.B))
                    Fire(true);              
            }
            if ((Keyboard.GetState().IsKeyDown(Keys.LeftShift) //МАНЕВР
                || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftTrigger))
                && !isManevring)
                {
                    isManevring=true; animationPhaseOld = animatorPhase;
                }
            }

                PosX += (int) Velocity.X;
                PosY += (int) Velocity.Y;
            
            if (isAlive)
            {
                if (PosX>Width-Size)  PosX = Width-Size;
                if (PosY>Heigth-Size) PosY = Heigth-Size;
                if (!isRespawn)
                {
                    if (PosX<0) PosX = 0;
                    if (PosY<0) PosY = 0;
                }
                
                if (overheat>0) overheat+=-0.4f*(float)(Player.level+1);
                //if (overheat>100)
            }
            if (!isManevring)
                Velocity = Vector2.Multiply (Velocity, (float) 0.95);
            else
                Velocity = Vector2.Multiply (Velocity, (float) 0.97);
            destignationRectangle = new Rectangle((int) PosX, (int) PosY, Size, Size);
            // Фаза = 15 угол 90, фаза 30 - угол 180, фаза 45 угол 270
            float angle = animatorPhase<15 ? MathF.Cos(MathHelper.PiOver2*animatorPhase/15) 
                        : animatorPhase<30 ? MathF.Cos(MathHelper.PiOver2+MathHelper.PiOver2*(animatorPhase-15)/15) 
                        : animatorPhase<45 ? MathF.Cos(-MathHelper.PiOver2+MathHelper.PiOver2*(animatorPhase-45)/15)
                        : MathF.Cos(MathHelper.PiOver2*(animatorPhase-60)/15); 
            GunPositions = new Vector2[2] { new Vector2((float)0.8, 0.2f + MathF.Abs (Velocity.Y)/60)*Size
                                           ,new Vector2((float)0.8, 0.8f - MathF.Abs (Velocity.Y)/60)*Size};
              
            if (Velocity.X>0)
            {   
                int ScaledSize;
                if (isManevring)
                    if (animationPhaseOld>shipAnimations.Count/2) 
                        ScaledSize = (int) ((float) Size*(1+0.3f*MathF.Sin(MathHelper.Pi*animationCounter/60)));
                    else
                        ScaledSize = (int) ((float) Size*(1-0.3f*MathF.Sin(MathHelper.Pi*animationCounter/60)));
                else
                    ScaledSize = Size;

                FlamePositions = new Vector2[2] {  new Vector2((float)0, 0.5f - 0.3f * angle)*ScaledSize
                                                ,new Vector2((float)0, 0.5f + 0.3f * angle)*ScaledSize};

                int s = (int) ((float)(ScaledSize/5+1)*(Velocity.X/12));
                throttleFlame.Add(new ThrottleFlame(new Vector2 (PosX,PosY)+FlamePositions[0],s));
                throttleFlame.Add(new ThrottleFlame(new Vector2 (PosX,PosY)+FlamePositions[1],s));
            }
            List <ThrottleFlame> removing = new List<ThrottleFlame> (){};
            foreach (var x in throttleFlame) {if (x.outed) removing.Add(x);x.Update();}
            foreach (var x in removing) throttleFlame.Remove(x);
            removing.Clear();  


            if (isManevring)
               {    
                    animationCounter++;
                    if (animationPhaseOld>shipAnimations.Count/2) 
                    {
                        animatorPhase--;
                        destignationRectangle = destignationRectangle.Scale(1+0.7f*MathF.Sin(MathHelper.Pi*animationCounter/60)); 
                    }        
                    else 
                    {
                        animatorPhase++;
                        destignationRectangle = destignationRectangle.Scale(1-0.3f*MathF.Sin(MathHelper.Pi*animationCounter/60));
                    }
                    if (animatorPhase > shipAnimations.Count-1) animatorPhase=0;
                    if (animatorPhase < 0) animatorPhase = shipAnimations.Count-1;
                    ShipSourceRectangle = shipAnimations[animatorPhase];
                    if (animatorPhase == 0)
                    {
                        isManevring=false;
                        //Velocity.Y=animationPhaseOld;
                        animationCounter=0;
                    }
                }
            else
            {
                animatorPhase = Velocity.Y<0 ? shipAnimations.Count+(int)Velocity.Y-1 : (int) Velocity.Y;
                ShipSourceRectangle = shipAnimations[animatorPhase];
            }
            if (isRespawn)
            {
                if (animationCounter<atlas.Columns) 
                {
                    animationCounter++; 
                    getSourcePosition(animationCounter/2,0,ref ShipSourceRectangle);
                } 
                else {animationCounter=0; isRespawn=false;}
            }
            if (!isManevring)
                foreach (var ship in EnemySpaceShipManager.ships)
                    if (ship.isAlive & destignationRectangle.Intersects(ship.destinationRectangle)) 
                    {
                        if (!isRespawn)
                            damage=100;
                            ship.damage=100;break;
                    }
           timer++;
           timer = timer > 400 ? 1 :timer++;
        }
        public static void Draw()
        {             
            if (isAlive)
            {   float lDepth = 0;
                if (isManevring) lDepth=0.1f;
                
                foreach (var x in throttleFlame) x.Draw(SpriteBatch);
                Spaceship.SpriteBatch.Draw(Texture2D
                                        ,destignationRectangle
                                        ,ShipSourceRectangle
                                        ,Color.White
                                        ,0
                                        , new Vector2(0, 0)
                                        , SpriteEffects.None
                                        ,lDepth);
            }
            else 
            {
                if (animationCounter>240) 
                {
                    damage=0;
                    ammo = 1000;
                    overheat = 0;
                    animationCounter=0;
                    Player.lives--;
                    PosX=-100;
                    PosY=Heigth/2;
                    Velocity=new Vector2(10,0);
                    isRespawn = true;
                    
                }
                //if (animationCounter==225) Fader.Fade();
                if (animationCounter<240 && animationCounter>36) 
                {
                    if (PosX > -Size)         PosX += -Size/10;

                    if (PosY > Heigth/2)      PosY += -4;
                    if (PosY < Heigth/2)      PosY +=  4;
                
                }
                if (animationCounter==1) explodeSound.Play((float) rnd.Next (4,7) /10,0,(float) ((PosX - Width/2) / Width/2));
                if (animationCounter<36)
                {
                    Spaceship.SpriteBatch.Draw(Texture2D
                                                ,new Rectangle((int) (PosX-(1.5*Size)), (int) (PosY-(1.5*Size)), 3*Size, 3*Size)
                                                ,new Rectangle((int) (animationCounter/3)*175,3*175, 175, 175)
                                                ,Color.White);
                }
                animationCounter++;
            }
        }
    }
    ///<summary> Фоновые "звёздочки"</summary>
    class Asteroids
    {
        public static int Width, Heigth;
        static public SpriteBatch SpriteBatch {get; set;}
        static Star[] stars;
        public static Random rnd = new Random();
        static public void Init(SpriteBatch SpriteBatch, int Width, int Heigth)
        {
            Asteroids.Width  = Width;
            Asteroids.Heigth = Heigth;
            Asteroids.SpriteBatch = SpriteBatch;
            stars = new Star[50];
            for (int i=0; i<50; i++)
                stars[i] = new Star(new Vector2(-rnd.Next(1,10),0));
        }
        public static void Draw ()
        {
            foreach (Star star in stars)
                star.Draw();
        }
        public static void Update ()
        {
            foreach (Star star in stars)
                star.Update();
        }
    }
    ///<summary> Одна "звёздочка"</summary>
    class Star
    {
        Vector2 Pos;
        Vector2 Dir;
        Color color;
       
        public static Texture2D Texture2D {get; set;}
        public Star (Vector2 Pos, Vector2 Dir)
        {
            this.Pos=Pos;
            this.Dir=Dir;
        }
        public Star (Vector2 Dir)
        {
            this.Dir = Dir;
            RandomSet();
        }
        public void Update()
        {
            Pos += Dir;
            if (Pos.X < 0)
            {
                RandomSet();
            }
        }
        public void RandomSet()
        {
            
            Pos = new Vector2(Asteroids.rnd.Next(Asteroids.Width,Asteroids.Width+300)
                             ,Asteroids.rnd.Next(0,Asteroids.Heigth+300));
            color = Color.FromNonPremultiplied(Asteroids.rnd.Next(127,256)
                                              ,Asteroids.rnd.Next(127,256)
                                              ,Asteroids.rnd.Next(127,256)
                                              ,Asteroids.rnd.Next(190,256));
        }
        public void Draw()
        {
            Asteroids.SpriteBatch.Draw (Texture2D,Pos,color);
        }
    }
     ///<summary> Отдельная "пуля"</summary>
    class Bullet
    {
        private int screenWidth=Game1.Width; // Плохо. не по SOLID - потом подправить например через конструктор
        public bool screenOut = false;
        private static SpriteBatch SpriteBatch {get; set;}
        Vector2 Position;
        Vector2 Direction;
        Color color = Color.White;
        private static float speedMultiply; 
        private static Random rnd = new Random();
        public static Texture2D Texture2D {get; set;}
        public static Texture2D FireTextures {get; set;}
        public static Texture2D BulletCollisions {get; set;}
        bool firstFire = true;
        SpriteEffects flip = SpriteEffects.None;
        bool isCollide = false;
        int collidePhase = 0;
        private static Texture2D currentTexture;

        //public static Effect effect;
        

        public static SoundEffect gun;
        public static SoundEffect hit;

        public Bullet (Vector2 Pos, Vector2 Dir, SpriteBatch spriteBatch, float muzzle_speed, bool flip = false)
        {
            speedMultiply = muzzle_speed;
            this.Position=Pos;
            this.Direction=Dir+new Vector2(speedMultiply+(float) (rnd.Next(-20,21)/10),(float) (rnd.Next(-20,21)/10));
            SpriteBatch = spriteBatch;
            currentTexture = Bullet.FireTextures;
            if (flip)
                this.flip = SpriteEffects.FlipHorizontally;
        }
        public Bullet (Vector2 Dir, SpriteBatch spriteBatch)
        {
            this.Direction = Dir;
            SpriteBatch = spriteBatch;
        }
        public void Update()
        {
            if (!this.isCollide) Position += Direction;
            if (Position.X > this.screenWidth) this.screenOut=true;
            
            Rectangle bulletRectangle = new Rectangle((int)Position.X,(int)Position.Y, Texture2D.Width, Texture2D.Height);
            //Работаем по вражеским кораблям
            if (!this.firstFire)
            {
                foreach (var ship in EnemySpaceShipManager.ships)
                    if (ship.isAlive & bulletRectangle.Intersects(ship.destinationRectangle)) 
                    {
                        this.isCollide=true;
                        ship.Velocity+=Vector2.Multiply(this.Direction,(float) 1/(10*EnemySpaceShip.Size));
                        ship.damage++;
                        Player.scores++;
                    }
                if (Spaceship.isAlive 
                    & !Spaceship.isRespawn 
                    & !Spaceship.isManevring 
                    & bulletRectangle.Intersects(Spaceship.destignationRectangle)) 
                    {
                        this.isCollide=true;
                        Spaceship.Velocity+=Vector2.Multiply(this.Direction,(float) 1/(10*Spaceship.Size));
                        Spaceship.damage++;
                    }    
            }
        }
        public void Draw()
        {
            if (this.firstFire) 
            {

                int rndMuzzle_Fire = rnd.Next(4);
                Rectangle sourceRectangle = new Rectangle (150*rndMuzzle_Fire,0,150,100);
                Rectangle destinationRectangle = new Rectangle ((int) Position.X-Spaceship.Size/2, (int) Position.Y-Spaceship.Size/2, Spaceship.Size, Spaceship.Size);
                Bullet.SpriteBatch.Draw (FireTextures
                                      ,destinationRectangle // Плохо. Берём размеры из несвязанного класса. Обратить внимание, когда будем стрелять от имени врагов.
                                      ,sourceRectangle
                                      ,Color.White
                                      ,0f
                                      ,new Vector2 (0,0)
                                      ,this.flip
                                      ,0
                                      );
                
                gun.Play((float)(rnd.Next (2,5)) /10,0f,(float) ((destinationRectangle.X - screenWidth/2) / screenWidth/2));
                                     
                this.firstFire = false;
            } 
            else 
                if (this.isCollide)
                {
                    Rectangle sourceRectangle = new Rectangle (100*collidePhase,0,100,100);
                    Rectangle destinationRectangle = new Rectangle ((int) Position.X-Spaceship.Size/2, (int) Position.Y-Spaceship.Size/2, Spaceship.Size, Spaceship.Size);
                    Bullet.SpriteBatch.Draw (BulletCollisions
                                        ,destinationRectangle
                                        ,sourceRectangle
                                        ,Color.White);
                    this.collidePhase++;                    
                    if (this.collidePhase>6) this.screenOut=true;
                    if (this.collidePhase==1) hit.Play((float)(rnd.Next (1,3)) /10,0f,(float) ((destinationRectangle.X - screenWidth/2) / screenWidth/2));  
                }    
                else
                {    
                    //effect.CurrentTechnique.Passes[0].Apply();
                    Bullet.SpriteBatch.Draw (Texture2D
                                            ,new Rectangle ((int) Position.X,(int) Position.Y,Texture2D.Width,Texture2D.Height)
                                            ,new Rectangle( 0,0, Texture2D.Width,Texture2D.Height) 
                                            ,color
                                            ,0f
                                            ,new Vector2(0,0)
                                            ,SpriteEffects.None
                                            ,0
                                            );
                }
        }
    }
    class EnemySpaceShipManager
    {
        private static int Width=Game1.Width, Height=Game1.Height;
        private static int timer=1;
        public static List<EnemySpaceShip> ships = new List<EnemySpaceShip>(){};
        private static Random rnd = new Random();
        private static List <EnemySpaceShip> shipForRemove = new List<EnemySpaceShip>(){};
        public static int MaxEnemyShips=1; 

        public static void Update()
        {
            if (Spaceship.isAlive && ships.Count < MaxEnemyShips && timer%25 == 0)
            {
                EnemySpaceShip currentShip = new EnemySpaceShip(new Vector2(Width+350,rnd.Next(0,Height-Height/10))
                                                                ,new Vector2 (rnd.Next(-15,-10),0)
                                                                ,0
                                                                ,Height/10);
                ships.Add(currentShip);
            }
            shipForRemove.Clear();
            foreach (var ship in ships) if (ship.isDestroyed || ship.outOfScreen) shipForRemove.Add(ship);
            foreach (var ship in shipForRemove) ships.Remove(ship);            
            foreach (var ship in ships) ship.Update(timer);
            timer++;
            timer = timer > 400 ? 1 :timer++;
        }
        public static void Draw ()
        {
            foreach(var ship in ships)
            {
                ship.Draw();
            }
        }
    }
    class EnemySpaceShip
    {
        private static int Width=Game1.Width, Height=Game1.Height;
        public static SpriteBatch SpriteBatch {get; set;}
        private static Random rnd = new Random();
        private static List <Rectangle> shipAnimations=new List<Rectangle>(){}; private int animatorPhase=0;
        private static List <ThrottleFlame> throttleFlame = new List<ThrottleFlame>() {};
        private static Vector2[] GunPositions, FlamePositions;
        public static Texture2D Texture2D {get; set;}
        public static int Size;
        public int damage = 0;
        ///<summary> Определяет теущую позицию выстрела/дула орудия </summary>   
        private int  activeGun = 0;
        public bool isAlive  { get { return damage < 50; } }
        public bool isDestroyed = false, outOfScreen = false;
        public float Maneuverability = 0.5f;
        public Vector2 Position;
        public Vector2 Velocity;
        public Color color = Color.White;
        Rectangle sourceRectangle; // Спрайт источник в спрайт-атласе
        public Rectangle destinationRectangle; 
        private int animationCounter=0;
        private static (int Columns,int Rows) atlas;
        private static (int X,int Y) spriteSize;

        public static SoundEffect explodeSound;


        private static void getSourcePosition (int Column,int Row,ref Rectangle Rectangle) 
        {
            Rectangle.X = Column*spriteSize.X;
            Rectangle.Y = Row*spriteSize.Y;
            Rectangle.Width  = spriteSize.X;
            Rectangle.Height = spriteSize.Y;
        }

        public static void Init ()
        {
            GunPositions = new Vector2[2] { new Vector2((float)0.75 ,(float)0.15)*Size
                                           ,new Vector2((float)0.75 ,(float)0.85)*Size};
            atlas = (30,3);
            spriteSize = ((int) Texture2D.Width/atlas.Columns, (int) Texture2D.Height/atlas.Rows);

            EnemySpaceShip.Size = Height / 10;


            //getSourcePosition(0,0,ref sourceRectangle);
            Rectangle rect;
            for (int R=0; R < atlas.Rows-1; R++)
                for (int C=0; C < atlas.Columns; C++)
                    {
                        rect = new Rectangle();
                        getSourcePosition(C,R,ref rect);
                        shipAnimations.Add (rect);
                    }
        }        

        public EnemySpaceShip (Vector2 Pos, Vector2 Dir, int kindOfShip = 0, int size = 175)
        {
            this.Position  = Pos;
            this.Velocity  = Dir;
            //this.Size      = size;
            //this.sourceRectangle = new Rectangle (350*kindOfShip,0,350,350);

        }
        public void Fire()
        {
            activeGun++;
            if (activeGun == GunPositions.Length) activeGun = 0; 
            BulletsManager.Fire(new Vector2 (Position.X,Position.Y)+GunPositions[activeGun]
                                            ,Velocity
                                            ,SpriteBatch
                                            ,-50, true);
            Position.X += 2;
            Position.X += rnd.Next(-2,4);
            Position.Y += rnd.Next(-2,3);
        }
        public void Atack(int timer)
        {
            Vector2 atackedShip = new Vector2 (Spaceship.PosX,Spaceship.PosY);
            Vector2 atackVector = atackedShip-this.Position;

            if (Spaceship.isAlive
                &  Math.Abs(Math.Abs(atackedShip.Y)-Math.Abs(Position.Y))<15
                && atackedShip.X<Position.X
                && timer%3 == 0) this.Fire();
            if (Spaceship.isAlive)
                 if (atackedShip.Y<Position.Y) Velocity.Y -= Maneuverability; else Velocity.Y += Maneuverability;
            Velocity.Y = Velocity.Y * (float)0.95;
            

        }
        public void Update(int timer)
        {
            this.Position+=this.Velocity;
            if (   this.Position.X <-1000 
                || this.Position.Y <-1000
                || this.Position.Y > 4000 
                || this.Position.X > 4000) 
                                            this.outOfScreen = true;
            
            this.destinationRectangle = new Rectangle ((int) Position.X, (int) Position.Y, Size, Size);
            
            animatorPhase = Velocity.Y<0 ? shipAnimations.Count+(int)(Velocity.Y * 1.5f) - 1 : (int) (Velocity.Y * 1.5f);
            
            this.sourceRectangle = shipAnimations[animatorPhase];
            this.Atack(timer);
        }
        public void Draw()
        {
            if (isAlive)
            {
                    SpriteBatch.Draw (   Texture2D
                                        ,this.destinationRectangle
                                        ,this.sourceRectangle
                                        ,color
                                     );
            }
            else
            {
                if (animationCounter==1) explodeSound.Play((float) rnd.Next (4,7) /10,0,(float) ((destinationRectangle.X - Width/2) / Width/2));
                if (animationCounter<36)
                {
                    SpriteBatch.Draw(Texture2D
                                    ,new Rectangle((int) (this.Position.X-Size*1.5),(int) (this.Position.Y-Size*1.5),3*Size,3*Size)
                                    ,new Rectangle((int) (animationCounter/3)*175,2*175, 175, 175)
                                    ,Color.White);
                    animationCounter++;
                }
                else
                {    
                    this.isDestroyed = true;
                    Player.scores+=10;
                }
            }
        }
    }
    class BulletsManager
    {
        private static int Width=Game1.Width, Height=Game1.Height;
        private static List<Bullet> Bullets          = new List<Bullet>(){};
        private static List<Bullet> bulletsForRemove = new List<Bullet>(){};

        public static void Fire(Vector2 Position, Vector2 Velocity,SpriteBatch SpriteBatch,float muzzle_speed,bool flip = false)
        {
            BulletsManager.Bullets.Add(new Bullet(Position,Velocity,SpriteBatch,muzzle_speed,flip)); 
        }
        public static void Update()
        { 
            bulletsForRemove.Clear();
            foreach (var B in Bullets) 
                {
                    B.Update();
                    if (B.screenOut) bulletsForRemove.Add(B);
                }
            foreach (var B in bulletsForRemove) Bullets.Remove(B);
        }
        public static void Draw()
        {
            foreach (var B in Bullets) B.Draw();
        }        
    }

}
