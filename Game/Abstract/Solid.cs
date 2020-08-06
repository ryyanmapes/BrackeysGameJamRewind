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

        // Code inspired by https://medium.com/@MattThorson/celeste-and-towerfall-physics-d24bd2ae0fc5
        public void Move(Vector2 transform)
        {
            Vector2 full_transform = transform + moveRemainder;

            Vector2 move_components = new Vector2((int)Math.Floor(full_transform.X), (int)Math.Floor(full_transform.Y));
            moveRemainder = full_transform - move_components;

            position = position + move_components;

            
            foreach (Entity entity in localLevel.getAllEntities())
            {
                if (entity.isRiding(this)) {
                    if (doWallCarry) entity.moveX(move_components.X, null);
                    if (doTopCarry) 
                        entity.moveY(move_components.Y, null);
                }
                else {
                    Vector2 overlap = getEntityOverlap(entity) * -1;

                    if (overlap != Vector2.Zero)
                    {
                        Vector2 move_direction_sign_components = new Vector2( move_components.X > 0 ? 1 : -1, move_components.Y > 0 ? 1 : -1);
                        if (doWallCarry) 
                            entity.moveX(overlap.X * move_direction_sign_components.X, this);
                        if (doTopCarry) 
                            entity.moveY(overlap.Y * move_direction_sign_components.Y, this);
                    }
                }
            }
            

        }
    
        public virtual Vector2 getEntityOverlap( Entity entity )
        {
            return entity.getOverlap(this);
        }
        
    }
}
