using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace RewindGame.Game
{
    public abstract class Solid : CollisionObject
    {
        protected Vector2 moveRemainder;

        protected bool doWallCarry = true;
        protected bool doTopCarry = true;

        public bool doRightCollision = true;
        public bool doLeftCollision = true;
        public bool doUpCollision = true;
        public bool doDownCollision = true;

        // Code inspired by https://medium.com/@MattThorson/celeste-and-towerfall-physics-d24bd2ae0fc5
        public void Move(Vector2 transform)
        {
            Vector2 full_transform = transform + moveRemainder;

            Vector2 move_components = new Vector2((int)Math.Floor(full_transform.X), (int)Math.Floor(full_transform.Y));
            moveRemainder = full_transform - move_components;

            var riding_entities = localLevel.getAllRidingEntities(this);

            do_collide = false;
            
            if (move_components.X != 0) {
                position.X += move_components.X;

                if (move_components.X > 0)
                {
                    foreach (Entity entity in localLevel.getAllEntities())
                    {
                        Vector2 overlap = getEntityOverlap(entity) * -1;
                        if (overlap.X != 0)
                        {
                            if (doRightCollision)
                                entity.moveX(this.getCollisionBox().getRight() - entity.getCollisionBox().getLeft(), this);
                        }
                        else if (riding_entities.Contains(entity))
                        {
                            entity.moveX(move_components.X, null);
                        }
                    }
                }
                else
                {
                    foreach (Entity entity in localLevel.getAllEntities())
                    {
                        Vector2 overlap = getEntityOverlap(entity) * -1;
                        if (overlap.X != 0)
                        {
                            if (doLeftCollision)
                                entity.moveX(this.getCollisionBox().getLeft() - entity.getCollisionBox().getRight(), this);
                        }
                        else if (riding_entities.Contains(entity))
                        {
                            entity.moveX(move_components.X, null);
                        }
                    }
                }
            }


            if (move_components.Y != 0)
            {
                position.Y += move_components.Y;

                if (move_components.Y > 0)
                {
                    foreach (Entity entity in localLevel.getAllEntities())
                    {
                        Vector2 overlap = getEntityOverlap(entity) * -1;
                        if (overlap.Y != 0)
                        {
                            if (doDownCollision)
                                entity.moveY(this.getCollisionBox().getBottom() - entity.getCollisionBox().getTop(), this);
                        }
                        else if (riding_entities.Contains(entity))
                        {
                            entity.moveY(move_components.Y, null);
                        }
                    }
                }
                else
                {
                    foreach (Entity entity in localLevel.getAllEntities())
                    {
                        Vector2 overlap = getEntityOverlap(entity) * -1;
                        if (overlap.Y != 0)
                        {
                            if (doUpCollision)
                                entity.moveY(this.getCollisionBox().getTop() - entity.getCollisionBox().getBottom(), this);
                        }
                        else if (riding_entities.Contains(entity))
                        {
                            entity.moveY(move_components.Y, null);
                        }
                    }
                }
            }

            do_collide = true;


        }
    
        public virtual Vector2 getEntityOverlap( Entity entity )
        {
            return entity.getOverlap(this);
        }
        
    }
}
