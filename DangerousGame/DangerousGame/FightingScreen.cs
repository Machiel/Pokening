﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace DangerousGame
{
    class FightingScreen : Screen
    {
        SpriteFont SpriteFont;
        Battle Battle;
        private int LastAction = 0;

        bool IsAttacking = false;
        bool FightOver = false;
        string DisplayText = "";

        public void Initialize(ContentManager contentManager)
        {



        }

        public void Reinitialize(Battle battle)
        {
            this.Battle = battle;
            DisplayText = "";
            FightOver = false;
        }

        public void LoadContent(ContentManager contentManager)
        {
            SpriteFont = contentManager.Load<SpriteFont>("Calibri");
        }

        public Pokening.Screens Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            Keys[] pressedKeys = keyboardState.GetPressedKeys();

            if(keyboardState.IsKeyUp(Keys.D1) && keyboardState.IsKeyUp(Keys.NumPad1) &&
                keyboardState.IsKeyUp(Keys.D2) && keyboardState.IsKeyUp(Keys.NumPad2) &&
                keyboardState.IsKeyUp(Keys.D3) && keyboardState.IsKeyUp(Keys.NumPad3) &&
                keyboardState.IsKeyUp(Keys.D4) && keyboardState.IsKeyUp(Keys.NumPad4)) {
                IsAttacking = false;
            }

            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                return Pokening.Screens.WorldScreen;
            }
            else
            {
                if (FightOver == false)
                {
                    if (Battle.States.EnemyTurn == Battle.GetState() && gameTime.TotalGameTime.Seconds - LastAction > 1)
                    {

                        Random rand = new Random();
                        int i = rand.Next(0, Battle.GetMonster().GetAttacks().Count());
                        Battle.Attack(Battle.GetMonster().GetAttacks()[i]);
                    }
                    else if ((pressedKeys.Contains(Keys.D1) || pressedKeys.Contains(Keys.D2) ||
                        pressedKeys.Contains(Keys.D3) ||
                        pressedKeys.Contains(Keys.NumPad1) || pressedKeys.Contains(Keys.NumPad2) ||
                        pressedKeys.Contains(Keys.NumPad3))
                        && !IsAttacking && Battle.GetState() == Battle.States.PlayerTurn)
                    {

                        Attack attack = Battle.GetActivePlayerMonster().GetAttacks()[0];
                        if ((pressedKeys.Contains(Keys.D1) || pressedKeys.Contains(Keys.NumPad1)) && Battle.GetActivePlayerMonster().GetAttacks().Count > 0)
                        {
                            attack = Battle.GetActivePlayerMonster().GetAttacks()[0];
                        }
                        else if ((pressedKeys.Contains(Keys.D2) || pressedKeys.Contains(Keys.NumPad2)) && Battle.GetActivePlayerMonster().GetAttacks().Count > 1)
                        {
                            attack = Battle.GetActivePlayerMonster().GetAttacks()[1];
                        }
                        else if ((pressedKeys.Contains(Keys.D3) || pressedKeys.Contains(Keys.NumPad3)) && Battle.GetActivePlayerMonster().GetAttacks().Count > 2)
                        {
                            attack = Battle.GetActivePlayerMonster().GetAttacks()[2];
                        }
                        Battle.Attack(attack);
                        IsAttacking = true;
                        LastAction = gameTime.TotalGameTime.Seconds;

                    }
                }

                if (Battle.GetOutcome() == Battle.Outcomes.PlayerWon)
                {
                    DisplayText = " YOU WON! < PRESS ESC TO EXIT > ";
                    FightOver = true;
                }
                else if (Battle.GetOutcome() == Battle.Outcomes.EnemyWon)
                {
                    DisplayText = " YOU LOST! < PRESS ESC TO EXIT > ";
                    FightOver = true;
                }


                return Pokening.Screens.FightingScreen;
            }
        }

        public void Draw(GraphicsDeviceManager graphics, SpriteBatch spriteBatch)
        {
            Texture2D text = new Texture2D(graphics.GraphicsDevice, Properties.WindowWidth, Properties.WindowHeight);

            Color[] data = new Color[Properties.WindowWidth * Properties.WindowHeight];
            for (int i = 0; i < data.Length; ++i) data[i] = Color.White;
            text.SetData(data);

            Texture2D healthTexture = new Texture2D(graphics.GraphicsDevice, 100, 20);

            int health = Battle.GetMonster().GetHealth();
            Color[] healthData = new Color[100 * 20];

            for (int x = 0; x < 100; x++) {

                Color c = Color.Green;

                if (x > (int)(((float)health / 100) * 100))
                {
                    c = Color.Red;
                }

                for (int y = 0; y < 20; y++)
                {
                    healthData[100*y + x] = c; 
                }

                
            }

            healthTexture.SetData(healthData);

            Texture2D myHealthTexture = new Texture2D(graphics.GraphicsDevice, 100, 20);

            int myHealth = Battle.GetActivePlayerMonster().GetHealth();
            Color[] myHealthData = new Color[100 * 20];

            for (int x = 0; x < 100; x++)
            {

                Color c = Color.Green;

                if (x > (int)(((float)myHealth / 100) * 100))
                {
                    c = Color.Red;
                }

                for (int y = 0; y < 20; y++)
                {
                    myHealthData[100 * y + x] = c;
                }


            }

            myHealthTexture.SetData(myHealthData);

            spriteBatch.Draw(text, Vector2.Zero, Color.White);
           
            spriteBatch.DrawString(SpriteFont, Battle.GetMonster().GetName().ToUpper(), new Vector2(40, 30), Color.Black);
            spriteBatch.DrawString(SpriteFont, ":L" + Battle.GetMonster().GetLevel(), new Vector2(50, 50), Color.Black);
            spriteBatch.DrawString(SpriteFont, "HP:", new Vector2(50, 70), Color.Black);
            spriteBatch.Draw(healthTexture, new Vector2(80, 70), Color.White);

            spriteBatch.DrawString(SpriteFont, Battle.GetActivePlayerMonster().GetName().ToUpper(), new Vector2(600, 400), Color.Black);
            spriteBatch.DrawString(SpriteFont, ":L" + Battle.GetActivePlayerMonster().GetLevel(), new Vector2(610, 420), Color.Black);
            spriteBatch.DrawString(SpriteFont, "HP:", new Vector2(610, 440), Color.Black);
            spriteBatch.Draw(myHealthTexture, new Vector2(640, 440), Color.White);

            spriteBatch.DrawString(SpriteFont, DisplayText, new Vector2(350, 300), Color.Red);


            spriteBatch.DrawString(SpriteFont, "1 = Thunderbolt", new Vector2(200, 500), Color.Black);
            spriteBatch.DrawString(SpriteFont, "2 = Tackle", new Vector2(400, 500), Color.Black);
            spriteBatch.DrawString(SpriteFont, "3 = Growl", new Vector2(200, 550), Color.Black);

            Texture2D monsterTexture = Battle.GetMonster().GetTexture();
            spriteBatch.Draw(monsterTexture, new Vector2(500, 20), Color.White);

            Texture2D playerMonsterTexture = Battle.GetActivePlayerMonster().GetTexture();
            spriteBatch.Draw(playerMonsterTexture, new Vector2(50, 400), Color.White);

        }
    }
}
