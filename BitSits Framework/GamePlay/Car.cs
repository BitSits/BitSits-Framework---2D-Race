using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Box2D.XNA;

namespace BitSits_Framework
{
    class Car
    {
        public Body body, steerWheel, driveWheel;
        RevoluteJoint steerJoint;
        World world;

        GameContent gameContent;

        Vector2 wheelSize = new Vector2(12, 18);

        public float steerAngle = 0, engineSpeed;
        float tempEngineSpeed, speedInc = 1f;

        public readonly float MaxSteerAngle = 20f / 180f * (float)Math.PI, MaxEngineSpeed = 10f;
        const int numberOfGears = 5; 
        const float MaxSpeed = 300, steerSpeed = 2f;

        public Car(Vector2 position, GameContent gameContent, World world)
        {
            this.world = world;
            this.gameContent = gameContent;

            BodyDef bd = new BodyDef();
            bd.position = position / gameContent.Scale;
            bd.type = BodyType.Dynamic;
            bd.bullet = true;

            body = world.CreateBody(bd);
            body.SetLinearDamping(1f); body.SetAngularDamping(0.1f);

            float width = gameContent.playerCar.Width, height = gameContent.playerCar.Height;

            FixtureDef fd = new FixtureDef();
            fd.density = 0.1f;
            //fd.restitution = .1f;

            CircleShape cs = new CircleShape();
            cs._p = new Vector2(0, -(height - width / 2)) / gameContent.Scale;
            cs._radius = width / 2 / gameContent.Scale;
            fd.shape = cs; body.CreateFixture(fd);

            PolygonShape ps = new PolygonShape();
            ps.SetAsBox(width / 2 / gameContent.Scale, (height - width / 2) / 2 / gameContent.Scale,
                new Vector2(0, -(height - width / 2) / 2) / gameContent.Scale, 0);
            fd.shape = ps; body.CreateFixture(fd);

            CreateWheels();
        }

        void CreateWheels()
        {
            float width = gameContent.playerCar.Width, height = gameContent.playerCar.Height;

            Vector2 halfws = wheelSize / 2 / gameContent.Scale;
            PolygonShape ps = new PolygonShape();
            BodyDef bd = new BodyDef(); bd.type = BodyType.Dynamic;
            FixtureDef fd = new FixtureDef(); fd.density = 0.1f;

            // Steer Wheel
            bd.position = body.Position + new Vector2(0, -(height - width / 2)) / gameContent.Scale;
            ps.SetAsBox(halfws.X, halfws.Y); fd.shape = ps;
            steerWheel = world.CreateBody(bd); steerWheel.CreateFixture(fd);

            RevoluteJointDef rjd = new RevoluteJointDef();
            rjd.Initialize(body, steerWheel, steerWheel.GetWorldCenter());
            rjd.enableMotor = true; rjd.maxMotorTorque = 100;
            steerJoint = world.CreateJoint(rjd) as RevoluteJoint;

            // Drive Wheel
            bd.position = body.Position + new Vector2(0, -halfws.Y);
            ps.SetAsBox(halfws.X, halfws.Y); fd.shape = ps;
            driveWheel = world.CreateBody(bd); driveWheel.CreateFixture(fd);

            PrismaticJointDef pjd = new PrismaticJointDef();
            pjd.Initialize(body, driveWheel, driveWheel.GetWorldCenter(), new Vector2(1, 0));
            pjd.lowerTranslation = pjd.upperTranslation = 0; pjd.enableLimit = true;
            world.CreateJoint(pjd);
        }

        /// <summary>
        /// This function applies a "friction" in a direction orthogonal to the body's axis.
        /// </summary>
        void killOrthogonalVelocity(Body targetBody)
        {
            Vector2 velocity = targetBody.GetLinearVelocity();
            float theta = targetBody.Rotation - (float)Math.PI / 2;
            Vector2 sidewaysAxis = new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta));
            targetBody.SetLinearVelocity(sidewaysAxis * Vector2.Dot(sidewaysAxis, velocity));
        }

        public virtual void Update(GameTime gameTime)
        {
            killOrthogonalVelocity(steerWheel);
            killOrthogonalVelocity(driveWheel);

            // Drive
            float theta = driveWheel.Rotation - (float)Math.PI / 2;

            if (tempEngineSpeed > engineSpeed) tempEngineSpeed = Math.Max(tempEngineSpeed - speedInc, engineSpeed);
            if (tempEngineSpeed < engineSpeed) tempEngineSpeed = Math.Min(tempEngineSpeed + speedInc, engineSpeed);

            driveWheel.ApplyForce(new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta)) * tempEngineSpeed,
                driveWheel.GetPosition());

            // Steer
            float mspeed = steerAngle - steerJoint.GetJointAngle();
            steerJoint.SetMotorSpeed(mspeed * steerSpeed);

            engineSpeed = 0;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(gameContent.playerCar, body.Position * gameContent.Scale, null, Color.White,
                body.Rotation, gameContent.playerCarOrigin, 1, SpriteEffects.None, 1);

            spriteBatch.DrawString(gameContent.debugFont,
                "velocity = " + body.GetLinearVelocity().Length().ToString("0"),
                body.Position * gameContent.Scale, Color.Black);

            spriteBatch.Draw(gameContent.blank, steerWheel.Position * gameContent.Scale,
                new Rectangle(0, 0, (int)wheelSize.X, (int)wheelSize.Y),
                Color.Gold, steerWheel.Rotation, wheelSize / 2, 1, SpriteEffects.None, 1);

            spriteBatch.Draw(gameContent.blank, driveWheel.Position * gameContent.Scale,
                new Rectangle(0, 0, (int)wheelSize.X, (int)wheelSize.Y),
                Color.Gray, driveWheel.Rotation, wheelSize / 2, 1, SpriteEffects.None, 1);
        }
    }
}
