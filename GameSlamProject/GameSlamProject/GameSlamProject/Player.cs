﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace GameSlamProject
{
    public class Player:Sprite
    {
        #region Constants
        const int MAX_JUMP_HEIGHT = 600;
        const int FLOOR_HEIGHT = 768;
        const int MOVE_DISTANCE = 5;
        const int JUMP_HEIGHT = 15;
        const int MELEE_RANGE = 90;
        Vector2 PLAYER_VELOCITY = new Vector2(5, 0);
        #endregion

        /// <summary>
        /// Player's health
        /// </summary>
        public int health;

        /// <summary>
        /// is the player invulnerable to enemies?
        /// </summary>
        public bool isInvulnerable;
        
        /// <summary>
        /// rate of fire coefficient for attacks 
        /// </summary>
        public int rateOfFire;

        /// <summary>
        /// duration in seconds of pup
        /// </summary>
        public int pupDuration;

        /// <summary>
        /// kills that the player has in this level
        /// </summary>
        public int killCount;

        /// <summary>
        /// does the player have a pup
        /// </summary>
        public bool hasPup;

        /// <summary>
        /// player's score
        /// </summary>
        public int score;
        
        /// <summary>
        /// can the player fire X attack?
        /// </summary>
        public bool canFire;

        /// <summary>
        /// can the player fire C attack?
        /// </summary>
        public bool canUseStrike;

        /// <summary>
        /// can the player fire V attack?
        /// </summary>
        public bool canUseFear;

        /// <summary>
        /// is the player in a jump
        /// </summary>
        public bool isJumping;

        /// <summary>
        /// is the player falling
        /// </summary>
        public bool isFalling;

        /// <summary>
        /// is the player colliding with an object
        /// </summary>
        public bool isColliding;

        /// <summary>
        /// check if the eagle object is in motion for literally no reason
        /// </summary>
        public bool eagleIsDoingShit;

        /// <summary>
        /// the bullet that exists as part of the player
        /// </summary>
        public Bullet myBullet;

        /// <summary>
        /// the eagle that exists as part of the player
        /// </summary>
        public Eagle myEagle;

        /// <summary>
        /// Is the player currently using fear
        /// </summary>
        public bool fearOn;

        public enum Animations
        {
            Running,
            Jumping,
            GavelAttack,
            GunAttack,
            PhoneAttack,
            Death,
            Idle,
            None
        }

        public Animations animationState;
       
        public Player(Texture2D loadedTex, Eagle myEagle, Bullet myBullet)
            : base(loadedTex)
        {
            health = 100;
            isInvulnerable = false;
            rateOfFire = 1;
            pupDuration = 30;
            killCount = 0;
            hasPup = false;
            score = 0;
            canFire = true;
            canUseStrike = true;
            canUseFear = true;
            isJumping = false;
            isFalling = false;
            isColliding = false;
            isAnimatable = false;
            animationState = Animations.None;
        }

        public Player(List<SpriteSheet> loadedSheets, Eagle myEagle, Bullet myBullet)
            : base(loadedSheets)
        {
            health = 100;
            isInvulnerable = false;
            rateOfFire = 1;
            pupDuration = 30;
            killCount = 0;
            hasPup = false;
            score = 0;
            canFire = true;
            canUseStrike = true;
            canUseFear = true;
            isJumping = false;
            isFalling = false;
            isColliding = false;
            isAnimatable = true;
            animationState = Animations.Idle;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (facing == Facing.Left)
            {
                spriteBatch.Draw(tex, pos, null, color, rotation, origin, scale, SpriteEffects.FlipHorizontally, 0);
            }
            else if (facing == Facing.Right)
            {
                spriteBatch.Draw(tex, pos, null, color, rotation, origin, scale, SpriteEffects.None, 0);
            }
            else
            {
                spriteBatch.Draw(tex, pos, null, color, rotation, origin, scale, SpriteEffects.None, 0);
            }
        }

        public void Draw(SpriteBatch spriteBatch, int sheet)
        {
            spriteSheets[sheet].DrawAnimation(spriteBatch);
        }

        /// <summary>
        /// Updates the sprite. Adds vel to pos, changes the drawRect, updates rect and the pixel perfect code.
        /// </summary>
        /// <param name="gameTime">GameTime from class</param>
        public void Update(GameTime gameTime, GraphicsDeviceManager graphics)
        {
            pos += vel;
            drawRect.X = (int)pos.X;
            drawRect.Y = (int)pos.Y;
            rect = new Rectangle((int)pos.X, (int)pos.Y, tex.Width, tex.Height);
            spriteTransform = Matrix.CreateTranslation(new Vector3(-origin, 0.0f)) * Matrix.CreateScale(scale) * Matrix.CreateRotationZ(rotation) * Matrix.CreateTranslation(new Vector3(pos, 0.0f));
            rect = CalculateBoundingRectangle(new Rectangle(0, 0, tex.Width, tex.Height), spriteTransform);
        }

        /// <summary>
        /// move the player based on the ks parameter
        /// </summary>
        /// <param name="ks"></param>
        public void Move(KeyboardState ks)
        {
            if(ks.IsKeyDown(Keys.Left)) 
            {
                this.pos.X -= MOVE_DISTANCE;
                facing = Facing.Left;
                animationState = Animations.Running;
            }
            else if (ks.IsKeyDown(Keys.Right))
            {
                this.pos.X += MOVE_DISTANCE;
                facing = Facing.Right;
                animationState = Animations.Running;
            }
            else
            {
                animationState = Animations.Idle;
            }
        }


        /// <summary>
        /// player uses one of four attacks based on ks parameter
        /// </summary>
        /// <param name="ks"></param>
        public void Attack(KeyboardState ks, KeyboardState ls, List<Enemy> enemyList)
        {
            if(ks.IsKeyDown(Keys.Z) && ls.IsKeyUp(Keys.Z)) 
            {
                Rectangle meleeHurtBox = new Rectangle();

                //RAISE UR ERECTIONS
                if (facing == Facing.Right)
                {
                    meleeHurtBox = new Rectangle((int)this.pos.X + tex.Width, (int)this.pos.Y, MELEE_RANGE, this.rect.Height);
                }
                else
                {
                    meleeHurtBox = new Rectangle((int)this.pos.X - tex.Width, (int)this.pos.Y, MELEE_RANGE, this.rect.Height);
                }
                foreach (Enemy e in enemyList)
                {
                    if (meleeHurtBox.Intersects(e.rect))
                    {
                        e.health -= 1;
                    }
                }
            }
            else if (ks.IsKeyDown(Keys.X) && ls.IsKeyUp(Keys.X))
            {
                if (this.canFire)
                {
                    myBullet.visible = true;
                    this.canFire = false;
                    myBullet.pos.X = this.pos.X;
                    myBullet.pos.Y = this.pos.Y;
                    myBullet.moveBullet(this);
                }
            }
            else if (ks.IsKeyDown(Keys.V))
            {
                if (this.canUseFear)
                {
                    fearOn = true;
                    hasPup = true;
                }
            }
            else
                if (!eagleIsDoingShit)
                {
                    if ((ks.IsKeyDown(Keys.C)) && (ls.IsKeyUp(Keys.C)))
                    {
                        eagleIsDoingShit = true;
                    }
                }
                else
                {
                    myEagle.visible = true;
                    myEagle.moveEagle(this);
                    myEagle.damageEnemies(enemyList);
                    
                }
        }

        /// <summary>
        /// check if the player has a pup and then use the pup
        /// </summary>
        /// <param name="p"></param>
        public void GetPup(Pup p)
        {
            //RAISE UR PUPPIES
            Vector2 pickupRange = new Vector2(this.pos.X + 10, this.pos.Y);
            if ((p.pos.X == pickupRange.X) && (!this.hasPup))
            {
                this.hasPup = true;
                p.UsePup(this);
                
            }

        }

        /// <summary>
        /// Player jumps and can still move
        /// </summary>
        public void Jump(KeyboardState ks, KeyboardState ls, World world)
        {
            if (!isJumping && !isFalling)
            {
                if ((ks.IsKeyDown(Keys.Up)) && (ls.IsKeyUp(Keys.Up)))
                {
                    isJumping = true;
                    isFalling = false;
                }
            }

            if (isJumping)
            {
                this.pos.Y -= JUMP_HEIGHT;
            }

            if (this.pos.Y + tex.Height / 2 <= MAX_JUMP_HEIGHT)
            {
                isJumping = false;
                isFalling = true;
            }

            if (isFalling)
            {
                if (this.pos.Y + tex.Height / 2 < world.GROUND_HEIGHT)
                {
                    this.pos.Y += JUMP_HEIGHT;
                }
                else
                {
                    isJumping = false;
                    isFalling = false;
                    this.pos.Y = world.GROUND_HEIGHT - tex.Height / 2;
                }
            }
        }

        /// <summary>
        /// Revert the player back to original state when the pup has ended
        /// </summary>
        public void RevertPlayer()
        {
            if (pupDuration == 0)
            {
                isInvulnerable = false;
                fearOn = false;
                rateOfFire = 1;
                hasPup = false;
            }

            if (!isColliding)
            {
                this.vel = new Vector2(0,0);
            }

        }

        /// <summary>
        /// decrements the pup duration by one
        /// </summary>
        public void DecrementPupDuration()
        {
            pupDuration -= 1;
        }


    }
}
