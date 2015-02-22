using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RLH.Components;
using Microsoft.Xna.Framework;

namespace BlockRLH.Actors
{
    public class Camera : Actor
    {
        /// <summary>
        /// Near clip distance for cameras
        /// </summary>
        const float NEAR_CLIP = .2f;

        /// <summary>
        /// Far clip distance for cameras
        /// </summary>
        const int FAR_CLIP = 700;

        private Actor target;
        private Vector3 offset;
        public Vector3 CameraTarget { get; private set; }

        private Matrix view;
        private Matrix projection;

        private float fov;

        public Camera(WorldInfo worldInfo, Actor target, float fieldOfView)
            : base(worldInfo)
        {
            if (target == null)
                throw new ArgumentNullException("target", "null not an acceptable value for parameter target");
            this.target = target;
            fov = fieldOfView;
            this.offset = Vector3.Zero;
            this.projection = Matrix.CreatePerspectiveFieldOfView(fov, worldInfo.Viewport.AspectRatio, NEAR_CLIP, FAR_CLIP);
        }

        protected override void Tick(GameTime delta) { UpdateCameraPosition(); }

        /// <summary>
        /// Updates the camera position, use this to offset from target
        /// </summary>
        protected virtual void UpdateCameraPosition()
        {
            this.location = Vector3.Add(target.Location, offset);
            this.rotator = target.Rotator;

            Matrix newCameraRotation = Matrix.CreateRotationX(this.rotator.X) * Matrix.CreateRotationY(this.rotator.Y);

            Vector3 cameraRotatedTarget = Vector3.Transform(Vector3.Forward, newCameraRotation);
            Vector3 cameraFinalTarget = this.location + cameraRotatedTarget;

            //Vector3 cameraRotatedUpVector = Vector3.Transform(Vector3.Up, newCameraRotation);
            //Vector3 cameraFinalUpVector = this.location + cameraRotatedUpVector;
            CameraTarget = cameraFinalTarget;
            this.view = Matrix.CreateLookAt(this.location, cameraFinalTarget, Vector3.Up);
        }

        /// <summary>
        /// Get the bounding frustum for this camera
        /// </summary>
        public BoundingFrustum BoundingView
        { get { return new BoundingFrustum(this.view * Projection); } }

        /// <summary>
        /// Get or set the camera's view target
        /// </summary>
        public Actor Target
        {
            get { return target; }
            set { if (value != null) target = value; }
        }

        /// <summary>
        /// Camera offsets
        /// </summary>
        public Vector3 Offset
        {
            get { return offset; }
            set { offset = value; }
        }

        /// <summary>
        /// Get the camera view point
        /// </summary>
        public Matrix View
        { get { return this.view; } set { this.view = value; } }

        /// <summary>
        /// Get the camera's projection
        /// </summary>
        public Matrix Projection
        { get { return this.projection; } }
    }
}
