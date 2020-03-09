﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameProject
{
    /// <summary>
    /// A burger
    /// </summary>
    public class Burger
    {
        #region Fields

        // graphic and drawing info
        Texture2D sprite;
        Rectangle drawRectangle;

        // burger stats
        int health = 100;

        // shooting support
        bool canShoot = true;
        bool shooting = false;
        bool makeShot = false;
        int elapsedCooldownMilliseconds = 0;

        // sound effect
        SoundEffect shootSound;

        #endregion

        #region Constructors

        /// <summary>
        ///  Constructs a burger
        /// </summary>
        /// <param name="contentManager">the content manager for loading content</param>
        /// <param name="spriteName">the sprite name</param>
        /// <param name="x">the x location of the center of the burger</param>
        /// <param name="y">the y location of the center of the burger</param>
        /// <param name="shootSound">the sound the burger plays when shooting</param>
        public Burger(ContentManager contentManager, string spriteName, int x, int y,
            SoundEffect shootSound)
        {
            LoadContent(contentManager, spriteName, x, y);
            this.shootSound = shootSound;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the collision rectangle for the burger
        /// </summary>
        public Rectangle CollisionRectangle
        {
            get { return drawRectangle; }
        }

        /// <summary>
        /// Gets or set the health for the burger
        /// </summary>
        public int Health
        {
            get { return health; }
            set { if (health > 0)
                {
                    health = value;
                }                

            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Updates the burger's location based on mouse. Also fires 
        /// french fries as appropriate
        /// </summary>
        /// <param name="gameTime">game time</param>
        /// <param name="mouse">the current state of the mouse</param>
        public void Update(GameTime gameTime, KeyboardState key)
        {
            MouseState mouse = Mouse.GetState();
            
            // burger should only respond to input if it still has health
            if (health > 0)
            {

                // move burger using keyboard
               if (key.IsKeyDown(Keys.W) || key.IsKeyDown(Keys.Up))
                {
                    drawRectangle.Y -= GameConstants.BurgerMovementAmount;
                }
                if (key.IsKeyDown(Keys.S) || key.IsKeyDown(Keys.Down))
                {
                    drawRectangle.Y += GameConstants.BurgerMovementAmount;
                }
                if (key.IsKeyDown(Keys.A) || key.IsKeyDown(Keys.Left))
                {
                    drawRectangle.X -= GameConstants.BurgerMovementAmount;
                }
                if (key.IsKeyDown(Keys.D) || key.IsKeyDown(Keys.Right))
                {
                    drawRectangle.X += GameConstants.BurgerMovementAmount;
                }
                if (key.IsKeyDown(Keys.Space) && shooting == false)
                {
                    shooting = true;
                    makeShot = true;
                }
                else
                {
                    makeShot = false;
                }

                if (key.IsKeyUp(Keys.Space) && shooting == true)
                {
                    shooting = false;
                }
            }
            // clamp burger in window
            if (drawRectangle.Left < 0)
            {
                drawRectangle.X = 0;
            }
            if (drawRectangle.Right > GameConstants.WindowWidth)
            {
                drawRectangle.X = GameConstants.WindowWidth - drawRectangle.Width;
            }
            if (drawRectangle.Top < 0)
            {
                drawRectangle.Y = 0;
            }
            if (drawRectangle.Bottom > GameConstants.WindowHeight)
            {
                drawRectangle.Y = GameConstants.WindowHeight - drawRectangle.Height;
            }

            // update shooting allowed
            if (canShoot == false)
            {
                elapsedCooldownMilliseconds += gameTime.ElapsedGameTime.Milliseconds;
                if (elapsedCooldownMilliseconds >= GameConstants.BurgerTotalCooldownMilliseconds || mouse.LeftButton == ButtonState.Released)
                {
                    canShoot = true;
                    elapsedCooldownMilliseconds = 0;
                }

            }

            // timer concept (for animations) introduced in Chapter 7

            // shoot if appropriate
            if (health > 0)
            {
                if ((mouse.LeftButton == ButtonState.Pressed || makeShot == true ) && canShoot == true)
                {
                    canShoot = false;
                    Projectile frenchFriesProjectile = new Projectile(ProjectileType.FrenchFries, 
                        sprite: Game1.GetProjectileSprite(ProjectileType.FrenchFries),
                         x: drawRectangle.Center.X,
                         y: drawRectangle.Center.Y - GameConstants.FrenchFriesProjectileOffset,
                         yVelocity: -GameConstants.FrenchFriesProjectileSpeed);
                    Game1.AddProjectile(frenchFriesProjectile);
                    shootSound.Play();
                }

            }

        }

        /// <summary>
        /// Draws the burger
        /// </summary>
        /// <param name="spriteBatch">the sprite batch to use</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, drawRectangle, Color.White);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Loads the content for the burger
        /// </summary>
        /// <param name="contentManager">the content manager to use</param>
        /// <param name="spriteName">the name of the sprite for the burger</param>
        /// <param name="x">the x location of the center of the burger</param>
        /// <param name="y">the y location of the center of the burger</param>
        private void LoadContent(ContentManager contentManager, string spriteName,
            int x, int y)
        {
            // load content and set remainder of draw rectangle
            sprite = contentManager.Load<Texture2D>(spriteName);
            drawRectangle = new Rectangle(x - sprite.Width / 2,
                y - sprite.Height / 2, sprite.Width,
                sprite.Height);

        }

        #endregion
    }
}
