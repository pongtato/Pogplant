﻿/******************************************************************************/
/*!
\file	FollowSpline.cs
\author Ng Tian Kiat
\par	email: tiankiat.ng\@digipen.edu
\details
	This script moves a gameobject along a series of waypoints generated by a spline,
    script (Catmullrom).

\copyright	Copyright (c) 2021 DigiPen Institute of Technology. Reproduction
			or disclosure of this file or its contents without the prior
			written consent of DigiPen Institute of Technology is prohibited.
*/
/******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    public class FollowSpline : MonoBehaviour
    {
        private CatmullRomSpline catmullRom = null;

        private float follow_speed = 9.0f;

        public float DelayToStart = 0.0f;

        private Transform[] waypoints = null; // Array of waypoints we get from running the CatmullRomSpline script.
        private uint current_waypoint_index = 1; // Initiallized to 2 so that skip the first 3 waypoints in waypoints array.
        private float alpha = 0.0f; // Clamped between [0,1], this value is used to interpolate between 2 waypoints.
        private float d_alpha = 0.0f; // This is the rate of change of alpha, determined by the distance between the 2 waypoints and the follow_speed (Essentially the time it takes to go between 2 waypoints)
        private bool isEnd = false; //  This is true when we have arrived a the end of the path
        private float time_between_waypoint = 0.0f;

        private int waypoint_offset = 2;

        private bool isInit = false;
        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;

            if (ECS.GetTagECS(entityID) == "Player")
                DelayToStart = 0.8f;
            else if (ECS.GetTagECS(entityID) == "WPG_0")
                 DelayToStart = 0.0f;
            else if (ECS.GetTagECS(entityID) == "WPG_1")
                 DelayToStart = 0.2f;
            else if (ECS.GetTagECS(entityID) == "WPG_2")
                 DelayToStart = 0.4f;
            else if (ECS.GetTagECS(entityID) == "WPG_3")
                DelayToStart = 0.6f;
            else if (ECS.GetTagECS(entityID) == "WPG_4")
                DelayToStart = 1.2f;
        }

        // Start is called before the first frame update
        public override void Start()
        {
            // Initialize waypoint array

            catmullRom = new CatmullRomSpline();
            catmullRom.InitializeSpline();

            waypoints = catmullRom.CalculateCatmullRomChain().ToArray();

            // Initialize d_alpha for first waypoint
            // Calculate d_alpha from follow_speed and distance between current and next waypoint
            float distance = Vector3.Distance(waypoints[current_waypoint_index - 1].Position, waypoints[current_waypoint_index].Position);
            d_alpha = 1.0f / (distance / follow_speed);

            //Console.WriteLine("alpha is" + d_alpha);

            // Initialize starting position and rotation of the play area
        }

        // Update is called once per frame
        public override void Update(ref Transform transform, ref Rigidbody rigidbody, ref float dt)
        {
            if (!isInit)
            {
                // this is to initialize the starting position and rotation to te start of the spline
                isInit = true;
                transform.Position = waypoints[0].Position;
                transform.Rotation = waypoints[0].Rotation;
            }

            if ((DelayToStart -= dt) <= 0.0f)
                FollowWaypoints(ref transform, ref dt);

            //if (ECS.GetTagECS(entityID) == "Player")
            //    catmullRom.DisplayCatmullRomSplineChain();
        }

        public override void LateUpdate(ref Transform transform, ref Rigidbody rigidbody, ref float dt)
        {

        }

        public void UpdateSpeed(float speed)
        {
            follow_speed = speed;
        }

        // This function interpolates the gameobjects transform between the current and next waypoint.
        private void FollowWaypoints(ref Transform transform, ref float dt)
        {
            // Only move if we are not at the end of path.
            if (!isEnd)
            {

                float translation_lerpSpeed = 10.0f;

                UpdateCurrentWaypoint(alpha);
                time_between_waypoint += dt;
                alpha = d_alpha * time_between_waypoint; // Calculate the current alpha between the 2 waypoints
                transform.Position += (Vector3.Lerp(waypoints[current_waypoint_index - 1].Position, waypoints[current_waypoint_index].Position, alpha) - transform.Position) * translation_lerpSpeed * dt;


                float rotation_lerp_speed = 0.3f;
                //original
                //transform.Rotation += (Vector3.RotateTowards(waypoints[current_waypoint_index - 1].Rotation, waypoints[current_waypoint_index].Rotation, alpha) - transform.Rotation) * rotation_lerp_speed * dt;

                //Replacement rotation code
                //transform.Rotation += (Vector3.GetRotationFromVector(waypoints[current_waypoint_index + 5].Position - ECS.GetGlobalPosition(entityID)) - transform.Rotation) * rotation_lerp_speed * dt;
                //transform.Rotation = (Vector3.GetRotationFromVector(waypoints[current_waypoint_index + 1].Position - ECS.GetGlobalPosition(entityID))) * rotation_lerp_speed * dt;

                Vector3 forward = Transform.GetForwardVector(entityID);
                Vector3 look_direction = waypoints[current_waypoint_index + waypoint_offset].Position - ECS.GetGlobalPosition(entityID);
                Vector3 look_vector = look_direction - forward;
                float interpolant = rotation_lerp_speed * dt; ;
                Vector3 look_point = ECS.GetGlobalPosition(entityID) + (forward + (look_vector * interpolant));

                Transform.LookAtClamped(entityID, look_point);
            }
        }
        void UpdateCurrentWaypoint(float alpha)
        {
            int end_offset = 20;// waypoint_offset + 15;
            if (alpha >= 1.0)
            {
                // Calculate d_alpha from follow_speed and distance between current and next waypoint
                float distance = Vector3.Distance(waypoints[current_waypoint_index].Position, waypoints[++current_waypoint_index].Position);
                d_alpha = 1.0f / (distance / follow_speed);

                time_between_waypoint = 0.0f; // Reset current time between waypoints
            }

            //if (current_waypoint_index >= 700)
            //    Console.WriteLine("Current waypoint " + current_waypoint_index + " / " + waypoints.Length);

            if (current_waypoint_index >= waypoints.Length - end_offset - 1)
                isEnd = true;
        }

        public void SetLockRotation (bool isLock)
        {
            //lockRotation = isLock;
        }

        public void LateUpdate(ref Transform transform, ref Rigidbody rigidbody)
        {
        }

        public override void OnTriggerEnter(uint id)
        {

        }
        public override void OnTriggerExit(uint id)
        {

        }
    }
}
