using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using RLH.Components;
using Core.Input;
using Microsoft.Xna.Framework.Input;
using System;
using BlockRLH.Actors.GameActors;
using System.Collections.Generic;
using BlockRLH.Components;
using Microsoft.Xna.Framework.Audio;

namespace BlockRLH.Actors
{
    /// <summary>
    /// A controller that is owned by a local player, mainly used for storing player information
    /// and calculating camera position
    /// </summary>
    public class PlayerController : Actor
    {
        private PlayerIndex playerIndex;
        private SignedInGamer gamer;
        private InputChannel playerInput;
        private Camera playerCamera;
        private Vector3 Gravity, Acceleration, LocationOffset;
        private BoundingBox bbox;
        private bool colliding, controllable;
        private int lastcollide, totalgt;
        private CollisionSolver collision;
        public List<Block> collidingBlocks;
        private float PlayerGravity;
        private DistanceMeter distanceMeter;

        private SoundEffect JumpSound;
        public SoundEffect Splash;
        public SoundEffect Win;
        private SoundEffect Step1;
        private SoundEffect Step2;
        private SoundEffect[] Hurt = new SoundEffect[3];
        private SoundEffect[] Pain = new SoundEffect[2];
        private const int MAX_STEP_DISTANCE = 35;
        private int steps = MAX_STEP_DISTANCE;
        private bool CurrentStep;
        private bool debug;
        private Vector3 lastPos;

        #region DEBUG BS

        private MouseState oldMouseState;

        #endregion

        /// <summary>
        /// Initializes a new instance of PlayerController
        /// </summary>
        /// <param name="worldInfo"></param>
        /// <param name="index"></param>
        public PlayerController(WorldInfo worldInfo, PlayerIndex index)
            : base(worldInfo)
        {
            this.playerIndex = index;
            this.gamer = Gamer.SignedInGamers[index];

            this.playerInput = WorldInfo.InputManager.GetChannel(index);
            this.WorldInfo.InputManager.ListenToInputChannel(HandleInput, this.playerIndex);
            this.Camera = new Camera(worldInfo, this, MathHelper.PiOver4);
            Mouse.SetPosition(WorldInfo.Viewport.Width / 2, WorldInfo.Viewport.Height / 2);
            this.oldMouseState = Mouse.GetState();

            PlayerGravity = -1.0f;

            JumpSound = worldInfo.Engine.Content.Load<SoundEffect>(@"Sounds/jump");
            Splash = worldInfo.Engine.Content.Load<SoundEffect>(@"Sounds/splash");
            Win = worldInfo.Engine.Content.Load<SoundEffect>(@"Sounds/win");

            Hurt[0] = worldInfo.Engine.Content.Load<SoundEffect>(@"Sounds/yell1");
            Hurt[1] = worldInfo.Engine.Content.Load<SoundEffect>(@"Sounds/yell2");
            Hurt[2] = worldInfo.Engine.Content.Load<SoundEffect>(@"Sounds/yell3");
            Pain[0] = worldInfo.Engine.Content.Load<SoundEffect>(@"Sounds/hurt1");
            Pain[1] = worldInfo.Engine.Content.Load<SoundEffect>(@"Sounds/hurt2");

            Step1 = worldInfo.Engine.Content.Load<SoundEffect>(@"Sounds/step1");
            Step2 = worldInfo.Engine.Content.Load<SoundEffect>(@"Sounds/step2");
            steps = 0;
            CurrentStep = true;

            controllable = true;

            collidingBlocks = new List<Block>();
            collision = new CollisionSolver();
            distanceMeter = new DistanceMeter(worldInfo.Engine.Content);

            debug = false;
            InitCamPhysics();
        }

        #region Draw

        public void DrawPlayerHUD()
        {
            float DistanceFromObjective;
            float DistanceFromWater;

            DistanceFromWater = this.location.Y - this.WorldInfo.WaterPlane.Location.Y;
            DistanceFromObjective = (float)(Math.Sqrt(Math.Pow((WorldInfo.obj.Location.X - this.location.X), 2) + Math.Pow((WorldInfo.obj.Location.Y - this.location.Y), 2) + Math.Pow((WorldInfo.obj.Location.Z - this.location.Z), 2)));

            this.WorldInfo.sb.Begin();

            distanceMeter.Draw(this.WorldInfo.sb, WorldInfo.sf, (int)(DistanceFromObjective / 10), "Objective:", new Vector2(12, 12));
            distanceMeter.Draw(this.WorldInfo.sb, WorldInfo.sf, (int)(DistanceFromWater / 10), "Water:", new Vector2(12, 45));
            //this.WorldInfo.sb.DrawString(WorldInfo.sf, "distance from objective: " + ((int)(DistanceFromObjective / 10)).ToString(), new Vector2(10, -8), Color.White);
            //this.WorldInfo.sb.DrawString(WorldInfo.sf, "distance from water: " + ((int)(DistanceFromWater / 10)).ToString(), new Vector2(10, 30), Color.White);
            //this.WorldInfo.sb.DrawString(WorldInfo.sf, (int)(this.location.X / 10) + ", " + (int)(this.location.Y / 10) + ", " + (int)(this.location.Z / 10), new Vector2(10, 68), Color.White);

            this.WorldInfo.sb.End();
        }

        #endregion

        #region Update

        protected override void Tick(GameTime delta)
        {
            if (this.BoundingBox.Intersects((WorldInfo.obj.GetComponent("Collision") as CubeCollisionComponent).GetBoundingBox()))
            {
                Win.Play();
                controllable = false;
                WorldInfo.PlayerTouchedObjective();
            }

            if (!WorldInfo.IsGameOver)
            {
                this.elapsed += (float)delta.ElapsedGameTime.TotalSeconds;
            }

            #region DEBUG BS

            totalgt += delta.ElapsedGameTime.Milliseconds;
            HandleMouse();

            if (totalgt - lastcollide > 200) colliding = false;
            if (totalgt - lastcollide > 100) Gravity = new Vector3(0, PlayerGravity, 0);

            collision.ResetLeftRight();
            CamPhysics(delta);
            if (debug)
            {
                Console.WriteLine("front: {0} back: {1} left: {2} right: {3}",collision.FrontCollision,collision.BackCollision,collision.LeftCollision,collision.RightCollision);
            }
            #endregion
        }

        #endregion

        #region Input Methods
        /// <summary>
        /// Put all the mouse stuff in one method.
        /// So much nicer. :D
        /// </summary>
        private void HandleMouse()
        {
            float rotationSpeed = .005f;

            MouseState currentState = Mouse.GetState();

            // we compare the old mouse state with the new one, if they are different....
            if (currentState != this.oldMouseState)
            {
                // we subtract them to get their difference
                float diffX = currentState.X - oldMouseState.X;
                float diffY = currentState.Y - oldMouseState.Y;

                // then we update our camera rotation accordingly
                rotator.Y = rotator.Y - rotationSpeed * diffX;
                rotator.X = rotator.X - rotationSpeed * diffY;

                // clamps the up and down pitch so that the camera doesn't because autistic <-- see what i did there
                rotator.X = MathHelper.Clamp(rotator.X, (float)-MathHelper.ToRadians(89), (float)MathHelper.ToRadians(89));

                // then we put out mouse back to the center of the screen
                Mouse.SetPosition(WorldInfo.Viewport.Width / 2, WorldInfo.Viewport.Height / 2);
            }
        }

        private void HandleInput(object sender, InputFiredArgs argument, float delta)
        {
            if (argument.InputType == InputType.Continuous && controllable)
            {
                Vector3 movement = Vector3.Zero;

                if (argument.Command == "MoveForward")
                {
                    movement.Z -= 30;
                    if (collision.TopCollision)
                        steps++;
                }
                else if (argument.Command == "MoveBackward")
                {
                    movement.Z += 30;
                    if (collision.TopCollision)
                        steps++;
                }

                if (argument.Command == "StrafeLeft")
                {
                    movement.X -= 30;
                    if (collision.TopCollision) steps++;
                }
                else if (argument.Command == "StrafeRight")
                {
                    movement.X += 30;
                    if (collision.TopCollision) steps++;
                }

                if (colliding)
                {
                    //location = lastPos;
                }

                if (steps >= MAX_STEP_DISTANCE)
                {
                    if (CurrentStep)
                        Step1.Play();
                    else
                        Step2.Play();

                    CurrentStep = !CurrentStep;
                    steps = 0;
                }

                if (collision.BotCollision)
                {
                    Random rand = new Random();

                    if(rand.Next(2)==1)
                        Hurt[rand.Next(3)].Play();

                    Pain[rand.Next(2)].Play();
                }

                //Matrix camRotation = Matrix.CreateRotationX(this.Rotator.X) * Matrix.CreateRotationY(this.Rotator.Y);
                //Vector3 RotatedVector = Vector3.Transform(movement, camRotation);

                // Fixes the "Camera tries to move in the direction you are looking" problem, so now you can look up while walking without bouncing around like a retard on a sugar rush
                this.location += Vector3.Transform(movement, Matrix.CreateRotationY(rotator.Y)) * delta;//RotatedVector * (delta);
                Console.WriteLine(rotator);
            }
            else if (argument.InputType == InputType.Debounced)
            {
                if (argument.Command == "Pause")
                    WorldInfo.PlayerBelowWater();

                if (argument.Command == "Jump" && controllable && collision.TopCollision)
                {
                    Acceleration.Y = 45f * delta;
                    JumpSound.Play();
                }

                if (argument.Command == "DumbassMode")
                {
                    this.SetPosition(WorldInfo.Objective.Location + new Vector3(10, 20, 0));
                }

                if (argument.Command == "PhysicsDebug")
                {
                    if (debug) debug = false;
                    else debug = true;

                    if (WorldInfo.debug) WorldInfo.debug = false;
                    else WorldInfo.debug = true;

                    if (collision.Debug) collision.Debug = false;
                    else collision.Debug = true;
                }
            }

        }

        #endregion

        #region PlayerMethods

        public void SetPosition(Vector3 position)
        {
            location = position;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the player index for this controller
        /// </summary>
        public PlayerIndex PlayerIndex
        {
            get { return playerIndex; }
            set { playerIndex = value; }
        }

        /// <summary>
        /// Gets or sets whether or not the player is controllable
        /// </summary>
        public bool Controllable
        {
            get { return controllable; }
            set { controllable = value; }
        }


        /// <summary>
        /// Get or set the camera for the player
        /// </summary>
        public Camera Camera
        {
            get { return playerCamera; }
            set { playerCamera = value; }
        }

        public BoundingBox BoundingBox { get { return bbox; } set { bbox = value; } }

        public Matrix View
        { get; set; }

        #endregion

        #region FPSCamMethods

        private void InitCamPhysics()
        {
            if (controllable)
            {
                Gravity = new Vector3(0, -1.25f, 0);
            }

            collision.SetScale(1.5f);
        }

        private void CamPhysics(GameTime delta)
        {
            collision.ResetSolver();
            GravityPass(delta);
            APass();
            BPass();
            CPass();
            SetPhysics();

            collidingBlocks.Clear();
        }

        public void GravityPass(GameTime delta)
        {
            Acceleration = Vector3.Add(Acceleration, Gravity * (float)delta.ElapsedGameTime.TotalSeconds);
            Acceleration = new Vector3(Acceleration.X, MathHelper.Clamp(Acceleration.Y, -2, 2), Acceleration.Z);

            location = Vector3.Add(Acceleration, location);
        }

        public void APass()
        {
            foreach (Block b in collidingBlocks)
            {
                LocationOffset = Vector3.Zero;
                UpdateBoundingBox();
                collision.UpdateFaces(bbox);
                LocationOffset = collision.Solve(b);
                location = Vector3.Add(location, new Vector3(LocationOffset.X, 0, 0));
            }
        }

        public void BPass()
        {
            foreach (Block b in collidingBlocks)
            {
                LocationOffset = Vector3.Zero;
                UpdateBoundingBox();
                collision.UpdateFaces(bbox);
                LocationOffset = collision.Solve(b);
                location = Vector3.Add(location, new Vector3(0, 0, LocationOffset.Z));
            }
        }

        public void CPass()
        {
            UpdateBoundingBox();
            LocationOffset = Vector3.Zero;
            collision.ResetCollisions();
            collision.TopCollision = false;
            collision.UpdateFaces(bbox);
            collision.ResetSolver();

            foreach (Block b in collidingBlocks)
            {
                float xdist = location.X - b.Location.X;
                float zdist = location.Z - b.Location.Z;

                if (xdist < 0) xdist *= -1;
                if (zdist < 0) zdist *= -1;

                xdist = (float)Math.Round(xdist, 4);
                zdist = (float)Math.Round(zdist, 4);

                float mindistance = InstancedBoxRenderer.SIZE / 2 + collision.Distance / 2;

                mindistance = (float)Math.Round(mindistance, 4);

                if ((xdist < mindistance && zdist < mindistance))
                    LocationOffset = Vector3.Add(LocationOffset, collision.Solve(b));
            }

            collision.NormalizeCollisions();

            int sideCollisions = collision.RightCollisions * collision.LeftCollisions;
            int frontCollisions = collision.FrontCollisions * collision.BackCollisions;

            location = Vector3.Add(location, new Vector3(0, LocationOffset.Y / collision.TopCollisions, 0));
        }

        public void UpdateBoundingBox()
        {
            float distance = collision.Distance;
            bbox.Min = Vector3.Subtract(location, new Vector3(distance / 2));
            bbox.Max = Vector3.Subtract(location, new Vector3(-distance / 2));
        }

        public void SetPhysics()
        {
            if (collision.TopCollision) Acceleration = Vector3.Zero;
            if (collision.BotCollision) Acceleration = Vector3.Zero;
        }

        #endregion

        #region CamCollisions

        /// <summary>
        /// Oh hey look a new CameraCollision Class.
        /// It's cool though, I don't deserve any credit for getting it to work lol
        /// 
        /// Complaint Counter:
        /// Times Someone has Complained about Collision Detection: 115
        /// Times I've actually cared because someone complained: 1
        /// Times I've decided NOT to work on collision detection because someone complained: 115
        /// Hours spent making this "piece of shit" engine work: 22
        /// Hours spent re-writing this "piece of shit" engine: 6
        /// Times I've complained about other peoples code not working quite as well so I can do things: 0
        /// Times I've added onto their code to MAKE things work for me: 100+
        /// 
        /// Credit given: 0 because your shit doesn't work you cock gobbling queerfag anal blazer! :)
        /// 
        /// Doesn't work as well as YOU'D like it, but at least there's progress right? 
        /// 
        /// Bugs Killed:
        /// 
        /// Bouncing
        /// Wall Bouncing
        /// Multiple Jumping
        /// Wall Jumping
        /// Wall Sticking <- KILLED! ->
        /// Ceiling Sticking
        /// 
        /// Amount of frustration being relieved by complaining about the complainers: ALL
        /// </summary>
        /// <param name="b">It's the lonely block who's wife divorced him and took the kids</param>
        /// <param name="elapsedGt">It's how long it's been since he's seen his kids.</param>
        public void CameraCollision(Block b)
        {
            collidingBlocks.Add(b);
            lastcollide = totalgt;
            colliding = true;
        }

        #endregion
        float elapsed = 0;

        public float GetTime()
        {
            return this.elapsed;
        }
    }
}
